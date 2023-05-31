using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEngine.Tilemaps;

public class AutoTileMake : MonoBehaviour {
#if UNITY_EDITOR
    [MenuItem("Extension/AutoTileMake")]
    public static void TileMake() {
        int tileSize = 32;
        byte[] pngData;
        SpriteMetaData[] MySheet = new SpriteMetaData[15];
        Texture2D mainTexture = new Texture2D(tileSize, tileSize * 15);
        Texture2D sourceTexture;
        int SelectCount = Selection.objects.Length;
        string[] FileNameSp = new string[2];
        string FileName;
        string TargetPath;

        string spriteName;
        TextureImporter importer;
        TerrainTile tile;

        for (int i = 0; i < SelectCount; i++) {
            for (int l = 0; l < 4; l++) {
                sourceTexture = (Texture2D)Selection.objects[i];
                FileNameSp = sourceTexture.name.Split('.');
                if (sourceTexture.height == tileSize * 5 && sourceTexture.width == tileSize) {
                    for (int k = l * tileSize * 15; k < tileSize * 15*(1+l); k++) {
                        for (int j = 0; j < tileSize; j++) {
                            mainTexture.SetPixel(j, k, sourceTexture.GetPixel(j, position(j, k, tileSize)));
                        }
                    }
                }
                TargetPath = "Assets/Resources/MapChip/" + FileNameSp[0] + "_Auto" + l.ToString() + ".png";
                pngData = mainTexture.EncodeToPNG();
                File.WriteAllBytes(TargetPath, pngData);
            }
            AssetDatabase.Refresh();

            tile = new TerrainTile();

            for (int j = 0; j < 4; j++) {
                TargetPath = "Assets/Resources/MapChip/" + FileNameSp[0] + "_Auto" + j.ToString() + ".png";
                sourceTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(TargetPath);
                FileName = sourceTexture.name;
                importer = TextureImporter.GetAtPath(TargetPath) as TextureImporter;
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Multiple;
                importer.filterMode = FilterMode.Point;
                importer.spritePixelsPerUnit = tileSize;
                for (int k = 0; k < 15; k++) {
                    MySheet[k].name = string.Format("{0}_{1}", sourceTexture.name, j*15+k);
                    MySheet[k].rect = new Rect(0, tileSize * (14 - k), tileSize, tileSize);
                    MySheet[k].alignment = 0;
                }
                importer.spritesheet = MySheet;
                AssetDatabase.ImportAsset(TargetPath, ImportAssetOptions.ForceUpdate);
            }
            Sprite[] sprites = Resources.LoadAll<Sprite>("MapChip/" + FileNameSp[0] + "_Auto0");
            Sprite[] allSprites = new Sprite[60];
            for (int j = 0; j < 59; j++) {
                if (j % 15 == 0 && j>0) {
                    sprites = Resources.LoadAll<Sprite>("MapChip/"+ FileNameSp[0] + "_Auto" + ((j - j % 15) / 15).ToString());
                }
                spriteName = FileNameSp[0] + "_Auto" + ((j - j % 15) / 15).ToString() + "_" + j.ToString();
                allSprites[j] = System.Array.Find<Sprite>(sprites, (sprite) => sprite.name.Equals(spriteName));

            }
            tile.m_Sprites = allSprites;
            AssetDatabase.CreateAsset(tile, "Assets/Resources/MapChip/" + FileNameSp[0] + ".asset");

        }

    }

    public static int position(int x, int y, int tileSize) {

        int Y,sy,returnX = 0;
        int[] m = new int[4];

        int[,] matrix = new int[,] {
            {0,0,0,0 },
            {0,0,1,0 },
            {0,1,1,0 },
            {1,0,1,0 },
            {1,1,1,0 },
            {1,1,1,1 },
            {3,0,3,0 },
            {3,0,3,1 },
            {3,1,3,0 },
            {3,1,3,1 },
            {3,3,3,3 },
            {4,2,3,0 },
            {4,2,3,1 },
            {4,4,3,3 },
            {4,4,4,4 },
        };
        
        Y = y % tileSize;
        y = (y - Y) / tileSize;
        sy = (y - y % 15) / 15;
        y %= 15;

        if (x < tileSize / 2 && Y < tileSize / 2) returnX = 0;
        else if (x >= tileSize / 2 && Y < tileSize / 2) returnX = 1;
        else if (x < tileSize / 2 && Y >= tileSize / 2) returnX = 2;
        else if (x >= tileSize / 2 && Y >= tileSize / 2) returnX = 3;


        if (sy == 1) {
            m[0] = matrix[y, 1];
            m[1] = matrix[y, 3];
            m[2] = matrix[y, 0];
            m[3] = matrix[y, 2];
            if (m[returnX] == 3) m[returnX] = 2;
            else if (m[returnX] == 2) m[returnX] = 3;
        } else if (sy == 2) {
            m[0] = matrix[y, 3];
            m[1] = matrix[y, 2];
            m[2] = matrix[y, 1];
            m[3] = matrix[y, 0];
        } else if (sy == 3) {
            m[0] = matrix[y, 2];
            m[1] = matrix[y, 0];
            m[2] = matrix[y, 3];
            m[3] = matrix[y, 1];
            if (m[returnX] == 3) m[returnX] = 2;
            else if (m[returnX] == 2) m[returnX] = 3;
        } else {
            m[0] = matrix[y, 0];
            m[1] = matrix[y, 1];
            m[2] = matrix[y, 2];
            m[3] = matrix[y, 3];
        }

        return m[returnX] * tileSize + Y;
    }

    static void Swap<T>(ref T a, ref T b) {
        var t = a;
        a = b;
        b = t;
    }
#endif
}

