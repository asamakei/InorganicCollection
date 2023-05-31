using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SysDB : MonoBehaviour{
    public static int SE;
    public static int BGM;
    public static int Back;
    public static int Enemy;
    public static int encount = 400;
    public static int encItem = 0;

    public static int Battle;
    public static int BattleResult;
    public static Vector3 playerVelocity;

    public static bool animationFlag;
    public static bool eventFlag = false;
    public static bool menueFlag = false;
    public static bool battleFlag = false;
    public static bool cameraMove = true;
    public static bool shopFlag = false;
    public static bool hasuFlag = false;
    public static bool netFlag = false;
    public static bool bgmOff = false;
    public static bool moveMenue = false;

    public static string sceneName;
    public static int randomInt(int min, int max) {return Random.Range(min, max+1);}
    public static bool dice(int rate) {return randomInt(0, 100) <= rate;}
}
