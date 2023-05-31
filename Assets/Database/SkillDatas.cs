using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class SkillDatas : ScriptableObject {
    public List<SkillData> SkillDataList = new List<SkillData>();
}

[System.Serializable]
public class SkillData {
    public string Name;
    [Multiline(3)]
    public string Detail;
    public int SP;
    public int Type;
    //0:誰か1体,1:敵全体,2:味方全体,3:自分自身,4:敵ランダム
    //5:戦闘不能にも使用可
    public int Priority;
    public int Genre;
    //0:攻撃,1:回復,2:バフ,3:デバフ,4:その他
    public bool Map;
    public bool Chemi;//化学技かどうか？
    public int Attribute;
}