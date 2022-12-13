using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InGameHudController : MonoBehaviour
{
    [SerializeField] private Slider sliderAllyTotalCp;
    [SerializeField] private Slider sliderEnemyTotalCp;

    [SerializeField] private TextMeshProUGUI txtAlly;
    [SerializeField] private TextMeshProUGUI txtEnemy;

    public void Init(int allyTotalCp, int enemyTotalEnemy)
    {
        var max = Mathf.Max(allyTotalCp, enemyTotalEnemy);
        sliderAllyTotalCp.minValue = 0;
        sliderAllyTotalCp.maxValue = max;

        sliderEnemyTotalCp.minValue = 0;
        sliderEnemyTotalCp.maxValue = max;

        sliderAllyTotalCp.value = allyTotalCp;
        sliderEnemyTotalCp.value = enemyTotalEnemy;

        this.txtAlly.text = allyTotalCp.ToString();
        this.txtEnemy.text = enemyTotalEnemy.ToString();
    }

    public void SetData(int allyTotalCp, int enemyTotalEnemy)
    {
        sliderAllyTotalCp.value = allyTotalCp ;
        sliderEnemyTotalCp.value = enemyTotalEnemy;

        this.txtAlly.text = allyTotalCp.ToString();
        this.txtEnemy.text = enemyTotalEnemy.ToString();
    }
}
