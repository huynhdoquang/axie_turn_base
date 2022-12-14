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

        heroLst = new List<BaseUnit>();
        //new one
        var prefab = GetRandomUnit<BaseHero>(Faction.Hero);
        var lst = GridManager.Instance.GetSpawnTiles(EnTileType.Atk);

        var skeletonData = GameManager.Instance.Mixer.AllyResult;
        Debug.Log("list here: " + lst.Count);
        for (int i = 0; i < lst.Count; i++)
        {
            var spawnedHero = Instantiate(prefab);
            spawnedHero.Init();
            var randomSpawnTile = lst[i];
            spawnedHero.UpdateSkeleton(skeletonData);
            randomSpawnTile.SetUnit(spawnedHero);

            heroLst.Add(spawnedHero);
        }

        GameManager.Instance.ChangeState(GameState.SpawnEnemies);
    }

    public void SpawnEnemies()
    {
        enemyLst = new List<BaseUnit>();
        //new one
        var prefab = GetRandomUnit<BaseEnemy>(Faction.Enemy);
        var lst = GridManager.Instance.GetSpawnTiles(EnTileType.Def);

        var skeletonData = GameManager.Instance.Mixer.EnemyResult;
        for (int i = 0; i < lst.Count; i++)
        {
            var spawnedHero = Instantiate(prefab);
            var randomSpawnTile = lst[i];

            spawnedHero.Init();
            spawnedHero.UpdateSkeleton(skeletonData);
            randomSpawnTile.SetUnit(spawnedHero);

            enemyLst.Add(spawnedHero);
        }

        GameManager.Instance.ChangeState(GameState.InitHud);
    }


    private T GetRandomUnit<T>(Faction faction) where T : BaseUnit {
        return (T)_units.Where(u => u.Faction == faction).OrderBy(o => Random.value).First().UnitPrefab;
    }

    private List<TileContext> indicatorTile = new List<TileContext>();
    public CharTurnData CurrentTurnData;
    public void SetSelectedHero(BaseHero hero, bool isEndAction = false) {

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

        indicatorTile = new List<TileContext>();

        if (hero == null)
        {
            if (isEndAction)
            {
                GameManager.Instance.RefreshIngameHud();
            }

            if (isEndAction && this.heroLst.Count > 0 && this.enemyLst.Count > 0)
            {
                //check turn
                foreach (var item in this.heroLst)
                {
                    var turnData = GridManager.Instance.CheckTurnData(item);
                    if (turnData.AtkUnitAvaiables.Count != 0
                        || turnData.MoveTileAvaiables.Count != 0)
                    {
                        return;
                    }
                }

                //not any move able
                MenuManager.Instance.ShowPanelInfo("DONT HAVE ANY MOVE ABLE.");
            }
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
                item.TileContext.ShowAtkAble();
                item.ShowCombatPredictDmg(SelectedHero.magicNumber);
                indicatorTile.Add(item.TileContext);
            }
        }
    }

    //
    public void RemoveUnit(BaseUnit baseUnit)
    {
        switch (baseUnit.Faction)
        {
            case Faction.Hero:
                heroLst.Remove(baseUnit);
                break;
            case Faction.Enemy:
                enemyLst.Remove(baseUnit);
                break;
            default:
                break;
        }
        
        Destroy(baseUnit.gameObject);

        //end game checker
        if (heroLst.Count == 0)
        {
            if (enemyLst.Count == 0)
            {
                //draw
                MenuManager.Instance.ShowPanelInfo("DRAW");
            }
            else
            {
                //lose
                MenuManager.Instance.ShowPanelInfo("LOSE");
            }
        }
        else
        {
            if (enemyLst.Count == 0)
            {
                //win
                MenuManager.Instance.ShowPanelInfo("<<< WIN >>>");
            }
        }
    }
}
