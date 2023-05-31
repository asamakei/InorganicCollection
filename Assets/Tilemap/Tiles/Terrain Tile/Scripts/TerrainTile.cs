using System;
using System.Collections;
using System.Collections.Generic;


#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

namespace UnityEngine.Tilemaps
{
	[Serializable]
	[CreateAssetMenu(fileName = "New Terrain Tile", menuName = "Tiles/Terrain Tile")]
	public class TerrainTile : TileBase
	{
		[SerializeField]
		public Sprite[] m_Sprites;
        public bool spriteFlag=true;

		public override void RefreshTile(Vector3Int location, ITilemap tileMap)
		{
			for (int yd = -1; yd <= 1; yd++)
				for (int xd = -1; xd <= 1; xd++)
				{
					Vector3Int position = new Vector3Int(location.x + xd, location.y + yd, location.z);
					if (TileValue(tileMap, position))
						tileMap.RefreshTile(position);
				}
		}

		public override void GetTileData(Vector3Int location, ITilemap tileMap, ref TileData tileData)
		{
			UpdateTile(location, tileMap, ref tileData);
		}

		private void UpdateTile(Vector3Int location, ITilemap tileMap, ref TileData tileData)
		{
			tileData.transform = Matrix4x4.identity;
			tileData.color = Color.white;

			int mask = TileValue(tileMap, location + new Vector3Int(0, 1, 0)) ? 1 : 0;
			mask += TileValue(tileMap, location + new Vector3Int(1, 1, 0)) ? 2 : 0;
			mask += TileValue(tileMap, location + new Vector3Int(1, 0, 0)) ? 4 : 0;
			mask += TileValue(tileMap, location + new Vector3Int(1, -1, 0)) ? 8 : 0;
			mask += TileValue(tileMap, location + new Vector3Int(0, -1, 0)) ? 16 : 0;
			mask += TileValue(tileMap, location + new Vector3Int(-1, -1, 0)) ? 32 : 0;
			mask += TileValue(tileMap, location + new Vector3Int(-1, 0, 0)) ? 64 : 0;
			mask += TileValue(tileMap, location + new Vector3Int(-1, 1, 0)) ? 128 : 0;

			byte original = (byte)mask;
            if ((original | 254) < 255) { mask = mask & 125; }
			if ((original | 251) < 255) { mask = mask & 245; }
			if ((original | 239) < 255) { mask = mask & 215; }
			if ((original | 191) < 255) { mask = mask & 95; }

			int index = GetIndex((byte)mask);
			if (index >= 0 && index < m_Sprites.Length && TileValue(tileMap, location))
			{
                int rotation = GetTransform((byte)mask);

				tileData.sprite = m_Sprites[index+15*rotation];
				tileData.transform = Matrix4x4.identity;
				tileData.color = Color.white;
				tileData.flags = TileFlags.LockTransform | TileFlags.LockColor;

                if (spriteFlag) tileData.colliderType = Tile.ColliderType.Sprite;
                else tileData.colliderType = Tile.ColliderType.None;
            }
		}

		private bool TileValue(ITilemap tileMap, Vector3Int position)
		{
			TileBase tile = tileMap.GetTile(position);
			return (tile != null && tile == this);
		}

		private int GetIndex(byte mask)
		{
			switch (mask)
			{
				case 0: return 0;
				case 1:
				case 4:
				case 16:
				case 64: return 1;
				case 5:
				case 20:
				case 80:
				case 65: return 2;
				case 7:
				case 28:
				case 112:
				case 193: return 3;
				case 17:
				case 68: return 4;
				case 21:
				case 84:
				case 81:
				case 69: return 5;
				case 23:
				case 92:
				case 113:
				case 197: return 6;
				case 29:
				case 116:
				case 209:
				case 71: return 7;
				case 31:
				case 124:
				case 241:
				case 199: return 8;
				case 85: return 9;
				case 87:
				case 93:
				case 117:
				case 213: return 10;
				case 95:
				case 125:
				case 245:
				case 215: return 11;
				case 119:
				case 221: return 12;
				case 127:
				case 253:
				case 247:
				case 223: return 13;
				case 255: return 14;
			}
			return -1;
		}

