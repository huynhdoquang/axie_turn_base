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
    public Tile OccupiedTile;
    public Faction Faction;

    private EnMapCrd curCrd;

    //attritube
    public int magicNumber;
    public int hp;
    public int curHp;

    public void SwicthCrd(EnMapCrd enMapCrd)
    {
        var localX = 0f;
        var localY = 0f;

        Vector2 offset = OccupiedTile.Offset;
       
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

        this.transform.position = new Vector3(localX, localY);
        this.curCrd = enMapCrd;
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
