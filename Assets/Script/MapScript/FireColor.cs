using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireColor : MonoBehaviour{
    public int ColorNum=0;
    [SerializeField]
    int FlagNum = 37;
    [SerializeField]
    FlagDatas flag = null;
    [SerializeField]
    bool lightFlag=false;
    [SerializeField]
    Sprite[] fire = new Sprite[4];
    GameObject lightObj;
    SpriteRenderer sp;
    public bool OnOff = false;

    /*void Start(){
        if(lightFlag)lightObj = transform.Find("light").gameObject;
        sp = GetComponent<SpriteRenderer>();
        if (((1 << ColorNum) & flag.Event[FlagNum]) == 0) Off();
        else On();
    }*/
    private void OnEnable() {
        if (lightFlag) lightObj = transform.Find("light").gameObject;
        sp = GetComponent<SpriteRenderer>();
        if (((1 << ColorNum) & flag.Event[FlagNum]) == 0) Off();
        else { OnOff = false; On(); }
    }
    private void OnDisable() {
        StopAllCoroutines();
    }

    public void On() {
        if (lightFlag && lightObj==null) lightObj = transform.Find("light").gameObject;
        flag.Event[FlagNum] |= 1 << ColorNum;
        if (lightFlag) lightObj.SetActive(true);
        if(!OnOff)StartCoroutine(anim());
        OnOff = true;
    }
    void Off() {
        flag.Event[FlagNum] |= 1 << ColorNum;
        flag.Event[FlagNum] -= 1 << ColorNum;
        sp.sprite = null;
        if (lightFlag) lightObj.SetActive(false);
        OnOff = false;
    }
    IEnumerator anim() {
        int animNum = SysDB.randomInt(0,3);
        OnOff = true;
        yield return new WaitForSeconds(SysDB.randomInt(0, 9) / 60f);
        while (true) {
            sp.sprite = fire[animNum];
            animNum = (animNum + 1) % 4;
            yield return new WaitForSeconds(10 / 60f);
            if (!OnOff) yield break;
        }
    }

}
