using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class GameController : MonoBehaviour{

    public SaveDatas saveData;
    public ItemDatas itemData;
    public AllyDatas allyData;
    public TutoDatas tutoData;
    public JumpDatas jumpData;
    public ItemDatas shopData;
    public FlagDatas flagData;
    public MonsterStates monster;
    public AudioClip[] BGM;
    public GameObject[] effect;
    public GameObject[] GameObjectsTohidden;
    public GameObject[] GameObjectsTohidden1;
    public int navi;
    public GameObject canvasObj;
    public GameObject cameraObj;
    public GameObject fadeCanv;
    PlaySE se;
    Vector2 mousePos;
    GameObject ca;
    Scene active;
    IEnumerator map;
    GameObject mapNameWin;
    GameObject jumpNameWin;
    RectTransform canvas;
    Camera cameraAsp;
    string nameMap;

    void Start() {
        SceneManager.sceneUnloaded += sceneUnload;
        SceneManager.sceneLoaded += activeScene;
        se = GetComponent<PlaySE>();
        ca = cameraObj;
        canvas = canvasObj.GetComponent<RectTransform>();
        //cameraAsp = cameraObj.GetComponent<Camera>();
        cameraAsp = null;
        for (int i = 0; i < itemData.ItemDataList.Count; i++) {
            if (itemData.ItemDataList[i].Name == "ナビゲーション") { navi = i; break; }
        }
    }
    public void mapDelete() {
        if (map != null) StopCoroutine(map);
        mapNameWin = GameObject.Find("mapNameWin");
        mapNameWin.transform.localPosition = new Vector3(-228, 802, 0);
    }
    public void mapName() { map = mapNameStart(); StartCoroutine(map); }
    IEnumerator mapNameStart() {
        if (SysDB.bgmOff) yield break;
        nameMap = "なぞのばしょ";
        for (int i = 0; i < saveData.SaveDataList.Count; i++) {
            if (saveData.SaveDataList[i].sceneName == SysDB.sceneName) nameMap = saveData.SaveDataList[i].spotName;
        }
        mapNameWin = GameObject.Find("mapNameWin");
        mapNameWin.transform.localPosition = new Vector3(-228,802,0);
        mapNameWin.transform.Find("mapName").GetComponent<Text>().text = nameMap;
        yield return new WaitForSeconds(10 / 60f);
        for (int i = 49; i >=0; i--) {
            mapNameWin.transform.localPosition += new Vector3(0,-i/10f,0);
            yield return new WaitForSeconds(1 / 60f);
        }
        yield return new WaitForSeconds(100 / 60f);
        for (int i = 0; i < 50; i++) {
            mapNameWin.transform.localPosition += new Vector3(0, i / 10f, 0);
            yield return new WaitForSeconds(1 / 60f);
        }
        yield return null;
    }
    public void jumpRes(int num) { StartCoroutine(jumpResister(num)); }
    public IEnumerator jumpResister(int num) {
        if (!jumpData.JumpDataList[num].movable) {
            SE(13);
            jumpData.JumpDataList[num].movable = true;
            jumpNameWin = GameObject.Find("mapJumpWin");
            jumpNameWin.transform.localPosition = new Vector3(233, 802, 0);
            yield return new WaitForSeconds(10 / 60f);
            for (int i = 49; i >= 0; i--) {
                jumpNameWin.transform.localPosition+=new Vector3(0, -i / 10f, 0);
                yield return new WaitForSeconds(1 / 60f);
            }
            yield return new WaitForSeconds(100 / 60f);
            for (int i = 0; i < 50; i++) {
                jumpNameWin.transform.localPosition += new Vector3(0, i / 10f, 0);
                yield return new WaitForSeconds(1 / 60f);
            }
        }
        yield return null;
    }
    public void sceneLoad(string scenePath) {//バトルシーンの遷移(前シーンの状態を保持)
        Fade fade = GameObject.Find("FadeCanvas").GetComponent<Fade>();
        SysDB.eventFlag = true;
        SysDB.battleFlag = true;
        ca.GetComponent<AudioSource>().Pause();
        fade.FadeIn(2,2, () => {
            GameObjectsTohidden = SceneManager.GetActiveScene().GetRootGameObjects();
            Transform MapEffect = GameObject.Find("MapEffect").transform;
            foreach (Transform children in MapEffect)Destroy(children.gameObject);
            foreach (GameObject obj in GameObjectsTohidden) {
                if (obj.name != "Empty" && obj!=fadeCanv) obj.SetActive(false);
            }
            GameObjectsTohidden1 = SceneManager.GetSceneByName("MainCamera").GetRootGameObjects();
            foreach (GameObject obj in GameObjectsTohidden1) {
                if (obj.name != "Main Camera" && obj.name != "FadeCanvas" && obj!= fadeCanv) obj.SetActive(false);
            }
            SceneManager.LoadScene(scenePath, LoadSceneMode.Additive);
            active = SceneManager.GetActiveScene();
            SysDB.eventFlag = false;
            fade.FadeOut(1,1);

        });
    }
    void activeScene(Scene scene, LoadSceneMode mode) {
        SceneManager.SetActiveScene(scene);
    }
    public void sceneUnload(Scene scene) {//バトルシーンの遷移(前シーンへ戻る)
        if (scene.name == "Battle") {
            Fade fade;
            if (fadeCanv != null) {
                fade = fadeCanv.GetComponent<Fade>();
                fade.FadeIn(1, 0.1f, () => {
                    foreach (GameObject obj in GameObjectsTohidden) {
                        obj.SetActive(true);
                        SceneManager.SetActiveScene(active);
                        if (obj.name == "Empty") obj.GetComponent<AudioSource>().UnPause();
                    }
                    foreach (GameObject obj in GameObjectsTohidden1) {
                        obj.SetActive(true);
                        SceneManager.SetActiveScene(active);
                        if (obj.name == "Main Camera") {
                            StartCoroutine(BGMFadeIn(obj.GetComponent<AudioSource>()));
                        }
                    }
                    fade.FadeOut(0, 0.2f);
                });
            }
        }

    }
    IEnumerator BGMFadeIn(AudioSource aud) {
        int frame = 40;
        float volume = aud.volume;
        aud.volume = 0.0f;
        for(int i=0;i<6;i++)yield return null;
        aud.UnPause();
        for (int i = 0; i < frame; i++) {
            aud.volume = (float)i/frame*volume;
            yield return null;
        }
        aud.volume = volume;
    }
    public void SE(int number){
        se.play(number);
    }
    public void addMonster(int ID,int level,int start) {
        int emptyNum;
        for (emptyNum = start; emptyNum < allyData.AllyDataList.Count; emptyNum++) {
            if (allyData.AllyDataList[emptyNum].ID == -1) break;
        }
        if(emptyNum< allyData.AllyDataList.Count) {
            resetAlly(emptyNum,-1);
            setAlly(emptyNum, ID,level);
            if (emptyNum < 4) myDB.party[emptyNum] = emptyNum;
        }
    }
    public bool NhJudge() {
        int GetCount = 0;
        int len = monster.GetFlag.Length;
        for (int i = 0; i < len; i++) {
            if (monster.GetFlag[i]) GetCount++;
        }
        if (GetCount * 100 / len >= 90) return true;
        else return false;
        /*if (GetCount >= len || GetCount == (len - 1) && monster.GetFlag[monster.MonsterDataList[74].FileNum] == false) {
            return true;
        } else return false;*/
    }
    public void setAlly(int index, int num, int lev) {
        AllyDatas ally = allyData;
        int[] skillLevel = { 3, 6, 10, 17, 24, 32, 40, 50 };
        int[] needExp = new int[100];
        int firstLevel = 10;
        needExp[0] = 22;
        for (int i = 1; i <= 98; i++) needExp[i] = Mathf.FloorToInt((1.625f / (i * 1.25f + 1.3f) + 1f) * needExp[i - 1]);

        if (index >= 0 && index < ally.AllyDataList.Count) {
            ally.AllyDataList[index].ID = num;
            if (num == -1) ally.AllyDataList[index].Name = "Nothing";
            else {
                ally.AllyDataList[index].Name = monster.MonsterDataList[num].Name;
                MonsterData mon = monster.MonsterDataList[ally.AllyDataList[index].ID];
                monster.Resister(num, true, true);
               
                //デバッグ用
                mon = new MonsterData();
                mon.HPGrow = 6;
                mon.SPGrow = 5;
                mon.AttackGrow = 4;
                mon.DefenceGrow = 4;
                mon.MagicGrow = 4;
                mon.MagicDefGrow = 4;
                mon.SpeedGrow = 4;

                ally.name = monster.MonsterDataList[num].Name;
                ally.AllyDataList[index].Exp = 0;
                ally.AllyDataList[index].NextExp = needExp[lev];
                ally.AllyDataList[index].Equip = -1;
                ally.AllyDataList[index].CharaPt = lev;
                ally.AllyDataList[index].Level = lev;
                ally.AllyDataList[index].HP = ally.AllyDataList[index].NowHP = lev * mon.HPGrow + 5 * firstLevel + SysDB.randomInt(-5, 5);
                ally.AllyDataList[index].SP = ally.AllyDataList[index].NowSP = lev * mon.SPGrow + 4 * firstLevel + SysDB.randomInt(-2, 2);
                ally.AllyDataList[index].Attack = lev * mon.AttackGrow + 3 * firstLevel;
                ally.AllyDataList[index].Defence = lev * mon.DefenceGrow + 3 * firstLevel;
                ally.AllyDataList[index].Magic = lev * mon.MagicGrow + 3 * firstLevel;
                ally.AllyDataList[index].MagicDef = lev * mon.MagicDefGrow + 3 * firstLevel;
                ally.AllyDataList[index].Speed = lev * mon.SpeedGrow + 3 * firstLevel;
                Array.Resize(ref ally.AllyDataList[index].Grow, 8);
                for (int i = 0; i < 8; i++) ally.AllyDataList[index].Grow[i] = 0;
                Array.Resize(ref ally.AllyDataList[index].Skill, 2);
                Array.Resize(ref ally.AllyDataList[index].SkillUse, 2);

                Array.Resize(ref ally.AllyDataList[index].Skill, 3);
                Array.Resize(ref ally.AllyDataList[index].SkillUse, 3);
                ally.AllyDataList[index].Skill[2] = 3;
                ally.AllyDataList[index].SkillUse[2] = 0;

                ally.AllyDataList[index].Skill[0] = 0;
                ally.AllyDataList[index].Skill[1] = 1;
                ally.AllyDataList[index].SkillUse[0] = 0;
                ally.AllyDataList[index].SkillUse[1] = 0;

                Array.Resize(ref ally.AllyDataList[index].Skill, monster.MonsterDataList[num].Skill.Length+2);
                Array.Resize(ref ally.AllyDataList[index].SkillUse, monster.MonsterDataList[num].Skill.Length+2);
                ally.AllyDataList[index].Skill[0] = 0;
                ally.AllyDataList[index].Skill[1] = 1;
                ally.AllyDataList[index].SkillUse[0] = 0;
                ally.AllyDataList[index].SkillUse[1] = 1;
                for (int i = 0; i < monster.MonsterDataList[num].Skill.Length; i++) {
                    if (lev >= skillLevel[i]) {
                        ally.AllyDataList[index].Skill[i+2] = monster.MonsterDataList[num].Skill[i];
                        ally.AllyDataList[index].SkillUse[i+2] = 1;
                    } else {
                        Array.Resize(ref ally.AllyDataList[index].Skill, i+2);
                        Array.Resize(ref ally.AllyDataList[index].SkillUse, i+2);
                    }
                }
                Array.Resize(ref ally.AllyDataList[index].Character,monster.MonsterDataList[num].Character.Length);
                for (int i = 0; i < monster.MonsterDataList[num].Character.Length; i++) ally.AllyDataList[index].Character[i] = 1;
            }
        }
    }
    public void resetAlly(int index, int num) {
        AllyDatas ally = allyData;
        setAlly(index, num, 1);
        if (index >= 0 && index < ally.AllyDataList.Count) {
            ally.AllyDataList[index].Equip = -1;
            ally.AllyDataList[index].CharaPt = 200;
            ally.AllyDataList[index].HP = 1;
            ally.AllyDataList[index].SP = 1;
            ally.AllyDataList[index].Level = 1;
            ally.AllyDataList[index].Attack = 1;
            ally.AllyDataList[index].Defence = 1;
            ally.AllyDataList[index].Magic = 1;
            ally.AllyDataList[index].MagicDef = 1;
            ally.AllyDataList[index].Speed = 1;
            Array.Resize(ref ally.AllyDataList[index].Grow, 8);
            Array.Resize(ref ally.AllyDataList[index].Skill, 2);
            Array.Resize(ref ally.AllyDataList[index].SkillUse, 2);
            ally.AllyDataList[index].Skill[0] = 0;
            ally.AllyDataList[index].Skill[1] = 1;
            ally.AllyDataList[index].SkillUse[0] = 0;
            ally.AllyDataList[index].SkillUse[1] = 0;
            Array.Resize(ref ally.AllyDataList[index].Character, 0);

        }
    }
    public bool getMon(int ID) {
        bool res=false;
        for(int i = 0; i < 4; i++) {
            if (myDB.party[i] >= 0) res |= allyData.AllyDataList[i].ID == ID;
        }
        return res;
    }
    private void Update() {
        if ((!SysDB.moveMenue) && (!SysDB.eventFlag) && (!SysDB.menueFlag) && Input.GetMouseButtonUp(0)) {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas, Input.mousePosition, cameraAsp, out mousePos);
            mousePos += new Vector2(414, 1475 / 2f);
            if ((!SysDB.battleFlag) && (!SysDB.eventFlag) && mousePos.y < 407 && mousePos.y > 154 && (mousePos - new Vector2(414, 407)).magnitude > 200) {
                SE(2);
                SysDB.sceneName = SceneManager.GetActiveScene().name;
                SceneManager.LoadScene("Scenes/Menue", LoadSceneMode.Additive);
            }
        } else if (Input.GetMouseButtonUp(0)) SysDB.moveMenue = false;
    }
}
