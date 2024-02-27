using System;
using System.Collections;
using UnityEngine;
using GoogleMobileAds.Api;

public class AdmobAdManager : MonoBehaviour
{
    public static AdmobAdManager instance;
    private RewardedAd rewardedAd;

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }
    public void Initialize()
    {
        //Initialize the Google Mobile Ads SDK.

        string appId = "ca-app-pub-1271528849752693~5354792864";
        string adUnitId = "ca-app-pub-1271528849752693/2153914458";

        MobileAds.Initialize(appId);

        RequestRewardedAd();
        //Initialize the Google Mobile Ads SDK.
    }
    // 보상형 광고
    private void RequestRewardedAd()
    {
        Debug.Log("RequestRewardedAd");

        string adUnitId = "ca-app-pub-1271528849752693/2153914458";

        this.rewardedAd = new RewardedAd(adUnitId);

        this.rewardedAd.OnAdLoaded += OnAdLoaded;
        this.rewardedAd.OnAdFailedToLoad += OnAdFailedToLoad;
        this.rewardedAd.OnAdOpening += OnAdOpening;
        this.rewardedAd.OnUserEarnedReward += OnAdRewarded;
        this.rewardedAd.OnAdClosed += OnAdClosed;

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the rewarded ad with the request.
        this.rewardedAd.LoadAd(request);
    }

    public void TR_ShowAds()
    {
        if (rewardedAd.IsLoaded())
        {
            Log_Manager.instance.Add_Log("광고를 재생합니다.");
            rewardedAd.Show();
        }
        else
        {
            Log_Manager.instance.Add_Log("시청 가능한 광고가 없습니다.");
            this.RequestRewardedAd();
        }
    }
    private void OnAdLoaded(object sender, EventArgs args)
    {
        Debug.Log("OnAdLoaded");
    }
    private void OnAdFailedToLoad(object sender, AdErrorEventArgs e)
    {
        Debug.Log("OnAdFailedToLoad");
    }
    private void OnAdOpening(object sender, EventArgs e)
    {
        Debug.Log("OnAdOpening");
    }
    private void OnAdStarted(object sender, EventArgs e)
    {
        Debug.Log("OnAdStarted");
    }
    private void OnAdRewarded(object sender, Reward args)
    {
        string type = args.Type;
        double amount = args.Amount;
        MonoBehaviour.print(
            "HandleRewardedAdRewarded event received for "
                        + amount.ToString() + " " + type);
        //int data = 0;
        //PlayerPrefs.GetInt("Data", data);
        //data = 1;
        //PlayerPrefs.SetInt("Data", data);
        //Donate_UIManager.This.Refresh();
    }
    private void OnAdClosed(object sender, EventArgs e)
    {
        Debug.Log("OnAdClosed");
        RequestRewardedAd();
    }
    private void OnAdLeavingApplication(object sender, EventArgs e)
    {
        Debug.Log("OnAdLeavingApplication");
    } 
}