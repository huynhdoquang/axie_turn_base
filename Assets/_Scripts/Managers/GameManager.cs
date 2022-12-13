using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameState GameState;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        ChangeState(GameState.GenerateGrid);
    }

    public void ChangeState(GameState newState)
    {
        GameState = newState;
        switch (newState)
        {
            case GameState.GenerateGrid:
                GridManager.Instance.GenerateGrid();
                break;
            case GameState.SpawnHeroes:
                UnitManager.Instance.SpawnHeroes();
                break;
            case GameState.SpawnEnemies:
                UnitManager.Instance.SpawnEnemies();
                break;
            case GameState.InitHud:
                {
                    RefreshIngameHud(isInit: true);
                    ChangeState(GameState.HeroesTurn);
                    break;
                }
                
            case GameState.HeroesTurn:
                break;
            case GameState.EnemiesTurn:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }
    }

    public void RefreshIngameHud(bool isInit = false)
    {
        var allies = UnitManager.Instance.heroLst;
        var enemies = UnitManager.Instance.enemyLst;

        var allyCp = 0;
        foreach (var item in allies)
        {
            allyCp += item.curHp;
        }

        var eneCp = 0;
        foreach (var item in enemies)
        {
            eneCp += item.curHp;
        }

        if(isInit)
            MenuManager.Instance.InGameHudController.Init(allyCp, eneCp);
        else
            MenuManager.Instance.InGameHudController.SetData(allyCp, eneCp);
    }

    EnMapCrd enMapCod = EnMapCrd.Base;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (enMapCod == EnMapCrd.Base)
            {
                enMapCod = EnMapCrd.Iso;
            }
            else if (enMapCod == EnMapCrd.Iso)
            {
                enMapCod = EnMapCrd.Base;
            }
            GridManager.Instance.SwitchCrd(enMapCod);
        }
    }

    public int CaculatorDmg(int attackerMagicNum, int targetMagicNum)
    {
        var dmgNumber = (3 + attackerMagicNum - targetMagicNum) % 3;
        switch (dmgNumber)
        {
            case 0:
                return 4;
            case 1:
                return 5;
            case 2:
                return 3;
            default:
                return 0;
        }
    }

    public void Restart(bool isLoadMapFromFile = false)
    {
        if (isLoadMapFromFile)
        {
            GridManager.Instance.ReGenMap();
        }

        //clear map
        foreach (var item in UnitManager.Instance.heroLst)
        {
            Destroy(item.gameObject);
        }
        foreach (var item in UnitManager.Instance.enemyLst)
        {
            Destroy(item.gameObject);
        }

        GridManager.Instance.SwitchCrd(EnMapCrd.Base);
        //
        ChangeState(GameState.SpawnHeroes);
    }
}

public enum GameState
{
    GenerateGrid = 0,
    SpawnHeroes = 1,
    SpawnEnemies = 2,
    InitHud = 3,
    HeroesTurn = 4,
    EnemiesTurn = 5
}
