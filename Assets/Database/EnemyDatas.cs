using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class EnemyDatas : ScriptableObject {
    public List<EnemyData> EnemyDataList = new List<EnemyData>();
}

[System.Serializable]
public class EnemyData {
    public string Name;
    public int ID;
    public int Level;
    public int HP;
    public int SP;
    public int Attack;
    public int Defence;
    public int Magic;
    public int MagicDef;
    public int Speed;
    public int Money;
    public int Exp;
    public int Equip;
    public int[] Skill;
    public int[] Character;
    public int[] Item;
    public int[] Rate;
    public int GetDiff;
}
