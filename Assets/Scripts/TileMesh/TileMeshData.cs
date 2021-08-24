using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "TileMap/Mesh Data")]
[System.Serializable]
public class TileMeshData : ScriptableObject
{
    public Vector2 rootPosition;
    public List<MapMesh> mapMeshes = new List<MapMesh>();


    [MenuItem("Assets/Tile Map/My Scriptable Object")]
    public void CreateAsset()
    {
        TileMeshData asset = ScriptableObject.CreateInstance<TileMeshData>();

        AssetDatabase.CreateAsset(asset, "Assets/TileMap.asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = asset;

        //return asset;
    }
}
