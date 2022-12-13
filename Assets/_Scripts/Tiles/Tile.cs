using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public abstract class Tile : MonoBehaviour {
    public string TileName;
    [SerializeField] protected SpriteRenderer _renderer;
    [SerializeField] protected Sprite _spriteBase;
    [SerializeField] protected Sprite _spriteIso;
    [SerializeField] private GameObject _highlight;
    [SerializeField] private bool _isWalkable;

    [SerializeField] private GameObject turnMoveAble;
    [SerializeField] private GameObject turnAtkAble;

    public BaseUnit OccupiedUnit;
    public bool Walkable => _isWalkable && OccupiedUnit == null;

    public Vector2 Offset = Vector2.zero;


    public virtual void Init(int x, int y)
    {
        Offset = new Vector2(x, y);
    }

    public void ResetHighLight()
    {
        _highlight.SetActive(false);
    }

    void OnMouseEnter()
    {
        _highlight.SetActive(true);
        MenuManager.Instance.ShowTileInfo(this);
    }

    void OnMouseExit()
    {
        _highlight.SetActive(false);
        MenuManager.Instance.ShowTileInfo(null);
    }

    void OnMouseDown() {
        if(GameManager.Instance.GameState != GameState.HeroesTurn) return;

        if (OccupiedUnit != null) { //is unit tile
            if (OccupiedUnit.Faction == Faction.Hero) //selected hero
            {
                UnitManager.Instance.SetSelectedHero((BaseHero)OccupiedUnit);
            } 
            else //click on enemy
            {
                //check to atk enemy if avaiable
                if (UnitManager.Instance.SelectedHero != null) {
                    var enemy = (BaseEnemy)OccupiedUnit;

                    var atkAvaiable = UnitManager.Instance.CurrentTurnData.AtkUnitAvaiables;
                    if (atkAvaiable != null && atkAvaiable.Contains(enemy))
                    {
                        //do atk
                        enemy.TakeDmg(UnitManager.Instance.SelectedHero);
                        UnitManager.Instance.SetSelectedHero(null);
                    }
                }
            }
        }
        else //click on empty tile
        {
            if (UnitManager.Instance.SelectedHero != null) {
                //move your buttt
                var moveAbleTiles = UnitManager.Instance.CurrentTurnData.MoveTileAvaiables;
                if (moveAbleTiles != null && moveAbleTiles.Contains(this))
                {
                    SetUnit(UnitManager.Instance.SelectedHero);
                    UnitManager.Instance.SetSelectedHero(null);
                }
            }
        }

    }

    public void SetUnit(BaseUnit unit) {
        if (unit.OccupiedTile != null) unit.OccupiedTile.OccupiedUnit = null;
        unit.transform.position = transform.position;
        OccupiedUnit = unit;
        unit.OccupiedTile = this;
    }

    public void OnSwitchCrd()
    {

    }

    //turn view state
    public void ShowMoveAble()
    {
        this.turnMoveAble.SetActive(true);
        this.turnAtkAble.SetActive(false);
    }

    public void ShowAtkAble()
    {
        this.turnMoveAble.SetActive(false);
        this.turnAtkAble.SetActive(true);
    }

    public void ResetState()
    {
        this.turnMoveAble.SetActive(false);
        this.turnAtkAble.SetActive(false);
    }

    private void OnDrawGizmos()
    {
        Handles.color = Color.blue;
        Handles.Label(this.transform.position, $"{this.Offset.x}, {this.Offset.y}");
    }
}