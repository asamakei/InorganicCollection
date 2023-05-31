using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MonsterBank : MonoBehaviour{

    public MonsterStates monDB;
    public AllyDatas allDB;
    public ItemDatas itemDB;
    public CharaDatas charDB;
    public Sprite emptyBox;
    public AudioClip[] SE;
    int buttonNum;
    int mode = 0;
    int pageNow=0,pageMax;
    int selectMon = -1,selectPre;
    GameObject partyWin,boxWin,statesWin;
    GameController gameCon;
    Image iconObj;
    Text[] statesObj;
    GameObject[] Button;
    Rect[] buttonRe;
    AudioSource audioS;
    AllyData preAlly;


    void Start(){//初期設定

        audioS = GetComponent<AudioSource>();
        Button = new GameObject[40];
        buttonRe = new Rect[40];
        statesObj = new Text[10];

        statesWin = GameObject.Find("StatesWindow");
        partyWin = GameObject.Find("PartyWindow");
        boxWin = GameObject.Find("BoxWindow");
        if(!SysDB.netFlag)gameCon = GameObject.Find("GameController").GetComponent<GameController>();

        statesObj[0] = statesWin.transform.Find("Name").gameObject.GetComponent<Text>();
        statesObj[1] = statesWin.transform.Find("StatesValue").gameObject.GetComponent<Text>();
        statesObj[2] = statesWin.transform.Find("ExValue").gameObject.GetComponent<Text>();
        statesObj[3] = statesWin.transform.Find("Equip").gameObject.GetComponent<Text>();
        statesObj[4] = statesWin.transform.Find("charText").gameObject.GetComponent<Text>();
        statesObj[5] = statesWin.transform.Find("charLevel").gameObject.GetComponent<Text>();
        statesObj[6] = statesWin.transform.Find("level").transform.Find("Text").GetComponent<Text>();
        statesObj[7] = statesWin.transform.Find("HP").transform.Find("Text").GetComponent<Text>();
        statesObj[8] = statesWin.transform.Find("SP").transform.Find("Text").GetComponent<Text>();
        iconObj = statesWin.transform.Find("icon").gameObject.GetComponent<Image>();
        for (int i = 0; i < 4; i++)Button[i] = partyWin.transform.Find("PartyMonster" + (i + 1).ToString()).gameObject;
        for (int i = 4; i < 24; i++)Button[i] = boxWin.transform.Find("MonsterButton" + (i - 3).ToString()).gameObject;
        Button[24] = GameObject.Find("BackButton");
        Button[25] = boxWin.transform.Find("UpButton").gameObject;
        Button[26] = boxWin.transform.Find("DownButton").gameObject;
        for (int i = 0; i < 27; i++) buttonRe[i] = buttonRect(Button[i]);
        for(int i = 0; i < allDB.AllyDataList.Count; i++) {
            if (allDB.AllyDataList[i].ID >= 1) pageMax = i-4;
        }
        pageMax = (pageMax - pageMax % 20) / 20+1;
        if (pageMax > 10) pageMax = 10;
        partyUpdate();
        boxUpdate(0);
        states(-1);
        StartCoroutine(main());
    }
    IEnumerator main() {
        yield return new WaitForSeconds(10/60f);
        SysDB.eventFlag = true;
        SysDB.menueFlag = true;
        while (true) {
            buttonNum = -1;
            while (mode != 0 || buttonNum == -1) yield return null;
            if (buttonNum == 24) {//閉じる
                if(myDB.party[0]+ myDB.party[1]+ myDB.party[2]+ myDB.party[3] == -4) {
                    playSE(3,0);
                    continue;
                }

                int lastEmpty=-1;
                for(int i = 4; i < allDB.AllyDataList.Count; i++) {
                    if(allDB.AllyDataList[i].ID<=0 && lastEmpty==-1) lastEmpty = i;
                    else if(allDB.AllyDataList[i].ID >= 1 && lastEmpty >= 4){
                        allDB.AllyDataList[lastEmpty] = allDB.AllyDataList[i];
                        lastEmpty++;
                    }
                }
                for(int i = lastEmpty; i < allDB.AllyDataList.Count; i++) {
                    allDB.AllyDataList[i] = new AllyData();
                    allDB.AllyDataList[i].Name = "";
                    allDB.AllyDataList[i].ID = -1;
                }

                SysDB.eventFlag = false;
                SysDB.menueFlag = false;
                if (!SysDB.netFlag) {
                    gameCon.SE(3);
                    SceneManager.SetActiveScene(SceneManager.GetSceneByName(SysDB.sceneName));
                }
                SysDB.netFlag = false;
                SceneManager.UnloadSceneAsync("Scenes/MonsterBank");
                yield break;
            } else if (buttonNum == 25) {//↑ボタン
                playSE(1, 0);
                if (pageNow == 0) pageNow = pageMax;
                else pageNow--;
                boxUpdate(pageNow);
            } else if (buttonNum == 26) {//↓ボタン
                playSE(1, 0);
                if (pageNow == pageMax) pageNow = 0;
                else pageNow++;
                boxUpdate(pageNow);
            } else if (buttonNum < 24) {//モンスター
                if (selectMon >= 0) {
                    playSE(2, 0);
                    selectPre = buttonNum;
                    if (buttonNum >= 4)selectPre += pageNow * 20;

                    if (selectPre < 4) {
                        myDB.party[selectPre] = selectPre;
                        if (allDB.AllyDataList[selectMon].ID == -1) myDB.party[selectPre] = -1;
                    }
                    if (selectMon < 4) {
                        myDB.party[selectMon] = selectMon;
                        if (allDB.AllyDataList[selectPre].ID == -1) myDB.party[selectMon] = -1;
                    }

                    preAlly = allDB.AllyDataList[selectMon];
                    allDB.AllyDataList[selectMon] = allDB.AllyDataList[selectPre];
                    allDB.AllyDataList[selectPre] = preAlly;

                    selectMon = -1;
                    partyUpdate();
                    boxUpdate(pageNow);
                } else {
                    selectPre = buttonNum;
                    if (buttonNum >= 4) selectPre += pageNow * 20;
                    if (allDB.AllyDataList[selectPre].ID >= 0) {
                        playSE(2, 0);
                        Button[buttonNum].GetComponent<Image>().color = new Color(0.5f, 0.5f, 1, 1);
                        selectMon = selectPre;
                        states(selectMon);
                    }
                }
            }
        }
    }
    void partyUpdate() {//パーティ画像更新
        for (int i = 0; i < 4; i++) {
            if (myDB.party[i] == -1) {
                Button[i].GetComponent<Image>().sprite = emptyBox;
            } else {
                Button[i].GetComponent<Image>().sprite
                    = monDB.MonsterDataList[allDB.AllyDataList[myDB.party[i]].ID].Icon;
            }
            if (selectMon == i) Button[i].GetComponent<Image>().color = new Color(0.5f, 0.56f, 1, 1);
            else Button[i].GetComponent<Image>().color = new Color(1, 1, 1, 1);

        }
    }
    void boxUpdate(int page) {//ボックス画像更新
        for (int i = page*20+4; i < (page+1)*20+4; i++) {
            if (allDB.AllyDataList[i].ID == -1) {
                Button[(i-4) % 20 + 4].GetComponent<Image>().sprite = emptyBox;
            } else {
                Button[(i-4) % 20 + 4].GetComponent<Image>().sprite
                    = monDB.MonsterDataList[allDB.AllyDataList[i].ID].Icon;
            }
            if(selectMon == i) Button[(i - 4) % 20 + 4].GetComponent<Image>().color = new Color(0.5f, 0.56f, 1, 1);
            else Button[(i - 4) % 20 + 4].GetComponent<Image>().color = new Color(1, 1, 1, 1);
        }
    }
    void states(int num) {
        int charaNum;
        string pre1 = "",pre2 = "";
        string[] rome = { "零", "Ⅰ", "Ⅱ", "Ⅲ", "Ⅳ", "Ⅴ" };
        if (num>=0 && allDB.AllyDataList[num].ID >= 0) {
            statesObj[0].text = allDB.AllyDataList[num].Name;
            statesObj[1].text = allDB.AllyDataList[num].Attack.ToString() + "\n"
                                + allDB.AllyDataList[num].Defence.ToString() + "\n"
                                + allDB.AllyDataList[num].Magic.ToString() + "\n"
                                + allDB.AllyDataList[num].MagicDef.ToString() + "\n"
                                + allDB.AllyDataList[num].Speed.ToString() + "\n";
            statesObj[2].text = allDB.AllyDataList[num].Exp.ToString() + "\n"
                                + allDB.AllyDataList[num].NextExp.ToString() + "\n"
                                + allDB.AllyDataList[num].CharaPt.ToString();
            statesObj[3].text = "";
            for (int i = 0; i < monDB.MonsterDataList[allDB.AllyDataList[num].ID].Character.Length; i++) {
                charaNum = monDB.MonsterDataList[allDB.AllyDataList[num].ID].Character[i];
                if (allDB.AllyDataList[num].Character[i] >= 0) {
                    pre1 += charDB.CharaDataList[charaNum].Name + "\n";
                    pre2 += rome[allDB.AllyDataList[num].Character[i]] + "\n";
                }
            }
            statesObj[4].text = pre1;
            statesObj[5].text = pre2;
            statesObj[6].text = allDB.AllyDataList[num].Level.ToString();
            statesObj[7].text = allDB.AllyDataList[num].HP.ToString();
            statesObj[8].text = allDB.AllyDataList[num].SP.ToString();
            iconObj.sprite = monDB.MonsterDataList[allDB.AllyDataList[num].ID].Icon;
        } else {
            statesObj[0].text = "----------";
            statesObj[1].text = "---\n---\n---\n---\n---";
            statesObj[2].text = "---\n---\n---";
            statesObj[3].text = "";
            statesObj[4].text = "";
            statesObj[5].text = "";
            statesObj[6].text = "---";
            statesObj[7].text = "---";
            statesObj[8].text = "---";
            iconObj.sprite = emptyBox;
        }
    }
    Rect buttonRect(GameObject obj) {//矩形
        RectTransform rect = obj.GetComponent<RectTransform>();
        float wid, hei;
        wid = rect.rect.width * obj.transform.lossyScale.x;
        hei = rect.rect.height * obj.transform.lossyScale.y;
        return new Rect(rect.position.x - wid / 2, rect.position.y - hei / 2, wid, hei);
    }
    void playSE(int i, int delay) { StartCoroutine(delaySE(i, delay)); }
    IEnumerator delaySE(int i, int delay) {//SE再生
        yield return new WaitForSeconds(delay / 60f);
        audioS.PlayOneShot(SE[i], 2);
    }
    void Update() {//ボタンタップ判定
        Vector3 mousePos;
        if (mode == 0 && (Input.GetMouseButtonUp(0) || Input.GetMouseButton(0))) {//メインメニュー
            mousePos = Input.mousePosition;
            for (int i = 24; i < 27; i++) {
                if (Input.GetMouseButtonUp(0)) {
                    if (buttonRe[i].Contains(mousePos)) buttonNum = i;
                    Button[i].GetComponent<Image>().color = new Color(0.5f, 0.56f, 1, 1);
                } else if (Input.GetMouseButton(0)) {
                    if (buttonRe[i].Contains(mousePos)) {
                        Button[i].GetComponent<Image>().color = new Color(1, 1, 1, 1);
                    } else if (Button[i] != null) Button[i].GetComponent<Image>().color = new Color(0.5f, 0.56f, 1, 1);
                }
            }
            for (int i = 0; i < 24; i++) {
                if (Input.GetMouseButtonUp(0)) {
                    if (buttonRe[i].Contains(mousePos))buttonNum = i;
                }else if (Input.GetMouseButton(0)) {
                    if (buttonRe[i].Contains(mousePos) && selectMon <0) {
                        if (i < 4) states(i);
                        else states(i + pageNow * 20);
                    }
                }
            }
        }
    }
}
