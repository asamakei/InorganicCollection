using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class JumpDatas : ScriptableObject {
    public List<JumpData> JumpDataList = new List<JumpData>();
}

[System.Serializable]
public class JumpData {
    public string Name;
    public string MapName;
    public Vector3 position;
    public bool movable;
}