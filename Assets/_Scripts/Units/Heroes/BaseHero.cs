using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseHero : BaseUnit
{
    [SerializeField] private GameObject indicator;

    public void SetSelected(bool isSelected)
    {
        indicator.SetActive(isSelected);
    }
}
