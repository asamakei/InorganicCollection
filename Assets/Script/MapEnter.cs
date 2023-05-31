using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapEnter : MonoBehaviour{
    public int BGM;
    public int special = 0;
    GameObject obj;
    AudioSource audioS;
    GameController gameCon;
    Vector3 playerPos;
    bool rePlay;
    void Start(){
        SysDB.sceneName = SceneManager.GetActiveScene().name;
        obj = GameObject.Find("GameController");
        if (obj != null) obj.GetComponent<GameController>().mapName();
        audioS = GameObject.Find("Main Camera").GetComponent<AudioSource>();
        gameCon = GameObject.Find("GameController").GetComponent<GameController>();
        if (!SysDB.bgmOff) bgm();
    }
    public void bgm() {
        playerPos = GameObject.Find("Player").transform.position;
        if (special == 1) {
            rePlay = false;
            if (playerPos.y > 30 || playerPos.x < -47) {
                if (audioS.clip != gameCon.BGM[6]) {
                    audioS.clip = gameCon.BGM[6];
                    rePlay = true;
                }
            } else {
                if (audioS.clip != gameCon.BGM[5]) {
                    audioS.clip = gameCon.BGM[5];
                    rePlay = true;
                }
            }
            if (rePlay) audioS.Play();
        } else if (special == 2) {
            rePlay = false;
            if (playerPos.y > -32) {
                if (audioS.clip != gameCon.BGM[1]) {
                    audioS.clip = gameCon.BGM[1];
                    rePlay = true;
                }
            } else {
                if (audioS.clip != gameCon.BGM[6]) {
                    audioS.clip = gameCon.BGM[6];
                    rePlay = true;
                }
            }
            if(rePlay)audioS.Play();
        } else if (special == 3) {
            rePlay = false;
            if (playerPos.y < 100) {
                if (audioS.clip != gameCon.BGM[9]) {
                    audioS.clip = gameCon.BGM[9];
                    rePlay = true;
                }
            } else {
                if (audioS.clip != gameCon.BGM[6]) {
                    audioS.clip = gameCon.BGM[6];
                    rePlay = true;
                }
            }
            if (rePlay) audioS.Play();
        } else {
            if (BGM >= 0) {
                if (audioS.clip != gameCon.BGM[BGM]) {
                    audioS.clip = gameCon.BGM[BGM];
                    audioS.Play();
                }
            } else {
                audioS.clip = null;
                audioS.Stop();
            }
        }
    }
}
