using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
 
public class NetController : MonoBehaviourPunCallbacks {

    public AllyDatas allyDB;
    public MonsterStates monDB;
    public GameObject[] Button;
    public GameObject[] Cursor;
    public GameObject[] Window;
    public GameObject netConPrefab;

    GameObject[] monsters = new GameObject[4];
    GameObject connection,instanceNet;
    GameObject canvas;
    IEnumerator buttonCoroutine;
    NetBattle netCon;
    Text connecting;
    Image[] ButtonImage;
    PlaySE SE;
    Rect[] buttonRe;
    Fade fade;
    float volume;
    int cursorNumber;
    int buttonNum;
    int[] buttonList;
    int[] cursorList;
    int lastButton;
    string roomName;
    string error="";
    bool battleFlag = true;
    bool getmouse;
    bool buttonFlag = false;
    bool returnToTitle = false;
    bool notmain = false;
    Text[] historyText = new Text[3];

    void Start() {
        SE = GetComponent<PlaySE>();
        netCon = GetComponent<NetBattle>();
        canvas = GameObject.Find("Canvas");
        fade = GameObject.Find("FadeCanvas").GetComponent<Fade>();
        connection = Window[3].transform.Find("Connecting").gameObject;
        connecting = Window[3].transform.Find("Text").GetComponent<Text>();
        Array.Resize(ref buttonRe, Button.Length);
        Array.Resize(ref ButtonImage, Button.Length);
        historyText[0] = Window[5].transform.Find("DateText").GetComponent<Text>();
        historyText[1] = Window[5].transform.Find("NameText").GetComponent<Text>();
        historyText[2] = Window[5].transform.Find("ResultText").GetComponent<Text>();

        for (int i = 0; i < Button.Length; i++) {
            buttonRe[i] = buttonRect(Button[i]);
            ButtonImage[i] = Button[i].GetComponent<Image>();
        }
        GetComponent<SaveLoad>().Load(1);
        if (!PlayerPrefs.HasKey("PlayerName")) error = "セーブデータが存在しません\nゲームを始めてセーブをしてください";
        else {
            MonsterCreate();
            Connecting(true);
        }//PhotonNetwork.Disconnect();
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster() {//接続成功
        Connecting(false);
        if (!notmain) StartCoroutine(main());
        else notmain = false;
    }
    public override void OnDisconnected(DisconnectCause cause){//接続失敗
        if (!returnToTitle) {
            error = "接続に失敗しました";
            StopAllCoroutines();
            for (int i = 1; i < Window.Length; i++) Window[i].SetActive(false);
            StartCoroutine(main());
        }
    }

    IEnumerator main() {
        Connecting(false);
        for (int i = 1; i < Window.Length; i++) Window[i].SetActive(false);
        if (error != "") {
            BattleAble(false);
            playSE(2);
            yield return Message(error);
            error = "";
        }
        yield return buttonPush(0, 5, 0);

        if (battleFlag || buttonNum == 5) playSE(0);
        else { playSE(1); buttonNum = -1; }
        switch (buttonNum) {
            case 0: StartCoroutine(SomeBody()); break;//誰かと
            case 1: StartCoroutine(SearchRoom()); break;//部屋を探す
            case 2: StartCoroutine(MakeRoom()); break;//部屋を作る
            case 3: StartCoroutine(PartyMake()); break;//パーティー編成
            case 4: StartCoroutine(History()); break;//対戦履歴
            case 5: StartCoroutine(ToTitle()); break;//戻る
            default: break;
        }
        if (!battleFlag)StartCoroutine(main());
    }
    //-------------------------------------------------
    IEnumerator SomeBody() {//誰かと対戦
        Window[2].SetActive(true);
        yield return buttonPush(8,10,1);
        if(buttonNum == 2) {//戻る
            playSE(1);
            Window[2].SetActive(false);
            StartCoroutine(main());
            yield break;
        }
        playSE(0);
        int rank = buttonNum == 0 ? 0 : partyRank(5);
        ExitGames.Client.Photon.Hashtable roomHash = new ExitGames.Client.Photon.Hashtable();
        roomHash.Add("r", rank);
        PhotonNetwork.JoinRandomRoom(roomHash, 2);
        connecting.text = "接続中...";
        connection.SetActive(true);
        Window[3].SetActive(true);
        Button[11].SetActive(false);

    }
    IEnumerator SearchRoom() {//部屋を探す
        Window[4].transform.Find("OKButton").Find("Text").GetComponent<Text>().text="探す";
        yield return keyboad(20);
        if (roomName == "") yield break;
        string newRoomName = "[friend]_" + "_" + roomName;
        PhotonNetwork.JoinRoom(newRoomName);
        connecting.text = "接続中...";
        connection.SetActive(true);
        Window[3].SetActive(true);
        Button[11].SetActive(false);
    }
    IEnumerator MakeRoom() {//部屋を作る
        Window[4].transform.Find("OKButton").Find("Text").GetComponent<Text>().text = "作る";
        yield return keyboad(20);
        if (roomName == "") yield break;
        int rank = -1;
        ExitGames.Client.Photon.Hashtable roomHash = new ExitGames.Client.Photon.Hashtable();
        roomHash.Add("r", rank);
        string newRoomName = "[friend]_" + "_" + roomName;
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.CustomRoomPropertiesForLobby = new string[] { "r" };
        roomOptions.CustomRoomProperties = roomHash;
        roomOptions.MaxPlayers = 2;
        PhotonNetwork.CreateRoom(newRoomName, roomOptions, TypedLobby.Default);
        connecting.text = "接続中...";
        connection.SetActive(true);
        Window[3].SetActive(true);
        Button[11].SetActive(false);
    }
    IEnumerator PartyMake() {//パーティー編成
        SysDB.netFlag = true;
        SceneManager.LoadScene("Scenes/MonsterBank", LoadSceneMode.Additive);
        while (SysDB.netFlag) yield return null;
        playSE(1);
        MonsterCreate();
        yield return new WaitForSeconds(10 / 60f);
        StartCoroutine(main());
    }
    IEnumerator History() {//対戦履歴
        for (int j = 0; j < 3; j++) historyText[j].text = "";
        for (int i = 0; i < 10; i++) {
            if (PlayerPrefs.HasKey("History" + i.ToString())) {
                string[] historyString;
                historyString = PlayerPrefs.GetString("History" + i.ToString()).Split('_');
                if (historyString.Length != 3) continue;
                for (int j = 0; j < 3; j++)historyText[j].text += historyString[j] + "\n";
                Debug.Log(i);
            } else break;
        }
        Window[5].SetActive(true);
        while (Input.GetKeyDown(KeyCode.Z) || Input.GetMouseButton(0)) yield return null;
        while (!Input.GetKeyDown(KeyCode.Z) && !Input.GetMouseButton(0)) yield return null;
        while (Input.GetKeyDown(KeyCode.Z) || Input.GetMouseButton(0)) yield return null;
        Window[5].SetActive(false);
        playSE(1);
        StartCoroutine(main());
    }
    IEnumerator ToTitle() {//タイトルに戻る
        returnToTitle = true;
        if (PhotonNetwork.IsConnected) PhotonNetwork.Disconnect();
        fade.GetComponent<Fade>().FadeIn(1, 1.0f);
        StartCoroutine(BGMFadeOut());
        yield return new WaitForSeconds(150 / 60f);
        SceneManager.LoadScene("Scenes/Title");
    }
    //-------------------------------------------------
    public override void OnJoinRandomFailed(short returnCode, string message) {//ランダムで失敗
        int rank = buttonNum == 0 ? 0 : partyRank(5);
        roomName = "";
        ExitGames.Client.Photon.Hashtable roomHash = new ExitGames.Client.Photon.Hashtable();
        roomHash.Add("r", rank);
        string newRoomName = "[Somebody]_" + SysDB.randomInt(1, 1000000) + "_" + SysDB.randomInt(1, 1000000);
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.CustomRoomPropertiesForLobby = new string[] { "r" };
        roomOptions.CustomRoomProperties = roomHash;
        roomOptions.MaxPlayers = 2;
        PhotonNetwork.CreateRoom(newRoomName, roomOptions, TypedLobby.Default);
    }
    public override void OnJoinedRoom() {//部屋に入った時
        StartCoroutine(WaitInRoom());
    }
    public override void OnCreateRoomFailed(short returnCode,string message){
        Window[1].SetActive(false);
        Window[2].SetActive(false);
        Window[3].SetActive(false);
        Window[4].SetActive(false);
        StartCoroutine(CreateFailed());
    }
    public override void OnJoinRoomFailed(short returnCode, string message) {
        Window[1].SetActive(false);
        Window[2].SetActive(false);
        Window[3].SetActive(false);
        Window[4].SetActive(false);
        StartCoroutine(JoinFailed());
    }
    IEnumerator CreateFailed() {
        playSE(2);
        yield return Message("すでに存在する部屋名です");
        StartCoroutine(main());
    }
    IEnumerator JoinFailed() {
        playSE(2);
        yield return Message("存在しない部屋名です");
        StartCoroutine(main());
    }
    IEnumerator WaitInRoom() {//部屋で待機
        if(roomName=="")connecting.text = "対戦相手を探しています...";
        else connecting.text = "部屋: "+roomName+"\n で待機中...";
        buttonCoroutine = buttonPush(11, 11, -1);
        StartCoroutine(buttonCoroutine);
        Button[11].SetActive(true);
        Debug.Log("現在の部屋:" + PhotonNetwork.CurrentRoom.Name);
        Window[3].SetActive(true);
        while (PhotonNetwork.CurrentRoom.PlayerCount<2) {
            if (buttonNum == 0) {
                playSE(1);
                connecting.text = "キャンセル中...";
                PhotonNetwork.LeaveRoom();
                yield break;
            }
            yield return new WaitForSeconds(20/60f);
        }
        StopCoroutine(buttonCoroutine);
        PhotonNetwork.CurrentRoom.IsOpen = false;
        buttonFlag = false;
        connection.SetActive(false);
        Button[11].SetActive(false);
        playSE(3);
        connecting.text = "相手が見つかりました\n対戦を開始します！";
        //バトル設定
        GameObject.Find("NetData").GetComponent<NetData>().DataReset();
        SysDB.netFlag = true;
        SysDB.BGM = 11;
        SysDB.Battle = -1;
        SysDB.SE = 0;
        SysDB.Back = 13;
        SysDB.Enemy = 1;
        PhotonNetwork.SerializationRate = 1;
        PhotonNetwork.SendRate = 1;
        //画面遷移
        yield return new WaitForSeconds(1);
        fade.GetComponent<Fade>().FadeIn(1, 1.0f);

        AudioSource audioS = GetComponent<AudioSource>();
        volume = audioS.volume;
        for (int i = 0; i < 150; i++) {
            audioS.volume = volume * (149 - i) / 149f;
            yield return new WaitForSeconds(1 / 60f);
        }
        audioS.Stop();
        audioS.volume = 0;

        instanceNet = PhotonNetwork.Instantiate("NetBattleController",Vector3.zero,Quaternion.identity);
        SceneManager.sceneLoaded += BattleStart;
        SceneManager.sceneUnloaded += BattleFinish;
        SceneManager.LoadScene("Scenes/Battle",LoadSceneMode.Additive);
        
    }
    void BattleStart(Scene scene, LoadSceneMode mode) {
        SceneManager.sceneLoaded -= BattleStart;
        if (scene.name == "Battle") {
            canvas.SetActive(false);
            for (int i = 0; i < 4; i++) {
                if(monsters[i]!=null) monsters[i].SetActive(false);
            }
            StartCoroutine(BattleStart());
        }
    }
    IEnumerator BattleStart() {
        BattleController batcon = GameObject.Find("BattleController").GetComponent<BattleController>();
        fade.FadeOut(1,1.0f);
        yield return new WaitForSeconds(10 / 60f);

    }
    public override void OnLeftRoom() {
        Window[3].SetActive(false);
        Connecting(true);
    }
    void Connecting(bool show) {
        if (show) {
            connecting.text = "接続中...";
            connection.SetActive(true);
            Window[3].SetActive(true);
            Button[11].SetActive(false);
        } else {
            connection.SetActive(false);
            Window[3].SetActive(false);
            Button[11].SetActive(false);
        }
    }
    IEnumerator keyboad(int max) {
        Text nameText = Window[4].transform.Find("Text").GetComponent<Text>();
        Window[4].SetActive(true);
        roomName = "";
        while (true) {
            nameText.text = "部屋名: " + roomName;
            yield return buttonPush(12, 50, -1);
            if (buttonNum >= 0 && buttonNum <= 25) {//アルファベット
                playSE(4);
                if (roomName.Length <= max) roomName += (char)(buttonNum + 'A');
            } else if (buttonNum >= 26 && buttonNum <= 35) {//数字
                playSE(4);
                if (roomName.Length <= max) roomName += (char)(buttonNum - 26 + '0');
            } else if (buttonNum == 36) {//決定
                if (roomName != "") {
                    playSE(0);
                    Window[4].SetActive(false);
                    yield break;
                } else playSE(1);
            } else if (buttonNum == 37) {//一文字削除
                playSE(1);
                if (roomName.Length > 0) roomName = roomName.Substring(0, roomName.Length - 1);
            } else if (buttonNum == 38) {//戻る
                roomName = "";
                playSE(1);
                Window[4].SetActive(false);
                StartCoroutine(main());
                yield break;
            }
        }
    }
    void MonsterCreate() {//パーティーモンスター表示
        GameObject obj;
        Vector3[] pos = { new Vector3(-11.4f, 2.3f, 0)
                        , new Vector3(-8.6f, 1.5f, 0)
                        , new Vector3(-11.2f, 0.3f, 0)
                        , new Vector3(-8.4f, -0.6f, 0) };
        for (int i = 0; i < 4; i++) {
            if (monsters[i] != null) Destroy(monsters[i]);
            if (myDB.party[i] < 0) continue;
            obj = monDB.MonsterDataList[allyDB.AllyDataList[myDB.party[i]].ID].FileName;
            if (obj == null) continue;
            monsters[i] = Instantiate(obj,pos[i],Quaternion.identity);
        }
    }

    void playSE(int num) { playSE(num, 0); }//効果音再生
    void playSE(int num,int delay) {StartCoroutine(se(num,delay));}
    IEnumerator se(int num,int delay) {
        yield return new WaitForSeconds(delay/60f);
        SE.play(num);
    }

    IEnumerator BGMFadeOut() {//BGMフェードアウト
        volume = GetComponent<AudioSource>().volume;
        for (int i = 0; i < 120; i++) {
            GetComponent<AudioSource>().volume = volume * (119 - i) / 119f;
            yield return new WaitForSeconds(1 / 60f);
        }
        GetComponent<AudioSource>().Pause();
        GetComponent<AudioSource>().volume = 0;
        yield return null;
    }

    //メッセージ表示
    IEnumerator Message(string message) { yield return StartCoroutine(Message(message,"","")); }
    IEnumerator Message(string message,string Yes,string No) {
        GameObject YesButton, NoButton,messageCursor;
        Window[1].SetActive(true);
        YesButton = Window[1].transform.Find("YesButton").gameObject;
        NoButton = Window[1].transform.Find("NoButton").gameObject;
        messageCursor = Window[1].transform.Find("Cursor").gameObject;
        Window[1].transform.Find("Text").GetComponent<Text>().text = message;

        if (Yes!=""||No!="") {//ボタン付き
            YesButton.SetActive(true);
            NoButton.SetActive(true);
            messageCursor.SetActive(false);
            YesButton.transform.Find("Text").GetComponent<Text>().text = Yes;
            NoButton.transform.Find("Text").GetComponent<Text>().text = No;
            yield return buttonPush(6, 7, -1);
            if (buttonNum == 0) playSE(0);
            else playSE(1);
            messageCursor.SetActive(true);
        } else {//ボタンなし
            YesButton.SetActive(false);
            NoButton.SetActive(false);
            messageCursor.SetActive(true);
            while (Input.GetKeyDown(KeyCode.Z) || Input.GetMouseButton(0)) yield return null;
            while (!Input.GetKeyDown(KeyCode.Z) && !Input.GetMouseButton(0)) yield return null;
            while (Input.GetKeyDown(KeyCode.Z) || Input.GetMouseButton(0)) yield return null;
            YesButton.SetActive(true);
            NoButton.SetActive(true);
        }
        Window[1].SetActive(false);
        yield return null;
    }

    //ボタン登録
    void buttonRegister(int start, int finish, int cursorConstant) { buttonRegister(start, finish, cursorConstant, ref buttonList, ref cursorList); }
    void buttonRegister(int start, int finish, int cursorConstant, ref int[] buttons, ref int[] cursorType) {
        Array.Resize(ref buttons, finish - start + 1);
        Array.Resize(ref cursorType, finish - start + 1);
        for (int i = 0; i < finish - start + 1; i++) {
            buttons[i] = start + i;
            cursorType[i] = cursorConstant;
        }
    }

    //ボタン処理
    IEnumerator buttonPush(int start, int finish, int cursorType) {
        buttonRegister(start,finish,cursorType);
        yield return buttonPush();
    }
    IEnumerator buttonPush() {
        buttonNum = -1;
        buttonFlag = true;
        while (buttonNum==-1)yield return null;
        buttonFlag = false;
        yield break;
    }

    Rect buttonRect(GameObject obj) {//矩形
        if (obj == null) return new Rect(0, 0, 0, 0);
        RectTransform rect = obj.GetComponent<RectTransform>();
        float wid, hei;
        wid = rect.rect.width * obj.transform.lossyScale.x;
        hei = rect.rect.height * obj.transform.lossyScale.y;
        return new Rect(rect.position.x - wid / 2, rect.position.y - hei / 2, wid, hei);
    }
    int partyRank(int rank) {//パーティーランク
        int ans,res=0,rankValue = 0;
        for (int i = 0; i < 4; i++) {
            if (myDB.party[i] < 0) continue;
            AllyData ally = allyDB.AllyDataList[myDB.party[i]];
            MonsterData mon = monDB.MonsterDataList[ally.ID];
            int[] allyS = { ally.HP,ally.SP,ally.Attack,ally.Defence,ally.Magic,ally.MagicDef,ally.Speed };
            int[] monS = { mon.HPMax,mon.SPMax,mon.AttackMax,mon.DefenceMax,mon.MagicMax,mon.MagicDefMax,mon.SpeedMax };
            for (int j = 0; j < 7; j++)rankValue += (int)Mathf.Floor(allyS[j]*1000/(monS[j]==0?1:monS[j]));
        }
        ans = (int)Mathf.Floor(rankValue / 7000f * rank);
        if (ans >= rank) ans=rank-1;
        if (res < ans) res = ans;
        return res+1;
    }
    void BattleAble(bool flag) {//可能か切り替え
        battleFlag = flag;
        for (int i = 0; i < 5; i++) {
            Button[i].GetComponent<Text>().color = flag?new Color( 1, 1, 1, 1): new Color( 0.5f, 0.5f, 0.6f, 1);
        }
    }
    void BattleFinish(Scene scene) {
        if (scene.name != "Battle") return;
        canvas.SetActive(true);
        for (int i = 1; i < Window.Length; i++) {
            Window[i].SetActive(false);
        }
        GetComponent<AudioSource>().Play();
        GetComponent<AudioSource>().volume = volume;
        SysDB.netFlag = false;
        notmain = true;
        SceneManager.sceneUnloaded -= BattleFinish;
        PhotonNetwork.LeaveRoom();
        MonsterCreate();
        StartCoroutine(main());
        fade.FadeOut(1,1.0f);
    }
    void Update(){
        if (buttonFlag) {//ボタン
            getmouse = Input.GetMouseButton(0);
            lastButton = -1;
            if (Input.GetMouseButtonUp(0) || getmouse) {
                for (int i = 0; i < buttonList.Length; i++) {
                    if (!buttonRe[buttonList[i]].Contains(Input.mousePosition))continue;
                    if (!getmouse) {//mouseUp
                        buttonNum = i;
                        if (cursorList[i] >= 0)Cursor[cursorList[i]].transform.position = new Vector3(-2000, 0, 0);
                        else ButtonImage[buttonList[i]].color = new Color(0.6f, 0.6f, 1, 1);
                    } else {//mouseDown
                        if (lastButton == i) continue;
                        if (cursorList[i] >= 0)Cursor[cursorList[i]].transform.position = Button[buttonList[i]].transform.position;
                        else ButtonImage[buttonList[i]].color = new Color(1, 1, 1, 1);
                        lastButton = i;
                    }
                    break;
                }
            }
            for (int i = 0; i < buttonList.Length; i++) {
                if (lastButton == i) continue;
                if (cursorList[i] >= 0) {
                    if (lastButton >= 0 && cursorList[i] == cursorList[lastButton]) continue;
                    Cursor[cursorList[i]].transform.position = new Vector3(-2000, 0, 0);
                } else ButtonImage[buttonList[i]].color = new Color(0.6f, 0.6f, 1, 1);
            }
        }

    }
}
