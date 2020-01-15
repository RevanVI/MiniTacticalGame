using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Character : MonoBehaviour
{
    public int MoveDistance;
    public int Health;
    public int Damage;

    public float Speed;

    public Tilemap CurrentTilemap;

    private Vector2Int _coords;
    private Vector2Int _targetCoords;

    public Vector2Int TargetCoords
    {
        get { return _targetCoords; }

        set { if (_isOnPlace)
              {
                _targetCoords = value;
                _isOnPlace = false;
              };
        }
    }

    private bool _isOnPlace;

    private Rigidbody2D _rb2d;

    // Start is called before the first frame update
    void Start()
    {
        _rb2d = GetComponent<Rigidbody2D>();
        _isOnPlace = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!_isOnPlace)
        {
            Vector3 targetWorldCoords = CurrentTilemap.GetCellCenterWorld(new Vector3Int(_targetCoords.x, _targetCoords.y, 0));
            Vector3 rotation = (targetWorldCoords - _rb2d.transform.position).normalized;
            float distance = (targetWorldCoords - _rb2d.transform.position).magnitude;
            _rb2d.transform.position = _rb2d.transform.position + rotation * distance * Speed * Time.deltaTime;

            Vector3Int newCoords = GridSystem.Instance.GetTilemapCoordsFromWorld(GridSystem.Instance.CurrentTilemap, _rb2d.transform.position);
            if (newCoords.x == _targetCoords.x && newCoords.y == _targetCoords.y)
            {
                _isOnPlace = true;
            }
            _coords.x = newCoords.x;
            _coords.y = newCoords.y;
        }
    }
}
