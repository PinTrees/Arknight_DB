using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using FireBaseClass;
using UI.ScrollViewItem;
public class RankingUI_HM : MonoBehaviour
{
    public Canvas mainCanvas;
    public Transform rankViewTr;
    public GameObject loadingPanel;

    List<RankingIT> rankItem;
    List<int> rankingData;
    List<string> users;
    public void Initialized()
    {
        mainCanvas.enabled = false;

        rankingData = new List<int>();
        users = new List<string>();

        rankItem = new List<RankingIT>();
        for(int i = 0; i < rankViewTr.childCount; i++)
            rankItem.Add(new RankingIT(rankViewTr.GetChild(i)));
    }

    public void trStartUI_Main()
    {
        StartCoroutine(StartUI_Main());
    }
    public IEnumerator StartUI_Main()
    {
        loadingPanel.SetActive(true);

        yield return StartCoroutine(FirebaseDataBase.instance.getStagePointRankingLimit20(rankingData, users));
        for (int i = 0; i < rankItem.Count; i++)
        {
            if(i >= rankingData.Count)
            {
                rankItem[i].Disable();
                continue;
            }
            rankItem[i].Refresh(users[i], rankingData[i].ToString());
        }
        loadingPanel.SetActive(false);
        mainCanvas.enabled = true;
    }
}
