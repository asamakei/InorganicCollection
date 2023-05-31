using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class BattleDB : ScriptableObject{
    public Vector4[] EnemyGroup;
    public int[,] Group = new int[100,4];
    public string[] buffName;
    public bool[] death = new bool[8];
    public int turn = 1;
    public int itemID = -1, itemTarget = -1;
    public bool auto;
    public int[,] States = new int[8, 15];
    public int[,] StatesCor = new int[8, 15];
    //States[モンスター番号(0～3:敵,4～7:自),ステータス番号]
    //ステータス番号  0:モンスターナンバー,1:レベル,2:最大HP,3:現在HP,4:最大SP,5:現在SP
    //6:物理攻撃 7:物理防御 8:魔法攻撃 9:魔法防御 10:素早さ 
    //StatesCor...補正後ステータス
    public int[] priority = new int[8];
    //priority[モンスター番号] 行動優先度
    public string[] CharaName = new string[8];
    //CharaName[モンスター番号]
    public int[,] behavior = new int[8, 2];
    //behavior[モンスター番号,オプション]
    //オプション: 0:相手　1:技ID
    public int[,] monsterBuff = new int[8, 40];
    //monsterBuff[モンスター番号,バフID]
}
