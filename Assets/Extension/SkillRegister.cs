using System.Collections;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

public class SkillRegister : EditorWindow {
    [MenuItem("Extension/SkillRegister")]
    static void ShowWindow() {
        EditorWindow.GetWindow<SkillRegister>();
    }
    SkillDatas skillDB = null;
    CharaDatas charaDB = null;
    ItemDatas itemDB = null;
    MonsterStates monDB = null;
    EnemyDatas eneDB = null;
    AllyDatas allyDB = null;

    string mainData;


    void OnGUI() {
        skillDB = EditorGUILayout.ObjectField("SkillDatas", skillDB, typeof(Object), true) as SkillDatas;
        charaDB = EditorGUILayout.ObjectField("CharaDatas", charaDB, typeof(Object), true) as CharaDatas;
        itemDB = EditorGUILayout.ObjectField("ItemDatas", itemDB, typeof(Object), true) as ItemDatas;
        monDB = EditorGUILayout.ObjectField("MonsterStates", monDB, typeof(Object), true) as MonsterStates;
        eneDB = EditorGUILayout.ObjectField("EnemyDatas", eneDB, typeof(Object), true) as EnemyDatas;
        allyDB = EditorGUILayout.ObjectField("AllyDatas", allyDB, typeof(Object), true) as AllyDatas;

        EditorGUILayout.LabelField("Material");
        mainData = EditorGUILayout.TextArea(mainData, GUILayout.Height(96.0f));
        if (GUILayout.Button("SkillMake")) {
            string[] arrayM = mainData.Split('\n');
            SkillData skill;
            int len = arrayM.Length;
            for (int i = 0; i < len; i++) {
                string[] arrayD = arrayM[i].Split(':');
                skill = new SkillData();
                skill.Name = arrayD[1];
                skill.SP = int.Parse(arrayD[2]);
                skill.Type = int.Parse(arrayD[3]);
                skill.Genre = int.Parse(arrayD[4]);
                skill.Priority = int.Parse(arrayD[5]);
                skill.Map = int.Parse(arrayD[6])==1;
                skill.Chemi = int.Parse(arrayD[7]) == 1;
                skill.Attribute = int.Parse(arrayD[8]);
                skill.Detail = arrayD[9].Replace(';', '\n');
                skillDB.SkillDataList[int.Parse(arrayD[0])] = skill;
            }
            EditorUtility.SetDirty(skillDB);
            AssetDatabase.SaveAssets();
        }
        if (GUILayout.Button("CharaMake")) {
            string[] arrayM = mainData.Split('\n');
            CharaData chara;
            int len = arrayM.Length;
            for (int i = 0; i < len; i++) {
                string[] arrayD = arrayM[i].Split(':');
                chara = new CharaData();
                chara.Name = arrayD[1];
                chara.Detail = new string[5];
                for(int j=0;j<5;j++)chara.Detail[j] = arrayD[j+2].Replace(';', '\n');
                charaDB.CharaDataList[int.Parse(arrayD[0])] = chara;
            }
            EditorUtility.SetDirty(charaDB);
            AssetDatabase.SaveAssets();
        }
        if (GUILayout.Button("ItemMake")) {
            string[] arrayM = mainData.Split('\n');
            ItemData item;
            int len = arrayM.Length;
            for (int i = 0; i < len; i++) {
                string[] arrayD = arrayM[i].Split(':');
                item = new ItemData();
                item.Name = arrayD[1];
                item.Detail = arrayD[2].Replace(';', '\n');
                item.Type = int.Parse(arrayD[3]);
                item.Map = int.Parse(arrayD[4])==1;
                item.money = int.Parse(arrayD[5]);
                item.Possess = int.Parse(arrayD[6]);
                itemDB.ItemDataList[int.Parse(arrayD[0])] = item;
            }
            EditorUtility.SetDirty(itemDB);
            AssetDatabase.SaveAssets();
        }
        if (GUILayout.Button("MonsterCharaMake")) {
            string[] arrayM = mainData.Split('\n');
            int len = arrayM.Length;
            for (int i = 0; i < len; i++) {
                string[] arrayD = arrayM[i].Split(':');
                if (arrayD[0] != "") {
                    int num = int.Parse(arrayD[0]);

                    System.Array.Resize(ref monDB.MonsterDataList[num].Character,int.Parse(arrayD[1]));
                    for(int j=0;j < int.Parse(arrayD[1]); j++) {
                        monDB.MonsterDataList[num].Character[j] = int.Parse(arrayD[j+2]);
                    }
                }
            }
            EditorUtility.SetDirty(monDB);
            AssetDatabase.SaveAssets();
        }
        if (GUILayout.Button("MonsterSkillMake")) {
            string[] arrayM = mainData.Split('\n');
            int len = arrayM.Length;
            for (int i = 0; i < len; i++) {
                string[] arrayD = arrayM[i].Split(':');
                if (arrayD[0] != "") {
                    int num = int.Parse(arrayD[0]);

                    System.Array.Resize(ref monDB.MonsterDataList[num].Skill, int.Parse(arrayD[1]));
                    for (int j = 0; j < int.Parse(arrayD[1]); j++) {
                        monDB.MonsterDataList[num].Skill[j] = int.Parse(arrayD[j + 2]);
                    }
                }
            }
            EditorUtility.SetDirty(monDB);
            AssetDatabase.SaveAssets();
        }
        if (GUILayout.Button("MonsterStatesMake")) {
            string[] arrayM = mainData.Split('\n');
            int len = arrayM.Length;
            for (int i = 0; i < len; i++) {
                string[] arrayD = arrayM[i].Split(':');
                if (arrayD[0] != "") {
                    int num = int.Parse(arrayD[0]);
                    monDB.MonsterDataList[num].HPGrow = int.Parse(arrayD[1]);
                    monDB.MonsterDataList[num].SPGrow = int.Parse(arrayD[2]);
                    monDB.MonsterDataList[num].AttackGrow = int.Parse(arrayD[3]);
                    monDB.MonsterDataList[num].DefenceGrow = int.Parse(arrayD[4]);
                    monDB.MonsterDataList[num].MagicGrow = int.Parse(arrayD[5]);
                    monDB.MonsterDataList[num].MagicDefGrow = int.Parse(arrayD[6]);
                    monDB.MonsterDataList[num].SpeedGrow = int.Parse(arrayD[7]);
                    monDB.MonsterDataList[num].HPMax = int.Parse(arrayD[8]);
                    monDB.MonsterDataList[num].SPMax = int.Parse(arrayD[9]);
                    monDB.MonsterDataList[num].AttackMax = int.Parse(arrayD[10]);
                    monDB.MonsterDataList[num].DefenceMax = int.Parse(arrayD[11]);
                    monDB.MonsterDataList[num].MagicMax = int.Parse(arrayD[12]);
                    monDB.MonsterDataList[num].MagicDefMax = int.Parse(arrayD[13]);
                    monDB.MonsterDataList[num].SpeedMax = int.Parse(arrayD[14]);
                    monDB.MonsterDataList[num].Flame = int.Parse(arrayD[15]);
                    monDB.MonsterDataList[num].Water = int.Parse(arrayD[16]);
                    monDB.MonsterDataList[num].Electrical = int.Parse(arrayD[17]);
                    monDB.MonsterDataList[num].Oxidation = int.Parse(arrayD[18]);
                    monDB.MonsterDataList[num].Reduction = int.Parse(arrayD[19]);
                    monDB.MonsterDataList[num].OxidationP = int.Parse(arrayD[20]);
                }
            }
            EditorUtility.SetDirty(monDB);
            AssetDatabase.SaveAssets();
        }
        if (GUILayout.Button("DetailMake")) {
            string[] arrayM = mainData.Split('\n');
            int len = arrayM.Length;
            for (int i = 0; i < len; i++) {
                string[] arrayD = arrayM[i].Split(':');
                if (arrayD[0] != "") {
                    int num = int.Parse(arrayD[0]);
                    monDB.MonsterDataList[num].Detail = arrayD[1].Replace(';','\n');
                    monDB.MonsterDataList[num].howMake = arrayD[2].Replace(';', '\n');
                }
            }
            EditorUtility.SetDirty(monDB);
            AssetDatabase.SaveAssets();
        }
        if (GUILayout.Button("EnemyMake")) {
            string[] arrayM = mainData.Split('\n');
            int len = arrayM.Length;
            for (int i = 0; i < len; i++) {
                string[] arrayD = arrayM[i].Split(':');
                if (arrayD[0] != "") {
                    int num = int.Parse(arrayD[0]);
                    EnemyData ene = new EnemyData();
                    ene.Name = arrayD[1];
                    ene.ID = int.Parse(arrayD[2]);
                    ene.Level = int.Parse(arrayD[3]);
                    ene.HP = int.Parse(arrayD[4]);
                    ene.SP = int.Parse(arrayD[5]);
                    ene.Attack = int.Parse(arrayD[6]);
                    ene.Defence = int.Parse(arrayD[7]);
                    ene.Magic = int.Parse(arrayD[8]);
                    ene.MagicDef = int.Parse(arrayD[9]);
                    ene.Speed = int.Parse(arrayD[10]);
                    ene.Money = int.Parse(arrayD[11]);
                    ene.Exp = int.Parse(arrayD[12]);
                    if (int.Parse(arrayD[13]) >= 0) {
                        System.Array.Resize(ref ene.Item,1);
                        System.Array.Resize(ref ene.Rate, 1);
                        ene.Item[0] = int.Parse(arrayD[13]);
                        ene.Rate[0] = int.Parse(arrayD[14]);
                    }
                    System.Array.Resize(ref ene.Character, int.Parse(arrayD[15]));
                    for(int j=0;j< int.Parse(arrayD[15]); j++)ene.Character[j]= int.Parse(arrayD[16+j]);
                    System.Array.Resize(ref ene.Skill, int.Parse(arrayD[20]));
                    for (int j = 0; j < int.Parse(arrayD[20]); j++) ene.Skill[j] = int.Parse(arrayD[21 + j]);
                    ene.GetDiff = int.Parse(arrayD[31]);
                    eneDB.EnemyDataList[int.Parse(arrayD[0])] = ene;
                }
            }
            EditorUtility.SetDirty(eneDB);
            AssetDatabase.SaveAssets();
        }
        if (GUILayout.Button("MonsterPlus")) {
            string num = mainData;
            int ID = int.Parse(num);
            for (int i = 4; i < allyDB.AllyDataList.Count; i++) {
                if (allyDB.AllyDataList[i].ID <= 0) {
                    AllyData ally = new AllyData();
                    ally.Name = monDB.MonsterDataList[ID].Name;
                    ally.ID = ID;
                    ally.Level = 5;
                    System.Array.Resize(ref ally.Skill, 2);
                    System.Array.Resize(ref ally.SkillUse, 2);
                    System.Array.Resize(ref ally.Grow, 7);
                    System.Array.Resize(ref ally.Character, monDB.MonsterDataList[ID].Character.Length);
                    ally.Skill[0] = 0;
                    ally.Skill[1] = 0;
                    allyDB.AllyDataList[i] = ally;
                    Debug.Log(i);
                    break;
                }
            }
        }
    }
}
#endif