using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapEdit : MonoBehaviour{

    public IEnumerator main(int num) {yield return StartCoroutine(main(num, 1));}
    public IEnumerator main(int num, int wait) {
        Tilemap[] obj;
        TileBase[] tile;
        if (num == 0) {//メンデレ水源の橋
            obj = new Tilemap[3];
            obj[0] = GameObject.Find("Grid").transform.Find("Ground3").GetComponent<Tilemap>();
            obj[1] = GameObject.Find("Grid").transform.Find("Object").GetComponent<Tilemap>();
            obj[2] = GameObject.Find("Grid").transform.Find("TopGround").GetComponent<Tilemap>();
            tile = new TileBase[3];
            tile[0] = obj[0].GetTile(new Vector3Int(-47, 17, 0));
            tile[1] = obj[0].GetTile(new Vector3Int(-47, 16, 0));
            tile[2] = obj[1].GetTile(new Vector3Int(-47, 16, 0));
            if (tile[0] != null) {
                for(int i = -47; i <= -36; i++) {
                    obj[0].SetTile(new Vector3Int(i, 17, 0), null);
                    obj[0].SetTile(new Vector3Int(i, 16, 0), null);
                    obj[1].SetTile(new Vector3Int(i, 16, 0), null);
                    obj[2].SetTile(new Vector3Int(i, 17, 0), tile[0]);
                    obj[2].SetTile(new Vector3Int(i, 16, 0), tile[1]);
                    obj[2].SetTile(new Vector3Int(i, 16, 1), tile[2]);
                }
                tile[0] = obj[0].GetTile(new Vector3Int(-48, 17, 0));
                obj[2].SetTile(new Vector3Int(-48, 17, 0), tile[0]);
                tile[0] = obj[0].GetTile(new Vector3Int(-35, 17, 0));
                obj[2].SetTile(new Vector3Int(-35, 17, 0), tile[0]);
                tile[0] = obj[0].GetTile(new Vector3Int(-48, 16, 0));
                obj[2].SetTile(new Vector3Int(-48, 16, 0), tile[0]);
                tile[0] = obj[0].GetTile(new Vector3Int(-35, 16, 0));
                obj[2].SetTile(new Vector3Int(-35, 16, 0), tile[0]);
                tile[0] = obj[1].GetTile(new Vector3Int(-48, 16, 0));
                obj[2].SetTile(new Vector3Int(-48, 16, 1), tile[0]);
                tile[0] = obj[1].GetTile(new Vector3Int(-35, 16, 0));
                obj[2].SetTile(new Vector3Int(-35, 16, 1), tile[0]);
            }

        } else if(num == 1) {//メンデレ水源の橋
            obj = new Tilemap[3];
            obj[0] = GameObject.Find("Grid").transform.Find("Ground3").GetComponent<Tilemap>();
            obj[1] = GameObject.Find("Grid").transform.Find("Object").GetComponent<Tilemap>();
            obj[2] = GameObject.Find("Grid").transform.Find("TopGround").GetComponent<Tilemap>();
            tile = new TileBase[3];
            tile[0] = obj[2].GetTile(new Vector3Int(-47, 17, 0));
            tile[1] = obj[2].GetTile(new Vector3Int(-47, 16, 0));
            tile[2] = obj[2].GetTile(new Vector3Int(-47, 16, 1));
            if (tile[0] != null) {
                for (int i = -47; i <= -36; i++) {
                    obj[2].SetTile(new Vector3Int(i, 17, 0), null);
                    obj[2].SetTile(new Vector3Int(i, 16, 0), null);
                    obj[2].SetTile(new Vector3Int(i, 16, 0), null);
                    obj[0].SetTile(new Vector3Int(i, 17, 0), tile[0]);
                    obj[0].SetTile(new Vector3Int(i, 16, 0), tile[1]);
                    obj[1].SetTile(new Vector3Int(i, 16, 0), tile[2]);
                }
                obj[2].SetTile(new Vector3Int(-48, 17, 0), null);
                obj[2].SetTile(new Vector3Int(-35, 17, 0), null);
                obj[2].SetTile(new Vector3Int(-48, 16, 0), null);
                obj[2].SetTile(new Vector3Int(-35, 16, 0), null);
                obj[2].SetTile(new Vector3Int(-48, 16, 1), null);
                obj[2].SetTile(new Vector3Int(-35, 16, 1), null);
            }

        } else if (num == 2) {//オストワルトの森の橋
            obj = new Tilemap[3];
            obj[0] = GameObject.Find("Grid").transform.Find("Ground3").GetComponent<Tilemap>();
            obj[1] = GameObject.Find("Grid").transform.Find("Object").GetComponent<Tilemap>();
            obj[2] = GameObject.Find("Grid").transform.Find("TopGround").GetComponent<Tilemap>();
            tile = new TileBase[3];
            tile[0] = obj[0].GetTile(new Vector3Int(-37, -24, 0));
            tile[1] = obj[0].GetTile(new Vector3Int(-37, -25, 0));
            tile[2] = obj[1].GetTile(new Vector3Int(-37, -25, 0));
            if (tile[0] != null) {
                for (int i = -38; i <= -30; i++) {
                    obj[0].SetTile(new Vector3Int(i, -24, 0), null);
                    obj[0].SetTile(new Vector3Int(i, -25, 0), null);
                    obj[1].SetTile(new Vector3Int(i, -25, 0), null);
                    obj[2].SetTile(new Vector3Int(i, -24, 0), tile[0]);
                    obj[2].SetTile(new Vector3Int(i, -25, 0), tile[1]);
                    obj[2].SetTile(new Vector3Int(i, -25, 1), tile[2]);
                }
                tile[0] = obj[0].GetTile(new Vector3Int(-39, -24, 0));
                obj[2].SetTile(new Vector3Int(-39, -24, 0), tile[0]);
                tile[0] = obj[0].GetTile(new Vector3Int(-39, -25, 0));
                obj[2].SetTile(new Vector3Int(-39, -25, 0), tile[0]);
                tile[0] = obj[1].GetTile(new Vector3Int(-39, -25, 0));
                obj[2].SetTile(new Vector3Int(-39, -25, 1), tile[0]);
                tile[0] = obj[0].GetTile(new Vector3Int(-29, -24, 0));
                obj[2].SetTile(new Vector3Int(-29, -24, 0), tile[0]);
                tile[0] = obj[0].GetTile(new Vector3Int(-29, -25, 0));
                obj[2].SetTile(new Vector3Int(-29, -25, 0), tile[0]);
                tile[0] = obj[1].GetTile(new Vector3Int(-29, -25, 0));
                obj[2].SetTile(new Vector3Int(-29, -25, 1), tile[0]);
                tile[0] = obj[0].GetTile(new Vector3Int(-39, -23, 0));
                obj[0].SetTile(new Vector3Int(-39, -25, -1), tile[0]);
                tile[0] = obj[0].GetTile(new Vector3Int(-29, -23, 0));
                obj[0].SetTile(new Vector3Int(-29, -25, -1), tile[0]);
            }

        } else if (num == 3) {//オストワルトの森の橋
            obj = new Tilemap[3];
            obj[0] = GameObject.Find("Grid").transform.Find("Ground3").GetComponent<Tilemap>();
            obj[1] = GameObject.Find("Grid").transform.Find("Object").GetComponent<Tilemap>();
            obj[2] = GameObject.Find("Grid").transform.Find("TopGround").GetComponent<Tilemap>();
            tile = new TileBase[3];
            tile[0] = obj[2].GetTile(new Vector3Int(-37, -24, 0));
            tile[1] = obj[2].GetTile(new Vector3Int(-37, -25, 0));
            tile[2] = obj[2].GetTile(new Vector3Int(-37, -25, 1));
            if (tile[0] != null) {
                for (int i = -38; i <= -30; i++) {
                    obj[2].SetTile(new Vector3Int(i, -24, 0), null);
                    obj[2].SetTile(new Vector3Int(i, -25, 0), null);
                    obj[2].SetTile(new Vector3Int(i, -25, 1), null);
                    obj[0].SetTile(new Vector3Int(i, -24, 0), tile[0]);
                    obj[0].SetTile(new Vector3Int(i, -25, 0), tile[1]);
                    obj[1].SetTile(new Vector3Int(i, -25, 0), tile[2]);
                }
  
                obj[2].SetTile(new Vector3Int(-39, -24, 0),null);
                obj[2].SetTile(new Vector3Int(-39, -25, 0),null);
                obj[2].SetTile(new Vector3Int(-39, -25, 1),null);
                obj[2].SetTile(new Vector3Int(-29, -24, 0),null);
                obj[2].SetTile(new Vector3Int(-29, -25, 0),null);
                obj[2].SetTile(new Vector3Int(-29, -25, 1),null);
                obj[0].SetTile(new Vector3Int(-39, -25, -1),null);
                obj[0].SetTile(new Vector3Int(-29, -25, -1),null);
            }
        } else if (num == 4) {//畑
            Vector3Int[] vec = new Vector3Int[84];
            Vector3Int pre;
            GameController gameCon;
            int rand;
            gameCon = GameObject.Find("GameController").GetComponent<GameController>();
            obj = new Tilemap[1];
            obj[0] = GameObject.Find("Grid").transform.Find("Object").GetComponent<Tilemap>();
            tile = new TileBase[3];
            tile[0] = obj[0].GetTile(new Vector3Int(-40, 43, 0));
            tile[1] = obj[0].GetTile(new Vector3Int(-41, 43, 0));
            tile[2] = obj[0].GetTile(new Vector3Int(-42, 43, 0));
            for(int i = -19; i < -19 + 12; i++) {
                for(int j = 23; j < 23 + 7; j++) {
                    vec[(i+19)*7+j-23]= new Vector3Int(i,j,0);
                }
            }
            if (wait > 0) {
                for (int i = 0; i < vec.Length; i++) {
                    rand = SysDB.randomInt(0, 83);
                    pre = vec[i];
                    vec[i] = vec[rand];
                    vec[rand] = pre;
                }
            }
            for (int k = 0; k < 2; k++) {
                if (k == 0 && wait == 0) continue;
                for (int i = 0; i < 12; i++) {
                    for (int j = 0; j < 7; j++) {
                        if (vec[i * 7 + j] != new Vector3Int(-18, 24, 0) && vec[i * 7 + j] != new Vector3Int(-9, 28, 0) && vec[i * 7 + j] != new Vector3Int(-13, 26, 0)) {
                            if (k == 1) obj[0].SetTile(vec[i * 7 + j], tile[SysDB.randomInt(1,2)]);
                            else obj[0].SetTile(vec[i * 7 + j], tile[k]);

                        }
                    }
                    if (wait > 0) {
                        gameCon.SE(21);
                        yield return new WaitForSeconds(wait * 7 / 60f);
                    }
                }
            }
            if(wait==0) obj[0].SetTile(new Vector3Int(-13, 26, 0), tile[SysDB.randomInt(1, 2)]);
        }
    }
}
