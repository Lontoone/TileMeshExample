using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Need to inhirt from MonoBehaviour to create Mesh object; 需要繼承MonoBehaviour才能產生GameObject
public class MapMesh
{
    public List<Tile> tiles = new List<Tile>();
    public Mesh mesh;
    public Material material;
    public Vector2Int meshOrder;
    public Transform root;
    private Vector2 rootPosition
    {
        get
        {
            if (root == null)
            {
                return Vector2.zero;
            }
            else
            { return root.position; }
        }
    }
    public Vector3 min
    {
        get
        {
            return (meshOrder - new Vector2(0.5f, 0.5f)) * TileMeshManager.MESH_SIZE + rootPosition;
        }
    }
    public Vector3 center { get { return (Vector3Int)meshOrder * TileMeshManager.MESH_SIZE + (Vector3)rootPosition; } }
    public Vector3 max { get { return (meshOrder + new Vector2(0.5f, 0.5f)) * TileMeshManager.MESH_SIZE + rootPosition; } }

    public Vector2 textureScaler
    {
        get { return new Vector2(TileMeshManager.MESH_SIZE, TileMeshManager.MESH_SIZE) / TileMeshManager.TEXTURE_SIZE; }
    }

    public int xTileCount { get { return TileMeshManager.TEXTURE_SIZE / TileMeshManager.TILE_SIZE; } }
    public int yTileCount { get { return TileMeshManager.TEXTURE_SIZE / TileMeshManager.TILE_SIZE; } }

    private Bounds bounds;

    private static readonly Vector2Int[] Range3x3 =
        new Vector2Int[] {
            new Vector2Int (-1,1),
            new Vector2Int (0,1),
            new Vector2Int (1,1),
            new Vector2Int (-1,0),
            new Vector2Int (1,0),
            new Vector2Int (-1,-1),
            new Vector2Int (0,-1),
            new Vector2Int (1,-1),
            //new Vector2Int (0,0),
        };

    public MapMesh() { }
    public MapMesh(Vector2Int _order , Transform _root)
    {
        meshOrder = _order;
        root = _root;
        //create mesh
        CreateMesh();
        material = CreateMaterial();
        CreateTiles();

        bounds.center = center;
        bounds.min = min;
        bounds.max = max;
    }

    private void CreateMesh()
    {
        Debug.Log("Create Mesh on " + meshOrder);

        float width = TileMeshManager.MESH_SIZE;
        float height = TileMeshManager.MESH_SIZE;

        mesh = new Mesh();

        var vertices = new Vector3[4];

        vertices[0] = new Vector3(0, 0, 0) + min;
        vertices[1] = new Vector3(width, 0, 0) + min;
        vertices[2] = new Vector3(0, height, 0) + min;
        vertices[3] = new Vector3(width, height, 0) + min;

        mesh.vertices = vertices;

        var tri = new int[6];

        tri[0] = 0;
        tri[1] = 2;
        tri[2] = 1;

        tri[3] = 2;
        tri[4] = 3;
        tri[5] = 1;

        mesh.triangles = tri;

        var normals = new Vector3[4];

        normals[0] = -Vector3.forward;
        normals[1] = -Vector3.forward;
        normals[2] = -Vector3.forward;
        normals[3] = -Vector3.forward;

        mesh.normals = normals;

        var uv = new Vector2[4];

        uv[0] = new Vector2(0, 0);
        uv[1] = new Vector2(1, 0);
        uv[2] = new Vector2(0, 1);
        uv[3] = new Vector2(1, 1);

        mesh.uv = uv;

    }
    private Material CreateMaterial()
    {
        Material mat = new Material(Shader.Find("Unlit/Texture"));
        Texture2D _tex = new Texture2D(TileMeshManager.TEXTURE_SIZE, TileMeshManager.TEXTURE_SIZE);
        mat.mainTexture = _tex;
        return mat;

    }

    private void CreateTiles()
    {
        //float halfTileSize = TileMeshManager.TILE_SIZE / 2;

        for (int i = 0; i < xTileCount; i++)
        {
            for (int j = 0; j < yTileCount; j++)
            {
                Tile _tile = new Tile(new Vector2Int(i, j), this);
                tiles.Add(_tile);
            }
        }

    }

    public int CheckTileCondition(Tile _centerTile)
    {
        Tile[] _jiugongge = RangeSelect(_centerTile, Range3x3);
        float res = 0;
        for (int i = 0; i < 8; i++)
        {
            if (CheckCellIsSameLayer(_centerTile, _jiugongge[i]))
            {
                int _pow = (7 - i);
                res += Mathf.Pow(2, _pow);
            }
        }
        _centerTile.condition = (int)res;
        return (int)res;
    }

