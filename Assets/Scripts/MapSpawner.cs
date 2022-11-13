using Net.Core.TextReader;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class MapTable : TextTable<TextRecord>
{
    public MapTable() : base(LoadType.FromResourcePath, "map_01", columns: 12, 0, containsHeader: false)
    {
        this.Load(true);
    }
}

public class MapSpawner : MonoBehaviour
{
    [SerializeField] private Tile _blackTile, _whiteTile;
    [SerializeField] private Transform _cam;
    [SerializeField] private int _width, _height;

    private MapTable mapTable;
    private Dictionary<Vector2, Tile> _tiles;
    

    /// <summary>
    /// row | col
    /// </summary>
    private int[,] mapData = new int[12, 12];

    public void PaserMap(string map)
    {
        if (mapTable == null)
        {
            mapTable = new MapTable();
            mapTable.Load();
        }

        for (int r = 0; r < mapTable.Rows.Length; r++)
        {
            for (int c = 0; c < mapTable.ColumnsCount; c++)
            {
                //parse map
                mapData[r, c] = ConverMapData(mapTable.Rows[r].Columns[c]);
            }
        }
        
        int ConverMapData(string input)
        {
            var e = EnTileType.Base;
            var isSuccess = Enum.TryParse(input, out EnTileType myStatus);
            if (isSuccess)
            {
                e = myStatus;
            }

            return (int)e;
        }


    }

    public void GenerateBaseGrid()
    {
        _tiles = new Dictionary<Vector2, Tile>();
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                var randomTile = x+y % 2 == 0 ? _whiteTile : _blackTile;
                var spawnedTile = Instantiate(randomTile, new Vector3(x, y), Quaternion.identity);
                spawnedTile.name = $"Tile {x} {y}";


                spawnedTile.Init(x, y);


                _tiles[new Vector2(x, y)] = spawnedTile;
            }
        }

        _cam.transform.position = new Vector3((float)_width / 2 - 0.5f, (float)_height / 2 - 0.5f, -10);

        GameManager.Instance.ChangeState(GameState.SpawnHeroes);
    }

    public Tile GetHeroSpawnTile()
    {
        return _tiles.Where(t => t.Key.x < _width / 2 && t.Value.Walkable).OrderBy(t => Random.value).First().Value;
    }

    public Tile GetEnemySpawnTile()
    {
        return _tiles.Where(t => t.Key.x > _width / 2 && t.Value.Walkable).OrderBy(t => UnityEngine.Random.value).First().Value;
    }

    public Tile GetTileAtPosition(Vector2 pos)
    {
        if (_tiles.TryGetValue(pos, out var tile)) return tile;
        return null;
    }
}




