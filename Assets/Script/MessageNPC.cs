using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MessageNPC : MonoBehaviour{
    [Multiline(3)]
    public string[] message;
    [Multiline(3)]
    public string[] startEvent;

    GameObject M_name;
    GameObject M_message;
    GameObject M_text;
    GameObject T_window;
    GameObject T_text;
    GameObject[] Button;
    GameObject Player;
    PlayerMove pmove;
    Rect[] buttonRe;
    Text nameText;
    Text messageText;
    Text tutorialText;
    RectTransform nameRect;
    RectTransform messageRect;
    RectTransform textRect;
    RectTransform tutoRect;
    RectTransform canvas;
    Camera cameraAsp;
    GameController gameCon;
    EventExist exist;
    AudioSource cAudio;
    CircleCollider2D cir;
    int mode = 0;
    int selectButtonNum;
    double bgmVolume = 0.3f;
    GameObject selectCursor,selectWin;

    void Start() {
        Player = GameObject.Find("Player");
        pmove = Player.GetComponent<PlayerMove>();
        cir = Player.GetComponent<CircleCollider2D>();
        M_name = GameObject.Find("M_name");
        M_message = GameObject.Find("M_message");
        M_text = GameObject.Find("M_text");
        T_window = GameObject.Find("Tutorial");
        T_text = GameObject.Find("TutorialText");
        nameText = M_name.GetComponent<Text>();
        messageText = M_text.GetComponent<Text>();
        tutorialText = T_text.GetComponent<Text>();

        nameRect = M_name.GetComponent<RectTransform>();
        messageRect = M_message.GetComponent<RectTransform>();
        textRect = M_text.GetComponent<RectTransform>();
        tutoRect = T_window.GetComponent<RectTransform>();

        exist = GetComponent<EventExist>();
        gameCon = GameObject.Find("GameController").GetComponent<GameController>();
        //cameraAsp = gameCon.cameraObj.GetComponent<Camera>();
        cameraAsp = null;
        canvas = gameCon.canvasObj.GetComponent<RectTransform>();
        Button = new GameObject[6];
        buttonRe = new Rect[6];
        selectWin = GameObject.Find("SelectWindow");
        for (int i = 0; i < 6; i++) {
            Button[i] = selectWin.transform.Find("Select" + (i + 1).ToString()).gameObject;
        }
        selectCursor = selectWin.transform.Find("SelectCursor").gameObject;
        if(startEvent!=null && startEvent.Length>=1)StartCoroutine(EventStart(true));
    }

    public void main(){
        StartCoroutine(EventStart(false));
    }
    private IEnumerator EventStart(bool firstFlag) {
        string[] Message;
        string[] stringArray;
        string[] commandArray;
        string lastCommand = "",text;
        bool pre, firstMessage = false;
        Vector3 pos,firPos,moveTo,sa = new Vector3(0,0,0);
        GameObject moveObj=null, moveObj2=null, camera = null,obj;
        SysDB.eventFlag = true;
        if (firstFlag) Message = startEvent;
        else Message = message;
        for (int i = 0; i < Message.Length; i++) {
            if (Message[i] == "") continue;
            Message[i] = Message[i].Replace("デュフー", "アマルガ");
            commandArray = Message[i].Split('\n');
            for (int k = 0; k < commandArray.Length; k++) {
                stringArray = commandArray[k].Split(':');
                nameText.text = stringArray[0];

                if (stringArray.Length == 2) {
                    messageText.text = stringArray[1].Replace(";", "\n");
                    messageText.text = messageText.text.Replace("[P]", myDB.playerName);
                }

                //以下コマンド入力
                lastCommand = stringArray[0];
                if (stringArray[0] == "BattleStart") {//BattleStart:"Scenes/Battle":SE:BGM:背景:敵グループ:特殊バトル番号
                    if (int.Parse(stringArray[2]) >= 0) gameCon.SE(int.Parse(stringArray[2]));
                    SysDB.SE = int.Parse(stringArray[2]);
                    SysDB.BGM = int.Parse(stringArray[3]);
                    SysDB.Back = int.Parse(stringArray[4]);
                    SysDB.Enemy = int.Parse(stringArray[5]);
                    if (stringArray.Length >= 7) SysDB.Battle = int.Parse(stringArray[6]);
                    else SysDB.Battle = 0;
                    gameCon.sceneLoad(stringArray[1]);
                    SysDB.BattleResult = 0;
                    while (true) {
                        yield return new WaitForSeconds(1 / 60f);
                        if (SysDB.BattleResult > 0) break;
                    }
                    yield return new WaitForSeconds(30 / 60f);
                } else if (stringArray[0] == "MoveTo") {//MoveTo:対象:移動モード:x:y:z:ジャンプ高さ:追加要素...
                                                        //移動モード:0瞬時　1:ジャンプ 2:瞬時(場所参照) 3:ジャンプ(場所参照)
                    moveObj = GameObject.Find(stringArray[1]);
                    if (stringArray.Length >= 9) {
                        if (stringArray[8] == "this") moveObj2 = this.gameObject;
                        else moveObj2 = GameObject.Find(stringArray[8]);
                        sa = moveObj.transform.position - moveObj2.transform.position;
                    }
                    firPos = moveObj.transform.position;
                    pos = new Vector3(float.Parse(stringArray[3]),
                                      float.Parse(stringArray[4]),
                                      float.Parse(stringArray[5]));
                    if (int.Parse(stringArray[2]) == 0) {
                        moveObj.transform.position = pos;
                    } else if (int.Parse(stringArray[2]) == 1) {
                        cir.enabled = false;
                        pre = SysDB.cameraMove;
                        if (stringArray[1] == "Player") {
                            SysDB.cameraMove = false;
                            camera = GameObject.Find("Main Camera");
                        }
                        float div = 1;
                        if (stringArray.Length == 8) div = float.Parse(stringArray[7]);
                        for (int j = 1; j <= 10 * div; j++) {
                            moveTo = firPos + (pos - firPos) * j / div / 10f
                                   + new Vector3(0, (25 - (j / div - 5) * (j / div - 5)) * float.Parse(stringArray[6]), 0) / 10f;
                            moveObj.transform.position = moveTo;

                            if (pre && stringArray[1] == "Player" && camera != null) {
                                camera.transform.position += (pos - firPos) / 10f;
                            }
                            yield return new WaitForSeconds(1 / 60f);
                        }
                        cir.enabled = true;
                        SysDB.cameraMove = pre;
                    } else if (int.Parse(stringArray[2]) == 3 || int.Parse(stringArray[2]) == 4) {
                        cir.enabled = false;
                        if (stringArray[7] == "this") pos += transform.position;
                        else pos += GameObject.Find(stringArray[7]).transform.position;
                        pre = SysDB.cameraMove;
                        if (stringArray[1] == "Player") {
                            SysDB.cameraMove = false;
                            camera = GameObject.Find("Main Camera");
                        }
                        float div = 1;
                        if (int.Parse(stringArray[2]) == 4) div = float.Parse(stringArray[9]);
                        for (int j = 1; j <= 10 * div; j++) {
                            moveTo = firPos + (pos - firPos) * j / div / 10f
                                   + new Vector3(0, (25 - (j / div - 5) * (j / div - 5)) * float.Parse(stringArray[6]), 0) / 10f;
                            moveObj.transform.position = moveTo;
                            if (pre && stringArray[1] == "Player" && camera != null) {
                                camera.transform.position += (pos - firPos) / div / 10f;
                            }
                            yield return new WaitForSeconds(1 / 60f);
                        }
                        cir.enabled = true;
                        SysDB.cameraMove = pre;
                    }
                } else if (stringArray[0] == "Move") {//Move:対象:移動モード:x:y:z:追加要素:追加要素...
                                                      //移動モード:0瞬時　1:ジャンプ
                    moveObj = GameObject.Find(stringArray[1]);
                    if (stringArray.Length >= 9) {
                        moveObj2 = GameObject.Find(stringArray[8]);
                        sa = moveObj.transform.position - moveObj2.transform.position;
                    }
                    firPos = moveObj.transform.position;
                    pos = new Vector3(float.Parse(stringArray[3]),
                                      float.Parse(stringArray[4]),
                                      float.Parse(stringArray[5])) + moveObj.transform.position;
                    if (int.Parse(stringArray[2]) == 0) {
                        moveObj.transform.position = pos;
                    } else if (int.Parse(stringArray[2]) == 1) {
                        pre = SysDB.cameraMove;
                        if (stringArray[1] == "Player") {
                            SysDB.cameraMove = false;
                            camera = GameObject.Find("Main Camera");
                        }
                        for (int j = 1; j <= 10; j++) {
                            moveTo = firPos + (pos - firPos) * j / 10f
                                   + new Vector3(0, (25 - (j - 5) * (j - 5)) * float.Parse(stringArray[6]), 0) / 10f;

                            moveObj.transform.position = moveTo;
                            if (stringArray.Length >= 9) moveObj2.transform.position = moveTo - sa;
                            if (pre && stringArray[1] == "Player" && camera != null) {
                                camera.transform.position += (pos - firPos) / 10f;
                            }
                            yield return new WaitForSeconds(1 / 60f);
                        }
                        SysDB.cameraMove = pre;
                    } else if (int.Parse(stringArray[2]) == 2) {
                        if (stringArray[1] == "Player") {
                            moveObj.GetComponent<PlayerMove>().EventMove = pos - moveObj.transform.position;
                            yield return new WaitForSeconds(int.Parse(stringArray[6]) / 60f);
                            moveObj.GetComponent<PlayerMove>().EventMove = Vector3.zero;
                        } else {
                            moveObj.GetComponent<CharaMove>().EventMove = pos - moveObj.transform.position;
                            yield return new WaitForSeconds(int.Parse(stringArray[6]) / 60f);
                            moveObj.GetComponent<CharaMove>().EventMove = Vector3.zero;
                        }
                    }
                } else if (stringArray[0] == "cameraMove") {//cameraMove:モード:対象:x:y:z:フレーム
                    SysDB.cameraMove = false;
                    camera = GameObject.Find("Main Camera");
                    moveTo = camera.transform.position;
                    if (int.Parse(stringArray[1]) == 0) {//moveTo
                        moveTo = new Vector3(float.Parse(stringArray[3]), float.Parse(stringArray[4]), float.Parse(stringArray[5]));
                    } else if (int.Parse(stringArray[1]) == 1) {//move
                        moveTo = camera.transform.position + new Vector3(float.Parse(stringArray[3]), float.Parse(stringArray[4]), float.Parse(stringArray[5]));
                    } else if (int.Parse(stringArray[1]) == 2) {//対象へ
                        moveTo = GameObject.Find(stringArray[2]).transform.position + new Vector3(float.Parse(stringArray[3]), float.Parse(stringArray[4]) - 5, float.Parse(stringArray[5])); ;
                    }
                    moveTo.Set(moveTo.x, moveTo.y, camera.transform.position.z);
                    pos = moveTo - camera.transform.position;
                    for (int j = 0; j < int.Parse(stringArray[6]); j++) {
                        camera.transform.Translate(pos / float.Parse(stringArray[6]));
                        yield return new WaitForSeconds(1 / 60f);
                    }
                    camera.transform.position = moveTo;
                } else if (stringArray[0] == "Speed") {//Speed:対象:速度
                    GameObject.Find(stringArray[1]).GetComponent<CharaMove>().speed=float.Parse(stringArray[2]);
                } else if (stringArray[0] == "playSE") {//playSE:SE番号:ディレイ
                    if (gameCon != null) {
                        gameCon.GetComponent<GameController>().SE(int.Parse(stringArray[1]));
                    }

                } else if (stringArray[0] == "FadeIn") {//FadeIn:フェード番号:時間
                    obj = GameObject.Find("FadeCanvas");
                    if (obj != null) {
                        obj.GetComponent<Fade>().FadeIn(int.Parse(stringArray[1]), float.Parse(stringArray[2]));
                    }
                } else if (stringArray[0] == "FadeOut") {//FadeOut:フェード番号:時間
                    obj = GameObject.Find("FadeCanvas");
                    if (obj != null) {
                        obj.GetComponent<Fade>().FadeOut(int.Parse(stringArray[1]), float.Parse(stringArray[2]));
                    }
                } else if (stringArray[0] == "Wait") {//Wait:フレーム
                    yield return new WaitForSeconds(int.Parse(stringArray[1]) / 60f);
                }else if (stringArray[0] == "FireJudge") {//FireJudge:ジャンプ先
                    bool fireflag;
                    FireColor fire = GetComponent<FireColor>();
                    int[] fireNum = { 62,76,14,80,78,64,58};
                    fireflag = gameCon.getMon(fireNum[fire.ColorNum]);
                    fireflag |= fire.OnOff;
                    if (fireflag) {
                        i = int.Parse(stringArray[1]) - 1;
                        break;
                    }
                } else if (stringArray[0] == "FireOn") {//FireOn:SE番号
                    FireColor fire;
                    fire = GetComponent<FireColor>();
                    if (fire.OnOff == false) {
                        fire.On();
                        if (gameCon != null) {
                            gameCon.GetComponent<GameController>().SE(int.Parse(stringArray[1]));
                        }
                        yield return new WaitForSeconds(30/60f);
                    }
                } else if (stringArray[0] == "MapEdit") {//MapEdit:番号
                    if (stringArray.Length == 3) {
                        StartCoroutine(GetComponent<MapEdit>().main(int.Parse(stringArray[1]), int.Parse(stringArray[2])));
                    } else StartCoroutine(GetComponent<MapEdit>().main(int.Parse(stringArray[1])));
                } else if (stringArray[0] == "MapName") {//MapNme:(表示/消去)
                    if (int.Parse(stringArray[1]) == 0) gameCon.GetComponent<GameController>().mapName();
                    else gameCon.GetComponent<GameController>().mapDelete();
                } else if (stringArray[0] == "PlayerDir") {//PlayerDir:x:y
                    obj = null;
                    obj = GameObject.Find("Player");
                    if (obj != null) obj.GetComponent<PlayerMove>().playerDirection(new Vector2(int.Parse(stringArray[1]), int.Parse(stringArray[2])));
                } else if (stringArray[0] == "SelfDir") {//SelfDir:x:y
                    GetComponent<CharaMove>().charaDirection(new Vector2(int.Parse(stringArray[1]), int.Parse(stringArray[2])));
                } else if (stringArray[0] == "Dir") {//Dir:オブジェクト名:x:y
                    if (stringArray[1] == "Player") {
                        GameObject.Find(stringArray[1]).GetComponent<PlayerMove>().playerDirection(new Vector2(int.Parse(stringArray[2]), int.Parse(stringArray[3])));
                    } else GameObject.Find(stringArray[1]).GetComponent<CharaMove>().charaDirection(new Vector2(int.Parse(stringArray[2]), int.Parse(stringArray[3])));
                } else if (stringArray[0] == "TargetDir") {//Dir:向くobj:対象obj
                    obj = GameObject.Find(stringArray[1]);
                    if (stringArray[1] != "Player") obj.GetComponent<CharaMove>().charaDirection(GameObject.Find(stringArray[2]).transform.position - obj.transform.position);
                    else obj.GetComponent<PlayerMove>().playerDirection(GameObject.Find(stringArray[2]).transform.position - obj.transform.position);
                } else if (stringArray[0] == "GoTo") {//GoTo:ラベル番号
                    i = int.Parse(stringArray[1]) - 1;
                    break;
                } else if (stringArray[0] == "MapChange") {//MapChange:マップ名:フェード番号:フェード時間:x:y:z:BGM変更:間フレーム
                    obj = GameObject.Find("FadeCanvas");
                    if (obj != null) {
                        obj.GetComponent<Fade>().FadeIn(int.Parse(stringArray[2]), float.Parse(stringArray[3]));
                    }
                    yield return new WaitForSeconds(float.Parse(stringArray[3]));
                    Scene scene = SceneManager.GetActiveScene();
                    if (SysDB.sceneName != stringArray[1]) {
                        SceneManager.UnloadSceneAsync(scene);
                        SysDB.sceneName = stringArray[1];
                        SceneManager.LoadScene("Scenes/" + stringArray[1], LoadSceneMode.Additive);
                        gameCon.GetComponent<GameController>().mapDelete();
                        obj = GameObject.Find("FadeCanvas");
                    }
                    if(stringArray.Length>=9) yield return new WaitForSeconds(int.Parse(stringArray[8])/60f);
                    if (obj != null) {
                        obj.GetComponent<Fade>().FadeOut(int.Parse(stringArray[2]), float.Parse(stringArray[3]));
                    }
                    AudioSource audioS = GameObject.Find("Main Camera").GetComponent<AudioSource>();
                    if (stringArray.Length == 8 && audioS.clip != gameCon.GetComponent<GameController>().BGM[int.Parse(stringArray[7])]) {
                        audioS.Stop();
                        audioS.clip = gameCon.GetComponent<GameController>().BGM[int.Parse(stringArray[7])];
                        audioS.Play();
                    } else if (stringArray.Length == 9) {
                        GameObject.Find("Player").GetComponent<PlayerMove>().playerDirection(new Vector2(int.Parse(stringArray[7]), int.Parse(stringArray[8])));
                    }
                    GameObject.Find("Player").transform.position =
                          new Vector3(float.Parse(stringArray[4]),
                                      float.Parse(stringArray[5]),
                                      float.Parse(stringArray[6]) * 0);
                } else if (stringArray[0] == "Event") {//Event:値
                    nameRect.localPosition = new Vector3(-279.5f, -1500, 0);
                    messageRect.localPosition = new Vector3(0, -1500, 0);
                    textRect.localPosition = new Vector3(4.2f, -1500, 0);
                    exist.changeEvent(int.Parse(stringArray[1]));
                    //break;
                } else if (stringArray[0] == "Treasure") {//Treasure:値
                    nameRect.localPosition = new Vector3(-279.5f, -1500, 0);
                    messageRect.localPosition = new Vector3(0, -1500, 0);
                    textRect.localPosition = new Vector3(4.2f, -1500, 0);
                    exist.changeTreasure(int.Parse(stringArray[1]));
                    //break;
                } else if (stringArray[0] == "FlagJump") {//FlagJump:E/T/Tu:値:行数:値:行数...
                    int value;
                    bool jump;
                    pre = false;
                    for (int j = 0; j < stringArray.Length / 2 - 1; j++) {
                        jump = false;
                        value = int.Parse(stringArray[j * 2 + 2]);
                        if (stringArray[1] == "E") jump = value == exist.getEvent();
                        else if (stringArray[1] == "T") jump = value == exist.getTreasure();
                        else if (stringArray[1] == "Tu") jump = value == exist.getTutorial();
                        else if (!int.TryParse(stringArray[1], out int res)) jump = ItemPossess(stringArray[1]) >= value;
                        else jump = judge(int.Parse(stringArray[1])) == int.Parse(stringArray[2]);

                        if (jump) {
                            i = int.Parse(stringArray[j * 2 + 3]) - 1;
                            pre = true;
                        }
                    }
                    if (pre) break;
                } else if (stringArray[0] == "Heal") {//Heal:
                    gameCon.GetComponent<GameController>().SE(11);
                    obj = Instantiate(gameCon.GetComponent<GameController>().effect[1], Player.transform.position + new Vector3(0, 0.3f, 0), Quaternion.identity);
                    obj.GetComponent<SpriteRenderer>().sortingOrder = 11;
                    obj.transform.localScale *= 1.5f;
                    AllyDatas ally = gameCon.GetComponent<GameController>().allyData;
                    for (int j = 0; j < 4; j++) {
                        if (myDB.party[j] >= 0) {
                            ally.AllyDataList[myDB.party[j]].NowHP = ally.AllyDataList[myDB.party[j]].HP;
                            ally.AllyDataList[myDB.party[j]].NowSP = ally.AllyDataList[myDB.party[j]].SP;
                        }
                    }
                    yield return new WaitForSeconds(1);
                } else if (stringArray[0] == "Effect") {//Effect:num:x:y:z:size:sort
                    obj = Instantiate(gameCon.GetComponent<GameController>().effect[int.Parse(stringArray[1])], 
                        new Vector3(float.Parse(stringArray[2]), float.Parse(stringArray[3]), float.Parse(stringArray[4])), Quaternion.identity);
                    obj.GetComponent<SpriteRenderer>().sortingOrder = int.Parse(stringArray[6]);
                    obj.transform.localScale *= float.Parse(stringArray[5]);
                } else if (stringArray[0] == "MonsterBank") {//MonsterBank:
                    SysDB.menueFlag = true;
                    SysDB.sceneName = SceneManager.GetActiveScene().name;
                    SceneManager.LoadScene("Scenes/MonsterBank", LoadSceneMode.Additive);
                } else if (stringArray[0] == "MonsterGene") {//MonsterGene:
                    SysDB.menueFlag = true;
                    SysDB.sceneName = SceneManager.GetActiveScene().name;
                    SceneManager.LoadScene("Scenes/MonsterGene", LoadSceneMode.Additive);
                } else if (stringArray[0] == "Shop") {//Shop:店名:商品名1:商品名2:…
                    for(int j = 0; j < gameCon.shopData.ItemDataList.Count; j++) {
                        gameCon.shopData.ItemDataList[j] = new ItemData();
                        gameCon.shopData.ItemDataList[j].Name = "";
                    }
                    if (stringArray.Length == 2 || stringArray[2] == "") {
                        /*for (int l = 0; l < gameCon.itemData.ItemDataList.Count; l++) {
                            if (gameCon.itemData.ItemDataList[l].Possess > 0 && gameCon.itemData.ItemDataList[l].Possess > 0) {
                                gameCon.shopData.ItemDataList[shopCount] = gameCon.itemData.ItemDataList[l];
                                shopCount++;
                                break;
                            }
                        }*/
                        gameCon.shopData.ItemDataList[0] = new ItemData();
                        gameCon.shopData.ItemDataList[0].Name = "";
                    } else {
                        for (int j = 2; j < stringArray.Length; j++) {
                            if (stringArray[j] == "") {
                                gameCon.shopData.ItemDataList[j - 2] = new ItemData();
                                gameCon.shopData.ItemDataList[j - 2].Name = "";
                                break;
                            }
                            for (int l = 0; l < gameCon.itemData.ItemDataList.Count; l++) {
                                if (gameCon.itemData.ItemDataList[l].Name == stringArray[j]) {
                                    gameCon.shopData.ItemDataList[j - 2] = gameCon.itemData.ItemDataList[l];
                                    break;
                                }
                            }
                        }
                    }
                    gameCon.shopData.ShopName = stringArray[1];
                    SysDB.shopFlag = true;
                    SysDB.menueFlag = true;
                    SysDB.sceneName = SceneManager.GetActiveScene().name;
                    SceneManager.LoadScene("Scenes/Shop", LoadSceneMode.Additive);
                    while (SysDB.shopFlag)yield return null;
                } else if (stringArray[0] == "BGM") {//BGM:(開始/停止):番号:フェード時間
                    camera = GameObject.Find("Main Camera");
                    cAudio = camera.GetComponent<AudioSource>();
                    if (int.Parse(stringArray[1]) == 0) {
                        cAudio.clip = gameCon.BGM[int.Parse(stringArray[2])];
                        yield return null;
                        cAudio.Play();
                    } else {
                        //bgmVolume = cAudio.volume;
                        if (stringArray.Length == 4) {
                            yield return StartCoroutine(bgmFade(int.Parse(stringArray[3])));
                        } else {
                            cAudio.Stop();
                            cAudio.volume = (float)bgmVolume;
                        }
                    }
                } else if (stringArray[0] == "Shake") {//Shake:強さx:強さy:回数
                    float stX, stY;
                    pre = SysDB.cameraMove;
                    SysDB.cameraMove = false;
                    stX = float.Parse(stringArray[1]);
                    stY = float.Parse(stringArray[2]);
                    pos = camera.transform.position;
                    for (int l = 0; l < int.Parse(stringArray[3]); l++) {
                        for (int j = 0; j < 12; j++) {
                            camera.transform.position = pos + new Vector3(stX * Mathf.Sin(Mathf.PI * j / 6f), stY * Mathf.Sin(Mathf.PI * j/6f), 0);
                            yield return new WaitForSeconds(1/60f);
                        }
                    }
                    camera.transform.position = pos;
                    SysDB.cameraMove = pre;
                } else if (stringArray[0] == "BoxBig") {//BoxBig:横幅:縦幅
                    BoxCollider2D bc;
                    bc = this.GetComponent<BoxCollider2D>();
                    bc.size = new Vector2(float.Parse(stringArray[1]), float.Parse(stringArray[2]));
                } else if (stringArray[0] == "SpriteDelete") {//SpriteDelete:
                    this.GetComponent<SpriteRenderer>().sprite=null;
                    this.transform.Find("shade").GetComponent<SpriteRenderer>().sprite = null;
                } else if (stringArray[0] == "SpriteDir") {//SpriteDir:対象:x:y(0/1)
                    obj = GameObject.Find(stringArray[1]);
                    obj.GetComponent<SpriteRenderer>().flipX = int.Parse(stringArray[2]) == 1;
                    obj.GetComponent<SpriteRenderer>().flipY = int.Parse(stringArray[3]) == 1;
                } else if (stringArray[0] == "TreasureBox") {//TreasureBox:
                    TreasureBox tre = GetComponent<TreasureBox>();
                    tre.ItemRegister(ref commandArray, k);
                    yield return StartCoroutine(tre.Open());
                } else if (stringArray[0] == "Mushroom") {//Mushroom:高さ1:高さ2:飛ぶ距離:持続時間:強制移動x:強制移動y:スケートか？
                    GameObject shade;
                    Vector3 shadePos = Vector3.zero, cameraPos, force;
                    CircleCollider2D cir;
                    float div = int.Parse(stringArray[4]) * 2;
                    Transform playerT, shadeT = null, cameraT;
                    bool first = true, bind = true;
                    bool skate = stringArray.Length == 8;
                    moveObj = GameObject.Find("Player");
                    playerT = moveObj.transform;
                    shade = moveObj.transform.Find("shade(Clone)").gameObject;
                    force = new Vector3(float.Parse(stringArray[5]), float.Parse(stringArray[6]), 0);
                    if (shade == null) shade = moveObj.transform.Find("shade").gameObject;
                    if (shade != null) {
                        shadePos = shade.transform.position;
                        shadeT = shade.transform;
                    }
                    if (skate) {
                        //moveObj.GetComponent<PlayerMove>().backSkate = true;
                        moveObj.GetComponent<PlayerMove>().skateDir = 180;
                    }
                    yield return null;
                    firPos = moveObj.transform.position;
                    pos = new Vector3(0, 1.2f, 0) + transform.position;
                    if (skate) pos -= new Vector3(0, 1.2f, 0);
                    if ((pos - firPos - new Vector3(0, 0.3f, 0)).magnitude <= 0.4f || skate) {
                        first = false;
                        if (!skate) gameCon.SE(9);
                        else gameCon.SE(12);
                        if (SysDB.playerVelocity != Vector3.zero || force != Vector3.zero) {
                            if (force == Vector3.zero) pos += SysDB.playerVelocity.normalized * float.Parse(stringArray[3]);
                            else pos += force.normalized * float.Parse(stringArray[3]);
                            stringArray[1] = stringArray[2];
                            bind = false;
                        }
                    } else {
                        stringArray[1] = (float.Parse(stringArray[1]) / 2).ToString();
                        div = int.Parse(stringArray[4]);
                    }
                    pre = SysDB.cameraMove;
                    SysDB.cameraMove = false;
                    camera = GameObject.Find("Main Camera");
                    cameraPos = camera.transform.position;
                    cameraT = camera.transform;
                    cir = Player.GetComponent<CircleCollider2D>();
                    cir.enabled = false;
                    for (int j = 1; j <= div; j++) {
                        if ((!first && j % 2 == 0) || skate) pmove.playerRotation(true);
                        if (j <= div / 2 && !first) {
                            transform.localScale = new Vector3(Mathf.Sin(9 * Mathf.PI * j / (div)) / 5 + 1,
                                                               Mathf.Cos(9 * Mathf.PI * j / (div)) / 5 + 1, 1);
                        } else transform.localScale = new Vector3(1, 1, 1);
                        moveTo = firPos + (pos - firPos) * j / div
                                + new Vector3(0, (25 - (j / (div / 10) - 5) * (j / (div / 10) - 5)) * float.Parse(stringArray[1]) / 10, 0);
                        playerT.position = moveTo;
                        if (shade != null) shadeT.position = shadePos + (pos - firPos) * j / div;
                        if (pre && camera != null) cameraT.position = cameraPos + (pos - firPos) * j / div;
                        yield return new WaitForSeconds(1 / 60f);
                    }
                    Player.GetComponent<Animator>().SetInteger("moving", 0);
                    transform.localScale = new Vector3(1, 1, 1);
                    cir.enabled = true;
                    playerT.position = pos;
                    if (shade != null) shadeT.position = shadePos + pos - firPos;
                    SysDB.cameraMove = pre;
                    if (bind) { i--; break; } else gameCon.GetComponent<GameController>().SE(6);
                } else if (stringArray[0] == "Item") {//Item:アイテム番号(名前):増減:表示フラグ
                    if (stringArray[1] == "お金") {
                        myDB.money += int.Parse(stringArray[2]);
                        if (int.Parse(stringArray[3]) == 1) {
                            camera = GameObject.Find("Main Camera");
                            obj = Instantiate(gameCon.effect[0], Vector3.zero, Quaternion.identity);
                            obj.transform.SetParent(GameObject.Find("Canvas").transform, false);
                            obj.GetComponent<RectTransform>().position = RectTransformUtility.WorldToScreenPoint(camera.GetComponent<Camera>(), GameObject.Find("Player").transform.position + new Vector3(0, 0.5f, 0));
                            text = int.Parse(stringArray[2]).ToString()+ "G";
                            obj.GetComponent<Text>().text = text;
                            obj.transform.Find("Damage").gameObject.GetComponent<Text>().text = text;
                            obj.transform.localScale *= 0.7f;
                            gameCon.GetComponent<GameController>().SE(7);
                        }
                    } else {
                        if (!int.TryParse(stringArray[1], out int res)) {
                            for (int j = 0; j < gameCon.itemData.ItemDataList.Count; j++) {
                                if (gameCon.itemData.ItemDataList[j].Name == stringArray[1]) { res = j; break; }
                            }
                        }
                        gameCon.itemData.ItemDataList[res].Possess += int.Parse(stringArray[2]);
                        if (int.Parse(stringArray[3]) == 1) {
                            camera = GameObject.Find("Main Camera");
                            obj = Instantiate(gameCon.effect[0], Vector3.zero, Quaternion.identity);
                            obj.transform.SetParent(GameObject.Find("Canvas").transform, false);
                            obj.GetComponent<RectTransform>().position = RectTransformUtility.WorldToScreenPoint(camera.GetComponent<Camera>(), GameObject.Find("Player").transform.position + new Vector3(0, 0.5f, 0));
                            text = gameCon.itemData.ItemDataList[res].Name + " x" + int.Parse(stringArray[2]).ToString();
                            obj.GetComponent<Text>().text = text;
                            obj.transform.Find("Damage").gameObject.GetComponent<Text>().text = text;
                            obj.transform.localScale *= 0.7f;
                            gameCon.GetComponent<GameController>().SE(7);
                        }
                    }

                } else if (stringArray[0] == "Destroy") {//Destroy:対象(省略で自分)
                    if (stringArray.Length == 1) Destroy(this.gameObject);
                    else Destroy(GameObject.Find(stringArray[1]));

                } else if (stringArray[0] == "GaGe") {//GaGe:
                    GameObject Ga, Ge;
                    Ga = GameObject.Find("Ga");
                    Ge = GameObject.Find("Ge");
                    if (Ga != null) {
                        if (gameCon.getMon(81))Ga.transform.position = new Vector3(-115.5f,-53,0);
                        else Ga.transform.position = new Vector3(-115.5f, -353, 0);
                    }
                    if (Ge != null) {
                        if (gameCon.getMon(79)) Ge.transform.position = new Vector3(-55.5f, -53, 0);
                        else Ge.transform.position = new Vector3(-55.5f, -353, 0);
                    }
                } else if (stringArray[0] == "Nh") {//Nh:
                    GameObject Nh;
                    Nh = GameObject.Find("Nh");
                    if (Nh != null) {
                        if (gameCon.NhJudge()) Nh.transform.position = new Vector3(-21.5f, -32, 0);
                        else Nh.transform.position = new Vector3(100, -32, 0);
                    }
                } else if (stringArray[0] == "CameraChange") {//CameraChange:0/1(動く/止まる)
                    if (int.Parse(stringArray[1]) == 0) SysDB.cameraMove = false;
                    else SysDB.cameraMove = true;
                } else if (stringArray[0] == "Select") {//Select:喋る人:セリフ:内容1:移動1:内容2:移動2...
                    if (!firstMessage) {
                        gameCon.GetComponent<GameController>().SE(10);
                        firstMessage = true;
                    }
                    if (stringArray[2] != "") {
                        nameText.text = stringArray[1];
                        messageText.text = stringArray[2].Replace(";", "\n");
                        messageText.text = messageText.text.Replace("[P]", myDB.playerName);
                        nameRect.localPosition = new Vector3(-274f, 135f, 0);
                        messageRect.localPosition = new Vector3(0, 45f, 0);
                        textRect.localPosition = new Vector3(4.2f, 23f, 0);

                    }
                    selectWin.SetActive(true);
                    selectWin.transform.localPosition = new Vector3(0, -332, 0);
                    for (int l = 0; l < 6; l++) {
                        buttonRe[l] = buttonRect(Button[l]);
                        if ((stringArray.Length - 3) / 2 <= l) Button[l].GetComponent<Text>().text = "";
                        else Button[l].GetComponent<Text>().text = stringArray[l * 2 + 3];
                    }
                    mode = 0;
                    while (Input.GetKeyDown(KeyCode.Z) || Input.GetMouseButton(0)) {
                        yield return null;
                    }
                    mode = 1;
                    selectButtonNum = -1;
                    while (mode != 1 || selectButtonNum == -1) yield return null;
                    mode = 0;
                    selectWin.transform.localPosition = new Vector3(0, -1400, 0);
                    nameRect.localPosition = new Vector3(-279.5f, -1500, 0);
                    messageRect.localPosition = new Vector3(0, -1500, 0);
                    textRect.localPosition = new Vector3(4.2f, -1500, 0);
                    i = int.Parse(stringArray[selectButtonNum * 2 + 4]) - 1;
                    break;
                } else if (stringArray[0] == "Tutorial") {//Tutorial:
                    if (gameCon.itemData.ItemDataList[gameCon.navi].Possess >= 1) {
                        gameCon.GetComponent<GameController>().SE(10);
                        tutoRect.localPosition = new Vector3(0, 344, 0);
                        tutorialText.text = "●アマルガのナビゲーション\n\n" + gameCon.tutoData.Tutorial[exist.TutorialFlagNumber];
                        while (Input.GetKeyDown(KeyCode.Z) || Input.GetMouseButton(0)) yield return null;
                        while (!Input.GetKeyDown(KeyCode.Z) && !Input.GetMouseButton(0)) yield return null;
                        while (Input.GetKeyDown(KeyCode.Z) || Input.GetMouseButton(0)) yield return null;
                        tutoRect.localPosition = new Vector3(0, -1500, 0);
                        exist.changeTutorial(-1);
                    }
                } else if (stringArray[0] == "MonsterGet") {//MonsterGet:ID:レベル
                    gameCon.addMonster(int.Parse(stringArray[1]), int.Parse(stringArray[2]),0);
                } else if (stringArray[0] == "ToMush") {//ToMush:
                    GetComponent<MushRoom>().ToMushroom();
                } else if (stringArray[0] == "ToHasu") {//ToHasu:オブジェクト名
                    GameObject.Find(stringArray[1]).GetComponent<MushRoom>().ToHasu();
                } else if (stringArray[0] == "Break") {
                    goto finish;
                } else if (stringArray[0] == "End") {
                    SysDB.cameraMove = true;
                    SysDB.eventFlag = false;
                    SceneManager.LoadScene("Scenes/Ending");
                    yield break;
                } else if (stringArray[0] == "JumpResister") {//JumpResister:番号:待つかどうか
                    if (int.Parse(stringArray[2]) == 1) yield return StartCoroutine(gameCon.jumpResister(int.Parse(stringArray[1])));
                    else gameCon.jumpRes(int.Parse(stringArray[1]));

                } else if (stringArray[0] == "FileRegister") {//FileRegister:番号
                    int regNum = int.Parse(stringArray[1]);
                    List<int> regi;
                    if (regNum == 0) regi = new List<int>() { 35, 42, 34, 79, 113, 126, 136, 138, 139, 141, 142 };
                    else if (regNum == 1) regi = new List<int>() { 143, 144, 145, 52, 146, 156, 157, 158, 159, 80, 160 };
                    else if (regNum == 2) regi = new List<int>() { 84, 85, 86, 87, 89, 90, 91, 92, 95, 97, 102 };
                    else if (regNum == 3) regi = new List<int>() { 103, 106, 107, 108, 109, 111, 112, 116, 117, 118, 119 };
                    else if (regNum == 4) regi = new List<int>() { 120, 122, 123, 124, 125, 128, 130, 131, 132, 133, 153 };
                    else if (regNum == 5) regi = new List<int>() { 154, 5, 6, 7, 17, 31, 44, 55, 76, 78, 82, 83 };
                    else regi = new List<int>() { };
                    foreach (int j in regi)gameCon.monster.RegistFlag[j-1] = true;
                } else {
                    if (!firstMessage) {
                        gameCon.GetComponent<GameController>().SE(10);
                        firstMessage = true;
                    }
                    SysDB.bgmOff = false;
                    nameRect.localPosition = new Vector3(-274f, 135f, 0);
                    messageRect.localPosition = new Vector3(0, 45f, 0);
                    textRect.localPosition = new Vector3(4.2f, 23f, 0);
                    while (Input.GetKeyDown(KeyCode.Z) || Input.GetMouseButton(0)) yield return null;
                    while (!Input.GetKeyDown(KeyCode.Z) && !Input.GetMouseButton(0)) yield return null;
                    while (Input.GetKeyDown(KeyCode.Z) || Input.GetMouseButton(0)) yield return null;

                    nameRect.localPosition = new Vector3(-279.5f, -1500, 0);
                    messageRect.localPosition = new Vector3(0, -1500, 0);
                    textRect.localPosition = new Vector3(4.2f, -1500, 0);
                }
            }
        }
        finish:
        SysDB.cameraMove = true;
        if (lastCommand!="BattleStart") SysDB.eventFlag = false;
        yield break;
    }
    IEnumerator bgmFade(int time) {
        for (int j = 1; j <= time; j++) {
            yield return new WaitForSeconds(1 / 60f);
            cAudio.volume = (float)bgmVolume * (time - j) / time;
        }
        cAudio.Stop();
        cAudio.volume = (float)bgmVolume;
        yield return null;
    }
    Rect buttonRect(GameObject obj) {//矩形
        RectTransform rect = obj.GetComponent<RectTransform>();
        float wid, hei;
        Vector2 widhei;
        Rect pos = ReRectPosition(obj, canvas.gameObject);
        widhei = ReRectScale(obj, canvas.gameObject);
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
    int judge(int num) {
        int pre = 0;
        if (num == 0) {
            for (int i = 0; i < 4; i++) if (myDB.party[i] >= 0) pre++;
            if (pre >= 2) return 1;
        } else if (num == 1) {
            if (ItemPossess("低級捕集セット") == 0) return 1;
        } else if (num == 2) {//モンスター図鑑50種類以上
            int sumMonster = 0;
            for(int i=0;i< gameCon.monster.GetFlag.Length;i++) sumMonster += gameCon.monster.GetFlag[i]?1:0;
            Debug.Log(sumMonster);
            if (sumMonster >= 50) return 1;
            else return 0;
        }else if(num == 3) {
            if (gameCon.getMon(14) && gameCon.getMon(47) && gameCon.getMon(57)) return 1;
        } else if (num == 4) {//炎色反応コンプリート
            if (gameCon.flagData.Event[37]>=127) return 1;
        } else if (num == 5) {//ハーバー・ボッシュのお供え
            return gameCon.itemData.ItemDataList[31].Possess * gameCon.itemData.ItemDataList[39].Possess >= 1 ? 1 : 0;
        } else if (num == 6) {//高温高圧のお供え
            return gameCon.itemData.ItemDataList[31].Possess * gameCon.itemData.ItemDataList[29].Possess>=1?1:0;
        } else if (num == 7) {//クリア
            return gameCon.flagData.Event[26];
        }
        return 0;
    }
    int ItemPossess(string name) {
        for(int i = 0; i < gameCon.itemData.ItemDataList.Count; i++) {
            if (name == gameCon.itemData.ItemDataList[i].Name) return gameCon.itemData.ItemDataList[i].Possess;
        }
        return 0;
    }
    void Update() {
        Vector2 mousePos;
        bool selectFlag;
        if (mode == 1 && (Input.GetMouseButtonUp(0) || Input.GetMouseButton(0))) {//モンスター
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas, Input.mousePosition, cameraAsp, out mousePos);
            selectFlag = false;
            for (int i = 0; i < 6; i++) {
                if (Input.GetMouseButtonUp(0) && Button[i].GetComponent<Text>().text != "") {
                    if (buttonRe[i].Contains(mousePos)) selectButtonNum = i;
                } else if (Input.GetMouseButton(0) && Button[i].GetComponent<Text>().text != "") {
                    if (buttonRe[i].Contains(mousePos)) {
                        selectFlag = true;
                        selectCursor.transform.position = Button[i].transform.position;
                    }
                }
            }
            if (!selectFlag) selectCursor.transform.position = new Vector3(-1000, 0, 0);
        }
    }
    /*void activeScene(Scene scene, LoadSceneMode mode) {
        if (scene.name == "MainCamera") {
            SceneManager.sceneLoaded -= activeScene;
            GameObject obj = GameObject.Find("FadeCanvas");
            if (obj != null) {
                obj.GetComponent<Fade>().FadeOut(mapFade, mapFadeTime);
            }
            GameObject.Find("Player").transform.position = mapPos;
            SysDB.eventFlag = false;
        }
    }*/
}
