using System;
using System.Text;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class BattleController : MonoBehaviour{
    public MonsterStates StatesData;
    public ItemDatas ItemData;
    public SkillDatas SkillData;
    public EnemyDatas EnemyData;
    public AllyDatas AllyData;
    public BattleDB BatDB;
    public GameObject[] Effect;
    public AudioClip[] BGM;
    public AudioClip[] Intro;
    public AudioClip[] SE;
    public Sprite[] BackGround;
    NetData netCon;

    int[] itemID = new int[100];
    int[,] preCommand = new int[4, 2];
    int moveCount=0;
    int monsterSize = 2;
    int move = 7;
    int buttonNum = -1;
    int skillButtonNum = -1;
    int commandButtonNum = -1;
    int selectButton;
    int skillCount;
    int skillStart;
    int partyNum;
    int result;
    int tutorialNum = 0;
    int sendButtonNum;
    int getButtonNum;
    int me=0;
    int you=0;
    int time = 30;
    int[,] random;
    int[] randIndex;
    bool tutorial;
    bool sendCheck = false;
    bool controll = true;
    bool skillControll = false;
    bool itemControll = false;
    bool movingFlag = false;
    bool messageFlag = false;
    bool monsterSend = false;
    bool MonsterGet = false;
    bool loseFlag = false;
    bool atpFlag;
    bool leftRoom = false;
    bool timerFlag = true;
    bool forcetimerFlag = false;
    bool historyMemo = false;
    bool commandFlag = false;
    int[,] speed = new int[8, 2];
    string netResult,battleResult,dateResult,nameResult;

    AudioSource audioS, audioSC;
    GameObject[] monster;
    GameObject[] states;
    GameObject[] HPvar;
    GameObject[] HPvalue;
    GameObject[] SPvar;
    GameObject[] SPvalue;
    GameObject[] Button;
    GameObject[,] lines;
    GameObject cursorAlly;
    GameObject canvas;
    GameObject black1, black2,turn;
    GameObject[] skillButton;
    GameObject skillWindow;
    GameObject skillCursor;
    GameObject statesWindow;
    GameObject arrowLine;
    GameObject winWindow;
    GameObject M_win;
    GameObject loseWindow;
    GameObject connectingWin;
    GameObject timerWin;
    GameObject skillSelectWin;
    GameObject commandWin;
    GameObject commandCursor;
    GameObject[] commandButton;
    GameObject[] getButton;
    GameObject getWin;
    RectTransform canvasAsp;
    Image S_sprite;
    IEnumerator timer;
    IEnumerator commandCoroutine;
    SkillWindow[] skillName;
    Camera cameraBattle;
    Camera cameraAsp;
    Rect[] buttonRe;
    Rect[] skillButtonRe;
    Rect[] commandButtonRe;
    Rect[] getButtonRe;
    Image[] buttonImage;
    Image[] skillButtonImage;
    Image[] getButtonImage;
    Text detail;
    Text selectText;
    Text M_text;
    Text M_name;
    Text timerText;
    Vector2 mousePos;
    RectTransform itemMaker;
    Vector3[] itemMakerPos;
    string[,] tText= { { "アマルガ","戦闘開始です。\nまずは、画面下の「二酸化炭素」をタップして\n命令を与えましょう。"}
                      ,{ "アマルガ","まずは、画面下の「二酸化炭素」をタップして\n命令を与えましょう。"}
                      ,{ "アマルガ","相手に攻撃をしましょう。\n「通常攻撃」を選んでください。"}
                      ,{ "アマルガ","攻撃先の選択です。\n画面下の「タングステン」を選びましょう。"}
                      ,{ "アマルガ","これで命令が完了しました。\n「決定」を押して行動開始です！"}
                      ,{ "アマルガ","決定」を押して行動開始です！"}
                      ,{ "アマルガ","大まかな流れはこんな感じです。\nでは頑張って私のタングステンを倒してみて\nください！"}
                      ,{ "アマルガ","頑張って戦いましょう！"}
    };

    void Start(){//ゲーム初期設定
        
        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(2));
        audioS = GetComponent<AudioSource>();
        audioSC = GameObject.Find("Main Camera Battle").GetComponent<AudioSource>();
        if (SysDB.netFlag) {
            me = PhotonNetwork.IsMasterClient ? 1 : 0;
            you = 1-me;
            netCon = GameObject.Find("NetData").GetComponent<NetData>();
            random = new int[2, 100];
            randIndex = new int[2];
            randIndex[0] = randIndex[1] = 0;
        }
        if (Intro[SysDB.BGM] != null) {
            audioSC.clip = Intro[SysDB.BGM];
            audioSC.PlayScheduled(AudioSettings.dspTime);
            audioS.clip = BGM[SysDB.BGM];
            audioS.PlayScheduled(AudioSettings.dspTime + ((float)audioSC.clip.samples / (float)audioSC.clip.frequency));
        } else {
            audioS.clip = BGM[SysDB.BGM];
            audioS.Play();
        }
        tutorial = SysDB.Enemy == 0;
        GameObject.Find("Back").GetComponent<SpriteRenderer>().sprite = BackGround[SysDB.Back];
        
        GameObject.Find("Back").transform.localScale = new Vector3(1, 1, 1)* 640 / BackGround[SysDB.Back].rect.width * 1.837425f;

        BatDB.Group[SysDB.Enemy, 0] = (int)BatDB.EnemyGroup[SysDB.Enemy].x;
        BatDB.Group[SysDB.Enemy, 1] = (int)BatDB.EnemyGroup[SysDB.Enemy].y;
        BatDB.Group[SysDB.Enemy, 2] = (int)BatDB.EnemyGroup[SysDB.Enemy].z;
        BatDB.Group[SysDB.Enemy, 3] = (int)BatDB.EnemyGroup[SysDB.Enemy].w;

        for(int i = 0; i < 8; i++) {
            for(int j=0;j<BatDB.monsterBuff.GetLength(1);j++)BatDB.monsterBuff[i, j] = 0;
        }
        for(int i = 0; i < 4; i++) {
            preCommand[i, 0] = -1;
            preCommand[i, 1] = -1;
        }

        black1 = GameObject.Find("Black1");
        black2 = GameObject.Find("Black2");
        turn = GameObject.Find("turn");
        canvas = GameObject.Find("Canvas");
        canvasAsp = canvas.GetComponent<RectTransform>();
        cameraBattle = GameObject.Find("Main Camera Battle").GetComponent<Camera>();
        //cameraAsp = cameraBattle;
        cameraAsp = null;
        monster = new GameObject[8];
        states = new GameObject[8];
        HPvar = new GameObject[4];
        HPvalue = new GameObject[4];
        SPvar = new GameObject[4];
        SPvalue = new GameObject[4];
        Button = new GameObject[4];
        lines = new GameObject[4, 4];
        buttonRe = new Rect[12];
        buttonImage = new Image[4];
        getButton = new GameObject[2];
        getButtonRe = new Rect[2];
        getButtonImage = new Image[4];
        commandButton = new GameObject[4];
        commandButtonRe = new Rect[4];
        skillButton = new GameObject[13];
        skillButtonRe = new Rect[13];
        skillButtonImage = new Image[13];
        skillName = new SkillWindow[2];
        skillName[0] = GameObject.Find("skillNameAlly").GetComponent<SkillWindow>();
        skillName[1] = GameObject.Find("skillNameEnemy").GetComponent<SkillWindow>();
        skillWindow = GameObject.Find("SkillWindow");
        skillCursor = GameObject.Find("SkillCursor");
        skillSelectWin = GameObject.Find("SkillSelectWindow");
        commandWin = GameObject.Find("CommandWin");
        commandCursor = GameObject.Find("CommandCursor");
        statesWindow = GameObject.Find("BattleStatesWin");
        winWindow = GameObject.Find("WinWindow");
        getWin = GameObject.Find("GetWin");
        arrowLine = GameObject.Find("Line");
        cursorAlly = GameObject.Find("BattleCursorAlly");
        detail = GameObject.Find("textDetail").GetComponent<Text>();
        selectText = GameObject.Find("SelectText").GetComponent<Text>();
        M_win = GameObject.Find("MessageWin");
        M_text = M_win.transform.Find("Text").GetComponent<Text>();
        M_name = M_win.transform.Find("Name").GetComponent<Text>();
        loseWindow = GameObject.Find("LoseWindow");
        connectingWin = GameObject.Find("ConnectingWin");
        timerWin = GameObject.Find("TimerWin");
        timerText = timerWin.transform.Find("Text").GetComponent<Text>();
        itemMaker = GameObject.Find("ItemMaker").GetComponent<RectTransform>();
        S_sprite = loseWindow.GetComponent<Image>();
        BatDB.itemTarget = -1;
        BatDB.itemID = -1;
        BatDB.turn = 1;
        for(int i = 1; i <= 10; i++) {
            skillButton[i-1] = skillWindow.transform.Find("Skill"+i.ToString()).gameObject;
        }
        getButton[0] = getWin.transform.Find("YesButton").gameObject;
        getButton[1] = getWin.transform.Find("NoButton").gameObject;
        getButtonRe[0] = buttonRect(getButton[0]);
        getButtonRe[1] = buttonRect(getButton[1]);
        getButtonImage[0] = getButton[0].GetComponent<Image>();
        getButtonImage[1] = getButton[1].GetComponent<Image>();
        skillButton[10] = skillWindow.transform.Find("Cancel").gameObject;
        skillButton[11] = skillWindow.transform.Find("Left").gameObject;
        skillButton[12] = skillWindow.transform.Find("Right").gameObject;
        for (int i = 0; i < 13; i++) {
            skillButtonRe[i] = buttonRect(skillButton[i]);
            skillButtonImage[i] = skillButton[i].GetComponent<Image>();
        }
        for (int i = 0; i < 4; i++) {
            commandButton[i]= commandWin.transform.Find("commandButton"+(i+1).ToString()).gameObject;
            commandButtonRe[i] = buttonRect(commandButton[i]);
        }
        for (int i = 0; i < 8; i++) BatDB.priority[i] = 0;
        statesWindow.SetActive(false);
        skillWindow.SetActive(false);
        winWindow.SetActive(false);
        loseWindow.SetActive(false);
        M_win.SetActive(false);
        connectingWin.SetActive(false);
        timerWin.SetActive(false);
        skillSelectWin.SetActive(false);
        commandWin.SetActive(false);
        getWin.SetActive(false);
        StartCoroutine(BattleMain());
    }

    IEnumerator BattleMain() {//バトル進行
        Text allyName, enemyName;
        allyName = GameObject.Find("NetAllyName").GetComponent<Text>();
        enemyName = GameObject.Find("NetEnemyName").GetComponent<Text>();

        if (SysDB.netFlag) {//相手の読み込み待ち
            for (int i = 0; i < 100; i++) {
                int ran = SysDB.randomInt(1, 2000000000);
                netCon.rand[me, i] = ran;
                random[me, i] = ran;
            }

            dateResult = DateTime.Now.Year.ToString() + "/";
            if (DateTime.Now.Month < 10) dateResult += " ";
            dateResult += DateTime.Now.Month.ToString() + "/";
            if (DateTime.Now.Day < 10) dateResult += " ";
            dateResult += DateTime.Now.Day.ToString() + " ";
            if (DateTime.Now.Hour < 10) dateResult += "0";
            dateResult += DateTime.Now.Hour.ToString() + ":";
            if (DateTime.Now.Minute < 10) dateResult += "0";
            dateResult += DateTime.Now.Minute.ToString();
            nameResult = "(接続切れ)";
            netCon.ready[me] = true;
            yield return Waiting(4);
            netCon.mode = 0;
            yield return Waiting(0);
            netCon.mode = 1;
            netCon.ready[me] = false;
            allyName.text = myDB.playerName;
            enemyName.text = netCon.playerName[you];
            nameResult = enemyName.text;

            for (int i = 0; i < 100; i++) {
                random[you, i] = netCon.rand[you, i];
            }
        } else {
            allyName.text = "";
            enemyName.text = "";
        }

        string preName;
        MonsterData mondata;
        int lev;
        for (int i = 0; i <= 3; i++) {
            if (SysDB.netFlag) {
                AllyData netAlly = netCon.MonsterGet(i);
                if (netAlly != null) {
                    states[i] = GameObject.Find("StatesEnemy" + (i + 1).ToString());
                    monster[i] = StatesData.MonsterDataList[netAlly.ID].FileName;
                    monster[i] = Instantiate(monster[i], new Vector3(-10 - i % 2 - 1.8f - move, (3 - i) * 1.4f - 1, -2), Quaternion.identity);
                    monster[i].name = "Enemy" + (i + 1).ToString();
                    monster[i].transform.localScale = new Vector3(-1, 1, 1) * monsterSize;
                    preName = netAlly.Name;
                    GameObject.Find("EnemyName" + (i + 1).ToString()).GetComponent<Text>().text = preName;
                    BatDB.CharaName[i] = preName;
                    BatDB.behavior[i, 0] = -1;
                    BatDB.behavior[i, 1] = -1;
                    BatDB.States[i, 0] = netAlly.ID;
                    BatDB.death[i] = false;
                    BatDB.States[i, 1] = netAlly.Level;
                    BatDB.States[i, 2] = netAlly.HP;
                    BatDB.States[i, 3] = netAlly.HP;
                    BatDB.States[i, 4] = netAlly.SP;
                    BatDB.States[i, 5] = netAlly.SP;
                    BatDB.States[i, 6] = netAlly.Attack;
                    BatDB.States[i, 7] = netAlly.Defence;
                    BatDB.States[i, 8] = netAlly.Magic;
                    BatDB.States[i, 9] = netAlly.MagicDef;
                    BatDB.States[i, 10] = netAlly.Speed;
                    for (int j = 1; j <= 10; j++) BatDB.StatesCor[i, j] = BatDB.States[i, j];

                    HPSPcontroll(0, i, 0);
                    HPSPcontroll(1, i, 0);
                    if (BatDB.States[i, 3] == 0) {
                        monster[i].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
                        monster[i].transform.Find("shade").gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
                        BatDB.death[i] = true;
                    }

                    int netLength;
                    netLength = netAlly.Skill.Length;
                    Array.Resize(ref EnemyData.EnemyDataList[BatDB.Group[SysDB.Enemy, i]].Skill, netLength);
                    for (int j = 0; j < netLength; j++) {
                        EnemyData.EnemyDataList[BatDB.Group[SysDB.Enemy, i]].Skill[j] = netAlly.Skill[j];
                    }
                    netLength = netAlly.Character.Length;
                    Array.Resize(ref EnemyData.EnemyDataList[BatDB.Group[SysDB.Enemy, i]].Character,netLength);
                    for (int j = 0; j < netLength; j++) {
                        EnemyData.EnemyDataList[BatDB.Group[SysDB.Enemy, i]].Character[j] = netAlly.Character[j];
                    }
                }
            } else {
                if (BatDB.Group[SysDB.Enemy, i] >= 0) {
                    if (SysDB.Battle != 6) StatesData.Resister(EnemyData.EnemyDataList[BatDB.Group[SysDB.Enemy, i]].ID, true, false);
                    states[i] = GameObject.Find("StatesEnemy" + (i + 1).ToString());
                    mondata = StatesData.MonsterDataList[EnemyData.EnemyDataList[BatDB.Group[SysDB.Enemy, i]].ID];

                    int firstLevel = 10;

                    monster[i] = StatesData.MonsterDataList[EnemyData.EnemyDataList[BatDB.Group[SysDB.Enemy, i]].ID].FileNameMiror;
                    if (monster[i] == null) monster[i] = StatesData.MonsterDataList[EnemyData.EnemyDataList[BatDB.Group[SysDB.Enemy, i]].ID].FileName;
                    monster[i] = Instantiate(monster[i], new Vector3(-10 - i % 2 - 1.8f - move, (3 - i) * 1.4f - 1, -2), Quaternion.identity);
                    monster[i].name = "Enemy" + (i + 1).ToString();
                    monster[i].transform.localScale = new Vector3(-1, 1, 1) * monsterSize;
                    preName = EnemyData.EnemyDataList[BatDB.Group[SysDB.Enemy, i]].Name;
                    GameObject.Find("EnemyName" + (i + 1).ToString()).GetComponent<Text>().text = preName;
                    BatDB.CharaName[i] = preName;
                    BatDB.behavior[i, 0] = -1;
                    BatDB.behavior[i, 1] = -1;
                    BatDB.States[i, 0] = EnemyData.EnemyDataList[BatDB.Group[SysDB.Enemy, i]].ID;
                    BatDB.death[i] = false;
                    BatDB.States[i, 1] = EnemyData.EnemyDataList[BatDB.Group[SysDB.Enemy, i]].Level;
                    lev = BatDB.States[i, 1];

                    BatDB.States[i, 2] = EnemyData.EnemyDataList[BatDB.Group[SysDB.Enemy, i]].HP
                                        + (mondata.HPGrow * lev + firstLevel * 5) * RandomInt(95, 105) / 100 + RandomInt(-5, 5);
                    BatDB.States[i, 3] = BatDB.States[i, 2];

                    BatDB.States[i, 4] = EnemyData.EnemyDataList[BatDB.Group[SysDB.Enemy, i]].SP
                                        + (mondata.SPGrow * lev + firstLevel * 4) * RandomInt(95, 105) / 100 + RandomInt(-2, 2);
                    BatDB.States[i, 5] = BatDB.States[i, 4];

                    BatDB.States[i, 6] = EnemyData.EnemyDataList[BatDB.Group[SysDB.Enemy, i]].Attack
                                        + (mondata.AttackGrow * lev + firstLevel * 3) * RandomInt(95, 105) / 100 + RandomInt(-2, 2);

                    BatDB.States[i, 7] = EnemyData.EnemyDataList[BatDB.Group[SysDB.Enemy, i]].Defence
                                        + (mondata.DefenceGrow * lev + firstLevel * 3) * RandomInt(95, 105) / 100 + RandomInt(-2, 2);

                    BatDB.States[i, 8] = EnemyData.EnemyDataList[BatDB.Group[SysDB.Enemy, i]].Magic
                                        + (mondata.MagicGrow * lev + firstLevel * 3) * RandomInt(95, 105) / 100 + RandomInt(-2, 2);

                    BatDB.States[i, 9] = EnemyData.EnemyDataList[BatDB.Group[SysDB.Enemy, i]].MagicDef
                                        + (mondata.MagicDefGrow * lev + firstLevel * 3) * RandomInt(95, 105) / 100 + RandomInt(-2, 2);

                    BatDB.States[i, 10] = EnemyData.EnemyDataList[BatDB.Group[SysDB.Enemy, i]].Speed
                                        + (mondata.SpeedGrow * lev + firstLevel * 3) * RandomInt(95, 105) / 100+RandomInt(-2,2);

                    for (int j = 1; j <= 10; j++) BatDB.StatesCor[i, j] = BatDB.States[i, j];
                }
            }
            yield return new WaitForSeconds(5 / 60f);
            if (myDB.party[i] >= 0) {
                states[i + 4] = GameObject.Find("StatesAlly" + (i + 1).ToString());
                monster[i + 4] = StatesData.MonsterDataList[AllyData.AllyDataList[myDB.party[i]].ID].FileName;
                monster[i + 4] = Instantiate(monster[i + 4], new Vector3(-10 + i % 2 + 1.8f + move, (3 - i) * 1.4f - 1, -2), Quaternion.identity);
                monster[i + 4].name = "Ally" + (i + 1).ToString();
                monster[i + 4].transform.localScale = new Vector3(1, 1, 1) * monsterSize;
                preName = StatesData.MonsterDataList[AllyData.AllyDataList[myDB.party[i]].ID].Name;
                GameObject.Find("AllyName" + (i + 1).ToString()).GetComponent<Text>().text = preName;
                BatDB.CharaName[i + 4] = preName;
                BatDB.behavior[i + 4, 0] = -1;
                BatDB.behavior[i + 4, 1] = -1;
                BatDB.States[i + 4, 0] = AllyData.AllyDataList[myDB.party[i]].ID;
                BatDB.death[i+4] = false;
                HPvar[i] = states[i + 4].transform.Find("BackBarHP").gameObject.transform.Find("BarHP").gameObject;
                HPvalue[i] = states[i + 4].transform.Find("BackBarHP").gameObject.transform.Find("HP").gameObject;
                SPvar[i] = states[i + 4].transform.Find("BackBarSP").gameObject.transform.Find("BarSP").gameObject;
                SPvalue[i] = states[i + 4].transform.Find("BackBarSP").gameObject.transform.Find("SP").gameObject;
                BatDB.States[i + 4, 1] = AllyData.AllyDataList[myDB.party[i]].Level;
                BatDB.States[i + 4, 2] = AllyData.AllyDataList[myDB.party[i]].HP;
                if(SysDB.netFlag) BatDB.States[i + 4, 3] = AllyData.AllyDataList[myDB.party[i]].HP;
                else BatDB.States[i + 4, 3] = AllyData.AllyDataList[myDB.party[i]].NowHP;
                BatDB.States[i + 4, 4] = AllyData.AllyDataList[myDB.party[i]].SP;
                if (SysDB.netFlag) BatDB.States[i + 4, 5] = AllyData.AllyDataList[myDB.party[i]].SP;
                else BatDB.States[i + 4, 5] = AllyData.AllyDataList[myDB.party[i]].NowSP;
                BatDB.States[i + 4, 6] = AllyData.AllyDataList[myDB.party[i]].Attack;
                BatDB.States[i + 4, 7] = AllyData.AllyDataList[myDB.party[i]].Defence;
                BatDB.States[i + 4, 8] = AllyData.AllyDataList[myDB.party[i]].Magic;
                BatDB.States[i + 4, 9] = AllyData.AllyDataList[myDB.party[i]].MagicDef;
                BatDB.States[i + 4, 10] = AllyData.AllyDataList[myDB.party[i]].Speed;
                for (int j = 1; j <= 10; j++) BatDB.StatesCor[i + 4, j] = BatDB.States[i+4,j];

                HPSPcontroll(0, i + 4, 0);
                HPSPcontroll(1, i + 4, 0);
                if (BatDB.States[i + 4, 3] == 0) {
                    monster[i + 4].GetComponent<SpriteRenderer>().color = new Color(1,1,1,0);
                    monster[i + 4].transform.Find("shade").gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
                    BatDB.death[i + 4] = true;
                }
            }
            yield return new WaitForSeconds(5 / 60f);
            statesCor(i);
            statesCor(i+4);
        }
        for (int i = 0; i <= 9; i++) {
            if (i % 3 == 0) {
                if (monster[i / 3] != null) {
                    StartCoroutine(MoveGameObject(monster[i / 3], new Vector3(move, 0, 0), 30,0));
                    StartCoroutine(MoveGameObject(states[i / 3], new Vector3(260, 0, 0), 10,20));
                }
                if (monster[i / 3 + 4] != null) {
                    StartCoroutine(MoveGameObject(monster[i / 3 + 4], new Vector3(-move, 0, 0), 30,0));
                    StartCoroutine(MoveGameObject(states[i / 3 + 4], new Vector3(-460, 0, 0), 10,20));
                }
                Button[i / 3] = GameObject.Find("Button" + (i / 3 + 1).ToString());
                StartCoroutine(MoveGameObject(Button[i / 3], new Vector3(0, 500, 0), 10, 20));
            }
            yield return new WaitForSeconds(1 / 60f);
        }
        if (SysDB.netFlag)Button[2].transform.Find("Name").GetComponent<Text>().color = new Color(0.5f,0.5f,0.5f);
        StartCoroutine(MoveGameObject(black1, new Vector3(0, -100, 0), 20, 0));
        StartCoroutine(MoveGameObject(black2, new Vector3(0, 100, 0), 20, 0));
        while(moveCount>0)yield return null;
        yield return new WaitForSeconds(40 / 60f);
        for (int i = 0; i < 8; i++) buttonRe[i] = buttonRect(states[i]);
        for (int i = 8; i < 12; i++) {
            buttonRe[i] = buttonRect(Button[i - 8]);
            buttonImage[i-8] = Button[i - 8].GetComponent<Image>();
        }

        bool charaFlag = false;
        for (int i = 2; i <= 5; i++) {//酸塩基
            string[] charatext = { "弱酸性", "強酸性", "弱塩基性", "強塩基性"};
            int[] buffID = { 11,12,13,14};

            for (int j = 0; j < 8; j++) {
                if (!Attackable(j) || monster[j] == null) continue;
                if (RandomInt(1,100,j>=4)<=charaLev(j,i)*20 && Attackable(j)) {
                    charaFlag = true;
                    Damage(-1,j,0,-1,charatext[i-2],0, false);
                    Buff(j,buffID[i-2],1100);
                }
            }
        }
        if (charaFlag) {
            playSE(33,0);
            yield return new WaitForSeconds(50 / 60f);
        }
        for (int i = 6; i <= 13; i++) {//酸塩基
            string[] charatext = { "攻撃UP", "防御UP", "魔法UP", "魔防UP", "素早さUP" , "回避UP","無敵","注目" };
            int[] buffID = { 1, 3, 5, 7, 9, 24,36,33 };
            charaFlag = false;
            for (int j = 0; j < 8; j++) {
                if (!Attackable(j) || monster[j] == null) continue;
                if (RandomInt(1, 100,j>=4) <= charaLev(j, i) * 20 && Attackable(j)) {
                    charaFlag = true;
                    Damage(-1, j, 0, -1, charatext[i - 6],0, false);
                    Buff(j, buffID[i - 6], i<12?5:2);
                }
            }
            if (charaFlag) {
                playSE(33, 0);
                yield return new WaitForSeconds(50 / 60f);
            }
        }
        if(tutorial&&tutorialNum==0)yield return StartCoroutine(TutorialMessage(0,0));
        if(SysDB.netFlag && timerFlag)timerStart(time);
        itemMakerPos = new Vector3[9]{
            new Vector3(-216,-259,0),new Vector3(-216, -354, 0),new Vector3(-216, -449, 0),new Vector3(-216, -544, 0),
            new Vector3(380,-219,0),new Vector3(380, -314, 0),new Vector3(380, -409, 0),new Vector3(380, -504, 0),
            new Vector3(-500,0,0)
        };
        itemMaker.localPosition = itemMakerPos[8];
        while (true) {//プレイヤー操作開始
            buttonNum = -1;
            while (buttonNum == -1 || skillControll) yield return null;

            if ((buttonNum < 4 || buttonNum > 7)&& tutorial && tutorialNum == 0){yield return StartCoroutine(TutorialMessage(1, 0)); continue; }
            if (buttonNum!=8 && tutorial && tutorialNum == 1) { yield return StartCoroutine(TutorialMessage(5, 0)); continue; }
            if (buttonNum == 11 && tutorial && tutorialNum == 2) { yield return StartCoroutine(TutorialMessage(7, 0)); continue; }
            if (buttonNum == 11 && SysDB.Battle == 1) { yield return textShow("村の子供","逃げるな、怪しい奴め！"); continue; }
            if (buttonNum == 11 && SysDB.Battle == 2) { yield return textShow("", "逃げられない！"); continue; }
            if (buttonNum == 11 && SysDB.Battle == 3) { yield return textShow("テトラ博士", "なんとかこいつを倒してくれ！"); continue; }
            if (buttonNum == 11 && SysDB.Battle == 4) { yield return textShow("王水", "逃がしません…"); continue; }
            if (buttonNum == 11 && SysDB.Battle == 5) { yield return textShow("", "逃げられない！"); continue; }
            if (buttonNum == 11 && SysDB.Battle == 6) { yield return textShow("謎の男", "…逃さん。"); continue; }
            if (buttonNum == 11 && SysDB.Battle == 7) { yield return textShow("モノ博士", "倒してくれ！"); continue; }
            if (buttonNum == 11 && SysDB.Battle == 8) { yield return textShow("", "逃げられない！"); continue; }
            if (buttonNum == 11 && SysDB.Battle == 9) { yield return textShow("", "逃げられない！"); continue; }
            if (buttonNum == 11 && SysDB.Battle == 10) { yield return textShow("", "逃げられない！"); continue; }
            if (buttonNum == 11 && SysDB.Battle == 11) { yield return textShow("ステンレス鋼", "逃さん！"); continue; }
            if (buttonNum == 11 && SysDB.Battle == 12) { yield return textShow("ペンタ博士", "何とか暴走を止めよう！"); continue; }
            if (buttonNum == 11 && SysDB.Battle == 13) { yield return textShow("ペンタ博士", "できるだけ時間を稼いで！"); continue; }
            if (buttonNum == 11 && SysDB.Battle == 14) { yield return textShow("", "逃げられない！"); continue; }
            if (buttonNum == 11 && SysDB.Battle == 15) { yield return textShow("", "逃げられない！"); continue; }

            controll = false;

            if (buttonNum == 8||buttonNum == 9) {//決定,自動
                if (buttonNum == 8) {
                    for (int i = 4; i < 8; i++) {
                        if (BatDB.behavior[i, 1] == -1 && Attackable(i) && monster[i]!=null) {
                            BatDB.behavior[i, 1] = 0;
                        }
                    }
                    StartCoroutine(battle());
                } else {
                    playSE(2, 0);
                    commandFlag = true;
                    commandWin.SetActive(true);
                    commandButtonNum = -1;
                    if (BatDB.turn == 1) commandButton[1].GetComponent<Text>().color = new Color(0.6f,0.6f,0.6f,1);
                    else commandButton[1].GetComponent<Text>().color = new Color(1,1,1,1);
                    while (commandButtonNum == -1 && !forcetimerFlag) yield return null;
                    if (forcetimerFlag) {
                        forcetimerFlag = false;
                        commandButtonNum = -1;
                    }
                    commandFlag = false;
                    commandWin.SetActive(false);
                    if (commandButtonNum == 0) {
                        StartCoroutine(battle());
                    } else if (commandButtonNum == 1) {//前ターンと同じ
                        for (int i = 4; i < 8; i++) {
                            if (BatDB.behavior[i, 1] == -1 && Attackable(i) && monster[i] != null) {
                                BatDB.behavior[i, 0] = preCommand[i - 4, 0];
                                BatDB.behavior[i, 1] = preCommand[i - 4, 1];
                            }
                        }
                        StartCoroutine(battle());
                    } else if (commandButtonNum == 2) {//全員防御
                        for (int i = 4; i < 8; i++) {
                            if (BatDB.behavior[i, 1] == -1 && Attackable(i) && monster[i] != null) {
                                BatDB.behavior[i, 1] = 1;
                            }
                        }
                        StartCoroutine(battle());
                    } else if (commandButtonNum == 3) {//キャンセル
                        controll = true;
                        playSE(5,0);
                    }
                }
                
            } else if (buttonNum == 10) {//道具
                if(!SysDB.netFlag)StartCoroutine(item());
            } else if(buttonNum == 11) {//逃走
                StartCoroutine(escape());
                yield break;
            } else if(buttonNum >= 0 && buttonNum <= 3) {//状態異常確認
                yield return StartCoroutine(StatesCheck(buttonNum));
            } else if (buttonNum >= 4 && buttonNum <=7) {
                commandCoroutine = command(buttonNum);
                StartCoroutine(commandCoroutine);
            }
        }
    }
    IEnumerator StatesCheck(int target) {//状態異常確認
        Text name = statesWindow.transform.Find("Name").GetComponent<Text>();
        Text chara = statesWindow.transform.Find("Chara").GetComponent<Text>();
        Text turn = statesWindow.transform.Find("Turn").GetComponent<Text>();
        Image image = statesWindow.transform.Find("Image").GetComponent<Image>();

        controll = true;
        Button[0].transform.Find("Name").GetComponent<Text>().text = "戻る";
        selectText.text = "▶対象を選択◀";
        Button[1].SetActive(false);
        Button[2].SetActive(false);
        Button[3].SetActive(false);
        statesWindow.SetActive(true);

        while (true) {
            if (target == -1)buttonNum = -1;
            else {
                buttonNum = target;
                target = -1;
            }
            while (buttonNum == -1 && !forcetimerFlag) yield return null;
            if (forcetimerFlag) {
                forcetimerFlag = false;
                break;
            }
            if (buttonNum == 8) {//戻る
                playSE(5, 0);
                controll = true;
                Button[0].transform.Find("Name").GetComponent<Text>().text = "決定";
                selectText.text = "";
                Button[1].SetActive(true);
                Button[2].SetActive(true);
                Button[3].SetActive(true);
                statesWindow.SetActive(false);
                break;
            } else if(buttonNum>=0&&buttonNum<=7) {
                int stock;
                bool show = BatDB.monsterBuff[buttonNum,21]>0 || buttonNum>=4;
                List<KeyValuePair<int, string>> charaData;
                charaData = new List<KeyValuePair<int, string>>();
                playSE(2, 0);
                name.text = BatDB.CharaName[buttonNum];
                image.sprite=monster[buttonNum].GetComponent<SpriteRenderer>().sprite;
                image.SetNativeSize();
                chara.text = "Level:  ";
                if (show) chara.text += BatDB.StatesCor[buttonNum, 1];
                else chara.text += "???";
                chara.text += "\nHP:  ";
                if (show) chara.text += BatDB.StatesCor[buttonNum, 3]+"/"+ BatDB.StatesCor[buttonNum, 2];
                else chara.text += "???/???";
                chara.text += "\nSP:  ";
                if (show) chara.text += BatDB.StatesCor[buttonNum, 5] + "/" + BatDB.StatesCor[buttonNum, 4];
                else chara.text += "???/???";
                chara.text += "\n\n▼状態異常▼\n";
                for (int i = 0; i < BatDB.monsterBuff.GetLength(1); i++) {
                    stock = BatDB.monsterBuff[buttonNum, i];
                    if (stock <= 0) continue;
                    charaData.Add(new KeyValuePair<int, string>(stock, BatDB.buffName[i]));
                }
                charaData.Sort((a, b) => b.Key - a.Key);
                turn.text = "";
                for (int i = 0; i < Mathf.Min(charaData.Count,12); i++) {
                    chara.text += charaData[i].Value + "\n";
                    if (charaData[i].Value == "ATP") {
                        turn.text += "次の攻撃まで\n";
                        continue;
                    }
                    if (charaData[i].Key < 1000) turn.text += "残り" + charaData[i].Key + "ターン\n";
                    else turn.text += "常時\n";
                }
                if (charaData.Count > 12) chara.text += "etc.";
            }
        }
        yield return null;
    }
    IEnumerator command(int num) {//命令処理
        int sp,type;
        string spSkill;

        if (BatDB.death[num] == true) {
            playSE(5,0);
            skillControll = false;
            controll = true;
            yield break;
        }
        itemControll = false;
        skillWindow.SetActive(true);
        skillControll = true;
        controll = false;
        cursorAlly.transform.position = monster[num].transform.position + new Vector3(0,1.8f,0);
        skillCount = -1;
        skillStart = 0;
        partyNum = num - 4;
        skillButton[11].SetActive(false);
        skillButton[12].SetActive(false);

        playSE(2, 0);
        for (int i = 0; i < AllyData.AllyDataList[myDB.party[num - 4]].Skill.Length; i++) {
            if (AllyData.AllyDataList[myDB.party[num - 4]].Skill[i] != -1)skillCount++;
        }

        skillSelect://ラベル//
        skillButtonNum = -1;

        for (int i = 0; i < 10; i++) {
            
            if (i +skillStart< AllyData.AllyDataList[myDB.party[num - 4]].Skill.Length && AllyData.AllyDataList[myDB.party[num - 4]].Skill[i + skillStart] != -1) {
                skillButton[i].GetComponent<Text>().text = SkillData.SkillDataList[AllyData.AllyDataList[myDB.party[num - 4]].Skill[i + skillStart]].Name;
                sp = SkillData.SkillDataList[AllyData.AllyDataList[myDB.party[num - 4]].Skill[i + skillStart]].SP;
                if (sp == 0) spSkill = "---";
                else spSkill = sp.ToString();
                if (sp > BatDB.States[num, 5]) skillButton[i].transform.Find("SP").gameObject.GetComponent<Text>().color =new Color(1,0,0,1);
                else skillButton[i].transform.Find("SP").gameObject.GetComponent<Text>().color = new Color(1, 1, 1, 1);
                skillButton[i].transform.Find("SP").gameObject.GetComponent<Text>().text = spSkill;
            } else {
                skillButton[i].GetComponent<Text>().text = "";
                skillButton[i].transform.Find("SP").gameObject.GetComponent<Text>().text = "";
            }

        }
        if (tutorial && tutorialNum == 0) yield return StartCoroutine(TutorialMessage(2,0));
        while (true) {
            if (skillButtonNum >= 0 && !(skillButtonNum==11||skillButtonNum==12)) break;
            yield return null;
        }
        if (tutorial && tutorialNum == 0 && skillButtonNum!=0) {
            yield return StartCoroutine(TutorialMessage(2, 0));
            goto skillSelect;
        }
        if (skillButtonNum == 10) {//戻る
            playSE(5, 0);
            for (int i = 0; i < 4; i++) {
                if (lines[num - 4, i] != null) Destroy(lines[num - 4, i]);
            }
            BatDB.behavior[num, 0] = -1;
            BatDB.behavior[num, 1] = -1;
            skillControll = false;
            cursorAlly.transform.position = new Vector3(0, 0, 0);
            skillWindow.SetActive(false);
            yield return new WaitForSeconds(10 / 60f);
            controll = true;
        } else if(skillButtonNum == 11) {//←
            playSE(4,0);
            skillStart -= 10;
            if (skillStart < 0) skillStart = skillCount - skillCount % 10;
            goto skillSelect;
        } else if (skillButtonNum == 12) {//→
            playSE(4, 0);
            skillStart += 10;
            if (skillStart > skillCount) skillStart = 0;
            goto skillSelect;
        }else if (skillButtonNum >= 0) {//技選択
            playSE(2,0);
            type = SkillData.SkillDataList[AllyData.AllyDataList[myDB.party[num - 4]].Skill[skillButtonNum + skillStart]].Type;
            if (type == 0 || type == 5) {//0:誰か1体
                controll = true;
                Button[0].transform.Find("Name").GetComponent<Text>().text = "戻る";
                selectText.text = "▶対象を選択◀";
                Button[1].SetActive(false);
                Button[2].SetActive(false);
                Button[3].SetActive(false);
                skillWindow.SetActive(false);
                yield return new WaitForSeconds(10 / 60f);
                if (tutorial && tutorialNum == 0) yield return StartCoroutine(TutorialMessage(3, 0));
                attackSelect://ラベル
                while (true) {
                    buttonNum = -1;
                    while (buttonNum < 0 || buttonNum >= 9) yield return null;
                    //if (buttonNum < 8 && Attackable(buttonNum) == false) playSE(5, 0);
                    //else break;
                    break;
                }
                if (tutorial && tutorialNum == 0 && buttonNum >= 4) {
                    yield return StartCoroutine(TutorialMessage(3, 0));
                    goto attackSelect;
                }
                if (buttonNum != 8) {
                    playSE(2, 0);
                    for (int i = 0; i < 4; i++) {
                        if(lines[num - 4, i]!=null) Destroy(lines[num - 4, i]);
                    }
                    lines[num - 4,0] = Arrow(num,buttonNum);
                    BatDB.behavior[num, 0] = buttonNum;
                    BatDB.behavior[num, 1] = AllyData.AllyDataList[myDB.party[num - 4]].Skill[skillButtonNum + skillStart];
                }
                controll = false;
                Button[0].transform.Find("Name").GetComponent<Text>().text = "決定";
                selectText.text = "";
                Button[1].SetActive(true);
                Button[2].SetActive(true);
                Button[3].SetActive(true);
                if (buttonNum == 8) {
                    playSE(5, 0);
                    skillWindow.SetActive(true);
                    skillControll = true;
                    buttonNum = -1;
                    skillButtonNum = -1;
                    skillCursor.transform.position = new Vector3(-1000, 0, 0);
                    detail.text = "";
                    goto skillSelect;
                }
                buttonNum = -1;

            } else if(type == 1 || type == 4) {//1:敵全体,4:敵ランダム
                for (int i = 0; i < 4; i++) {
                    if (lines[num - 4, i] != null) Destroy(lines[num - 4, i]);
                }
                for (int i = 0; i < 4; i++) {
                    if(Attackable(i))lines[num - 4, i] = Arrow(num, i);
                }
                BatDB.behavior[num, 0] = num;
                BatDB.behavior[num, 1] = AllyData.AllyDataList[myDB.party[num - 4]].Skill[skillButtonNum + skillStart];
            } else if(type == 2) {//2:味方全体
                for (int i = 0; i < 4; i++) {
                    if (lines[num - 4, i] != null) Destroy(lines[num - 4, i]);
                }
                for (int i = 4; i < 8; i++) {
                    if (Attackable(i)) lines[num - 4, i - 4] = Arrow(num, i);
                }
                BatDB.behavior[num, 0] = num;
                BatDB.behavior[num, 1] = AllyData.AllyDataList[myDB.party[num - 4]].Skill[skillButtonNum + skillStart];
            } else if(type == 3) {//3:自分自身
                for (int i = 0; i < 4; i++) {
                    if (lines[num - 4, i] != null) Destroy(lines[num - 4, i]);
                }
                lines[num - 4, 0] = Arrow(num, num);
                BatDB.behavior[num, 0] = num;
                BatDB.behavior[num, 1] = AllyData.AllyDataList[myDB.party[num - 4]].Skill[skillButtonNum + skillStart];
            }

            skillCursor.transform.position = new Vector3(-1000, 0, 0);
            detail.text = "";
            skillControll = false;
            cursorAlly.transform.position = new Vector3(0, 0, 0);
            skillWindow.SetActive(false);
            for (int i = 0; i < 10; i++) yield return null;
            controll = true;

            if (tutorial && tutorialNum == 0)yield return StartCoroutine(TutorialMessage(4, 1));
        }
    }
    IEnumerator item() {//道具選択処理
        int sp, type;
        string spSkill;

        skillWindow.SetActive(true);
        skillControll = true;
        itemControll = true;
        controll = false;
        skillCount = -1;
        skillStart = 0;


        playSE(2, 0);
        for (int i = 0; i < itemID.Length; i++)itemID[i] = -1;
        for (int i = 0; i < ItemData.ItemDataList.Count; i++) {
            if (ItemData.ItemDataList[i].Possess > 0 && ItemData.ItemDataList[i].Type >= 0) {
                itemID[skillCount+1] = i;
                skillCount++;
            }
        }

        if (skillCount <= 9) {
            skillButton[11].SetActive(false);
            skillButton[12].SetActive(false);
        } else {
            skillButton[11].SetActive(true);
            skillButton[12].SetActive(true);
        }
    itemSelect://ラベル//
        skillButtonNum = -1;

        for (int i = 0; i < 10; i++) {
            if (i + skillStart < itemID.Length && itemID[i + skillStart] >=0) {
                skillButton[i].GetComponent<Text>().text = ItemData.ItemDataList[itemID[i + skillStart]].Name;
                sp = ItemData.ItemDataList[itemID[i + skillStart]].Possess;
                spSkill = "x"+sp.ToString();
                skillButton[i].transform.Find("SP").gameObject.GetComponent<Text>().text = spSkill;
                skillButton[i].transform.Find("SP").gameObject.GetComponent<Text>().color = new Color(1, 1, 1, 1);
            } else {
                skillButton[i].GetComponent<Text>().text = "";
                skillButton[i].transform.Find("SP").gameObject.GetComponent<Text>().text = "";
            }

        }
        while (true) {
            if (skillButtonNum >= 0 && !((skillButtonNum==11||skillButtonNum==12)&& skillCount <= 9)) break;
            yield return null;
        }
        if (skillButtonNum == 10) {//戻る
            itemMaker.localPosition = itemMakerPos[8];
            playSE(5, 0);
            BatDB.itemTarget = -1;
            BatDB.itemID = -1;
            skillControll = false;
            itemControll = false;
            cursorAlly.transform.position = new Vector3(0, 0, 0);
            skillWindow.SetActive(false);
            yield return new WaitForSeconds(10 / 60f);
            controll = true;
        } else if (skillButtonNum == 11) {//←
            playSE(4, 0);
            skillStart -= 10;
            if (skillStart < 0) skillStart = skillCount - skillCount % 10;
            goto itemSelect;
        } else if (skillButtonNum == 12) {//→
            playSE(4, 0);
            skillStart += 10;
            if (skillStart > skillCount) skillStart = 0;
            goto itemSelect;
        } else if (skillButtonNum >= 0) {//道具選択
            playSE(2, 0);
            type = ItemData.ItemDataList[itemID[skillButtonNum + skillStart]].Type;
            if (type == 0 || type == 5) {//0:誰か1体
                controll = true;
                Button[0].transform.Find("Name").GetComponent<Text>().text = "戻る";
                selectText.text = "▶対象を選択◀";
                Button[1].SetActive(false);
                Button[2].SetActive(false);
                Button[3].SetActive(false);
                skillWindow.SetActive(false);
                yield return new WaitForSeconds(10 / 60f);

                while (true) {
                    buttonNum = -1;
                    while (buttonNum < 0 || buttonNum >= 9) yield return null;
                    //if (buttonNum < 8 && Attackable(buttonNum) == false) playSE(5, 0);
                    //else break;
                    break;
                }
                if (buttonNum != 8) {
                    playSE(2, 0);
                    BatDB.itemTarget = buttonNum;
                    itemMaker.localPosition = itemMakerPos[buttonNum];
                    BatDB.itemID = itemID[skillButtonNum + skillStart];
                }
                controll = false;
                Button[0].transform.Find("Name").GetComponent<Text>().text = "決定";
                selectText.text = "";
                Button[1].SetActive(true);
                Button[2].SetActive(true);
                Button[3].SetActive(true);
                if (buttonNum == 8) {
                    playSE(5, 0);
                    skillWindow.SetActive(true);
                    skillControll = true;
                    itemControll = true;
                    buttonNum = -1;
                    skillButtonNum = -1;
                    skillCursor.transform.position = new Vector3(-1000, 0, 0);
                    detail.text = "";
                    goto itemSelect;
                }
                buttonNum = -1;

            } else if (type == 1 || type == 4) {//1:敵全体,4:敵ランダム
                BatDB.itemTarget = -1;
                BatDB.itemID = itemID[skillButtonNum + skillStart + skillStart];
            } else if (type == 2) {//2:味方全体
                BatDB.itemTarget = -1;
                BatDB.itemID = itemID[skillButtonNum + skillStart + skillStart];
            }

            skillCursor.transform.position = new Vector3(-1000, 0, 0);
            detail.text = "";
            skillControll = false;
            skillWindow.SetActive(false);
            yield return new WaitForSeconds(10 / 60f);
            controll = true;

        }
    }
    IEnumerator escape() {//逃走処理
        if (SysDB.netFlag && timerFlag) timerStop(true);
        if(SysDB.netFlag) battleResult = "敗北";
        for (int i = 0; i < 4; i++) {
            for (int j = 0; j < 4; j++) {
                if (lines[j, i] != null) Destroy(lines[j, i]);
            }
        }
        playSE(2, 0);
        StartCoroutine(MoveGameObject(black1, new Vector3(0, 100, 0), 20, 0));
        StartCoroutine(MoveGameObject(black2, new Vector3(0, -100, 0), 20, 0));
        if (SysDB.netFlag) {//相手の命令待ち
            netCon.escape[me] = true;
            netCon.commandFlag[me] = true;
            netCon.mode = 2;
            yield return Waiting(2);
            netCon.mode = 3;
            yield return Waiting(3);
            netCon.mode = 1;
            netCon.commandFlag[me] = false;
            for (int i = 1; i <= 3; i++) {
                netCon.noNeedSend[i] = false;
                netCon.sendNoNeed[i] = false;
                netCon.getEnemy[i] = false;
            }
        }

        yield return new WaitForSeconds(30 / 60f);
        playSE(0, 0);
        for (int i = 4; i < 8; i++) {
            if (monster[i] != null) {
                monster[i].transform.localScale = new Vector3(-1, 1, 1) * monsterSize;
                StartCoroutine(MoveGameObject(monster[i], new Vector3(move, 0, 0), 60, 0));
            }

        }
        yield return new WaitForSeconds(10 / 60f);
        result = 3;
        StartCoroutine(battleFinish());
        yield return null;
    }
    IEnumerator battleFinish() {//終了トランジション
        if ((!SysDB.netFlag)&&!MonsterGet) {
            for (int i = 0; i < 4; i++) {
                if (myDB.party[i] >= 0) {
                    AllyData.AllyDataList[myDB.party[i]].NowHP = BatDB.States[i + 4, 3];
                    AllyData.AllyDataList[myDB.party[i]].NowSP = BatDB.States[i + 4, 5];
                }
            }
        }

        Fade fade;
        if (!SysDB.netFlag) fade = GameObject.Find("Main Camera").transform.Find("GameController").GetComponent<GameController>().fadeCanv.GetComponent<Fade>();
        fade = GameObject.Find("FadeCanvas").GetComponent<Fade>();
        fade.FadeIn(1,1f);
        for (int i = 0; i < 90; i++) {
            yield return new WaitForSeconds(1 / 60f);
            if (audioS.volume > 0.006f) audioS.volume -= 0.006f;
        }
        SysDB.BattleResult = result;
        SysDB.battleFlag = false;
        if (SysDB.netFlag && !historyMemo) {
            historyMemo = true;
            if (nameResult == "") {
                nameResult = "(接続切れ)";
                battleResult = "--";
            }
            netResult = dateResult + "_" + nameResult + "_" + battleResult;
            for (int i = 8; i >= 0; i--) {
                if (!PlayerPrefs.HasKey("History" + i.ToString())) continue;
                PlayerPrefs.SetString("History"+(i+1).ToString(), PlayerPrefs.GetString("History" + i.ToString()));
            }
            PlayerPrefs.SetString("History" + 0.ToString(), netResult);
            Debug.Log(netResult+battleResult);
            SceneManager.UnloadSceneAsync("Scenes/Battle");
        } else{
            if (loseFlag) {
                yield return new WaitForSeconds(1);
                SceneManager.LoadScene("Scenes/Title");
            } else SceneManager.UnloadSceneAsync("Scenes/Battle");
        }
    }

    IEnumerator battle() {//行動決定
        int pre,min1=900000000,min2= 900000000;

        for (int i = 4; i < 8; i++) {
            if (Attackable(i) && monster[i] != null) {
                preCommand[i - 4, 0] = BatDB.behavior[i, 0];
                preCommand[i - 4, 1] = BatDB.behavior[i, 1];
            }
        }

        for (int i = 0; i < 4; i++) {
            for (int j = 0; j < 4; j++) {
                if (lines[j, i] != null) Destroy(lines[j, i]);
            }
        }
        playSE(2, 0);
        if(SysDB.netFlag && timerFlag)timerStop(true);
        StartCoroutine(MoveGameObject(black1, new Vector3(0, 100, 0), 20, 0));
        StartCoroutine(MoveGameObject(black2, new Vector3(0, -100, 0), 20, 0));

        if (SysDB.netFlag) {//相手の命令待ち
            netCon.commandFlag[me] = true;
            for (int i = 0; i < 4; i++) {
                netCon.command[me, i, 0] = BatDB.behavior[i + 4, 0];
                netCon.command[me, i, 1] = BatDB.behavior[i + 4, 1];
            }
            for(int i = 0; i < 100; i++) {
                int ran = SysDB.randomInt(1,2000000000);
                netCon.rand[me, i] = ran;
                random[me, i] = ran;
            }
            netCon.mode = 2;
            yield return Waiting(2);
            netCon.mode = 3;
            yield return Waiting(3);
            netCon.mode = 1;
            netCon.commandFlag[me] = false;
            for(int i = 1; i <= 3; i++) {
                netCon.noNeedSend[i] = false;
                netCon.sendNoNeed[i] = false;
                netCon.getEnemy[i] = false;
            }
            if (netCon.escape[you]) {
                StartCoroutine(Win());
                yield break;
            }
            for (int i = 0; i < 100; i++) {
                random[you, i] = netCon.rand[you, i];
            }
            randIndex[0] = 0;
            randIndex[1] = 0;
        }

        for (int i = 0; i < 4; i++) {//律速段階
            if(monster[i] != null) {
                if (BatDB.monsterBuff[i, 18] > 0 && BatDB.StatesCor[i, 10] < min1) min1 = BatDB.StatesCor[i, 10];
            }
        }
        for (int i = 4; i < 8; i++) {
            if (monster[i] != null) {
                if (BatDB.monsterBuff[i, 18] > 0 && BatDB.StatesCor[i, 10] < min2) min2 = BatDB.StatesCor[i, 10];
            }
        }
        for (int i = 0; i < 8;i++) {
            if (monster[i] != null && Attackable(i)) {
                if (SysDB.netFlag && i < 4) {
                    BatDB.behavior[i, 0] = netCon.command[you, i, 0];
                    BatDB.behavior[i, 1] = netCon.command[you, i, 1];
                    if (BatDB.behavior[i, 0] >= 0) BatDB.behavior[i, 0] = (BatDB.behavior[i, 0] + 4) % 8;
                }
                if (i >= 0 && Attackable(i) && BatDB.monsterBuff[i, 19] > 0 && RandomInt(1, 100, i >= 4) <= 60) {//励起状態
                    BatDB.behavior[i, 1] = AutoBattle(i, true);
                    BatDB.behavior[i, 0] = skillTarget(i, BatDB.behavior[i, 1]);
                }
                if (BatDB.behavior[i, 1] == -1) {
                    BatDB.behavior[i, 1] = AutoBattle(i, true);//選択してなかったらオートバトル
                    BatDB.priority[i] = SkillData.SkillDataList[BatDB.behavior[i, 1]].Priority;//優先度
                    if (BatDB.priority[i] == 0) BatDB.behavior[i, 1] = -1;
                } else {
                    BatDB.priority[i] = SkillData.SkillDataList[BatDB.behavior[i, 1]].Priority;//優先度
                }

                speed[i, 0] = i;
                speed[i, 1] = BatDB.StatesCor[i, 10];

                if (BatDB.monsterBuff[i, 9] > 0) speed[i, 1] = speed[i, 1] * 3 / 2;
                else if (BatDB.monsterBuff[i, 10] > 0) speed[i, 1] = speed[i, 1] * 2 / 3;
                if (BatDB.monsterBuff[i, 22] > 0) speed[i, 1] = speed[i, 1] * 3 / 2;

                if (i < 4 && min1 < speed[i, 1]) speed[i, 1] = min1;
                if (i >= 4 && min2 < speed[i, 1]) speed[i, 1] = min2;
                speed[i, 1] = (int)Mathf.Floor(speed[i, 1] * RandomInt(90, 111,i>=4)/100f) + BatDB.priority[i]*10000;
            } else {
                speed[i, 0] = -1;
                speed[i, 1] = -1;
            }
        }
        if (SysDB.netFlag && me == 1) {
            for(int i = 0; i < 4; i++) {
                pre = speed[i, 0];
                speed[i, 0] = speed[i + 4, 0];
                speed[i + 4, 0] = pre;
                pre = speed[i, 1];
                speed[i, 1] = speed[i + 4, 1];
                speed[i + 4, 1] = pre;
            }
        }
        for (int i = 0; i < 7; i++) {
            for(int j = 0; j < 7-i; j++) {
                if (speed[j, 1] < speed[j + 1, 1] || speed[j, 1] == speed[j + 1, 1] && RandomInt(1, 100,me==0)<=50) {
                    pre = speed[j, 0];
                    speed[j, 0] = speed[j + 1, 0];
                    speed[j + 1, 0] = pre;
                    pre = speed[j, 1];
                    speed[j, 1] = speed[j + 1, 1];
                    speed[j + 1, 1] = pre;
                }
            }
        }
        yield return new WaitForSeconds(30 / 60f);

        if (BatDB.itemID >= 0) {
            yield return new WaitForSeconds(10 / 60f);
            movingFlag = true;
            StartCoroutine(itemUse());
            while (movingFlag) yield return null;
            yield return new WaitForSeconds(30 / 60f);
        }
        itemMaker.localPosition = itemMakerPos[8];
        for (int i = 0; i < 8; i++) {//行動開始
                                     //Behavior[,0]:ターゲットナンバー
                                     //Behavior[,1]:スキルナンバー
            movingFlag = true;
            pre = speed[i, 0];
            if (pre >= 0 && Attackable(pre) && BatDB.behavior[pre, 1] == -1) BatDB.behavior[pre, 1] = AutoBattle(pre,false);
            if (pre >= 0 && Attackable(pre) && BatDB.behavior[pre, 0] == -1) BatDB.behavior[pre, 0] = skillTarget(pre, BatDB.behavior[pre, 1]);
            if (pre >= 0 && Attackable(pre) && BatDB.behavior[pre, 0] == -2) continue;

            if (pre >= 0 && Attackable(pre) && BatDB.monsterBuff[pre,19]>0&& RandomInt(1, 100, pre >= 4)<= 60 && BatDB.behavior[pre, 0] == -1) {//励起状態
                BatDB.behavior[pre, 1] = AutoBattle(pre,false);
                BatDB.behavior[pre, 0] = skillTarget(pre, BatDB.behavior[pre, 1]);
            }

            if (pre >= 0 && monster[pre] != null && !BatDB.death[pre] && BatDB.behavior[pre, 1]!=-2) {
                if ((!Attackable(BatDB.behavior[pre, 0])) && SkillData.SkillDataList[BatDB.behavior[pre, 1]].Type != 5) {
                    BatDB.behavior[pre, 0] = skillTarget(pre, BatDB.behavior[pre, 1]);
                }
                if (BatDB.behavior[pre, 0] != -1) {
                    if (BatDB.monsterBuff[pre, 17] > 0) {
                        movingFlag = false;
                        continue;
                    }
                    StartCoroutine(skill(pre, BatDB.behavior[pre, 0], BatDB.behavior[pre, 1]));
                    while (movingFlag) yield return null;
                    movingFlag = false;
                    yield return new WaitForSeconds(30 / 60f);
                }
            }
            if(pre >= 0 && monster[pre] != null && !BatDB.death[pre] && BatDB.monsterBuff[pre,20]>0) {
                //放射性崩壊
                int nowHP=BatDB.States[pre,3];
                int radioDamage, rA = 17, rB = 14;

                radioDamage = Mathf.RoundToInt((nowHP >= rA?Mathf.Sqrt(nowHP+rB*rB/4.0f-rA)-rB/2.0f+(float)rA/rB:(float)nowHP/rB)*3.0f/2.0f);
                if (radioDamage <= 0) radioDamage = 1;
                playSE(32, 0);
                Damage(-1,pre,radioDamage,6,"",0, false);
                yield return new WaitForSeconds(30 / 60f);
            }
            if (pre >= 0 && monster[pre] != null && !BatDB.death[pre] && BatDB.monsterBuff[pre, 35] > 0) {
                //肥料
                int nowHP = BatDB.States[pre, 3];
                int radioDamage, rA = 17, rB = 14;

                radioDamage = Mathf.RoundToInt((nowHP >= rA ? Mathf.Sqrt(nowHP + rB * rB / 4.0f - rA) - rB / 2.0f + (float)rA / rB : (float)nowHP / rB) * 3.0f / 2.0f);
                if (radioDamage <= 0) radioDamage = 1;
                playSE(9, 0);
                Damage(-1, pre, 3*radioDamage, 3, "",0, false);
                yield return new WaitForSeconds(30 / 60f);
            }
            //勝利判定
            if (!Attackable(0) && !Attackable(1) && !Attackable(2) && !Attackable(3)) {
                StartCoroutine(Win());//勝利処理
                yield break;
            } else if(!Attackable(4) && !Attackable(5) && !Attackable(6) && !Attackable(7)) {
                StartCoroutine(Lose());//敗北処理
                yield break;
            }
        }
        StartCoroutine(MoveGameObject(black1, new Vector3(0, -100, 0), 20, 0));
        StartCoroutine(MoveGameObject(black2, new Vector3(0, 100, 0), 20, 0));
        BatDB.itemTarget = -1;
        BatDB.itemID = -1;
        BatDB.turn += 1;
        turn.GetComponent<Text>().text = BatDB.turn.ToString()+"ターン";
        yield return new WaitForSeconds(20 / 60f);
        controll = true;
        for (int i = 0; i < 8; i++) {
            BatDB.behavior[i, 0] = -1;
            BatDB.behavior[i, 1] = -1;
            BatDB.priority[i] = 0;
            for (int j = 0; j < BatDB.monsterBuff.GetLength(1); j++) {
                if (j == 38) continue;
                if (BatDB.monsterBuff[i, j] >= 1 || BatDB.monsterBuff[i, j] < 1000)BatDB.monsterBuff[i, j] -= 1;
            }
            if (monster[i] != null) {
                if (BatDB.monsterBuff[i, 0] <= 0 && BatDB.monsterBuff[i, 17] <= 0) {
                    monster[i].GetComponent<Animator>().speed = 1;
                }
            }
        }
        if (tutorial && tutorialNum == 1) { yield return StartCoroutine(TutorialMessage(6, 1));}
        if (SysDB.netFlag && timerFlag) timerStart(time);
    }

    IEnumerator Win() {//勝利処理
        int time = 20;
        int exp = 0, money = 0, expPre;
        int ene,upLevel,upStates,lev,skillLen;
        int[] refe = { 2,4,6,7,8,9,10};
        int[] states = new int[12];
        int[] needExp = new int[99];
        int[] skillLevel = { 3,6,10,17,24,32,40,50};
        int[] statesMax = new int[7];
        bool itemGet = false;
        bool skillExist;
        string text = "";
        float audioVolume = audioS.volume;
        MonsterData mon;
        Text winDetail;
        for (int i = time - 1; i >= 0; i--) {
            audioS.volume = audioVolume * i / (float)time;
            yield return new WaitForSeconds(1 / 60f);
        }
        audioS.loop = false;
        audioS.clip = BGM[2];
        if(!leftRoom)audioS.Play();
        audioS.volume = audioVolume;
        yield return new WaitForSeconds(60 / 60f);
        winWindow.SetActive(true);
        if (!SysDB.netFlag) {
            text = "▼戦利品▼";
            for (int i = 0; i < 4; i++) {
                ene = BatDB.Group[SysDB.Enemy, i];
                if (ene >= 0) {
                    money += EnemyData.EnemyDataList[ene].Money;
                    exp += EnemyData.EnemyDataList[ene].Exp;
                    for (int j = 0; j < EnemyData.EnemyDataList[ene].Item.Length; j++) {
                        if (EnemyData.EnemyDataList[ene].Item[j] >= 0 && EnemyData.EnemyDataList[ene].Rate[j] >= RandomInt(1, 100)) {
                            text += "\n" + ItemData.ItemDataList[EnemyData.EnemyDataList[ene].Item[j]].Name;
                            ItemData.ItemDataList[EnemyData.EnemyDataList[ene].Item[j]].Possess += 1;
                            itemGet = true;
                        }
                    }
                }
            }
            winWindow.transform.Find("itemDetail").GetComponent<Text>().text = itemGet ? text : "";

            myDB.money += money;
            playSE(13, 0);
            text = "戦闘に勝利！\n経験値を" + exp.ToString() + "獲得\n"
                 + money.ToString() + "G" + (itemGet ? "と戦利品" : "") + "を獲得";
            winWindow.transform.Find("winDetail").GetComponent<Text>().text = text;
            while (!Input.GetMouseButton(0)) yield return null;
            while (!Input.GetMouseButtonUp(0)) yield return null;
            needExp[0] = 22;
            for (int i = 1; i <= 98; i++) needExp[i] = Mathf.FloorToInt((1.625f / (i * 1.25f + 1.3f) + 1f) * needExp[i - 1]);
            for (int i = 1; i <= 98; i++) needExp[i] -= 20;
            int maxLevel = 1;
            for (int i = 0; i < 4; i++) {
                if (myDB.party[i] >= 0) maxLevel = Math.Max(maxLevel, AllyData.AllyDataList[myDB.party[i]].Level);
            }
            for (int i = 0; i < 4; i++) {
                if (myDB.party[i] >= 0) {
                    if (AllyData.AllyDataList[myDB.party[i]].Level + 30 <= maxLevel) expPre = exp * 5;
                    else if (AllyData.AllyDataList[myDB.party[i]].Level + 20 <= maxLevel) expPre = exp * 4;
                    else if (AllyData.AllyDataList[myDB.party[i]].Level + 10 <= maxLevel) expPre = exp * 3;
                    else if (AllyData.AllyDataList[myDB.party[i]].Level + 5 <= maxLevel) expPre = exp*2;
                    else expPre = exp;
                    upLevel = 0;
                    if(monster[i+4]!=null && Attackable(i+4) && st(i+4,3)>0)AllyData.AllyDataList[myDB.party[i]].Exp += expPre;
                    while (true) {
                        if (!(monster[i + 4] != null && Attackable(i + 4) && st(i+4, 3) > 0)) break;
                        if (AllyData.AllyDataList[myDB.party[i]].Level + upLevel >= 99) {
                            AllyData.AllyDataList[myDB.party[i]].NextExp = 0;
                            break;
                        }
                        AllyData.AllyDataList[myDB.party[i]].NextExp -= expPre;
                        if (AllyData.AllyDataList[myDB.party[i]].NextExp <= 0) {
                            upLevel += 1;
                            expPre = -AllyData.AllyDataList[myDB.party[i]].NextExp;
                            if (AllyData.AllyDataList[myDB.party[i]].Level + upLevel >= 99) {
                                AllyData.AllyDataList[myDB.party[i]].NextExp = 0;
                                break;
                            }
                            AllyData.AllyDataList[myDB.party[i]].NextExp = needExp[AllyData.AllyDataList[myDB.party[i]].Level + upLevel] - 10;
                        } else break;
                    }
                    if (upLevel >= 1) {
                        playSE(13, 0);

                        winWindow.transform.Find("itemDetail").GetComponent<Text>().text = "";
                        winWindow.transform.Find("winDetail").GetComponent<Text>().text
                            = StatesData.MonsterDataList[AllyData.AllyDataList[myDB.party[i]].ID].Name + "\nのレベルが上がった";
                        winWindow.transform.Find("StatesName").GetComponent<Text>().text
                            = "Level\n\n最大HP\n最大SP\n攻撃力\n防御力\n魔法力\n魔防力\n素早さ";
                        winWindow.transform.Find("StatesArrow").GetComponent<Text>().text = "→\n\n";
                        winWindow.transform.Find("StatesValue1").GetComponent<Text>().text
                            = AllyData.AllyDataList[myDB.party[i]].Level.ToString() + "\n\n";
                        winWindow.transform.Find("StatesValue2").GetComponent<Text>().text
                            = (AllyData.AllyDataList[myDB.party[i]].Level + upLevel).ToString() + "\n\n";
                        winWindow.transform.Find("StatesUp").GetComponent<Text>().text
                            = "(+" + upLevel.ToString() + ")\n\n";
                        AllyData.AllyDataList[myDB.party[i]].Level += upLevel;
                        AllyData.AllyDataList[myDB.party[i]].CharaPt += upLevel;

                        states[refe[0]] = AllyData.AllyDataList[myDB.party[i]].HP;
                        states[refe[1]] = AllyData.AllyDataList[myDB.party[i]].SP;
                        states[refe[2]] = AllyData.AllyDataList[myDB.party[i]].Attack;
                        states[refe[3]] = AllyData.AllyDataList[myDB.party[i]].Defence;
                        states[refe[4]] = AllyData.AllyDataList[myDB.party[i]].Magic;
                        states[refe[5]] = AllyData.AllyDataList[myDB.party[i]].MagicDef;
                        states[refe[6]] = AllyData.AllyDataList[myDB.party[i]].Speed;

                        MonsterData monMax = StatesData.MonsterDataList[AllyData.AllyDataList[myDB.party[i]].ID];
                        statesMax[0] = monMax.HPMax - states[refe[0]];
                        statesMax[1] = monMax.SPMax - states[refe[1]];
                        statesMax[2] = monMax.AttackMax - states[refe[2]];
                        statesMax[3] = monMax.DefenceMax - states[refe[3]];
                        statesMax[4] = monMax.MagicMax - states[refe[4]];
                        statesMax[5] = monMax.MagicDefMax - states[refe[5]];
                        statesMax[6] = monMax.SpeedMax - states[refe[6]];

                        for (int j = 0; j < 7; j++) {
                            upStates = Math.Min(statesMax[j], AllyData.AllyDataList[myDB.party[i]].Grow[j] * upLevel + RandomInt(-upLevel, upLevel));
                            if (upStates < 0) upStates = 0;
                            winWindow.transform.Find("StatesArrow").GetComponent<Text>().text += "→\n";
                            winWindow.transform.Find("StatesValue1").GetComponent<Text>().text
                                += states[refe[j]].ToString() + "\n";
                            winWindow.transform.Find("StatesValue2").GetComponent<Text>().text
                                += (states[refe[j]] + upStates).ToString() + "\n";
                            if (upStates > 0) {
                                winWindow.transform.Find("StatesUp").GetComponent<Text>().text
                                    += "(+" + upStates.ToString() + ")\n";
                                states[refe[j]] += upStates;
                                if (j == 0) {
                                    BatDB.States[i + 4, refe[j] + 1] += upStates;
                                    AllyData.AllyDataList[myDB.party[i]].NowHP += upStates;
                                } else if (j == 1) {
                                    BatDB.States[i + 4, refe[j] + 1] += upStates;
                                    AllyData.AllyDataList[myDB.party[i]].NowSP += upStates;

                                }
                            } else winWindow.transform.Find("StatesUp").GetComponent<Text>().text += "\n";

                        }
                        AllyData.AllyDataList[myDB.party[i]].HP = states[refe[0]];
                        AllyData.AllyDataList[myDB.party[i]].SP = states[refe[1]];
                        AllyData.AllyDataList[myDB.party[i]].Attack = states[refe[2]];
                        AllyData.AllyDataList[myDB.party[i]].Defence = states[refe[3]];
                        AllyData.AllyDataList[myDB.party[i]].Magic = states[refe[4]];
                        AllyData.AllyDataList[myDB.party[i]].MagicDef = states[refe[5]];
                        AllyData.AllyDataList[myDB.party[i]].Speed = states[refe[6]];

                        while (!Input.GetMouseButton(0)) yield return null;
                        while (!Input.GetMouseButtonUp(0)) yield return null;

                        mon = StatesData.MonsterDataList[AllyData.AllyDataList[myDB.party[i]].ID];
                        lev = AllyData.AllyDataList[myDB.party[i]].Level;
                        winDetail = winWindow.transform.Find("winDetail").GetComponent<Text>();
                        for (int j = 0; j < mon.Skill.Length; j++) {
                            if (mon.Skill[j] >= 0 && lev - upLevel < skillLevel[j] && lev >= skillLevel[j]) {
                                skillExist = false;
                                skillLen = AllyData.AllyDataList[myDB.party[i]].Skill.Length;
                                for (int k = 0; k < skillLen; k++) {
                                    if (AllyData.AllyDataList[myDB.party[i]].Skill[k] == mon.Skill[j]) {
                                        skillExist = true;
                                        break;
                                    }
                                }
                                if (!skillExist) {
                                    playSE(17, 0);
                                    winDetail.text = mon.Name + "　は\n" + SkillData.SkillDataList[mon.Skill[j]].Name + "　を覚えた！";
                                    Array.Resize(ref AllyData.AllyDataList[myDB.party[i]].Skill, skillLen + 1);
                                    Array.Resize(ref AllyData.AllyDataList[myDB.party[i]].SkillUse, skillLen + 1);
                                    AllyData.AllyDataList[myDB.party[i]].Skill[skillLen] = mon.Skill[j];
                                    AllyData.AllyDataList[myDB.party[i]].SkillUse[skillLen] = 0;
                                    while (!Input.GetMouseButton(0)) yield return null;
                                    while (!Input.GetMouseButtonUp(0)) yield return null;
                                }
                            } else if (lev < skillLevel[j]) break;
                        }
                        if (AllyData.AllyDataList[myDB.party[i]].Skill.Length > 10) {
                            partyNum = i;
                            yield return skillSelect(myDB.party[i]);
                        }
                    }
                }
            }
        } else {
            if (leftRoom) {
                battleResult = "接続切";
                text = "相手の通信が切断されました\n対戦を終了します";
            } else if (netCon.escape[you]) {
                battleResult = "勝利";
                playSE(13, 0);
                text = "相手の降参により\n"+netCon.playerName[you] + "に勝利した！";
            } else {
                battleResult = "勝利";
                playSE(13, 0);
                text = netCon.playerName[you] + "に勝利した！";
            }
            winWindow.transform.Find("winDetail").GetComponent<Text>().text = text;
            while (!Input.GetMouseButton(0)) yield return null;
            while (!Input.GetMouseButtonUp(0)) yield return null;
        }
        yield return null;
        result = 1;
        StartCoroutine(battleFinish());
    }
    IEnumerator Lose() {//敗北処理
        int time = 20;
        float audioVolume = audioS.volume;
        for (int i = time - 1; i >= 0; i--) {
            audioS.volume = audioVolume * i / (float)time;
            yield return new WaitForSeconds(1 / 60f);
        }
        audioS.loop = false;
        audioS.clip = BGM[9];
        audioS.Play();
        audioS.volume = audioVolume;
        yield return new WaitForSeconds(20 / 60f);
        yield return StartCoroutine(textShow("","全滅した。"));
        if(SysDB.netFlag) battleResult = "敗北";
        loseFlag = true;
        yield return StartCoroutine(battleFinish());
    }
    IEnumerator Waiting(int num) {//通信待機処理
        IEnumerator timeout = timeOutTimer(60);
        if(timerFlag)StartCoroutine(timeout);
        connectingWin.SetActive(true);
        while (!(netCon.noNeedSend[num] && netCon.sendNoNeed[num])) {
            if (leftRoom) {
                connectingWin.SetActive(false);
                yield return Win();
            }
            yield return null;
        }
        if (timerFlag) StopCoroutine(timeout);
        connectingWin.SetActive(false);
    }
    IEnumerator timeOutTimer(int time) {
        for (int i = 0; i < time; i++) {
            yield return new WaitForSeconds(1);
        }
        leftRoom = true;
        PhotonNetwork.Disconnect();
    }
    IEnumerator netTimer(int time) {
        timerWin.SetActive(true);
        for (int i = 0; i <= time; i++) {
            if (time - i < 10) timerText.text = "残り " + (time - i) + "秒";
            else timerText.text = "残り" + (time - i) + "秒";
            yield return new WaitForSeconds(1);
        }
        timerStop(false);
    }
    void timerStart(int time) {
        timer = netTimer(time);
        StartCoroutine(timer);
    }
    void timerStop(bool force) {
        timerWin.SetActive(false);
        if (force) {
            StopCoroutine(timer);
        } else {
            skillControll = false;
            controll = false;
            skillButtonNum = -1;
            buttonNum = -1;
            Button[0].transform.Find("Name").GetComponent<Text>().text = "決定";
            selectText.text = "";
            Button[1].SetActive(true);
            Button[2].SetActive(true);
            Button[3].SetActive(true);
            skillWindow.SetActive(false);
            statesWindow.SetActive(false);
            forcetimerFlag = true;
            if (commandCoroutine != null) {
                StopCoroutine(commandCoroutine);
                commandCoroutine = null;
            }
            cursorAlly.transform.position = new Vector3(0, 0, 0);
            for (int i = 4; i < 7; i++) {
                if (BatDB.behavior[i, 1] == -1 && Attackable(i) && monster[i] != null) {
                    BatDB.behavior[i, 1] = 0;
                }
            }
            StartCoroutine(battle());
        }
    }
    /*public override void OnDisconnected(DisconnectCause cause) {
        leftRoom = true;
    }*/
    IEnumerator skillSelect(int num) {
        int skillCount = AllyData.AllyDataList[num].Skill.Length-2;
        int skillPage = 0;
        int pageMax = skillCount % 10 == 0 ? skillCount / 10:(skillCount/10+1) ;
        int selectCount=0;
        bool[] skillTrue = new bool[skillCount];
        List<int> skillList = new List<int>();
        Text[] skillText = new Text[10];
        Text[] skillSP = new Text[10];
        Text skillSelect = skillSelectWin.transform.Find("Text").GetComponent<Text>();
        skillSelectWin.SetActive(true);
        skillWindow.SetActive(true);
        skillStart = 2;
        skillControll = true;
        itemControll = false;
        controll = false;
        //messageFlag = true;

        skillButton[10].transform.Find("Name").GetComponent<Text>().text = "決定";
        for(int i = 2; i < skillCount + 2; i++)skillList.Add(AllyData.AllyDataList[num].Skill[i]);
        for(int i = 0; i < 10; i++) {
            skillText[i] = skillButton[i].GetComponent<Text>();
            skillSP[i] = skillButton[i].transform.Find("SP").GetComponent<Text>();
        }
        if (skillCount <= 10) {
            skillButton[11].SetActive(false);
            skillButton[12].SetActive(false);
        } else {
            skillButton[11].SetActive(true);
            skillButton[12].SetActive(true);
        }
        skillButtonNum = -1;
        while (true) {
            if (skillCount - 8 - selectCount < 0) {
                skillSelect.text = "残り0個";
                skillSelect.color = new Color(1,0.7f,1,1);
            } else {
                skillSelect.text = "残り" + (skillCount - 8 - selectCount) + "個";
                skillSelect.color = Color.white;
            }

            if (skillButtonNum == 11 || skillButtonNum == 12 || skillButtonNum == -1) {
                for(int i = 0; i < 10; i++) {
                    if (skillCount <= i + 10 * skillPage) {
                        skillText[i].text = "";
                        skillSP[i].text = "";
                    } else {
                        skillText[i].text = SkillData.SkillDataList[skillList[i+10*skillPage]].Name;
                        skillSP[i].text = SkillData.SkillDataList[skillList[i + 10 * skillPage]].SP.ToString();
                        if (skillTrue[i + 10 * skillPage]) {
                            skillText[i].color = new Color(1, 0.7f, 0.7f, 1);
                            skillSP[i].color = new Color(1, 0.7f, 0.7f, 1);
                        } else {
                            skillText[i].color = Color.white;
                            skillSP[i].color = Color.white;
                        }
                    }
                }
            }
            skillStart = 2+skillPage * 10;
            skillButtonNum = -1;
            while (skillButtonNum < 0) yield return null;
            if ((skillButtonNum == 11 || skillButtonNum == 12) && skillCount <= 10) skillButtonNum = -2;
            if (skillButtonNum == 10) {//決定
                if (skillCount - 8 - selectCount > 0) {
                    playSE(5, 0);
                } else {
                    playSE(2, 0);
                    skillSelectWin.SetActive(false);
                    skillWindow.SetActive(false);
                    skillControll = false;
                    yield return new WaitForSeconds(20 / 60f);
                    break;
                }
            } else if(skillButtonNum == 11 || skillButtonNum == 12) {//←→
                playSE(4,0);
                skillPage += skillButtonNum == 11 ? -1 : 1;
                skillPage = (skillPage + pageMax) % pageMax;
            }else if(skillButtonNum >= 0) {//スキル
                int skillNum = skillButtonNum + 10 * skillPage;
                if (skillTrue[skillNum]) {
                    selectCount--;
                    playSE(5, 0);
                    skillText[skillButtonNum].color = Color.white;
                    skillSP[skillButtonNum].color = Color.white;
                } else {
                    selectCount++;
                    playSE(2, 0);
                    skillText[skillButtonNum].color = new Color(1f, 0.7f, 0.7f, 1);
                    skillSP[skillButtonNum].color = new Color(1, 0.7f, 0.7f, 1);
                }
                skillTrue[skillNum] = !skillTrue[skillNum];
                skillCursor.transform.position = new Vector3(-1000, 0, 0);
            }
        }
        List<int> newSkillList = new List<int>();
        newSkillList.Add(0);
        newSkillList.Add(1);
        for (int i = 0; i < skillCount; i++) {
            if (!skillTrue[i]) {
                newSkillList.Add(skillList[i]);
            }
        }
        Array.Resize(ref AllyData.AllyDataList[num].Skill, newSkillList.Count);
        Array.Resize(ref AllyData.AllyDataList[num].SkillUse, newSkillList.Count);
        for (int i = 0; i < newSkillList.Count; i++) {
            AllyData.AllyDataList[num].Skill[i] = newSkillList[i];
            AllyData.AllyDataList[num].SkillUse[i] = 0;
        }
    }
    IEnumerator skill(int monsterNum, int targetNum, int skillNum) {//技能処理
        int Delay;
        int damage,attribute;
        GameObject effect;
        AudioClip se;
        Vector3 move;
        Monster mon;
        Animator monAni;
        mon = monster[monsterNum].GetComponent<Monster>();
        monAni = monster[monsterNum].GetComponent<Animator>();
        skillName[monsterNum >= 4 ? 0 : 1].main(SkillData.SkillDataList[skillNum].Name);
        if (SkillData.SkillDataList[skillNum].SP > BatDB.States[monsterNum, 5]) {
            yield return new WaitForSeconds(10 / 60f);
            skillNum = -1;
            playSE(6, 0);
            Damage(monsterNum, monsterNum, 0, -1, "SP不足",0, false);
            yield return new WaitForSeconds(30 / 60f);
        } else HPSPcontroll(1, monsterNum, -SkillData.SkillDataList[skillNum].SP);

        if (skillNum >= 0) {
            attribute = SkillData.SkillDataList[skillNum].Attribute;
            for (int i = 0; (1 << i) <= attribute; i++) {
                if (((1 << i) & attribute) != 0) {
                    if (i < 3 && BatDB.monsterBuff[monsterNum, i + 29] > 0) {
                        yield return new WaitForSeconds(50 / 60f);
                        skillNum = -1;
                        playSE(6, 0);
                        Damage(monsterNum, monsterNum, 0, -1, "失敗", 0, false);
                        yield return new WaitForSeconds(30 / 60f);
                    }
                }
            }
        }
        if(skillNum==100&&BatDB.monsterBuff[monsterNum,15]>0 || skillNum == 101 && BatDB.monsterBuff[monsterNum, 16] > 0) {
            yield return new WaitForSeconds(50 / 60f);
            skillNum = -1;
            playSE(6, 0);
            Damage(monsterNum, monsterNum, 0, -1, "失敗",0, false);
            yield return new WaitForSeconds(30 / 60f);
        }
        if(targetNum>=0 && (targetNum<4 && monsterNum>=4 || targetNum>=4 && monsterNum < 4)) {
            List<int> targetList = target(monsterNum<4);
            bool targetFlag = false;
            for (int i = 0; i < targetList.Count; i++) {
                if (targetList[i] == targetNum) targetFlag = true;
            }
            if (!targetFlag)targetNum = targetList[RandomInt(0,targetList.Count-1,monsterNum>=4)];
        }

        atpFlag = false;
        if (skillNum == 0) {//通常攻撃
            SysDB.animationFlag = true;
            monAni.SetBool("attack", true);
            Delay = mon.AttackDelay;
            effect = mon.AttackEffect;
            se = mon.AttackSE;
            move = new Vector3((monsterNum >= 4 ? -1 : 1) * 0.3f, 0, 0);
            StartCoroutine(MoveGameObject(monster[monsterNum], move, 5, 0));
            yield return new WaitForSeconds((Delay + 5) / 60f);
            if (effect != null) {
                effectCreate(effect, monster[targetNum].transform.position + new Vector3(0, 0.5f, -1), targetNum < 4 ? 1 : -1);
                damage = DamageCaluculate(monsterNum, targetNum, 100, 0, 100, true,0);
                if (Damage(monsterNum, targetNum, damage, 0, "", 95,0, true)) {
                    if (se != null) audioS.PlayOneShot(se, 3);
                    if (RandomInt(1, 100,monsterNum>=4) <= 15+charaLev(monsterNum, 42) * 5 && Attackable(targetNum) && charaLev(monsterNum, 42)>0) {
                        Damage(monsterNum, targetNum, 0, -1, "放射性崩壊", 0, false, 30);
                        Buff(targetNum, 20, 5);
                        playSE(32, 30);
                    }
                }

            }
            while (SysDB.animationFlag) yield return null;
            StartCoroutine(MoveGameObject(monster[monsterNum], -move, 5, 0));

        } else if (skillNum == 1) {//防御
            playSE(7, 0);
            effectCreate(Effect[1], monster[monsterNum].transform.Find("shade").gameObject.transform.position + new Vector3(0, 0.8f, 0), 1);
            yield return new WaitForSeconds(30 / 60f);
            Buff(monsterNum, 0, 1);
        } else if (skillNum == 2) {//全体攻撃
            SysDB.animationFlag = true;
            monAni.SetBool("attack", true);
            Delay = mon.AttackDelay;
            effect = mon.AttackEffect;
            se = mon.AttackSE;
            move = new Vector3((monsterNum >= 4 ? -1 : 1) * 0.3f, 0, 0);
            StartCoroutine(MoveGameObject(monster[monsterNum], move, 5, 0));
            yield return new WaitForSeconds((Delay + 5) / 60f);
            if (effect != null) {
                for (targetNum = monsterNum >= 4 ? 0 : 4; targetNum < (monsterNum >= 4 ? 4 : 8); targetNum++) {
                    if (Attackable(targetNum)) {
                        effectCreate(effect, monster[targetNum].transform.position + new Vector3(0, 0.5f, -1), targetNum < 4 ? 1 : -1);
                        damage = DamageCaluculate(monsterNum, targetNum, 60, 0, 100, true,0);
                        Damage(monsterNum, targetNum, damage, 0, "",0,true);
                    }
                }
            }
            if (se != null) audioS.PlayOneShot(se, 3);
            while (SysDB.animationFlag) yield return null;
            StartCoroutine(MoveGameObject(monster[monsterNum], -move, 5, 0));
        } else if (skillNum >= 3 && skillNum <= 5) {//ヒーラン系
            playSE(8, 0);
            monAni.SetBool("magic", true);
            effectCreate(Effect[3], monster[monsterNum].transform.Find("shade").gameObject.transform.position, 1);
            yield return new WaitForSeconds(50 / 60f);
            playSE(6 + skillNum, 0);
            effectCreate(Effect[4], monster[targetNum].transform.position + new Vector3(0, 0.5f, -1), targetNum < 4 ? 1 : -1);
            damage = DamageCaluculate(monsterNum, targetNum, 0, 75 * (skillNum - 2), 0, false,0);
            Damage(monsterNum, targetNum, damage, 3, "",0, false);
            yield return new WaitForSeconds(60 / 60f);
            monAni.SetBool("magic", false);
        } else if (skillNum >= 6 && skillNum <= 8) {//テトラヒーラン系
            playSE(8, 0);
            monAni.SetBool("magic", true);
            effectCreate(Effect[3], monster[monsterNum].transform.Find("shade").gameObject.transform.position, 1);
            yield return new WaitForSeconds(50 / 60f);
            playSE(3 + skillNum, 0);
            for (targetNum = monsterNum < 4 ? 0 : 4; targetNum < (monsterNum < 4 ? 4 : 8); targetNum++) {
                if (Attackable(targetNum)) {
                    effectCreate(Effect[4], monster[targetNum].transform.position + new Vector3(0, 0.5f, -1), targetNum < 4 ? 1 : -1);
                    damage = DamageCaluculate(monsterNum, targetNum, 0, 50 * (skillNum - 5), 0, false,0);
                    Damage(monsterNum, targetNum, damage, 3, "",0, false);
                }
            }
            yield return new WaitForSeconds(60 / 60f);
            monAni.SetBool("magic", false);
        } else if (skillNum == 9) {//リバイブ
            playSE(8, 0);
            monAni.SetBool("magic", true);
            effectCreate(Effect[3], monster[monsterNum].transform.Find("shade").gameObject.transform.position, 1);
            yield return new WaitForSeconds(50 / 60f);
            playSE(12, 0);
            effectCreate(Effect[7], monster[targetNum].transform.position + new Vector3(0, 1, -1), targetNum < 4 ? 1 : -1);
            if (BatDB.death[targetNum]) {
                damage = DamageCaluculate(monsterNum, targetNum, 0, 100, 0, false,0);
                Damage(monsterNum, targetNum, damage, 3, "",0, false);
                BatDB.death[targetNum] = false;
                if (targetNum >= 4) GameObject.Find("AllyName" + (targetNum - 3).ToString()).GetComponent<Text>().color = new Color(0, 0, 0);
                else GameObject.Find("EnemyName" + (targetNum + 1).ToString()).GetComponent<Text>().color = new Color(0, 0, 0);
                monster[targetNum].GetComponent<Monster>().life();
            }
            BatDB.behavior[targetNum, 0] = -2;
            BatDB.behavior[targetNum, 1] = -2;
            yield return new WaitForSeconds(60 / 60f);
            monAni.SetBool("magic", false);
        } else if (skillNum >= 10 && skillNum <= 18) {//フレイマン・ウォータン・エレカン系
            int[] attri = { 0b1, 0b1, 0b1, 0b10, 0b10, 0b10, 0b100, 0b100, 0b100 };
            playSE(8, 0);
            monAni.SetBool("magic", true);
            effectCreate(Effect[3], monster[monsterNum].transform.Find("shade").gameObject.transform.position, 1);
            yield return new WaitForSeconds(50 / 60f);

            playSE(8 + skillNum, 0);
            effectCreate(Effect[skillNum], monster[targetNum].transform.position + new Vector3(0, 1, -1), targetNum < 4 ? 1 : -1);
            damage = DamageCaluculate(monsterNum, targetNum, 0, 125 + ((skillNum - 10) % 3)*75, 100, false,attri[skillNum-10]);
            Damage(monsterNum, targetNum, damage, 0, "",100, attri[skillNum - 10], false);
            yield return new WaitForSeconds(60 / 60f);
            monAni.SetBool("magic", false);
        } else if (skillNum >= 19 && skillNum <= 27) {//テトラフレイマン・ウォータン・エレカン系
            int[] attri = { 0b1,0b1,0b1,0b10,0b10,0b10,0b100,0b100,0b100};
            playSE(8, 0);
            monAni.SetBool("magic", true);
            effectCreate(Effect[3], monster[monsterNum].transform.Find("shade").gameObject.transform.position, 1);
            yield return new WaitForSeconds(50 / 60f);
            playSE(-1 + skillNum, 0);
            for (targetNum = monsterNum < 4 ? 4 : 0; targetNum < (monsterNum < 4 ? 8 : 4); targetNum++) {
                if (Attackable(targetNum)) {
                    effectCreate(Effect[skillNum - 9], monster[targetNum].transform.position + new Vector3(0, 1, -1), targetNum < 4 ? 1 : -1);
                    damage = DamageCaluculate(monsterNum, targetNum, 0, 90 + ((skillNum - 19) % 3)*60, 100, false,attri[skillNum-19]);
                    Damage(monsterNum, targetNum, damage, 0, "",95, attri[skillNum - 19], false);
                }
            }
            yield return new WaitForSeconds(60 / 60f);
            monAni.SetBool("magic", false);
        } else if (skillNum >= 28 && skillNum <= 32) {//絶対零度・律速段階・励起・放射性崩壊・分析
            playSE(8, 0);
            monAni.SetBool("magic", true);
            effectCreate(Effect[3], monster[monsterNum].transform.Find("shade").gameObject.transform.position, 1);
            yield return new WaitForSeconds(50 / 60f);


            effectCreate(Effect[skillNum - 8], monster[targetNum].transform.position + new Vector3(0, skillNum == 32 ? 0 : 1, -1), targetNum < 4 ? 1 : -1);
            playSE(skillNum - 1, 0);

            yield return new WaitForSeconds(60 / 60f);
            string[] buff = { "絶対零度", "律速段階", "励起状態", "放射性崩壊", "分析完了" };
            int[] hitPer = { 40,75,50,80,80};
            if (Damage(monsterNum, targetNum, 0, -1, buff[skillNum - 28], hitPer[skillNum - 28], 0, false)) {
                Buff(targetNum, skillNum - 11, 5 + (skillNum == 32 ? 5 : 0));
                playSE(32, 0);
            }
            yield return new WaitForSeconds(60 / 60f);
            monAni.SetBool("magic", false);
        } else if (skillNum >= 33 && skillNum <= 42) {//ステータスUpDown
            GameObject eff;
            Color[] col = { new Color(1,0.3f,0.3f,1),
                            new Color(1, 1, 0.3f, 1),
                            new Color(1, 0.3f, 1, 1),
                            new Color(1, 0.7f, 0.3f, 1),
                            new Color(0.3f, 1, 0.3f, 1)};
            playSE(8, 0);
            monAni.SetBool("magic", true);
            effectCreate(Effect[3], monster[monsterNum].transform.Find("shade").gameObject.transform.position, 1);
            yield return new WaitForSeconds(30 / 60f);
            eff = effectCreate(Effect[25], monster[targetNum].transform.position + new Vector3(0, 0, -1), targetNum < 4 ? 1 : -1);
            eff.GetComponent<SpriteRenderer>().color = col[(skillNum - 33) % 5];
            yield return new WaitForSeconds(60 / 60f);
            string[] buff = { "攻撃", "防御", "魔法", "魔防", "素早さ" };
            Damage(monsterNum, targetNum, 0, -1, buff[(skillNum - 33) % 5] + (skillNum <= 37 ? "UP" : "DOWN"),0, false);
            Buff(targetNum, (skillNum - 33) % 5 * 2 + (skillNum <= 37 ? 1 : 2), 5);
            effectCreate(Effect[skillNum <= 37 ? 26 : 27], monster[targetNum].transform.position + new Vector3(0, 1, -1), targetNum < 4 ? 1 : -1);
            playSE(skillNum <= 37 ? 33 : 34, 0);
            yield return new WaitForSeconds(60 / 60f);
            monAni.SetBool("magic", false);
        } else if (skillNum == 43 || skillNum == 44) {//デバフ消去,バフ消去
            GameObject eff;
            List<int> debuffList;
            if (skillNum == 43) debuffList = new List<int>() { 2, 4, 6, 8, 10, 17, 18, 19, 20, 21, 25, 29, 30, 31 };//デバフ
            else debuffList = new List<int>() { 1, 3, 5, 7, 9, 22, 23, 24, 26, 27, 28, 32, 33, 34, 35, 36, 37, 38 };//バフ
            playSE(8, 0);
            monAni.SetBool("magic", true);
            effectCreate(Effect[3], monster[monsterNum].transform.Find("shade").gameObject.transform.position, 1);
            yield return new WaitForSeconds(50 / 60f);
            playSE(skillNum == 43 ? 35 : 36, 0);
            eff = effectCreate(Effect[skillNum == 43 ? 28 : 29], monster[targetNum].transform.position + new Vector3(0, 0.5f, -1), targetNum < 4 ? 1 : -1);
            if (skillNum == 44) eff.GetComponent<SpriteRenderer>().color = new Color(1,0.4f,1,1);
            foreach (int i in debuffList) {
                if (BatDB.monsterBuff[targetNum, i] < 1000) {
                    BatDB.monsterBuff[targetNum, i] = 0;
                    if(i==17 && BatDB.monsterBuff[targetNum, 0] <= 0)monster[targetNum].GetComponent<Animator>().speed = 1;
                }
            }
            yield return new WaitForSeconds(60 / 60f);
            monAni.SetBool("magic", false);
        } else if (skillNum == 45) {//加熱加圧
            playSE(8, 0);
            monAni.SetBool("magic", true);
            effectCreate(Effect[3], monster[monsterNum].transform.Find("shade").gameObject.transform.position, 1);
            yield return new WaitForSeconds(50 / 60f);
            playSE(37, 0);
            effectCreate(Effect[30], monster[targetNum].transform.position + new Vector3(0, 0.5f, -1), targetNum < 4 ? 1 : -1);
            yield return new WaitForSeconds(60 / 60f);
            BatDB.monsterBuff[targetNum, 23] += 2;
            if (BatDB.monsterBuff[targetNum, 23] >= 4) {
                yield return new WaitForSeconds(30 / 60f);
                playSE(19, 0);
                BatDB.monsterBuff[targetNum, 23] = 0;
                Buff(targetNum, 22, 5);
                Damage(monsterNum, targetNum, 0, -1, "超臨界流体",0, false);
                yield return new WaitForSeconds(30 / 60f);
            }
            monAni.SetBool("magic", false);
        } else if (skillNum == 46) {//エントロピー増大
            List<int> targetList = new List<int>();
            targetList = target(monsterNum<4);
            SysDB.animationFlag = true;
            monAni.SetBool("attack", true);
            Delay = mon.AttackDelay;
            effect = mon.AttackEffect;
            se = mon.AttackSE;
            move = new Vector3((monsterNum >= 4 ? -1 : 1) * 0.3f, 0, 0);
            StartCoroutine(MoveGameObject(monster[monsterNum], move, 5, 0));
            yield return new WaitForSeconds((Delay + 5) / 60f);
            if (effect != null) {
                for (int i = 0; i < 5; i++) {
                    targetNum = targetList[RandomInt(0,targetList.Count-1,monsterNum>=4)];
                    effectCreate(effect, monster[targetNum].transform.position + new Vector3(0, 0.5f, -1), targetNum < 4 ? 1 : -1);
                    damage = DamageCaluculate(monsterNum, targetNum, 50, 0, 100, true,0);
                    if (Damage(monsterNum, targetNum, damage*RandomInt(90,110,monsterNum>=4)/100, 0, "", 95,0,true)) {
                        if (se != null) audioS.PlayOneShot(se, 3);
                    }
                    yield return new WaitForSeconds(10/60f);
                }
            }
            while (SysDB.animationFlag) yield return null;
            StartCoroutine(MoveGameObject(monster[monsterNum], -move, 5, 0));
        }else if (skillNum >=47 && skillNum <= 49) {//炎水電気攻撃
            int[] attri = { 0b1, 0b10, 0b100 };
            SysDB.animationFlag = true;
            monAni.SetBool("attack", true);
            Delay = mon.AttackDelay;
            move = new Vector3((monsterNum >= 4 ? -1 : 1) * 0.3f, 0, 0);
            StartCoroutine(MoveGameObject(monster[monsterNum], move, 5, 0));
            yield return new WaitForSeconds((Delay + 5) / 60f);
            effectCreate(Effect[skillNum-16], monster[targetNum].transform.position + new Vector3(0, 0.5f, -1), targetNum < 4 ? 1 : -1);
            damage = DamageCaluculate(monsterNum, targetNum, 120, 0, 100, true,attri[skillNum-47]);
            if (Damage(monsterNum, targetNum, damage, 0, "", 95, attri[skillNum - 47], true)) playSE(38, 0);
            yield return new WaitForSeconds(70 / 60f);
            while (SysDB.animationFlag) yield return null;
            StartCoroutine(MoveGameObject(monster[monsterNum], -move, 5, 0));
        } else if (skillNum >= 50 && skillNum <= 54) {//全体強化系
            string[] buff = { "攻撃", "防御", "魔法", "魔防", "素早さ" };
            playSE(8, 0);
            monAni.SetBool("magic", true);
            effectCreate(Effect[3], monster[monsterNum].transform.Find("shade").gameObject.transform.position, 1);
            yield return new WaitForSeconds(50 / 60f);
            effectCreate(Effect[skillNum-16], monster[monsterNum].transform.position + new Vector3(0, 0.5f, -1), targetNum < 4 ? 1 : -1);
            playSE(skillNum-10, 0);
            yield return new WaitForSeconds(50 / 60f);
            playSE(33, 0);
            for (targetNum = monsterNum < 4 ? 0 : 4; targetNum < (monsterNum < 4 ? 4 : 8); targetNum++) {
                if (Attackable(targetNum)) {
                    Damage(monsterNum, targetNum, 0, -1, buff[skillNum - 50] + "UP",0, false);
                    Buff(targetNum, (skillNum - 50) * 2 +1, 5);
                    effectCreate(Effect[26], monster[targetNum].transform.position + new Vector3(0, 1, -1), targetNum < 4 ? 1 : -1);
                }
            }
            yield return new WaitForSeconds(60 / 60f);
            monAni.SetBool("magic", false);
        } else if (skillNum == 55||skillNum==57) {//ブラウン運動,燃焼(光)
            playSE(8, 0);
            monAni.SetBool("magic", true);
            effectCreate(Effect[3], monster[monsterNum].transform.Find("shade").gameObject.transform.position, 1);
            yield return new WaitForSeconds(30 / 60f);
            effectCreate(Effect[39 + (skillNum == 55 ? 0 : 2)], monster[targetNum].transform.position + new Vector3(0, 0, 2.5f), targetNum < 4 ? 1 : -1);
            playSE(45 + (skillNum == 55 ? 0 : 2), 0);
            yield return new WaitForSeconds(60 / 60f);
            Damage(monsterNum, targetNum, 0, -1, skillNum==55?"回避UP":"命中DOWN",0, false);
            Buff(targetNum, skillNum == 55 ? 24 : 25, 5);
            effectCreate(Effect[26+(skillNum==55?0:1)], monster[targetNum].transform.position + new Vector3(0, 1, -1), targetNum < 4 ? 1 : -1);
            playSE(33 + (skillNum == 55 ? 0 : 1), 0);
            yield return new WaitForSeconds(60 / 60f);
            monAni.SetBool("magic", false);
        } else if (skillNum == 56 || skillNum == 58) {//全体弱化系
            playSE(8, 0);
            monAni.SetBool("magic", true);
            effectCreate(Effect[3], monster[monsterNum].transform.Find("shade").gameObject.transform.position, 1);
            yield return new WaitForSeconds(50 / 60f);
            for (targetNum = monsterNum >= 4 ? 0 : 4; targetNum < (monsterNum >= 4 ? 4 : 8); targetNum++) {
                if (Attackable(targetNum)) {
                    effectCreate(Effect[skillNum == 56 ? 40 : 42], monster[targetNum].transform.position + new Vector3(0, 0.5f, -1), targetNum < 4 ? 1 : -1);
                }
            }
            playSE(skillNum == 56 ? 46 : 48, 0);
            yield return new WaitForSeconds(50 / 60f);
            playSE(34, 0);
            for (targetNum = monsterNum >= 4 ? 0 : 4; targetNum < (monsterNum >= 4 ? 4 : 8); targetNum++) {
                if (Attackable(targetNum)) {
                    Damage(monsterNum, targetNum, 0, -1, skillNum==56?"素早さDOWN":"命中DOWN",0, false);
                    Buff(targetNum, skillNum==56?10:25, 5);
                    effectCreate(Effect[27], monster[targetNum].transform.position + new Vector3(0, 1, -1), targetNum < 4 ? 1 : -1);
                }
            }
            yield return new WaitForSeconds(60 / 60f);
            monAni.SetBool("magic", false);
        } else if (skillNum >= 59 && skillNum <= 62) {//酸塩基
            playSE(8, 0);
            monAni.SetBool("magic", true);
            effectCreate(Effect[3], monster[monsterNum].transform.Find("shade").gameObject.transform.position, 1);
            yield return new WaitForSeconds(50 / 60f);
            effectCreate(Effect[29], monster[targetNum].transform.position + new Vector3(0, skillNum == 32 ? 0 : 1, -1), targetNum < 4 ? 1 : -1);
            playSE(36, 0);
            yield return new WaitForSeconds(60 / 60f);
            int[] buffid = { 12,14,11,13};
            string[] buff = { "強酸性", "強塩基性", "弱酸性", "弱塩基性" };
            if (Damage(monsterNum, targetNum, 0, -1, buff[skillNum - 59], 95,0, false)) {
                Buff(targetNum, buffid[skillNum - 59], 5);
                playSE(32, 0);
            }
            yield return new WaitForSeconds(60 / 60f);
            monAni.SetBool("magic", false);
        } else if (skillNum == 63) {//酸性雨
            playSE(8, 0);
            monAni.SetBool("magic", true);
            effectCreate(Effect[3], monster[monsterNum].transform.Find("shade").gameObject.transform.position, 1);
            yield return new WaitForSeconds(50 / 60f);
            for (targetNum = monsterNum >= 4 ? 0 : 4; targetNum < (monsterNum >= 4 ? 4 : 8); targetNum++) {
                if (Attackable(targetNum)) {
                    Buff(targetNum, 11, 5);
                    effectCreate(Effect[43], monster[targetNum].transform.position + new Vector3(0, skillNum == 32 ? 0 : 1, -1), targetNum < 4 ? 1 : -1);
                    damage = DamageCaluculate(monsterNum, targetNum, 0, 75, 100, false, 0b10);
                    Damage(monsterNum, targetNum, damage, 0, "", 95,0b10, false);
                }
            }
            playSE(49, 0);
            yield return new WaitForSeconds(50 / 60f);
            for (targetNum = monsterNum >= 4 ? 0 : 4; targetNum < (monsterNum >= 4 ? 4 : 8); targetNum++) {
                if (Attackable(targetNum)) {
                    Damage(monsterNum, targetNum, 0, -1, "弱酸性", 100, false);
                }
            }
            yield return new WaitForSeconds(60 / 60f);
            monAni.SetBool("magic", false);
        } else if (skillNum >= 64 && skillNum <= 66) {//炎色反応・水鉄砲・自由電子
            int[] attri = { 0b1, 0b10, 0b100 };
            playSE(8, 0);
            monAni.SetBool("magic", true);
            effectCreate(Effect[3], monster[monsterNum].transform.Find("shade").gameObject.transform.position, 1);
            yield return new WaitForSeconds(50 / 60f);

            if(skillNum==64)playSE(19, 0);
            else if (skillNum == 65) playSE(22, 0);
            else if (skillNum == 66) playSE(25, 0);
            effectCreate(Effect[skillNum-20], monster[targetNum].transform.position + new Vector3(0, 1, -1), targetNum < 4 ? 1 : -1);
            damage = DamageCaluculate(monsterNum, targetNum, 0, 200, 100, false, attri[skillNum-64]);
            Damage(monsterNum, targetNum, damage, 0, "",100, attri[skillNum - 64], false);
            yield return new WaitForSeconds(60 / 60f);
            monAni.SetBool("magic", false);
        } else if (skillNum >= 67 && skillNum <= 72) {//封印・吸収
            GameObject eff;
            Color[] col = { new Color(1,0.3f,0.3f,1),
                            new Color(0.3f, 0.3f, 1, 1),
                            new Color(1, 1, 0.3f, 1)};
            playSE(8, 0);
            monAni.SetBool("magic", true);
            effectCreate(Effect[3], monster[monsterNum].transform.Find("shade").gameObject.transform.position, 1);
            yield return new WaitForSeconds(50 / 60f);

            eff=effectCreate(Effect[skillNum<=69?47:48], monster[targetNum].transform.position + new Vector3(0, skillNum == 32 ? 0 : 1, -1), targetNum < 4 ? 1 : -1);
            eff.GetComponent<SpriteRenderer>().color = col[(skillNum-67)%3];
            playSE(skillNum <= 69 ? 34 : 33, 0);

            yield return new WaitForSeconds(10 / 60f);
            string[] buff = { "炎", "水", "電気" };
            if (Damage(monsterNum, targetNum, 0, -1, buff[(skillNum - 67)%3]+(skillNum<=69?"封印":"吸収"), 100,0, false)) {
                Buff(targetNum, skillNum<=69?(skillNum-38): (skillNum - 44), 5);
                //playSE(32, 0);
            }
            yield return new WaitForSeconds(60 / 60f);
            monAni.SetBool("magic", false);
        }else if (skillNum == 73 || skillNum == 74) {//軽金属・重金属
            GameObject eff;
            SysDB.animationFlag = true;
            monAni.SetBool("attack", true);
            Delay = mon.AttackDelay;
            effect = mon.AttackEffect;
            se = mon.AttackSE;
            move = new Vector3((monsterNum >= 4 ? -1 : 1) * 0.3f, 0, 0);
            StartCoroutine(MoveGameObject(monster[monsterNum], move, 5, 0));
            yield return new WaitForSeconds((Delay + 5) / 60f);
            if (effect != null) {
                eff = effectCreate(effect, monster[targetNum].transform.position + new Vector3(0, 0.5f, -1), targetNum < 4 ? 1 : -1);
                eff.transform.localScale *= (skillNum == 73 ? 0.75f : 1.4f);
                damage = DamageCaluculate(monsterNum, targetNum, skillNum==73?75:150, 0, 100, true,0);
                if (Damage(monsterNum, targetNum, damage, 0, "", 95,0, false)) {
                    if (se != null) audioS.PlayOneShot(se, 3);
                }

            }
            while (SysDB.animationFlag) yield return null;
            StartCoroutine(MoveGameObject(monster[monsterNum], -move, 5, 0));
        } else if (skillNum == 75|| skillNum == 76) {//ネオンサイン,燐光
            GameObject eff;
            playSE(8, 0);
            monAni.SetBool("magic", true);
            effectCreate(Effect[3], monster[monsterNum].transform.Find("shade").gameObject.transform.position, 1);
            yield return new WaitForSeconds(50 / 60f);
            playSE(48, 0);
            eff = effectCreate(Effect[49], monster[targetNum].transform.position + new Vector3(0, skillNum == 32 ? 0 : 1, -1), targetNum < 4 ? 1 : -1);
            eff.GetComponent<SpriteRenderer>().color = skillNum == 75 ? new Color(1f, 0.4f, 0.8f, 1) : new Color(0.7f, 0.7f, 1f, 1);
            Damage(monsterNum, targetNum, 0, -1, "注目", 100, false);
            Buff(targetNum, 33, 5);
            yield return new WaitForSeconds(60 / 60f);
            monAni.SetBool("magic", false);
        } else if (skillNum == 77 || skillNum == 78) {//刺激臭,腐卵臭
            GameObject eff;
            playSE(8, 0);
            monAni.SetBool("magic", true);
            effectCreate(Effect[3], monster[monsterNum].transform.Find("shade").gameObject.transform.position, 1);
            yield return new WaitForSeconds(50 / 60f);
            playSE(50, 0);
            eff = effectCreate(Effect[50], monster[targetNum].transform.position + new Vector3(0, skillNum == 32 ? 0 : 1, -1), targetNum < 4 ? 1 : -1);
            eff.GetComponent<SpriteRenderer>().color = skillNum == 75 ? new Color(1f, 0.4f, 0.8f, 1) : new Color(0.7f, 0.7f, 1f, 1);
            Damage(monsterNum, targetNum, 0, -1, "異臭", 100, false);
            Buff(targetNum, 34, 5);
            yield return new WaitForSeconds(60 / 60f);
            monAni.SetBool("magic", false);
        } else if (skillNum >= 79 && skillNum <= 81) {//酸化還元王水
            GameObject eff;
            int[] attri = { 0b1000, 0b10000, 0b100000 };
            playSE(8, 0);
            monAni.SetBool("magic", true);
            effectCreate(Effect[3], monster[monsterNum].transform.Find("shade").gameObject.transform.position, 1);
            yield return new WaitForSeconds(50 / 60f);
            playSE(skillNum <= 80 ? 51 : 52, 0);
            eff = effectCreate(Effect[skillNum<=80?51:52], monster[targetNum].transform.position + new Vector3(0, skillNum == 32 ? 0 : 1, -1), targetNum < 4 ? 1 : -1);
            if(skillNum==79)eff.GetComponent<SpriteRenderer>().color = new Color(1f, 0.7f, 0.7f, 1);
            damage = DamageCaluculate(monsterNum, targetNum, 0, skillNum==8?150:120, 100, true, attri[skillNum-79]);
            Damage(monsterNum, targetNum, damage, 0, "", 95, false);
            yield return new WaitForSeconds(60 / 60f);
            monAni.SetBool("magic", false);
        } else if (skillNum == 82 || skillNum == 83) {//パンケーキ缶詰
            playSE(8, 0);
            monAni.SetBool("magic", true);
            effectCreate(Effect[3], monster[monsterNum].transform.Find("shade").gameObject.transform.position, 1);
            yield return new WaitForSeconds(50 / 60f);
            playSE(10, 0);
            effectCreate(Effect[53], monster[targetNum].transform.position + new Vector3(0, 0.5f, -1), targetNum < 4 ? 1 : -1);
            damage = DamageCaluculate(monsterNum, targetNum, 75, 75, 0, false,0);
            Damage(monsterNum, targetNum, damage, 3, "",0, false);
            yield return new WaitForSeconds(60 / 60f);
            monAni.SetBool("magic", false);
        } else if (skillNum == 84) {//肥料
            GameObject eff;
            playSE(8, 0);
            monAni.SetBool("magic", true);
            effectCreate(Effect[3], monster[monsterNum].transform.Find("shade").gameObject.transform.position, 1);
            yield return new WaitForSeconds(50 / 60f);
            playSE(53, 0);
            eff = effectCreate(Effect[53], monster[targetNum].transform.position + new Vector3(0, skillNum == 32 ? 0 : 1, -1), targetNum < 4 ? 1 : -1);
            eff.transform.localScale*=0.7f;
            Damage(monsterNum, targetNum, 0, -1, "肥料", 100, false);
            Buff(targetNum, 35, 5);
            yield return new WaitForSeconds(60 / 60f);
            monAni.SetBool("magic", false);
        } else if (skillNum == 85 || skillNum == 86) {//水素爆発・温暖化
            GameObject eff;
            playSE(8, 0);
            monAni.SetBool("magic", true);
            effectCreate(Effect[3], monster[monsterNum].transform.Find("shade").gameObject.transform.position, 1);
            yield return new WaitForSeconds(50 / 60f);
            playSE(skillNum==85?54:48, 0);
            for (targetNum = monsterNum < 4 ? 4 : 0; targetNum < (monsterNum < 4 ? 8 : 4); targetNum++) {
                if (Attackable(targetNum)) {
                    eff = effectCreate(Effect[skillNum == 85 ? 54 : 50], monster[targetNum].transform.position + new Vector3(0, 1, -1), targetNum < 4 ? 1 : -1);
                    if (skillNum == 86) eff.GetComponent<SpriteRenderer>().color = new Color(1,0.3f,0.3f,0.1f);
                    damage = DamageCaluculate(monsterNum, targetNum, skillNum==85?100:0, skillNum == 86 ? 100 : 0, 100, false,0b1);
                    Damage(monsterNum, targetNum, damage, 0, "",0b1, false);
                }
            }
            yield return new WaitForSeconds(60 / 60f);
            monAni.SetBool("magic", false);
        } else if (skillNum >= 87 && skillNum <= 94) {//発熱反応,テルミット...鍾乳石
            int[] effNum = { 50,55,56,57,58,59,60,61};
            int[] seNum = { 48,55,56,18,26,24,43,57};
            int[] attri = { 0b1, 0b1, 0b1, 0b1, 0b101, 0b100, 0b1000, 0b0 };
            int[] phy = {  0,  0,250,125,  0,120,200,270 };
            int[] mag = {230,230,  0,125,200,120,  0,  0 };
            GameObject eff;
            playSE(8, 0);
            monAni.SetBool("magic", true);
            effectCreate(Effect[3], monster[monsterNum].transform.Find("shade").gameObject.transform.position, 1);
            yield return new WaitForSeconds(50 / 60f);

            playSE(seNum[skillNum-87], 0);
            eff = effectCreate(Effect[effNum[skillNum - 87]], monster[targetNum].transform.position + new Vector3(0, 1, -1), targetNum < 4 ? 1 : -1);
            if (skillNum == 87) eff.GetComponent<SpriteRenderer>().color = new Color(1, 0.3f, 0.3f, 0.1f);
            if (skillNum == 90) eff.GetComponent<SpriteRenderer>().color = new Color(0.6f, 0.6f, 0.6f, 1);
            damage = DamageCaluculate(monsterNum, targetNum, phy[skillNum - 87], mag[skillNum-87], 100, false, attri[skillNum - 87]);
            Damage(monsterNum, targetNum, damage, 0, "",95, attri[skillNum - 87], false);
            yield return new WaitForSeconds(60 / 60f);
            monAni.SetBool("magic", false);
        } else if (skillNum >= 95 && skillNum <= 98) {//不活性ガス,ガラス,加硫,ATP
            int[] effNum = { 62,63,64,65 };
            int[] seNum = { 39,39,58,59 };
            int[] buffNum = { 36,37,32,38 };
            string[] buffN = { "無敵","ガラス","耐久","ATP" };
            GameObject eff;
            playSE(8, 0);
            monAni.SetBool("magic", true);
            effectCreate(Effect[3], monster[monsterNum].transform.Find("shade").gameObject.transform.position, 1);
            yield return new WaitForSeconds(50 / 60f);

            playSE(seNum[skillNum - 95], 0);
            eff = effectCreate(Effect[effNum[skillNum - 95]], monster[targetNum].transform.position + new Vector3(0, skillNum==97?0:1, -1), targetNum < 4 ? 1 : -1);
            Damage(monsterNum, targetNum, 0, -1, buffN[skillNum - 95],0, false);
            Buff(targetNum, buffNum[skillNum - 95], skillNum==95?1:5);
            yield return new WaitForSeconds(60 / 60f);
            monAni.SetBool("magic", false);
        } else if (skillNum ==99) {//融雪
            playSE(8, 0);
            monAni.SetBool("magic", true);
            effectCreate(Effect[3], monster[monsterNum].transform.Find("shade").gameObject.transform.position, 1);
            yield return new WaitForSeconds(50 / 60f);
            playSE(35, 0);
            for (targetNum = monsterNum < 4 ? 0 : 4; targetNum < (monsterNum < 4 ? 4 : 8); targetNum++) {
                if (Attackable(targetNum)) {
                    Damage(monsterNum, targetNum, 0, -1, "絶対零度解除",0, false);
                    if (BatDB.monsterBuff[targetNum, 17] > 0) {
                        BatDB.monsterBuff[targetNum, 17]=0;
                        if(BatDB.monsterBuff[targetNum, 0] == 0) {
                            monster[targetNum].GetComponent<Animator>().speed = 1;
                        }
                    }
                    effectCreate(Effect[66], monster[targetNum].transform.position + new Vector3(0, 1, -1), targetNum < 4 ? 1 : -1);
                }
            }
            yield return new WaitForSeconds(60 / 60f);
            monAni.SetBool("magic", false);
        } else if (skillNum == 100 || skillNum == 101) {//イオン化

            playSE(8, 0);
            monAni.SetBool("magic", true);
            effectCreate(Effect[3], monster[monsterNum].transform.Find("shade").gameObject.transform.position, 1);
            yield return new WaitForSeconds(60 / 60f);
            Damage(monsterNum, monsterNum, 0, -1, skillNum == 100 ? "陽イオン" : "陰イオン",0,false);
            effectCreate(Effect[skillNum == 100 ? 26 : 27], monster[monsterNum].transform.position + new Vector3(0, 1, -1), targetNum < 4 ? 1 : -1);
            playSE(30, 0);
            damage = DamageCaluculate(monsterNum, targetNum, 0, 150, 100, false,0);
            if (skillNum == 101 && BatDB.monsterBuff[monsterNum, 15] > 0 || skillNum == 100 && BatDB.monsterBuff[monsterNum, 16] > 0) {
                damage = damage * 3 / 2;
            }
            Damage(monsterNum, targetNum, damage, 0, "",0, false);
            Buff(monsterNum, skillNum == 100 ? 15 : 16, 5);
            if (skillNum == 100) {
                yield return new WaitForSeconds(30 / 60f);
                Damage(monsterNum,monsterNum, damage, 3, "",0, false);
            }
            yield return new WaitForSeconds(60 / 60f);
            monAni.SetBool("magic", false);
        }
        if (atpFlag) BatDB.monsterBuff[monsterNum, 38] = 0;
        movingFlag = false;
        yield return null;
    }
    IEnumerator itemUse() {//道具処理

        if (!Attackable(BatDB.itemTarget) && BatDB.itemTarget >= 0) {
            while (true) {
                if (!Attackable(BatDB.itemTarget) && ItemData.ItemDataList[BatDB.itemID].Type == 5) break;
                if (BatDB.itemTarget >= 4) BatDB.itemTarget = RandomInt(4, 7);
                else if (BatDB.itemTarget < 4 && BatDB.itemTarget >= 0) BatDB.itemTarget = RandomInt(0, 3);
                if (Attackable(BatDB.itemTarget)) break;
            }
        }
        skillName[0].main(ItemData.ItemDataList[BatDB.itemID].Name);
        yield return new WaitForSeconds(20 / 60f);

        if (BatDB.itemID == 0 || BatDB.itemID == 1 || BatDB.itemID == 2) {//希HP薬,HP薬,濃HP薬
            int[] damage = { 30, 120, 400 };
            playSE(10 + BatDB.itemID, 0);
            effectCreate(Effect[4], monster[BatDB.itemTarget].transform.position + new Vector3(0, 0.5f, -1), BatDB.itemTarget < 4 ? 1 : -1);
            Damage(-1, BatDB.itemTarget, damage[BatDB.itemID], 3, "",0, false);
            yield return new WaitForSeconds(60 / 60f);
        } else if (BatDB.itemID == 3 || BatDB.itemID == 4 || BatDB.itemID == 5) {//希SP薬,SP薬,濃SP薬
            int[] damage = { 30, 100, 200 };
            playSE(6 + BatDB.itemID, 0);
            effectCreate(Effect[6], monster[BatDB.itemTarget].transform.position + new Vector3(0, 0.5f, -1), BatDB.itemTarget < 4 ? 1 : -1);
            Damage(-1, BatDB.itemTarget, damage[BatDB.itemID - 3], 4, "",0, false);
            yield return new WaitForSeconds(60 / 60f);
        } else if (BatDB.itemID == 6 || BatDB.itemID == 7) {//希復活薬,濃復活薬
            int damage;
            playSE(12, 0);
            effectCreate(Effect[7], monster[BatDB.itemTarget].transform.position + new Vector3(0, 1, -1), BatDB.itemTarget < 4 ? 1 : -1);
            if (BatDB.death[BatDB.itemTarget]) {
                damage = BatDB.States[BatDB.itemTarget, 2];
                if (BatDB.itemID == 6) damage = (int)Mathf.Floor(damage / 2);
                Damage(-1,BatDB.itemTarget, damage, 3, "",0, false);
                BatDB.death[BatDB.itemTarget] = false;
                if (BatDB.itemTarget >= 4) GameObject.Find("AllyName" + (BatDB.itemTarget - 3).ToString()).GetComponent<Text>().color = new Color(0, 0, 0);
                else GameObject.Find("EnemyName" + (BatDB.itemTarget + 1).ToString()).GetComponent<Text>().color = new Color(0, 0, 0);
                monster[BatDB.itemTarget].GetComponent<Monster>().life();
            }
            yield return new WaitForSeconds(60 / 60f);
        } else if (BatDB.itemID >= 42 && BatDB.itemID <= 48) {//デバフ消去系
            int[] reset = { 14, 12, 17,18,19,20 };
            string[] debuffname = { "塩基性", "酸性", "絶対零度", "律速段階","励起状態","放射性崩壊","能力DOWN"};
            playSE(35, 0);
            effectCreate(Effect[28], monster[BatDB.itemTarget].transform.position + new Vector3(0, 0.5f, -1), BatDB.itemTarget < 4 ? 1 : -1);
            Damage(-1, BatDB.itemTarget, 0, -1, debuffname[BatDB.itemID-42]+"解除", 0, false);
            if (BatDB.itemID <= 47 && BatDB.monsterBuff[BatDB.itemTarget, reset[BatDB.itemID - 42]]<1000) {
                BatDB.monsterBuff[BatDB.itemTarget, reset[BatDB.itemID - 42]] = 0;
                if (BatDB.itemID == 44 && BatDB.monsterBuff[BatDB.itemTarget, 0] <= 0) {
                    monster[BatDB.itemTarget].GetComponent<Animator>().speed = 1;
                }
            }
            if (BatDB.itemID <= 43 && BatDB.monsterBuff[BatDB.itemTarget, reset[BatDB.itemID - 42] - 1]<1000) {
                BatDB.monsterBuff[BatDB.itemTarget, reset[BatDB.itemID - 42]-1] = 0;
            }
            if (BatDB.itemID == 48) {
                int[] debuffid = { 2,4,6,8,10,25};
                for(int i=0;i<6;i++)BatDB.monsterBuff[BatDB.itemTarget, debuffid[i]] = 0;
            }
            yield return new WaitForSeconds(60 / 60f);
        } else if (BatDB.itemID >= 49 && BatDB.itemID <= 54) {//バフ系
            int[] reset = { 1, 3, 5, 7, 9, 21 };
            string[] debuffname = { "攻撃UP", "防御UP", "魔法UP", "魔防UP", "素早さUP", "分析完了" };
            playSE(BatDB.itemID==54?31:33, 0);
            effectCreate(Effect[BatDB.itemID == 54 ? 24 : 26], monster[BatDB.itemTarget].transform.position + new Vector3(0, BatDB.itemID == 54 ? 0 : 0.5f, -1), BatDB.itemTarget < 4 ? 1 : -1);
            Damage(-1, BatDB.itemTarget, 0, -1, debuffname[BatDB.itemID - 49], 0, false);
            Buff(BatDB.itemTarget, reset[BatDB.itemID - 49], BatDB.itemID == 54 ? 10 : 5);
            yield return new WaitForSeconds(60 / 60f);
        } else if (BatDB.itemID == 26 || BatDB.itemID == 27 || BatDB.itemID == 28) {//捕集セット
            int percent, level = 99, difficulty = 0;
            bool getAble = true;
            playSE(14, 0);
            effectCreate(Effect[8], monster[BatDB.itemTarget].transform.position + new Vector3(0, 1, -1), BatDB.itemTarget < 4 ? 1 : -1);
            yield return new WaitForSeconds(40 / 60f);
            if (BatDB.itemTarget < 4) {
                difficulty = EnemyData.EnemyDataList[BatDB.Group[SysDB.Enemy, BatDB.itemTarget]].GetDiff;
                if (difficulty <= -100) getAble = false;
                difficulty += (BatDB.itemID - 26) * 5;
                difficulty += (BatDB.States[BatDB.itemTarget, 2] - BatDB.States[BatDB.itemTarget, 3]) * 10 / BatDB.States[BatDB.itemTarget, 2];
                level = BatDB.States[BatDB.itemTarget, 1];
                if (difficulty < 0) difficulty = 0;
                percent = 90 * 10 * (difficulty + 1) / (difficulty + level + 1);
            } else { getAble = false; percent = 0; }
            if (!getAble) percent = 0;
            Debug.Log(percent / 10f + "%");
            if (percent >= RandomInt(1, 1000)) {//成功
                int time = 10;
                AllyData resistData;
                bool empt = false;
                float audioVolume = audioS.volume;
                playSE(16, 0);
                yield return new WaitForSeconds(40 / 60f);
                for (int i = time - 1; i >= 0; i--) {
                    audioS.volume = audioVolume * i / time;
                    yield return new WaitForSeconds(1 / 60f);
                }
                audioS.loop = false;
                audioS.clip = BGM[8];
                audioS.Play();
                for (int i = 0; i < 4; i++) {
                    if (i == BatDB.itemTarget) continue;
                    if (monster[i] != null) {
                        monster[i].transform.localScale = new Vector3(1, 1, 1) * monsterSize;
                        StartCoroutine(MoveGameObject(monster[i], new Vector3(-move, 0, 0), 60, 0));
                        StartCoroutine(MoveGameObject(states[i], new Vector3(-260, 0, 0), 10, 20));
                    }

                }
                audioS.volume = audioVolume;
                yield return new WaitForSeconds(60 / 60f);
                yield return StartCoroutine(textShow("", BatDB.CharaName[BatDB.itemTarget] + "\nを捕集した！"));
                yield return StartCoroutine(textShow("", "モンスターボックスに送るモンスターを\n選択してください。"));

                selectText.text = "▶対象を選択◀";
                Button[0].transform.Find("Name").GetComponent<Text>().text = "送らない";
                Button[1].SetActive(false);
                Button[2].SetActive(false);
                Button[3].SetActive(false);
                skillWindow.SetActive(false);
                if (myDB.party[0] != -1 && myDB.party[1] != -1 && myDB.party[2] != -1 && myDB.party[3] != -1) {
                    Button[0].SetActive(false);
                    selectText.transform.position += new Vector3(0,40,0);
                    empt = true;
                }
                monsterSend = true;
                while (true) {
                    sendButtonNum = -1;
                    sendCheck = false;
                    while (sendButtonNum == -1 || sendButtonNum == 8 && empt) yield return null;
                    playSE(2, 0);

                    if (sendButtonNum != 8) {
                        sendCheck = true;
                        getWin.SetActive(true);
                        getButtonNum = -1;

                        getWin.transform.Find("Text").GetComponent<Text>().text = BatDB.CharaName[sendButtonNum] + "\nをモンスターボックスに送ります";
                        while (getButtonNum == -1) yield return null;
                        getWin.SetActive(false);
                        if (getButtonNum == 1)playSE(5, 0);
                        else {playSE(2, 0);break;}
                    } else break;
                }
                MonsterGet = true;
                yield return new WaitForSeconds(60 / 60f);
                for (int i = 0; i < 4; i++) {
                    if (myDB.party[i] >= 0) {
                        AllyData.AllyDataList[myDB.party[i]].NowHP = BatDB.States[i + 4, 3];
                        AllyData.AllyDataList[myDB.party[i]].NowSP = BatDB.States[i + 4, 5];
                    }
                }
                resistData = monsterResist(BatDB.itemTarget);
                if (sendButtonNum == 8) {
                    for(int i = 0; i < 4; i++) {
                        if (myDB.party[i] == -1) {
                            myDB.party[i] = i;
                            AllyData.AllyDataList[i] = resistData;
                            break;
                        }
                    }
                } else if(sendButtonNum>=4){
                    for(int i = 4; i < AllyData.AllyDataList.Count; i++) {
                        if (AllyData.AllyDataList[i].ID == -1 || i == AllyData.AllyDataList.Count - 1) {
                            AllyData.AllyDataList[i] = AllyData.AllyDataList[sendButtonNum-4];
                            AllyData.AllyDataList[sendButtonNum - 4] = resistData;
                            break;
                        }
                    }
                } else if (sendButtonNum < 4) {
                    for (int i = 4; i < AllyData.AllyDataList.Count; i++) {
                        if (AllyData.AllyDataList[i].ID == -1 || i== AllyData.AllyDataList.Count-1) {
                            AllyData.AllyDataList[i] = resistData;
                            break;
                        }
                    }
                }
                StatesData.Resister(resistData.ID, true,true);
                ItemData.ItemDataList[BatDB.itemID].Possess -= 1;
                yield return StartCoroutine(battleFinish());
                yield break;
            } else {
                if(getAble)yield return StartCoroutine(DamageCoroutine(BatDB.itemTarget, 0, 5, "", true, 0, false));//失敗
                else yield return StartCoroutine(DamageCoroutine(BatDB.itemTarget, 0, 9, "", true, 0, false));//無効
            }
        }
        ItemData.ItemDataList[BatDB.itemID].Possess -= 1;
        movingFlag = false;
        yield return null;
    }
    List<int> target(bool ally) {
        List<int> tList = new List<int>();

        for (int i = (ally ? 4 : 0); i < (ally ? 8 : 4); i++) {
            if (Attackable(i) && BatDB.monsterBuff[i, 33] > 0 && BatDB.monsterBuff[i, 34] <= 0) tList.Add(i);
        }

        if (tList.Count == 0) {
            for (int i = (ally ? 4 : 0); i < (ally ? 8 : 4); i++) {
                if (Attackable(i) && BatDB.monsterBuff[i, 34] <= 0) tList.Add(i);
            }
        }
        if (tList.Count == 0) {
            for (int i = (ally ? 4 : 0); i < (ally ? 8 : 4); i++) {
                if (Attackable(i)) tList.Add(i);
            }
        }
        return tList;
    }
    AllyData monsterResist(int num) {
        AllyData mon = new AllyData();
        EnemyData ene;
        MonsterData sta,maxMon;
        int rand;
        ene = EnemyData.EnemyDataList[BatDB.Group[SysDB.Enemy, num]];
        sta = StatesData.MonsterDataList[ene.ID];

        mon.Name = ene.Name;
        mon.ID = ene.ID;
        mon.Level = ene.Level;
        mon.NowHP = BatDB.States[num, 3];
        mon.NowSP = BatDB.States[num, 5];
        mon.HP = BatDB.States[num, 2] - Math.Max(ene.HP,0);
        mon.SP = BatDB.States[num, 4] - Math.Max(ene.SP, 0);
        if (mon.NowHP > mon.HP) mon.NowHP = mon.HP;
        if (mon.NowSP > mon.SP) mon.NowSP = mon.SP;
        mon.Attack = BatDB.States[num, 6] - Math.Max(ene.Attack, 0);
        mon.Defence = BatDB.States[num, 7] - Math.Max(ene.Defence, 0);
        mon.Magic = BatDB.States[num, 8] - Math.Max(ene.Magic, 0);
        mon.MagicDef = BatDB.States[num, 9] - Math.Max(ene.MagicDef, 0);
        mon.Speed = BatDB.States[num, 10] - Math.Max(ene.Speed, 0);

        maxMon = StatesData.MonsterDataList[mon.ID];
        mon.NowHP = mon.HP = Math.Min(mon.HP,maxMon.HPMax);
        mon.NowSP = mon.SP = Math.Min(mon.SP, maxMon.SPMax);
        mon.Attack = Math.Min(mon.Attack, maxMon.AttackMax);
        mon.Defence = Math.Min(mon.Defence, maxMon.DefenceMax);
        mon.Magic = Math.Min(mon.Magic, maxMon.MagicMax);
        mon.MagicDef = Math.Min(mon.MagicDef, maxMon.MagicDefMax);
        mon.Speed = Math.Min(mon.Speed, maxMon.SpeedMax);

        mon.Equip = -1;
        mon.CharaPt = ene.Level;
        mon.NextExp = 22;
        mon.Exp = 0;
        for (int i = 1; i <= mon.Level; i++) {
            mon.NextExp = Mathf.FloorToInt((1.625f / (i * 1.25f + 1.3f) + 1f) * mon.NextExp);
            mon.Exp += mon.NextExp-20;
        }
        mon.NextExp -= 20;
        rand = RandomInt(0,mon.NextExp-1);
        mon.Exp += rand;
        mon.NextExp -= rand;
        Array.Resize(ref mon.Skill, ene.Skill.Length);
        Array.Resize(ref mon.SkillUse, ene.Skill.Length);
        Array.Resize(ref mon.Character, StatesData.MonsterDataList[ene.ID].Character.Length);
        Array.Resize(ref mon.Grow, 7);
        for (int i = 0; i < mon.Character.Length; i++) mon.Character[i] = 1;
        mon.Grow[0] = sta.HPGrow;
        mon.Grow[1] = sta.SPGrow;
        mon.Grow[2] = sta.AttackGrow;
        mon.Grow[3] = sta.DefenceGrow;
        mon.Grow[4] = sta.MagicGrow;
        mon.Grow[5] = sta.MagicDefGrow;
        mon.Grow[6] = sta.SpeedGrow;
        for(int i=0;i< ene.Skill.Length; i++) {
            mon.Skill[i] = ene.Skill[i];
            mon.SkillUse[i] = 0;
        }
        return mon;
    }
    void Buff(int mon,int buff,int turn) {
        int acid=0;
        bool reset = false;
        string chara="";
        for (int i = 35; i <= 40; i++) {
            if (i == 39) continue;
            if (RandomInt(1, 100,mon>=4) <= charaLev(mon, i) * 20) {
                if (i == 35 && (buff == 11 || buff == 12)) chara = "酸性化";
                if (i == 36 && (buff == 13 || buff == 14)) chara = "塩基性化";
                if (i == 37 && (buff >= 11 && buff <= 14)) chara = buff <= 12 ? "酸性化" : "塩基性化";
                if (i == 38 && (buff == 29 && buff <= 31)) chara = "封印";
                if (i == 40 && buff == 25) chara = "命中DOWN";
            }
            if (chara != "") {
                reset = true;
                Damage(-1, mon, 0, -1, chara + "無効", 0, false, 30);
                playSE(60,30);
                break;
            }
        }
        if (!reset) {
            BatDB.monsterBuff[mon, buff] = turn;
            for (int i = 1; i < 10; i += 2) {
                if (BatDB.monsterBuff[mon, i] > 0 && BatDB.monsterBuff[mon, i + 1] > 0) {
                    BatDB.monsterBuff[mon, i] = BatDB.monsterBuff[mon, i + 1] = 0;
                }
            }
            if (BatDB.monsterBuff[mon, 11] > 0) acid += 1;
            if (BatDB.monsterBuff[mon, 12] > 0) acid += 2;
            if (BatDB.monsterBuff[mon, 13] > 0) acid += -1;
            if (BatDB.monsterBuff[mon, 14] > 0) acid += -2;
            if (acid > 2) acid = 2;
            else if (acid < -2) acid = -2;
            if (BatDB.monsterBuff[mon, 11] > 0 && acid != 1) BatDB.monsterBuff[mon, 11] = 0;
            if (BatDB.monsterBuff[mon, 12] > 0 && acid != 2) BatDB.monsterBuff[mon, 12] = 0;
            if (BatDB.monsterBuff[mon, 13] > 0 && acid != -1) BatDB.monsterBuff[mon, 13] = 0;
            if (BatDB.monsterBuff[mon, 14] > 0 && acid != -2) BatDB.monsterBuff[mon, 14] = 0;

            if (buff == 15) BatDB.monsterBuff[mon, 16] = 0;
            if (buff == 16) BatDB.monsterBuff[mon, 15] = 0;

            if (buff == 0 || buff == 17) {
                monster[mon].GetComponent<Animator>().speed = 0;
            }
        }

    }
    IEnumerator attackFalse(int num) {
        yield return null;
    }
    IEnumerator textShow(string name,string text) {//メッセージ表示処理
        messageFlag = true;
        M_win.SetActive(true);
        M_text.text = text;
        M_name.text = name;
        if (name == "")M_text.transform.position += new Vector3(0,40,0);
        while (Input.GetKeyDown(KeyCode.Z) || Input.GetMouseButton(0)) yield return null;
        while (!Input.GetKeyDown(KeyCode.Z) && !Input.GetMouseButton(0)) yield return null;
        while (Input.GetKeyDown(KeyCode.Z) || Input.GetMouseButton(0)) yield return null;
        if (name == "") M_text.transform.position -= new Vector3(0, 40, 0);
        M_win.SetActive(false);
        messageFlag = false;
    }
    IEnumerator TutorialMessage(int num,int increase) {
        if (num >= 0)yield return StartCoroutine(textShow(tText[num,0], tText[num,1]));
        else yield return null;
        tutorialNum += increase;
    }
    int DamageCaluculate(int attack,int defence,int pRate,int mRate,int dRate,bool counter,int attri) {//ダメージ計算
        //attri::0b(超酸化,還元,酸化,電気,水,炎)
        float pAttack, mAttack;//p:physical. m:magic
        float pDefence, mDefence;
        int pDamage, mDamage;
        int damage;
        int[] monAttri = new int[6];
        bool cure = false;
        pAttack = BatDB.StatesCor[attack, 6];
        mAttack = BatDB.StatesCor[attack, 8];
        pDefence = BatDB.StatesCor[defence, 7];
        mDefence = BatDB.StatesCor[defence, 9];

        //弱酸,強酸,弱塩基,強塩基 [attack,defence]
        float[,] rate = { { 0.5f,0.5f,1.25f,1.25f },
                            { 0.5f,0.5f,1.5f,1.5f },
                            { 1.25f,1.25f,0.5f,0.5f },
                            { 1.5f,1.5f,0.5f,0.5f }};

        if (BatDB.monsterBuff[attack, 1] > 0) pAttack = (int)Mathf.Floor(pAttack * 5 / 4f);
        else if (BatDB.monsterBuff[attack, 2] > 0) pAttack = (int)Mathf.Floor(pAttack * 4 / 5f);
        if (BatDB.monsterBuff[attack, 5] > 0) mAttack = (int)Mathf.Floor(mAttack * 5 / 4f);
        else if (BatDB.monsterBuff[attack, 6] > 0) mAttack = (int)Mathf.Floor(mAttack * 4 / 5f);
        if (BatDB.monsterBuff[defence, 3] > 0) pDefence = (int)Mathf.Floor(pDefence * 2);
        else if (BatDB.monsterBuff[defence, 4] > 0) pDefence = (int)Mathf.Floor(pDefence / 2);
        if (BatDB.monsterBuff[defence, 7] > 0) mDefence = (int)Mathf.Floor(mDefence * 2f);
        else if (BatDB.monsterBuff[defence, 8] > 0) mDefence = (int)Mathf.Floor(mDefence / 2f);

        if (BatDB.monsterBuff[attack, 22] > 0) {//超臨界流体
            pAttack = (int)Mathf.Floor(pAttack * 5 / 4f);
            mAttack = (int)Mathf.Floor(mAttack * 5 / 4f);
        }
        if (BatDB.monsterBuff[defence, 22] > 0) {//超臨界流体
            pDefence = (int)Mathf.Floor(pDefence * 2);
            mDefence = (int)Mathf.Floor(mDefence * 2);
        }
        if(BatDB.monsterBuff[attack, 38] > 0) {//ATPによる強化
            pAttack = (int)Mathf.Floor(pAttack * 5 / 4f);
            mAttack = (int)Mathf.Floor(mAttack * 5 / 4f);
            atpFlag = true;
        }

        pDefence = (int)Mathf.Floor(pDefence * dRate / 100f);
        mDefence = (int)Mathf.Floor(mDefence * dRate / 100f);

        if (pDefence <= 0)pDefence = 1;
        if (mDefence <= 0)mDefence = 1;
        pDamage = (int)Mathf.Floor(pAttack * pAttack / (3*pDefence + pAttack));
        mDamage = (int)Mathf.Floor(mAttack * mAttack / (3*mDefence + mAttack));

        for(int i = 11; i <= 14; i++) {
            for(int j = 11; j <= 14; j++) {
                if(BatDB.monsterBuff[attack,i]>0&& BatDB.monsterBuff[defence, j] > 0) {
                    pDamage = (int)Mathf.Floor(pDamage*rate[i-11, j-11]);
                    break;
                }
            }
        }

        if (charaLev(defence, 23) > 0 && dRate > 0) pDamage = (int)Mathf.Floor(pDamage * (75 - charaLev(defence, 23) * 5) / 100f);//物理軽減
        if (charaLev(defence, 24) > 0 && dRate > 0) mDamage = (int)Mathf.Floor(mDamage * (85 - charaLev(defence, 24) * 5) / 100f);//魔法軽減

        for (int i = 0; i < 3; i++) {//属性ダメージ増加
            int lev = charaLev(attack, 31 + i);
            if (lev <= 0) continue;
            if ((attri & 0b101) > 0 && i==0) mDamage = (int)Mathf.Floor(mDamage * (100 + lev * 10) / 100f);
            if (((attri & 0b010) > 0 && i == 1) || ((attri & 0b100) > 0 && i == 2)) {
                pDamage = (int)Mathf.Floor(pDamage * (100 + lev * 10) / 100f);
                mDamage = (int)Mathf.Floor(mDamage * (100 + lev * 10) / 100f);
            }
        }

        damage = (int)Mathf.Floor(pDamage * pRate / 100 + mDamage * mRate / 100);

        if ((attri & 0b010000) > 0 && charaLev(defence, 20) > 0)damage = (int)Mathf.Floor(damage * (16 - charaLev(defence, 20)) / 10f);//還元特効
        if ((attri & 0b101000) > 0 && charaLev(defence, 21) > 0)damage = (int)Mathf.Floor(damage * (16 - charaLev(defence, 21)) / 10f);//酸化特効

        if (charaLev(defence, 25) > 0 && dRate > 0) {//混合物
            int rateD=100;
            int lev = charaLev(defence, 25);
            if (lev == 1) rateD = RandomInt(80,120,attack>=4);
            else if (lev == 2) rateD = RandomInt(80, 120,attack>=4);
            else if (lev == 3) rateD = RandomInt(80, 115,attack>=4);
            else if (lev == 4) rateD = RandomInt(75, 105,attack>=4);
            else if (lev == 5) rateD = RandomInt(70, 100,attack>=4);
            damage = (int)Mathf.Floor(damage * rateD / 100f);
        }
        if (charaLev(defence, 22) > 0 && dRate > 0) damage = (int)Mathf.Floor(damage * (75 - charaLev(defence, 22) * 5) / 100f);//貴ガス
        if (BatDB.monsterBuff[defence,0]>0 && dRate>0)damage = (int)Mathf.Floor(damage/2);

        for(int i = 0; i < 4; i++) {//属性ダメージ軽減
            int lev = charaLev(defence, 26 + (i<=2?i:4));
            if (lev <= 0) continue;
            if (i <= 2) {
                if ((attri & (1 << i)) != 0) damage = (int)Mathf.Floor(damage * (100 - lev * 20) / 100f);
            } else {
                if ((attri & 0b101000) != 0) damage = RandomInt(1,100,attack>=4)<=(lev*20)?0:damage;
            }
        }
        if (charaLev(attack, 41)>0) {
            for(int i = (attack<4?0:4); i < (attack<4?4:8); i++) {
                if (i == attack) continue;
                if(Attackable(i) && charaLev(i,41)>0) {
                    damage = (int)Mathf.Floor(damage * (100 + charaLev(attack, 41) * 10) / 100f);
                    break;
                }
            }
        }
        MonsterData mon = StatesData.MonsterDataList[BatDB.States[defence, 0]];
        monAttri[0] = mon.Flame;
        monAttri[1] = mon.Water;
        monAttri[2] = mon.Electrical;
        monAttri[3] = mon.Oxidation;
        monAttri[4] = mon.Reduction;
        monAttri[5] = mon.OxidationP;
        for (int i = 0; (1<<i) <= attri;i++) {
            if (((1<<i) & attri) != 0) {
                if (monAttri[i] > 0) {
                    damage = (int)Mathf.Floor(damage * 0.75f);
                } else if (monAttri[i] < 0) {
                    damage = (int)Mathf.Floor(damage * 1.25f);
                }
                if (i<3&&BatDB.monsterBuff[defence, 26 + i] > 0) cure=true;
            }
        }

        if (cure) damage *= -1;
        if(damage>0 && (attri&0b10)>0 && charaLev(defence, 29) > 0) damage = -(int)Mathf.Floor(damage * (20+charaLev(defence, 29) * 10) / 100f);
        return damage;
    }
    void Damage(int useNum, int monsterNum, int value, int mode, string text, int attri, bool counter,int delay) { StartCoroutine(DamageCoroutine(monsterNum, value, mode, text, true, delay, counter)); }
    void Damage(int useNum, int monsterNum, int value, int mode, string text,int attri,bool counter) { Damage(useNum,monsterNum, value, mode, text,attri, counter,0); }
    bool Damage(int useNum,int monsterNum, int value, int mode, string text,int hit, int attri, bool counter) {return Damage(useNum,monsterNum,value,mode,text,hit,attri,counter, 0);}
    bool Damage(int useNum, int monsterNum, int value, int mode, string text, int hit,int attri, bool counter, int delay) {
        if (hit < 100) {
            if (BatDB.monsterBuff[monsterNum, 24] > 0) hit = hit * 6 / 10;
            if (useNum >= 0 && BatDB.monsterBuff[useNum, 25] > 0) hit = hit * 6 / 10;
        }
        bool hitFlag = RandomInt(1, 100,useNum>=4) <= hit;
        int delayFlame = 0;
        if (hitFlag && counter) {
            if (BatDB.monsterBuff[monsterNum, 37] > 0) {//ガラス
                Damage(monsterNum, useNum, DamageCaluculate(monsterNum, useNum, 10, 0, 100, false, 0b0), 0, "", 0, false, delayFlame);
                delayFlame += 30;
            }
            if (RandomInt(1,100,monsterNum>=4)<=charaLev(monsterNum, 0) * 20) {//針状結晶
                Damage(monsterNum, useNum, DamageCaluculate(monsterNum, useNum, 40, 0, 100, false, 0b0), 0, "", 0, false, delayFlame);
                effectCreate(Effect[69], monster[useNum].transform.position + new Vector3(0, 1, -1), useNum < 4 ? 1 : -1, delayFlame);
                delayFlame += 30;
            }
            if (RandomInt(1, 100, monsterNum >= 4)<=(charaLev(monsterNum, 1) * 20)) {//自然発火
                Damage(monsterNum, useNum, DamageCaluculate(monsterNum, useNum, 0, 40, 100, false, 0b1), 0, "",0b1, false, delayFlame);
                effectCreate(Effect[10], monster[useNum].transform.position + new Vector3(0, 1, -1), useNum < 4 ? 1 : -1, delayFlame);
                delayFlame += 30;
            }
            if (RandomInt(1, 100, monsterNum >= 4)<=(charaLev(monsterNum, 39) * 20)) {//酸塩基リセット
                string[] charaName = { "弱酸","強酸","弱塩基","強塩基"};
                for (int i = 11; i <= 14; i++) {
                    if(BatDB.monsterBuff[useNum,i]>0 && BatDB.monsterBuff[useNum, i] < 1000) {
                        Damage(monsterNum, useNum, 0, -1, charaName[i-11]+"性解除", 0, false, delayFlame);
                        playSE(32,delayFlame);
                        BatDB.monsterBuff[useNum, i] = 0;
                        break;
                    }
                }
                delayFlame += 30;
            }
        }
        if (hitFlag) {
            if (RandomInt(1, 100, monsterNum >= 4)<=(charaLev(monsterNum, 18) * 20) && (attri & 0b10) != 0) {//溶けて発熱
                Damage(monsterNum, useNum, DamageCaluculate(monsterNum, useNum, 0, 10, 100, false, 0b1), 0, "", 0b1, false, delayFlame);
                effectCreate(Effect[10], monster[useNum].transform.position + new Vector3(0, 1, -1), useNum < 4 ? 1 : -1, delayFlame);
                delayFlame += 30;
            }
            delayFlame = 30;
            for (int i = 14; i <= 17; i++) {//溶けて酸塩基
                string[] buffName = {"弱酸性","強酸性","弱塩基性","強塩基性" };
                if ((attri & 2) != 0 && RandomInt(1, 100, monsterNum >= 4)<=(charaLev(monsterNum, i) * 20)) {
                    Damage(-1,monsterNum,0,-1,buffName[i-14],0, false, delayFlame);
                    playSE(36,delayFlame);
                    Buff(monsterNum, i - 3, 5);
                    delayFlame += 30;
                    break;
                }
            }
            if ((attri & 2) != 0 && RandomInt(1, 100, monsterNum >= 4)<=(charaLev(monsterNum, 19) * 20)) {//溶けて吸熱
                Damage(-1, monsterNum, 0, -1, "炎吸収", 0, false, delayFlame);
                playSE(33, delayFlame);
                Buff(monsterNum, 26, 5);
                delayFlame += 30;
            }
        }
        StartCoroutine(DamageCoroutine(monsterNum, value, mode, text, hitFlag,delay,counter));
        return hitFlag;
    }
    IEnumerator DamageCoroutine(int monsterNum,int value,int mode,string text,bool hit,int delay,bool counter) {//ダメージ処理
        //mode=0:ダメージ 1:ミス 2:ノーダメージ 3:HP回復,4:SP回復,5:ミス,6:放射性崩壊,7:バリア,8:ガラス,9:無効,-1:文字列....
        GameObject damage = Instantiate(Effect[19], Vector3.zero, Quaternion.identity);
        Vector3 position = monster[monsterNum].transform.localPosition + new Vector3(0, 0.5f, -1);
        yield return new WaitForSeconds(delay/60f);
        if (value < 0 && mode == 0) {
            value *= -1;
            mode = 3;
        }
        if (mode == 0 && BatDB.monsterBuff[monsterNum, 36] > 0) mode = 7;
        if (mode == 0 && BatDB.monsterBuff[monsterNum, 37] > 0) mode = 8;

        Vector2 damagePos;
        damage.transform.SetParent(canvas.transform, false);
        damagePos = RectTransformUtility.WorldToScreenPoint(cameraBattle, position);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasAsp, damagePos, cameraAsp, out damagePos);
        damage.GetComponent<RectTransform>().localPosition = damagePos;

        if (!hit) mode = 1;

        if (mode == 1) StartCoroutine(textMove(damage, new Color(0.5f, 0.5f, 1, 1), "Miss", 2));
        else if (mode == 2) StartCoroutine(textMove(damage, new Color(0.7f, 0.7f, 0.7f, 1), "No Damage!", 2));
        else if (mode == 3) StartCoroutine(textMove(damage, new Color(0.6f, 1, 0.8f, 1), value.ToString(), 1));
        else if (mode == 4) StartCoroutine(textMove(damage, new Color(0.6f, 0.8f, 1, 1), value.ToString(), 1));
        else if (mode == 5) StartCoroutine(textMove(damage, new Color(0.5f, 0.5f, 1, 1), "Miss", 2));
        else if (mode == 6) StartCoroutine(textMove(damage, new Color(1f, 0.3f, 1, 1), value.ToString(), 2));
        else if (mode == -1) StartCoroutine(textMove(damage, new Color(1, 1, 1, 1), text, 1));
        else if (mode == 0)StartCoroutine(textMove(damage, new Color(1, 1, 1, 1), value.ToString(), 0));
        else if (mode == 9) StartCoroutine(textMove(damage, new Color(1, 0.5f, 0.5f, 1), "無効", 2));
        else StartCoroutine(textMove(damage, new Color(0, 0, 0, 0), "", 1));
        if (mode == 0||mode==6) {//ダメージ

            HPSPcontroll(0,monsterNum,-value);
            for(int i = 0; i < 20; i++) {
                monster[monsterNum].transform.eulerAngles += new Vector3(0, 0, monsterNum >= 4 ? -1 : 1)*(i<10?1:-1);
                monster[monsterNum].transform.Find("shade").gameObject.transform.eulerAngles += new Vector3(0, 0, monsterNum >= 4 ? 1 : -1) * (i < 10 ? 1 : -1);
                yield return new WaitForSeconds(1 / 60f);
            }
        }else if(mode == 3) HPSPcontroll(0, monsterNum, value);//HP回復
        else if (mode == 4)HPSPcontroll(1, monsterNum, value);//SP回復
        else if(mode == 5 || mode == 9) {//ミス
            playSE(15, 0);
            for (int i = 0; i < 6; i++) {
                monster[monsterNum].transform.position += new Vector3(monsterNum < 4 ? -1 : 1, 0, 0)*0.09f;
                yield return new WaitForSeconds(1 / 60f);
            }
            yield return new WaitForSeconds(40 / 60f);
            for (int i = 0; i < 6; i++) {
                monster[monsterNum].transform.position += new Vector3(monsterNum < 4 ? 1 : -1, 0, 0) * 0.09f;
                yield return new WaitForSeconds(1 / 60f);
            }
        } else if (mode == 1)playSE(15, 0);//ミス
        else if (mode == 7) {//バリア
            playSE(60, 0);
            effectCreate(Effect[67], monster[monsterNum].transform.position + new Vector3(0, 1, -0.5f), monsterNum < 4 ? 1 : -1);
        } else if (mode == 8) {//ガラス
            playSE(61, 0);
            effectCreate(Effect[68], monster[monsterNum].transform.position + new Vector3(0, 1, -0.5f), monsterNum < 4 ? 1 : -1);
            BatDB.monsterBuff[monsterNum, 37] = 0;
        }
        yield return null;
    }
    IEnumerator textMove(GameObject obj,Color c,string text,int type) {
        int frame=70,vanish=20;
        RectTransform rect = obj.GetComponent<RectTransform>();
        Text[] tex = new Text[5];
        int byteCount = getByte(text);
        tex[0] = obj.GetComponent<Text>();
        tex[1] = obj.transform.Find("DamageDown").GetComponent<Text>();
        tex[2] = obj.transform.Find("DamageLeft").GetComponent<Text>();
        tex[3] = obj.transform.Find("DamageRight").GetComponent<Text>();
        tex[4] = obj.transform.Find("Damage").GetComponent<Text>();

        for (int i = 0; i < 5; i++) {
            tex[i].text = text;
            if (byteCount > 7) {
                tex[i].fontSize = 7 * 50 / byteCount - (i == 4 ? 3 : 0);
            }
            
        }
        tex[4].color = c;

        if (type == 0) {
            for(int i = 0; i < frame; i++) {
                for(int j = 0; j < 5; j++) {
                    if(i>frame-vanish)tex[j].color-=new Color(0,0,0,(float)1/vanish);
                }
                rect.localPosition += new Vector3(0, (float)(frame / 1.5f - i) / 16, 0);
                yield return new WaitForSeconds(1/60f);
            }
        }else if (type == 1) {
            for (int i = 0; i < frame; i++) {
                for (int j = 0; j < 5; j++) {
                    if (i > frame - vanish) tex[j].color -= new Color(0, 0, 0, (float)1 / vanish);
                }
                rect.localPosition += new Vector3(0, (float)(frame - i) / 26, 0);
                yield return new WaitForSeconds(1 / 60f);
            }
        } else if (type == 2) {
            for (int i = 0; i < frame; i++) {
                for (int j = 0; j < 5; j++) {
                    if (i > frame - vanish) tex[j].color -= new Color(0, 0, 0, (float)1 / vanish);
                }
                rect.localPosition += new Vector3(3*Mathf.Cos((float)i/frame*10*Mathf.PI), (float)(frame / 1.5f - i) / 16, 0);
                yield return new WaitForSeconds(1 / 60f);
            }
        }
        Destroy(obj);
        yield return null;
    }
    int getByte(string str) {
        int res = 0;
        foreach(char s in str) {
            if (s >= 'a' && s <= 'z' || s >= 'A' && s <= 'Z' || s >= '0' && s <= '9') res+=1;
            else res += 2;
        }
        return res;
    }
    IEnumerator endurance(int num) {
        yield return new WaitForSeconds(30/60f);
        Damage(-1, num, 0, -1, "耐久",0, false);
        playSE(59,0);
        yield return null;
    }
    void HPSPcontroll(int mode,int num,int change) {//HP増減処理
        //mode=0:HP mode=1:SP
        float Pnow, Pmax;
        Pmax = BatDB.States[num, 2 + mode * 2];
        Pnow = BatDB.States[num, 3 + mode * 2];
        Pnow += change;
        if (Pnow <= 0 && (BatDB.monsterBuff[num,32]>0 || RandomInt(1,100,num>=4)<=charaLev(num,34)*5)) {//耐久
            if (num - 4 >= 0)HPvar[num - 4].transform.localScale = new Vector3(0, 1, 1);
            StartCoroutine(endurance(num));
            BatDB.monsterBuff[num, 32] = 0;
            Pnow = BatDB.States[num, 3 + mode * 2] = 1;
        }
        if (Pnow <= 0) {
            Pnow = 0;
            if (BatDB.States[num, 3 + mode * 2] > 0 && mode == 0) {
                playSE(3,20);
                monster[num].GetComponent<Monster>().death();
                if(num>=4)GameObject.Find("AllyName" + (num - 3).ToString()).GetComponent<Text>().color = new Color(0.5f,0.5f,0.5f);
                else GameObject.Find("EnemyName" + (num + 1).ToString()).GetComponent<Text>().color = new Color(0.5f, 0.5f, 0.5f);
                for (int i = 0; i < BatDB.monsterBuff.GetLength(1); i++) {
                    if (BatDB.monsterBuff[num, i] < 1000) {
                        BatDB.monsterBuff[num, i] = 0;
                    }else BatDB.monsterBuff[num, i] = 1100;
                }
                BatDB.death[num] = true;
            }
        } else if (Pnow > Pmax) Pnow = Pmax;
        BatDB.States[num, 3 + mode * 2] = (int)Pnow;

        if (num >= 4) {//バー変更
            if (mode == 0) {
                Color color;
                HPvar[num - 4].transform.localScale = new Vector3(Pnow / Pmax, 1, 1);
                HPvalue[num - 4].GetComponent<Text>().text = Pnow.ToString();
                if (Pnow / Pmax > 0.5)color = new Color(0.3f, 1.0f, 0.3f);
                else if (Pnow / Pmax > 0.2) color = new Color(1.0f, 1.0f, 0.3f);
                else color = new Color(1.0f, 0.3f, 0.3f);
                HPvar[num - 4].GetComponent<Image>().color = color;
            } else {
                SPvar[num - 4].transform.localScale = new Vector3(Pnow / Pmax, 1, 1);
                SPvalue[num - 4].GetComponent<Text>().text = Pnow.ToString();
            }
        }
        statesCor(num);
    }
    void effectCreate(GameObject obj, Vector3 pos, int scale,int delay) {
        StartCoroutine(delayEffect(obj,pos,scale,delay));
    }
    GameObject effectCreate(GameObject obj,Vector3 pos,int scale) {//エフェクト表示
        return Instantiate(obj, pos, Quaternion.identity);
    }
    IEnumerator delayEffect(GameObject obj, Vector3 pos, int scale, int delay) {
        yield return new WaitForSeconds(delay/60f);
        Instantiate(obj, pos, Quaternion.identity);
    }
    void statesCor(int i) {//ステータス補正処理
        for (int j = 1; j <= 10; j++) BatDB.StatesCor[i, j] = BatDB.States[i, j];
    }

    void playSE(int i,int delay) { StartCoroutine(delaySE(i, delay)); }
    IEnumerator delaySE(int i,int delay) {//SE再生
        yield return new WaitForSeconds(delay / 60f);
        audioS.PlayOneShot(SE[i], 2);
    }

    GameObject Arrow(int attacker,int defender) {//命令矢印描画
        GameObject arrow;
        LineRenderer renderer;
        Color lineColor = Color.black;
        if (attacker == 4) lineColor = new Color(1, 0, 0);
        else if (attacker == 5) lineColor = new Color(0, 0, 1);
        else if (attacker == 6) lineColor = new Color(1, 1, 0);
        else if (attacker == 7) lineColor = new Color(0, 1, 0);
        arrow = Instantiate(arrowLine,new Vector3(-10,0,0),Quaternion.identity);
        renderer = arrow.GetComponent<LineRenderer>();
        renderer.startWidth=0.2f;
        if (attacker != defender) {
            renderer.positionCount = 2;
            renderer.SetPosition(0, monster[attacker].transform.Find("shade").gameObject.transform.position + new Vector3(0, 0, 1));
            renderer.SetPosition(1, monster[defender].transform.Find("shade").gameObject.transform.position + new Vector3(0, 0, 1));
        } else {
            renderer.positionCount = 3;
            renderer.SetPosition(0, monster[attacker].transform.Find("shade").gameObject.transform.position + new Vector3(0, 0, 1));
            renderer.SetPosition(1, monster[defender].transform.Find("shade").gameObject.transform.position + new Vector3(-1f, 0, 1));
            renderer.SetPosition(2, monster[defender].transform.Find("shade").gameObject.transform.position + new Vector3(0, 0, 1));

        }
        renderer.startColor = lineColor;
        renderer.endColor = lineColor;
        return arrow;
    }

    bool Attackable(int i) {//攻撃可能か
        bool result=true;
        if (i < 0 || i > 7) return false;
        if (monster[i] == null) return false;
        if (BatDB.StatesCor[i, 3] == 0) result = false;
        return result;
    }
    int charaLev(int objNum,int num) {//特性のレベル取得
        for (int i = 0; i < StatesData.MonsterDataList[BatDB.States[objNum, 0]].Character.Length; i++) {
            if (StatesData.MonsterDataList[BatDB.States[objNum, 0]].Character[i] == num) {
                if (objNum >= 4) {
                    if (AllyData.AllyDataList[myDB.party[objNum - 4]].Character.Length <= i) return 1;
                    return AllyData.AllyDataList[myDB.party[objNum - 4]].Character[i];
                } else {
                    if (EnemyData.EnemyDataList[BatDB.Group[SysDB.Enemy, objNum]].Character.Length <= i) return 1;
                    return EnemyData.EnemyDataList[BatDB.Group[SysDB.Enemy, objNum]].Character[i];
                }
            }
        }
        return -1;
    }
    int AutoBattle(int num,bool all) {//オートバトル処理
        float maxScore=0;
        int maxNum=0;
        int referNum,skillNum = 0;
        int skillID;
        int[] genre = new int[5];
        if (num >= 4) {
            referNum = myDB.party[num - 4];
            for (int i = 0; i < AllyData.AllyDataList[referNum].Skill.Length; i++) {
                if (AllyData.AllyDataList[referNum].Skill[i] >= 0) {
                    genre[SkillData.SkillDataList[AllyData.AllyDataList[referNum].Skill[i]].Genre]++;
                    skillNum++;
                } else break;
            }
        } else {
            referNum = BatDB.Group[SysDB.Enemy, num];
            for (int i = 0; i < EnemyData.EnemyDataList[referNum].Skill.Length; i++) {
                if (EnemyData.EnemyDataList[referNum].Skill[i] >= 0) {
                    genre[SkillData.SkillDataList[EnemyData.EnemyDataList[referNum].Skill[i]].Genre]++;
                    skillNum++;
                } else break;
            }
        }
        
        for(int i = 0; i < skillNum; i++) {
            if (num >= 4)skillID = AllyData.AllyDataList[referNum].Skill[i];
            else  skillID = EnemyData.EnemyDataList[referNum].Skill[i];

            float skillScore = genre[SkillData.SkillDataList[skillID].Genre]/(float)skillNum;
            skillScore = 1;
            //skillScore = Mathf.Sqrt(skillScore);
            skillScore *= skillAssessment(num,skillID);

            if((SkillData.SkillDataList[skillID].Priority!=0)&&!all) skillScore=0;

            if (BatDB.monsterBuff[num, 19] > 0) skillScore = RandomInt(1,100,num>=4);
            if (skillScore > maxScore) {
                maxScore = skillScore;
                maxNum = skillID;
            }
            
        }
        
        return maxNum;
    }
    float skillAssessment(int num, int skill) {//スキル評価

        float score = 1;
        float[] HP = new float[8];
        float[] SP = new float[8];
        float phy = (st(num, 6) >= (float)st(num, 8)) ? 1 : 0.6f;
        float mag = (st(num, 6) <= (float)st(num, 8)) ? 1 : 0.8f;

        int allyCount = 0;
        int enemyCount = 0;
        List<int> allyList = new List<int>();
        List<int> enemyList = new List<int>();

        for (int i = 0; i < 8; i++) {
            int pre = (i + (num < 4 ? 4 : 0)) % 8;
            if (monster[pre] == null) continue;

            if (i < 4) enemyCount++;
            else allyCount++;

            if (Attackable(pre)) {
                if (i < 4) enemyList.Add(pre);
                else allyList.Add(pre);
                HP[pre] = st(pre, 3) / (float)st(pre, 2);
                SP[pre] = st(pre, 5) / (float)st(pre, 4);
            }

        }
        int targetCount = 0;
        if (skill == 0) {//通常攻撃
            score *= phy;
            if (RandomInt(1, 4, num >= 4) != 1) {
                if (SP[num] >= 0.5f) score *= 0.2f;
                else score *= 1 - SP[num];
            } else score *= 0.8f;
        } else if (skill == 1) {//防御
            if (RandomInt(1, 4, num >= 4) == 1) {
                if (HP[num] >= 0.5f) score *= 0.01f;
                else if (HP[num] >= 0.25f) score *= 0.3f;
                else score *= 1 - HP[num];
            } else score *= 0.01f;
            score *= RandomInt(50,100,num>=4)/100f;
        } else if (skill == 2) {//全体攻撃
            score *= phy;
            score *= (enemyList.Count-0.5f) / 4f;
            score *= RandomInt(10,100,num>=4)/100f;
        } else if (skill >= 3 && skill <= 5) {//ヒーラン系
            foreach (int i in allyList) targetCount += (HP[i] <= 0.5f) ? 1 : 0;
            if (targetCount == 0) score = 0.05f;
            else score *= 0.7f - targetCount * 0.05f;
            //score *= mag;
            score *= (95 + skill) / 100f;
        } else if (skill >= 6 && skill <= 8) {//テトラヒーラン系
            foreach (int i in allyList) targetCount += (HP[i] <= 0.5f) ? 1 : 0;
            if (targetCount == 0) score *= 0.05f;
            else score *= 0.6f + targetCount * 0.1f;
            //score *= mag;
            score *= (92 + skill) / 100f;
        } else if (skill == 9) {//リバイブ
            targetCount = allyCount - allyList.Count;
            if (targetCount == 0) score = 0;
            //else score *= Mathf.Min(HP[num] * 1.5f, 1);
        } else if (skill >= 10 && skill <= 18) {//フレイマン系
            score *= RandomInt(50, 100, num >= 4) / 100f;
            score *= (98 + (skill - 10) % 3) / 100f;
            score *= mag;
        } else if (skill >= 19 && skill <= 27) {//テトラフレイマン系
            score *= RandomInt(40, 100, num >= 4) / 100f;
            score *= enemyList.Count  / 4f;
            score *= (98 + (skill - 19) % 3) / 100f;
            score *= mag;
        } else if (skill >= 28 && skill <= 31) {//絶対零度系
            score *= RandomInt(50, 70, num >= 4) / 100f;
            foreach (int i in enemyList) targetCount += bf(i, skill - 11) > 1 ? 0 : 1;
            if (targetCount == 0) score = 0;
        } else if (skill == 32) {//分析
            score = 0;
        } else if (skill >= 33 && skill <= 37){//強化系
            foreach (int i in allyList) {
                if (bf(i, (skill - 33) * 2 + 1) <= 0) {
                    if (skill - 27 == 6) targetCount += st(i, 6) > st(i, 8) ? 1 : 0;
                    else if (skill - 27 == 8) targetCount += st(i, 8) > st(i, 6) ? 1 : 0;
                    else targetCount++;
                }
            }
            if (targetCount == 0) score = 0;
            else score *= RandomInt(40,90,num>=4)/100f;
        } else if (skill >= 38 && skill <= 42) {//弱化系
            foreach (int i in enemyList) {
                if (bf(i, (skill - 38) * 2 + 1) <= 1) {
                    if (skill - 32 == 6) targetCount += st(i, 6) > st(i, 8) ? 1 : 0;
                    else if (skill - 32 == 8) targetCount += st(i, 8) > st(i, 6) ? 1 : 0;
                    else targetCount++;
                }
            }
            if (targetCount == 0) score = 0;
            else score *= RandomInt(40, 90, num >= 4)/100f;
        } else if (skill == 43 || skill == 44) {//デバフ・バフ消去
            int debuffCount;
            int maxCount = 0;
            List<int> debuffList;

            if (skill == 43) debuffList = new List<int>() { 2, 4, 6, 8, 10, 17, 18, 19, 20, 21, 25, 29, 30, 31 };//デバフ
            else debuffList = new List<int>() { 1, 3, 5, 7, 9, 22, 23, 24, 26, 27, 28, 32, 33, 34, 35, 36, 37, 38 };//バフ

            foreach (int i in skill==43?allyList:enemyList) {
                debuffCount = 0;
                foreach (int j in debuffList) debuffCount += bf(i, j) > 0 ? 1:0 ;
                maxCount = Mathf.Max(maxCount, debuffCount);
            }
            if (maxCount == 0) score = 0;
            score *= RandomInt(80, 100, num >= 4) / 100f;
            score *= (100-debuffList.Count+maxCount)/100f;
        } else if (skill == 45) {//加熱・加圧
            score = RandomInt(1, 60, num >= 4) / 100f;
            if (bf(num, 23) > 0) score = 1;
            if (bf(num, 24) > 0) score = 0;
        } else if (skill == 46) {//エントロピー増大
            score *= phy;
            score *= (enemyList.Count - 0.5f) / 4f;
            score *= RandomInt(10, 100, num >= 4)/100f;
        } else if (skill >= 47 && skill <= 49) {//炎・水・電気攻撃
            score *= phy;
            score *= RandomInt(60, 90, num >= 4)/100f;
            if (bf(num, skill - 18) > 0) score = 0;
        } else if (skill >= 50 && skill <= 54) {//全体強化系
            foreach (int i in allyList) {
                if (bf(i, (skill - 50) * 2 + 1) <= 1) {
                    if (skill - 44 == 6) targetCount += st(i, 6) > st(i, 8) ? 1 : 0;
                    else if (skill - 44 == 8) targetCount += st(i, 8) > st(i, 6) ? 1 : 0;
                    else targetCount++;
                }
            }
            if (targetCount == 0) score = 0;
            else score *= (targetCount*0.1f+0.6f)*RandomInt(70, 100, num >= 4) / 100f;
        } else if (skill == 55) {//ブラウン運動
            foreach (int i in allyList) {
                if (bf(i, 24) <= 1)targetCount++;
            }
            if (targetCount == 0) score = 0;
            else score *= RandomInt(50, 80, num >= 4) / 100f;
        } else if (skill >= 56 && skill <= 63) {//デバフ系
            int[] debuff = { 10,25,25,12,14,11,13,11};
            foreach (int i in enemyList) {
                if (bf(i, debuff[skill - 56]) <= 1)targetCount++;
            }
            if (targetCount == 0) score = 0;
            else score *= (targetCount * 0.1f + 0.6f) * RandomInt(30, 80, num >= 4) / 100f;
            if (skill >= 59 && skill <= 62) score *= RandomInt(30, 80, num >= 4) / 100f;
        } else if (skill >= 64 && skill <= 66) {//炎色反応・水鉄砲・自由電子
            score *= mag;
            score *= RandomInt(60, 90, num >= 4) / 100f;
            if (bf(num, skill - 35) > 0) score = 0;
        } else if (skill >= 67 && skill <= 69) {//封印系
            foreach (int i in enemyList) {
                if (bf(i, skill - 38) <= 1) targetCount++;
            }
            if (targetCount == 0) score = 0;
            else score *= RandomInt(20, 80, num >= 4) / 100f;
        } else if (skill >= 70 && skill <= 72) {//吸収系
            foreach (int i in allyList) {
                if (bf(i, skill - 44) <= 1) targetCount++;
            }
            if (targetCount == 0) score = 0;
            else score *= RandomInt(20, 80, num >= 4) / 100f;
        } else if (skill == 73 || skill == 74) {//軽・重金属攻撃
            score *= phy;
            foreach (int i in enemyList) {
                if (skill == 73 && HP[i] <= 0.3f) targetCount++;
                if (skill == 74 && HP[i] >= 0.7f) targetCount++;
            }
            if (targetCount == 0) score = 0;
            else score *= RandomInt(50, 90, num >= 4) / 100f;
        } else if (skill == 75 || skill == 76) {//ネオンサイン・燐光
            score *= HP[num]* RandomInt(50, 90, num >= 4) / 100f;
        } else if (skill == 77 || skill == 78) {//刺激臭・腐卵臭
            score *= (1-HP[num]) * RandomInt(20, 80, num >= 4) / 100f;
        } else if (skill >= 79 && skill <= 81) {//酸化還元
            score *= mag;
            score *= RandomInt(30, 90, num >= 4)/ 100f;
        } else if (skill == 82) {//ふわふわパンケーキ
            foreach (int i in allyList) targetCount += (HP[i] <= 0.6f) ? 1 : 0;
            if (targetCount == 0) score *= 0.1f;
            else score *= 1 - targetCount * 0.05f;
            score *= mag* RandomInt(60,80,num>=4)/100f;
            score *= (95 + skill) / 100f;
        } else if (skill == 83) {//缶詰
            targetCount += (HP[num] <= 0.6f) ? 1 : 0;
            if (targetCount == 0) score *= 0.1f;
            else score *= 1 - targetCount * 0.05f;
            score *= mag * RandomInt(60, 80, num >= 4) / 100f;
            score *= (95 + skill) / 100f;
        } else if (skill == 84) {//肥料
            targetCount += (HP[num] <= 0.6f) ? 1 : 0;
            if (targetCount == 0) score *= 0.1f;
            else score *= 1 - targetCount * 0.05f;
            if (bf(num, 35) > 0) score = 0;
            score *= mag * RandomInt(60, 80, num >= 4) / 100f;
            score *= (95 + skill) / 100f;
        } else if (skill == 85 || skill == 86) {//全体魔法系
            score *= RandomInt(70, 100, num >= 4) / 100f;
            score *= (enemyList.Count - 1) / 4f;
            score *= mag;
        } else if (skill >= 87 && skill <= 94) {//酸化還元
            score *= (mag+phy)/2f;
            score *= RandomInt(30, 90, num >= 4) / 100f;
        } else if (skill >= 95 && skill <= 98) {//その他バフ系
            int[] buff = { 36,37,32,38};
            foreach (int i in allyList) {
                if (bf(i, buff[skill-95]) <= 0)targetCount++;
            }
            if (targetCount == 0) score = 0;
            else score *= RandomInt(30, 90, num >= 4) / 100f;
        } else if (skill == 99) {//融雪剤
            foreach (int i in allyList) {
                if (bf(i, 17) >= 1) targetCount++;
            }
            if (targetCount == 0) score = 0;
            else score *= (targetCount*0.1f+0.7f)*RandomInt(80, 100, num >= 4) / 100f;
        } else if (skill == 100 || skill==101) {//陽イオン,陰イオン
            if (bf(num, 15+(skill-100)) >= 1) score = 0;
            else if (bf(num, 16-(skill-100)) >= 1) score *= 0.8f;
            else score *= 0.6f;
            score *= RandomInt(60, 90, num >= 4) / 100f;
        } else score = 0;
        //ステータス番号  0:モンスターナンバー,1:レベル,2:最大HP,3:現在HP,4:最大SP,5:現在SP
        //6:物理攻撃 7:物理防御 8:魔法攻撃 9:魔法防御 10:素早さ 

        if (st(num,5) < SkillData.SkillDataList[skill].SP) score = 0;
        return score;
    }
    int skillTarget(int num,int skill) {//相手を取得
        int target;
        float maxScore = -1;
        float[] score = new float[8];
        float[] HP = new float[8];
        float[] SP = new float[8];

        List<int> allyList = new List<int>();
        List<int> enemyList = new List<int>();

        if (SkillData.SkillDataList[skill].Type == 3) return num;
        for (int i = 0; i < 8; i++) {
            int pre = (i + (num < 4 ? 4 : 0)) % 8;
            if (monster[pre] == null || (!Attackable(pre)&&skill!=9)) continue;
            if (i < 4) enemyList.Add(pre);
            else allyList.Add(pre);
            HP[pre] = st(pre, 3) / (float)st(pre, 2);
            SP[pre] = st(pre, 5) / (float)st(pre, 4);
            score[pre] = 0;
        }
        
        if (skill == 0 || skill>=10 && skill<=18 || skill>=47 && skill<=49 
            || skill>=64 && skill<=66 || skill==73 || skill==74
            || skill>=79 && skill<=81 || skill>=85 && skill<=94 || skill==100 || skill==101) {

            //一番体力が少ないやつをちょっと狙う
            if (RandomInt(0, 4,num>=4) < 1) {
                foreach(int i in enemyList)score[i] = (2-HP[i])*RandomInt(70,130,num>=4)/100f;
            } else foreach (int i in enemyList) score[i] = RandomInt(10, 100, num >= 4) / 100f;
        } else if(skill >= 3 && skill<=5 || skill>=82 && skill<=83) {//一番体力が少ないやつを回復
            foreach (int i in allyList) score[i] = 1 - HP[i];
        } else if (skill == 9) {//戦闘不能を回復
            foreach(int i in allyList) {
                if (HP[i] == 0) score[i] = RandomInt(90, 110, num >= 4) / 100f;
                else score[i] = RandomInt(10, 20, num >= 4) / 100f;
            }
        } else if (skill >= 28 && skill<=32 || skill>=38 && skill<=42) {//デバフ
            int[] debuff = { 17,18,19,20,21,0,0,0,0,0,2,4,6,8,10};
            foreach (int i in enemyList) {
                if (bf(i, debuff[skill - 28]) > 0) score[i] = RandomInt(10, 30, num >= 4) / 100f;
                else score[i] = RandomInt(90, 110, num >= 4) / 100f;
            }
        } else if (skill >= 33 && skill <= 37) {//バフ
            int[] debuff = { 1, 3, 5, 7, 9 };
            foreach (int i in allyList) {
                if (bf(i, debuff[skill - 33]) > 0) score[i] = RandomInt(10, 30, num >= 4) / 100f;
                else score[i] = RandomInt(90, 110, num >= 4) / 100f;
            }
        } else if (skill >= 43 && skill <= 44) {//デバフ・バフ消去
            List<int> debuffList;
            if (skill == 43) debuffList = new List<int>() { 2, 4, 6, 8, 10, 17, 18, 19, 20, 21, 25, 29, 30, 31 };//デバフ
            else debuffList = new List<int>() { 1, 3, 5, 7, 9, 22, 23, 24, 26, 27, 28, 32, 33, 34, 35, 36, 37, 38 };//バフ

            foreach (int i in skill==43?allyList:enemyList) {
                foreach(int j in debuffList) {
                    if(bf(i, j) > 0) score[i]++;
                }
                score[i] *= RandomInt(90, 110, num >= 4) / 100f;
            }
        } else if (skill == 55) {//ブラウン運動
            foreach (int i in allyList) {
                if (bf(i, 24) > 0) score[i] = RandomInt(10, 30, num >= 4) / 100f;
                else score[i] = RandomInt(90, 110, num >= 4) / 100f;
            }
        } else if (skill >= 56 && skill <= 62) {//デバフいろいろ
            int[] debuff = { 10, 25, 25, 12, 14, 11, 13, 11 };
            foreach (int i in enemyList) {
                if (bf(i, debuff[skill - 56]) > 0) score[i] = RandomInt(10, 30, num >= 4) / 100f;
                else score[i] = RandomInt(90, 110, num >= 4) / 100f;
            }
        } else if (skill >= 67 && skill <= 69) {//デバフいろいろ
            int[] debuff = { 29,30,31 };
            foreach (int i in enemyList) {
                if (bf(i, debuff[skill - 67]) > 0) score[i] = RandomInt(10, 30, num >= 4) / 100f;
                else score[i] = RandomInt(90, 110, num >= 4) / 100f;
            }
        } else if (skill >= 70 && skill <= 72) {//バフいろいろ
            int[] debuff = { 26, 27, 28 };
            foreach (int i in allyList) {
                if (bf(i, debuff[skill - 70]) > 0) score[i] = RandomInt(10, 30, num >= 4) / 100f;
                else score[i] = RandomInt(90, 110, num >= 4) / 100f;
            }
        } else if (skill >= 75 && skill <= 78) {//バフいろいろ
            int[] debuff = { 33, 33, 34, 34 };
            foreach (int i in allyList) {
                if (bf(i, debuff[skill - 75]) > 0) score[i] = RandomInt(10, 30, num >= 4) / 100f;
                else score[i] = RandomInt(90, 110, num >= 4) / 100f;
            }
        } else if (skill >= 96 && skill <= 97) {//バフいろいろ
            int[] debuff = { 37, 32};
            foreach (int i in allyList) {
                if (bf(i, debuff[skill - 96]) > 0) score[i] = RandomInt(10, 30, num >= 4) / 100f;
                else score[i] = RandomInt(90, 110, num >= 4) / 100f;
            }
        } else if (skill == 84) {//肥料
            foreach (int i in allyList) {
                if (bf(i, 35) > 0) score[i] = RandomInt(10, 30, num >= 4) / 100f;
                else score[i] = RandomInt(100, 130, num >= 4) / 100f * (2-HP[i]);
            }
        }

        target = 0;
        foreach(int i in allyList) {
            if (maxScore < score[i]) {
                target = i;
                maxScore = score[i];
            }
        }
        foreach (int i in enemyList) {
            if (maxScore < score[i]) {
                target = i;
                maxScore = score[i];
            }
        }
        return target;
    }
    int st(int num, int sta) { return BatDB.StatesCor[num, sta]; }
    int bf(int num, int baf) { return BatDB.monsterBuff[num, baf]; }

    IEnumerator MoveGameObject(GameObject obj, Vector3 pos, int frame, int delay) {//オブジェクト移動処理
        StartCoroutine(MoveToGameObject(obj, obj.transform.localPosition + pos, frame, delay));
        yield return null;
    }
    IEnumerator MoveToGameObject(GameObject obj, Vector3 pos, int frame, int delay) {//オブジェクト移動処理
        Vector3 moving = pos - obj.transform.localPosition;
        moveCount++;
        yield return new WaitForSeconds(delay / 60f);
        for (int i = 0; i < frame; i++) {
            obj.transform.localPosition += moving / frame;
            //obj.transform.Translate(moving / frame);
            yield return new WaitForSeconds(1 / 60f);
        }
        moveCount--;
        obj.transform.localPosition = pos;
    }
    Rect buttonRect(GameObject obj) {//矩形
        if (obj == null) return Rect.zero;
        RectTransform rect = obj.GetComponent<RectTransform>();
        float wid, hei;
        Vector2 widhei;
        Rect pos = ReRectPosition(obj, canvasAsp.gameObject);
        widhei = ReRectScale(obj, canvasAsp.gameObject);
        wid = rect.rect.width * widhei.x;
        hei = rect.rect.height * widhei.y;
        return new Rect(pos.x - wid / 2,
                        pos.y - hei / 2, wid, hei);
    }
    Vector2 ReRectScale(GameObject obj, GameObject canv) {
        if (obj == canv) return Vector2.one;
        Vector2 parent = ReRectScale(obj.transform.parent.gameObject, canv);
        return new Vector2(obj.transform.localScale.x * parent.x, obj.transform.localScale.y * parent.y);
    }
    Rect ReRectPosition(GameObject obj, GameObject canv) {
        if (obj.transform.parent.gameObject == canv) return new Rect(obj.transform.localPosition.x, obj.transform.localPosition.y, 1, 1);
        Rect parent = ReRectPosition(obj.transform.parent.gameObject, canv);
        float wi, he;
        wi = obj.transform.parent.localScale.x * parent.width;
        he = obj.transform.parent.localScale.y * parent.height;
        return new Rect(parent.x + obj.transform.localPosition.x * wi, parent.y + obj.transform.localPosition.y * he, wi, he);
    }
    int RandomInt(int min,int max) {return RandomInt(min, max, false,false);}
    int RandomInt(int min,int max,bool ally) {return RandomInt(min, max, ally, true);}
    int RandomInt(int min,int max,bool ally,bool net) {
        if (SysDB.netFlag&&net) {
            int ind = ally ? 1 : 0;
            if (me == 1) ind = 1 - ind;
            double ans;
            randIndex[ind]++;
            if (randIndex[ind] >= 100) randIndex[ind] = 1;
            ans = random[ind, randIndex[ind] - 1]/2000000001f;
            ans = ans * (max - min+1) + min;
            return (int)Math.Floor(ans);
        } else return SysDB.randomInt(min, max);
    }
    void Update(){//ボタンタップ判定
        if (SysDB.netFlag && PhotonNetwork.CurrentRoom.PlayerCount < 2 && !leftRoom) leftRoom = true;
        if (SysDB.netFlag && !PhotonNetwork.IsConnected && !leftRoom) leftRoom = true;
        if (!messageFlag) {
            if (!monsterSend && (!commandFlag) && controll && (Input.GetMouseButtonUp(0) || Input.GetKeyUp(KeyCode.Z))) {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasAsp, Input.mousePosition, cameraAsp, out mousePos);
                for (int i = 0; i < 12; i++) {
                    if (SysDB.netFlag && i == 10) continue;
                    if (buttonRe[i].Contains(mousePos)) buttonNum = i;
                    if (i >= 8 && Button[i - 8] != null) buttonImage[i - 8].color = new Color(0.7f, 0.7f, 0.7f, 1);
                }
            }
            if (!monsterSend && commandFlag && (Input.GetMouseButtonUp(0) || Input.GetKeyUp(KeyCode.Z))) {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasAsp, Input.mousePosition, cameraAsp, out mousePos);
                for (int i = 0; i < 4; i++) {
                    if (commandButtonRe[i].Contains(mousePos)) {
                        if (!(BatDB.turn == 1 && i == 1))commandButtonNum = i;
                        commandCursor.transform.position = new Vector3(-1000, 0, 0);
                    }
                }
            }
            if (!monsterSend && commandFlag && (Input.GetMouseButton(0) || Input.GetKey(KeyCode.Z))) {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasAsp, Input.mousePosition, cameraAsp, out mousePos);
                selectButton = -1;
                for (int i = 0; i < 4; i++) {
                    if (commandButtonRe[i].Contains(mousePos)) {
                        selectButton = i;
                        commandCursor.transform.position = commandButton[i].transform.position;
                    }
                }
                if (selectButton == -1) commandCursor.transform.position = new Vector3(-1000, 0, 0);
            }
            if (!monsterSend && controll && (Input.GetMouseButton(0) || Input.GetKey(KeyCode.Z))) {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasAsp, Input.mousePosition, cameraAsp, out mousePos);
                for (int i = 8; i < 12; i++) {
                    if (buttonRe[i].Contains(mousePos)) {
                        //if (Button[i - 8].GetComponent<Image>().color != new Color(1, 1, 1, 1)) playSE(1,0);
                        if (SysDB.netFlag && i == 10) continue;
                        buttonImage[i - 8].color = new Color(1, 1, 1, 1);
                    } else if (Button[i - 8] != null) buttonImage[i - 8].color = new Color(0.7f, 0.7f, 0.7f, 1);
                }
            }
            if (skillControll && !controll && (Input.GetMouseButtonUp(0) || Input.GetKeyUp(KeyCode.Z))) {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasAsp, Input.mousePosition, cameraAsp, out mousePos);
                for (int i = 0; i < 13; i++) {
                    if (skillButtonRe[i].Contains(mousePos)) {
                        if (i >= 10 || i < 10 && skillButton[i].GetComponent<Text>().text != "") skillButtonNum = i;
                    }
                    if (i >= 10) skillButtonImage[i].color = new Color(0.7f, 0.7f, 0.7f, 1);

                }
            }
            if ((!sendCheck) && monsterSend && (Input.GetMouseButtonUp(0) || Input.GetKeyUp(KeyCode.Z))) {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasAsp, Input.mousePosition, cameraAsp, out mousePos);
                for (int i = 0; i < 9; i++) {
                    if (buttonRe[i].Contains(mousePos)) sendButtonNum = i;
                }
            }
            if ((!sendCheck) && monsterSend && (Input.GetMouseButton(0) || Input.GetKey(KeyCode.Z))) {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasAsp, Input.mousePosition, cameraAsp, out mousePos);
                if (buttonRe[8].Contains(mousePos)){
                    buttonImage[0].color = new Color(1, 1, 1, 1);
                }else buttonImage[0].color = new Color(0.7f, 0.7f, 0.7f, 1);
            }else if(monsterSend) buttonImage[0].color = new Color(0.7f, 0.7f, 0.7f, 1);

            if (sendCheck && monsterSend && (Input.GetMouseButton(0) || Input.GetKey(KeyCode.Z))) {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasAsp, Input.mousePosition, cameraAsp, out mousePos);
                for (int i = 0; i < 2; i++) {
                    if (getButtonRe[i].Contains(mousePos)) {
                        getButtonImage[i].color = new Color(1, 1, 1, 1);
                    } else {
                        getButtonImage[i].color = new Color(0.7f, 0.7f, 0.7f, 1);
                    }
                }
            } else if (sendCheck && monsterSend) {
                getButtonImage[0].color = new Color(0.7f, 0.7f, 0.7f, 1);
                getButtonImage[1].color = new Color(0.7f, 0.7f, 0.7f, 1);
            }
            if (sendCheck && monsterSend && (Input.GetMouseButtonUp(0) || Input.GetKeyUp(KeyCode.Z))) {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasAsp, Input.mousePosition, cameraAsp, out mousePos);
                for (int i = 0; i < 2; i++) {
                    if (getButtonRe[i].Contains(mousePos)) {
                        getButtonNum = i;
                    }
                }
            }
            if (skillControll && !controll && (Input.GetMouseButton(0) || Input.GetKey(KeyCode.Z))) {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasAsp, Input.mousePosition, cameraAsp, out mousePos);
                selectButton = -1;
                for (int i = 0; i < 13; i++) {
                    if (skillButtonRe[i].Contains(mousePos)) {
                        if (i >= 10) {
                            //if (skillButton[i].GetComponent<Image>().color != new Color(1, 1, 1, 1)) playSE(1, 0);
                            skillButtonImage[i].color = new Color(1, 1, 1, 1);
                        } else {
                            if (skillButton[i].GetComponent<Text>().text != "") {
                                //if (skillCursor.transform.position.x <= -600) playSE(1, 0);
                                selectButton = i;
                                if (!itemControll) detail.text = SkillData.SkillDataList[AllyData.AllyDataList[myDB.party[partyNum]].Skill[i + skillStart]].Detail;
                                else detail.text = ItemData.ItemDataList[itemID[i + skillStart]].Detail;
                                skillCursor.transform.position = skillButton[i].transform.position;

                            }
                        }
                    } else {
                        if (i >= 10) skillButtonImage[i].color = new Color(0.7f, 0.7f, 0.7f, 1);
                    }
                }
                if (selectButton == -1) {
                    skillCursor.transform.position = new Vector3(-1000, 0, 0);
                    detail.text = "";
                }
            }
        } else {
            skillCursor.transform.position = new Vector3(-1000, 0, 0);
            detail.text = "";
            selectButton = -1;
        }
    }
}
