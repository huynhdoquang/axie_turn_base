using AxieMixer.Unity;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MixerLoader : MonoBehaviour
{
    private string allyId = "82694";
    private string enemyId = "26322";

    public Axie2dBuilderResult AllyResult;
    public Axie2dBuilderResult EnemyResult;

    const bool USE_GRAPHIC = false;

    Axie2dBuilder builder => Mixer.Builder;
    void Start()
    {
        Mixer.Init();
        List<string> animationList = builder.axieMixerMaterials.GetMixerStuff(AxieFormType.Normal).GetAnimatioNames();

    }

    public async UniTask Fetch()
    {
        List<UniTask<Axie2dBuilderResult>> tasks = new List<UniTask<Axie2dBuilderResult>>();
        var t1 = GetAxiesGenes(allyId);
        var t2 = GetAxiesGenes(enemyId);
        tasks.Add(t1);
        tasks.Add(t2);

        var taskResult = await UniTask.WhenAll(tasks);

        AllyResult = taskResult[0];
        EnemyResult = taskResult[1];
    }

    private async UniTask<Axie2dBuilderResult> GetAxiesGenes(string axieId)
    {
        string searchString = "{ axie (axieId: \"" + axieId + "\") { id, genes, newGenes}}";
        JObject jPayload = new JObject();
        jPayload.Add(new JProperty("query", searchString));

        var wr = new UnityWebRequest("https://graphql-gateway.axieinfinity.com/graphql", "POST");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jPayload.ToString().ToCharArray());
        wr.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
        wr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        wr.SetRequestHeader("Content-Type", "application/json");
        wr.timeout = 10;

        await wr.SendWebRequest().ToUniTask();
        //yield return wr.SendWebRequest();
        if (wr.error == null)
        {
            var result = wr.downloadHandler != null ? wr.downloadHandler.text : null;
            if (!string.IsNullOrEmpty(result))
            {
                JObject jResult = JObject.Parse(result);
                string genesStr = (string)jResult["data"]["axie"]["newGenes"];

                //ProcessMixer(axieId, genesStr, USE_GRAPHIC);

                if (string.IsNullOrEmpty(genesStr))
                {
                    Debug.LogError($"[{axieId}] genes not found!!!");
                    return null;
                }
                float scale = 0.01f;

                var builderResult = builder.BuildSpineFromGene(axieId, genesStr, scale, USE_GRAPHIC);

                return builderResult;
            }
        }

        return null;
    }

    void SpawnSkeletonAnimation(Axie2dBuilderResult builderResult)
    {
        GameObject go = new GameObject("DemoAxie");
        go.transform.localPosition = new Vector3(0f, -2.4f, 0f);
        SkeletonAnimation runtimeSkeletonAnimation = SkeletonAnimation.NewSkeletonAnimationGameObject(builderResult.skeletonDataAsset);
        runtimeSkeletonAnimation.gameObject.layer = LayerMask.NameToLayer("Player");
        runtimeSkeletonAnimation.transform.SetParent(go.transform, false);
        runtimeSkeletonAnimation.transform.localScale = Vector3.one;

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
