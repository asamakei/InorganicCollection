using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GeneDatas : ScriptableObject {
    public List<GeneData> GeneDataList = new List<GeneData>();
}

[System.Serializable]
public class GeneData {
    public string Name;
    public int Monster1;
    public int Monster2;
    public int Monster3;
    public int Monster4;
    public int Item = -1;//1:2:3:4 高温 低音 電気 圧力
    public int Result1;
    public int Result2;
    public int Result3;
    public int Result4;
}
