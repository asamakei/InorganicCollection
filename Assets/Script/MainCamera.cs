using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour{

    private GameObject player;
    private GameObject canvas;
    private Vector3 cameraPosition;
    private Transform skyBack,playerT;
    private int sx=0, sy=0, ssg=0;
    public BattleDB batDB;

    void Start(){
        player = GameObject.Find("Player");
        playerT = player.transform;
        canvas = transform.Find("Advertisement").transform.Find("black").gameObject;
        StartCoroutine(fade());
    }
    IEnumerator fade() {
        yield return new WaitForSeconds(10 / 60f);
        canvas.SetActive(false);
    }
    public void skySet(Transform sky,int x,int y,int sg) {
        skyBack = sky;
        sx = x;sy = y;ssg = sg;
    }
    void LateUpdate() {
        cameraPosition = player.transform.position;
        cameraPosition.y -= 5;
        cameraPosition.z = this.transform.position.z;
        if (SysDB.cameraMove)transform.position = cameraPosition;
        if (skyBack!=null) {
            skyBack.position = cameraPosition + new Vector3(0, 6, 0);
        }
    }
}
