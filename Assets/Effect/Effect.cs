using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Effect : MonoBehaviour {
    public Vector3 rotation = Vector3.zero;
    public GameObject[] effect;
    public float autoDelete = -1f;
    public GameObject child;
    private void Start() {
        transform.eulerAngles = rotation;
        if (autoDelete > 0) StartCoroutine(vanish(autoDelete));
    }
    void delete() {
        Destroy(this.gameObject);
    }

    IEnumerator meteorFire() {
        GameObject obj = GameObject.Find("movie");
        GameObject fire;
        for (int i = 0; i < 10; i++) {
            fire = Instantiate(effect[0], transform.position+new Vector3(10,0,0), Quaternion.identity, obj.transform);
            fire.transform.localScale *= 4;
            fire.transform.eulerAngles = new Vector3(0,0,SysDB.randomInt(0,359));
            for(int j=0;j<10;j++)yield return null;
        }
    }
    IEnumerator vanish(float time) {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        yield return new WaitForSeconds(time);
        for(int i = 0; i < 60; i++) {
            sr.color = new Vector4(1,1,1,(60-i)/60f);
            yield return new WaitForSeconds(1/60f);
        }
        delete();
    }
    public IEnumerator Damage(int time,int delay,int mode) {
        Text sr = GetComponent<Text>();
        Text sr2 = child.GetComponent<Text>();
        Transform tr = transform;
        yield return new WaitForSeconds(delay/60f);
        for (int i = 0; i < time; i++) {
            tr.position += new Vector3(0, (time / 2-i),0);
            yield return new WaitForSeconds(1 / 60f);
        }
        yield return new WaitForSeconds(40 / 60f);
        for (int i = 0; i < 20; i++) {
            sr.color = new Vector4(0.3f, 0.3f, 1, (20 - i) / 20f);
            sr2.color = new Vector4(0.3f, 0.3f, 1, (20 - i) / 20f);
            yield return new WaitForSeconds(1 / 60f);
        }
        delete();
    }
}
