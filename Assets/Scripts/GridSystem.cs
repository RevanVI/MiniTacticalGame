using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridSystem : MonoBehaviour
{
    public static GridSystem Instance;
    public Tilemap CurrentTilemap;
    public Tilemap Movemap;
    public Camera CurrentCamera;
    public Tile MoveTile;

    public PlayerController CurCharacter;

    private bool _blockClick;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("GridSystem object already exists");
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        _blockClick = false;
        CurCharacter.OnMoveEnded.AddListener(EnableClick);
        Debug.Log($"Tilemap data:\n ");
        Debug.Log($"Bounds: ({CurrentTilemap.cellBounds.x}, {CurrentTilemap.cellBounds.x})\n");
        Debug.Log($"Origin: ({CurrentTilemap.origin.x}, {CurrentTilemap.origin.x})\n");
        Debug.Log($"Size : {CurrentTilemap.size})");

        CurCharacter._coords = new Vector2Int(-3, 0);
        PrintMoveMap(3, new Vector3Int(CurCharacter._coords.x, CurCharacter._coords.y, 0));
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !_blockClick)
        {
            _blockClick = true;
            Vector3Int cellPosition = GetTilemapCoordsFromScreen(CurrentTilemap, Input.mousePosition);
            PrintTileInfo(cellPosition);
            List<Vector3Int> moveMap = GetMoveMap(3, new Vector3Int(CurCharacter._coords.x, CurCharacter._coords.y, 0));
            int index = moveMap.IndexOf(cellPosition);
            if (index != -1)
            {
                Movemap.ClearAllTiles();
                PrintMoveMap(3, cellPosition);
                CurCharacter.TargetCoords = new Vector2Int(cellPosition.x, cellPosition.y);
            }
            else
            {
                Debug.Log("Cell out of move map");
                _blockClick = false;
            }
        }
    }

    private List<Vector3Int> GetMoveMap(int moveDistance, Vector3Int position)
    {
        List<Vector3Int> map = new List<Vector3Int>();
        Vector3Int curPosition = position;

        for (int rotation = 0; rotation < 4; ++rotation)
        {
            Vector3Int moveVector;
            TileBase tile;
            if (rotation == 0)
                moveVector = new Vector3Int(0, 1, 0);
            else if (rotation == 1)
                moveVector = new Vector3Int(1, 0, 0);
            else if (rotation == 2)
                moveVector = new Vector3Int(0, -1, 0);
            else
                moveVector = new Vector3Int(-1, 0, 0);
            for (int step = 1; step <= moveDistance; ++step)
            {
                curPosition += moveVector;
                tile = CurrentTilemap.GetTile(curPosition);
                if (tile is RoadTile)
                {
                    RoadTile roadTile = tile as RoadTile;
                    if (roadTile.isBlock)
                        break;
                }
                map.Add(curPosition);
            }
            curPosition = position;
        }
        return map;
    }

    private void PrintMoveMap(int moveDistance, Vector3Int position)
    {
        List<Vector3Int> mapToPaint = GetMoveMap(moveDistance, position);
        foreach (var tilePosition in mapToPaint)
        {
            Movemap.SetTile(tilePosition, MoveTile);
        }
    }

    public void PrintTileInfo(Vector3Int cellPosition)
    {
        TileBase tile = CurrentTilemap.GetTile(cellPosition);
        RoadTile roadTile = tile as RoadTile;
        if (tile != null)
            Debug.Log($"Tile at position ({cellPosition.x}, {cellPosition.y}) exists");
        else
            Debug.Log($"Tile at position ({cellPosition.x}, {cellPosition.y}) does not exist");
        if (roadTile != null)
            Debug.Log($"This tile is RoadTile and isBlock = {roadTile.isBlock}");
        if (tile is Tile)
        {
            Tile tTile = tile as Tile;
        }
    }

    public Vector3Int GetTilemapCoordsFromWorld(Tilemap tilemap, Vector3 worldCoords)
    {
        return tilemap.WorldToCell(worldCoords);
    }

    public Vector3Int GetTilemapCoordsFromScreen(Tilemap tilemap, Vector3 screenCoords)
    {
        Ray ray = CurrentCamera.ScreenPointToRay(Input.mousePosition);
        Vector3 worldPosition = ray.GetPoint(-ray.origin.z / ray.direction.z);
        return tilemap.WorldToCell(worldPosition);
    }

    public void EnableClick()
    {
        _blockClick = false;
    }



}
