using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMeshManager : MonoBehaviour
{
    public const int TEXTURE_SIZE = 512;
    public const int TILE_SIZE = 64;
    public const int MESH_SIZE = 5;
    public static int tileWidthPerMesh { get { return TEXTURE_SIZE / TILE_SIZE; } }

    //public static TileImageCollection imageCollection;

    private static List<MapMesh> mapMeshes = new List<MapMesh>();  //private
    private static TileMeshObject meshRoot;
    private static Transform meshParent
    {
        get
        {
            if (meshRoot == null)
            {
                meshRoot = FindObjectOfType<TileMeshObject>();
                if (meshRoot == null)
                {
                    meshRoot = new GameObject().AddComponent<TileMeshObject>();
                    meshRoot.Init();
                    meshRoot.name = "Tile Mesh";
                    TileMeshManager.mapMeshes.Clear();
                }
            }
            return meshRoot.transform;
        }
    }

    public static void Draw(Vector2 _worldPos, int _layer, TileImageCollection imageCollection)
    {
        //get the bounds of _worldPos 取得包含此點的map mesh
        MapMesh _map = mapMeshes.Find(x => x.ContainsPoint(_worldPos));

        Vector2 _world2LocalPos = meshParent.InverseTransformPoint(_worldPos);

        if (_map == null)
        {
            //_map = CreateMapMesh(_worldPos);
            _map = CreateMapMesh(_world2LocalPos);
        }

        _map.Draw(_world2LocalPos, _map, imageCollection, _layer);

    }
    public static Tile GetTile(Vector2 _worldPos)
    {
        MapMesh _map = mapMeshes.Find(x => x.ContainsPoint(_worldPos));
        if (_map != null)
        {
            //Debug.Log(" contains "+ _map.meshOrder+" "+ _worldPos);
            return _map.GetLocalTile(_worldPos);
        }
        return null;
    }
    public static MapMesh GetMeshAt(Vector2Int _order)
    {
        return mapMeshes.Find(x => x.meshOrder == _order);
    }

    private static MapMesh CreateMapMesh(Vector2 _point)
    {
        //get the xy order for the mesh to cover the point:
        // x /( size/2) - x / size
        /*
        Vector2Int _halfSnapMeshPosition = new Vector2Int(
            (int)_point.x / (MESH_SIZE / 2),
            (int)_point.y / (MESH_SIZE / 2)
            );*/

        Vector2Int _snapMeshPosition = new Vector2Int(
           Mathf.RoundToInt(_point.x / (MESH_SIZE)),
           Mathf.RoundToInt(_point.y / (MESH_SIZE))
          );

        //create Mesh Parent:
        GameObject meshObj = new GameObject();

        meshObj.hideFlags = HideFlags.HideInHierarchy;
        meshObj.transform.SetParent(meshParent.transform);
        MapMesh _newMesh = new MapMesh(_snapMeshPosition, meshParent.transform);

        MeshRenderer mr = meshObj.AddComponent<MeshRenderer>();
        MeshFilter mf = meshObj.AddComponent<MeshFilter>();
        mf.mesh = _newMesh.mesh;
        mr.material = _newMesh.material;

        mapMeshes.Add(_newMesh);

        return _newMesh;
    }
}
