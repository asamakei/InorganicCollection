using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fog : MonoBehaviour{
    Vector3 pos,aim;
    Transform trans;
    int speed = 1000;
    void Start(){
        trans = transform;
        pos = trans.position;
        aim = new Vector3(20, -15, 0);
        StartCoroutine(scroll());
    }
    IEnumerator scroll() {
        while (true) {
            for (int i = 0; i < speed; i++) {
                trans.position = pos + aim * ((float)i / speed);
                yield return new WaitForSeconds(1 / 60f);
            }
        }
    }
}
