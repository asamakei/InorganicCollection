using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DebugMode : MonoBehaviour{
    public AllyDatas ally;
    public MonsterStates monster;
    public SkillDatas skillDB;
    int num=0;
    bool debugMode = false;
    GameController gameCon;
    
    void Start(){
        if (debugMode) {
            gameCon = GameObject.Find("GameController").GetComponent<GameController>();
            myDB.party[0] = 0;
            myDB.party[1] = 1;
            myDB.party[2] = 2;
            myDB.party[3] = 3;
            myDB.money = 9999999;

            for (int i = 1; i < monster.MonsterDataList.Count; i++) {
                if (monster.MonsterDataList[i].Name != "") num++;
            }

            for (int i = 0; i < 400; i++) gameCon.resetAlly(i, -1);

            for (int i = 0; i < 4; i++) gameCon.setAlly(i, 1, 45);
            for (int i = 4; i < num; i++) gameCon.setAlly(i, SysDB.randomInt(1, num), 45);
            for (int i = 0; i < 4; i++) {
                int skillCount = SysDB.randomInt(6, 10);
                Array.Resize(ref ally.AllyDataList[i].Skill, skillCount);
                Array.Resize(ref ally.AllyDataList[i].SkillUse, skillCount);
                ally.AllyDataList[i].Skill[0] = 0;
                ally.AllyDataList[i].Skill[1] = 1;
                for (int j = 2; j < skillCount; j++) {
                    ally.AllyDataList[i].Skill[j] = SysDB.randomInt(2, skillDB.SkillDataList.Count - 1);
                    ally.AllyDataList[i].SkillUse[j] = 0;
                }
                //ally.AllyDataList[i].HP = 5000;
                //ally.AllyDataList[i].NowHP = 5000;
                //ally.AllyDataList[i].Attack = 5000;
            }
            for (int i = 4; i < num; i++) {
                int skillCount = SysDB.randomInt(2, 10);
                Array.Resize(ref ally.AllyDataList[i].Skill, skillCount);
                Array.Resize(ref ally.AllyDataList[i].SkillUse, skillCount);
                ally.AllyDataList[i].Skill[0] = 0;
                ally.AllyDataList[i].Skill[1] = 1;
                for (int j = 2; j < skillCount; j++) {
                    ally.AllyDataList[i].Skill[j] = SysDB.randomInt(2, skillDB.SkillDataList.Count - 1);
                    ally.AllyDataList[i].SkillUse[j] = 0;
                }
            }
        }
        for(int i = 0; i < monster.MonsterFile.Length; i++) {
            if (monster.MonsterFile[i] >= 0 && monster.MonsterFile[i] < monster.MonsterDataList.Count) {
                if (monster.MonsterFile[i] == 8) {
                    monster.GetFlag[i]=true;
                    monster.FileFlag[i] = true;
                }
                monster.MonsterDataList[monster.MonsterFile[i]].FileNum = i;
            }
        }
    }
}
