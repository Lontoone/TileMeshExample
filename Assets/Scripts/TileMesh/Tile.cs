using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Tile
{
    public Vector2Int localOrderPosition;
    public Vector2Int worldOrderPosition;
    public Vector2 worldPosition
    {
        get
        {
            /*
            float scaler = (float)TileMeshManager.MESH_SIZE / TileMeshManager.TEXTURE_SIZE;
            return new Vector2(orderPosition.x + 0.5f, orderPosition.y + 0.5f) * TileMeshManager.TILE_SIZE * scaler + (Vector2)meshMap.min;*/
            return Tile.GetWorldPosition(localOrderPosition, meshMap);
        }
    }
    public int layer = 0;
    public int condition = 0;
    private Bounds bounds;
    public MapMesh meshMap;


    public Tile() { }
    public Tile(Vector2Int _pos, MapMesh _mapMesh)
    {
        localOrderPosition = _pos;      
        worldOrderPosition = new Vector2Int(_pos.x + _mapMesh.meshOrder.x * _mapMesh.xTileCount,
                                            _pos.y + _mapMesh.meshOrder.y * _mapMesh.yTileCount);

        meshMap = _mapMesh;
        bounds.min = minWorldPos;
        bounds.max = maxWorldPos;
    }

    public Vector2 minWorldPos
    {
        get
        {
            float scaler = (float)TileMeshManager.MESH_SIZE / TileMeshManager.TEXTURE_SIZE;
            return new Vector2(
                (localOrderPosition.x - 0.5f) * TileMeshManager.TILE_SIZE,
                (localOrderPosition.y - 0.5f) * TileMeshManager.TILE_SIZE
                ) * scaler + (Vector2)meshMap.mesh.bounds.min;
        }
    }
    public Vector2 maxWorldPos
    {
        get
        {
            float scaler = (float)TileMeshManager.MESH_SIZE / TileMeshManager.TEXTURE_SIZE;
            return new Vector2(
                (localOrderPosition.x + 0.5f) * TileMeshManager.TILE_SIZE,
                (localOrderPosition.y + 0.5f) * TileMeshManager.TILE_SIZE
                ) * scaler + (Vector2)meshMap.mesh.bounds.min;
        }
    }

    public bool ContainsPoint(Vector2 _worldPos)
    {
        return bounds.Contains(_worldPos);
    }

    public static Vector2 GetWorldPosition(Vector2Int _orderPos, MapMesh _map)
    {
        float scaler = (float)TileMeshManager.MESH_SIZE / TileMeshManager.TEXTURE_SIZE;

        return new Vector2(_orderPos.x + 0.5f, _orderPos.y + 0.5f) * TileMeshManager.TILE_SIZE * scaler + (Vector2)_map.min;
    }
    public static Vector2 GetWorldPosition(Vector2Int _worldOrderPosp)
    {
        float scaler = (float)TileMeshManager.MESH_SIZE / TileMeshManager.TEXTURE_SIZE;

        return new Vector2(_worldOrderPosp.x + 0.5f, _worldOrderPosp.y + 0.5f) * TileMeshManager.TILE_SIZE * 0.5f * scaler;
    }

    //public Bounds bounds;
}
