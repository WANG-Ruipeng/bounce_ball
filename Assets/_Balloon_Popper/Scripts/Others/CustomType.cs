using System.Collections.Generic;
using UnityEngine;

namespace ClawbearGames
{

    #region --------------------Ingame Enums
    public enum IngameState
    {
        Ingame_Playing = 0,
        Ingame_Revive = 1,
        Ingame_GameOver = 2,
        Ingame_CompleteLevel = 3,
    }

    public enum PlayerState
    {
        Player_Prepare = 0,
        Player_Living = 1,
        Player_Died = 2,
        Player_CompletedLevel = 3,
    }

    #endregion



    #region --------------------Ads Enums
    public enum BannerAdType
    {
        NONE = 0,
        ADMOB = 1,
        UNITY = 2,
    }

    public enum InterstitialAdType
    {
        UNITY = 0,
        ADMOB = 1,
    }


    public enum RewardedAdType
    {
        UNITY = 0,
        ADMOB = 1,
    }
    #endregion



    #region --------------------View Enums
    public enum ViewType
    {
        HOME_VIEW = 0,
        LEADERBOARD_VIEW = 1,
        LOADING_VIEW = 2,
        INGAME_VIEW = 3,
        REVIVE_VIEW = 4,
        ENDGAME_VIEW = 5,
    }

    #endregion



    #region --------------------Classes

    [System.Serializable]
    public class InterstitialAdConfiguration
    {
        [SerializeField] private IngameState ingameStateWhenShowingAd = IngameState.Ingame_CompleteLevel;
        public IngameState IngameStateWhenShowingAd { get { return ingameStateWhenShowingAd; } }

        [SerializeField] private int ingameStateAmountWhenShowingAd = 3;
        public int IngameStateAmountWhenShowingAd { get { return ingameStateAmountWhenShowingAd; } }


        [SerializeField] private float delayTimeWhenShowingAd = 2f;
        public float DelayTimeWhenShowingAd { get { return delayTimeWhenShowingAd; } }

        [SerializeField] private List<InterstitialAdType> listInterstitialAdType = new List<InterstitialAdType>();
        public List<InterstitialAdType> ListInterstitialAdType { get { return listInterstitialAdType; } }
    }


    public class LeaderboardData
    {
        public string Username { private set; get; }
        public void SetUsername(string username)
        {
            Username = username;
        }

        public int Level { private set; get; }
        public void SetLevel(int level)
        {
            Level = level;
        }
    }

    public class LeaderboardDataComparer : IComparer<LeaderboardData>
    {
        public int Compare(LeaderboardData dataX, LeaderboardData dataY)
        {
            if (dataX.Level < dataY.Level)
                return 1;
            if (dataX.Level > dataY.Level)
                return -1;
            else
                return 0;
        }
    }

    #endregion
}
