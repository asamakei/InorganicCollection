using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NetBattle : MonoBehaviourPunCallbacks, IPunObservable {

    public AllyDatas allyDB;
    public BattleDB BatDB;
    public EnemyDatas eneDB;
    NetData netD;
    int me,you;

    private void Awake() {
        me = PhotonNetwork.IsMasterClient ? 1 : 0;
        you = PhotonNetwork.IsMasterClient ? 0 : 1;
        netD = GameObject.Find("NetData").GetComponent<NetData>();
    }

    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        //netD.mode==4:戦闘準備完了フラグを送信 → mode=0
        //netD.mode==0:プレイヤー情報送信(初回のみ) → mode=1
        //netD.mode==1:なにもしない(命令待機中) → mode=2
        //netD.mode==2:命令完了フラグを送信(命令決定) → mode=3
        //netD.mode==3:命令情報・乱数送信 → mode=1
        //互いに受け取りが完了したらmodeの値を変更する
        //その条件は　!(netD.noNeedSend[mode] && netD.sendNoNeed[mode])

        if (stream.IsWriting) {////////////////////////送信///////////////////////////////
            stream.SendNext(netD.mode);
            Debug.Log(netD.mode+"で送信");
            if(netD.mode == 4) {//準備完了フラグ送信
                if (!(netD.noNeedSend[4] && netD.sendNoNeed[4])) {
                    if (netD.getEnemy[4]) netD.sendNoNeed[4] = true;
                    stream.SendNext(netD.getEnemy[4]);
                    stream.SendNext(netD.ready[me]);
                }
            } else if (netD.mode == 0) {//モンスター情報・乱数送信
                if (!(netD.noNeedSend[0] && netD.sendNoNeed[0])) {
                    if (netD.getEnemy[0]) netD.sendNoNeed[0] = true;
                    stream.SendNext(netD.getEnemy[0]);
                    stream.SendNext(myDB.playerName);
                    for (int i = 0; i < 4; i++) {
                        stream.SendNext(myDB.party[i] >= 0);
                        if (myDB.party[i] >= 0) {
                            AllyData ally = allyDB.AllyDataList[myDB.party[i]];
                            stream.SendNext(ally.Name);
                            stream.SendNext(ally.ID);
                            stream.SendNext(ally.Level);
                            stream.SendNext(ally.HP);
                            stream.SendNext(ally.SP);
                            stream.SendNext(ally.Attack);
                            stream.SendNext(ally.Defence);
                            stream.SendNext(ally.Magic);
                            stream.SendNext(ally.MagicDef);
                            stream.SendNext(ally.Speed);
                            stream.SendNext(ally.Equip);
                            stream.SendNext(ally.Skill.Length);
                            for (int j = 0; j < ally.Skill.Length; j++) stream.SendNext(ally.Skill[j]);
                            stream.SendNext(ally.SkillUse.Length);
                            for (int j = 0; j < ally.SkillUse.Length; j++) stream.SendNext(ally.SkillUse[j]);
                            stream.SendNext(ally.Character.Length);
                            for (int j = 0; j < ally.Character.Length; j++) stream.SendNext(ally.Character[j]);
                        }
                    }
                    for (int i = 0; i < 100; i++) {
                        stream.SendNext(netD.rand[me, i]);
                    }
                }
            }else if (netD.mode == 2) {//命令完了フラグ送信
                if (!(netD.noNeedSend[2] && netD.sendNoNeed[2])) {
                    if (netD.getEnemy[2]) netD.sendNoNeed[2] = true;
                    stream.SendNext(netD.getEnemy[2]);
                    stream.SendNext(netD.commandFlag[me]);
                }
            }else if(netD.mode == 3) {//命令情報・乱数送信
                if (!(netD.noNeedSend[3] && netD.sendNoNeed[3])) {
                    if (netD.getEnemy[3]) netD.sendNoNeed[3] = true;
                    stream.SendNext(netD.getEnemy[3]);
                    stream.SendNext(netD.escape[me]);
                    if (!netD.escape[me]) {
                        for (int i = 0; i < 4; i++) {
                            stream.SendNext(netD.command[me, i, 0]);
                            stream.SendNext(netD.command[me, i, 1]);
                        }
                        for (int i = 0; i < 100; i++) {
                            stream.SendNext(netD.rand[me, i]);
                        }
                    }
                }
            }
        } else {////////////////////////////////受信////////////////////////////////////
            int mode;
            mode = (int)stream.ReceiveNext();
            if(mode == 4) {//準備完了フラグ受信
                netD.noNeedSend[4] = (bool)stream.ReceiveNext();
                netD.getEnemy[4] = true;
                netD.ready[you] = (bool)stream.ReceiveNext();
            } else if (mode == 0) {//モンスター情報・乱数受信
                netD.noNeedSend[0] = (bool)stream.ReceiveNext();
                netD.getEnemy[0] = true;

                int loadCount;
                bool partyNum;
                netD.playerName[you] = (string)stream.ReceiveNext();
                for (int i = 0; i < 4; i++) {
                    AllyData ally = new AllyData();
                    partyNum = (bool)stream.ReceiveNext();
                    if (partyNum) {
                        ally.Name = (string)stream.ReceiveNext();
                        ally.ID = (int)stream.ReceiveNext();
                        ally.Level = (int)stream.ReceiveNext();
                        ally.HP = (int)stream.ReceiveNext();
                        ally.SP = (int)stream.ReceiveNext();
                        ally.Attack = (int)stream.ReceiveNext();
                        ally.Defence = (int)stream.ReceiveNext();
                        ally.Magic = (int)stream.ReceiveNext();
                        ally.MagicDef = (int)stream.ReceiveNext();
                        ally.Speed = (int)stream.ReceiveNext();
                        ally.Equip = (int)stream.ReceiveNext();
                        loadCount = (int)stream.ReceiveNext();
                        Array.Resize(ref ally.Skill, loadCount);
                        for (int j = 0; j < loadCount; j++) ally.Skill[j] = (int)stream.ReceiveNext();
                        loadCount = (int)stream.ReceiveNext();
                        Array.Resize(ref ally.SkillUse, loadCount);
                        for (int j = 0; j < loadCount; j++) ally.SkillUse[j] = (int)stream.ReceiveNext();
                        loadCount = (int)stream.ReceiveNext();
                        Array.Resize(ref ally.Character, loadCount);
                        for (int j = 0; j < loadCount; j++) ally.Character[j] = (int)stream.ReceiveNext();
                        BatDB.Group[SysDB.Enemy, i] = 70 + i;
                        eneDB.EnemyDataList[70 + i].Name = ally.Name;
                    } else {
                        ally = null;
                        BatDB.Group[SysDB.Enemy, i] = -1;
                    }
                    netD.monster[i] = ally;
                }
                for (int i = 0; i < 100; i++) {
                    netD.rand[you, i] = (int)stream.ReceiveNext();
                }
            } else if (mode == 2) {//命令完了フラグ受信
                netD.noNeedSend[2] = (bool)stream.ReceiveNext();
                netD.getEnemy[2] = true;
                netD.commandFlag[you] = (bool)stream.ReceiveNext();
            } else if (mode == 3) {//命令情報・乱数受信
                netD.noNeedSend[3] = (bool)stream.ReceiveNext();
                netD.getEnemy[3] = true;
                netD.escape[you] = (bool)stream.ReceiveNext();
                if (!netD.escape[you]) {
                    for (int i = 0; i < 4; i++) {
                        netD.command[you, i, 0] = (int)stream.ReceiveNext();
                        netD.command[you, i, 1] = (int)stream.ReceiveNext();
                    }
                    for (int i = 0; i < 100; i++) {
                        netD.rand[you, i] = (int)stream.ReceiveNext();
                    }
                }
            }
        }
    }
}
