using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DEMO : MonoBehaviour
{
    //Test:
    public TileImageCollection imageCollection;
    private Tile _previousTile, _currentSelectedTile;
    private Camera _cam;

    public int paintLayer = 1;

    public void Start()
    {
        //temp
        //TileMeshManager.imageCollection = imageCollection;
        _cam = Camera.main;
    }
    public void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Vector2 _worldMousePos = _cam.ScreenToWorldPoint(Input.mousePosition);
            _currentSelectedTile = TileMeshManager.GetTile(_worldMousePos);
            if (_currentSelectedTile == _previousTile && _previousTile != null)
            {
                return;
            }
            else
            {
                TileMeshManager.Draw(_worldMousePos, paintLayer,imageCollection);
                _previousTile = _currentSelectedTile;
            }
        }
    }

    private void FixedUpdate()
    {

    }

    public void OnDrawGizmos()
    {
            /*
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.red;
        //Gizemos
        for (int i = 0; i < TileMeshManager.mapMeshes.Count; i++)
        {
            Handles.Label(TileMeshManager.mapMeshes[i].center, TileMeshManager.mapMeshes[i].meshOrder + " \n"+TileMeshManager.mapMeshes[i].min + " \n" + TileMeshManager.mapMeshes[i].max, style);
                for (int j = 0; j < TileMeshManager.mapMeshes[i].tiles.Count; j++)
                {
                    Tile t = TileMeshManager.mapMeshes[i].tiles[j];
                    //Handles.Label(t.worldPosition, t.localOrderPosition + " \n" + t.condition + " \n" + t.layer, style);
                    Handles.Label(t.worldPosition, t.localOrderPosition + " \n" + t.condition + " \n" + t.worldOrderPosition, style);
            }
        }*/
    }
}
