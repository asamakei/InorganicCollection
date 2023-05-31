using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveLoad : MonoBehaviour{
    public AllyDatas ally;
    public ItemDatas item;
    public SaveDatas savedata;
    public FlagDatas flagdata;
    public MonsterStates monDB;
    public JumpDatas jumpDB;
    int number = -1;
    Scene title;
    GameObject Player,obj;
    Vector3 pos;

    public void Save() {
        Player = GameObject.Find("Player");
        PlayerPrefs.SetString("PlayerName", myDB.playerName);//プレイヤー名
        PlayerPrefs.SetString("SceneName", SysDB.sceneName);//現在マップ名
        PlayerPrefs.SetFloat("PlayerPositionX", Player.transform.position.x);//現在位置
        PlayerPrefs.SetFloat("PlayerPositionY", Player.transform.position.y);
        PlayerPrefs.SetFloat("PlayerPositionZ", Player.transform.position.z);

        PlayerPrefs.SetInt("HasuFlag", SysDB.hasuFlag?1:0);//ハスフラグ
        PlayerPrefs.SetInt("encount", SysDB.encount);//エンカウント
        PlayerPrefs.SetInt("encItem", SysDB.encItem);//エンカウント


        for (int i = 0; i < item.ItemDataList.Count; i++) {//アイテム所持数
            PlayerPrefs.SetInt("ItemPossess" + i.ToString(), item.ItemDataList[i].Possess);
        }
        PlayerPrefs.SetInt("Money", myDB.money);//所持金
        for (int i = 0; i < 4; i++) {//パーティー編成
            PlayerPrefs.SetInt("AllyParty" + i.ToString(), myDB.party[i]);
        }
        for(int i = 0; i < flagdata.Event.Length; i++) {//イベントフラグ
            PlayerPrefs.SetInt("EventFlag"+i.ToString(), flagdata.Event[i]);
        }
        for (int i = 0; i < flagdata.Treasure.Length; i++) {//宝箱フラグ
            PlayerPrefs.SetInt("TreasureFlag" + i.ToString(), flagdata.Treasure[i]);
        }
        for (int i = 0; i < flagdata.Tutorial.Length; i++) {//チュートリアルフラグ
            PlayerPrefs.SetInt("TutorialFlag" + i.ToString(), flagdata.Tutorial[i]);
        }
        for (int i = 0; i < jumpDB.JumpDataList.Count; i++) {//ジャンプフラグ
            PlayerPrefs.SetInt("Jump" + i.ToString(), jumpDB.JumpDataList[i].movable ? 1:0);
        }
        PlayerPrefs.SetInt("AllyStatesCount", ally.AllyDataList.Count);
        for (int i = 0; i < ally.AllyDataList.Count; i++) {//仲間モンスターステータス
            PlayerPrefs.SetInt("AllyStates" + i.ToString() + "ID", ally.AllyDataList[i].ID);
            if (ally.AllyDataList[i].ID >= 1) {
                PlayerPrefs.SetInt("AllyStates" + i.ToString() + "SkillCount", ally.AllyDataList[i].Skill.Length);
                PlayerPrefs.SetInt("AllyStates" + i.ToString() + "SkillUseCount", ally.AllyDataList[i].SkillUse.Length);
                PlayerPrefs.SetInt("AllyStates" + i.ToString() + "CharaCount", ally.AllyDataList[i].Character.Length);
                PlayerPrefs.SetInt("AllyStates" + i.ToString() + "GrowCount", ally.AllyDataList[i].Grow.Length);
                PlayerPrefs.SetInt("AllyStates" + i.ToString() + "Level", ally.AllyDataList[i].Level);
                PlayerPrefs.SetInt("AllyStates" + i.ToString() + "HP", ally.AllyDataList[i].HP);
                PlayerPrefs.SetInt("AllyStates" + i.ToString() + "NowHP", ally.AllyDataList[i].NowHP);
                PlayerPrefs.SetInt("AllyStates" + i.ToString() + "SP", ally.AllyDataList[i].SP);
                PlayerPrefs.SetInt("AllyStates" + i.ToString() + "NowSP", ally.AllyDataList[i].NowSP);
                PlayerPrefs.SetInt("AllyStates" + i.ToString() + "Attack", ally.AllyDataList[i].Attack);
                PlayerPrefs.SetInt("AllyStates" + i.ToString() + "Defence", ally.AllyDataList[i].Defence);
                PlayerPrefs.SetInt("AllyStates" + i.ToString() + "Magic", ally.AllyDataList[i].Magic);
                PlayerPrefs.SetInt("AllyStates" + i.ToString() + "MagicDef", ally.AllyDataList[i].MagicDef);
                PlayerPrefs.SetInt("AllyStates" + i.ToString() + "Speed", ally.AllyDataList[i].Speed);
                PlayerPrefs.SetInt("AllyStates" + i.ToString() + "Exp", ally.AllyDataList[i].Exp);
                PlayerPrefs.SetInt("AllyStates" + i.ToString() + "NextExp", ally.AllyDataList[i].NextExp);
                PlayerPrefs.SetInt("AllyStates" + i.ToString() + "CharaPt", ally.AllyDataList[i].CharaPt);
                PlayerPrefs.SetInt("AllyStates" + i.ToString() + "Equip", ally.AllyDataList[i].Equip);
                for (int j = 0; j < ally.AllyDataList[i].Skill.Length; j++) {
                    PlayerPrefs.SetInt("AllyStates" + i.ToString() + "Skill" + j.ToString(), ally.AllyDataList[i].Skill[j]);
                }
                for (int j = 0; j < ally.AllyDataList[i].SkillUse.Length; j++) {
                    PlayerPrefs.SetInt("AllyStates" + i.ToString() + "SkillUse" + j.ToString(), ally.AllyDataList[i].SkillUse[j]);
                }
                for (int j = 0; j < ally.AllyDataList[i].Character.Length; j++) {
                    PlayerPrefs.SetInt("AllyStates" + i.ToString() + "Character" + j.ToString(), ally.AllyDataList[i].Character[j]);
                }
                for (int j = 0; j < ally.AllyDataList[i].Grow.Length; j++) {
                    PlayerPrefs.SetInt("AllyStates" + i.ToString() + "Grow" + j.ToString(), ally.AllyDataList[i].Grow[j]);
                }
            }
        }
        for(int i = 0; i < monDB.FileFlag.Length; i++) {
            PlayerPrefs.SetInt("FileFlag" + i.ToString(), monDB.FileFlag[i] ? 1 : 0);
            PlayerPrefs.SetInt("GetFlag" + i.ToString(), monDB.GetFlag[i] ? 1 : 0);
            PlayerPrefs.SetInt("RegistFlag" + i.ToString(), monDB.RegistFlag[i] ? 1 : 0);
        }
        ////////////シーン固有データセーブ
        number = mapNum(SysDB.sceneName);
        if (number != -1) {
            for (int i = 0; i < savedata.SaveDataList[number].ObjectName.Length; i++) {
                obj = GameObject.Find(savedata.SaveDataList[number].ObjectName[i]);
                if (obj != null) {
                    PlayerPrefs.SetFloat(obj.name + SysDB.sceneName + "PositionX", obj.transform.position.x);
                    PlayerPrefs.SetFloat(obj.name + SysDB.sceneName + "PositionY", obj.transform.position.y);
                    PlayerPrefs.SetFloat(obj.name + SysDB.sceneName + "PositionZ", obj.transform.position.z);
                }
            }
        }
        PlayerPrefs.Save();
    }

    private string GetString(string id,string empty) {
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
    public void Load(int mode) {
        int length;

        myDB.playerName = GetString("PlayerName","プレイヤー");//プレイヤー名

        for (int i = 0; i < GetInt("AllyStatesCount",170); i++) {//仲間モンスターステータス
            if (i >= ally.AllyDataList.Count) ally.AllyDataList.Add(new AllyData());
            ally.AllyDataList[i].ID = GetInt("AllyStates" + i.ToString() + "ID",1);
            ally.AllyDataList[i].Name = "";

            if (ally.AllyDataList[i].ID >= 1) {
                ally.AllyDataList[i].Name = monDB.MonsterDataList[ally.AllyDataList[i].ID].Name;
                ally.AllyDataList[i].Level = GetInt("AllyStates" + i.ToString() + "Level",1);
                ally.AllyDataList[i].HP = GetInt("AllyStates" + i.ToString() + "HP",1);
                if (SysDB.netFlag) ally.AllyDataList[i].NowHP = ally.AllyDataList[i].HP;
                else ally.AllyDataList[i].NowHP = GetInt("AllyStates" + i.ToString() + "NowHP",1);
                ally.AllyDataList[i].SP = GetInt("AllyStates" + i.ToString() + "SP",1);
                if (SysDB.netFlag) ally.AllyDataList[i].NowSP = ally.AllyDataList[i].SP;
                else ally.AllyDataList[i].NowSP = GetInt("AllyStates" + i.ToString() + "NowSP",1);
                ally.AllyDataList[i].Attack = GetInt("AllyStates" + i.ToString() + "Attack",1);
                ally.AllyDataList[i].Defence = GetInt("AllyStates" + i.ToString() + "Defence",1);
                ally.AllyDataList[i].Magic = GetInt("AllyStates" + i.ToString() + "Magic",1);
                ally.AllyDataList[i].MagicDef = GetInt("AllyStates" + i.ToString() + "MagicDef",1);
                ally.AllyDataList[i].Speed = GetInt("AllyStates" + i.ToString() + "Speed",1);
                ally.AllyDataList[i].Exp = GetInt("AllyStates" + i.ToString() + "Exp",1);
                ally.AllyDataList[i].NextExp = GetInt("AllyStates" + i.ToString() + "NextExp",1);
                ally.AllyDataList[i].CharaPt = GetInt("AllyStates" + i.ToString() + "CharaPt",1);
                ally.AllyDataList[i].Equip = GetInt("AllyStates" + i.ToString() + "Equip",0);

                length = GetInt("AllyStates" + i.ToString() + "SkillCount",2);
                ally.AllyDataList[i].Skill = new int[length];
                Array.Resize(ref ally.AllyDataList[i].Skill,length);
                for (int j = 0; j < length; j++) ally.AllyDataList[i].Skill[j] = GetInt("AllyStates" + i.ToString() + "Skill" + j.ToString(),0);
                length = GetInt("AllyStates" + i.ToString() + "SkillUseCount", ally.AllyDataList[i].Skill.Length);
                ally.AllyDataList[i].SkillUse = new int[length];
                Array.Resize(ref ally.AllyDataList[i].SkillUse, length);
                for (int j = 0; j < length; j++) ally.AllyDataList[i].SkillUse[j] = GetInt("AllyStates" + i.ToString() + "SkillUse" + j.ToString(),0);
                length = GetInt("AllyStates" + i.ToString() + "CharaCount",6);
                ally.AllyDataList[i].Character = new int[length];
                Array.Resize(ref ally.AllyDataList[i].Character, length);
                for (int j = 0; j < length; j++) ally.AllyDataList[i].Character[j] = GetInt("AllyStates" + i.ToString() + "Character" + j.ToString(),0);
                length = GetInt("AllyStates" + i.ToString() + "GrowCount",7);
                ally.AllyDataList[i].Grow = new int[length];
                Array.Resize(ref ally.AllyDataList[i].Grow, length);
                for (int j = 0; j < length; j++) ally.AllyDataList[i].Grow[j] = GetInt("AllyStates" + i.ToString() + "Grow" + j.ToString(),2);
            }
        }
        for(int i= GetInt("AllyStatesCount", 170); i < ally.AllyDataList.Count; i++) {
            ally.AllyDataList[i].ID = -1;
            ally.AllyDataList[i].Name = "";
        }
        for (int i = 0; i < 4; i++) {//パーティー編成
            myDB.party[i] = GetInt("AllyParty" + i.ToString(),i);
        }
        if (mode == 0) {
            SysDB.sceneName = GetString("SceneName", "FirstTileMap");//現在マップ名
            if (mapNum(SysDB.sceneName) < 0) {//エラーの場合
                pos = new Vector3(-31f, 20f,0f);
            } else {
                pos = new Vector3(GetFloat("PlayerPositionX", 0),//現在位置
                                  GetFloat("PlayerPositionY", 0),
                                  GetFloat("PlayerPositionZ", 0));
            }
            title = SceneManager.GetActiveScene();
            SysDB.hasuFlag = GetInt("HasuFlag",0) == 1;//ハスフラグ
            if (PlayerPrefs.HasKey("encount")) {
                SysDB.encount = GetInt("encount",400);//エンカウント
                SysDB.encItem = GetInt("encItem",0);//エンカウント
            } else {
                SysDB.encount = 400;
                SysDB.encItem = 0;
            }

            for (int i = 0; i < item.ItemDataList.Count; i++) {//アイテム所持数
                item.ItemDataList[i].Possess = GetInt("ItemPossess" + i.ToString(),0);
            }
            myDB.money = GetInt("Money",1000);//所持金
            for (int i = 0; i < flagdata.Event.Length; i++) {//イベントフラグ
                flagdata.Event[i] = GetInt("EventFlag" + i.ToString(),0);
            }
            for (int i = 0; i < flagdata.Treasure.Length; i++) {//宝箱フラグ
                flagdata.Treasure[i] = GetInt("TreasureFlag" + i.ToString(),0);
            }
            for (int i = 0; i < flagdata.Tutorial.Length; i++) {//チュートリアルフラグ
                flagdata.Tutorial[i] = GetInt("TutorialFlag" + i.ToString(),0);
            }
            for (int i = 0; i < jumpDB.JumpDataList.Count; i++) {//ジャンプフラグ
                jumpDB.JumpDataList[i].movable = GetInt("Jump" + i.ToString(),0)==1;
            }
            for (int i = 0; i < monDB.FileFlag.Length; i++) {
                monDB.FileFlag[i] = GetInt("FileFlag"+i.ToString(),0) == 1;
                monDB.GetFlag[i] = GetInt("GetFlag" + i.ToString(),0) == 1;
                monDB.RegistFlag[i] = GetInt("RegistFlag" + i.ToString(),0) == 1;
            }

            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.LoadScene("Scenes/MainCamera", LoadSceneMode.Additive);
        }
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        
        if (scene.name == "MainCamera") {
            Player = GameObject.Find("Player");
            if (Player != null) Player.transform.position = pos;
            if (mapNum(SysDB.sceneName) < 0) SysDB.sceneName = "FirstTileMap";
            SceneManager.LoadScene("Scenes/" + SysDB.sceneName, LoadSceneMode.Additive);
        } else {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.SetActiveScene(scene);
            ////////////シーン固有データロード
            number = mapNum(SysDB.sceneName);
            if (number != -1) {
                for (int i = 0; i < savedata.SaveDataList[number].ObjectName.Length; i++) {
                    obj = GameObject.Find(savedata.SaveDataList[number].ObjectName[i]);
                    if (obj != null) {
                        obj.transform.position = new Vector3(
                        GetFloat(obj.name + SysDB.sceneName + "PositionX",0),
                        GetFloat(obj.name + SysDB.sceneName + "PositionY",0),
                        GetFloat(obj.name + SysDB.sceneName + "PositionZ",0));
                    }
                }
            }
            SceneManager.UnloadSceneAsync(title);
        }
    }
    public int mapNum(string name) {
        for(int i = 0; i < savedata.SaveDataList.Count; i++) {
            if (name == savedata.SaveDataList[i].sceneName) return i;
        }
        return -1;
    }
    public void GameReset() {
        for (int i = 0; i < flagdata.Event.Length; i++) flagdata.Event[i] = 0;
        for (int i = 0; i < flagdata.Treasure.Length; i++) flagdata.Treasure[i] = 0;
        for (int i = 0; i < flagdata.Tutorial.Length; i++) flagdata.Tutorial[i] = 0;
        for (int i = 0; i < item.ItemDataList.Count; i++) item.ItemDataList[i].Possess = 0;
        for (int i = 0; i < ally.AllyDataList.Count; i++) {
            ally.AllyDataList[i] = new AllyData();
            ally.AllyDataList[i].ID = -1;
            ally.AllyDataList[i].Name = "";
        }
        int firstLevel = 10;
        AllyData firstAlly = new AllyData();
        firstAlly.Name = "二酸化炭素";
        firstAlly.ID = 8;
        firstAlly.Level = 1;
        firstAlly.HP = monDB.MonsterDataList[8].HPGrow + 5 * firstLevel;
        firstAlly.NowHP = monDB.MonsterDataList[8].HPGrow + 5 * firstLevel;
        firstAlly.SP = monDB.MonsterDataList[8].SPGrow + 4 * firstLevel;
        firstAlly.NowSP = monDB.MonsterDataList[8].SPGrow + 4 * firstLevel;
        firstAlly.Attack = monDB.MonsterDataList[8].AttackGrow + 3 * firstLevel;
        firstAlly.Defence = monDB.MonsterDataList[8].DefenceGrow + 3 * firstLevel;
        firstAlly.Magic = monDB.MonsterDataList[8].MagicGrow + 3 * firstLevel;
        firstAlly.MagicDef = monDB.MonsterDataList[8].MagicDefGrow + 3 * firstLevel;
        firstAlly.Speed = monDB.MonsterDataList[8].SpeedGrow + 3 * firstLevel;
        firstAlly.Exp = 0;
        firstAlly.NextExp = 16;
        firstAlly.Equip = -1;
        firstAlly.CharaPt = 1;
        Array.Resize(ref firstAlly.Skill, 2);
        Array.Resize(ref firstAlly.SkillUse, 2);
        Array.Resize(ref firstAlly.Character, 3);
        firstAlly.Skill[0] = 0;
        firstAlly.Skill[1] = 1;
        firstAlly.SkillUse[0] = 1;
        firstAlly.SkillUse[1] = 1;
        firstAlly.Character[0] = 1;
        firstAlly.Character[1] = 1;
        firstAlly.Character[2] = 1;
        firstAlly.Grow[0] = monDB.MonsterDataList[8].HPGrow;
        firstAlly.Grow[1] = monDB.MonsterDataList[8].SPGrow;
        firstAlly.Grow[2] = monDB.MonsterDataList[8].AttackGrow;
        firstAlly.Grow[3] = monDB.MonsterDataList[8].DefenceGrow;
        firstAlly.Grow[4] = monDB.MonsterDataList[8].MagicGrow;
        firstAlly.Grow[5] = monDB.MonsterDataList[8].MagicDefGrow;
        firstAlly.Grow[6] = monDB.MonsterDataList[8].SpeedGrow;
        ally.AllyDataList[0] = firstAlly;
        myDB.party[0] = 0;
        myDB.party[1] = myDB.party[2] = myDB.party[3] = -1;
        myDB.money = 0;
        for (int i = 0; i < monDB.FileFlag.Length; i++) monDB.FileFlag[i] = false;
        for (int i = 0; i < monDB.GetFlag.Length; i++) monDB.GetFlag[i] = false;
        for (int i = 0; i < monDB.RegistFlag.Length; i++) monDB.RegistFlag[i] = false;
        for (int i = 0; i < jumpDB.JumpDataList.Count; i++) jumpDB.JumpDataList[i].movable = false;
        monDB.GetFlag[7]= monDB.FileFlag[7] = true;
        jumpDB.JumpDataList[0].movable = true;
    }
}
