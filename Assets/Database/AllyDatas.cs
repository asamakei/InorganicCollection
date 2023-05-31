using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class AllyDatas : ScriptableObject {
    public List<AllyData> AllyDataList = new List<AllyData>();
}

[System.Serializable]
public class AllyData {
    public string Name;
    public int ID;
    public int Level;
    public int HP;
    public int NowHP;
    public int SP;
    public int NowSP;
    public int Attack;
    public int Defence;
    public int Magic;
    public int MagicDef;
    public int Speed;
    public int Exp;
    public int NextExp;
    public int Equip;
    public int CharaPt;
    public int[] Skill;
    public int[] SkillUse;//スキル使用許可:0 禁止:1
    public int[] Character;//特徴レベル
    public int[] Grow = new int[7];
    public int preNumber;
}