    public int[] CheckNearbyTileCondition(Tile _centerTile, out Tile[] _jiugongge)
    {
        int[] res = new int[8];

        _jiugongge = RangeSelect(_centerTile, Range3x3);
        for (int i = 0; i < 8; i++)
        {
            if (_jiugongge[i] == null) { continue; }

            int _code = CheckTileCondition(_jiugongge[i]);
            res[i] = _code;
        }
        return res;
    }
    private bool CheckCellIsSameLayer(Tile _center, Tile _target)
    {
        if (_target == null || _center == null)
        {
            return false;
        }

        // Check if is same tile type?  檢查是否為同種類的tile圖片
        return _center.layer == _target.layer;
    }

    public Tile[] RangeSelect(Tile _center, Vector2Int[] _ranges)
    {
        Tile[] _res = new Tile[_ranges.Length];
        if (_center == null) { return _res; }

        //Get tiles in this range: 取得範圍內的tile
        for (int i = 0; i < _ranges.Length; i++)
        {
            //Vector2Int _cellPos = _center.localOrderPosition + _ranges[i];
            //_res[i] = GetTile(_cellPos);
            Vector2Int _cellPos = _center.worldOrderPosition + _ranges[i];
            _res[i] = GetTileByWorldOrder(_cellPos);
        }

        return _res;
    }


