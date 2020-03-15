using EasyMobile;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdsController : MonoBehaviour
{

    void Awake()
    {
        if (!RuntimeManager.IsInitialized())
            RuntimeManager.Init();
    }
    // Start is called before the first frame update
    void Start()
    {
        //Advertising.ShowBannerAd(BannerAdPosition.Bottom);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator showBanner()
    {
        Advertising.ShowBannerAd(BannerAdPosition.Bottom);
        yield return new WaitForSeconds(5f);
        Advertising.DestroyBannerAd();
    }

    public void showInterstitial()
    {
        if (Advertising.IsInterstitialAdReady())
            Advertising.ShowInterstitialAd();
    }

    public void showBannerAndClose()
    {
        Advertising.ShowBannerAd(BannerAdPosition.Top);
        //StartCoroutine(showBanner());
    }
}
