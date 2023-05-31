using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour{
    public int AttackDelay;
    public GameObject AttackEffect;
    public AudioClip AttackSE;

    void attackFinish() {
        GetComponent<Animator>().SetBool("attack", false);
        SysDB.animationFlag = false;
    }

    public void death() {
        StartCoroutine(vanish());
    }
    IEnumerator vanish() {
        SpriteRenderer color1, color2;
        color1 = GetComponent<SpriteRenderer>();
        color2 = transform.Find("shade").GetComponent<SpriteRenderer>();

        yield return new WaitForSeconds(20 / 60f);
        for (int i = 1; i <= 30; i++) {
            color1.color = new Color(1, 1, 1, (30 - i) / 30f);
            color2.color = new Color(1, 1, 1, (30 - i) / 30f);
            yield return new WaitForSeconds(1 / 60f);
        }
    }
    public void life() {
        StartCoroutine(appear());
    }
    IEnumerator appear() {
        SpriteRenderer color1, color2;
        color1 = GetComponent<SpriteRenderer>();
        color2 = transform.Find("shade").GetComponent<SpriteRenderer>();

        yield return new WaitForSeconds(20 / 60f);
        for (int i = 1; i <= 30; i++) {
            color1.color = new Color(1, 1, 1, i / 30f);
            color2.color = new Color(1, 1, 1, i / 30f);
            yield return new WaitForSeconds(1 / 60f);
        }
    }
}
