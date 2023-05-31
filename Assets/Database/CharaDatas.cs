using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class CharaDatas : ScriptableObject {
    public List<CharaData> CharaDataList = new List<CharaData>();
}

[System.Serializable]
public class CharaData {
    public string Name;
    [Multiline(3)]
    public string[] Detail = new string[5];
}
