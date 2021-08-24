using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TileMeshObject : MonoBehaviour
{
    public TileMeshData data;
    public void Init()
    {
        data = new TileMeshData();
        data.CreateAsset();
    }
    public void Save()
    {
        //TODO: called when edited
        data.rootPosition = transform.position;
    }

    //Call from editor
    public void LoadTileMap()
    {
        //TODO...Load
    }
}
