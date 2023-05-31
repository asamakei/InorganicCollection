using UnityEngine;
using System.Collections;

using UnityEditor;

#if UNITY_EDITOR
using UnityEditor.Animations;
#endif

public class CharaChip : MonoBehaviour{
#if UNITY_EDITOR
    [MenuItem("Extension/CharaMake")]
    public static void SliceCaller() {
        int x = 6;
        int y = 4;
        int SelectCount = Selection.objects.Length;
        Texture MyTexture;
        for (int i = 0; i < SelectCount; i++) {
            MyTexture = (Texture)Selection.objects[i];
            Slicer(MyTexture,x,y);
            Sprite[] MySprites;
            string SpriteFilePath = AssetDatabase.GetAssetPath(Selection.instanceIDs[i]);
            string[] PathSp = SpriteFilePath.Split(new char[] { '/','.'});
            MySprites = Resources.LoadAll<Sprite>("CharaChip/"+PathSp[PathSp.Length-2]);
            Animation(MySprites);
            makeAnimator(PathSp[PathSp.Length - 2]);
            createPrefab(PathSp[PathSp.Length - 2], MySprites);

        }
    }
    public static void Slicer(Texture TargetTexture, int x, int y) {
        int halfSquare = 16;

        SpriteMetaData[] MySheet = new SpriteMetaData[x * y];
        float width = TargetTexture.width / x;
        float height = TargetTexture.height / y;
        int Count = 0;
        string TargetPath = AssetDatabase.GetAssetPath(TargetTexture);
        TextureImporter importer = TextureImporter.GetAtPath(TargetPath) as TextureImporter;
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Multiple;
        importer.filterMode = FilterMode.Point;
        importer.spritePixelsPerUnit = halfSquare * 2;
        for (int i = y - 1; i >= 0; i--) {
            for(int j = 0; j < x; j++) {
                MySheet[Count].name = string.Format("{0}_{1}", TargetTexture.name, Count);
                MySheet[Count].rect = new Rect(width*j,height*i,width,height);
                MySheet[Count].alignment = 9;
                MySheet[Count].pivot = new Vector2(0.5f, halfSquare / height);
                Count++;
            }
        }
        importer.spritesheet = MySheet;
        AssetDatabase.ImportAsset(TargetPath,ImportAssetOptions.ForceUpdate);
    }

