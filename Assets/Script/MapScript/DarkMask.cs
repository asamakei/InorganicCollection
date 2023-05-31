using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkMask : MonoBehaviour{
    GameObject player;
    Vector3 pos;
    void Start(){
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update(){
        pos = player.transform.position+new Vector3(0,0,-1);
        if(pos.y > 30 || pos.x < -47) transform.position = pos;
    }
}
