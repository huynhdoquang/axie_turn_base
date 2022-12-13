using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BaseHero : BaseUnit
{
    [SerializeField] private GameObject indicator;

    public void Init()
    {
        hp = 16;
        curHp = 16;

        this.InitStatus();
    }
    public void SetSelected(bool isSelected)
    {
        indicator.SetActive(isSelected);
    }
}
