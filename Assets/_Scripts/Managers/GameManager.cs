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
            case GameState.HeroesTurn:
                break;
            case GameState.EnemiesTurn:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }
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

    public int CaculatorDmg(BaseUnit attacker, BaseUnit target)
    {
        var dmgNumber = (3 + attacker.magicNumber - target.magicNumber) % 3;
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
}

public enum GameState
{
    GenerateGrid = 0,
    SpawnHeroes = 1,
    SpawnEnemies = 2,
    HeroesTurn = 3,
    EnemiesTurn = 4
}
