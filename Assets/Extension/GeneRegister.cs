using System.Collections;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

public class GeneRegister : EditorWindow {
    [MenuItem("Extension/GeneRegister")]
    static void ShowWindow() {
        EditorWindow.GetWindow<GeneRegister>();
    }
    GeneDatas geneDB = null;
    string material;
    string item;
    string result;

    void OnGUI() {
        geneDB = EditorGUILayout.ObjectField("GeneDatas", geneDB, typeof(Object), true) as GeneDatas;
        EditorGUILayout.LabelField("Material");
        material = EditorGUILayout.TextArea(material, GUILayout.Height(96.0f));
        EditorGUILayout.LabelField("Item");
        item = EditorGUILayout.TextArea(item, GUILayout.Height(96.0f));
        EditorGUILayout.LabelField("Result");
        result = EditorGUILayout.TextArea(result, GUILayout.Height(96.0f));
        if (GUILayout.Button("Make")) {
            int len=0;
            string[] arrayM,arrayI,arrayR,code;
            GeneData gene;
            arrayM = material.Split('\n');
            arrayI = item.Split('\n');
            arrayR = result.Split('\n');

            len = arrayM.Length;
            if (len > arrayI.Length) len = arrayI.Length;
            if (len > arrayR.Length) len = arrayR.Length;
            for(int i = 2; i < len+2; i++) {
                if (i >= geneDB.GeneDataList.Count) geneDB.GeneDataList.Add(new GeneData());
                gene = geneDB.GeneDataList[i];
                code = arrayM[i-2].Split(',');
                gene.Monster1 = int.Parse(code[0]);
                gene.Monster2 = int.Parse(code[1]);
                gene.Monster3 = int.Parse(code[2]);
                gene.Monster4 = int.Parse(code[3]);
                code = arrayR[i - 2].Split(',');
                gene.Result1 = int.Parse(code[0]);
                gene.Result2 = int.Parse(code[1]);
                gene.Result3 = int.Parse(code[2]);
                gene.Result4 = int.Parse(code[3]);
                gene.Item = int.Parse(arrayI[i - 2]);
                gene.Name = "";
            }
            EditorUtility.SetDirty(geneDB);
            AssetDatabase.SaveAssets();
        }
    }
}
#endif