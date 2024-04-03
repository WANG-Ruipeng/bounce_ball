using UnityEngine;

namespace ClawbearGames
{
    [System.Serializable]
    public class ObstacleData
    {
        public string ObstacleID = string.Empty;
        public Vector3 Position = Vector3.zero;
        public Vector3 Angles = Vector3.zero;
        public Vector3 Scale = Vector3.one;
        public string HexColor = string.Empty;
    }
}
