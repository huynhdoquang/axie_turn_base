using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemy : BaseUnit
{
    public void Init()
    {
        hp = 32;
        curHp = 32;

        this.InitStatus();
    }
}
