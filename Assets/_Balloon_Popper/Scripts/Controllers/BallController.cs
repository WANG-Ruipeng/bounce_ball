using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClawbearGames
{
    public class BallController : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D rigid2D = null;
        [SerializeField] private CircleCollider2D ballCircleCollider2D = null;
        [SerializeField] private SpriteRenderer spriteRenderer = null;

        public Rigidbody2D Rigidbody2D => rigid2D;

        private void Update()
        {
            Vector2 viewportPos = Camera.main.WorldToViewportPoint(transform.position);
            if (viewportPos.y <= -0.1f || viewportPos.y >= 1.1f || viewportPos.x <= -0.1f || viewportPos.x >= 1.1f)
            {
                rigid2D.bodyType = RigidbodyType2D.Kinematic;
                gameObject.SetActive(false);
                IngameManager.Instance.OnBallDisable();
            }

            if (Input.GetKeyDown(KeyCode.Q) && IngameManager.Instance.currentFadeAmount >= 0)
            {
                if (ballCircleCollider2D.enabled)
                {
                    ballCircleCollider2D.enabled = false;
                    if (spriteRenderer != null)
                    {
                        spriteRenderer.color = Color.black;
                    }
                }
                else
                {
                    ballCircleCollider2D.enabled = true;
                    if (spriteRenderer != null)
                    {
                        spriteRenderer.color = Color.white;
                    }
                }
            }
        }


        private void OnCollisionEnter2D(Collision2D collision)
        {
            Vector2 contactPoint = collision.contacts[0].point;
            EffectManager.Instance.CreateImpactEffect(contactPoint, Color.gray, ((Vector2)transform.position - contactPoint).normalized);
            ServicesManager.Instance.SoundManager.PlaySound(ServicesManager.Instance.SoundManager.BallCollided);
        }




        /// <summary>
        /// Init parameters of this ball.
        /// </summary>
        public void OnInit()
        {
            rigid2D.gravityScale = 0f;
            rigid2D.bodyType = RigidbodyType2D.Kinematic;
            ballCircleCollider2D.enabled = true;
        }


        /// <summary>
        /// Push this ball with given direction.
        /// </summary>
        /// <param name="forceDir"></param>
        public void Push(Vector2 forceDir)
        {
            ballCircleCollider2D.enabled = true;
            rigid2D.bodyType = RigidbodyType2D.Dynamic;
            rigid2D.AddForce(forceDir, ForceMode2D.Impulse);
        }



        /// <summary>
        /// Disable physic and move this ball to the given position and scale down.
        /// </summary>
        /// <param name="pos"></param>
        public void MoveToPosition(Vector2 pos)
        {
            rigid2D.bodyType = RigidbodyType2D.Static;
            ballCircleCollider2D.enabled = false;
            StartCoroutine(CRMoveToPos(pos));
        }


        /// <summary>
        /// Coroutine move to given position.
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        private IEnumerator CRMoveToPos(Vector2 pos)
        {
            ServicesManager.Instance.SoundManager.PlaySound(ServicesManager.Instance.SoundManager.BallSwallowed);
            Vector2 startPos = transform.position;
            float moveTime = 0.5f;
            float t = 0;
            while (t < moveTime)
            {
                t += Time.deltaTime;
                float factor = t / moveTime;
                transform.position = Vector2.Lerp(startPos, pos, factor);
                transform.localScale = Vector2.Lerp(Vector2.one, Vector2.zero, factor);
                transform.localEulerAngles += Vector3.forward * 500f * Time.deltaTime;
                yield return null;
            }

            ballCircleCollider2D.enabled = true;
            rigid2D.bodyType = RigidbodyType2D.Dynamic;
            transform.localScale = Vector3.one;
            gameObject.SetActive(false);
            IngameManager.Instance.OnBallDisable();
        }
    }
}