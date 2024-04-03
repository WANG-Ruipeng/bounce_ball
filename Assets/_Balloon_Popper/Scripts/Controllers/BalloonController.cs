using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClawbearGames
{
    public class BalloonController : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer = null;
        [SerializeField] private LayerMask ballLayerMask = new LayerMask();


        public SpriteRenderer SpriteRenderer => spriteRenderer;
        public float Radius { get { return spriteRenderer.bounds.extents.x; } }


        /// <summary>
        /// Setup this balloon.
        /// </summary>
        public void OnSetup()
        {
            StartCoroutine(CRCheckCollideWithBall());
        }



        /// <summary>
        /// Coroutine checking collide with the ball.
        /// </summary>
        /// <returns></returns>
        private IEnumerator CRCheckCollideWithBall()
        {
            while (gameObject.activeSelf)
            {
                if (Physics2D.OverlapCircle(transform.position, spriteRenderer.bounds.extents.x, ballLayerMask))
                {
                    OnExplode();
                }

                yield return null;
            }
        }



        /// <summary>
        /// Explode this balloon.
        /// </summary>
        public void OnExplode()
        {
            ServicesManager.Instance.SoundManager.PlaySound(ServicesManager.Instance.SoundManager.BalloonExploded);
            EffectManager.Instance.CreateBalloonExplodedEffect(transform.position, spriteRenderer.color, transform.localScale.x);
            gameObject.SetActive(false);
            IngameManager.Instance.OnBalloonExploded();
        }


        /// <summary>
        /// Determine whether the given position is overlap with this balloon.
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public bool IsOverlap(Vector2 pos)
        {
            return Vector2.Distance(transform.position, pos) <= spriteRenderer.bounds.extents.x + 0.05f;
        }
    }
}
