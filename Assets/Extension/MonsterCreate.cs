using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Animations;


public class MonsterCreate : EditorWindow {
#if UNITY_EDITOR
    [MenuItem("Extension/MonsterMake")]
    static void ShowWindow() {
        EditorWindow.GetWindow<MonsterCreate>();
    }

    MonsterStates monDB = null;
    string prefabName = "";
    string animName = "";
    string monName = "";
    string formula = "";

    Object idleSprite = null;
    Object attackSprite = null;
    Object magicSprite = null;
    Object listSprite = null;

    Vector2 idleCut = Vector2.zero;
    Vector2 attackCut = Vector2.zero;
    Vector2 magicCut = Vector2.zero;
    float idleSpeed = 1;
    float attackSpeed = 1;
    float magicSpeed = 1;
    SpriteMetaData[] MySheet;

    void OnGUI() {
        prefabName = EditorGUILayout.TextField("PrefabName", prefabName);
        animName = EditorGUILayout.TextField("AnimName", animName);
        monName = EditorGUILayout.TextField("MonsterName", monName);
        formula = EditorGUILayout.TextField("Formula", formula);
        monDB = EditorGUILayout.ObjectField("MonsterStates", monDB, typeof(Object), true) as MonsterStates;
        GUILayout.Space(20);
        idleSprite = EditorGUILayout.ObjectField("IdleSprite", idleSprite, typeof(Object), true);
        idleCut = EditorGUILayout.Vector2Field("IdleCut", idleCut);
        idleSpeed = EditorGUILayout.FloatField("IdleSpeed",idleSpeed);
        GUILayout.Space(20);
        attackSprite = EditorGUILayout.ObjectField("AttackSprite", attackSprite, typeof(Object), true);
        attackCut = EditorGUILayout.Vector2Field("AttackCut", attackCut);
        attackSpeed = EditorGUILayout.FloatField("AttackSpeed", attackSpeed);
        GUILayout.Space(20);
        magicSprite = EditorGUILayout.ObjectField("MagicSprite", magicSprite, typeof(Object), true);
        magicCut = EditorGUILayout.Vector2Field("MagicCut", magicCut);
        magicSpeed = EditorGUILayout.FloatField("MagicSpeed", magicSpeed);
        GUILayout.Space(20);
        listSprite = EditorGUILayout.ObjectField("ListSprite", listSprite, typeof(Object), true);

        if (GUILayout.Button("Make")) {

            Object obj;
            Vector2 cut;
            float speed;
            int index;
            string[] file = { "idle","attack","magic"};
            Vector3[] vect = {new Vector3(30,180,0), new Vector3(-90, 270, 0), new Vector3(150, 270, 0) };
            Sprite MySprites = null;

            for (int i = 0; i < 4; i++) {

                if (i == 0) {
                    obj = idleSprite;
                    cut = idleCut;
                } else if (i == 1) {
                    obj = attackSprite;
                    cut = attackCut;
                } else if(i==2){
                    obj = magicSprite;
                    cut = magicCut;
                } else {
                    obj = listSprite;
                    cut = new Vector2(1,1);
                }
                System.Type type = null;
                if (obj!=null)type = obj.GetType();
                if (obj!=null && type == typeof(UnityEngine.Texture2D)) {
                    Texture2D tex = obj as Texture2D;
                    float width = tex.width / cut.x;
                    float height = tex.height / cut.y;
                    if (i == 3) {
                        width = 50;
                        height = 50;
                        cut = new Vector2(tex.width/50, tex.height/50);
                    }
                    MySheet = new SpriteMetaData[(int)cut.x * (int)cut.y];
                    string TargetPath = AssetDatabase.GetAssetPath(obj);
                    TextureImporter importer = TextureImporter.GetAtPath(TargetPath) as TextureImporter;
                    importer.textureType = TextureImporterType.Sprite;
                    importer.spriteImportMode = SpriteImportMode.Multiple;
                    importer.filterMode = FilterMode.Point;
                    importer.spritePixelsPerUnit = 100;

                    for (int y = 0; y < cut.y; y++) {
                        for (int x = 0; x < cut.x; x++) {
                            index = ((int)cut.y-y-1) * (int)cut.x + x;
                            MySheet[index].name = obj.name + "_" + index.ToString();
                            MySheet[index].rect = new Rect(width * x, height * y, width, height);
                            MySheet[index].alignment = 7;
                        }
                    }
                    importer.spritesheet = MySheet;
                    AssetDatabase.ImportAsset(TargetPath, ImportAssetOptions.ForceUpdate);
                    AssetDatabase.Refresh();
                    if (i == 0) {
                        MySprites = AssetDatabase.LoadAssetAtPath<Sprite>(TargetPath);
                    }
                }
            }
            AnimatorController Target = AnimatorController.CreateAnimatorControllerAtPath("Assets/Resources/Monster/" + animName + ".controller");
            AnimatorStateMachine rootStateMachine = Target.layers[0].stateMachine;
            AnimatorState[] state = new AnimatorState[3];
            Target.AddParameter("attack", AnimatorControllerParameterType.Bool);
            Target.AddParameter("magic", AnimatorControllerParameterType.Bool);
            for (int j = 0; j < 3; j++) {
                AnimationClip NewClip = new AnimationClip();
                SerializedObject serializedClip = new SerializedObject(NewClip);
                SerializedProperty settings = serializedClip.FindProperty("m_AnimationClipSettings");
                while (settings.Next(true)) { if (settings.name == "m_LoopTime") break; }
                if (j != 1) settings.boolValue = true;
                else settings.boolValue = false;
                serializedClip.ApplyModifiedProperties();
                AssetDatabase.CreateAsset(NewClip, "Assets/Resources/Monster/" + animName + "_" + file[j] + ".anim");
                state[j] = rootStateMachine.AddState(animName + "_" + file[j], vect[j]);
                state[j].motion = NewClip;
                if (j == 0)speed = idleSpeed;
                else if (j == 1)speed = attackSpeed;
                else speed = magicSpeed;
                state[j].speed = speed;
            }

            rootStateMachine.defaultState = state[0];
            AnimatorStateTransition[] targetTransitionList = new AnimatorStateTransition[4];
            targetTransitionList[0] = state[0].AddTransition(state[1]);
            targetTransitionList[1] = state[1].AddTransition(state[0]);
            targetTransitionList[2] = state[0].AddTransition(state[2]);
            targetTransitionList[3] = state[2].AddTransition(state[0]);

            targetTransitionList[0].exitTime = 0f;
            targetTransitionList[0].duration = 0f;
            targetTransitionList[0].hasExitTime = false;
            targetTransitionList[0].AddCondition(AnimatorConditionMode.If, 1, "attack");
            targetTransitionList[1].exitTime = 0f;
            targetTransitionList[1].duration = 0f;
            targetTransitionList[1].hasExitTime = true;
            targetTransitionList[2].exitTime = 0f;
            targetTransitionList[2].duration = 0f;
            targetTransitionList[2].hasExitTime = false;
            targetTransitionList[2].AddCondition(AnimatorConditionMode.If, 1, "magic");
            targetTransitionList[3].exitTime = 0f;
            targetTransitionList[3].duration = 0f;
            targetTransitionList[3].hasExitTime = false;
            targetTransitionList[3].AddCondition(AnimatorConditionMode.IfNot, 0, "magic");

            GameObject myGameObject = new GameObject();
            GameObject shade = new GameObject("shade");
            myGameObject.AddComponent<SpriteRenderer>();
            myGameObject.GetComponent<SpriteRenderer>().sprite = MySprites;
            myGameObject.GetComponent<SpriteRenderer>().sortingOrder = 2;
            myGameObject.AddComponent<Animator>();
            myGameObject.GetComponent<Animator>().runtimeAnimatorController = Target;
            myGameObject.AddComponent<Monster>();
            myGameObject.transform.localScale = new Vector3(2,2,1);
            shade.AddComponent<SpriteRenderer>();
            shade.GetComponent<SpriteRenderer>().sortingOrder = 1;
            shade.GetComponent<SpriteRenderer>().sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Resources/Picture/shade.png");
            shade.transform.localScale = new Vector3(1,0.3f,1);
            GameObject obje = Instantiate(shade, myGameObject.transform.position, Quaternion.identity);
            obje.name = "shade";
            obje.transform.parent = myGameObject.transform;
            obje.transform.position = Vector3.zero;
            obje.transform.parent = myGameObject.transform;
            if(prefabName!="") PrefabUtility.SaveAsPrefabAsset(myGameObject, "Assets/Resources/MonsterPre/" + prefabName + ".prefab");
            if (monDB != null) {
                for(int i = 0; i < monDB.MonsterDataList.Count; i++) {
                    if(monDB.MonsterDataList[i].Name== "" || monDB.MonsterDataList[i].Name == monName) {
                        monDB.MonsterDataList[i].Name = monName;
                        monDB.MonsterDataList[i].Formula = formula;
                        monDB.MonsterDataList[i].FileName = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Resources/MonsterPre/" + prefabName + ".prefab");
                        monDB.MonsterDataList[i].Icon = null;
                        break;
                    }
                }
            }
            DestroyImmediate(myGameObject);
            DestroyImmediate(shade);

        }
    }
#endif
}
#endif