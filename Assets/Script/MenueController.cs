using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class MenueController : MonoBehaviour{
    public MonsterStates StatesData;
    public ItemDatas ItemData;
    public SkillDatas SkillData;
    public CharaDatas CharaData;
    public AllyDatas AllyData;
    public SaveDatas SaveData;
    public JumpDatas jumpData;
    public FlagDatas flagData;
    public AudioClip[] SE;
    GameObject[] monster,line,Button;
    GameObject gameCon;
    GameObject monsterWin, monsterCursor;
    GameObject statesWin;
    GameObject skillWin,skillCursor;
    GameObject jumpWin, jumpCursor;
    GameObject saveWin;
    GameObject alertWin;
    GameObject messageWin;
    GameObject charaWin;
    GameObject fileWin,fileCursor;
    GameObject fade;
    GameObject fadeGameCon;
    GameObject tutoWin;
    GameObject saveBlack;
    JumpData[] jumpList;
    Rect[] buttonRe;
    Image[] window;
    Sprite sprite;
    Vector3[] posMonster;
    Text skillDetail,message,equipText;
    Text[] charaText,fileText;
    Text chPre, chNew, chCost,chButton,fiChara,fiExpe,fileComp;
    Image fileImage;
    AudioSource audioS;

    int mode = 0;
    int party;
    int skillStart = 0;
    int itemStart = 0;
    int buttonNum = -1;
    int monsterButtonNum = -1;
    int skillButtonNum = -1;
    int saveButtonNum = -1;
    int alertButtonNum = -1;
    int jumpButtonNum = -1;
    int charaButtonNum = -1;
    int fileButtonNum = -1;
    int skillNumber;
    int itemNumber;
    int[] itemList = new int[200];
    bool skillUse = false;
    bool skillReset;
    bool itemUse = false;
    bool itemReset;
    bool itemFlag = false;
    bool itemBreak = false;

    void Start(){
        SysDB.menueFlag = true;
        SysDB.eventFlag = true;

        line = new GameObject[4];
        monster = new GameObject[4];
        Button = new GameObject[60];
        buttonRe = new Rect[Button.Length];
        window = new Image[4];
        posMonster = new Vector3[4];
        charaText = new Text[8];
        fileText = new Text[8];
        jumpList = new JumpData[jumpData.JumpDataList.Count];
        gameCon = GameObject.Find("GameController");
        monsterWin = GameObject.Find("MonsterWindow");
        statesWin = GameObject.Find("StatesWindow");
        skillWin = GameObject.Find("SkillWindow");
        saveWin = GameObject.Find("SaveWindow");
        alertWin = GameObject.Find("AlertWindow");
        jumpWin = GameObject.Find("JumpWindow");
        fileWin = GameObject.Find("FileWindow");
        messageWin = GameObject.Find("MenueMessageWin");
        charaWin = GameObject.Find("CharaWindow");
        tutoWin = GameObject.Find("TutoWindow");
        fade = GameObject.Find("MenueFade");
        fadeGameCon = GameObject.Find("FadeCanvas");
        audioS = GetComponent<AudioSource>();
        saveBlack = saveWin.transform.Find("saveWait").gameObject;
        monsterCursor = monsterWin.transform.Find("Cursor").gameObject;
        jumpCursor = jumpWin.transform.Find("Cursor").gameObject;
        skillCursor = skillWin.transform.Find("Cursor").gameObject;
        skillDetail = skillWin.transform.Find("detail").Find("textDetail").gameObject.GetComponent<Text>();
        message = messageWin.transform.Find("Text").GetComponent<Text>();
        fileCursor = fileWin.transform.Find("Cursor").gameObject;

        for (int i = 0; i < 4; i++) {
            if (myDB.party[i] >= 0) {
                monster[i] = StatesData.MonsterDataList[AllyData.AllyDataList[myDB.party[i]].ID].FileName;
                window[i] = GameObject.Find("MonsterImage"+i.ToString()).GetComponent<Image>();
                sprite = monster[i].GetComponent<SpriteRenderer>().sprite;
                window[i].sprite = sprite;
                window[i].rectTransform.sizeDelta = new Vector2(sprite.rect.width,sprite.rect.height);
                line[i] = GameObject.Find("Line" + i.ToString());
                line[i].transform.Find("MonsterName" + i.ToString()).GetComponent<Text>().text = StatesData.MonsterDataList[AllyData.AllyDataList[myDB.party[i]].ID].Name;
                line[i].transform.Find("MonsterName" + i.ToString()).GetComponent<Text>().color = new Color(0.8f,1f,0.7f);
                monsterWin.transform.Find("MonsterName" + (i + 1).ToString()).GetComponent<Text>().text = StatesData.MonsterDataList[AllyData.AllyDataList[myDB.party[i]].ID].Name;
                line[i].transform.Find("Level").Find("LevelValue").GetComponent<Text>().text = AllyData.AllyDataList[myDB.party[i]].Level.ToString();
                HPSPcontroll(i);
            } else {
                line[i] = GameObject.Find("Line" + i.ToString());
                line[i].SetActive(false);
                monsterWin.transform.Find("MonsterName" + (i + 1).ToString()).GetComponent<Text>().text = "";
            }
            posMonster[i] = line[i].transform.position;

        }

        for (int i = 0; i < 7; i++)Button[i] = GameObject.Find("MainButton" + i.ToString());
        for (int i = 7; i < 12; i++)Button[i] = monsterWin.transform.Find("MonsterName" + (i - 6).ToString()).gameObject;
        Button[12] = statesWin.transform.Find("Equip").gameObject;
        for (int i = 13; i < 26; i++)Button[i] = skillWin.transform.Find("Skill" + (i - 13).ToString()).gameObject;
        for (int i = 26; i < 29; i++)Button[i] = saveWin.transform.Find("Save" + (i - 26).ToString()).gameObject;
        Button[29] = GameObject.Find("YesButton");
        Button[30] = GameObject.Find("NoButton");
        for (int i = 31; i <39; i++)Button[i] = jumpWin.transform.Find("Jump" + (i - 31).ToString()).gameObject;
        for (int i = 39; i < 47; i++) Button[i] = statesWin.transform.Find("charaText"+(i-39)).gameObject;
        Button[47] = charaWin.transform.Find("Button1").gameObject;
        Button[48] = charaWin.transform.Find("Button2").gameObject;
        for (int i = 49; i < 57; i++) Button[i] = fileWin.transform.Find("File"+(i-49)).gameObject;
        Button[57] = fileWin.transform.Find("Button1").gameObject;
        Button[58] = fileWin.transform.Find("Button2").gameObject;
        Button[59] = fileWin.transform.Find("Button3").gameObject;

        for (int i = 0; i <= 7; i++) charaText[i] = Button[i+39].GetComponent<Text>();
        for (int i = 0; i <= 7; i++) fileText[i] = Button[i + 49].GetComponent<Text>();
        equipText = Button[12].GetComponent<Text>();
        chPre = charaWin.transform.Find("Text1").GetComponent<Text>();
        chNew = charaWin.transform.Find("Text2").GetComponent<Text>();
        chCost = charaWin.transform.Find("Text3").GetComponent<Text>();
        chButton = Button[47].transform.Find("Text").GetComponent<Text>();
        fiChara = fileWin.transform.Find("Chara").GetComponent<Text>();
        fiExpe = fileWin.transform.Find("Detail").GetComponent<Text>();
        fileImage = fileWin.transform.Find("Image").GetComponent<Image>();
        fileComp = fileWin.transform.Find("Comp").GetComponent<Text>();

        for (int i = 0; i < Button.Length; i++) buttonRe[i] = buttonRect(Button[i]);
        monsterWin.SetActive(false);
        statesWin.SetActive(false);
        skillWin.SetActive(false);
        saveWin.SetActive(false);
        alertWin.SetActive(false);
        jumpWin.SetActive(false);
        messageWin.SetActive(false);
        charaWin.SetActive(false);
        fileWin.SetActive(false);
        tutoWin.SetActive(false);
        saveBlack.SetActive(false);
        GameObject.Find("PlayerName").GetComponent<Text>().text = myDB.playerName;
        GameObject.Find("Money").GetComponent<Text>().text = myDB.money.ToString()+"G";
        StartCoroutine(MainMenue());
    }
    IEnumerator MainMenue() {
        while (true) {
            buttonNum = -1;
            while (mode != 0 || buttonNum == -1) yield return null;
            if(buttonNum>=1) playSE(4, 0);
            if (buttonNum == 0) {//戻る
                gameCon.GetComponent<GameController>().SE(3);
                SysDB.eventFlag = false;
                SysDB.menueFlag = false;
                SceneManager.SetActiveScene(SceneManager.GetSceneByName(SysDB.sceneName));
                SceneManager.UnloadSceneAsync("Scenes/Menue");
                yield break;
            }else if(buttonNum == 1) {//モンスター
                mode = 1;
                monsterWin.SetActive(true);
                StartCoroutine(Monster());
            } else if (buttonNum == 2) {//道具
                mode = 2;
                skillWin.SetActive(true);
                StartCoroutine(item());
            } else if (buttonNum == 3) {//セーブ
                mode = 3;
                saveWin.SetActive(true);
                StartCoroutine(save());
            } else if (buttonNum == 4) {//移動
                mode = 5;
                jumpWin.SetActive(true);
                StartCoroutine(jump());
            }else if(buttonNum == 5) {//ファイル
                mode = 8;
                fileWin.SetActive(true);
                StartCoroutine(file());
            }else if(buttonNum == 6) {//設定

            }
        }
    }
    //////////////////////////////////////////////////////////////////////////////////////////
    //↓モンスター//////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////////////////
    IEnumerator Monster() {//モンスター選択
        while (true) {
            monsterButtonNum = -1;
            while (mode != 1 || monsterButtonNum == -1) {
                if (itemBreak) {
                    itemBreak = false;
                    monsterWin.SetActive(false);
                    mode = 2;
                    itemUse = false;
                    itemReset = true;
                    skillWin.SetActive(true);
                    yield break;
                }
                yield return null;
            }
            if (monsterButtonNum <= 10 && myDB.party[monsterButtonNum - 7] == -1) continue;
            if (monsterButtonNum == 11 && !skillUse && !itemUse) {//戻る
                playSE(3, 0);
                monsterWin.SetActive(false);
                mode = 0;
                yield break;
            } else if ((monsterButtonNum <= 10 && !skillUse && !itemUse) || (monsterButtonNum == 11 && (skillUse||itemUse))) {//各モンスター
                if (monsterButtonNum <= 10) playSE(4, 0);
                if (monsterButtonNum == 11) {
                    playSE(3, 0);
                    monsterButtonNum = 7 + party;
                }
                skillReset = true;
                itemReset = true;

                mode = 2;
                if (!itemUse) {
                    for (int i = 0; i < 4; i++) {
                        if (myDB.party[monsterButtonNum - 7] != -1 && i != monsterButtonNum - 7) line[i].SetActive(false);
                    }
                    statesWin.SetActive(true);
                }
                skillWin.SetActive(true);
                line[monsterButtonNum - 7].transform.position = posMonster[0];
                int pre = myDB.party[monsterButtonNum - 7];
                if (pre >= 0)statesReload(pre);
                if (!skillUse && !itemUse) {
                    StartCoroutine(skill(monsterButtonNum - 7));
                }
                skillUse = false;
                itemUse = false;
            } else if(monsterButtonNum <= 10 && skillUse) {
                StartCoroutine(skillUsing(party, monsterButtonNum - 7,skillNumber));
            } else if(monsterButtonNum <= 10 && itemUse) {
                StartCoroutine(itemUsing(monsterButtonNum - 7, itemNumber));
            }
        }
    }
    void statesReload(int pre) {
        string[] rome = { "零", "Ⅰ", "Ⅱ", "Ⅲ", "Ⅳ", "Ⅴ" };
        statesWin.transform.Find("StatesValue").gameObject.GetComponent<Text>().text
            = AllyData.AllyDataList[pre].Attack.ToString() + "\n" + AllyData.AllyDataList[pre].Defence.ToString() + "\n"
              + AllyData.AllyDataList[pre].Magic.ToString() + "\n" + AllyData.AllyDataList[pre].MagicDef.ToString() + "\n"
              + AllyData.AllyDataList[pre].Speed.ToString();
        statesWin.transform.Find("ExValue").gameObject.GetComponent<Text>().text
            = AllyData.AllyDataList[pre].Exp.ToString() + "\n" + AllyData.AllyDataList[pre].NextExp.ToString() + "\n"
              + AllyData.AllyDataList[pre].CharaPt.ToString();
        statesWin.transform.Find("Equip").gameObject.GetComponent<Text>().text = "";
        for (int i = 0; i <= 7; i++)charaText[i].text = "";

        for (int i = 0; i < AllyData.AllyDataList[pre].Character.Length; i++) {
            if (i >= StatesData.MonsterDataList[AllyData.AllyDataList[pre].ID].Character.Length || StatesData.MonsterDataList[AllyData.AllyDataList[pre].ID].Character[i] == -1) break;
            else {
                charaText[i].text = rome[AllyData.AllyDataList[pre].Character[i]]
                    +"　"+CharaData.CharaDataList[StatesData.MonsterDataList[AllyData.AllyDataList[pre].ID].Character[i]].Name;
            }
        }
    }
    IEnumerator skill(int partyNum) {//スキル選択
        int skillCount = -1;
        int sp,skillNum,chLevel;
        int[] chPt = {0,5,15,25,35 }; 
        bool charaUp;
        string spSkill;
        string[] rome = { "零", "Ⅰ", "Ⅱ", "Ⅲ", "Ⅳ", "Ⅴ" };

        party = partyNum;
        skillStart = 0;
        for (int i = 0; i < AllyData.AllyDataList[myDB.party[partyNum]].Skill.Length; i++) {
            if (AllyData.AllyDataList[myDB.party[partyNum]].Skill[i] != -1) skillCount++;
        }

    skillSelect://ラベル//
        skillReset = false;
        skillButtonNum = -1;

        for (int i = 0; i < 10; i++) {
            if (i + skillStart < AllyData.AllyDataList[myDB.party[partyNum]].Skill.Length && AllyData.AllyDataList[myDB.party[partyNum]].Skill[i + skillStart] != -1) {
                Button[i + 13].GetComponent<Text>().text = SkillData.SkillDataList[AllyData.AllyDataList[myDB.party[partyNum]].Skill[i + skillStart]].Name;
                sp = SkillData.SkillDataList[AllyData.AllyDataList[myDB.party[partyNum]].Skill[i + skillStart]].SP;
                if (sp == 0) spSkill = "---";
                else spSkill = sp.ToString();
                if (SkillData.SkillDataList[AllyData.AllyDataList[myDB.party[partyNum]].Skill[i + skillStart]].Map) {
                    Button[i + 13].GetComponent<Text>().color = new Color(1, 1, 1, 1);
                    if (sp > AllyData.AllyDataList[myDB.party[partyNum]].NowSP) Button[i + 13].transform.Find("SP").gameObject.GetComponent<Text>().color = new Color(1, 0, 0, 1);
                    else Button[i + 13].transform.Find("SP").gameObject.GetComponent<Text>().color = new Color(1, 1, 1, 1);
                } else {
                    Button[i + 13].transform.Find("SP").gameObject.GetComponent<Text>().color = new Color(0.5f, 0.5f, 0.6f, 1);
                    Button[i + 13].GetComponent<Text>().color = new Color(0.5f, 0.5f, 0.6f, 1);
                }
                Button[i + 13].transform.Find("SP").gameObject.GetComponent<Text>().text = spSkill;
            } else {
                Button[i + 13].GetComponent<Text>().text = "";
                Button[i + 13].transform.Find("SP").gameObject.GetComponent<Text>().text = "";
            }

        }
        int skillNumCount = AllyData.AllyDataList[myDB.party[partyNum]].Skill.Length;
        if (skillNumCount <= 10) {
            Button[24].SetActive(false);
            Button[25].SetActive(false);
        } else {
            Button[24].SetActive(true);
            Button[25].SetActive(true);
        }

        if(AllyData.AllyDataList[myDB.party[partyNum]].CharaPt>=5 
            && StatesData.MonsterDataList[AllyData.AllyDataList[myDB.party[partyNum]].ID].Character.Length>0 
            && flagData.Event[41] == 0) {
            tutoWin.SetActive(true);
            yield return new WaitForSeconds(30/60f);
            flagData.Event[41] = 1;
            playSE(13, 0);
            while (Input.GetKeyDown(KeyCode.Z) || Input.GetMouseButton(0)) yield return null;
            while (!Input.GetKeyDown(KeyCode.Z) && !Input.GetMouseButton(0)) yield return null;
            while (Input.GetKeyDown(KeyCode.Z) || Input.GetMouseButton(0)) yield return null;
            tutoWin.SetActive(false);
        }

        while (true) {
            skillButtonNum = -1;
            while (mode != 2 || skillButtonNum == -1) {
                if(skillReset) goto skillSelect;
                yield return null;
            }
            if((skillButtonNum == 24 || skillButtonNum == 25) && skillNumCount <= 10) {
                skillButtonNum = -1;
                goto skillSelect;
            }
            if (skillButtonNum == 23) {//戻る
                playSE(3, 0);
                skillWin.SetActive(false);
                statesWin.SetActive(false);
                for (int i = 0; i < 4; i++) {
                    if (myDB.party[i] >= 0) {
                        line[i].SetActive(true);
                        line[i].transform.position = posMonster[i];
                    }
                }
                mode = 1;
                yield break;
            } else if (skillButtonNum == 24) {//←ボタン
                playSE(5, 0);
                skillStart -= 10;
                if (skillStart < 0) skillStart = skillCount - skillCount % 10;
                goto skillSelect;
            } else if (skillButtonNum == 25) {//→ボタン
                playSE(5, 0);
                skillStart += 10;
                if (skillStart > skillCount) skillStart = 0;
                goto skillSelect;
            } else if (skillButtonNum == 12) {//装備外す
                /*playSE(12, 0);
                equip(partyNum, -1);
                statesReload(myDB.party[partyNum]);*/
            }else if(skillButtonNum>=39 && skillButtonNum<=46){//特性
                if (charaText[skillButtonNum - 39].text != "") {
                    playSE(4, 0);
                    charaWin.SetActive(true);
                    mode = 7;
                    
                    while (true) {
                        chLevel = AllyData.AllyDataList[myDB.party[partyNum]].Character[skillButtonNum - 39];
                        chPre.text = rome[chLevel] + "　" + CharaData.CharaDataList[StatesData.MonsterDataList[AllyData.AllyDataList[myDB.party[partyNum]].ID].Character[skillButtonNum - 39]].Name;
                        chPre.text += "\n" + CharaData.CharaDataList[StatesData.MonsterDataList[AllyData.AllyDataList[myDB.party[partyNum]].ID].Character[skillButtonNum - 39]].Detail[chLevel - 1];
                        chLevel++;
                        if (chLevel <= 5) {
                            chNew.text = rome[chLevel] + "　" + CharaData.CharaDataList[StatesData.MonsterDataList[AllyData.AllyDataList[myDB.party[partyNum]].ID].Character[skillButtonNum - 39]].Name;
                            chNew.text += "\n" + CharaData.CharaDataList[StatesData.MonsterDataList[AllyData.AllyDataList[myDB.party[partyNum]].ID].Character[skillButtonNum - 39]].Detail[chLevel - 1];
                            chCost.text = "↓　特性Pt " + chPt[chLevel-1] + " 消費";
                        } else {
                            chCost.text = "これ以上強化できません";
                            chNew.text = "";
                        }
                        chLevel--;
                        charaUp = chLevel <= 4 && chPt[chLevel] <= AllyData.AllyDataList[myDB.party[partyNum]].CharaPt;

                        if (charaUp) chButton.color = new Color(1, 1, 1, 1);
                        else chButton.color = new Color(1, 0.4f, 0.4f, 1);

                        charaButtonNum = -1;
                        while (mode != 7 || charaButtonNum == -1) yield return null;
                        if (charaButtonNum == 47) {
                            if (charaUp) {
                                playSE(13,0);
                                AllyData.AllyDataList[myDB.party[partyNum]].Character[skillButtonNum - 39]++;
                                AllyData.AllyDataList[myDB.party[partyNum]].CharaPt -= chPt[chLevel];
                                statesReload(myDB.party[partyNum]);
                            } else playSE(3, 0);
                        } else {
                            playSE(3,0);
                            charaWin.SetActive(false);
                            mode = 2;
                            break;
                        }
                    }
                }

            } else {//スキル
                if (skillButtonNum - 13 + skillStart <= skillCount) {
                    skillNum = AllyData.AllyDataList[myDB.party[partyNum]].Skill[skillButtonNum - 13 + skillStart];
                    if (SkillData.SkillDataList[skillNum].Map && AllyData.AllyDataList[myDB.party[partyNum]].NowSP >= SkillData.SkillDataList[skillNum].SP
                         && AllyData.AllyDataList[myDB.party[partyNum]].NowHP > 0) {
                        mode = 1;
                        playSE(4, 0);
                        skillUse = true;
                        skillWin.SetActive(false);
                        statesWin.SetActive(false);
                        for (int i = 0; i < 4; i++) {
                            if (myDB.party[i] >= 0) {
                                line[i].SetActive(true);
                                line[i].transform.position = posMonster[i];
                            }
                        }
                        skillNumber = skillNum;
                        StartCoroutine(Monster());
                    } else {
                        playSE(3, 0);
                    }
                }
            }
        }
    }
    IEnumerator skillUsing(int monsterNum,int targetNum,int skillNum) {
        int damage;
        if (AllyData.AllyDataList[myDB.party[monsterNum]].NowSP < SkillData.SkillDataList[skillNum].SP) {
            playSE(3,0);
            yield break;
        }
        
        if (skillNum >= 3 && skillNum <= 5 || skillNum>=82 && skillNum <= 84) {//ヒーラン系
            if (AllyData.AllyDataList[myDB.party[targetNum]].NowHP > 0) {
                playSE(6, 0);
                damage = DamageCaluculate(monsterNum, targetNum, 0, 100, 0);
                HPSPchange(targetNum, 0, damage);
                AllyData.AllyDataList[myDB.party[monsterNum]].NowSP -= SkillData.SkillDataList[skillNum].SP;
            } else playSE(3,0);
        }else if(skillNum >= 6 && skillNum <= 8) {//テトラヒーラン系
            playSE(6, 0);
            for (int i = 0; i < 4; i++) {
                if (myDB.party[i]>=0 && AllyData.AllyDataList[myDB.party[i]].NowHP > 0) {
                    damage = DamageCaluculate(monsterNum, i, 0, 70, 0);
                    HPSPchange(i, 0, damage);
                }
            }
            AllyData.AllyDataList[myDB.party[monsterNum]].NowSP -= SkillData.SkillDataList[skillNum].SP;
        } else if(skillNum == 9) {//リバイブ
            if (AllyData.AllyDataList[myDB.party[targetNum]].NowHP <= 0) {
                playSE(9, 0);
                damage = DamageCaluculate(monsterNum, targetNum, 0, 100, 0); ;
                HPSPchange(targetNum, 2, damage);
                AllyData.AllyDataList[myDB.party[monsterNum]].NowSP -= SkillData.SkillDataList[skillNum].SP;
            } else playSE(3,0);

        }


        for (int i = 0; i < 4; i++) {if (myDB.party[i] >= 0) HPSPcontroll(i);}
        skillReset = true;
        yield return null;
    }
    int DamageCaluculate(int attack, int defence, int pRate, int mRate, int dRate) {//ダメージ計算
        float pAttack, mAttack;
        float pDefence, mDefence;
        int pDamage, mDamage;
        int damage;
        pAttack = AllyData.AllyDataList[myDB.party[attack]].Attack;
        mAttack = AllyData.AllyDataList[myDB.party[attack]].Magic;
        pDefence = (int)Mathf.Floor(AllyData.AllyDataList[myDB.party[defence]].Defence * dRate / 100f);
        mDefence = (int)Mathf.Floor(AllyData.AllyDataList[myDB.party[defence]].MagicDef * dRate / 100f);
        if (pDefence <= 0) pDefence = 1;
        if (mDefence <= 0) mDefence = 1;
        pDamage = (int)Mathf.Floor(pAttack * pAttack / (pDefence + pAttack));
        mDamage = (int)Mathf.Floor(mAttack * mAttack / (mDefence + mAttack));
        damage = (int)Mathf.Floor(pDamage * pRate / 100 + mDamage * mRate / 100);
        return damage;
    }
    //////////////////////////////////////////////////////////////////////////////////////////
    //↓道具//////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////////////////
    IEnumerator item() {//道具選択
        int itemNum;
        string possess;
        itemStart = 0;
        itemFlag = true;
    itemSelect://ラベル//

        int itemCount = -1;
        for (int i = 0; i < ItemData.ItemDataList.Count; i++) {
            if (ItemData.ItemDataList[i].Possess > 0) {
                itemCount++;
                itemList[itemCount] = i;
            }
        }
        if (itemStart > itemCount) itemStart -= 10;
        if (itemStart < 0) itemStart = 0;
        itemReset = false;
        skillButtonNum = -1;

        for (int i = 0; i < 10; i++) {
            if (i + itemStart < itemList.GetLength(0) && i + itemStart<=itemCount) {
                Button[i + 13].GetComponent<Text>().text = ItemData.ItemDataList[itemList[i + itemStart]].Name;
                possess = "x"+ ItemData.ItemDataList[itemList[i + itemStart]].Possess.ToString();
                if (ItemData.ItemDataList[itemList[i + itemStart]].Map) {
                    Button[i + 13].GetComponent<Text>().color = new Color(1, 1, 1, 1);
                    Button[i + 13].transform.Find("SP").gameObject.GetComponent<Text>().color = new Color(1, 1, 1, 1);
                } else {
                    Button[i + 13].transform.Find("SP").gameObject.GetComponent<Text>().color = new Color(0.5f, 0.5f, 0.6f, 1);
                    Button[i + 13].GetComponent<Text>().color = new Color(0.5f, 0.5f, 0.6f, 1);
                }
                Button[i + 13].transform.Find("SP").gameObject.GetComponent<Text>().text = possess;
            } else {
                Button[i + 13].GetComponent<Text>().text = "";
                Button[i + 13].transform.Find("SP").gameObject.GetComponent<Text>().text = "";
            }

        }
        if (itemCount+1 <= 10) {
            Button[24].SetActive(false);
            Button[25].SetActive(false);
        } else {
            Button[24].SetActive(true);
            Button[25].SetActive(true);
        }
        while (true) {
            skillButtonNum = -1;
            while (mode != 2 || skillButtonNum == -1) {
                if (itemReset) goto itemSelect;
                yield return null;
            }
            if ((skillButtonNum == 24 || skillButtonNum == 25) && itemCount <= 9) skillButtonNum = -1;
            if (skillButtonNum == 23) {//戻る
                playSE(3, 0);
                skillWin.SetActive(false);
                monsterWin.SetActive(false);
                mode = 0;
                itemFlag = false;
                yield break;
            } else if (skillButtonNum == 24) {//←ボタン
                playSE(5, 0);
                itemStart -= 10;
                if (itemStart < 0) itemStart = itemCount - itemCount % 10;
                goto itemSelect;
            } else if (skillButtonNum == 25) {//→ボタン
                playSE(5, 0);
                itemStart += 10;
                if (itemStart > itemCount) itemStart = 0;
                goto itemSelect;
            } else if(skillButtonNum>=0){//道具
                if (skillButtonNum - 13 + itemStart <= itemCount) {
                    itemNum = itemList[skillButtonNum - 13 + itemStart];
                    if (ItemData.ItemDataList[itemNum].Map && ItemData.ItemDataList[itemNum].Possess > 0) {
                        if (ItemData.ItemDataList[itemNum].Type == -1) {
                            StartCoroutine(itemUsing(0, itemNum));
                            goto itemSelect;
                        } else {
                            playSE(4, 0);
                            mode = 1;
                            itemUse = true;
                            skillWin.SetActive(false);
                            monsterWin.SetActive(true);
                            itemNumber = itemNum;
                            StartCoroutine(Monster());
                        }
                    } else playSE(3, 0);
                }
            }
        }
    }
    IEnumerator itemUsing(int targetNum, int itemNum) {
        if (ItemData.ItemDataList[itemNum].Possess <= 0) {
            playSE(3, 0);
            yield break;
        }
        if (ItemData.ItemDataList[itemNum].Type==-2) {//装備品
            playSE(12, 0);
            equip(targetNum,itemNum);
            itemBreak = true;
        }else if (itemNum>=0 && itemNum<=5) {//HPSP薬
            int[] damage = { 30,100,200,30,100,200};
            if (AllyData.AllyDataList[myDB.party[targetNum]].NowHP > 0) {
                playSE(6+itemNum%3, 0);
                HPSPchange(targetNum, itemNum<=2?0:1, damage[itemNum]);
                ItemData.ItemDataList[itemNum].Possess -= 1;
            } else playSE(3, 0);
        } else if (itemNum == 6 || itemNum == 7) {//復活薬
            int damage;
            if (AllyData.AllyDataList[myDB.party[targetNum]].NowHP <= 0) {
                playSE(9, 0);
                damage = AllyData.AllyDataList[myDB.party[targetNum]].HP;
                if (itemNum == 6) damage = (int)Mathf.Floor(damage / 2f);
                HPSPchange(targetNum, 2, damage);
                ItemData.ItemDataList[itemNum].Possess -= 1;
            } else playSE(3, 0);

        } else if (itemNum == 9 || itemNum == 10) {//モンスター除け/寄せ
            playSE(4, 0);
            ItemData.ItemDataList[itemNum].Possess -= 1;
            if (itemNum == 9) {
                SysDB.encItem = 1600;
                SysDB.encount = 20000;
                yield return StartCoroutine(messageShow("しばらくモンスターが出現しにくくなった。"));
            } else {
                SysDB.encItem = 1600;
                SysDB.encount = 100;
                yield return StartCoroutine(messageShow("しばらくモンスターが出現しやすくなった。"));
            }
        }

        for (int i = 0; i < 4; i++) { if (myDB.party[i] >= 0) HPSPcontroll(i); }
        skillReset = true;
        if (ItemData.ItemDataList[itemNum].Possess <= 0) itemBreak = true;
        yield return null;
    }
    void equip(int target,int item) {
        int eqNum = AllyData.AllyDataList[myDB.party[target]].Equip;
        int HP, SP, Atk, Def, Mgc, Mgd, Spe;
        if (eqNum >= 0)ItemData.ItemDataList[eqNum].Possess++;
        if(item>=0)ItemData.ItemDataList[item].Possess--;
        AllyData.AllyDataList[myDB.party[target]].Equip = item;
        for (int i = 0; i <= 1; i++) {
            if (i == 1) item = eqNum;
            HP = SP = Atk = Def = Mgc = Mgd = Spe = 0;

            if (item == 21)Atk = 30;//活性化の剣
            if(i == 1) {HP *= -1;SP *= -1;Atk *= -1;Def *= -1;Mgc *= -1;Mgd *= -1;Spe *= -1;}
            AllyData.AllyDataList[myDB.party[target]].HP += HP;
            AllyData.AllyDataList[myDB.party[target]].NowHP += HP;
            AllyData.AllyDataList[myDB.party[target]].SP += SP;
            AllyData.AllyDataList[myDB.party[target]].NowSP += SP;
            AllyData.AllyDataList[myDB.party[target]].Attack += Atk;
            AllyData.AllyDataList[myDB.party[target]].Defence += Def;
            AllyData.AllyDataList[myDB.party[target]].Magic += Mgc;
            AllyData.AllyDataList[myDB.party[target]].MagicDef += Mgd;
            AllyData.AllyDataList[myDB.party[target]].Speed += Spe;
        }
    }
    //////////////////////////////////////////////////////////////////////////////////////////
    //↓セーブ//////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////////////////
    IEnumerator save() {

    saveSelect://ラベル//
        int monsterNum,level,mapNum;
        string nameList="";
        string levelList="";
        if (PlayerPrefs.HasKey("PlayerName")) {
            mapNum = GetComponent<SaveLoad>().mapNum(PlayerPrefs.GetString("SceneName"));
            saveWin.transform.Find("OtherData").GetComponent<Text>().text
                = "セーブデータ\n\n名前：" + PlayerPrefs.GetString("PlayerName") + "\n"
                + "所持金：" + PlayerPrefs.GetInt("Money").ToString() + "G\n";
            if (mapNum >= 0) {
                saveWin.transform.Find("OtherData").GetComponent<Text>().text += "場所：" + SaveData.SaveDataList[mapNum].spotName;
            }
            for (int i = 0; i < 4; i++) {
                monsterNum = PlayerPrefs.GetInt("AllyParty" + i.ToString());
                if (monsterNum >= 0) {
                    nameList += StatesData.MonsterDataList[PlayerPrefs.GetInt("AllyStates" + i.ToString() + "ID")].Name;
                    level = PlayerPrefs.GetInt("AllyStates" + i.ToString() + "Level");
                    levelList += "Level.";
                    if (level < 100) levelList += " ";
                    if (level <  10) levelList += " ";
                    levelList += level.ToString();
                }
                nameList += "\n";
                levelList += "\n";
            }
            saveWin.transform.Find("MonsterLevel").GetComponent<Text>().text = levelList;
            saveWin.transform.Find("MonsterName").GetComponent<Text>().text = nameList;
        } else {
            saveWin.transform.Find("OtherData").GetComponent<Text>().text = "セーブデータ\n\n なし";
            saveWin.transform.Find("MonsterLevel").GetComponent<Text>().text = "";
            saveWin.transform.Find("MonsterName").GetComponent<Text>().text = "";
        }
        while (true) {
            saveButtonNum = -1;
            while (mode != 3 || saveButtonNum == -1) {yield return null;}
            if (saveButtonNum == 26) {//戻る
                //PlayerPrefs.DeleteAll();
                playSE(3, 0);
                saveWin.SetActive(false);
                mode = 0;
                yield break;
            } else if (saveButtonNum == 27) {//セーブ
                playSE(4, 0);
                saveBlack.SetActive(true);
                yield return new WaitForSeconds(60 / 60f);
                GetComponent<SaveLoad>().Save();
                saveBlack.SetActive(false);
                playSE(10, 0);
                goto saveSelect;
            } else if (saveButtonNum == 28) {//タイトルへ
                playSE(4, 0);
                alertWin.SetActive(true);
                alertWin.transform.Find("Text").GetComponent<Text>().text = "本当にタイトルに\n戻りますか？";
                mode = 4;
                StartCoroutine(alert());
            }
        }
    }
    IEnumerator alert() {
        while (true) {
            alertButtonNum = -1;
            while (mode != 4 || alertButtonNum == -1) { yield return null; }
            if (alertButtonNum == 30) {//戻る
                playSE(3, 0);
                alertWin.SetActive(false);
                mode = 3;
                yield break;
            } else if (alertButtonNum == 29) {//タイトルへ
                playSE(4, 0);
                yield return new WaitForSeconds(10 / 60f);
                StartCoroutine(BGMFadeOut());
                fade.GetComponent<Fade>().FadeIn(1, 1.0f);
                yield return new WaitForSeconds(150 / 60f);
                SceneManager.LoadScene("Scenes/Title");
            }
        }
    }
    IEnumerator BGMFadeOut() {
        AudioSource aud = GameObject.Find("Main Camera").GetComponent<AudioSource>();
        int frame = 130;
        float volume = aud.volume;
        for(int i = frame; i >= 0; i--) {
            aud.volume = (float)i / frame*volume;
            yield return null;
        }
        
    }
    //////////////////////////////////////////////////////////////////////////////////////////
    //↓移動//////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////////////////
    IEnumerator jump() {
        int count = 0, dataNum;
        int jumpPage = 0;
        for (int i = 0; i < jumpData.JumpDataList.Count; i++) {
            if (jumpData.JumpDataList[i].movable) {
                jumpList[count] = jumpData.JumpDataList[i];
                count++;
            }
        }
        if (count <= 5) {
            Button[37].SetActive(false);
            Button[38].SetActive(false);
        } else {
            Button[37].SetActive(true);
            Button[38].SetActive(true);
        }
        while (true) {
            for(int i = jumpPage*5; i< jumpPage*5+5; i++) {
                if(i<count)Button[i-jumpPage*5 + 31].GetComponent<Text>().text = jumpList[i].Name;
                else Button[i - jumpPage * 5 + 31].GetComponent<Text>().text = "";
            }
            jumpButtonNum = -1;
            while (mode != 5 || jumpButtonNum == -1) { yield return null; }
            if ((jumpButtonNum == 37 || jumpButtonNum == 38) && count <= 5) continue;
            if (jumpButtonNum == 36) {//戻る
                playSE(3, 0);
                jumpWin.SetActive(false);
                mode = 0;
                yield break;
            } else if (jumpButtonNum == 37) {//←
                playSE(5, 0);
                jumpPage -= 1;
                if (jumpPage < 0) jumpPage = (count-1 - (count-1) % 5)/5;
            } else if (jumpButtonNum == 38) {//→
                playSE(5, 0);
                jumpPage += 1;
                if (jumpPage*5 >= count) jumpPage = 0;
            } else {//ボタン
                dataNum = jumpButtonNum - 31 + jumpPage * 5;
                if (dataNum>=0&&dataNum<count&&jumpList[dataNum].movable) {
                    playSE(4, 0);
                    alertWin.SetActive(true);
                    alertWin.transform.Find("Text").GetComponent<Text>().text = jumpList[dataNum].Name + "\nに移動しますか？";
                    mode = 4;
                    StartCoroutine(mapJump(dataNum));
                }
            }
        }
    }
    IEnumerator mapJump(int dataNum) {
        GameObject player; 
        alertButtonNum = -1;
        while (mode != 4 || alertButtonNum == -1) { yield return null; }
        if (alertButtonNum == 30) {//いいえ
            playSE(3, 0);
            alertWin.SetActive(false);
            mode = 5;
            yield break;
        } else if (alertButtonNum == 29) {//はい
            playSE(11, 0);
            yield return new WaitForSeconds(10 / 60f);
            SysDB.hasuFlag = false;
            fade.GetComponent<Fade>().FadeIn(1, 1.0f);
            fadeGameCon.GetComponent<Fade>().FadeIn(1, 1.0f);
            yield return new WaitForSeconds(150 / 60f);
            player = GameObject.Find("Player");
            if (SysDB.sceneName != jumpList[dataNum].MapName) {
                SceneManager.UnloadSceneAsync("Scenes/"+ SysDB.sceneName);
                SceneManager.LoadScene("Scenes/" + jumpList[dataNum].MapName, LoadSceneMode.Additive);
                SysDB.sceneName = jumpList[dataNum].MapName;
            } else {
                player.transform.position = jumpList[dataNum].position;
                GameObject.Find("Grid").GetComponent<MapEnter>().bgm();
                SysDB.sceneName = jumpList[dataNum].MapName;
                SceneManager.SetActiveScene(SceneManager.GetSceneByName(SysDB.sceneName));
            }
            player.transform.position = jumpList[dataNum].position;
            player.GetComponent<PlayerMove>().playerDirection(new Vector2(0,-1));
            SysDB.eventFlag = false;
            SysDB.menueFlag = false;
           
            fade.GetComponent<Fade>().FadeOut(1, 0);
            fadeGameCon.GetComponent<Fade>().FadeOut(1, 0.5f);
            SceneManager.UnloadSceneAsync("Scenes/Menue");
            yield return new WaitForSeconds(60 / 60f);
            yield break;
        }

    }
    //////////////////////////////////////////////////////////////////////////////////////////
    //↓ファイル//////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////////////////
    IEnumerator file() {
        int[] fileID;
        bool[] getFlag,fileFlag;
        Text unknown = fileWin.transform.Find("UnknownImage").GetComponent<Text>();
        MonsterData mon;
        int fileCount = 0,getCount=0,page=0,pageMax,monID;
        for (int i = 0; i < StatesData.FileFlag.Length; i++) {
            if (StatesData.FileFlag[i]|| StatesData.RegistFlag[i]) fileCount++;
            if (StatesData.GetFlag[i]) getCount++;
        }

        pageMax = (fileCount - 1) / 8+1;
        fileID = new int[fileCount];
        fileFlag = new bool[fileCount];
        getFlag = new bool[fileCount];
        fileCount = 0;
        for (int i = 0; i < StatesData.FileFlag.Length; i++) {
            if (StatesData.FileFlag[i] || StatesData.RegistFlag[i]) {
                fileID[fileCount] = StatesData.MonsterFile[i];
                getFlag[fileCount] = StatesData.GetFlag[i];
                fileFlag[fileCount] = StatesData.FileFlag[i];
                fileCount++;
            }
        }

        if (fileCount <= 8) {
            Button[58].SetActive(false);
            Button[59].SetActive(false);
        } else {
            Button[58].SetActive(true);
            Button[59].SetActive(true);
        }

        fiExpe.text = "";
        fiChara.text = "";
        fileImage.rectTransform.sizeDelta = new Vector2(0,0);
        fileComp.text = (100*getCount/StatesData.FileFlag.Length).ToString()+"%";
        GameObject.Find("TextFile").GetComponent<Text>().text = "モンスターファイル　コンプリート率:     "+ getCount + "種";
        StartCoroutine(anim());
        while (true) {
            for(int i = 0; i < 8; i++) {
                if (page * 8 + i >= fileCount) fileText[i].text = "";
                else {
                    fileText[i].text = StatesData.MonsterDataList[fileID[page * 8 + i]].Name;
                    if (getFlag[page*8+i]) fileText[i].color = new Color(1,1,1,1);
                    else fileText[i].color = new Color(0.5f, 0.5f, 0.7f, 1);
                }
            }
            fileButtonNum = -1;
            while (mode != 8 || fileButtonNum == -1) yield return null;
            if ((fileButtonNum == 58 || fileButtonNum == 59) && fileCount <= 8) continue;
            if (fileButtonNum == 57) {//戻る
                playSE(3, 0);
                fileWin.SetActive(false);
                mode = 0;
                yield break;
            }else if(fileButtonNum == 58 || fileButtonNum==59) {//矢印
                playSE(5, 0);
                page = (page + pageMax + fileButtonNum*2-117) % pageMax;
            } else if(fileButtonNum<=56 && fileButtonNum>=49 && fileText[fileButtonNum-49].text!="") {//詳細
                playSE(4, 0);
                int fileIndex = fileButtonNum - 49 + page * 8;
                monID = fileID[fileIndex];
                mon = StatesData.MonsterDataList[monID];

                if (getFlag[fileIndex]) {
                    fiExpe.text = "<color='#a0a0ff'>" + mon.Name + "</color>\n" + mon.Formula + "\n\n" + mon.Detail + "\n\n" + mon.howMake;
                    fiChara.text = "[特性]\n";
                    for (int i = 0; i < mon.Character.Length; i++) {
                        fiChara.text += CharaData.CharaDataList[mon.Character[i]].Name + "\n";
                    }
                } else {
                    fiExpe.text = "<color='#a0a0ff'>" + mon.Name + "</color>\n" + mon.Formula + "\n\n" + mon.howMake;
                    fiChara.text = "";
                }

                sprite = StatesData.MonsterDataList[monID].FileName.GetComponent<SpriteRenderer>().sprite;
                fileImage.sprite = sprite;
                if (getFlag[fileIndex] || fileFlag[fileIndex]) {
                    fileImage.rectTransform.sizeDelta = new Vector2(sprite.rect.width, sprite.rect.height) * 1.3f;
                    unknown.text = "";
                } else {
                    fileImage.rectTransform.sizeDelta = new Vector2(0, 0);
                    unknown.text = "? ? ?";
                }
                fileImage.GetComponent<Animator>().runtimeAnimatorController
                    = StatesData.MonsterDataList[monID].FileName.GetComponent<Animator>().runtimeAnimatorController;

            }

        }
    }
    IEnumerator anim() {
        SpriteRenderer sp;
        sp = fileImage.GetComponent<SpriteRenderer>();
        while (mode == 8) {
            fileImage.sprite = sp.sprite;
            yield return null;
        }
    }
    //////////////////////////////////////////////////////////////////////////////////////////
    //↓その他システム//////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////////////////
    void HPSPchange(int num,int mode,int damage) {//HPSP増減
        //mode 0:HP 1:SP 2:HP蘇生
        if (mode == 0 || mode == 2) {
            if(myDB.party[num] >= 0 && (mode==2 || AllyData.AllyDataList[myDB.party[num]].NowHP > 0)) {
                AllyData.AllyDataList[myDB.party[num]].NowHP += damage;
                if(AllyData.AllyDataList[myDB.party[num]].NowHP>= AllyData.AllyDataList[myDB.party[num]].HP) {
                    AllyData.AllyDataList[myDB.party[num]].NowHP = AllyData.AllyDataList[myDB.party[num]].HP;
                }
            }
        }else if(mode == 1) {
            if (myDB.party[num] >= 0 && AllyData.AllyDataList[myDB.party[num]].NowHP > 0) {
                AllyData.AllyDataList[myDB.party[num]].NowSP += damage;
                if (AllyData.AllyDataList[myDB.party[num]].NowSP >= AllyData.AllyDataList[myDB.party[num]].SP) {
                    AllyData.AllyDataList[myDB.party[num]].NowSP = AllyData.AllyDataList[myDB.party[num]].SP;
                }
            }
        }
    }
    void HPSPcontroll(int num) {//HPSP更新
        Color color;
        float Pnow, Pmax;

        Pnow = AllyData.AllyDataList[myDB.party[num]].NowHP;
        Pmax = AllyData.AllyDataList[myDB.party[num]].HP;
        line[num].transform.Find("BackBarHP").Find("HPvalue").GetComponent<Text>().text
            = Pnow.ToString() + "/" + Pmax.ToString();
        line[num].transform.Find("BackBarHP").Find("BarHP").transform.localScale = new Vector3(Pnow / Pmax, 1, 1);
        if (Pnow / Pmax > 0.5) color = new Color(0.3f, 1.0f, 0.3f);
        else if (Pnow / Pmax > 0.2) color = new Color(1.0f, 1.0f, 0.3f);
        else color = new Color(1.0f, 0.3f, 0.3f);
        line[num].transform.Find("BackBarHP").Find("BarHP").GetComponent<Image>().color = color;

        Pnow = AllyData.AllyDataList[myDB.party[num]].NowSP;
        Pmax = AllyData.AllyDataList[myDB.party[num]].SP;
        line[num].transform.Find("BackBarSP").Find("SPvalue").GetComponent<Text>().text
            = Pnow.ToString() + "/" + Pmax.ToString();
        line[num].transform.Find("BackBarSP").Find("BarSP").transform.localScale = new Vector3(Pnow / Pmax, 1, 1);

    }
    IEnumerator messageShow(string mes) {
        int preMode;
        message.text = mes;
        messageWin.SetActive(true);
        preMode = mode;
        mode = -1;
        while (Input.GetKeyDown(KeyCode.Z) || Input.GetMouseButton(0)) yield return null;
        while (!Input.GetKeyDown(KeyCode.Z) && !Input.GetMouseButton(0)) yield return null;
        while (Input.GetKeyDown(KeyCode.Z) || Input.GetMouseButton(0)) yield return null;
        messageWin.SetActive(false);
        skillButtonNum = -1;
        mode = preMode;
        yield return null;
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
        bool monsterFlag,skillFlag,jumpFlag,equipFlag,charaFlag,fileFlag;
        if (mode==0 && (Input.GetMouseButtonUp(0) || Input.GetMouseButton(0))) {//メインメニュー
            mousePos = Input.mousePosition;
            for (int i = 0; i < 7; i++) {
                if (Input.GetMouseButtonUp(0)) {
                    if (buttonRe[i].Contains(mousePos)) buttonNum = i;
                    Button[i].GetComponent<Image>().color = new Color(0.5f, 0.56f, 1, 1);
                }else if (Input.GetMouseButton(0)) {
                    if (buttonRe[i].Contains(mousePos)) {
                        Button[i].GetComponent<Image>().color = new Color(1, 1, 1, 1);
                    } else if (Button[i] != null) Button[i].GetComponent<Image>().color = new Color(0.5f, 0.56f, 1, 1);
                }
            }
        }
        if (mode == 1 && (Input.GetMouseButtonUp(0) || Input.GetMouseButton(0))) {//モンスター
            mousePos = Input.mousePosition;
            monsterFlag = false;
            for (int i = 7; i < 12; i++) {
                if (Input.GetMouseButtonUp(0)) {
                    if (buttonRe[i].Contains(mousePos)) monsterButtonNum = i;
                    if(i==11)Button[i].GetComponent<Image>().color = new Color(0.5f, 0.56f, 1, 1);
                } else if (Input.GetMouseButton(0)) {
                    if (buttonRe[i].Contains(mousePos)) {
                        if(i<=10 && myDB.party[i-7]>=0)monsterFlag = true;
                        if (i == 11) Button[i].GetComponent<Image>().color = new Color(1, 1, 1, 1);
                        else monsterCursor.transform.position = Button[i].transform.position-new Vector3(5,0,0);
                    } else if (Button[i] != null) {
                        if (i == 11) Button[i].GetComponent<Image>().color = new Color(0.5f, 0.56f, 1, 1);
                    }
                }
            }
            if (!monsterFlag) monsterCursor.transform.position = new Vector3(-1000,0,0);
        }
        if (mode == 2 && (Input.GetMouseButtonUp(0) || Input.GetMouseButton(0))) {//スキル
            mousePos = Input.mousePosition;
            skillFlag = false;
            for (int i = 13; i < 26; i++) {
                if (Input.GetMouseButtonUp(0)) {
                    if (buttonRe[i].Contains(mousePos)) skillButtonNum = i;
                    if (i >= 23 && i <= 25) Button[i].GetComponent<Image>().color = new Color(0.5f, 0.56f, 1, 1);
                } else if (Input.GetMouseButton(0)) {
                    if (buttonRe[i].Contains(mousePos)) {
                        if (i >= 23 && i <= 25) Button[i].GetComponent<Image>().color = new Color(1, 1, 1, 1);
                        else {
                            if (Button[i].transform.Find("SP").GetComponent<Text>().text != "") {
                                if(itemFlag) skillDetail.text = ItemData.ItemDataList[itemList[i - 13 + itemStart]].Detail;
                                else skillDetail.text = SkillData.SkillDataList[AllyData.AllyDataList[myDB.party[party]].Skill[i - 13 + skillStart]].Detail;
                                skillCursor.transform.position = Button[i].transform.position + new Vector3(0, 2, 0);
                                skillFlag = true;
                            }
                        }
                    } else if (Button[i] != null) {
                        if (i >= 23 && i <= 25) Button[i].GetComponent<Image>().color = new Color(0.5f, 0.56f, 1, 1);
                    }
                }
            }
            equipFlag = false;
            for (int i = 12; i < 13; i++) {
                if (Input.GetMouseButtonUp(0)) {
                    if (buttonRe[i].Contains(mousePos)) skillButtonNum = i;
                } else if (Input.GetMouseButton(0)) {
                    if (buttonRe[i].Contains(mousePos)) equipFlag = true;
                }
                if (equipFlag)equipText.color=new Color(0.6f,0.6f,1,1);
                else equipText.color = new Color(1, 1, 1, 1);
            }
            for (int i = 39; i < 47; i++) {
                charaFlag = false;
                if (Input.GetMouseButtonUp(0)) {
                    if (buttonRe[i].Contains(mousePos)) skillButtonNum = i;
                } else if (Input.GetMouseButton(0)) {
                    if (buttonRe[i].Contains(mousePos)) charaFlag = true;
                }
                if (charaFlag) charaText[i-39].color = new Color(0.6f, 0.6f, 1, 1);
                else charaText[i-39].color = new Color(1, 1, 1, 1);
            }
            if (!skillFlag) {
                skillCursor.transform.position = new Vector3(-1000, 0, 0);
                skillDetail.text = "";
            }
        }
        if (mode == 3 && (Input.GetMouseButtonUp(0) || Input.GetMouseButton(0))) {//セーブ
            mousePos = Input.mousePosition;
            for (int i = 26; i < 29; i++) {
                if (Input.GetMouseButtonUp(0)) {
                    if (buttonRe[i].Contains(mousePos)) saveButtonNum = i;
                    Button[i].GetComponent<Image>().color = new Color(0.5f, 0.56f, 1, 1);
                } else if (Input.GetMouseButton(0)) {
                    if (buttonRe[i].Contains(mousePos)) {
                        Button[i].GetComponent<Image>().color = new Color(1, 1, 1, 1);
                    } else if (Button[i] != null) Button[i].GetComponent<Image>().color = new Color(0.5f, 0.56f, 1, 1);
                }
            }
        }
        if (mode == 4 && (Input.GetMouseButtonUp(0) || Input.GetMouseButton(0))) {//セーブ
            mousePos = Input.mousePosition;
            for (int i = 29; i < 31; i++) {
                if (Input.GetMouseButtonUp(0)) {
                    if (buttonRe[i].Contains(mousePos)) alertButtonNum = i;
                    Button[i].GetComponent<Image>().color = new Color(0.5f, 0.56f, 1, 1);
                } else if (Input.GetMouseButton(0)) {
                    if (buttonRe[i].Contains(mousePos)) {
                        Button[i].GetComponent<Image>().color = new Color(1, 1, 1, 1);
                    } else if (Button[i] != null) Button[i].GetComponent<Image>().color = new Color(0.5f, 0.56f, 1, 1);
                }
            }
        }
        if (mode == 5 && (Input.GetMouseButtonUp(0) || Input.GetMouseButton(0))) {//ジャンプ
            mousePos = Input.mousePosition;
            jumpFlag = false;
            for (int i = 31; i < 39; i++) {
                if (Input.GetMouseButtonUp(0)) {
                    if (buttonRe[i].Contains(mousePos)) jumpButtonNum = i;
                    if (i >= 36 && i <= 38) Button[i].GetComponent<Image>().color = new Color(0.5f, 0.56f, 1, 1);
                } else if (Input.GetMouseButton(0)) {
                    if (buttonRe[i].Contains(mousePos)) {
                        if (i >= 36 && i <= 38) Button[i].GetComponent<Image>().color = new Color(1, 1, 1, 1);
                        else {
                            if (Button[i].GetComponent<Text>().text != "") {
                                jumpCursor.transform.position = Button[i].transform.position + new Vector3(0, 0, 0);
                                jumpFlag = true;
                            }
                        }
                    } else if (Button[i] != null) {
                        if (i >= 36 && i <= 38) Button[i].GetComponent<Image>().color = new Color(0.5f, 0.56f, 1, 1);
                    }
                }
            }
            if (!jumpFlag)jumpCursor.transform.position = new Vector3(-1000, 0, 0);
        }
        if (mode == 7 && (Input.GetMouseButtonUp(0) || Input.GetMouseButton(0))) {//特性
            mousePos = Input.mousePosition;
            for (int i = 47; i < 49; i++) {
                if (Input.GetMouseButtonUp(0)) {
                    if (buttonRe[i].Contains(mousePos)) charaButtonNum = i;
                    Button[i].GetComponent<Image>().color = new Color(0.5f, 0.56f, 1, 1);
                } else if (Input.GetMouseButton(0)) {
                    if (buttonRe[i].Contains(mousePos)) {
                        Button[i].GetComponent<Image>().color = new Color(1, 1, 1, 1);
                    } else if (Button[i] != null) Button[i].GetComponent<Image>().color = new Color(0.5f, 0.56f, 1, 1);
                }
            }
        }
        if (mode == 8 && (Input.GetMouseButtonUp(0) || Input.GetMouseButton(0))) {//スキル
            mousePos = Input.mousePosition;
            fileFlag = false;
            for (int i = 49; i < 60; i++) {
                if (Input.GetMouseButtonUp(0)) {
                    if (buttonRe[i].Contains(mousePos)) fileButtonNum = i;
                    if (i >= 57 && i <= 59) Button[i].GetComponent<Image>().color = new Color(0.5f, 0.56f, 1, 1);
                } else if (Input.GetMouseButton(0)) {
                    if (buttonRe[i].Contains(mousePos)) {
                        if (i >= 57 && i <= 59) Button[i].GetComponent<Image>().color = new Color(1, 1, 1, 1);
                        else {
                            if (fileText[i-49].text != "") {
                                fileCursor.transform.position = Button[i].transform.position + new Vector3(0, 2, 0);
                                fileFlag = true;
                            }
                        }
                    } else if (Button[i] != null) {
                        if (i >= 57 && i <= 59) Button[i].GetComponent<Image>().color = new Color(0.5f, 0.56f, 1, 1);
                    }
                }
            }
            if (!fileFlag)fileCursor.transform.position = new Vector3(-1000, 0, 0);
        }

    }

}