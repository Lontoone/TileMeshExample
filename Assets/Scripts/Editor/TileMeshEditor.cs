using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


/// <summary>
/// This is used to find the mouse position when it's over a SceneView.
/// Used by tools that are menu invoked.
/// </summary>
[InitializeOnLoad]
public class TileMeshEditor : Editor
{

    public bool isDrawing = false;
    //Test:
    public TileImageCollection imageCollection;
    private Tile _previousTile, _currentSelectedTile;

    public int paintLayer = 1;
    public TileMeshEditor() { }
    public TileMeshEditor(TileImageCollection _collection)
    {
        imageCollection = _collection;
    }

    private void OnEnable()
    {
        //TileMeshManager.imageCollection = imageCollection;
    }

    private void OnSceneGUI()
    {
        // This will have scene events including mouse down on scenes objects
        Event cur = Event.current;
        Debug.Log(cur);
        Vector2 _mousePosition = cur.mousePosition;
        
    }
}
