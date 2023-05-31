using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Encount : MonoBehaviour{
    [SerializeField]
    int SE=-1;
    [SerializeField]
    int BGM =-1;
    [SerializeField]
    int Back =-1;
    [SerializeField]
    int MapNum = -1;
    Transform player;
    BattleDB batDB;
    int backChange;
    GameController gameCon;
    private void Start() {
        player = GameObject.Find("Player").transform;
        batDB = GameObject.Find("Main Camera").GetComponent<MainCamera>().batDB;
    }

    public void main() {
        //int count,limit=0;
        int rand;
        GameObject obj;
        obj = GameObject.Find("GameController");
        gameCon = obj.GetComponent<GameController>();
        rand = enemy(MapNum);
        if (rand >= 0) {
            if (obj != null) {
                gameCon = obj.GetComponent<GameController>();
                if (SE >= 0) gameCon.SE(SE);
                SysDB.SE = SE;
                SysDB.BGM = BGM;
                if (backChange >= 0) SysDB.Back = backChange;
                else SysDB.Back = Back;
                SysDB.Enemy = rand;
                SysDB.Battle = 0;
                gameCon.sceneLoad("Scenes/Battle");
            }
        }
    }
    int enemy(int num) {
        float x, y;
        x = player.position.x;
        y = player.position.y;
        backChange = -1;
        if (num == 0) {//メンデレ水源
            if (y > 25 && y > 0.3639f * x + 49f && x < 33) return enemyNum(0);//チュートリアルエリア
            else if (y > 25 || y > 8 && x > -40) return enemyNum(1);//↑以降、橋まで
            else if(y>-31)return enemyNum(2);//↑それ以降
        } else if (num == 1) {//オストワルトの森
            if (y < -42 && x < -50) return enemyNum(17);//ボーキサイト
            else if (y > 0.2309f * x -4.5f) return enemyNum(3);//前半
            else return enemyNum(4);//後半
        } else if (num == 2) {//ボッシュ洞窟
            if (y > 36 && x<-47) return enemyNum(5);//前半
            else if (y < 34 && x < -64 && (x<-98 || y>-20)) return enemyNum(6);//後半
        } else if (num == 3) {//ソルベ雪山
            if (x > -70) return enemyNum(7);//前半
            else if (y < 40) return enemyNum(8);//洞窟
            else return enemyNum(9);//後半
        } else if (num == 4) {//ヘス砂漠
            if (y < 53) return enemyNum(10);//前半
            else if(y<100)return enemyNum(11);//後半
        } else if (num == 5) {//シャルル溶岩地帯
            if (y >41 && ((y<103 && x>=-69)||(y<83 && x<-69))) return enemyNum(12);//前半
            else if(y>41) return enemyNum(13);//後半
        } else if (num == 6) {//メンデレ村地下
            if (x < -29 && y < -29) return enemyNum(18);//水銀
            return enemyNum(14);//全域
        } else if (num == 7) {//アボガド遺跡
            if(y>6 && y<88)return enemyNum(15);//前半
            else if (y >= 88) return enemyNum(16);//後半
        }
        return -1;
    }
    int enemyNum(int num) {
        int[] ene, enePar, numPar;
        ene = new int[1] { -1 };
        enePar = new int[1] { -1 };
        numPar = new int[1] { -1 };

        if (num == 0) {
            ene = new int[3] { 17, 18, 19 };
            enePar = new int[3] { 1, 1, 1 };
            numPar = new int[4] { 1, 0, 0, 0 };
        } else if(num == 1) {
            ene = new int[4] { 17, 18, 19 ,20};
            enePar = new int[4] { 1, 1, 1 ,1 };
            numPar = new int[4] { 2, 8, 5, 0 };
        } else if(num == 2) {
            ene = new int[5] { 17, 18, 19, 20 ,21 };
            enePar = new int[5] { 1, 1, 1, 1, 1 };
            numPar = new int[4] { 2, 8, 5, 0 };
        } else if (num == 3) {
            ene = new int[4] { 22, 23, 24, 64 };
            enePar = new int[4] { 1, 1, 1, 1 };
            numPar = new int[4] { 0, 4, 4, 1 };
        } else if (num == 4) {
            ene = new int[6] { 22, 23, 24, 25, 26, 64 };
            enePar = new int[6] { 5, 5, 5, 5, 1, 5 };
            numPar = new int[4] { 0, 3, 4, 1 };
        } else if (num == 5) {
            ene = new int[4] { 27, 28, 29, 63 };
            enePar = new int[4] { 1, 1, 1, 1 };
            numPar = new int[4] { 0, 3, 4, 1 };
        } else if (num == 6) {
            ene = new int[6] { 27, 28, 29, 30, 31, 63 };
            enePar = new int[6] { 5, 5, 5, 5, 1, 5 };
            numPar = new int[4] { 0, 3, 4, 1 };
        } else if (num == 7) {
            ene = new int[4] { 32, 33, 34, 62 };
            enePar = new int[4] { 5, 5, 5, 5 };
            numPar = new int[4] { 0, 2, 4, 2 };
        } else if (num == 8) {
            ene = new int[4] { 32, 33, 34, 35};
            enePar = new int[4] { 5, 5, 5, 3 };
            numPar = new int[4] { 0, 2, 4, 2 };
            backChange = 10;
        } else if (num == 9) {
            ene = new int[5] { 32, 33, 34, 36, 62 };
            enePar = new int[5] { 5, 5, 5, 3, 5 };
            numPar = new int[4] { 0, 1, 4, 3 };
        } else if (num == 10) {
            ene = new int[4] { 37, 38, 39, 65 };
            enePar = new int[4] { 5, 5, 5, 5 };
            numPar = new int[4] { 0, 1, 4, 4 };
        } else if (num == 11) {
            ene = new int[5] { 37, 38, 39, 40, 41 };
            enePar = new int[5] { 5, 5, 5, 5, 1 };
            numPar = new int[4] { 0, 1, 4, 4 };
        } else if (num == 12) {
            ene = new int[3] { 42, 43, 44 };
            enePar = new int[3] { 5, 5, 5 };
            numPar = new int[4] { 0, 1, 4, 4 };
        } else if (num == 13) {
            ene = new int[5] { 42, 43, 44, 45, 46 };
            enePar = new int[5] { 5, 5, 5, 5, 1 };
            numPar = new int[4] { 0, 1, 4, 4 };
        } else if (num == 14) {
            ene = new int[4] { 47, 48, 49, 50 };
            enePar = new int[4] { 5, 5, 5, 5 };
            numPar = new int[4] { 0, 1, 4, 4 };
        } else if (num == 15) {
            ene = new int[3] { 51, 52, 53 };
            enePar = new int[3] { 5, 5, 5 };
            numPar = new int[4] { 0, 1, 4, 4 };
        } else if (num == 16) {
            ene = new int[5] { 51, 52, 53, 54, 55 };
            enePar = new int[5] { 5, 5, 5, 5, 1 };
            numPar = new int[4] { 0, 1, 4, 4 };
        } else if (num == 17) {//ボーキサイト
            ene = new int[1] { 58 };
            enePar = new int[1] { 1 };
            numPar = new int[4] { 1, 1, 0, 0 };
            if (gameCon.flagData.Event[26] == 0) return -1;
        } else if (num == 18) {//水銀
            ene = new int[1] { 61 };
            enePar = new int[1] { 1 };
            numPar = new int[4] { 0, 1, 1, 0 };
            //if (gameCon.flagData.Event[26] == 0) return -1;
        }
        return randomNum(ene,enePar,numPar);
    }
    int randomNum(int[] ene,int[] enePar,int[] numPar) {
        int length = ene.Length,randomNum;
        int eneCount=0;
        int[] enemy = { -1,-1,-1,-1};
        Vector4 Group;

        if (length <= 0 || ene[0] == -1) return -1;
        for (int i = 1; i < 4; i++) numPar[i] += numPar[i - 1];
        for (int i = 1; i < length; i++) enePar[i] += enePar[i - 1];

        randomNum = SysDB.randomInt(1, numPar[3]);
        for (int i = 0; i < 4; i++) {
            if (randomNum <= numPar[i]) {
                eneCount = i+1;
                break;
            }
        }

        for (int j = 0; j < eneCount; j++) {
            randomNum = SysDB.randomInt(1, enePar[length - 1]);
            for (int i = 0; i < length; i++) {
                if (randomNum <= enePar[i]) {
                    enemy[j] = ene[i];
                    break;
                }
            }
        }
        if (eneCount == 1)Group = new Vector4(-1,enemy[0],-1,-1);
        else if(eneCount == 2)Group = new Vector4(-1, enemy[0], enemy[1], -1);
        else if(eneCount == 3)Group = new Vector4(enemy[0], enemy[1], enemy[2], -1);
        else if(eneCount == 4)Group = new Vector4(enemy[0], enemy[1], enemy[2], enemy[3]);
        else return -1;
        batDB.EnemyGroup[14] = Group;
        return 14;
    }
}
