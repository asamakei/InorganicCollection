using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackSky : MonoBehaviour{
    Transform cameraM,player;

    void Start() {
        cameraM = GameObject.Find("Main Camera").transform;
        player = GameObject.Find("Player").transform;
        transform.position = cameraM.position + new Vector3(0, 6, 0);
        cameraM.GetComponent<MainCamera>().skySet(this.transform,1,1,0);
    }
}