    public static void Animation(Sprite[] TargetSprites) {
        int[] dir = {-1, 3, 0, 9, 6, 12, 15, 18, 21 };
        for (int j = 1; j < 17; j++) {
            AnimationClip NewClip = new AnimationClip();
            NewClip.wrapMode = WrapMode.Loop;
            SerializedObject serializedClip = new SerializedObject(NewClip);
            SerializedProperty settings = serializedClip.FindProperty("m_AnimationClipSettings");
            while (settings.Next(true)) {
                if (settings.name == "m_LoopTime") break;
            }
            settings.boolValue = true;
            serializedClip.ApplyModifiedProperties();
            EditorCurveBinding curveBinding = new EditorCurveBinding();
            string[] FileNameSp = new string[2];
            FileNameSp = TargetSprites[0].name.Split('_');
            ObjectReferenceKeyframe[] Keyframes;
            if (j <= 8) {
                Keyframes = new ObjectReferenceKeyframe[4];
                for (int i = 0; i < 4; i++) {
                    int plus = 0;
                    if (i == 3) plus = 1;
                    Keyframes[i] = new ObjectReferenceKeyframe();
                    Keyframes[i].time = 1.0f / 60 * i;
                    Keyframes[i].value = System.Array.Find<Sprite>(TargetSprites, (sprite) => sprite.name.Equals(FileNameSp[0] + "_" + (dir[j] + i % 3 + plus).ToString()));
                }
            } else {
                Keyframes = new ObjectReferenceKeyframe[1];
                Keyframes[0] = new ObjectReferenceKeyframe();
                Keyframes[0].time = 0;
                Keyframes[0].value = System.Array.Find<Sprite>(TargetSprites, (sprite) => sprite.name.Equals(FileNameSp[0] + "_" + (dir[j-8] + 1).ToString()));
            }
            curveBinding.type = typeof(SpriteRenderer);
            curveBinding.path = string.Empty;
            curveBinding.propertyName = "m_Sprite";
            AnimationUtility.SetObjectReferenceCurve(NewClip, curveBinding, Keyframes);
            if(j<=8)AssetDatabase.CreateAsset(NewClip, "Assets/CharaChip/Animation/" + FileNameSp[0] + "_" + j.ToString() + ".anim");
            else AssetDatabase.CreateAsset(NewClip, "Assets/CharaChip/Animation/" + FileNameSp[0] + "_" + (j-8).ToString() + "s.anim");
        }
    }
    public static void makeAnimator(string fileName) {
        AnimationClip[] Motion = new AnimationClip[16];
        for (int i = 1; i < 9; i++) {
            Motion[i-1] = AssetDatabase.LoadAssetAtPath<AnimationClip>("Assets/CharaChip/Animation/" + fileName + "_" + i.ToString()+".anim");
        }
        for (int i = 1; i < 9; i++) {
            Motion[i+7] = AssetDatabase.LoadAssetAtPath<AnimationClip>("Assets/CharaChip/Animation/" + fileName + "_" + i.ToString() + "s.anim");
        }
        AnimatorController Target =
            AnimatorController.CreateAnimatorControllerAtPath("Assets/CharaChip/Animation/"+fileName+".controller");
        Target.AddParameter("direction",AnimatorControllerParameterType.Int);
        Target.AddParameter("moving", AnimatorControllerParameterType.Int);

        AnimatorStateMachine rootStateMachine = Target.layers[0].stateMachine;
        AnimatorState[] state = new AnimatorState[16];
        for (int i = 1; i < 17; i++) {
            state[i-1] = rootStateMachine.AddState("anim" + i.ToString(), new Vector3(100 * i, 0, 0));
            state[i-1].motion = Motion[i-1];
        }
        rootStateMachine.defaultState = state[1];
        AnimatorStateTransition[] targetTransitionList = new AnimatorStateTransition[72];
        int count = 0;
        for (int i = 1; i < 9; i++) {
            for (int j = 1; j < 9; j++) {
                if (i != j) {
                    targetTransitionList[count] = state[i-1].AddTransition(state[j-1]);
                    count++;
                }
            }
        }
        for(int i = 1; i < 9; i++) {
            targetTransitionList[count] = state[i - 1].AddTransition(state[i + 7]);
            targetTransitionList[count+1] = state[i + 7].AddTransition(state[i - 1]);
            count+=2;
        }
        for (int i = 0; i < 56; i++) {
            targetTransitionList[i].duration = 0f;
            targetTransitionList[i].hasExitTime = false;
            int myThreshold = 0;
            for (int j = 1; j < 9; j++) {
                if (targetTransitionList[i].destinationState == state[j-1]) {
                    myThreshold = j;
                    break;
                }
            }
            targetTransitionList[i].AddCondition(AnimatorConditionMode.Equals, myThreshold, "direction");
        }
        for (int i = 56; i < 72; i+=2) {
            targetTransitionList[i].duration = 0f;
            targetTransitionList[i].hasExitTime = false;
            targetTransitionList[i].AddCondition(AnimatorConditionMode.Equals, 0, "moving");
            targetTransitionList[i+1].duration = 0f;
            targetTransitionList[i+1].hasExitTime = false;
            targetTransitionList[i+1].AddCondition(AnimatorConditionMode.Equals, 1, "moving");
        }

    }

    public static void createPrefab(string fileName,Sprite[] MySprites) {
        GameObject myGameObject = new GameObject();
        GameObject shade = new GameObject("shade");
        CircleCollider2D Colli;
        myGameObject.AddComponent<SpriteRenderer>();
        myGameObject.GetComponent<SpriteRenderer>().sprite = MySprites[1];
        myGameObject.GetComponent<SpriteRenderer>().sortingOrder = 10;
        myGameObject.AddComponent<CharaMove>();
        myGameObject.AddComponent<CircleCollider2D>();
        myGameObject.GetComponent<CircleCollider2D>().offset = new Vector2(0,-0.2f);
        myGameObject.GetComponent<CircleCollider2D>().radius = 0.35f;
        Colli = myGameObject.AddComponent<CircleCollider2D>();
        Colli.radius = 0.7f;
        Colli.isTrigger = true;
        Colli.offset = new Vector2(0, -0.2f);
        myGameObject.AddComponent<MessageNPC>();
        myGameObject.AddComponent<Rigidbody2D>();
        myGameObject.GetComponent<Rigidbody2D>().collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        myGameObject.GetComponent<Rigidbody2D>().gravityScale = 0;
        myGameObject.AddComponent<Animator>();
        myGameObject.GetComponent<Animator>().runtimeAnimatorController = AssetDatabase.LoadAssetAtPath<AnimatorController>("Assets/CharaChip/Animation/" + fileName + ".controller");
        shade.AddComponent<SpriteRenderer>();
        shade.GetComponent<SpriteRenderer>().sortingOrder = 9;
        shade.GetComponent<SpriteRenderer>().sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Picture/CharaShade_8dir.png");
        GameObject obj = Instantiate(shade, myGameObject.transform.position, Quaternion.identity);
        obj.transform.localScale = new Vector3(2, 4, 1);
        obj.transform.position = new Vector3(shade.transform.position.x, shade.transform.position.y-0.42f, 0);
        obj.transform.parent = myGameObject.transform;
        PrefabUtility.SaveAsPrefabAsset(myGameObject,"Assets/CharaChip/" + fileName+".prefab");
        DestroyImmediate(myGameObject);
        DestroyImmediate(shade);
    }
#endif
}
