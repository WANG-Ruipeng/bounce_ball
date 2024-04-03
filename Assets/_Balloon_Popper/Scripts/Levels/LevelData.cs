using System.Collections.Generic;
using UnityEngine;

namespace ClawbearGames
{
    [System.Serializable]
    public class LevelData
    {
        public int LevelNumber = 1;
        public int BallAmount = 1;
        public int DrawAmount = 1;
        public int FadeAmount = 1;
        public string TopBackgroundColorHex = string.Empty;
        public string BottomBackgroundColorHex = string.Empty;
        public Vector2 BallIndicatorPosition = Vector2.zero;
        public List<BalloonData> ListBalloonData = new List<BalloonData>();
        public List<ObstacleData> ListObstacleData = new List<ObstacleData>();
        public List<BlackHoleData> ListBlackHoleData = new List<BlackHoleData>();
        public List<SpinnerData> ListSpinnerData = new List<SpinnerData>();
        public List<BombData> ListBombData = new List<BombData>();
        public List<DoorData> ListDoorData = new List<DoorData>();
    }
}
