using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureBox : MonoBehaviour{
    public int FlagNumber = -1;
    public Sprite[] boxSprite = new Sprite[4];
    public string[] ItemName;
    public int[] ItemCount;

    bool boxflag = false;//false:開けてない,true:開けてる
    FlagDatas flagDB;
    SpriteRenderer spriteRen;
    GameController gameCon;
    void Start(){
        spriteRen = GetComponent<SpriteRenderer>();
        flagDB = GetComponent<EventExist>().flag;
        boxflag = FlagNumber>=0 && flagDB.Treasure[FlagNumber] >= 1;
        if (boxflag) spriteRen.sprite = boxSprite[3];
        else spriteRen.sprite = boxSprite[0];
        gameCon = GameObject.Find("GameController").GetComponent<GameController>();
    }
    public void ItemRegister(ref string[] commandArray,int k) {
        if (!boxflag) {
            Array.Resize(ref commandArray, k + 1 + ItemCount.Length * 2);
            for (int j = 0; j < ItemCount.Length * 2; j += 2) {
                commandArray[k + 1 + j] = "Item:" + ItemName[j] + ":" + ItemCount[j].ToString() + ":1";
                if (ItemName[j] == "お金")commandArray[k + 1 + j + 1] = "Info:" + ItemCount[j].ToString() + "Gを手に入れた！";
                else commandArray[k + 1 + j + 1] = "Info:" + ItemName[j] + "を" + ItemCount[j].ToString() + "個手に入れた！";
            }
        }
    }
    public IEnumerator Open() {
        if (!boxflag) {
            gameCon.SE(23);
            for (int i = 0; i < 4; i++) {
                spriteRen.sprite = boxSprite[i];
                yield return new WaitForSeconds(5/60f);
            }
            boxflag = true;
            flagDB.Treasure[FlagNumber] = 1;
        }

        yield return null;
    }
}
