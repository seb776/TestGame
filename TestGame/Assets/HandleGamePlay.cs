using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleGamePlay : MonoBehaviour
{
    enum EDirecton
    {
        UP,
        DOWN,
        LEFT,
        RIGHT,
        NONE
    }

    int _currentLevel = 0;
    public GameObject ParticlePrefab;
    public GameObject Player;
    public GameObject TextNextLvl;
    public GameObject LevelDisplayGO;
    public GameObject StartUI;
    public float Speed;
    public GameObject DebugPos;
    public GameObject PaintGroundPrefab;
    public GameObject Debug;

    GenerateLevel.Map _map;
    private int _toFillCount;
    Vector2Int _position;
    Vector2 _dirMove;
    EDirecton _lastDir;
    bool _hasWin;
    bool _started;

    // Use this for initialization
    void Start()
    {
        _started = false;
        Reset(0);
        //GenerateLevel.GenerateRandomMap(@"C:\Users\z0rg_2080\Desktop\testA.Map", 10, 10);
        //GenerateLevel.GenerateRandomMap(@"C:\Users\z0rg_2080\Desktop\testB.Map", 12, 7);
    }

    void Reset(int lvl)
    {
        GenerateLevel.ClearMapParts();
        _map = GenerateLevel.Instance.LoadLevel(lvl);
        _lastDir = EDirecton.NONE;
        _position = _map.Start;
        _toFillCount = 0;
        _dirMove = Vector2.zero;
        _hasWin = false;
        TextNextLvl.SetActive(false);
        var newFloatCoords = GenerateLevel.IntToFloatCoordinates(_position);
        Player.transform.position = new Vector3(newFloatCoords.x, Player.transform.position.y, newFloatCoords.y);
    }

    void Move(EDirecton dir)
    {
        _position = GenerateLevel.FloatToIntCoordinates(new Vector2(Player.transform.position.x, Player.transform.position.z));


        if (DebugPos != null)
        {
            var floatPaintPos = GenerateLevel.IntToFloatCoordinates(_position);

            DebugPos.transform.position = new Vector3(floatPaintPos.x, DebugPos.transform.position.y, floatPaintPos.y);
            if (_map.Access(_position) == 0)
                DebugPos.GetComponent<MeshRenderer>().material.color = Color.blue;
            else
                DebugPos.GetComponent<MeshRenderer>().material.color = Color.red;
        }

        if (_dirMove.magnitude < double.Epsilon)
        {
            switch (dir)
            {
                case EDirecton.UP:
                    _dirMove = Vector2.up;
                    break;
                case EDirecton.DOWN:
                    _dirMove = Vector2.down;
                    break;
                case EDirecton.LEFT:
                    _dirMove = Vector2.left;
                    break;
                case EDirecton.RIGHT:
                    _dirMove = Vector2.right;
                    break;
                default:
                    break;
            }
        }
        if (_dirMove.magnitude >= double.Epsilon)
        {
            Ray r = new Ray();
            r.origin = Player.transform.position;
            r.direction = new Vector3(_dirMove.x, 0.0f, _dirMove.y);
            RaycastHit hit;
            if (Debug != null)
                Debug.transform.position = Player.transform.position + new Vector3(_dirMove.x, 0.0f, _dirMove.y);
            if (Physics.Raycast(r, out hit) && hit.distance < 0.75f) // wall _dirMove = 0
            {
                _dirMove = Vector2.zero;
                _lastDir = EDirecton.NONE;

                // We realign the position with the grid to avoid accumulating errors
                var newFloatCoords = GenerateLevel.IntToFloatCoordinates(_position);
                Player.transform.position = new Vector3(newFloatCoords.x, Player.transform.position.y, newFloatCoords.y);
            }
            else
            {
                Player.transform.position = Player.transform.position + new Vector3(_dirMove.x, 0.0f, _dirMove.y) * Time.deltaTime * Speed;

                if (_map.Access(_position) == 0)
                {
                    var floatPaintPos = GenerateLevel.IntToFloatCoordinates(_position);
                    var paintGround = GameObject.Instantiate(PaintGroundPrefab);
                    paintGround.transform.position = new Vector3(floatPaintPos.x, paintGround.transform.position.y, floatPaintPos.y);
                    _map.Set(_position, -1);
                    _toFillCount++;

                    var particle = GameObject.Instantiate(ParticlePrefab);
                    particle.transform.position = new Vector3(floatPaintPos.x, paintGround.transform.position.y, floatPaintPos.y);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!_started)
        {
            if (Input.GetKey(KeyCode.KeypadEnter))
            {
                _started = true;
            }
            return;
        }
        StartUI.SetActive(false);
        LevelDisplayGO.GetComponent<TextMesh>().text = "Level " + (_currentLevel + 1);
        if (_hasWin)
        {
            if (Input.GetKey(KeyCode.KeypadEnter))
            {
                ++_currentLevel;
                Reset(_currentLevel);
            }
            return;
        }
        if (_toFillCount == _map.ToFillCount)
        {
            _hasWin = true;
            TextNextLvl.SetActive(true);
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            _lastDir = EDirecton.UP;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            _lastDir = EDirecton.DOWN;
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            _lastDir = EDirecton.LEFT;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            _lastDir = EDirecton.RIGHT;
        }
        Move(_lastDir);
    }
}
