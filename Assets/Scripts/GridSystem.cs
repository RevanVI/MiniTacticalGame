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
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !_blockClick)
        {
            _blockClick = true;
            Vector3Int cellPosition = GetTilemapCoordsFromScreen(CurrentTilemap, Input.mousePosition);
            PrintTileInfo(cellPosition);
            Movemap.ClearAllTiles();
            PrintMoveMap(3, cellPosition);
            CurCharacter.TargetCoords = new Vector2Int(cellPosition.x, cellPosition.y);
        }
    }

    private void PrintMoveMap(int moveDistance, Vector3Int position)
    {
        Vector3Int printPosition = position;
        TileBase tile = CurrentTilemap.GetTile(printPosition);
        if (tile is RoadTile)
        {
            RoadTile roadTile = tile as RoadTile;
            if (roadTile.isBlock)
                return;
        }
        //paint tiles on x axis starting from center point
        for (printPosition.x = position.x - 1; printPosition.x >= position.x - moveDistance; --printPosition.x)
        {
            tile = CurrentTilemap.GetTile(printPosition);
            if (tile is RoadTile)
            {
                RoadTile roadTile = tile as RoadTile;
                if (roadTile.isBlock)
                    break;
            }
            Movemap.SetTile(printPosition, MoveTile);
            Debug.Log($"<x: Print position ({printPosition.x}, {printPosition.y})");
        }
        printPosition = position;
        for (printPosition.x = position.x + 1; printPosition.x <= position.x + moveDistance; ++printPosition.x)
        {
            tile = CurrentTilemap.GetTile(printPosition);
            if (tile is RoadTile)
            {
                RoadTile roadTile = tile as RoadTile;
                if (roadTile.isBlock)
                    break;
            }
            Movemap.SetTile(printPosition, MoveTile);
            Debug.Log($">x: Print position ({printPosition.x}, {printPosition.y})");
        }
        printPosition = position;
        //print tiles on y axis
        for (printPosition.y = position.y - 1; printPosition.y >= position.y - moveDistance; --printPosition.y)
        {
            tile = CurrentTilemap.GetTile(printPosition);
            if (tile is RoadTile)
            {
                RoadTile roadTile = tile as RoadTile;
                if (roadTile.isBlock)
                    break;
            }
            Movemap.SetTile(printPosition, MoveTile);
            Debug.Log($"<y: Print position ({printPosition.x}, {printPosition.y})");
        }
        printPosition = position;
        for (printPosition.y = position.y + 1; printPosition.y <= position.y + moveDistance; ++printPosition.y)
        {
            tile = CurrentTilemap.GetTile(printPosition);
            if (tile is RoadTile)
            {
                RoadTile roadTile = tile as RoadTile;
                if (roadTile.isBlock)
                    break;
            }
            Movemap.SetTile(printPosition, MoveTile);
            Debug.Log($">y: Print position ({printPosition.x}, {printPosition.y})");
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
