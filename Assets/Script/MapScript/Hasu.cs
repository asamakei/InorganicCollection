using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Hasu : MonoBehaviour{

    int count;
    bool moveFlag=false, trigPlayer;
    GameObject player,mapEffect, grid,spiral;
    Tilemap hasuChip;
    Vector3 move;
    Vector3 moveDiff;
    Vector3 pos1, pos2,screenPos;
    Vector2 mousePos;
    Vector3Int posGrid;
    Vector3Int[] dir;
    AudioSource audioS;
    TileBase tileName;
    RectTransform canvas;
    public float speed;
    public GameObject wave;
    public Vector3 autoMove;
    public bool useful;
    public int moveMode = -1;


    void Start(){
        grid = GameObject.Find("Grid");
        player = GameObject.Find("Player");
        mapEffect = GameObject.Find("MapEffect");
        canvas = GameObject.Find("Canvas").GetComponent<RectTransform>();
        hasuChip = GameObject.Find("hasuChip").GetComponent<Tilemap>();
        audioS = GetComponent<AudioSource>();
        pos1 = this.transform.position + new Vector3(0, 0.38f, 0);
        pos2 = this.transform.position + new Vector3(0, 0.38f, 0);
        count = 0;
        dir =new Vector3Int[8];
        dir[0] = new Vector3Int(1, 0, 0);
        dir[1] = new Vector3Int(-1, 0, 0);
        dir[2] = new Vector3Int(0, 1, 0);
        dir[3] = new Vector3Int(0, -1, 0);
        dir[4] = new Vector3Int(1, 1, 0);
        dir[5] = new Vector3Int(1, -1, 0);
        dir[6] = new Vector3Int(-1, 1, 0);
        dir[7] = new Vector3Int(-1, -1, 0);
        if (moveMode == 0) {
            spiral = GameObject.Find("spiral");
        }
    }

    void FixedUpdate(){
        pos1 = this.transform.position + new Vector3(0, 0.38f, 0);
        moveDiff = pos1 - pos2;
        pos2 = this.transform.position + new Vector3(0, 0.38f, 0);
        if (useful) {
            move = player.transform.position - (this.transform.position + new Vector3(0, 0.38f, 0));
            if (move.magnitude <= 0.8f && !SysDB.eventFlag && !SysDB.menueFlag) {
                if (move.magnitude <= 0.1f) {
                    move *= 0;
                    moveDiff *= 0.9f;
                }
                move.Normalize();
                move *= speed;
                player.transform.Translate(moveDiff);
                if (!moveFlag) {
                    Instantiate(wave, transform.position, Quaternion.identity).transform.SetParent(mapEffect.transform);
                    audioS.PlayOneShot(audioS.clip);
                    moveFlag = true;
                }
                move = moveDiff + move * 0.03f;
                move += autoMove;
                if (move.magnitude >= 0.07f) {
                    move.Normalize();
                    move *= 0.07f;
                }
                if (moveMode == 0) {
                    if ((this.transform.position - spiral.transform.position).magnitude <= 10) {
                        move += gaiseki(this.transform.position - spiral.transform.position, new Vector3(0, 0, 1), true) * 0.003f;
                        move += (spiral.transform.position - this.transform.position).normalized * 0.002f;
                    }
                }
                transform.Translate(move);
                if (moveDiff.magnitude >= 0.01f) count++;
                if (count >= 10) {
                    count = 0;
                    Instantiate(wave, transform.position, Quaternion.identity).transform.SetParent(mapEffect.transform);
                }
            } else moveFlag = false;
        } else {
            move.Normalize();
            move *= speed;
            move = moveDiff*0.9f + move * 0.03f;
            move += autoMove;
            if (move.magnitude >= 0.07f) {
                move.Normalize();
                move *= 0.07f;
            }
            transform.Translate(move);
            if (moveDiff.magnitude >= 0.01f) count++;
            if (count >= 20) {
                count = 0;
                Instantiate(wave, transform.position, Quaternion.identity).transform.SetParent(mapEffect.transform);
            }
        }
    }
    Vector3 gaiseki(Vector3 v1,Vector3 v2,bool normal) {
        Vector3 v;
        v = new Vector3(v1.y*v2.z-v1.z*v2.y, (v1.z * v2.x - v1.x * v2.z)*0.8f, v1.x * v2.y - v1.y * v2.x);
        if (normal) v.Normalize();
        return v;
    }
    void Update() {
        if (SysDB.eventFlag == false && SysDB.menueFlag == false && SysDB.battleFlag == false) {
            if (Input.GetMouseButtonUp(0) && grid!=null && hasuChip!=null) {
                move = player.transform.position - (this.transform.position + new Vector3(0, 0.38f, 0));
                if (move.magnitude <= 1f) {
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas, Input.mousePosition, null, out mousePos);
                    mousePos += new Vector2(414,1475/2f);
                    if (mousePos.y > 407 && mousePos.y < 660 && (mousePos - new Vector2(414, 407)).magnitude > 200) {
                        posGrid = new Vector3Int(
                                    (int)Mathf.Floor(player.transform.position.x - grid.transform.position.x),
                                    (int)Mathf.Floor(player.transform.position.y - grid.transform.position.y),
                                    (int)Mathf.Floor(player.transform.position.z - grid.transform.position.z)
                                    );
                        tileName = hasuChip.GetTile(posGrid);
                        if (tileName != null) {
                            if (tileName.name == "hasuChip_0") {
                                for (int i = 0; i < 8; i++) {
                                    tileName = hasuChip.GetTile(posGrid + dir[i]);
                                    if (tileName!=null && tileName.name == "hasuChip_1") {
                                        double left=0;
                                        posGrid += dir[i];
                                        if (i == 1 || i == 6 || i == 7) left = -0.5f;
                                        GetComponent<MessageNPC>().message[0]
                                            = "playSE:6:10\nMoveTo:Player:1:" + (posGrid.x+1+left).ToString() + ":" + (posGrid.y+1).ToString() + ":0:0.2:1";
                                        GetComponent<MessageNPC>().main();
                                        SysDB.hasuFlag = false;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                } else if(trigPlayer==true){
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas, Input.mousePosition, null, out mousePos);
                    mousePos += new Vector2(414, 1475 / 2f);
                    if (mousePos.y > 407 && mousePos.y < 660 && (mousePos - new Vector2(414, 407)).magnitude > 200) {
                        GetComponent<MessageNPC>().message[0]
                        = "playSE:6:10\nMoveTo:Player:3:0:0.5:0:0.4:" + this.name;
                        GetComponent<MessageNPC>().main();
                        SysDB.hasuFlag = true;
                    }
                }
            }
        }
    }
    void OnTriggerStay2D(Collider2D collision) {
        if (collision.transform.name == "Player") trigPlayer = true;
    }
    void OnTriggerExit2D(Collider2D collision) {
        if (collision.transform.name == "Player") trigPlayer = false;
    }
}
