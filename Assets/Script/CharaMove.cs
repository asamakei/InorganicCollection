using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharaMove : MonoBehaviour{

    public bool notCharacter = false;
    public float speed = 0.05f;
    public bool chase = false;
    public GameObject chaseTarget;
    public bool random = true;
    public float restPase = 100;
    public float movePase = 15;
    public int customMove = 0;
    public Vector2 firstDirect = new Vector2(0,-1);
    public EaseType EventTrigger = EaseType.None;
    public Vector3 EventMove;
    public bool stop=false;
    //public Component Event;

    private Vector2 charaVelocity;
    private Animator Anim;
    private float moveTime = 0;
    private bool trigPlayer = false;
    private bool colliPlayer = false;
    private Vector2 mousePos;
    private GameObject player;
    private Vector2 pos;
    private Vector2 playerDir;
    private PlayerMove playerMove;
    private float cos;
    private Camera cameraAsp;
    private RectTransform canvas;
    private GameController gameCon;

    void Start(){
        if(!notCharacter) Anim = this.GetComponent<Animator>();
        gameCon = GameObject.Find("GameController").GetComponent<GameController>();
        canvas = gameCon.canvasObj.GetComponent<RectTransform>();
        //cameraAsp = gameCon.cameraObj.GetComponent<Camera>();
        cameraAsp = null;
        player = GameObject.Find("Player");
        playerMove = player.GetComponent<PlayerMove>();
        playerDir = firstDirect;
        charaDirection(firstDirect);

    }

    void Update() {
        ////////イベント実行処理/////////

        if (trigPlayer) {
            if ((EventTrigger == EaseType.PressKey || EventTrigger == EaseType.DirectionKey) && !SysDB.eventFlag && !SysDB.menueFlag) {//決定キーで実行
                if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Z)) {
                    mousePos = Input.mousePosition;
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas, Input.mousePosition, cameraAsp, out mousePos);
                    mousePos += new Vector2(414, 1475 / 2f);
                    if (mousePos.y > 407 && mousePos.y < 660 && (mousePos - new Vector2(414, 407)).magnitude > 200 || Input.GetKeyDown(KeyCode.Z)) {

                        Vector2 pos = this.transform.position - player.transform.position;
                        playerDir = playerMove.dir;
                        cos = Vector2.Dot(pos, playerDir) / (pos.magnitude * playerDir.magnitude);
                        if (cos >= 1 / Mathf.Sqrt(2) || cos >= 0.5f || EventTrigger == EaseType.PressKey) {

                            playerMove.playerDirection(pos);
                            charaDirection(-pos);

                            charaEvent();
                        }

                    }
                }
            } else if (EventTrigger == EaseType.OnTrigger && !SysDB.eventFlag && !SysDB.menueFlag) {//Trigger接触で実行
                charaEvent();
            }
        }
        if (colliPlayer) {
            if (EventTrigger == EaseType.OnCollision && !SysDB.eventFlag && !SysDB.menueFlag) {//Collision接触で実行
                charaEvent();
            }
        }
    }

    void FixedUpdate(){

        ////////キャラクター移動処理/////////
        float rand = Random.value;
        if (!notCharacter) {
            if (chase == true) {
                if (!colliPlayer) charaVelocity = chaseTarget.transform.position - this.transform.position;
                else charaVelocity = Vector2.zero;
            }
            if (random == true) {
                moveTime++;
                if (moveTime > movePase + restPase) {
                    moveTime = 0;
                    rand *= 2 * Mathf.PI;
                    charaVelocity.x = Mathf.Cos(rand);
                    charaVelocity.y = Mathf.Sin(rand);
                } else if (moveTime > movePase) charaVelocity = Vector2.zero;
            }

            if (customMove == 1) {
                moveTime++;
                charaVelocity = new Vector2(Mathf.Cos(moveTime / 4), Mathf.Sin(moveTime / 4));
            }
            if (SysDB.eventFlag && customMove != 1) charaVelocity = Vector2.zero;
            if (EventMove != Vector3.zero) {
                charaVelocity = EventMove;
                stop = true;
            } else if (stop) {
                charaVelocity = Vector3.zero;
                stop = false;
            }
            if (charaVelocity != Vector2.zero) {
                charaVelocity *= speed / charaVelocity.magnitude;
                Anim.speed = speed * 1.25f;
                charaDirection(charaVelocity);
            } else {
                Anim.speed = 0f;
                Anim.SetInteger("moving", 0);
            }
            if (customMove == 1) charaVelocity = Vector2.zero;

            this.transform.Translate(charaVelocity);
        }

    }

    void OnTriggerStay2D(Collider2D collision) {
        if (collision.transform.name == "Player") trigPlayer = true;
    }
    void OnTriggerExit2D(Collider2D collision) {
        if (collision.transform.name == "Player") trigPlayer = false;
    }
    void OnCollisionStay2D(Collision2D collision) {
        if (collision.transform.name == "Player") colliPlayer = true;
    }
    void OnCollisionExit2D(Collision2D collision) {
        if (collision.transform.name == "Player") colliPlayer = false;
    }

    public void charaDirection(Vector2 dire) {

        float theta = Mathf.Atan2(dire.y, dire.x);
        int direction = 2;
        theta *= 180 / Mathf.PI;
        if (157.5 <= theta) direction = 4;
        else if (112.5 <= theta && theta < 157.5) direction = 6;
        else if (67.5 <= theta && theta < 112.5) direction = 7;
        else if (22.5 <= theta && theta < 67.5) direction = 8;
        else if (-22.5 <= theta && theta < 22.5) direction = 5;
        else if (-67.5 <= theta && theta < -22.5) direction = 3;
        else if (-112.5 <= theta && theta < -67.5) direction = 2;
        else if (-157.5 <= theta && theta < -112.5) direction = 1;
        else if (theta < -157.5) direction = 4;

        if (!notCharacter) {
            Anim.SetInteger("direction", direction);
            Anim.SetInteger("moving", 1);
        }
    }

    public void charaEvent() {
        this.GetComponent<MessageNPC>().main();
    }

}
public enum EaseType {
    None,
    PressKey,
    DirectionKey,
    OnCollision,
    OnTrigger,
}
