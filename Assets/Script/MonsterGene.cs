using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MonsterGene : MonoBehaviour {

    public MonsterStates monDB;
    public AllyDatas allDB;
    public ItemDatas itemDB;
    public CharaDatas charDB;
    public GeneDatas geneDB;
    public FlagDatas flagDB;
    public SkillDatas skillDB;
    public Sprite emptyBox;
    public GameObject effect;
    public AudioClip[] SE;
    public Sprite[] itemSprite;
    int numGene;
    Vector3 scaleItemGene;
    Vector3 scaleGene;
    bool skip=false,messageFlag=false,tutorial;
    int buttonNum,itemNum=-1;
    int mode = 0;
    int pageNow = 0, pageMax;
    int selectMon = -1, selectPre,itemButtonNum,checkButtonNum, skillButtonNum;
    int TutorialNum=0;
    int empty1, empty2,prev;
    GameObject gameCon,itemCursor,itemDetail,skillCursor,skillDetail;
    GameObject partyWin, boxWin, statesWin,resultWin,itemWin,checkWin,geneWin,messageWin,mWin,skillWin,falseWin;
    Text M_text, M_name;
    Image iconObj;
    Text[] statesObj;
    GameObject[] Button,geneMon;
    Rect[] buttonRe;
    AudioSource audioS;
    AllyData preAlly;
    AllyData[] monster;
    AllyData[] allyDB;
    ItemData[] item = new ItemData[15];
    SkillData[] skill = new SkillData[10];
    IEnumerator skipGene;
    List<int> resultList;

    void Start() {//初期設定
        monster = new AllyData[8];
        for (int i = 0; i < 8; i++) {
            monster[i] = new AllyData();
            monster[i].ID = -1;
        }
        allyDB = new AllyData[allDB.AllyDataList.Count];
        for (int i = 0; i < allDB.AllyDataList.Count; i++) {
            allyDB[i] = allDB.AllyDataList[i];
            allyDB[i].preNumber = i;
        }
        audioS = GetComponent<AudioSource>();
        Button = new GameObject[58];
        buttonRe = new Rect[58];
        statesObj = new Text[10];
        geneMon = new GameObject[5];

        messageWin = GameObject.Find("MessageWindow");
        checkWin = GameObject.Find("CheckWindow");
        geneWin = GameObject.Find("GeneWindow");
        statesWin = GameObject.Find("StatesWindow");
        gameCon = GameObject.Find("GameController");
        partyWin = GameObject.Find("PartyWindow");
        resultWin = GameObject.Find("ResultWindow");
        boxWin = GameObject.Find("BoxWindow");
        itemWin = GameObject.Find("ItemWindow");
        mWin = GameObject.Find("MessageWinG");
        skillWin = GameObject.Find("SkillWindow");
        falseWin = GameObject.Find("FalseWindow");
        M_text = mWin.transform.Find("Text").GetComponent<Text>();
        M_name = mWin.transform.Find("Name").GetComponent<Text>();

        itemCursor = itemWin.transform.Find("Cursor").gameObject;
        itemDetail = itemWin.transform.Find("detail").transform.Find("textDetail").gameObject;
        skillCursor = skillWin.transform.Find("Cursor").gameObject;
        skillDetail = skillWin.transform.Find("detail").transform.Find("textDetail").gameObject;

        for (int i = 0; i < 4; i++) geneMon[i] = geneWin.transform.Find("Monster" + (i + 1).ToString()).gameObject;
        geneMon[4] = geneWin.transform.Find("item").gameObject;

        statesObj[0] = statesWin.transform.Find("Name").gameObject.GetComponent<Text>();
        statesObj[1] = statesWin.transform.Find("StatesValue").gameObject.GetComponent<Text>();
        statesObj[2] = statesWin.transform.Find("skillList1").gameObject.GetComponent<Text>();
        statesObj[3] = statesWin.transform.Find("skillList2").gameObject.GetComponent<Text>();
        statesObj[4] = statesWin.transform.Find("charText").gameObject.GetComponent<Text>();
        statesObj[5] = statesWin.transform.Find("charLevel").gameObject.GetComponent<Text>();
        statesObj[6] = statesWin.transform.Find("level").transform.Find("Text").GetComponent<Text>();
        statesObj[7] = statesWin.transform.Find("HP").transform.Find("Text").GetComponent<Text>();
        statesObj[8] = statesWin.transform.Find("SP").transform.Find("Text").GetComponent<Text>();
        iconObj = statesWin.transform.Find("icon").gameObject.GetComponent<Image>();
        for (int i = 0; i < 4; i++) Button[i] = partyWin.transform.Find("PartyMonster" + (i + 1).ToString()).gameObject;
        for (int i = 4; i < 24; i++) Button[i] = boxWin.transform.Find("MonsterButton" + (i - 3).ToString()).gameObject;
        Button[24] = GameObject.Find("BackButton");
        Button[25] = boxWin.transform.Find("UpButton").gameObject;
        Button[26] = boxWin.transform.Find("DownButton").gameObject;
        Button[27] = partyWin.transform.Find("ItemButton").gameObject;
        for (int i = 28; i < 32; i++) Button[i] = resultWin.transform.Find("ResultMonster" + (i - 27).ToString()).gameObject;
        for (int i = 32; i < 43; i++) Button[i] = itemWin.transform.Find("Item" + (i - 32).ToString()).gameObject;
        for (int i = 43; i < 45; i++) Button[i] = checkWin.transform.Find("button" + (i - 42).ToString()).gameObject;
        for (int i = 45; i < 58; i++) Button[i] = skillWin.transform.Find("Skill"+(i-45).ToString()).gameObject;
        for (int i = 0; i < 58; i++) buttonRe[i] = buttonRect(Button[i]);
        for (int i = 0; i < allyDB.Length; i++) {
            if (allyDB[i].ID >= 1) pageMax = i - 4;
        }

        tutorial = flagDB.Event[7] == 0;
        if (tutorial && TutorialNum == 0) {
            for(int i = 4; i < allDB.AllyDataList.Count; i++) {
                if (allDB.AllyDataList[i].Name == ""|| allDB.AllyDataList[i].Name == "Nothing" || allDB.AllyDataList[i].ID<=0) {
                    empty1 = i;
                    gameCon.GetComponent<GameController>().setAlly(i,1,5);
                    Array.Resize(ref allDB.AllyDataList[i].Skill, 2);
                    Array.Resize(ref allDB.AllyDataList[i].SkillUse, 2);
                    break;
                }
            }
            for (int i = empty1+1; i < allDB.AllyDataList.Count; i++) {
                if (allDB.AllyDataList[i].Name == ""|| allDB.AllyDataList[i].Name == "Nothing" || allDB.AllyDataList[i].ID <= 0) {
                    empty2 = i;
                    gameCon.GetComponent<GameController>().setAlly(i, 8, 5);
                    Array.Resize(ref allDB.AllyDataList[i].Skill, 2);
                    Array.Resize(ref allDB.AllyDataList[i].SkillUse, 2);
                    break;
                }
            }
        }

        itemWin.SetActive(false);
        checkWin.SetActive(false);
        geneWin.SetActive(false);
        messageWin.SetActive(false);
        mWin.SetActive(false);
        skillWin.SetActive(false);
        falseWin.SetActive(false);

        pageMax = (pageMax - pageMax % 20) / 20 + 1;
        partyUpdate();
        boxUpdate(0);
        states(-1);
        StartCoroutine(main());
    }
    IEnumerator main() {
        yield return new WaitForSeconds(10 / 60f);
        SysDB.eventFlag = true;
        SysDB.menueFlag = true;
        if (tutorial && TutorialNum == 0)yield return StartCoroutine(messageShow("ジ=トリ博士", "これが合成画面だ。\nとりあえず「水」と「二酸化炭素」をやる、\n画面下の右の枠から「水」を選んでみろ。"));
        while (true) {
            if (tutorial && TutorialNum == 1) yield return StartCoroutine(messageShow("ジ=トリ博士", "今選んだ「水」を、\n左側の素材の枠に入れてみろ。"));
            if (tutorial && TutorialNum == 2) { yield return StartCoroutine(messageShow("ジ=トリ博士", "次は、一番下の\n「二酸化炭素」を選んでみろ。")); TutorialNum = 3; }
            if (tutorial && TutorialNum == 4) yield return StartCoroutine(messageShow("ジ=トリ博士", "今選んだ「二酸化炭素」を、\n左側の素材の枠に入れてみろ。"));
            if (tutorial && TutorialNum == 5) { yield return StartCoroutine(messageShow("ジ=トリ博士", "さあ、最後に「結果」にある\n新しいモンスターを選んで合成だ。")); TutorialNum = 6; }

            buttonNum = -1;
            while (mode != 0 || buttonNum == -1) yield return null;
            if(tutorial) {
                //if (buttonNum == 7) continue;
                if (buttonNum != 25 && buttonNum != 26 && buttonNum + pageNow * 20 - 4 != empty1 && TutorialNum == 0) {
                    yield return StartCoroutine(messageShow("ジ=トリ博士", "画面下の右の枠から\n一番下にある「水」を選んでみろ。"));
                    continue;
                }else if ((buttonNum<0 || buttonNum>=4) && TutorialNum == 1) {
                    yield return StartCoroutine(messageShow("ジ=トリ博士", "今選んだ「水」を、\n左側の素材の枠に入れてみろ。"));
                    continue;
                }else if (buttonNum != 25 && buttonNum != 26 && buttonNum + pageNow * 20 - 4 != empty2 && TutorialNum == 3) {
                    yield return StartCoroutine(messageShow("ジ=トリ博士", "画面下の右の枠から\n一番下にある「二酸化炭素」を選んでみろ。"));
                    continue;
                } else if ((buttonNum < 0 || buttonNum >= 4 || buttonNum == prev) && TutorialNum == 4) {
                    yield return StartCoroutine(messageShow("ジ=トリ博士", "今選んだ「二酸化炭素」を、\n左側の素材の枠に入れてみろ。"));
                    continue;
                } else if (buttonNum!=28 && TutorialNum == 6) {
                    yield return StartCoroutine(messageShow("ジ=トリ博士", "「結果」にある新しいモンスターを\n選んで合成開始だ。"));
                    continue;
                }
            }
            if (buttonNum == 24) {//閉じる
                gameCon.GetComponent<GameController>().SE(3);
                SysDB.eventFlag = false;
                SysDB.menueFlag = false;
                SceneManager.SetActiveScene(SceneManager.GetSceneByName(SysDB.sceneName));
                SceneManager.UnloadSceneAsync("Scenes/MonsterGene");
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
            } else if (buttonNum == 27) {//アイテム
                playSE(4, 0);
                mode = 1;
                itemWin.SetActive(true);
                StartCoroutine(itemSelect());
            } else if (buttonNum < 24) {//モンスター
                if (tutorial && TutorialNum == 4) TutorialNum = 5;
                if (tutorial && TutorialNum == 3) TutorialNum = 4;
                if (tutorial && TutorialNum == 1) { TutorialNum = 2; prev = buttonNum; }
                if (tutorial && TutorialNum == 0) TutorialNum = 1;
                if (selectMon >= 0) {
                    playSE(2, 0);
                    selectPre = buttonNum;
                    if (buttonNum >= 4) selectPre += pageNow * 20;

                    if (selectPre >= 4) {//ボックスへ
                        if (selectMon >= 4) {//ボックスから
                            preAlly = allyDB[selectMon-4];
                            allyDB[selectMon-4] = allyDB[selectPre-4];
                            allyDB[selectPre-4] = preAlly;
                        } else {//素材から
                            preAlly = monster[selectMon];
                            monster[selectMon] = allyDB[selectPre - 4];
                            allyDB[selectPre - 4] = preAlly;
                        }

                    }else{//素材へ
                        if (selectMon >= 4) {//ボックスから
                            preAlly = allyDB[selectMon - 4];
                            allyDB[selectMon - 4] = monster[selectPre];
                            monster[selectPre] = preAlly;
                        } else {//素材から
                            preAlly = monster[selectMon];
                            monster[selectMon] = monster[selectPre];
                            monster[selectPre] = preAlly;
                        }
                    }
                    selectMon = -1;
                    partyUpdate();
                    boxUpdate(pageNow);
                } else {   
                    selectPre = buttonNum;
                    if (buttonNum >= 4) {
                        selectPre += pageNow * 20;
                        preAlly = allyDB[selectPre - 4];
                    } else preAlly = monster[buttonNum];
                    if (preAlly != null && preAlly.ID >= 0) {
                        playSE(2, 0);
                        Button[buttonNum].GetComponent<Image>().color = new Color(0.5f, 0.5f, 1, 1);
                        selectMon = selectPre;
                        states(selectMon);
                    }
                }
            }else if (buttonNum < 32) {//結果モンスター
                
                if (monster[buttonNum - 24].ID >= 1) {

                    bool countFlag = false;
                    for(int i = 0; i < 4; i++) {
                        if (monster[i].ID > 0 && monster[i].Level < 5) countFlag =true;
                    }
                    if (countFlag) {
                        playSE(3, 0);
                        falseWin.SetActive(true);
                        while (Input.GetKeyDown(KeyCode.Z) || Input.GetMouseButton(0)) yield return null;
                        while (!Input.GetKeyDown(KeyCode.Z) && !Input.GetMouseButton(0)) yield return null;
                        while (Input.GetKeyDown(KeyCode.Z) || Input.GetMouseButton(0)) yield return null;
                        falseWin.SetActive(false);
                    } else {
                        
                        playSE(4, 0);
                        bool[] skillFlag = new bool[skillDB.SkillDataList.Count];
                        List<int> skillList = new List<int>();
                        for (int i = 0; i < 4; i++) {
                            AllyData monPre = monster[i];
                            if (monPre.ID <= 0) continue;
                            for (int j = 2; j < monPre.Skill.Length; j++) skillFlag[monPre.Skill[j]] = true;
                        }
                        for (int i = 2; i < skillFlag.Length; i++) {
                            if (skillFlag[i] && !skillDB.SkillDataList[i].Chemi) skillList.Add(i);
                        }
                        if (skillList.Count > 0) yield return skillSelect(skillList);
                        else {
                            resultList = new List<int>();
                            resultList.Add(0);
                            resultList.Add(1);
                        }
                        checkWin.SetActive(true);
                        mode = 2;
                        skipGene = geneStart(buttonNum - 28);
                        StartCoroutine(skipGene);
                    }
                }
            }
        }
    }
    IEnumerator skillSelect(List<int> list) {
        Text[] skillText = new Text[10];
        Text[] skillSP = new Text[10];
        Text countText = skillWin.transform.Find("Count").GetComponent<Text>();
        bool[] skillTrue = new bool[list.Count];
        int skillCount = 0;
        int skillPage = 0;
        int skillPageMax = list.Count%10==0?list.Count/10:(list.Count/10+1);
        mode = 3;
        for (int i = 0; i < 10; i++) {
            skillText[i] = skillWin.transform.Find("Skill" + i.ToString()).GetComponent<Text>();
            skillSP[i] = skillText[i].transform.Find("SP").GetComponent<Text>();
            skillText[i].color = Color.white;
            skillSP[i].color = Color.white;
        }
        skillWin.SetActive(true);
        skillButtonNum = -1;
        if (list.Count <= 10) {
            Button[56].SetActive(false);
            Button[57].SetActive(false);
        } else {
            Button[56].SetActive(true);
            Button[57].SetActive(true);
        }
        while (true) {
            countText.text = skillCount.ToString() + "/8";
            if (skillCount > 8) countText.color = Color.red;
            else countText.color = Color.white;
            if (skillButtonNum >= 56 || skillButtonNum == -1) {
                for (int i = 0; i < 10; i++) {
                    if (list.Count <= i + skillPage * 10) {
                        skillText[i].text = "";
                        skillSP[i].text = "";
                        skill[i] = null;
                    } else {
                        skillText[i].text = skillDB.SkillDataList[list[i + skillPage * 10]].Name;
                        skillSP[i].text = skillDB.SkillDataList[list[i + skillPage * 10]].SP.ToString();
                        skill[i] = skillDB.SkillDataList[list[i + skillPage * 10]];
                        if (skillTrue[i + skillPage * 10]) {
                            skillText[i].color = new Color(0.6f, 1, 0.6f, 1);
                            skillSP[i].color = new Color(0.6f, 1, 0.6f, 1);
                        } else {
                            skillText[i].color = Color.white;
                            skillSP[i].color = Color.white;
                        }
                    }
                }
            }

            skillButtonNum = -1;
            while (mode != 3 || skillButtonNum == -1) yield return null;
            if (list.Count <= 10 && (skillButtonNum == 56 || skillButtonNum == 57)) skillButtonNum = 58;
            if (skillButtonNum == 55) {//決定
                if (skillCount <= 8) {
                    playSE(4, 0);
                    break;
                } else playSE(0, 0);
            } else if (skillButtonNum == 56 || skillButtonNum == 57) {//←,→
                playSE(1, 0);
                skillPage += skillButtonNum == 56 ? -1 : 1;
                skillPage = (skillPage + skillPageMax) % skillPageMax;
            } else if(skillButtonNum<=55){//スキル
                int num = skillButtonNum - 45 + skillPage * 10;
                if (skillTrue[num]) {
                    skillTrue[num] = false;
                    num = skillButtonNum - 45;
                    skillText[num].color = Color.white;
                    skillSP[num].color = Color.white;
                    playSE(0, 0);
                    skillCount--;
                } else {
                    skillTrue[num] = true;
                    num = skillButtonNum - 45;
                    skillText[num].color = new Color(0.6f, 1, 0.6f, 1);
                    skillSP[num].color = new Color(0.6f, 1, 0.6f, 1);
                    playSE(4, 0);
                    skillCount++;
                }
            }
        }
        resultList = new List<int>();
        resultList.Add(0);
        resultList.Add(1);
        for (int i = 0; i < list.Count; i++) {
            if (skillTrue[i]) resultList.Add(list[i]);
        }
        mode = 2;
        skillWin.SetActive(false);
    }
    IEnumerator geneStart(int num) {//合成開始
        yield return new WaitForSeconds(10 / 60f);
        Debug.Log(itemNum + 10);

        int itemID=0;
        if (itemNum == 1) itemID = 29;
        else if (itemNum == 3) itemID = 30;
        else if (itemNum == 4) itemID = 31;
        else if (itemNum == 5) itemID = 32;
        else if (itemNum == 6) itemID = 33;
        else if (itemNum == 7) itemID = 34;
        else if (itemNum == 8) itemID = 35;
        else if (itemNum == 9) itemID = 36;
        else if (itemNum == 10) itemID = 37;
        else if (itemNum == 11) itemID = 38;

        checkButtonNum = -1;
        while (mode != 2 || checkButtonNum == -1) yield return null;
        if (checkButtonNum == 43) {//生成する
            if (tutorial && TutorialNum == 6) TutorialNum = 7;
            playSE(4, 0);

        } else {//戻る
            playSE(0, 0);
            mode = 0;
            checkWin.SetActive(false);
            yield break;
        }

        if (itemID >= 1 && itemID <= 36 && itemID != 34) {
            itemDB.ItemDataList[itemID].Possess -= 1;
            if (itemDB.ItemDataList[itemID].Possess < 0) itemDB.ItemDataList[itemID].Possess = 0;
        }

        /*for (int i = 0; i < 4; i++) {
            if (monster[i].Equip >= 0) itemDB.ItemDataList[monster[i].Equip].Possess++;
        }*/
        geneWin.GetComponent<Image>().color = new Color(1, 1, 1, 0);
        for (int i = 0; i < 5; i++) geneMon[i].GetComponent<Image>().color = new Color(1, 1, 1, 0);
        for (int i = 0; i < 4; i++) {
            if (monster[i].ID >= 1) {
                geneMon[i].GetComponent<Image>().sprite = monDB.MonsterDataList[monster[i].ID].FileName.GetComponent<SpriteRenderer>().sprite;
                geneMon[i].GetComponent<Image>().SetNativeSize();
            } else {
                geneMon[i].GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);
            }
        }
        if (itemNum >= 1) {
            geneMon[4].GetComponent<Image>().sprite = itemSprite[itemNum];
            geneMon[4].GetComponent<Image>().SetNativeSize();
        } else {
            geneMon[4].GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);
        }
        geneMon[0].transform.localPosition = new Vector3(0, 58, 0) + new Vector3(0, 1, 0) * 100;
        geneMon[1].transform.localPosition = new Vector3(0, 58, 0) + new Vector3(0, -1, 0) * 100;
        geneMon[2].transform.localPosition = new Vector3(0, 58, 0) + new Vector3(-3, 0, 0) * 100;
        geneMon[3].transform.localPosition = new Vector3(0, 58, 0) + new Vector3(3, 0, 0) * 100;
        Vector3 scale = geneMon[0].transform.localScale;
        Vector3 scaleItem = geneMon[4].transform.localScale;
        numGene = num;
        scaleItemGene = scaleItem;
        scaleGene = scale;
        geneWin.SetActive(true);
        skip = true;
        for (int i = 0; i < 60; i++) {
            geneWin.GetComponent<Image>().color = new Color(1, 1, 1, i/59f);
            for (int j = 0; j < 5; j++) geneMon[j].GetComponent<Image>().color = new Color(1, 1, 1, i / 59f);
            yield return new WaitForSeconds(1 / 60f);
        }
        yield return new WaitForSeconds(30 / 60f);
        playSE(5,0);
        for (int i = 0; i < 30; i++) {
            geneMon[0].transform.localPosition = new Vector3(0, 58, 0) + new Vector3(3 * Mathf.Cos(i / 10f + Mathf.PI / 2), Mathf.Sin(i / 10f + Mathf.PI / 2), 0) * 100;
            geneMon[1].transform.localPosition = new Vector3(0, 58, 0) + new Vector3(3 * Mathf.Cos(i / 10f - Mathf.PI / 2), Mathf.Sin(i / 10f - Mathf.PI / 2), 0) * 100;
            geneMon[2].transform.localPosition = new Vector3(0, 58, 0) + new Vector3(3 * Mathf.Cos(i / 10f + Mathf.PI), Mathf.Sin(i / 10f + Mathf.PI), 0) * 100; 
            geneMon[3].transform.localPosition = new Vector3(0, 58, 0) + new Vector3(3 * Mathf.Cos(i / 10f), Mathf.Sin(i / 10f), 0) * 100;
            yield return new WaitForSeconds(1/60f);
        }
        float pre;
        for (int i = 30; i < 150; i++) {
            pre = (150 - i) / 120f;
            geneMon[4].transform.localScale = scaleItem*pre;
            geneMon[0].transform.localScale = geneMon[1].transform.localScale = geneMon[2].transform.localScale = geneMon[3].transform.localScale = scale * pre;
            geneMon[0].transform.localPosition = new Vector3(0, 58, 0) + new Vector3(3 * pre * Mathf.Cos(i / 10f + Mathf.PI / 2), pre*Mathf.Sin(i / 10f + Mathf.PI / 2), 0) * 100;
            geneMon[1].transform.localPosition = new Vector3(0, 58, 0) + new Vector3(3 * pre * Mathf.Cos(i / 10f - Mathf.PI / 2), pre*Mathf.Sin(i / 10f - Mathf.PI / 2), 0) * 100;
            geneMon[2].transform.localPosition = new Vector3(0, 58, 0) + new Vector3(3 * pre * Mathf.Cos(i / 10f + Mathf.PI), pre*Mathf.Sin(i / 10f + Mathf.PI), 0) * 100;
            geneMon[3].transform.localPosition = new Vector3(0, 58, 0) + new Vector3(3 * pre * Mathf.Cos(i / 10f), pre*Mathf.Sin(i / 10f), 0) * 100;
            yield return new WaitForSeconds(1 / 60f);
        }
        playSE(7, 0);
        yield return new WaitForSeconds(90 / 60f);
        StartCoroutine(geneFinish());
    }
    IEnumerator geneFinish() {
        int num=numGene;
        Vector3 scaleItem=scaleItemGene;
        Vector3 scale=scaleGene;
        skip = false;
        playSE(6, 0);
        geneWin.GetComponent<Image>().color = new Color(1, 1, 1, 1);
        geneMon[4].transform.localScale = Vector3.zero;
        geneMon[0].transform.localScale = geneMon[1].transform.localScale = geneMon[2].transform.localScale = geneMon[3].transform.localScale = Vector3.zero;

        GameObject eff = Instantiate(effect, geneWin.transform);
        GameObject mon = Instantiate(monDB.MonsterDataList[monster[num + 4].ID].FileName);
        StartCoroutine(anim(mon, geneMon[4]));
        mon.transform.localScale = Vector3.zero;
        eff.transform.localScale = new Vector3(5, 1.5f, 1);
        eff.transform.localPosition = geneMon[4].transform.localPosition + new Vector3(0, 50, 0);
        Image img = geneMon[4].GetComponent<Image>();
        img.color = new Color(1, 1, 1, 0);
        geneMon[4].transform.localScale = scaleItem;
        img.sprite = monDB.MonsterDataList[monster[num + 4].ID].FileName.GetComponent<SpriteRenderer>().sprite;
        img.SetNativeSize();
        for (int i = 0; i < 30; i++) {
            img.color = new Color(1, 1, 1, i / 29f);
            yield return new WaitForSeconds(1 / 60f);
        }
        img.color = new Color(1, 1, 1, 1);
        yield return new WaitForSeconds(120 / 60f);
        playSE(8, 0);
        messageWin.SetActive(true);
        monDB.Resister(monster[num + 4].ID,true,true);
        messageWin.transform.Find("Text").GetComponent<Text>().text = monDB.MonsterDataList[monster[num + 4].ID].Name + "\nを生成した！";
        while (Input.GetKeyDown(KeyCode.Z) || Input.GetMouseButton(0)) yield return null;
        while (!Input.GetKeyDown(KeyCode.Z) && !Input.GetMouseButton(0)) yield return null;
        while (Input.GetKeyDown(KeyCode.Z) || Input.GetMouseButton(0)) yield return null;
        messageWin.SetActive(false);
        Destroy(mon);
        checkWin.SetActive(false);
        for (int i = 0; i < 4; i++) {
            if (monster[i].ID >= 1) {
                allDB.AllyDataList[monster[i].preNumber] = new AllyData();
                allDB.AllyDataList[monster[i].preNumber].ID = -1;
            }
            if (monster[i].ID >= 1 && monster[i].preNumber < 4) myDB.party[monster[i].preNumber] = -1;
        }
        for (int i = 0; i < allDB.AllyDataList.Count; i++) {
            if (allDB.AllyDataList[i].ID <= 0) {
                statesCreate(num + 4);
                allDB.AllyDataList[i] = monster[num + 4];
                if (i < 4) myDB.party[i] = i;
                break;
            }
        }
        for (int i = 0; i < 8; i++) {
            monster[i] = new AllyData();
            monster[i].ID = -1;
        }
        itemNum = 0;
        Button[27].transform.Find("Text").GetComponent<Text>().text = "-------";
        allyDB = new AllyData[allDB.AllyDataList.Count];
        for (int i = 0; i < allDB.AllyDataList.Count; i++) {
            allyDB[i] = allDB.AllyDataList[i];
            allyDB[i].preNumber = i;
        }
        partyUpdate();
        boxUpdate(0);
        resultUpdate();
        for (int i = 0; i < 60; i++) {
            geneWin.GetComponent<Image>().color = new Color(1, 1, 1, (59 - i) / 59f);
            for (int j = 0; j < 5; j++) geneMon[j].GetComponent<Image>().color = new Color(1, 1, 1, (59 - i) / 59f);
            yield return new WaitForSeconds(1 / 60f);
        }
        for (int i = 0; i < 4; i++) geneMon[i].transform.localScale = scale;
        pageNow = 0;
        selectMon = -1;
        selectPre = -1;
        mode = 0;
        if (tutorial && TutorialNum == 7) { yield return StartCoroutine(messageShow("ジ=トリ博士", "これが合成の流れだ。\n「閉じる」を押してくれ。")); TutorialNum = 8; flagDB.Event[7] = 1; }

    }
    IEnumerator anim(GameObject copy,GameObject paste) {
        Image pImage = paste.GetComponent<Image>();
        SpriteRenderer pSprite = copy.GetComponent<SpriteRenderer>();
        while (true) {
            if (copy == null) yield break;
            pImage.sprite = pSprite.sprite;
            yield return null;
        }
    }
    IEnumerator itemSelect() {
        int saveIndex=0;
        string itemName;
        itemButtonNum = -1;
        for(int i = 0; i < 10; i++) {
            item[i] = null;
            Button[i + 32].GetComponent<Text>().text = "";
            Button[i + 32].transform.Find("SP").GetComponent<Text>().text = "";

        }
        for (int i = 0; i < itemDB.ItemDataList.Count; i++) {
            if(itemDB.ItemDataList[i].Type==-3 && itemDB.ItemDataList[i].Possess >= 1) {
                item[saveIndex] = itemDB.ItemDataList[i];
                Button[saveIndex + 32].GetComponent<Text>().text=item[saveIndex].Name;
                Button[saveIndex + 32].transform.Find("SP").GetComponent<Text>().text = "x"+item[saveIndex].Possess.ToString();
                saveIndex++;
            }
        }
        itemButtonNum = -1;
        while (mode != 1 || itemButtonNum == -1) yield return null;
        itemName = "-------";
        if (itemButtonNum == 42) {//戻る
            playSE(0, 0);
            itemNum = -1;
        } else {//アイテム
            playSE(4, 0);
            itemName = item[itemButtonNum - 32].Name;
            if (itemName == "高温の素") itemNum = 1;
            //else if (itemName == "低温の素") itemNum = 2;
            else if (itemName == "電気の素") itemNum = 3;
            else if (itemName == "高圧の素") itemNum = 4;
            else if (itemName == "紫外線の素") itemNum = 5;
            else if (itemName == "光の素") itemNum = 6;
            else if (itemName == "V<size=24>2</size>O<size=24>5</size>触媒") itemNum = 7;
            else if (itemName == "高温高圧の素") itemNum = 8;
            else if (itemName == "ハーバー・ボッシュ") itemNum = 9;
            else if (itemName == "Pt触媒") itemNum = 10;
            else if (itemName == "MnO<size=24>2</size>触媒") itemNum = 11;
        }
        itemWin.SetActive(false);
        mode = 0;
        Button[27].transform.Find("Text").GetComponent<Text>().text = itemName;
        resultUpdate();
        yield break;
    }
    IEnumerator messageShow(string name,string text) {
        mWin.SetActive(true);
        messageFlag = true;
        M_text.text = text;
        M_name.text = name;
        while (Input.GetKeyDown(KeyCode.Z) || Input.GetMouseButton(0)) yield return null;
        while (!Input.GetKeyDown(KeyCode.Z) && !Input.GetMouseButton(0)) yield return null;
        while (Input.GetKeyDown(KeyCode.Z) || Input.GetMouseButton(0)) yield return null;
        mWin.SetActive(false);
        messageFlag =false;
        yield return null;
    }
    void partyUpdate() {//素材画像更新
        for (int i = 0; i < 4; i++) {
            if (monster[i]==null || monster[i].ID == -1) {
                Button[i].GetComponent<Image>().sprite = emptyBox;
            } else {
                Button[i].GetComponent<Image>().sprite
                    = monDB.MonsterDataList[monster[i].ID].Icon;
            }
            if (selectMon == i) Button[i].GetComponent<Image>().color = new Color(0.5f, 0.56f, 1, 1);
            else Button[i].GetComponent<Image>().color = new Color(1, 1, 1, 1);

        }
    }
    void boxUpdate(int page) {//ボックス画像更新
        for (int i = page * 20; i < (page + 1) * 20; i++) {
            if (allyDB[i] == null || allyDB[i].ID == -1) {
                Button[i % 20 + 4].GetComponent<Image>().sprite = emptyBox;
            } else {
                Button[i % 20 + 4].GetComponent<Image>().sprite
                    = monDB.MonsterDataList[allyDB[i].ID].Icon;
            }
            if (selectMon == i) Button[i % 20 + 4].GetComponent<Image>().color = new Color(0.5f, 0.56f, 1, 1);
            else Button[i % 20 + 4].GetComponent<Image>().color = new Color(1, 1, 1, 1);
        }
        resultUpdate();
    }
    void resultUpdate() {//結果画像更新
        int[] id = new int[4];
        bool able = false,preAble;
        int countFlag = 0;
        int num = -1,result;
        for (int i = 0; i < geneDB.GeneDataList.Count; i++) {
            id[0] = geneDB.GeneDataList[i].Monster1;
            id[1] = geneDB.GeneDataList[i].Monster2;
            id[2] = geneDB.GeneDataList[i].Monster3;
            id[3] = geneDB.GeneDataList[i].Monster4;
            for (int j = 0; j < 4; j++) {
                if (monster[j].ID >= 1) {
                    preAble = false;
                    for (int k = 0; k < 4; k++) {
                        preAble = preAble || monster[j].ID == id[k];
                    }
                    if(monster[j].ID>=1)able = preAble;
                    if (!able) break;
                }
            }
            for(int j = 0; j < 4; j++) {
                preAble = false;
                for (int k = 0; k < 4; k++) preAble = preAble || monster[k].ID == id[j];
                if(id[j]>=1) able = able && preAble;
            }
            if (geneDB.GeneDataList[i].Item >= 1) able = able && itemNum == geneDB.GeneDataList[i].Item;
            else able = able && itemNum <= 0;
            if (able) {
                num = i;
                break;
            }
        }

        if (num == -1) {
            countFlag += monster[0].ID >= 1 ? 1 : 0;
            countFlag += monster[1].ID >= 1 ? 1 : 0;
            countFlag += monster[2].ID >= 1 ? 1 : 0;
            countFlag += monster[3].ID >= 1 ? 1 : 0;
            countFlag += itemNum > 0 ? 1 : 0;
            if (countFlag <= 1) {
                for (int i = 28; i < 32; i++) {
                    Button[i].GetComponent<Image>().sprite = emptyBox;
                    monster[i - 24].ID = 0;
                }
            } else {
                for (int i = 28; i < 32; i++) {
                    Button[i].GetComponent<Image>().sprite = Button[i - 28].GetComponent<Image>().sprite;
                    monster[i - 24].ID = monster[i - 28].ID;
                    statesCreate(i - 24);
                }
            }
        } else {
            for (int i = 28; i < 32; i++) {
                result = 0;
                if (i==28)result = geneDB.GeneDataList[num].Result1;
                else if(i==29) result = geneDB.GeneDataList[num].Result2;
                else if (i == 30) result = geneDB.GeneDataList[num].Result3;
                else if (i == 31) result = geneDB.GeneDataList[num].Result4;
                if (result >= 1) {
                    monster[i - 24].ID = result;
                    Button[i].GetComponent<Image>().sprite
                        = monDB.MonsterDataList[result].Icon;
                } else {
                    Button[i].GetComponent<Image>().sprite = emptyBox;
                    monster[i - 24].ID = 0;
                }
                statesCreate(i - 24);
            }
        }
    }
    void statesCreate(int num) {//合成ステータス計算
        int[] growList = new int[7];
        int growMax=0,sumChara=0,skillLen;
        //bool skill;
        if (monster[num].ID >= 1) {
            MonsterData data = monDB.MonsterDataList[monster[num].ID];
            monster[num].Name = data.Name;
            monster[num].Level = 1;

            for (int i = 0; i < 7; i++) {
                growMax = 0;
                for (int j = 0; j < 4; j++) {
                    if (monster[j].ID >= 1)growMax = max(monster[j].Grow[i] - getGrow(i, monDB.MonsterDataList[monster[j].ID]), growMax);
                }
                monster[num].Grow[i] = getGrow(i,data)+growMax+1;
                growMax = 0;
                for (int j = 0; j < 4; j++) {
                    //if (monster[j].ID >= 1) growMax = max(getState(i,monster[j]), growMax);
                    if (monster[j].ID >= 1) growMax = max(monster[j].Level, growMax);

                }
                growList[i] = monster[num].Grow[i]+growMax/2;
            }
            for (int i = 0; i < 4; i++) {
                if(monster[i].ID>=1)sumChara += monster[i].Level - 1;
            }
            int lev= 10;
            monster[num].NowHP = monster[num].HP = growList[0] + lev * 5;
            monster[num].NowSP = monster[num].SP = growList[1] + lev * 4;
            monster[num].Attack = growList[2] + lev * 3;
            monster[num].Defence = growList[3] + lev * 3;
            monster[num].Magic = growList[4] + lev * 3;
            monster[num].MagicDef = growList[5] + lev * 3;
            monster[num].Speed = growList[6] + lev * 3;
            monster[num].Exp = 0;
            monster[num].NextExp = 16;
            monster[num].CharaPt = 1;
            monster[num].Equip = -1;
            MonsterData mon = monDB.MonsterDataList[monster[num].ID];

            monster[num].NowHP = monster[num].HP = Math.Min(monster[num].HP, mon.HPMax);
            monster[num].NowSP = monster[num].SP = Math.Min(monster[num].SP, mon.SPMax);
            monster[num].Attack  = Math.Min(monster[num].Attack, mon.AttackMax);
            monster[num].Defence = Math.Min(monster[num].Defence, mon.DefenceMax);
            monster[num].Magic = Math.Min(monster[num].Magic, mon.MagicMax);
            monster[num].MagicDef = Math.Min(monster[num].MagicDef, mon.MagicDefMax);
            monster[num].Speed = Math.Min(monster[num].Speed, mon.SpeedMax);

            if (resultList != null) {
                skillLen = resultList.Count;
                Array.Resize(ref monster[num].Skill, skillLen);
                for (int i = 0; i < skillLen; i++)monster[num].Skill[i] = resultList[i];
                Array.Resize(ref monster[num].SkillUse, skillLen);
                for (int i = 0; i < skillLen; i++) monster[num].SkillUse[i] = 0;
            }
            Array.Resize(ref monster[num].Character, monDB.MonsterDataList[monster[num].ID].Character.Length);
            for (int i = 0; i < monster[num].Character.Length; i++)monster[num].Character[i] = 1;
        }
    }
    int getGrow(int num,MonsterData mon) {
        if (num == 0) return mon.HPGrow;
        else if (num == 1) return mon.SPGrow;
        else if (num == 2) return mon.AttackGrow;
        else if (num == 3) return mon.DefenceGrow;
        else if (num == 4) return mon.MagicGrow;
        else if (num == 5) return mon.MagicDefGrow;
        else if (num == 6) return mon.SpeedGrow;
        else return 0;
    }
    int getState(int num, AllyData mon) {
        if (num == 0) return mon.HP;
        else if (num == 1) return mon.SP;
        else if (num == 2) return mon.Attack;
        else if (num == 3) return mon.Defence;
        else if (num == 4) return mon.Magic;
        else if (num == 5) return mon.MagicDef;
        else if (num == 6) return mon.Speed;
        else return 0;
    }
    void states(int num) {//ステータス表示
        string pre1 = "", pre2 = "";
        string[] rome = { "零", "Ⅰ", "Ⅱ", "Ⅲ", "Ⅳ", "Ⅴ" };
        AllyData Ally=null;
        if (num >= 4) Ally = allyDB[num - 4];
        else if (num >= 0) Ally = monster[num];
        else if (num <= -2) Ally = monster[num + 18];
        if ((num >= 0 || num<=-2) && Ally!=null && Ally.ID >= 1) {
            statesObj[0].text = Ally.Name;
            statesObj[1].text = Ally.Attack.ToString() + "\n"
                                + Ally.Defence.ToString() + "\n"
                                + Ally.Magic.ToString() + "\n"
                                + Ally.MagicDef.ToString() + "\n"
                                + Ally.Speed.ToString() + "\n";
            statesObj[2].text = "";
            statesObj[3].text = "";
            if (num+18 < 4 || num+18 >= 8) {
                for (int i = 2; i < Ally.Skill.Length; i += 2) {
                    SkillData skill = skillDB.SkillDataList[Ally.Skill[i]];
                    if (skill.Chemi) statesObj[2].text += "<color=#555577>" + skill.Name + "</color>\n";
                    else statesObj[2].text += skill.Name + "\n";
                }
                for (int i = 3; i < Ally.Skill.Length; i += 2) {
                    SkillData skill = skillDB.SkillDataList[Ally.Skill[i]];
                    if (skill.Chemi) statesObj[3].text += "<color=#555577>" + skill.Name + "</color>\n";
                    else statesObj[3].text += skill.Name + "\n";
                }
            }
            for (int i = 0; i < monDB.MonsterDataList[Ally.ID].Character.Length; i++) {
                if (Ally.Character[i] >= 0) {
                    pre1 += charDB.CharaDataList[monDB.MonsterDataList[Ally.ID].Character[i]].Name + "\n";
                    pre2 += rome[Ally.Character[i]] + "\n";
                }
            }
            statesObj[4].text = pre1;
            statesObj[5].text = pre2;
            statesObj[6].text = Ally.Level.ToString();
            statesObj[7].text = Ally.HP.ToString();
            statesObj[8].text = Ally.SP.ToString();
            iconObj.sprite = monDB.MonsterDataList[Ally.ID].Icon;
        } else {
            statesObj[0].text = "----------";
            statesObj[1].text = "---\n---\n---\n---\n---";
            statesObj[2].text = "";
            statesObj[3].text = "";
            statesObj[4].text = "";
            statesObj[5].text = "";
            statesObj[6].text = "---";
            statesObj[7].text = "---";
            statesObj[8].text = "---";
            iconObj.sprite = emptyBox;
        }
    }
    int max(int a,int b) { if (a > b) return a; else return b; }
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
        bool itemFlag;
        bool skillSelectFlag;
        if (!messageFlag) {
            if (skip && (Input.GetMouseButtonUp(0) || Input.GetMouseButton(0))) {
                StopCoroutine(skipGene);
                StartCoroutine(geneFinish());
            }
            if (mode == 0 && (Input.GetMouseButtonUp(0) || Input.GetMouseButton(0))) {//メインメニュー
                mousePos = Input.mousePosition;
                for (int i = 0; i < 32; i++) {
                    if ((i >= 0 && i < 24) || (i >= 28 && i < 32)) {
                        if (Input.GetMouseButtonUp(0)) {
                            if (buttonRe[i].Contains(mousePos)) buttonNum = i;
                        } else if (Input.GetMouseButton(0)) {
                            if (buttonRe[i].Contains(mousePos) && selectMon < 0) {
                                if (i < 4) states(i);
                                else if (i < 24) states(i + pageNow * 20);
                                else if (i >= 28 && i < 32) states(i - 32 - 10);
                            }
                        }
                    } else {
                        if (Input.GetMouseButtonUp(0)) {
                            if (buttonRe[i].Contains(mousePos)) buttonNum = i;
                            Button[i].GetComponent<Image>().color = new Color(0.5f, 0.56f, 1, 1);
                        } else if (Input.GetMouseButton(0)) {
                            if (buttonRe[i].Contains(mousePos)) {
                                Button[i].GetComponent<Image>().color = new Color(1, 1, 1, 1);
                            } else if (Button[i] != null) Button[i].GetComponent<Image>().color = new Color(0.5f, 0.56f, 1, 1);
                        }
                    }
                }
            }
            if (mode == 1 && (Input.GetMouseButtonUp(0) || Input.GetMouseButton(0))) {//アイテム
                mousePos = Input.mousePosition;
                itemFlag = false;
                for (int i = 32; i < 43; i++) {
                    if (Input.GetMouseButtonUp(0)) {
                        if (buttonRe[i].Contains(mousePos)) {
                            if (i == 42 || (i <= 41 && item[i - 32] != null)) itemButtonNum = i;
                        }
                        if (i == 42) Button[i].GetComponent<Image>().color = new Color(0.5f, 0.56f, 1, 1);
                    } else if (Input.GetMouseButton(0)) {
                        if (buttonRe[i].Contains(mousePos)) {
                            if (i <= 41 && item[i - 32] != null) {
                                itemFlag = true;
                                itemDetail.GetComponent<Text>().text = item[i - 32].Detail;
                                itemCursor.transform.position = Button[i].transform.position - new Vector3(5, -3, 0);
                            } else if (i == 42) Button[i].GetComponent<Image>().color = new Color(1, 1, 1, 1);
                        } else if (Button[i] != null) {
                            if (i == 42) Button[i].GetComponent<Image>().color = new Color(0.5f, 0.56f, 1, 1);
                        }
                    }
                }
                if (!itemFlag) {
                    itemCursor.transform.position = new Vector3(-1000, 0, 0);
                    itemDetail.GetComponent<Text>().text = "";
                }
            }
            if (mode == 2 && (Input.GetMouseButtonUp(0) || Input.GetMouseButton(0))) {//チェック
                mousePos = Input.mousePosition;
                for (int i = 43; i < 45; i++) {
                    if (Input.GetMouseButtonUp(0)) {
                        if (buttonRe[i].Contains(mousePos)) checkButtonNum = i;
                        Button[i].GetComponent<Image>().color = new Color(0.5f, 0.56f, 1, 1);
                    } else if (Input.GetMouseButton(0)) {
                        if (buttonRe[i].Contains(mousePos)) {
                            Button[i].GetComponent<Image>().color = new Color(1, 1, 1, 1);
                        } else if (Button[i] != null) Button[i].GetComponent<Image>().color = new Color(0.5f, 0.56f, 1, 1);
                    }
                }
            }
            if (mode == 3 && (Input.GetMouseButtonUp(0) || Input.GetMouseButton(0))) {//スキル選択
                mousePos = Input.mousePosition;
                skillSelectFlag = false;
                for (int i = 45; i < 58; i++) {
                    if (Input.GetMouseButtonUp(0)) {
                        if (buttonRe[i].Contains(mousePos)) {
                            if (i >= 55 || (i <=54  && skill[i - 45] != null)) skillButtonNum = i;
                        }
                        if (i >= 55) Button[i].GetComponent<Image>().color = new Color(0.5f, 0.56f, 1, 1);
                    } else if (Input.GetMouseButton(0)) {
                        if (buttonRe[i].Contains(mousePos)) {
                            if (i <= 54 && skill[i - 45] != null) {
                                skillSelectFlag = true;
                                skillDetail.GetComponent<Text>().text = skill[i - 45].Detail;
                                skillCursor.transform.position = Button[i].transform.position - new Vector3(5, -3, 0);
                            } else if (i >= 55) Button[i].GetComponent<Image>().color = new Color(1, 1, 1, 1);
                        } else if (Button[i] != null) {
                            if (i >= 55) Button[i].GetComponent<Image>().color = new Color(0.5f, 0.56f, 1, 1);
                        }
                    }
                }
                if (!skillSelectFlag) {
                    skillCursor.transform.position = new Vector3(-1000, 0, 0);
                    skillDetail.GetComponent<Text>().text = "";
                }
            }
        }

    }
}
