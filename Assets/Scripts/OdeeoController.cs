using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OdeeoController : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private RectTransform rect; 
    
    public bool IsInitialized => PlayOnSDK.IsInitialized();
    
    public bool IsAdAvailable => _adUnit.IsAdAvailable();

    private AdUnit _adUnit;
    
    public void Awake()
    {
        PlayOnSDK.OnInitializationFinished += OnInitialized;
        PlayOnSDK.OnInitializationFailed += OnInitializationFailed;

        InitSDK();
    }

    void InitSDK()
    {
        PlayOnSDK.Initialize("8e260d33-2b04-4315-9801-76c809dfb1bc");
        
        Debug.LogError("Init!");
    }
    
    void OnInitialized()
    {
        Debug.LogError("Initialized!");
        LoadAd();
    }

    void LoadAd(bool showAd = false)
    {
        var adType = PlayOnSDK.AdUnitType.AudioLogoAd;
        _adUnit = new AdUnit (adType);
        
        PlayOnSDK.Position adLocation = PlayOnSDK.Position.BottomLeft;
        _adUnit.LinkLogoToRectTransform(adLocation, rect, canvas);
        
        _adUnit.AdCallbacks.OnClose += OnAdsClose;
        _adUnit.AdCallbacks.OnUserClose += OnAdsClose;

        if (showAd)
        {
            StopAllCoroutines();
            StartCoroutine(WaitForAdsAvailable());
        }
    }

    void OnAdsClose()
    {
        Invoke(nameof(ShowAds), 30);
    }

    void OnInitializationFailed(int errorParam, string error)
    {
        Debug.LogError("Init Failed!");
        Invoke(nameof(InitSDK), 2);
    }
    
    public void OnApplicationPause(bool pauseStatus) 
    {
        PlayOnSDK.onApplicationPause(pauseStatus);
    }

    public void ShowAds()
    {
        if (!IsInitialized)
            return;
        
        if (IsAdAvailable)
        {
            _adUnit.ShowAd();
            Debug.LogError("Show Ads");
        }
        else 
            LoadAd(true);
    }

    public void CloseAds()
    {
        if (IsInitialized)
            _adUnit.CloseAd();
    }

    IEnumerator WaitForAdsAvailable()
    {
        while (_adUnit == null || !IsAdAvailable)
            yield return null;
        
        ShowAds();
    }
}
