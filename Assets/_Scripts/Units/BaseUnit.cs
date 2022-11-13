using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseUnit : MonoBehaviour {
    public string UnitName;
    public Tile OccupiedTile;
    public Tile OccupiedTileIso;
    public Faction Faction;

    private EnMapCrd curCrd;

    public void SwicthCrd(EnMapCrd enMapCrd)
    {
        var localX = 0f;
        var localY = 0f;

        Vector2 offet = Vector2.zero;
        switch (enMapCrd)
        {
            case EnMapCrd.Base:
                offet = OccupiedTile.Offet;
                break;
            case EnMapCrd.Iso:
                offet = OccupiedTileIso.Offet;
                break;
            default:
                break;
        }

        var x = offet.x;
        var y = offet.y;

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
}
