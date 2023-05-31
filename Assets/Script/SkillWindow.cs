using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillWindow : MonoBehaviour{

    GameObject nameText;
    Image color1;
    Text color2;
    IEnumerator coroutine,preCoroutine;

    void Start(){
        nameText = transform.Find("Text").gameObject;
    }

    public void main(string skillName) {
        coroutine = colorChange(skillName);
        if(preCoroutine!=null)StopCoroutine(preCoroutine);
        StartCoroutine(coroutine);
        preCoroutine = coroutine;
    }
    IEnumerator colorChange(string skillName) {
        color1 = GetComponent<Image>();
        color2 = nameText.GetComponent<Text>();
        color2.text = skillName;

        color1.color = new Color(1, 1, 1);
        color2.color = new Color(1, 1, 1);
        yield return new WaitForSeconds(50 / 60f);
        for (int i = 1; i <= 50; i++) {
            color1.color = new Color(1, 1, 1,(50-i)/50f);
            color2.color = new Color(1, 1, 1, (50 - i) / 50f);
            yield return new WaitForSeconds(1 / 60f);
        }
    }
}
