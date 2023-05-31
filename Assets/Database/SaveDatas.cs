using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class SaveDatas : ScriptableObject {
    public List<SaveData> SaveDataList = new List<SaveData>();
}

[System.Serializable]
public class SaveData {
    public string sceneName;
    public string spotName;
    public string[] ObjectName;
}