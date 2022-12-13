using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public enum EnMoveAvaiable
{
    Left,
    Right,
    Top,
    Bot
}

public class CharTurnData
{
    public List<BaseUnit> AtkUnitAvaiables;
    public List<TileContext> MoveTileAvaiables;
}

public class GridManager : MonoBehaviour {
    public static GridManager Instance;
    [SerializeField] private int _width, _height;

    [SerializeField] private Tile _grassTile, _grassTileIso, _mountainTile;

    [SerializeField] private Transform _cam;

    [SerializeField] private MapReader mapReader;

    [Header("Container")]
    [SerializeField] private Transform baseContainer;
    [SerializeField] private Transform isoContainer;

    public Dictionary<Vector2, TileContext> TileContexts;

    public EnMapCrd CurMapCrd;

    /// <summary>
    /// iso 100 x 50
    /// </summary>

    void Awake() {
        Instance = this;
    }

    public void GenerateGrid()
    {
        mapReader.Init();


        TileContexts = new Dictionary<Vector2, TileContext>();
        //base
        for (int x = 0; x < _width; x++) //_width
        {
            for (int y = 0; y < _height; y++) {
                var randomTile = _grassTile;
                var localX = x;
                var localY = -y;
                var spawnedTile = Instantiate(randomTile, new Vector3(localX, localY), Quaternion.identity, baseContainer);
                spawnedTile.name = $"Tile {x} {y}";

              
                spawnedTile.Init(x,y);

                var offset = new Vector2(x, y);
                TileContexts.Add(offset, new TileContext(offset));
                TileContexts[offset].baseView = spawnedTile;
                spawnedTile.context = TileContexts[offset];
            }
        }

        //iso
        for (int x = 0; x < _width; x++) //_width
        {
            for (int y = 0; y < _height; y++)
            {
                var randomTile = _grassTileIso;
                var localX = (x + y) * 0.5f * 2;
                var localY = (x - y) * 0.25f * 2;
                var spawnedTile = Instantiate(randomTile, new Vector3(localX, localY), Quaternion.identity, isoContainer);
                spawnedTile.name = $"Tile {x} {y}";


                spawnedTile.Init(x, y);

                var offset = new Vector2(x, y);
                TileContexts[offset].isoView = spawnedTile;
                spawnedTile.context = TileContexts[offset];
            }
        }

        baseContainer.gameObject.SetActive(true);
        isoContainer.gameObject.SetActive(false);

        _cam.transform.position = new Vector3((float)_width / 2 - 0.5f, -(float)_height / 2 - 0.5f + 1, -10);

        GameManager.Instance.ChangeState(GameState.SpawnHeroes);

        CurMapCrd = EnMapCrd.Base;
    }

    public void SwitchCrd(EnMapCrd enMapCrd)
    {
        CurMapCrd = enMapCrd;

        baseContainer.gameObject.SetActive(enMapCrd == EnMapCrd.Base);
        isoContainer.gameObject.SetActive(enMapCrd == EnMapCrd.Iso);

        //todo: reset cam pos
        switch (enMapCrd)
        {
            case EnMapCrd.Base:
                _cam.transform.position = new Vector3((float)_width / 2 - 0.5f, -(float)_height / 2 - 0.5f + 1, -10);
                break;
            case EnMapCrd.Iso:
                _cam.transform.position = new Vector3((float)_width / 2 * 2 - 0.5f * 2, 0, -10);
                break;
            default:
                break;
        }

        //reset highlight visual
        foreach (var item in TileContexts)
        {
            item.Value.OnSwitchCrd();
        }

        //todo: recheck entity pos
        foreach (var item in UnitManager.Instance.heroLst)
        {
            item.SwicthCrd(enMapCrd);
        }

        foreach (var item in UnitManager.Instance.enemyLst)
        {
            item.SwicthCrd(enMapCrd);
        }
    }

    public Tile GetOccupiedTile(Vector2 offset, EnMapCrd enMapCrd)
    {
       /* switch (enMapCrd)
        {
            case EnMapCrd.Base:
                return _tiles[offset];
            case EnMapCrd.Iso:
                return _tilesIso[offset];
            default:
                break;
        }*/

        return null;
    }

    public Tile GetHeroSpawnTile() {
        return TileContexts.Where(t => t.Key.x < _width / 2 && t.Value.baseView.Walkable).OrderBy(t => Random.value).First().Value.baseView;
    }

    public Tile GetEnemySpawnTile()
    {
        return TileContexts.Where(t => t.Key.x > _width / 2 && t.Value.baseView.Walkable).OrderBy(t => Random.value).First().Value.baseView;
    }

    public TileContext GetTileContextAtPosition(Vector2 pos)
    {
        if (TileContexts.TryGetValue(pos, out var tile)) return tile;
        return null;
    }

    public List<TileContext> GetSpawnTiles(EnTileType enTileType)
    {
        var tiles = new List<TileContext>();
        for (int r = 0; r < mapReader.MapData.GetLength(0); r++)
        {
            for (int c = 0; c < mapReader.MapData.GetLength(1); c++)
            {
                var e = (EnTileType)(mapReader.MapData[r, c]);

                if (e == enTileType) //ally
                {
                    tiles.Add(TileContexts[new Vector2(r, c)]);
                }
            }
        }
        return tiles;
    }

