using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuManager : MonoBehaviour {
    public static MenuManager Instance;

    [SerializeField] private GameObject _selectedHeroObject,_tileObject,_tileUnitObject;

    [SerializeField] private GameObject panelMsg;

    void Awake() {
        Instance = this;
    }

    public void ShowTileInfo(Tile tile) {

        if (tile == null)
        {
            _tileObject.SetActive(false);
            _tileUnitObject.SetActive(false);
            return;
        }

        _tileObject.GetComponentInChildren<Text>().text = tile.TileName;
        _tileObject.SetActive(true);

        if (tile.context.OccupiedUnit) {
            _tileUnitObject.GetComponentInChildren<Text>().text = tile.context.OccupiedUnit.UnitName;
            _tileUnitObject.SetActive(true);
        }
    }

    public void ShowSelectedHero(BaseHero hero) {
        if (hero == null) {
            _selectedHeroObject.SetActive(false);
            return;
        }

        _selectedHeroObject.GetComponentInChildren<Text>().text = hero.UnitName;
        _selectedHeroObject.SetActive(true);
    }

    public void ShowPanelInfo(string msg)
    {
        this.panelMsg.SetActive(true);
        this.panelMsg.GetComponentInChildren<TextMeshProUGUI>().text = msg;
    }

    public void ResetGame()
    {
        GameManager.Instance.Restart();
        this.panelMsg.SetActive(false);
    }
}
