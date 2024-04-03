using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClawbearGames
{
    public class ObstacleController : MonoBehaviour
    {
        [SerializeField] private string obstacleID = "Obstacle_0";
        [SerializeField] private SpriteRenderer spriteRenderer = null;


        public SpriteRenderer SpriteRenderer => spriteRenderer;
        public string ObstacleID => obstacleID;
    }
}