		private int GetTransform(byte mask)
		{
			switch (mask)
			{
				case 4:
				case 20:
				case 28:
				case 68:
				case 84:
				case 92:
				case 116:
				case 124:
				case 93:
				case 125:
				case 221:
				case 253:
					return 1;
				case 16:
				case 80:
				case 112:
				case 81:
				case 113:
				case 209:
				case 241:
				case 117:
				case 245:
				case 247:
					return 2;
				case 64:
				case 65:
				case 193:
				case 69:
				case 197:
				case 71:
				case 199:
				case 213:
				case 215:
				case 223:
					return 3;
			}
			return 0;
		}
	}

#if UNITY_EDITOR
	[CustomEditor(typeof(TerrainTile))]
	public class TerrainTileEditor : Editor
	{
		private TerrainTile tile { get { return (target as TerrainTile); } }

		public void OnEnable()
		{
			if (tile.m_Sprites == null || tile.m_Sprites.Length != 60)
			{
				tile.m_Sprites = new Sprite[60];
				EditorUtility.SetDirty(tile);
			}
		}


		public override void OnInspectorGUI()
		{
			EditorGUILayout.LabelField("Place sprites shown based on the contents of the sprite.");
			EditorGUILayout.Space();

			float oldLabelWidth = EditorGUIUtility.labelWidth;
			EditorGUIUtility.labelWidth = 210;

			EditorGUI.BeginChangeCheck();
            tile.spriteFlag = EditorGUILayout.ToggleLeft("Sprite", tile.spriteFlag);
            tile.m_Sprites[0] = (Sprite) EditorGUILayout.ObjectField("Filled", tile.m_Sprites[0], typeof(Sprite), false, null);
			tile.m_Sprites[1] = (Sprite) EditorGUILayout.ObjectField("Three Sides", tile.m_Sprites[1], typeof(Sprite), false, null);
			tile.m_Sprites[2] = (Sprite) EditorGUILayout.ObjectField("Two Sides and One Corner", tile.m_Sprites[2], typeof(Sprite), false, null);
			tile.m_Sprites[3] = (Sprite) EditorGUILayout.ObjectField("Two Adjacent Sides", tile.m_Sprites[3], typeof(Sprite), false, null);
			tile.m_Sprites[4] = (Sprite) EditorGUILayout.ObjectField("Two Opposite Sides", tile.m_Sprites[4], typeof(Sprite), false, null);
			tile.m_Sprites[5] = (Sprite) EditorGUILayout.ObjectField("One Side and Two Corners", tile.m_Sprites[5], typeof(Sprite), false, null);
			tile.m_Sprites[6] = (Sprite) EditorGUILayout.ObjectField("One Side and One Lower Corner", tile.m_Sprites[6], typeof(Sprite), false, null);
			tile.m_Sprites[7] = (Sprite) EditorGUILayout.ObjectField("One Side and One Upper Corner", tile.m_Sprites[7], typeof(Sprite), false, null);
			tile.m_Sprites[8] = (Sprite) EditorGUILayout.ObjectField("One Side", tile.m_Sprites[8], typeof(Sprite), false, null);
			tile.m_Sprites[9] = (Sprite) EditorGUILayout.ObjectField("Four Corners", tile.m_Sprites[9], typeof(Sprite), false, null);
			tile.m_Sprites[10] = (Sprite) EditorGUILayout.ObjectField("Three Corners", tile.m_Sprites[10], typeof(Sprite), false, null);
			tile.m_Sprites[11] = (Sprite) EditorGUILayout.ObjectField("Two Adjacent Corners", tile.m_Sprites[11], typeof(Sprite), false, null);
			tile.m_Sprites[12] = (Sprite) EditorGUILayout.ObjectField("Two Opposite Corners", tile.m_Sprites[12], typeof(Sprite), false, null);
			tile.m_Sprites[13] = (Sprite) EditorGUILayout.ObjectField("One Corner", tile.m_Sprites[13], typeof(Sprite), false, null);
			tile.m_Sprites[14] = (Sprite) EditorGUILayout.ObjectField("Empty", tile.m_Sprites[14], typeof(Sprite), false, null);

            tile.m_Sprites[15] = (Sprite)EditorGUILayout.ObjectField("Filled", tile.m_Sprites[15], typeof(Sprite), false, null);
            tile.m_Sprites[16] = (Sprite)EditorGUILayout.ObjectField("Three Sides", tile.m_Sprites[16], typeof(Sprite), false, null);
            tile.m_Sprites[17] = (Sprite)EditorGUILayout.ObjectField("Two Sides and One Corner", tile.m_Sprites[17], typeof(Sprite), false, null);
            tile.m_Sprites[18] = (Sprite)EditorGUILayout.ObjectField("Two Adjacent Sides", tile.m_Sprites[18], typeof(Sprite), false, null);
            tile.m_Sprites[19] = (Sprite)EditorGUILayout.ObjectField("Two Opposite Sides", tile.m_Sprites[19], typeof(Sprite), false, null);
            tile.m_Sprites[20] = (Sprite)EditorGUILayout.ObjectField("One Side and Two Corners", tile.m_Sprites[20], typeof(Sprite), false, null);
            tile.m_Sprites[21] = (Sprite)EditorGUILayout.ObjectField("One Side and One Lower Corner", tile.m_Sprites[21], typeof(Sprite), false, null);
            tile.m_Sprites[22] = (Sprite)EditorGUILayout.ObjectField("One Side and One Upper Corner", tile.m_Sprites[22], typeof(Sprite), false, null);
            tile.m_Sprites[23] = (Sprite)EditorGUILayout.ObjectField("One Side", tile.m_Sprites[23], typeof(Sprite), false, null);
            tile.m_Sprites[24] = (Sprite)EditorGUILayout.ObjectField("Four Corners", tile.m_Sprites[24], typeof(Sprite), false, null);
            tile.m_Sprites[25] = (Sprite)EditorGUILayout.ObjectField("Three Corners", tile.m_Sprites[25], typeof(Sprite), false, null);
            tile.m_Sprites[26] = (Sprite)EditorGUILayout.ObjectField("Two Adjacent Corners", tile.m_Sprites[26], typeof(Sprite), false, null);
            tile.m_Sprites[27] = (Sprite)EditorGUILayout.ObjectField("Two Opposite Corners", tile.m_Sprites[27], typeof(Sprite), false, null);
            tile.m_Sprites[28] = (Sprite)EditorGUILayout.ObjectField("One Corner", tile.m_Sprites[28], typeof(Sprite), false, null);
            tile.m_Sprites[29] = (Sprite)EditorGUILayout.ObjectField("Empty", tile.m_Sprites[29], typeof(Sprite), false, null);

            tile.m_Sprites[30] = (Sprite)EditorGUILayout.ObjectField("Filled", tile.m_Sprites[30], typeof(Sprite), false, null);
            tile.m_Sprites[31] = (Sprite)EditorGUILayout.ObjectField("Three Sides", tile.m_Sprites[31], typeof(Sprite), false, null);
            tile.m_Sprites[32] = (Sprite)EditorGUILayout.ObjectField("Two Sides and One Corner", tile.m_Sprites[32], typeof(Sprite), false, null);
            tile.m_Sprites[33] = (Sprite)EditorGUILayout.ObjectField("Two Adjacent Sides", tile.m_Sprites[33], typeof(Sprite), false, null);
            tile.m_Sprites[34] = (Sprite)EditorGUILayout.ObjectField("Two Opposite Sides", tile.m_Sprites[34], typeof(Sprite), false, null);
            tile.m_Sprites[35] = (Sprite)EditorGUILayout.ObjectField("One Side and Two Corners", tile.m_Sprites[35], typeof(Sprite), false, null);
            tile.m_Sprites[36] = (Sprite)EditorGUILayout.ObjectField("One Side and One Lower Corner", tile.m_Sprites[36], typeof(Sprite), false, null);
            tile.m_Sprites[37] = (Sprite)EditorGUILayout.ObjectField("One Side and One Upper Corner", tile.m_Sprites[37], typeof(Sprite), false, null);
            tile.m_Sprites[38] = (Sprite)EditorGUILayout.ObjectField("One Side", tile.m_Sprites[38], typeof(Sprite), false, null);
            tile.m_Sprites[39] = (Sprite)EditorGUILayout.ObjectField("Four Corners", tile.m_Sprites[39], typeof(Sprite), false, null);
            tile.m_Sprites[40] = (Sprite)EditorGUILayout.ObjectField("Three Corners", tile.m_Sprites[40], typeof(Sprite), false, null);
            tile.m_Sprites[41] = (Sprite)EditorGUILayout.ObjectField("Two Adjacent Corners", tile.m_Sprites[41], typeof(Sprite), false, null);
            tile.m_Sprites[42] = (Sprite)EditorGUILayout.ObjectField("Two Opposite Corners", tile.m_Sprites[42], typeof(Sprite), false, null);
            tile.m_Sprites[43] = (Sprite)EditorGUILayout.ObjectField("One Corner", tile.m_Sprites[43], typeof(Sprite), false, null);
            tile.m_Sprites[44] = (Sprite)EditorGUILayout.ObjectField("Empty", tile.m_Sprites[44], typeof(Sprite), false, null);

            tile.m_Sprites[45] = (Sprite)EditorGUILayout.ObjectField("Filled", tile.m_Sprites[45], typeof(Sprite), false, null);
            tile.m_Sprites[46] = (Sprite)EditorGUILayout.ObjectField("Three Sides", tile.m_Sprites[46], typeof(Sprite), false, null);
            tile.m_Sprites[47] = (Sprite)EditorGUILayout.ObjectField("Two Sides and One Corner", tile.m_Sprites[47], typeof(Sprite), false, null);
            tile.m_Sprites[48] = (Sprite)EditorGUILayout.ObjectField("Two Adjacent Sides", tile.m_Sprites[48], typeof(Sprite), false, null);
            tile.m_Sprites[49] = (Sprite)EditorGUILayout.ObjectField("Two Opposite Sides", tile.m_Sprites[49], typeof(Sprite), false, null);
            tile.m_Sprites[50] = (Sprite)EditorGUILayout.ObjectField("One Side and Two Corners", tile.m_Sprites[50], typeof(Sprite), false, null);
            tile.m_Sprites[51] = (Sprite)EditorGUILayout.ObjectField("One Side and One Lower Corner", tile.m_Sprites[51], typeof(Sprite), false, null);
            tile.m_Sprites[52] = (Sprite)EditorGUILayout.ObjectField("One Side and One Upper Corner", tile.m_Sprites[52], typeof(Sprite), false, null);
            tile.m_Sprites[53] = (Sprite)EditorGUILayout.ObjectField("One Side", tile.m_Sprites[53], typeof(Sprite), false, null);
            tile.m_Sprites[54] = (Sprite)EditorGUILayout.ObjectField("Four Corners", tile.m_Sprites[54], typeof(Sprite), false, null);
            tile.m_Sprites[55] = (Sprite)EditorGUILayout.ObjectField("Three Corners", tile.m_Sprites[55], typeof(Sprite), false, null);
            tile.m_Sprites[56] = (Sprite)EditorGUILayout.ObjectField("Two Adjacent Corners", tile.m_Sprites[56], typeof(Sprite), false, null);
            tile.m_Sprites[57] = (Sprite)EditorGUILayout.ObjectField("Two Opposite Corners", tile.m_Sprites[57], typeof(Sprite), false, null);
            tile.m_Sprites[58] = (Sprite)EditorGUILayout.ObjectField("One Corner", tile.m_Sprites[58], typeof(Sprite), false, null);
            tile.m_Sprites[59] = (Sprite)EditorGUILayout.ObjectField("Empty", tile.m_Sprites[59], typeof(Sprite), false, null);
            if (EditorGUI.EndChangeCheck())
				EditorUtility.SetDirty(tile);

			EditorGUIUtility.labelWidth = oldLabelWidth;
		}
	}
#endif
}
