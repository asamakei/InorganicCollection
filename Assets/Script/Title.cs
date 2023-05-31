using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour{
    public MonsterStates StatesData;
    public SaveDatas SaveData;
    public AudioClip[] SE;
    public GameObject[] prefab;

    GameObject[] Button;
    GameObject titleCursor;
    GameObject nameCursor;
    GameObject saveWin;
    GameObject alertWin;
    GameObject nameWin;
    GameObject okWin;
    GameObject nameText;
    GameObject blackWin;
    GameObject[] movie;
    RectTransform canvas;
    Camera cameraAsp;
    AudioSource audioS;
    Fade fade;
    Rect[] buttonRe;
    Scene title;
    Transform atom;


    int titleButtonNum = -1;
    int saveButtonNum = -1;
    int alertButtonNum = -1;
    int nameButtonNum = -1;
    int okButtonNum = -1;
    int mode = 0;
    bool saveReset = false;

    void Start(){
        SysDB.cameraMove = true;
        canvas = GameObject.Find("Canvas").GetComponent<RectTransform>();
        cameraAsp = GameObject.Find("Main Camera").GetComponent<Camera>();
        title = SceneManager.GetActiveScene();
        movie = new GameObject[20];
        Button = new GameObject[62];
        buttonRe = new Rect[62];
        titleCursor = GameObject.Find("titleCursor");
        nameCursor = GameObject.Find("nameCursor");
        saveWin = GameObject.Find("SaveWindow");
        alertWin = GameObject.Find("AlertWindow");
        nameWin = GameObject.Find("NameWindow");
        okWin = GameObject.Find("OkWindow");
        nameText = GameObject.Find("playerName");
        blackWin = GameObject.Find("blackWindow");
        audioS = GetComponent<AudioSource>();
        fade = GameObject.Find("TitleFade").GetComponent<Fade>();
        movie[0] = GameObject.Find("Canvas").transform.Find("movie").gameObject;
        movie[1] = GameObject.Find("horse1");
        movie[2] = GameObject.Find("horse2");
        movie[3] = GameObject.Find("bird1");
        movie[4] = GameObject.Find("bird2");
        movie[5] = GameObject.Find("bird3");
        movie[6] = GameObject.Find("Flare");
        movie[7] = GameObject.Find("back1");
        movie[8] = GameObject.Find("yadokari3");
        movie[9] = GameObject.Find("Flare1");
        movie[10] = GameObject.Find("light");
        movie[11] = GameObject.Find("back");
        movie[12] = GameObject.Find("s1");
        movie[13] = GameObject.Find("Ag1");
        movie[14] = GameObject.Find("yadokari1");
        movie[15] = GameObject.Find("yadokari2");
        movie[16] = GameObject.Find("yadokari3");
        atom = GameObject.Find("AtomList").transform;
        foreach (Transform atomT in atom) {
            float dir = 2 * Mathf.PI * SysDB.randomInt(1,360)/360f;
            atomT.GetComponent<Rigidbody2D>().AddForce(new Vector2(Mathf.Cos(dir), Mathf.Sin(dir))*SysDB.randomInt(1000,3000));
            atomT.GetComponent<Rigidbody2D>().AddTorque(SysDB.randomInt(2000,3000));
        }

        for (int i = 0; i < 3; i++) {
            Button[i] = GameObject.Find("titleButton"+(i+1).ToString());
            buttonRe[i] = buttonRect(Button[i]);
        }
        for (int i = 3; i < 6; i++) {
            //Button[i] = saveWin.transform.Find("Save" + (i - 3).ToString()).gameObject;
            Button[i] = GameObject.Find("Save" + (i - 3).ToString()).gameObject;
            buttonRe[i] = buttonRect(Button[i]);
        }
        Button[6] = GameObject.Find("YesButton");
        buttonRe[6] = buttonRect(Button[6]);
        Button[7] = GameObject.Find("NoButton");
        buttonRe[7] = buttonRect(Button[7]);
        for (int i = 8; i < 60; i++) {
            Button[i] = nameWin.transform.Find("window").transform.Find("name" + (i - 7).ToString()).gameObject;
            buttonRe[i] = buttonRect(Button[i]);
        }
        Button[60] = GameObject.Find("nameYes");
        buttonRe[60] = buttonRect(Button[60]);
        Button[61] = GameObject.Find("nameNo");
        buttonRe[61] = buttonRect(Button[61]);

        saveWin.SetActive(false);
        alertWin.SetActive(false);
        nameWin.SetActive(false);
        okWin.SetActive(false);
        movie[0].SetActive(false);
        movie[7].SetActive(false);
        movie[12].SetActive(false);
        movie[13].SetActive(false);
        blackWin.SetActive(false);
        StartCoroutine(titleMain());
    }
    private string GetString(string id, string empty) {
        if (PlayerPrefs.HasKey(id)) return PlayerPrefs.GetString(id);
        else return empty;
    }
    private int GetInt(string id, int empty) {
        if (PlayerPrefs.HasKey(id)) return PlayerPrefs.GetInt(id);
        else return empty;
    }
    private float GetFloat(string id, float empty) {
        if (PlayerPrefs.HasKey(id)) return PlayerPrefs.GetFloat(id);
        else return empty;
    }
    IEnumerator titleMain() {
        while (true) {
            titleButtonNum = -1;
            while (mode != 0 || titleButtonNum == -1) { yield return null; }
            if (titleButtonNum == 0) {//初めから
                playSE(2, 0);
                GetComponent<SaveLoad>().GameReset();
                yield return new WaitForSeconds(50 / 60f);
                fade.FadeIn(1, 2.0f);
                float volume = GetComponent<AudioSource>().volume;
                for (int i = 0; i < 120; i++) {
                    GetComponent<AudioSource>().volume = volume * (119 - i) / 119f;
                    yield return new WaitForSeconds(1 / 60f);
                }
                GetComponent<AudioSource>().Pause();
                GetComponent<AudioSource>().volume = volume;
                yield return new WaitForSeconds(60 / 60f);
                StartCoroutine(gameStart());
                
                yield break;
            } else if(titleButtonNum == 1) {//続きから
                playSE(4, 0);
                mode = 1;
                saveWin.SetActive(true);
                StartCoroutine(load());
            } else if(titleButtonNum == 2) {//通信
                playSE(2, 0);
                fade.FadeIn(1, 1.0f);
                float volume = GetComponent<AudioSource>().volume;
                for (int i = 0; i < 120; i++) {
                    GetComponent<AudioSource>().volume = volume * (119 - i) / 119f;
                    yield return new WaitForSeconds(1 / 60f);
                }
                yield return new WaitForSeconds(20 / 60f);
                SysDB.eventFlag = false;
                SysDB.menueFlag = false;
                SysDB.battleFlag = false;
                SceneManager.LoadScene("Scenes/NetWork");
            }
        }
    }
    IEnumerator load() {
    saveSelect://ラベル//
        int monsterNum, level, mapNum;
        string nameList = "";
        string levelList = "";
        float volume;
        bool saveNot = true;
        if (PlayerPrefs.HasKey("PlayerName")) {
            mapNum = GetComponent<SaveLoad>().mapNum(GetString("SceneName", "Village1"));
            saveWin.transform.Find("OtherData").GetComponent<Text>().text
                = "セーブデータ\n\n名前：" + GetString("PlayerName","プレイヤー") + "\n"
                + "所持金：" + GetInt("Money",1000).ToString() + "G\n";
            if (mapNum >= 0) {
                saveWin.transform.Find("OtherData").GetComponent<Text>().text += "場所：" + SaveData.SaveDataList[mapNum].spotName;
            }
            for (int i = 0; i < 4; i++) {
                monsterNum = GetInt("AllyParty" + i.ToString(),170);
                if (monsterNum >= 0) {
                    nameList += StatesData.MonsterDataList[GetInt("AllyStates" + i.ToString() + "ID",1)].Name;
                    level = GetInt("AllyStates" + i.ToString() + "Level",1);
                    levelList += "Level.";
                    if (level < 100) levelList += " ";
                    if (level < 10) levelList += " ";
                    levelList += level.ToString();
                }
                nameList += "\n";
                levelList += "\n";
            }
            saveWin.transform.Find("MonsterLevel").GetComponent<Text>().text = levelList;
            saveWin.transform.Find("MonsterName").GetComponent<Text>().text = nameList;
        } else {
            saveNot = false;
            saveWin.transform.Find("OtherData").GetComponent<Text>().text = "セーブデータ\n\n なし";
            saveWin.transform.Find("MonsterLevel").GetComponent<Text>().text = "";
            saveWin.transform.Find("MonsterName").GetComponent<Text>().text = "";
        }
        saveReset = false;
        Button[4].SetActive(saveNot);
        Button[5].SetActive(saveNot);
        while (true) {
            saveButtonNum = -1;
            while (mode != 1 || saveButtonNum == -1) {
                if (saveReset) goto saveSelect;
                yield return null;
            }
            if (saveButtonNum == 3) {//戻る
                playSE(3, 0);
                mode = 0;
                saveWin.SetActive(false);
                yield break;
            } else if (saveButtonNum == 4) {//ロード
                playSE(2, 0);
                fade.FadeIn(1, 1.0f);
                volume = GetComponent<AudioSource>().volume;
                for (int i = 0; i < 120; i++) {
                    GetComponent<AudioSource>().volume = volume * (119-i)/119f;
                    yield return new WaitForSeconds(1 / 60f);
                }
                yield return new WaitForSeconds(20 / 60f);
                SysDB.eventFlag = false;
                SysDB.menueFlag = false;
                SysDB.battleFlag = false;
                GetComponent<SaveLoad>().Load(0);
            } else if (saveButtonNum == 5) {//消去
                playSE(4, 0);
                mode = 2;
                alertWin.SetActive(true);
                StartCoroutine(alert());
            }
        }
    }
    IEnumerator alert() {
        while (true) {
            alertButtonNum = -1;
            while (mode != 2 || alertButtonNum == -1) { yield return null; }
            if (alertButtonNum == 6) {//はい
                playSE(1, 0);
                PlayerPrefs.DeleteAll();
                saveReset = true;
                mode = 1;
                alertWin.SetActive(false);
                yield break;
            } else if (alertButtonNum == 7) {//いいえ
                playSE(3, 0);
                mode = 1;
                alertWin.SetActive(false);
                yield break;
            }
        }
    }
    IEnumerator gameStart() {
        //ゲーム開始ムービー初期設定
        GameObject obj,obj1,obj2;
        Image img;
        float colorA,r,g,b;
        string[] playerName = {"","","","","","",""};
        string[] daku1 = { "カ", "キ", "ク", "ケ", "コ", "サ", "シ", "ス", "セ", "ソ", "タ", "チ", "ツ", "テ", "ト", "ハ", "ヒ", "フ", "ヘ", "ホ", "ウ", "パ", "ピ", "プ", "ペ", "ポ" };
        string[] daku2 = { "ガ", "ギ", "グ", "ゲ", "ゴ", "ザ", "ジ", "ズ", "ゼ", "ゾ", "ダ", "ヂ", "ヅ", "デ", "ド", "バ", "ビ", "ブ", "ベ", "ボ", "ヴ", "バ", "ビ", "ブ", "ベ", "ボ" };
        string[] handaku1 = { "ハ", "ヒ", "フ", "ヘ", "ホ", "バ", "ビ", "ブ", "ベ", "ボ" };
        string[] handaku2 = { "パ", "ピ", "プ", "ペ", "ポ", "パ", "ピ", "プ", "ペ", "ポ" };
        string[] komoji1 = { "ア", "イ", "ウ", "エ", "オ", "ヤ", "ユ", "ヨ" };
        string[] komoji2 = { "ァ", "ィ", "ゥ", "ェ", "ォ", "ャ", "ュ", "ョ" };
        string preName;
        int charCount = 0;
        Destroy(atom.gameObject);
        GetComponent<AudioSource>().clip = null;
        movie[0].SetActive(true);
        obj = movie[0].transform.Find("space").transform.Find("meteor").gameObject;
        obj.SetActive(false);
        yield return new WaitForSeconds(80 / 60f);
        nameWin.SetActive(true);
        fade.FadeOut(1, 0);
        mode = 3;
        while (true) {//名前決定
            nameButtonNum = -1;
            while (mode != 3 || nameButtonNum == -1)yield return null;
            if (nameButtonNum == 57) {//削除
                playSE(3, 0);
                if (charCount <= 7 && charCount>=1) {
                    playerName[charCount - 1] = "";
                    charCount--;
                }
            } else if (nameButtonNum == 58) {//決定
                mode = 4;
                okButtonNum = -1;
                okWin.SetActive(true);
                preName = "";
                for (int i = 0; i < 7; i++)preName += playerName[i];
                if (preName == "") okButtonNum = 61;
                else playSE(4, 0);
                okWin.transform.Find("text").gameObject.GetComponent<Text>().text = preName+" で良いですか？";

                while (mode != 4 || okButtonNum == -1) yield return null;
                if (okButtonNum == 60) {//はい
                    playSE(4, 0);
                    myDB.playerName = preName;
                    break;
                } else if(okButtonNum == 61) {//いいえ
                    playSE(3, 0);
                    okWin.SetActive(false);
                    mode = 3;
                }
            } else if (nameButtonNum == 54 && charCount >= 1) {//゛
                if (charCount <= 6 && charCount >= 1) {
                    for (int i = 0; i <= 25; i++) {
                        if (playerName[charCount - 1] == daku1[i]) {
                            playSE(8, 0);
                            playerName[charCount - 1] = daku2[i];
                            break;
                        }
                    }
                }
            } else if (nameButtonNum == 55 && charCount >= 1) {//゜
                for (int i = 0; i <= 9; i++) {
                    if (playerName[charCount - 1] == handaku1[i]) {
                        playSE(8, 0);
                        playerName[charCount - 1] = handaku2[i];
                        break;
                    }
                }
            } else if (nameButtonNum == 56 && charCount>=1) {//小文字
                for (int i = 0; i <= 7; i++) {
                    if (playerName[charCount - 1] == komoji1[i]) {
                        playSE(8, 0);
                        playerName[charCount - 1] = komoji2[i];
                        break;
                    }
                }
            } else if(nameButtonNum>=8 && nameButtonNum <= 53 || nameButtonNum==59) {//それぞれの文字
                if (charCount <= 6) {
                    playSE(8, 0);
                    playerName[charCount] = Button[nameButtonNum].GetComponent<Text>().text;
                    charCount++;
                }
            }
            preName = "";
            for(int i = 0; i < 7; i++) {
                if (playerName[i] == "") preName += "＿ ";
                else preName += playerName[i]+" ";
            }
            nameText.GetComponent<Text>().text = preName;

        }
        fade.FadeIn(3,1.0f);
        yield return new WaitForSeconds(120 / 60f);
        nameWin.SetActive(false);
        okWin.SetActive(false);
        fade.FadeOut(3, 1.0f);
        //映像開始
        yield return new WaitForSeconds(30 / 60f);
        obj.SetActive(true);
        yield return new WaitForSeconds(800 / 60f);
        obj = movie[0].transform.Find("space").gameObject;
        obj.SetActive(false);
        StartCoroutine(MoveGameObject(movie[3],new Vector3(-270,0,0),200,100));
        StartCoroutine(MoveGameObject(movie[4], new Vector3(270, 40, 0), 400, 230));
        StartCoroutine(MoveGameObject(movie[5], new Vector3(270, -100, 0), 300, 300));
        //StartCoroutine(meteor(100));
        playSE(5, 50);
        playSE(5, 400);
        //yield return MoveGameObject(movie[6], new Vector3(-270, -80, 0), 540, 100);
        yield return MoveGameObject(movie[6], new Vector3(-270, -80, 0), 340, 100);

        yield return new WaitForSeconds(110 / 60f);
        movie[7].SetActive(true);
        StartCoroutine(MoveGameObject(movie[8], new Vector3(-150, 0, 0), 2000, 0));
        playSE(5, 150);
        playSE(5, 500);
        //yield return StartCoroutine(MoveGameObject(movie[9], new Vector3(-300, -60, 0), 560, 100));
        yield return StartCoroutine(MoveGameObject(movie[9], new Vector3(-300, -60, 0), 360, 100));
        yield return new WaitForSeconds(140 / 60f);
        playSE(6, 0);
        StartCoroutine(shake(movie[7],1,500,500,20,60));
        yield return new WaitForSeconds(150 / 60f);
        obj1 = Instantiate(prefab[1],Vector3.zero,Quaternion.identity,movie[7].transform);
        //obj1.transform.parent = canvas.transform;
        obj1.transform.localPosition = new Vector3(-454, 100, 0);
        obj1.transform.localScale = new Vector3(1,0.7f,1);
        playSE(7, 0);
        for (int i = 0; i < 100; i++) {
            obj1.transform.localScale *= Mathf.Pow(30,1/70f);
            yield return new WaitForSeconds(1 / 60f);
        }
        yield return new WaitForSeconds(100 / 60f);
        movie[7].SetActive(false);
        StartCoroutine(shake(movie[11], 1, 500, 300, 20, 0));
        yield return new WaitForSeconds(60 / 60f);
        Destroy(obj1);
        obj2 = Instantiate(prefab[1], Vector3.zero, Quaternion.identity, movie[11].transform);
        //obj2.transform.parent = canvas.transform;
        obj2.transform.localPosition = new Vector3(-454, 100, 0);
        obj2.transform.localScale = new Vector3(1, 0.7f, 1);
        playSE(7, 0);
        for (int i = 0; i < 100; i++) {
            obj2.transform.localScale *= Mathf.Pow(30, 1 / 70f);
            yield return new WaitForSeconds(1 / 60f);
        }
        movie[1].SetActive(false);
        movie[2].SetActive(false);
        movie[13].SetActive(true);
        yield return new WaitForSeconds(200 / 60f);
        img = obj2.GetComponent<Image>();
        r = img.color.r;
        g = img.color.g;
        b = img.color.b;
        colorA = img.color.a;
        for (int i = 0; i < 200; i++) {
            img.color = new Color(r * (199 - i) / 200f, g * (199 - i) / 200f, b * (199 - i) / 200f, colorA);
            yield return new WaitForSeconds(1 / 60f);
        }
        //Destroy(obj1);
        Destroy(obj2);
        yield return new WaitForSeconds(200 / 60f);
        movie[7].SetActive(true);
        movie[12].SetActive(true);
        movie[14].SetActive(false);
        movie[15].SetActive(false);
        movie[16].SetActive(false);
        yield return new WaitForSeconds(200 / 60f);
        yield return fade.FadeIn(3,5);
        blackWin.SetActive(true);
        yield return fade.FadeOut(0, 0);
        fade.gameObject.SetActive(false);
        Destroy(fade.gameObject);
        SysDB.encount = 400;
        SysDB.encItem = 0;
        SysDB.eventFlag = false;
        SysDB.bgmOff = true;
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene("Scenes/MainCamera", LoadSceneMode.Additive);
        //マップ移動
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        GameObject player;
        if (scene.name == "MainCamera") {
            player = GameObject.Find("Player");
            player.transform.position = new Vector3(-10.5f, 3.5f,0);
            player.GetComponent<PlayerMove>().playerDirection(new Vector2(1,0));
            SceneManager.LoadScene("Scenes/Village1", LoadSceneMode.Additive);
        } else if(scene.name == "Village1") {
            SceneManager.SetActiveScene(scene);
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.UnloadSceneAsync(title);
        }
    }
    IEnumerator meteor(int delay) {//隕石エフェクト
        GameObject obj = GameObject.Find("Flare");
        GameObject fire;
        Vector2 metePos;
        yield return new WaitForSeconds(delay / 60f);
        for (int i = 0; i < 140; i++) {
            metePos = obj.transform.localPosition;
            fire = Instantiate(prefab[0],metePos,Quaternion.identity,movie[0].transform);
            fire.transform.localPosition += new Vector3(SysDB.randomInt(0,20), SysDB.randomInt(-15, 25),0);
            fire.transform.localScale *= 3;
            yield return new WaitForSeconds(5 / 60f);
        }
        yield return null;
    }
    IEnumerator MoveGameObject(GameObject obj, Vector3 pos, int frame, int delay) {//オブジェクト移動処理
        yield return StartCoroutine(MoveToGameObject(obj, obj.transform.localPosition + pos, frame, delay));
    }
    IEnumerator MoveToGameObject(GameObject obj, Vector3 pos, int frame, int delay) {//オブジェクト移動処理
        Vector3 moving = pos - obj.transform.localPosition;
        yield return new WaitForSeconds(delay / 60f);
        for (int i = 0; i < frame; i++) {
            obj.transform.localPosition += moving / frame;
            yield return new WaitForSeconds(1 / 60f);
        }
        //obj.transform.position = pos;
    }

    Rect buttonRect(GameObject obj) {//矩形
        RectTransform rect = obj.GetComponent<RectTransform>();
        float wid, hei;
        Vector2 widhei;
        Rect pos = ReRectPosition(obj,canvas.gameObject);
        widhei = ReRectScale(obj,canvas.gameObject);
        wid = rect.rect.width * widhei.x;
        hei = rect.rect.height * widhei.y;
        return new Rect(pos.x - wid / 2,
                        pos.y - hei / 2, wid, hei);
    }
    Vector2 ReRectScale(GameObject obj,GameObject canv) {
        if (obj == canv) return Vector2.one;
        Vector2 parent = ReRectScale(obj.transform.parent.gameObject,canv);
        return new Vector2(obj.transform.localScale.x*parent.x, obj.transform.localScale.y*parent.y);
    }
    Rect ReRectPosition(GameObject obj, GameObject canv) {
        if (obj.transform.parent.gameObject == canv)return new Rect(obj.transform.localPosition.x, obj.transform.localPosition.y,1,1);
        Rect parent = ReRectPosition(obj.transform.parent.gameObject, canv);
        float wi,he;
        wi = obj.transform.parent.localScale.x*parent.width;
        he = obj.transform.parent.localScale.y*parent.height;
        return new Rect(parent.x + obj.transform.localPosition.x * wi, parent.y + obj.transform.localPosition.y * he, wi,he);
    }

    IEnumerator shake(GameObject obj,int mode,int speed,int frame,int scale,int delay) {//振動
        float move;
        yield return new WaitForSeconds(delay / 60f);
        for (int i = 0; i < frame; i++) {
            move = Mathf.Cos(Mathf.PI/1000f*speed*i)*scale;
            obj.transform.localPosition += new Vector3(mode == 0 ? move : 0, mode == 1 ? move : 0, 0);
            yield return new WaitForSeconds(1 / 60f);
        }
    }
    void Update() {//ボタンタップ判定
        Vector2 mousePos;
        bool titleFlag,nameFlag;
        if (mode == 0 && (Input.GetMouseButtonUp(0) || Input.GetMouseButton(0))) {//メイン
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas, Input.mousePosition, null, out mousePos);
            titleFlag = false;
            for (int i = 0; i < 3; i++) {
                if (Input.GetMouseButtonUp(0)) {
                    if (buttonRe[i].Contains(mousePos)) titleButtonNum = i;
                } else if (Input.GetMouseButton(0)) {
                    if (buttonRe[i].Contains(mousePos)) {
                        titleCursor.transform.position = Button[i].transform.position;
                        titleFlag = true;
                    }
                }
            }
            if (!titleFlag) titleCursor.transform.position = new Vector3(-1000, 0, 0);
        }
        if (mode == 1 && (Input.GetMouseButtonUp(0) || Input.GetMouseButton(0))) {//ロード
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas, Input.mousePosition, null, out mousePos);
            for (int i = 3; i < 6; i++) {
                if (Input.GetMouseButtonUp(0)) {
                    if (buttonRe[i].Contains(mousePos) && Button[i].activeSelf) saveButtonNum = i;
                    Button[i].GetComponent<Image>().color = new Color(0.5f, 0.56f, 1, 1);
                } else if (Input.GetMouseButton(0)) {
                    if (buttonRe[i].Contains(mousePos)) {
                        Button[i].GetComponent<Image>().color = new Color(1, 1, 1, 1);
                    } else if (Button[i] != null) Button[i].GetComponent<Image>().color = new Color(0.5f, 0.56f, 1, 1);
                }
            }
        }
        if (mode == 2 && (Input.GetMouseButtonUp(0) || Input.GetMouseButton(0))) {//消しますか
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas, Input.mousePosition, null, out mousePos);
            for (int i = 6; i < 8; i++) {
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
        if (mode == 3 && (Input.GetMouseButtonUp(0) || Input.GetMouseButton(0))) {//名前入力
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas, Input.mousePosition, null, out mousePos);
            nameFlag = false;
            for (int i = 8; i < 60; i++) {
                if (Input.GetMouseButtonUp(0)) {
                    if (buttonRe[i].Contains(mousePos)) nameButtonNum = i;
                } else if (Input.GetMouseButton(0)) {
                    if (buttonRe[i].Contains(mousePos)) {
                        nameCursor.transform.position = Button[i].transform.position;
                        if (i == 57 || i == 58) nameCursor.transform.localScale = new Vector3(2.5f,2.5f,1);
                        else nameCursor.transform.localScale = new Vector3(1.2f, 2.5f, 1);
                        nameFlag = true;
                    }
                }
            }
            if (!nameFlag)nameCursor.transform.position = new Vector3(-1000, 0, 0);
        }
        if (mode == 4 && (Input.GetMouseButtonUp(0) || Input.GetMouseButton(0))) {//名前決定
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas, Input.mousePosition, null, out mousePos);
            for (int i = 60; i < 62; i++) {
                if (Input.GetMouseButtonUp(0)) {
                    if (buttonRe[i].Contains(mousePos)) okButtonNum = i;
                    Button[i].GetComponent<Image>().color = new Color(0.5f, 0.56f, 1, 1);
                } else if (Input.GetMouseButton(0)) {
                    if (buttonRe[i].Contains(mousePos)) {
                        Button[i].GetComponent<Image>().color = new Color(1, 1, 1, 1);
                    } else if (Button[i] != null) Button[i].GetComponent<Image>().color = new Color(0.5f, 0.56f, 1, 1);
                }
            }
        }
    }
    void playSE(int i, int delay) { StartCoroutine(delaySE(i, delay)); }
    IEnumerator delaySE(int i, int delay) {//SE再生
        yield return new WaitForSeconds(delay / 60f);
        audioS.PlayOneShot(SE[i], 2);
    }
}
