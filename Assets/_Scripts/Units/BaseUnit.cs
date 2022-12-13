using AxieMixer.Unity;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BaseUnit : MonoBehaviour {

    //ui
    [SerializeField] private TextMeshPro txtMagicNumber;
    [SerializeField] private TextMeshPro txtHP;
    [SerializeField] private TextMeshPro txtPredictDmg;

    [SerializeField] private SkeletonAnimation runtimeSkeletonAnimation;

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

    public void ShowCombatPredictDmg(int attackerMagicNum)
    {
        this.txtPredictDmg.gameObject.SetActive(attackerMagicNum != -1);
        var loseHp = GameManager.Instance.CaculatorDmg(attackerMagicNum, this.magicNumber);
        this.txtPredictDmg.text = $"-{loseHp}";
    }

    //
    public void InitStatus()
    {
        this.txtHP.text = $"{curHp}/{hp}";

        //magic num
        this.magicNumber = Random.Range(1, 4);
        this.txtMagicNumber.text = $"{magicNumber}";

        ShowCombatPredictDmg(-1);
    }

    //skeleton
    public void UpdateSkeleton(Axie2dBuilderResult builderResult)
    {
        this.runtimeSkeletonAnimation.skeletonDataAsset = builderResult.skeletonDataAsset;

        //runtimeSkeletonAnimation.gameObject.layer = LayerMask.NameToLayer("Player");
        //runtimeSkeletonAnimation.transform.SetParent(go.transform, false);
        runtimeSkeletonAnimation.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

        runtimeSkeletonAnimation.GetComponent<MeshRenderer>().sortingOrder = 2;

        runtimeSkeletonAnimation.gameObject.AddComponent<AutoBlendAnimController>();
        runtimeSkeletonAnimation.state.SetAnimation(0, "action/idle/normal", true);

        if (builderResult.adultCombo.ContainsKey("body") &&
            builderResult.adultCombo["body"].Contains("mystic") &&
            builderResult.adultCombo.TryGetValue("body-class", out var bodyClass) &&
            builderResult.adultCombo.TryGetValue("body-id", out var bodyId))
        {
            runtimeSkeletonAnimation.gameObject.AddComponent<MysticIdController>().Init(bodyClass, bodyId);
        }
        runtimeSkeletonAnimation.skeleton.FindSlot("shadow").Attachment = null;
    }
}
