using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Ending : MonoBehaviour{
    GameObject TitleObj;
    GameObject[] TextObj;
    RectTransform TitleTra;
    RectTransform[] TextTra;
    Text TitleTex;
    Text[] TextTex;
    AudioSource audioS;
    Scene title;
    public FlagDatas flagDB;
    float vol;
    string[,] tex = { {"無機コレクト"," ","スタッフクレジット","","","","","","","","","","" },
                      {"企画・制作","慶","","","","","","","","","","","" },
                      {"BGM","PeriTune","夢にみた緑","ユーフルカ","龍的交響楽","にーおんのBGM素材","in cremonica mono+","MoonWind","","","","","" },
                      {"SE","効果音ラボ","くらげ工匠","音人","魔王魂","Oculus Audio Pack 01","小森 平","","","","","","" },
                      {"マップチップ","ねくら","Loose Leaf","てびきのてびき。","","","","","","","","","" },
                      {"キャラチップ","誰そ彼亭","藤田工房","","","","","","","","","","" },
                      {"エフェクト画像","ぴぽや","シュガーポット","panop","茫然の流もの喫茶","コミュ将の素材倉庫","Material Forward","","","","","","" },
                      {"モンスター画像","慶","","","","","","","","","","","" },
                      {"ボーキサイト原案","髑髏林　禿鷹丸","","","","","","","","","","","" },
                      {"背景画像","ぴぽや","白螺小屋","よく訓練された素材屋","","","","","","","","","" },
                      {"システム画像","ぴぽや","化け猫缶","MORGUE","慶","","","","","","","","" },
                      {"その他色々","慶","","","","","","","","","","","" },
                      {"デバッグ","斗ライ","ゆ","慶","","","","","","","","","" },
                      {"制作ツール","Unity","","","","","","","","","","","" },
    };
    int[] flag = { -1,-1,-1,-1,1
            ,-1,-1,2,-1,-1
            ,1,-1,-1,-1,-1
            ,-1,-1,-1,-1,-1
            ,-1,-1,-1,-1,-1
            ,-1,1}; 
    void Start(){
        TextObj = new GameObject[12];
        TextTra = new RectTransform[12];
        TextTex = new Text[12];

        TitleObj = GameObject.Find("Title");
        TitleTra = TitleObj.GetComponent<RectTransform>();
        TitleTex = TitleObj.GetComponent<Text>();
        TitleTex.text = "";
        TitleTex.color = new Color(0.5f,0.7f,1,0);
        for (int i = 0; i < 12; i++) {
            TextObj[i] = TitleObj.transform.Find("Text" + (i + 1).ToString()).gameObject;
            TextTra[i] = TextObj[i].GetComponent<RectTransform>();
            TextTex[i] = TextObj[i].GetComponent<Text>();
            TextTex[i].text = "";
            TextTex[i].color = new Color(1, 1, 1, 0);
        }
        StartCoroutine(credit());

    }
    IEnumerator credit() {
        
        audioS = GetComponent<AudioSource>();
        audioS.Play();

        yield return new WaitForSeconds(3);
        yield return Show(0, 300, 1f);
        yield return Show(0, 60, 0f);
        Debug.Log(tex.GetLength(0));
        for (int i = 1; i < tex.GetLength(0); i++) {
            yield return Show(i, 200, 1f);
            yield return Show(i, 60, 0f);
        }

        vol = audioS.volume;
        for (int i = 1; i <= 180; i++) {
            audioS.volume = vol *(180f-i) / 180f;
            yield return new WaitForSeconds(1/60f);
        }
        yield return new WaitForSeconds(3);
        for(int i=0;i<=26;i++) flagDB.Event[i] = flag[i];
        title = SceneManager.GetActiveScene();
        SysDB.bgmOff = true;
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene("Scenes/MainCamera", LoadSceneMode.Additive);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        GameObject player;
        if (scene.name == "MainCamera") {
            player = GameObject.Find("Player");
            player.transform.position = new Vector3(-10.5f, 3.5f, 0);
            player.GetComponent<PlayerMove>().playerDirection(new Vector2(1, 0));
            SceneManager.LoadScene("Scenes/Village1", LoadSceneMode.Additive);
        } else if (scene.name == "Village1") {
            SceneManager.SetActiveScene(scene);
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.UnloadSceneAsync(title);
        }
    }

    IEnumerator Show(int stringID,int frame,float col) {
        TitleTex.text = tex[stringID, 0];
        StartCoroutine(textShow(TitleTex, 20, col));
        yield return new WaitForSeconds(1);
        for (int i = 1; i <12;i++) {
            TextTex[i - 1].text = tex[stringID, i];
            if (tex[stringID, i] == "") break;
            StartCoroutine(textShow(TextTex[i-1],20,col));
            yield return new WaitForSeconds(20/60f);
        }
        yield return new WaitForSeconds(frame/60f);
    }
    IEnumerator textShow(Text obj,int frame,float col) {
        float firstC = obj.color.a;
        float r, g, b;
        r = obj.color.r;
        g = obj.color.g;
        b = obj.color.b;
        for (int i = 0; i < frame; i++) {
            obj.color = new Color(r,g,b, firstC + (col - firstC) * i /frame);
            yield return new WaitForSeconds(1 / 60f);
        }
        obj.color = new Color(r,g,b,col);
    }
}
