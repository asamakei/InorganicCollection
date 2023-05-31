using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ItemDatas : ScriptableObject {
    public string ShopName;
    public List<ItemData> ItemDataList = new List<ItemData>();
}

[System.Serializable]
public class ItemData {
    public string Name;
    [Multiline(3)]
    public string Detail;
    public int Possess;
    public int Type;
    //0:誰か1体,1:敵全体,2:味方全体,4:敵ランダム,-1:マップ時モンスター非対象,
    //-2:装備,-3:合成時のみ,5:戦闘不能にも使用可能
    public bool Map;
    public int money;
}
