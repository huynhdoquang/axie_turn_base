using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class UnitManager : MonoBehaviour {
    public static UnitManager Instance;

    private List<ScriptableUnit> _units;
    public BaseHero SelectedHero;


    public MapReader mapSpawner;

    public List<BaseUnit> heroLst;
    public List<BaseUnit> enemyLst;

    void Awake() {
        Instance = this;

        _units = Resources.LoadAll<ScriptableUnit>("Units").ToList();

    }

    public void SpawnHeroes() {
        /*var heroCount = 2;

        for (int i = 0; i < heroCount; i++) {
            var randomPrefab = GetRandomUnit<BaseHero>(Faction.Hero);
            var spawnedHero = Instantiate(randomPrefab);
            var randomSpawnTile = GridManager.Instance.GetHeroSpawnTile();

            randomSpawnTile.SetUnit(spawnedHero);
        }*/


        //new one
        var prefab = GetRandomUnit<BaseHero>(Faction.Hero);
        var lst = GridManager.Instance.GetSpawnTiles(EnTileType.Atk);
        var lst_iso = GridManager.Instance.GetSpawnTilesIso(EnTileType.Atk);

        Debug.Log("list here: " + lst.Count);
        for (int i = 0; i < lst.Count; i++)
        {
           
            var spawnedHero = Instantiate(prefab);
            var randomSpawnTile = lst[i];

            randomSpawnTile.SetUnit(spawnedHero);

            heroLst.Add(spawnedHero);
        }

        GameManager.Instance.ChangeState(GameState.SpawnEnemies);
    }

    public void SpawnEnemies()
    {
        /*var enemyCount = 3;

        for (int i = 0; i < enemyCount; i++)
        {
            var randomPrefab = GetRandomUnit<BaseEnemy>(Faction.Enemy);
            var spawnedEnemy = Instantiate(randomPrefab);
            var randomSpawnTile = GridManager.Instance.GetEnemySpawnTile();

            randomSpawnTile.SetUnit(spawnedEnemy);
        }*/


        //new one
        var prefab = GetRandomUnit<BaseEnemy>(Faction.Enemy);
        var lst = GridManager.Instance.GetSpawnTiles(EnTileType.Def);
        var lst_iso = GridManager.Instance.GetSpawnTilesIso(EnTileType.Def);

        for (int i = 0; i < lst.Count; i++)
        {

            var spawnedHero = Instantiate(prefab);
            var randomSpawnTile = lst[i];

            randomSpawnTile.SetUnit(spawnedHero);

            enemyLst.Add(spawnedHero);
        }

        GameManager.Instance.ChangeState(GameState.HeroesTurn);
    }

    private T GetRandomUnit<T>(Faction faction) where T : BaseUnit {
        return (T)_units.Where(u => u.Faction == faction).OrderBy(o => Random.value).First().UnitPrefab;
    }

    private List<Tile> indicatorTile = new List<Tile>();
    public CharTurnData CurrentTurnData;
    public void SetSelectedHero(BaseHero hero) {

        if(SelectedHero != null) SelectedHero.SetSelected(false);
        SelectedHero = hero;

        //ingame hud
        MenuManager.Instance.ShowSelectedHero(hero);

        //show character indicator.
        //reset indicator
        foreach (var item in indicatorTile)
        {
            item.ResetState();
        }
        indicatorTile = new List<Tile>();

        if (hero == null)
        {
            return;
        }

        //
        hero.SetSelected(true);

        this.CurrentTurnData = GridManager.Instance.CheckTurnData(hero);
        //safe check 
        if (CurrentTurnData.MoveTileAvaiables != null)
        {
            foreach (var item in CurrentTurnData.MoveTileAvaiables)
            {
                item.ShowMoveAble();
                indicatorTile.Add(item);
            }
        }
        //safe check 
        if (CurrentTurnData.AtkUnitAvaiables != null)
        {
            foreach (var item in CurrentTurnData.AtkUnitAvaiables)
            {
                item.OccupiedTile.ShowAtkAble();
                indicatorTile.Add(item.OccupiedTile);
            }
        }
    }
}
