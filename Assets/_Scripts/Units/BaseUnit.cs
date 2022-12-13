using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseUnit : MonoBehaviour {

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

    public void TakeDmg(BaseUnit attacker)
    {
        var loseHp = GameManager.Instance.CaculatorDmg(attacker, this);
        curHp -= loseHp;
        if(curHp <= 0)
        {
            curHp = 0;
            //todo: set state to die
        }
    }
}
