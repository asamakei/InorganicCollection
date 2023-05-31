using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetData : MonoBehaviour{

    public AllyData[] monster = new AllyData[4];
    public string[] playerName = new string[2];
    public bool[] ready = new bool[2];
    public bool[] escape = new bool[2];
    public int[,,] command = new int[2, 4, 2];
    public bool[] commandFlag = new bool[2];
    public int[,] rand = new int[2,100];
    public int mode = 4;

    public bool[] noNeedSend = new bool[5];
    public bool[] sendNoNeed = new bool[5];
    public bool[] getEnemy = new bool[5];

    public void MonsterSet(int monsterNum, AllyData ally) {
        monster[monsterNum] = ally;
    }
    public AllyData MonsterGet(int monsterNum) {
        return monster[monsterNum];
    }
    public void DataReset() {
        monster = new AllyData[4];
        playerName = new string[2];
        ready = new bool[2];
        escape = new bool[2];
        command = new int[2, 4, 2];
        commandFlag = new bool[2];
        rand = new int[2, 100];
        mode = 4;
        noNeedSend = new bool[5];
        sendNoNeed = new bool[5];
        getEnemy = new bool[5];
    }
}
