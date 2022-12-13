using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TileContext
{
    //view
    public Tile baseView;
    public Tile isoView;

    //
    public Vector2 Offset;
    public bool isMoveAble;
    public bool isAtkAble;
    public bool isHighLight;

    //
    public BaseUnit OccupiedUnit;

    public TileContext(Vector2 offset)
    {
        this.Offset = offset;
    }

    public void SetUnit(BaseUnit unit)
    {
        baseView.SetUnit(unit);
        isoView.SetUnit(unit);
    }

    //
    public void OnSwitchCrd()
    {
        baseView.OnSwitchCrd();
        isoView.OnSwitchCrd();
    }

    //turn view state
    public void ShowMoveAble()
    {
        this.isMoveAble = true;
        baseView.ShowMoveAble();
        isoView.ShowMoveAble();
    }

    public void ShowAtkAble()
    {
        this.isAtkAble = true;
        baseView.ShowAtkAble();
        isoView.ShowAtkAble();
    }

    public void ResetState()
    {
        this.isMoveAble = false;
        this.isAtkAble = false;
        baseView.ResetState();
        isoView.ResetState();
    }

}

public abstract class Tile : MonoBehaviour {
    public string TileName;
    [SerializeField] protected SpriteRenderer _renderer;
    [SerializeField] protected Sprite _spriteBase;
    [SerializeField] protected Sprite _spriteIso;
    [SerializeField] private GameObject _highlight;
    [SerializeField] private bool _isWalkable;

    [SerializeField] private GameObject turnMoveAble;
    [SerializeField] private GameObject turnAtkAble;

    public bool Walkable => _isWalkable && context.OccupiedUnit == null;

    public Vector2 Offset = Vector2.zero;

    //
    public TileContext context;


    public virtual void Init(int x, int y)
    {
        Offset = new Vector2(x, y);
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

        OnClickTile();
    }

    public void OnClickTile()
    {
        if (GameManager.Instance.GameState != GameState.HeroesTurn) return;

        if (context.OccupiedUnit != null)
        { //is unit tile
            if (context.OccupiedUnit.Faction == Faction.Hero) //selected hero
            {
                UnitManager.Instance.SetSelectedHero((BaseHero)context.OccupiedUnit);
            }
            else //click on enemy
            {
                var selectedHero = UnitManager.Instance.SelectedHero;
                //check to atk enemy if avaiable
                if (selectedHero != null)
                {
                    var enemy = (BaseEnemy)context.OccupiedUnit;

                    var atkAvaiable = UnitManager.Instance.CurrentTurnData.AtkUnitAvaiables;
                    if (atkAvaiable != null && atkAvaiable.Contains(enemy))
                    {
                        var atkMagicNumber = selectedHero.magicNumber;
                        //do atk
                        enemy.TakeDmg(atkMagicNumber);
                        UnitManager.Instance.SetSelectedHero(null, isEndAction: true);
                    }
                }
            }
        }
        else //click on empty tile
        {
            if (UnitManager.Instance.SelectedHero != null)
            {
                //move your buttt
                var moveAbleTiles = UnitManager.Instance.CurrentTurnData.MoveTileAvaiables;
                if (moveAbleTiles != null && moveAbleTiles.Contains(context))
                {
                    SetUnit(UnitManager.Instance.SelectedHero);
                    UnitManager.Instance.SetSelectedHero(null, isEndAction: true);
                }
            }
        }
    }

    public void SetUnit(BaseUnit unit) {
        if (unit.TileContext != null) unit.TileContext.OccupiedUnit = null;
        //get TileContext
        unit.UpdateTileContext(context);
    }

    #region view
    public void OnSwitchCrd()
    {
        this._highlight.SetActive(false);
        this.turnMoveAble.SetActive(context.isMoveAble);
        this.turnAtkAble.SetActive(context.isAtkAble);
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
    #endregion

    private void OnDrawGizmos()
    {
        Handles.color = Color.blue;
        Handles.Label(this.transform.position, $"{this.Offset.x}, {this.Offset.y}");
    }
}