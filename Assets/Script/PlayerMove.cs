using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class PlayerMove : MonoBehaviour{

    public float speed = 0.1f;//移動速度
    public Vector2 dir = new Vector2(0,-1);//方向(テンキー)
    public Vector2 EventMove = new Vector3(0,0,0);
    public AudioClip[] SE;
    public GameObject preFoot;
    public Sprite[] footPrint;
    GameObject obj;
    Transform mapEffect;
    bool rightKey,leftKey,upKey,downKey;
    Vector2 playerVelocity;
    Vector3 screenPos;
    Vector2 mousePos;
    Vector2 pos1, pos2;
    Vector3Int posGrid;
    Animator Anim;
    bool AnimFlag=true;
    bool foot=false;
    int step=0,stepTime=15,encount=0;
    float steep;
    AudioSource audioS;
    int gridCount, tileNum;
    bool Ladder=false,PlayerLadder;
    bool Foot = false, PlayerFoot;
    bool Skate = false, PlayerSkate;
    public bool backSkate;
    public int skateDir=0;
    GameObject grid;
    Tilemap[] tile;
    RectTransform canvas;
    Camera cameraAsp;
    Encount enc;

    GameObject M_name;
    GameObject M_message;
    GameObject M_text;
    Text nameText;
    Text messageText;
    Text tutorialText;
    RectTransform nameRect;
    RectTransform messageRect;
    RectTransform textRect;

    void Start(){
        M_name = GameObject.Find("M_name");
        M_message = GameObject.Find("M_message");
        M_text = GameObject.Find("M_text");
        //cameraAsp = GameObject.Find("Main Camera").GetComponent<Camera>();
        cameraAsp = null;
        canvas = GameObject.Find("Canvas").GetComponent<RectTransform>();
        nameText = M_name.GetComponent<Text>();
        messageText = M_text.GetComponent<Text>();
        nameRect = M_name.GetComponent<RectTransform>();
        messageRect = M_message.GetComponent<RectTransform>();
        textRect = M_text.GetComponent<RectTransform>();

        mapEffect = GameObject.Find("MapEffect").transform;
        Anim = this.GetComponent<Animator>();
        audioS = this.GetComponent<AudioSource>();
        getGrid();
    }
    void getGrid() {
        grid = GameObject.Find("Grid");
        enc = grid.GetComponent<Encount>();
        if (grid != null) {
            gridCount = 0;
            tile = new Tilemap[20];
            foreach (Transform child in grid.transform) {
                tile[gridCount] = child.gameObject.GetComponent<Tilemap>();
                gridCount++;
            }
        }
    }
    private void Update() {
        if (Skate && Input.GetMouseButtonUp(0)) {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas, Input.mousePosition, cameraAsp, out mousePos);
            mousePos += new Vector2(414, 1475 / 2f);

            if (mousePos.y < 660 && mousePos.y > 407 && (mousePos - new Vector2(414, 407)).magnitude > 200) {
                backSkate = !backSkate;
            }
        }
    }
    void FixedUpdate(){
        TileBase tileName;
        int[] pre = { 0, 2, 0, 6, 4, 4, 6, 0, 2 };
        rightKey = Input.GetKey(KeyCode.RightArrow);
        leftKey = Input.GetKey(KeyCode.LeftArrow);
        upKey = Input.GetKey(KeyCode.UpArrow);
        downKey = Input.GetKey(KeyCode.DownArrow);
        playerVelocity = Vector2.zero;

        if (rightKey==true)playerVelocity.x += 1;
        if (leftKey == true) playerVelocity.x -= 1;
        if (upKey == true) playerVelocity.y += 1;
        if (downKey == true) playerVelocity.y -= 1;

        Anim.SetBool("ladder", Ladder);
        if (Input.GetMouseButton(0)) {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas, Input.mousePosition, cameraAsp, out mousePos);
            mousePos += new Vector2(414, 1475 / 2f);
            screenPos = Camera.main.ScreenToWorldPoint(mousePos);
            if (mousePos.y > 660) {
                screenPos -= transform.position;
                screenPos = mousePos - new Vector2(414,1000);
                screenPos.z = 0;
                playerVelocity.x = screenPos.x;
                playerVelocity.y = screenPos.y;
            } else if((mousePos - new Vector2(414,407)).magnitude<=200){
                screenPos = mousePos - new Vector2(414, 407);
                playerVelocity.x = screenPos.x;
                playerVelocity.y = screenPos.y;
                SysDB.moveMenue = true;
            }
        }
        playerVelocity.Normalize();
        if (!SysDB.eventFlag) {
            pos1 = pos2;
            pos2 = transform.position;
        }
        if (Skate && (pos2 - pos1).magnitude>1/32f) playerVelocity += (pos2 - pos1) * 100;
        SysDB.playerVelocity = playerVelocity;
        if (SysDB.eventFlag) playerVelocity = EventMove;

        if (playerVelocity != Vector2.zero) {
            playerVelocity *= speed / playerVelocity.magnitude;
            if (Skate)playerVelocity *= 1.5f;
            Anim.speed = speed * 1.25f;

            if (Skate) {
                if (backSkate && skateDir < 180) skateDir += 10;
                else if (!backSkate && skateDir > 0) skateDir -= 10;
                if (skateDir < 0) skateDir = 0;
                else if (skateDir > 180) skateDir = 180;
                playerDirection(Quaternion.Euler(0, 0, skateDir) * playerVelocity);
            } else playerDirection(playerVelocity); 
            dir = playerVelocity;
            AnimFlag = true;
            if(grid==null) getGrid();
            step++;
            if(SysDB.hasuFlag==false && Skate==false)encount++;

            if (SysDB.encItem >= 1) {
                SysDB.encItem--;
                if (SysDB.encItem == 0) {
                    if (SysDB.encount < 400)StartCoroutine(messageShow("Info","モンスター寄せの効果が切れた。"));
                    else if (SysDB.encount > 400)StartCoroutine(messageShow("Info", "モンスター除けの効果が切れた。"));
                    encount = 100;
                    SysDB.encount = 400;
                }

            }

             posGrid = new Vector3Int(
                        (int)Mathf.Floor(transform.position.x - grid.transform.position.x),
                        (int)Mathf.Round(transform.position.y - grid.transform.position.y-0.7f),
                        (int)Mathf.Floor(transform.position.z - grid.transform.position.z)
                        );
            tileNum = -1;
            PlayerLadder = false;//はしご
            PlayerFoot = false;//足跡
            PlayerSkate = false;//スケート
            steep = 0;//勾配
            for (int i = gridCount-1; i >=0 ;i--) {
                tileName = tile[i].GetTile(posGrid);
                if (tile[i].gameObject.GetComponent<TilemapRenderer>().sortingOrder > 10) continue;
                if (tileName != null) {
                    string[] stringArray;
                    stringArray = tileName.name.Split('_');
                    if (stringArray.Length>=2 && stringArray[1] == "mura") {
                        tileNum = stepSound(0, int.Parse(stringArray[2]), "");
                    }else if(stringArray[0] == "forest2") {
                        tileNum = stepSound(1, int.Parse(stringArray[1]), "");
                    } else if (stringArray.Length >= 2 && stringArray[1] == "town") {
                        tileNum = stepSound(2, int.Parse(stringArray[2]), "");
                    } else if (stringArray.Length >= 2 && stringArray[1] == "cave") {
                        tileNum = stepSound(3, int.Parse(stringArray[2]), "");
                    } else if (stringArray.Length >= 2 && stringArray[1] == "sand" && stringArray[0]=="m") {
                        tileNum = stepSound(4, int.Parse(stringArray[2]), "");
                    } else if (stringArray[0] == "townMack") {
                        tileNum = stepSound(5, int.Parse(stringArray[1]), "");
                    } else if (stringArray[0] == "castle") {
                        tileNum = stepSound(6, int.Parse(stringArray[1]), "");
                    } else if (stringArray[0] == "snowtown") {
                        tileNum = stepSound(7, int.Parse(stringArray[1]), "");
                    } else if (stringArray.Length >= 2 && stringArray[1] == "snow02") {
                        tileNum = stepSound(8, int.Parse(stringArray[2]), "");
                    } else if (stringArray.Length >= 2 && stringArray[1] == "icecave") {
                        tileNum = stepSound(9, int.Parse(stringArray[2]), "");
                    } else if (stringArray.Length >= 2 && stringArray[1] == "volc") {
                        tileNum = stepSound(10, int.Parse(stringArray[2]), "");
                    } else if (stringArray.Length >= 2 && stringArray[1] == "skelcave") {
                        tileNum = stepSound(11, int.Parse(stringArray[2]), "");
                    } else if (stringArray.Length >= 2 && stringArray[1] == "mori2") {
                        tileNum = stepSound(12, int.Parse(stringArray[2]), "");
                    } else if (stringArray.Length >= 2 && stringArray[1] == "sanc") {
                        tileNum = stepSound(13, int.Parse(stringArray[2]), "");
                    } else tileNum = stepSound(-1, -1, tileName.name);
                    if (tileNum != -1) break;
                }
            }
            if (Skate) stepTime = 60;
            else stepTime = 15;
            if (Skate == false && PlayerSkate == true) {
                backSkate = false;
                skateDir = 0;
                step = stepTime;
            }
            if (Skate)Anim.speed = 0.01f;
            Ladder = PlayerLadder;
            Foot = PlayerFoot;
            Skate = PlayerSkate;

            if (tileNum != -1 && step >= stepTime && SysDB.hasuFlag==false)playSE(tileNum, 0);
            if(Foot && (step>=stepTime || step == (stepTime - stepTime % 2) / 2)) {
                obj = Instantiate(preFoot, transform.position - new Vector3(0, 0.4f, 0), Quaternion.identity);
                obj.transform.SetParent(mapEffect);
                obj.GetComponent<SpriteRenderer>().sprite = footPrint[pre[Anim.GetInteger("direction")] + (foot ? 1 : 0)];
                foot = !foot;
            }
            if (Skate) {
                obj = Instantiate(preFoot, transform.position - new Vector3(0, 0.4f, 0), Quaternion.identity);
                obj.GetComponent<Effect>().autoDelete = 1;
                obj.transform.SetParent(mapEffect);
                obj.GetComponent<SpriteRenderer>().sprite = footPrint[pre[Anim.GetInteger("direction")]/2+8];
                foot = !foot;
            }
            if (step>=stepTime)step = 0;
            if (encount >= SysDB.encount) {
                if (enc != null) enc.main();
                encount = 0;
            }
            if (steep != 0) {
                playerVelocity += new Vector2(0, steep * playerVelocity.x);
                playerVelocity.Normalize();
                playerVelocity *= speed;
            }
            if (Ladder) playerVelocity *= 0.5f;
            transform.Translate(playerVelocity);
        } else if(AnimFlag) {
            Anim.speed = 0f;
            Anim.SetInteger("moving", 0);
            AnimFlag = false;
            step = stepTime;
        }
    }
    int stepSound(int map,int chip,string other) {
        if (map == 0) {//mura
            switch (chip) {
                case 2:
                case 3:
                case 41:
                case 42:
                case 43:
                case 44:
                case 83: return 0;//石の床
                case 0:
                case 70:
                case 71:
                case 72:
                case 73:
                case 74:
                case 75:
                case 78:
                case 80:
                case 86:
                case 87:
                case 88:
                case 1: return 2;//砂利
                case 102:
                case 103:
                case 104:
                case 105:
                case 110:
                case 118: return 3;//土壌
            }
        } else if (map == 1) {//forest2
            switch (chip) {
                case 79:
                case 87:
                case 95:
                case 97:
                case 98:
                case 99:
                case 89:
                case 90:
                case 91: return 4;//橋
            }
        } else if (map == 2) {//town
            switch (chip) {
                case 4: return 4;//橋
            }
        } else if (map == 3) {//cave
            if (chip == 30 || chip == 38) PlayerLadder = true;
            switch (chip) {
                case 1:
                case 2:
                case 3:
                case 8:
                case 9:
                case 10:
                case 11:
                case 12:
                case 16:
                case 18:
                case 19:
                case 20:
                case 24:
                case 25:
                case 26: return 2;
                case 55:
                case 69: return 4;//橋
            }
        } else if (map == 4) {//sand
            switch (chip) {
                case 0:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                case 15:
                case 16:
                case 17:
                case 18:
                case 19:
                case 23:
                case 24:
                case 25:
                case 26: { PlayerFoot = true; return 3; }
                case 2:
                case 27:
                case 42:
                case 43:
                case 47:
                case 48:
                case 49:
                case 50:
                case 51:
                case 55:
                case 56:
                case 57:
                case 63:
                case 64:
                case 65: return 0;
            }
        } else if (map == 5) {//townMack
            switch (chip) {
                case 0:
                case 27:
                case 51:
                case 77: return 0;
            }
        } else if (map == 6) {//castle
            switch (chip) {
                case 5:
                case 6:
                case 7: return 0;
            }
        } else if (map == 7) {//snowtown
            switch (chip) {
                case 3:
                case 41:
                case 42:
                case 43: return 4;
                case 0:
                case 46:
                case 47:
                case 48:
                case 54:
                case 55:
                case 56:
                case 57:
                case 58:
                case 59:
                case 62:
                case 63:
                case 64:
                case 65:
                case 66: { PlayerFoot = true; return 3; }
            }
        } else if (map == 8) {//snow02
            switch (chip) {
                case 54:
                case 55:
                case 62: { steep = 1; PlayerFoot = true; return 3; }
                case 56:
                case 57:
                case 65: { steep = -1; PlayerFoot = true; return 3; }
            }
        } else if (map == 9) {//icecave
            switch (chip) {
                case 1:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 13:
                case 14:
                case 15:
                case 16:
                case 17:
                case 21:
                case 22:
                case 23: return 0;
            }
        } else if (map == 10) {//volc
            switch (chip) {
                case 0:
                case 1:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                case 12:
                case 13:
                case 14:
                case 15:
                case 16:
                case 17:
                case 21:
                case 22:
                case 23: return 2;
                case 40: return 0;
            }
        } else if (map == 11) {//skelcave
            switch (chip) {
                case 0:
                case 4:
                case 5:
                case 6:
                case 12:
                case 13:
                case 14:
                case 20:
                case 21:
                case 22:
                case 23:
                case 24:
                case 31:
                case 32: return 2;
            }
        } else if (map == 12) {//mori2
            switch (chip) {
                case 1:
                case 3:
                case 7:
                case 8:
                case 9:
                case 15:
                case 17:
                case 23:
                case 24:
                case 25:
                case 118: return 2;
                case 34: return 0;
            }
        } else if (map == 13) {//sanc
            switch (chip) {
                case 0:
                case 2:
                case 58:
                case 59:
                case 60: return 0;
            }
        } else if (map == -1) {//Autotile
            switch (other) {
                case "t_town01":
                case "t_sanc02":
                case "snow03": return 0;
                case "t_fairy02": return 1;
                case "t_sand":
                case "t_dang03":
                case "t_cave03":
                case "t_cave05":
                case "t_dang01":
                case "snow02":
                case "t_dang02": return 2;
                case "ice":
                case "t_snow01": { PlayerSkate = true; return 5; }
            }
        }
        return -1;
    }

    private void playSE(int num,int delay) {StartCoroutine(se(num,delay));}//SE再生
    IEnumerator se(int num, int delay) {
        yield return new WaitForSeconds(delay / 60f);
        audioS.PlayOneShot(SE[num], 3);
    }
    IEnumerator messageShow(string name,string mess) {
        SysDB.eventFlag = true;
        nameText.text = name;
        messageText.text = mess;
        nameRect.localPosition = new Vector3(-274f, 135f, 0);
        messageRect.localPosition = new Vector3(0, 45f, 0);
        textRect.localPosition = new Vector3(4.2f, 23f, 0);
        while (Input.GetKeyDown(KeyCode.Z) || Input.GetMouseButton(0)) yield return null;
        while (!Input.GetKeyDown(KeyCode.Z) && !Input.GetMouseButton(0)) yield return null;
        while (Input.GetKeyDown(KeyCode.Z) || Input.GetMouseButton(0)) yield return null;
        SysDB.eventFlag = false;
        nameRect.localPosition = new Vector3(-279.5f, -1500, 0);
        messageRect.localPosition = new Vector3(0, -1500, 0);
        textRect.localPosition = new Vector3(4.2f, -1500, 0);
    }
    public void playerDirection(Vector2 dire) {
        float theta = Mathf.Atan2(dire.y, dire.x);
        int direction = 2;
        theta *= 180 / Mathf.PI;
        if (112.5 <= theta && theta < 157.5) direction = 6;//7:テンキー
        else if (67.5 <= theta && theta < 112.5) direction = 7;//8
        else if (22.5 <= theta && theta < 67.5) direction = 8;//9
        else if (-22.5 <= theta && theta < 22.5) direction = 5;//6
        else if (-67.5 <= theta && theta < -22.5) direction = 3;//3
        else if (-112.5 <= theta && theta < -67.5) direction = 2;//2
        else if (-157.5 <= theta && theta < -112.5) direction = 1;//1
        else direction = 4;//4
        if (Anim != null) {
            Anim.SetInteger("moving", 1);
            if(direction!= Anim.GetInteger("direction"))Anim.SetInteger("direction", direction);
        } else {
            GetComponent<Animator>().SetInteger("moving", 1);
            GetComponent<Animator>().SetInteger("direction", direction);
        }
    }
    public void playerRotation(bool clock) {
        int[] dirClock = { 1, 4, 1, 2, 6, 3, 7, 8, 5};
        int[] antClock = { 1, 2, 3, 5, 1, 8, 4, 6, 7};
        int pre = Anim.GetInteger("moving");
        Anim.SetInteger("moving", 1);
        if (clock) {
            Anim.SetInteger("direction", dirClock[Anim.GetInteger("direction")]);
        } else {
            Anim.SetInteger("direction", antClock[Anim.GetInteger("direction")]);
        }
    }
}