    //check this point is inside the bounds of this mesh
    public bool ContainsPoint(Vector2 _point)
    {
        bounds.min = min;
        bounds.max = max;
        if (bounds.Contains(_point))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public Tile GetLocalTile(Vector2Int _point)
    {
        int index = yTileCount * _point.x + _point.y;

        //if (index < 0 || index >= tiles.Count)
        if (_point.x >= xTileCount || _point.y >= yTileCount || index < 0 || _point.x < 0 || _point.y < 0)
        {
            return null;
        }
        else
        {
            return tiles[index];
        }
    }
    public Tile GetTileByWorldOrder(Vector2Int _worldOrder)
    {
        int _width = TileMeshManager.tileWidthPerMesh;
        Vector2Int _meshOrder = _worldOrder / _width;
        //負的mesh order從-1開始，例如x=-1，meshOrder=-1
        if (_worldOrder.x < 0 && _worldOrder.x % _width != 0)
        {
            _meshOrder.x -= 1;
        }
        if (_worldOrder.y < 0 && _worldOrder.y % _width != 0)
        {
            _meshOrder.y -= 1;
        }

        MapMesh _mesh = TileMeshManager.GetMeshAt(_meshOrder);
        if (_mesh != null)
        {
            Vector2Int _localOrderPos = _worldOrder - _meshOrder * TileMeshManager.tileWidthPerMesh;
            //Dobule check
            if (_worldOrder.x < 0)
                _localOrderPos.x = ((_worldOrder.x % TileMeshManager.tileWidthPerMesh) + TileMeshManager.tileWidthPerMesh) % TileMeshManager.tileWidthPerMesh;

            return _mesh.GetLocalTile(_localOrderPos);
        }
        return null;
    }

    public Tile GetLocalTile(Vector2 point)
    {
        /*
        point = new Vector2(Mathf.Clamp(point.x, min.x, max.x),
                             Mathf.Clamp(point.y, min.y, max.y));*/

        Vector2 _point = point + (Vector2)bounds.extents;
        _point = _point / textureScaler;
        //Debug.Log("clamp " + min + " " + max);


        Vector2Int _previousTileCound = new Vector2Int(
            meshOrder.x * xTileCount,
            meshOrder.y * yTileCount
        );

        Vector2Int _snapTilePosition = new Vector2Int(
          Mathf.RoundToInt(_point.x / TileMeshManager.TILE_SIZE - 0.25f),
          Mathf.RoundToInt(_point.y / TileMeshManager.TILE_SIZE - 0.25f)
         );
        Vector2Int _tilePos = _snapTilePosition - _previousTileCound;
        return GetLocalTile(_tilePos);
    }


    public void Draw(Vector2 _worldPos, MapMesh _map, TileImageCollection imageCollection, int _layer)
    {
        //Texture2D currentTex = material.mainTexture as Texture2D;
        DrawBuffer drawBuffer = new DrawBuffer();

        //center Tile
        Tile _centerTile = GetLocalTile(_worldPos);

        if (_centerTile == null) { return; }
        //set draw target Layer
        _centerTile.layer = _layer;

        drawBuffer.tiles.Add(_centerTile);

        //nearby tiles
        Tile[] _jiugongge = RangeSelect(_centerTile, Range3x3);

        drawBuffer.tiles.AddRange(_jiugongge);

        drawBuffer.CheckCondition();
        DrawBuffer[] eachMapBuffs = drawBuffer.SplitByMeshMap();

        for (int i = 0; i < eachMapBuffs.Length; i++)
        {
            Texture2D _tex = eachMapBuffs[i].mapMesh.material.mainTexture as Texture2D;

            _tex = SetPixel(imageCollection, eachMapBuffs[i].conditions.ToArray(), eachMapBuffs[i].tiles.ToArray(), _tex);
            _tex.Apply();
            eachMapBuffs[i].mapMesh.material.mainTexture = _tex;
        }

    }
    private class DrawBuffer
    {
        public MapMesh mapMesh;
        public List<int> conditions = new List<int>();
        public List<Tile> tiles = new List<Tile>();
        public DrawBuffer[] SplitByMeshMap()
        {
            List<DrawBuffer> res = new List<DrawBuffer>();
            for (int i = 0; i < tiles.Count; i++)
            {
                if (tiles[i] == null) { continue; }

                DrawBuffer _container = res.Find(x => x.mapMesh == tiles[i].meshMap);
                if (_container != null)
                {
                    _container.tiles.Add(tiles[i]);
                    _container.conditions.Add(tiles[i].condition);
                }
                else
                {
                    _container = new DrawBuffer();
                    _container.mapMesh = tiles[i].meshMap;
                    _container.tiles.Add(tiles[i]);
                    _container.conditions.Add(tiles[i].condition);
                    res.Add(_container);
                }
            }
            return res.ToArray();
        }

        public void CheckCondition()
        {
            for (int i = 0; i < tiles.Count; i++)
            {
                if (tiles[i] == null)
                {
                    conditions.Add(0);
                    continue;
                }
                int _conditionCode = tiles[i].meshMap.CheckTileCondition(tiles[i]);
                conditions.Add(_conditionCode);
            }
        }

    }

    private Texture2D SetPixel(TileImageCollection imageCollection, int[] _conditionCodes, Tile[] _tiles, Texture2D _currentTex)
    {
        for (int i = 0; i < _conditionCodes.Length; i++)
        {
            if (_tiles[i] == null || _tiles[i].layer == 0)
            {
                continue;
            }
            _currentTex = SetPixel(imageCollection, _conditionCodes[i], _tiles[i], _currentTex);
        }
        return _currentTex;
    }
    public Texture2D SetPixel(TileImageCollection imageCollection, int _conditionCode, Tile _tile, Texture2D _currentTex)
    {
        if (_tile == null || _tile.layer == 0) { return _currentTex; }

        //Debug.Log(_tile.localOrderPosition + " set " + _conditionCode + " name " + imageCollection.GetSprite(_conditionCode).name);
        Texture2D _tileImage = imageCollection.GetSprite(_conditionCode).texture;
        Vector2 _scaler = new Vector2((float)_tileImage.width / TileMeshManager.TILE_SIZE,
                                      (float)_tileImage.height / TileMeshManager.TILE_SIZE);

        Texture2D destTex = _currentTex;

        Color[] pixs = new Color[TileMeshManager.TILE_SIZE * TileMeshManager.TILE_SIZE];

        for (var y = 0; y < TileMeshManager.TILE_SIZE; y++)
        {
            for (var x = 0; x < TileMeshManager.TILE_SIZE; x++)
            {

                float _u = x * _scaler.x / _tileImage.width;
                float _v = y * _scaler.y / _tileImage.height;
                pixs[y * TileMeshManager.TILE_SIZE + x] = _tileImage.GetPixelBilinear(_u, _v);
            }
        }

        destTex.SetPixels(
                        _tile.localOrderPosition.x * TileMeshManager.TILE_SIZE,
                        _tile.localOrderPosition.y * TileMeshManager.TILE_SIZE,
                        TileMeshManager.TILE_SIZE,
                        TileMeshManager.TILE_SIZE,
                        pixs);

        return destTex;
    }

    /*
    private void ApplyStylePixel(TileImageCollection imageCollection, int _conditionCode, Tile _tile)
    {
        if (_tile == null) { return; }

        Texture2D _tileImage = imageCollection.GetSprite(_conditionCode).texture;
        Vector2 _scaler = new Vector2((float)_tileImage.width / TileMeshManager.TILE_SIZE,
                                      (float)_tileImage.height / TileMeshManager.TILE_SIZE);

        Texture2D destTex = material.mainTexture as Texture2D;

        Color[] pixs = new Color[TileMeshManager.TILE_SIZE * TileMeshManager.TILE_SIZE];

        for (var y = 0; y < TileMeshManager.TILE_SIZE; y++)
        {
            for (var x = 0; x < TileMeshManager.TILE_SIZE; x++)
            {

                float _u = x * _scaler.x / _tileImage.width;
                float _v = y * _scaler.y / _tileImage.height;
                pixs[y * TileMeshManager.TILE_SIZE + x] = _tileImage.GetPixelBilinear(_u, _v);
            }
        }

        destTex.SetPixels(
                        _tile.orderPosition.x * TileMeshManager.TILE_SIZE,
                        _tile.orderPosition.y * TileMeshManager.TILE_SIZE,
                        TileMeshManager.TILE_SIZE,
                        TileMeshManager.TILE_SIZE,
                        pixs);

        destTex.Apply();
        material.mainTexture = destTex;
    }*/


}