    //Utils
    private bool IsMoveAble(BaseUnit baseUnit)
    {
        var offset = baseUnit.TileContext.Offset;

        var faction = baseUnit.Faction;

        var lstCheck = new List<Vector2>()
                                    { new Vector2(offset.x + 1, offset.y),
                                    new Vector2(offset.x - 1, offset.y),
                                    new Vector2(offset.x, offset.y + 1),
                                    new Vector2(offset.x, offset.y - 1)};

        if (faction == Faction.Hero)
        {
            foreach (var item in UnitManager.Instance.heroLst)
            {
                var of = item.TileContext.Offset;
                if (lstCheck.Contains(of))
                {
                    return true;
                }
            }
        }

        if (faction == Faction.Enemy)
        {
            foreach (var item in UnitManager.Instance.enemyLst)
            {
                var of = item.TileContext.Offset;
                if (lstCheck.Contains(of))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private List<TileContext> CheckAvaiableMove(BaseUnit baseUnit)
    {
        var returnLst = new List<TileContext>();

        var isMoveAble = IsMoveAble(baseUnit);
        if (isMoveAble)
        {
            var lst_moveAble = new List<EnMoveAvaiable>();

            //find closet enemy
            var faction = baseUnit.Faction;

            var dist = 0f;
            float closetDist = _width + _height;
            var lst = new List<Vector2>();
            if (faction == Faction.Hero)
            {
                foreach (var item in UnitManager.Instance.enemyLst)
                {
                    dist = (item.TileContext.Offset - baseUnit.TileContext.Offset).magnitude;

                    if (dist <= closetDist)
                    {
                        closetDist = dist;
                        lst.Add(item.TileContext.Offset);
                    }
                }

                foreach (var item in lst)
                {
                    var subtract = item - baseUnit.TileContext.Offset;

                    if (subtract.x > 0)
                    {
                        //add left
                        if (!lst_moveAble.Contains(EnMoveAvaiable.Right)) lst_moveAble.Add(EnMoveAvaiable.Right);
                    }
                    if (subtract.x < 0)
                    {
                        //add right
                        if (!lst_moveAble.Contains(EnMoveAvaiable.Left)) lst_moveAble.Add(EnMoveAvaiable.Left);
                    }
                    if (subtract.y > 0)
                    {
                        //add top
                        if (!lst_moveAble.Contains(EnMoveAvaiable.Top)) lst_moveAble.Add(EnMoveAvaiable.Top);
                    }
                    if (subtract.y < 0)
                    {
                        //add bot
                        if (!lst_moveAble.Contains(EnMoveAvaiable.Bot)) lst_moveAble.Add(EnMoveAvaiable.Bot);
                    }
                }

                
                foreach (var item in lst_moveAble)
                {
                    switch (item)
                    {
                        case EnMoveAvaiable.Left:
                            {
                                var tile = GetTileContextAtPosition(baseUnit.TileContext.Offset + new Vector2(-1, 0));
                                if (tile.OccupiedUnit == null)
                                    returnLst.Add(tile);
                                break;
                            }
                        case EnMoveAvaiable.Right:
                            {
                                var tile = GetTileContextAtPosition(baseUnit.TileContext.Offset + new Vector2(1, 0));
                                if (tile.OccupiedUnit == null)
                                    returnLst.Add(tile);
                                break;
                            }
                        case EnMoveAvaiable.Top:
                            {
                                var tile = GetTileContextAtPosition(baseUnit.TileContext.Offset + new Vector2(0, 1));
                                if (tile.OccupiedUnit == null)
                                    returnLst.Add(tile);
                                break;
                            }
                        case EnMoveAvaiable.Bot:
                            {
                                var tile = GetTileContextAtPosition(baseUnit.TileContext.Offset + new Vector2(0, -1));
                                if (tile.OccupiedUnit == null)
                                    returnLst.Add(tile);
                                break;
                            }
                        default:
                            break;
                    }
                }
            }
        }
        return returnLst;
    }

    public CharTurnData CheckTurnData(BaseUnit baseUnit)
    {
        var charTurnData = new CharTurnData();
        charTurnData.MoveTileAvaiables = CheckAvaiableMove(baseUnit);

        //
        charTurnData.AtkUnitAvaiables = new List<BaseUnit>();
        var offset = baseUnit.TileContext.Offset;
        var faction = baseUnit.Faction;

        var lstCheck = new List<Vector2>()
                                    { new Vector2(offset.x + 1, offset.y),
                                    new Vector2(offset.x - 1, offset.y),
                                    new Vector2(offset.x, offset.y + 1),
                                    new Vector2(offset.x, offset.y - 1)};

        if (faction == Faction.Hero)
        {
            foreach (var item in UnitManager.Instance.enemyLst)
            {
                var of = item.TileContext.Offset;
                if (lstCheck.Contains(of))
                {
                    charTurnData.AtkUnitAvaiables.Add(item);
                }
            }
        }

        if (faction == Faction.Enemy)
        {
            foreach (var item in UnitManager.Instance.heroLst)
            {
                var of = item.TileContext.Offset;
                if (lstCheck.Contains(of))
                {
                    charTurnData.AtkUnitAvaiables.Add(item);
                }
            }
        }

        return charTurnData;
    }
    public void MoveUnit()
    {

    }
}