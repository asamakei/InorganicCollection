using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class MonsterStates : ScriptableObject {
    public List<MonsterData> MonsterDataList = new List<MonsterData>();
    public int[] MonsterFile;
    public bool[] FileFlag;
    public bool[] GetFlag;
    public bool[] RegistFlag;

    public void Resister(int ID,bool File,bool Get) {
        if (ID >= 1) {
            int num = MonsterDataList[ID].FileNum;
            FileFlag[num] |= File;
            GetFlag[num] |= Get;
        }
    }
    public void getInfo(int ID) {
        if (ID >= 1)RegistFlag[MonsterDataList[ID].FileNum] =true;
    }
}

[System.Serializable]
public class MonsterData {
    public string Name;
    public string Formula;
    public GameObject FileName;
    public GameObject FileNameMiror;
    public Sprite Icon;
    [Multiline(3)]
    public string Detail;
    [Multiline(3)]
    public string howMake;
    public int HPGrow;
    public int SPGrow;
    public int AttackGrow;
    public int DefenceGrow;
    public int MagicGrow;
    public int MagicDefGrow;
    public int SpeedGrow;
    public int HPMax;
    public int SPMax;
    public int AttackMax;
    public int DefenceMax;
    public int MagicMax;
    public int MagicDefMax;
    public int SpeedMax;
    public int Flame;
    public int Water;
    public int Electrical;
    public int Oxidation;
    public int Reduction;
    public int OxidationP;
    public int FileNum;
    public int[] Skill;
    public int[] Character;
}