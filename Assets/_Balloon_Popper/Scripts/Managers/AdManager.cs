﻿using System.Collections.Generic;
using UnityEngine;

namespace ClawbearGames
{
    public class AdManager : MonoBehaviour
    {

        [Header("Banner Ad Configuration")]
        [SerializeField] private BannerAdType bannerAdType = BannerAdType.NONE;
        [SerializeField] private float delayTimeWhenShowingBannerAd = 0.5f;


        [Header("Interstitial Ad Configuration")]
        [SerializeField] private List<InterstitialAdConfiguration> listInterstitialAdConfiguration = new List<InterstitialAdConfiguration>();

        [Header("Rewarded Ad Configuration")]
        [SerializeField] private float delayTimeWhenShowingRewardedAd = 0.25f;
        [SerializeField] private List<RewardedAdType> listRewardedAdType = new List<RewardedAdType>();

        [Header("AdManager References")]
        [SerializeField] private AdmobController admobController = null;
        [SerializeField] private UnityAdController unityAdController = null;

        private List<int> listIngameStateNumber = new List<int>();
        private RewardedAdType readyAdType = RewardedAdType.UNITY;

        private bool isCalledback = false;
        private bool isRewarded = false;
        private void OnEnable()
        {
            IngameManager.IngameStateChanged += IngameManager_IngameStateChanged;
        }

        private void OnDisable()
        {
            IngameManager.IngameStateChanged -= IngameManager_IngameStateChanged;
        }


        private void Start()
        {
            foreach (InterstitialAdConfiguration o in listInterstitialAdConfiguration)
            {
                listIngameStateNumber.Add(o.IngameStateAmountWhenShowingAd);
            }

            //Show banner ad
            if (bannerAdType == BannerAdType.ADMOB)
            {
                admobController.ShowBannerAd(delayTimeWhenShowingBannerAd);
            }
            if (bannerAdType == BannerAdType.UNITY)
            {
                unityAdController.ShowBannerAd(delayTimeWhenShowingBannerAd);
            }
        }

        private void Update()
        {
            if (isCalledback)
            {
                isCalledback = false;
                if (isRewarded)
                {
                    if (IngameManager.Instance.IngameState == IngameState.Ingame_Revive)
                    {
                        IngameManager.Instance.SetContinueGame(); //Revive 
                    }
                }
                else
                {
                    if (ViewManager.Instance.ActiveViewType == ViewType.REVIVE_VIEW)
                    {
                        if (IngameManager.Instance.IngameState == IngameState.Ingame_Revive)
                            IngameManager.Instance.GameOver();
                    }
                }
            }
        }



        /// <summary>
        /// Get calls everytime IngameState changed.
        /// </summary>
        /// <param name="ingameState"></param>
        private void IngameManager_IngameStateChanged(IngameState ingameState)
        {
            for (int i = 0; i < listIngameStateNumber.Count; i++)
            {
                if (listInterstitialAdConfiguration[i].IngameStateWhenShowingAd == ingameState)
                {
                    //Count down the number of detected IngameState 
                    listIngameStateNumber[i]--;

                    //The number of detected IngameState already counted down to 0 -> check the priority to show interstitial ad.
                    if (listIngameStateNumber[i] <= 0)
                    {
                        //Reset IngameStateAmount
                        listIngameStateNumber[i] = listInterstitialAdConfiguration[i].IngameStateAmountWhenShowingAd;

                        for (int a = 0; a < listInterstitialAdConfiguration[i].ListInterstitialAdType.Count; a++)
                        {
                            InterstitialAdType type = listInterstitialAdConfiguration[i].ListInterstitialAdType[a];
                            if (type == InterstitialAdType.ADMOB && admobController.IsInterstitialAdReady())
                            {
                                admobController.ShowInterstitialAd(listInterstitialAdConfiguration[i].DelayTimeWhenShowingAd);
                                break;
                            }
                            else if (type == InterstitialAdType.UNITY && unityAdController.IsInterstitialAdReady())
                            {
                                unityAdController.ShowInterstitialAd(listInterstitialAdConfiguration[i].DelayTimeWhenShowingAd);
                                break;
                            }
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Determines whether rewarded ad is ready.
        /// </summary>
        /// <returns></returns>
        public bool IsRewardedAdReady()
        {
            for (int i = 0; i < listRewardedAdType.Count; i++)
            {
                if (listRewardedAdType[i] == RewardedAdType.UNITY && unityAdController.IsRewardedAdReady())
                {
                    readyAdType = RewardedAdType.UNITY;
                    return true;
                }
                else if (listRewardedAdType[i] == RewardedAdType.ADMOB && admobController.IsRewardedAdReady())
                {
                    readyAdType = RewardedAdType.ADMOB;
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// Show the rewarded ad with delay time.
        /// </summary>
        /// <param name="delay"></param>
        public void ShowRewardedAd()
        {
            if (readyAdType == RewardedAdType.UNITY)
            {
                unityAdController.ShowRewardedAd(delayTimeWhenShowingRewardedAd);
            }
            else if (readyAdType == RewardedAdType.ADMOB)
            {
                admobController.ShowRewardedAd(delayTimeWhenShowingRewardedAd);
            }
        }




        /// <summary>
        /// Handle actions when rewarded ad was closed.
        /// </summary>
        /// <param name="isFinished"></param>
        public void OnRewardedAdClosed(bool isFinished)
        {
            isCalledback = true;
            isRewarded = isFinished;
        }



        /// <summary>
        /// Handle actions when player click restart level while playing.
        /// </summary>
        public void HandlePlayerRestartLevel()
        {
            for (int i = 0; i < listIngameStateNumber.Count; i++)
            {
                if (listInterstitialAdConfiguration[i].IngameStateWhenShowingAd == IngameState.Ingame_GameOver)
                {
                    //Count down the number of detected IngameState 
                    listIngameStateNumber[i]--;
                }
            }
        }
    }
}
