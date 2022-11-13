using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class GridManager : MonoBehaviour {
    public static GridManager Instance;
    [SerializeField] private int _width, _height;

    [SerializeField] private Tile _grassTile, _mountainTile;

    [SerializeField] private Transform _cam;

    [SerializeField] private MapReader mapReader;

    private Dictionary<Vector2, Tile> _tiles;


    void Awake() {
        Instance = this;
    }

    public void GenerateGrid()
    {

        mapReader.Init();

        _tiles = new Dictionary<Vector2, Tile>();
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++) {
                var randomTile = /*Random.Range(0, 6) == 3 ? _mountainTile :*/ _grassTile;
                var spawnedTile = Instantiate(randomTile, new Vector3(x, y), Quaternion.identity);
                spawnedTile.name = $"Tile {x} {y}";

              
                spawnedTile.Init(x,y);


                _tiles[new Vector2(x, y)] = spawnedTile;
            }
        }

        _cam.transform.position = new Vector3((float)_width / 2 - 0.5f, (float)_height / 2 - 0.5f, -10);

        GameManager.Instance.ChangeState(GameState.SpawnHeroes);
    }

    public Tile GetHeroSpawnTile() {
        return _tiles.Where(t => t.Key.x < _width / 2 && t.Value.Walkable).OrderBy(t => Random.value).First().Value;
    }

    public Tile GetEnemySpawnTile()
    {
        return _tiles.Where(t => t.Key.x > _width / 2 && t.Value.Walkable).OrderBy(t => Random.value).First().Value;
    }

    public Tile GetTileAtPosition(Vector2 pos)
    {
        if (_tiles.TryGetValue(pos, out var tile)) return tile;
        return null;
    }

    public List<Tile> GetSpawnTiles(EnTileType enTileType)
    {
        var tiles = new List<Tile>();
        for (int r = 0; r < mapReader.MapData.GetLength(0); r++)
        {
            for (int c = 0; c < mapReader.MapData.GetLength(1); c++)
            {
                var e = (EnTileType)(mapReader.MapData[r, c]);
                
                if (e == enTileType) //ally
                {
                    tiles.Add(_tiles[new Vector2(r, c)]);
                }
            }
        }
        return tiles;
    }

}