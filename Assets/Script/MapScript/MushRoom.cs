using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushRoom : MonoBehaviour {

    BoxCollider2D box;
    CircleCollider2D cir;
    SpriteRenderer sp;
    Transform tra;
    CharaMove cha;
    EventExist flag;
    Hasu hasu;
    GameObject shade, wave;

    [SerializeField]
    Sprite mush=null, wood=null;
    [SerializeField]
    int type=0;

    void Start(){
        if (type == 0) {
            box = GetComponent<BoxCollider2D>();
            sp = GetComponent<SpriteRenderer>();
            cha = GetComponent<CharaMove>();
            tra = transform;
            flag = GetComponent<EventExist>();
            if (flag.flag.Event[flag.EventFlagNumber] > 0) ToMushroom();
            else ToWood();
        }else if (type == 1) {
            hasu = GetComponent<Hasu>();
            sp = GetComponent<SpriteRenderer>();
            shade = transform.Find("shade").gameObject;
            wave = transform.Find("wave").gameObject;
            flag = GetComponent<EventExist>();
            if (flag.flag.Event[flag.EventFlagNumber] > 0) ToHasu();
            else ToNotHasu();
        }
    }
    public void ToMushroom() {
        cha.EventTrigger = EaseType.OnTrigger;
        box.enabled = false;
        sp.sortingOrder = 9;
        sp.sprite = mush;
    }
    public void ToWood() {
        cha.EventTrigger = EaseType.PressKey;
        box.enabled = true;
        sp.sortingOrder = 10;
        sp.sprite = wood;
    }
    public void ToHasu() {
        hasu.enabled = true;
        sp.sprite = mush;
        shade.SetActive(true);
        wave.SetActive(true);
    }
    public void ToNotHasu() {
        hasu.enabled = false;
        sp.sprite = wood;
        shade.SetActive(false);
        wave.SetActive(false);
    }
}
