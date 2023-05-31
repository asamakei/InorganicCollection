using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;

public class GoogleAds : MonoBehaviour {

    void Start() {
        MobileAds.Initialize(initStatus => { });
        RequestBanner();
    }

    private void RequestBanner() {

#if UNITY_ANDROID
        //string adUnitId = "ca-app-pub-3940256099942544/6300978111";
        string adUnitId = "ca-app-pub-9234834940896786/4388882203";
#elif UNITY_IPHONE
        //string adUnitId = "ca-app-pub-3940256099942544/2934735716";//テスト
        string adUnitId = "ca-app-pub-9234834940896786/8342062946";
#else
        string adUnitId = "unexpected_platform";
#endif

        BannerView bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Bottom);
        AdRequest request = new AdRequest.Builder()
            //.AddTestDevice(AdRequest.TestDeviceSimulator)
            .Build();
        bannerView.LoadAd(request);

    }
}