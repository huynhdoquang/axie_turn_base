using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BaseUnit : MonoBehaviour {

    //ui
    [SerializeField] private TextMeshPro txtMagicNumber;
    [SerializeField] private TextMeshPro txtHP;

    //grid
    public string UnitName;
    public TileContext TileContext;
    public Faction Faction;

    //attritube
    public int magicNumber;
    public int hp;
    public int curHp;

    EnMapCrd curMapCrd;

    public void UpdateTileContext(TileContext tileContext)
    {
        switch (curMapCrd)
        {
            case EnMapCrd.Base:
                this.transform.position = tileContext.baseView.transform.position;
                break;
            case EnMapCrd.Iso:
                this.transform.position = tileContext.isoView.transform.position;
                break;
            default:
                break;
        }

        this.TileContext = tileContext;
        this.TileContext.OccupiedUnit = this;
    }

    public void SwicthCrd(EnMapCrd enMapCrd)
    {
        /*//re-check pos
        var localX = 0f;
        var localY = 0f;

        Vector2 offset = TileContext.Offset;
       
        var x = offset.x;
        var y = offset.y;

        switch (enMapCrd)
        {
            case EnMapCrd.Base:
                localX = x;
                localY = -y;
                break;
            case EnMapCrd.Iso:
                localX = (x + y) * 0.5f * 2;
                localY = (x - y) * 0.25f * 2;
                break;
            default:
                break;
        }

        this.transform.position = new Vector3(localX, localY);*/

        //
        curMapCrd = enMapCrd;
        switch (enMapCrd)
        {
            case EnMapCrd.Base:
                {
                    this.transform.position = TileContext.baseView.transform.position;
                    break;
                }
            case EnMapCrd.Iso:
                {
                    this.transform.position = TileContext.isoView.transform.position;
                    break;
                }
            default:
                break;
        }
    }

    public void TakeDmg(int attackerMagicNum)
    {
        var loseHp = GameManager.Instance.CaculatorDmg(attackerMagicNum, this.magicNumber);
        curHp -= loseHp;
        if(curHp <= 0)
        {
            curHp = 0;
            //todo: set state to die
            UnitManager.Instance.RemoveUnit(this);
        }

        this.txtHP.text = $"{curHp}/{hp}";
    }

    //
    public void InitStatus()
    {
        this.txtHP.text = $"{curHp}/{hp}";

        //magic num
        this.magicNumber = Random.Range(1, 4);
        this.txtMagicNumber.text = $"{magicNumber}";
    }

    public void SetHp(int curHp)
    {
        this.curHp = curHp;
        this.txtHP.text = $"{curHp}/{hp}";
    }

}
