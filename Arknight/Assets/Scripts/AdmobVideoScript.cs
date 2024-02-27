using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using GoogleMobileAds.Api;

public class AdmobVideoScript : MonoBehaviour
{
    public static AdmobVideoScript This;
    private RewardedAd videoAd;
    string videoID;

    private void Awake()
    {
        This = this;
        DontDestroyOnLoad(this.gameObject);
    }
    public void Initialized()
    {
        string appId = "ca-app-pub-1271528849752693~5354792864";
        videoID = "ca-app-pub-1271528849752693/2153914458";

        MobileAds.Initialize(appId);

        videoAd = new RewardedAd(videoID);
        Handle(videoAd);
        Load();
    }

    public void Refresh()
    {
    }

    private void Handle(RewardedAd videoAd)
    {
        videoAd.OnAdLoaded += HandleOnAdLoaded;
        videoAd.OnAdFailedToLoad += HandleOnAdFailedToLoad;
        videoAd.OnAdFailedToShow += HandleOnAdFailedToShow;
        videoAd.OnAdOpening += HandleOnAdOpening;
        videoAd.OnAdClosed += HandleOnAdClosed;
        videoAd.OnUserEarnedReward += HandleOnUserEarnedReward;
    }

    private void Load()
    {
        AdRequest request = new AdRequest.Builder().Build();
        videoAd.LoadAd(request);
    }

    public void ReloadAd()
    {
        videoID = "ca-app-pub-1271528849752693/2153914458";

        videoAd = new RewardedAd(videoID);
        Handle(videoAd);
        Load();
    }

    public void TR_ShowAds()
    {
        if (videoAd.IsLoaded())
        {
            Log_Manager.instance.Add_Log("광고를 재생합니다.");
            videoAd.Show();
        }
        else
        {
            Log_Manager.instance.Add_Log("시청 가능한 광고가 없습니다. 광고를 불러옵니다.");
            ReloadAd();
        }
    }

    //광고가 로드되었을 때
    public void HandleOnAdLoaded(object sender, EventArgs args)
    {
        Debug.Log("HandleOnAdLoaded");
    }
    //광고 로드에 실패했을 때
    public void HandleOnAdFailedToLoad(object sender, AdErrorEventArgs args)
    {

    }
    //광고 보여주기를 실패했을 때
    public void HandleOnAdFailedToShow(object sender, AdErrorEventArgs args)
    {

    }
    //광고가 제대로 실행되었을 때
    public void HandleOnAdOpening(object sender, EventArgs args)
    {

    }
    //광고가 종료되었을 때
    public void HandleOnAdClosed(object sender, EventArgs args)
    {
        //새로운 광고 Load
        ReloadAd();
    }
    //광고를 끝까지 시청하였을 때
    public void HandleOnUserEarnedReward(object sender, Reward args)
    {
        //보상이 들어갈 곳입니다.
        string type = args.Type;
        double amount = args.Amount;
        //Log_Manager.instance.Add_Log("HandleRewardedAdRewarded event received for " + amount.ToString() + " " + type);
        
        int data;
        if (PlayerPrefs.HasKey("Data").Equals(false))
            data = 0;
        else
        {
            data = PlayerPrefs.GetInt("Data");
            data++;
        }

        PlayerPrefs.SetInt("Data", data);
        PlayerPrefs.Save();

        Donate_UIManager.This.Refresh();
    }
}