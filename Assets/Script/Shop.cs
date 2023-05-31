using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Shop : MonoBehaviour{

    int buttonNum,buttonS=-1,buttonF=-1;
    int page=0,shopCount=0;
    GameObject[] Button;
    Rect[] buttonRe;
    Image[] buttonIm;
    Text[] text,moneyText;
    Text detailText,possessText,money;
    Text buyCount, sumMoney,buyText;
    Vector2 mousePos;
    RectTransform cursor;
    GameObject gameCon,buyWin;
    bool cursorFlag;
    bool getmouse;
    bool shopType;
    public ItemDatas ShopDB,ItemDB;

    void Start(){
        shopType = ShopDB.ItemDataList[0].Name == "";
        Button = new GameObject[15];
        buttonRe = new Rect[Button.Length];
        text = new Text[6];
        moneyText = new Text[6];
        buttonIm = new Image[9];
        for (int i = 0; i <= 5; i++) {
            Button[i] = GameObject.Find("goods" + i.ToString());
            text[i] = Button[i].GetComponent<Text>();
            moneyText[i] = Button[i].transform.Find("money").GetComponent<Text>();
        }
        buyWin = GameObject.Find("ItemBuy");
        buyCount = buyWin.transform.Find("Count").GetComponent<Text>();
        sumMoney = buyWin.transform.Find("sumMoney").GetComponent<Text>();

        Button[6] = GameObject.Find("ShopExit");
        Button[7] = GameObject.Find("ShopRight");
        Button[8] = GameObject.Find("ShopLeft");
        Button[9] = GameObject.Find("Buy");
        Button[10] = GameObject.Find("NotBuy");
        Button[11] = GameObject.Find("Up1");
        Button[12] = GameObject.Find("Up10");
        Button[13] = GameObject.Find("Down1");
        Button[14] = GameObject.Find("Down10");

        buyText = Button[9].transform.Find("Text").GetComponent<Text>();
        if (shopType) buyText.text = "売る";
        for(int i = 6; i <= 14; i++)buttonIm[i-6] = Button[i].GetComponent<Image>();
        detailText = GameObject.Find("ItemDetail").transform.Find("Text").GetComponent<Text>();
        possessText = GameObject.Find("ItemDetail").transform.Find("Possess").GetComponent<Text>();
        money = GameObject.Find("PossessMoney").transform.Find("Possess").GetComponent<Text>();
        cursor = GameObject.Find("ShopCursor").GetComponent<RectTransform>();
        for (int i = 0; i < Button.Length; i++) {
            buttonRe[i] = buttonRect(Button[i]);
        }
        GameObject.Find("ShopName").transform.Find("Text").GetComponent<Text>().text = ShopDB.ShopName;
        gameCon = GameObject.Find("GameController");
        detailText.text = "";
        possessText.text = "";

        for(int i = 0; i < ShopDB.ItemDataList.Count; i++) {
            if (ShopDB.ItemDataList[i].Name == "") break;
            else shopCount++;
        }
        gameCon.GetComponent<GameController>().SE(2);
        buyWin.SetActive(false);
        StartCoroutine(main());
    }
    IEnumerator main() {
        if (shopCount <= 6) {
            Button[7].SetActive(false);
            Button[8].SetActive(false);
        } else {
            Button[7].SetActive(true);
            Button[8].SetActive(true);
        }
        if (shopType) {
            shopReset();
            if (shopCount <= 6) {
                Button[7].SetActive(false);
                Button[8].SetActive(false);
            } else {
                Button[7].SetActive(true);
                Button[8].SetActive(true);
            }
        }
        reload();
        yield return new WaitForSeconds(10 / 60f);
        while (true) {
            yield return StartCoroutine(ButtonPush(0,8));
            if ((buttonNum == 7 || buttonNum == 8) && shopCount <= 6) continue;
            if(buttonNum>=0 && buttonNum <= 5) {//アイテム
                gameCon.GetComponent<GameController>().SE(4);
                yield return StartCoroutine(buy(page * 6 + buttonNum));
            } else if (buttonNum == 6) {//閉じる
                gameCon.GetComponent<GameController>().SE(3);
                SysDB.shopFlag = false;
                SysDB.menueFlag = false;
                SceneManager.SetActiveScene(SceneManager.GetSceneByName(SysDB.sceneName));
                SceneManager.UnloadSceneAsync("Scenes/Shop");
                yield break;
            } else if(buttonNum==7|| buttonNum == 8) {//ページ変更
                gameCon.GetComponent<GameController>().SE(5);
                page += 15 - buttonNum * 2;
                if (page * 6 >= shopCount) page = 0;
                else if (page < 0) page = (shopCount - 1) / 6;
                reload();
            }

        }
    }
    IEnumerator buy(int num) {
        int ten=0, one=1,buySum;
        ItemData item = ShopDB.ItemDataList[num];
        bool buyAble;
        buyWin.SetActive(true);
        detailText.text = item.Detail;
        possessText.text = item.Possess.ToString();

        if (shopType) {
            buyWin.transform.Find("TextName").GetComponent<Text>().text = item.Name + "  いくつ売りますか？";
            buyWin.transform.Find("Money").GetComponent<Text>().text = item.money/2 + "G";
        } else {
            buyWin.transform.Find("TextName").GetComponent<Text>().text = item.Name + "  いくつ買いますか？";
            buyWin.transform.Find("Money").GetComponent<Text>().text = item.money + "G";
        }
        while (true) {
            buySum = ten * 10 + one;
            buyCount.text = ten + " " + one;
            sumMoney.text = item.money * buySum + "G";

            if (shopType) {
                buyAble = buySum <= item.Possess;
                sumMoney.text = item.money * buySum/2 + "G";
            } else {
                buyAble = !(buySum + item.Possess >= 100 || buySum * item.money > myDB.money);
                sumMoney.text = item.money * buySum + "G";
            }

            if (buyAble) buyText.color = new Color(1, 1, 1, 1);
            else buyText.color = new Color(1, 0.4f, 0.4f, 1); 

            yield return StartCoroutine(ButtonPush(9, 14));
            if (buttonNum == 9) {//買う
                if (buyAble) {
                    gameCon.GetComponent<GameController>().SE(20);
                    if (shopType) {
                        ShopDB.ItemDataList[num].Possess -= buySum;
                        myDB.money += buySum * (item.money/2);
                        buyWin.SetActive(false);
                        detailText.text = possessText.text = "";
                        shopReset();
                    } else {
                        ShopDB.ItemDataList[num].Possess += buySum;
                        myDB.money -= buySum * item.money;
                        buyWin.SetActive(false);
                        detailText.text = possessText.text = "";
                    }
                    reload();
                    break;
                } else gameCon.GetComponent<GameController>().SE(3);
            }else if(buttonNum == 10) {//戻る
                gameCon.GetComponent<GameController>().SE(3);
                buyWin.SetActive(false);
                break;
            } else {
                gameCon.GetComponent<GameController>().SE(5);
                if (buttonNum == 11)one++;
                else if(buttonNum == 12)ten++;
                else if (buttonNum == 13)one--;
                else if (buttonNum == 14)ten--;
                one = (one + 10) % 10;
                ten = (ten + 10) % 10;
            }
        }
    }
    IEnumerator ButtonPush(int s,int f) {
        buttonS = s;
        buttonF = f;
        buttonNum = -1;
        while(s>buttonNum || buttonNum>f)yield return null;
        buttonS = -1;
        buttonF = -1;
    }
    Rect buttonRect(GameObject obj) {//矩形
        if (obj == null) return new Rect(0, 0, 0, 0);
        RectTransform rect = obj.GetComponent<RectTransform>();
        float wid, hei;
        wid = rect.rect.width * obj.transform.lossyScale.x;
        hei = rect.rect.height * obj.transform.lossyScale.y;
        return new Rect(rect.position.x - wid / 2, rect.position.y - hei / 2, wid, hei);
    }
    void reload() {
        money.text = myDB.money.ToString() + "G";
        for (int i = 0; i < 6; i++) {
            if (page * 6 + i >= shopCount) {
                text[i].text = moneyText[i].text = "";
                moneyText[i].text = "";
            } else {
                text[i].text = ShopDB.ItemDataList[page * 6 + i].Name;
                if(shopType) moneyText[i].text = ShopDB.ItemDataList[page * 6 + i].money/2 + "G";
                else moneyText[i].text = ShopDB.ItemDataList[page * 6 + i].money+"G";
            }
        }
    }
    void shopReset() {
        int itemCount=0;
        if (shopType) {
            for(int i = 0; i < ItemDB.ItemDataList.Count; i++) {
                if(ItemDB.ItemDataList[i].Possess>0 && ItemDB.ItemDataList[i].money > 0) {
                    ShopDB.ItemDataList[itemCount] = ItemDB.ItemDataList[i];
                    itemCount++;
                }
            }
            ShopDB.ItemDataList[itemCount] = new ItemData();
            ShopDB.ItemDataList[itemCount].Name = "";
        }
        shopCount = itemCount;
        if (page * 6 >= shopCount) page = 0;
        else if (page < 0) page = (shopCount - 1) / 6;
    }
    int min(int a, int b) { if (a > b) return b; else return a; }
    void Update(){
        getmouse = Input.GetMouseButton(0);
        if (buttonF>=0 && (Input.GetMouseButtonUp(0) || getmouse)) {
            mousePos = Input.mousePosition;
            cursorFlag = false;
            for (int i = buttonS; i <= buttonF; i++) {
                if (buttonRe[i].Contains(mousePos)) {
                    if (i >= 0 && i <= 5 && text[i].text == "") continue;

                    if (!getmouse) {
                        if(i>=6&&i<=14) buttonIm[i - 6].color = new Color(0.5f, 0.56f, 1, 1);
                        buttonNum = i;
                        break;
                    }
                    if (i >= 0 && i <= 5 && getmouse) {
                        cursor.position = Button[i].transform.position;
                        detailText.text = ShopDB.ItemDataList[page * 6 + i].Detail;
                        possessText.text = ShopDB.ItemDataList[page * 6 + i].Possess.ToString();
                        cursorFlag = true;
                    }
                    if (i >= 6 && i <= 14 && getmouse) buttonIm[i - 6].color = new Color(1, 1, 1, 1);
                    else if (i >= 6 && i <= 14) buttonIm[i - 6].color = new Color(0.5f, 0.56f, 1, 1);
                } else {
                    if (i >= 6 && i<=14) buttonIm[i-6].color = new Color(0.5f, 0.56f, 1, 1);               }
            }
            if (!cursorFlag) {
                cursor.position = new Vector3(-1000, 0, 0);
                if(buttonF<=8)detailText.text = possessText.text = "";
            }
        }
    }
}