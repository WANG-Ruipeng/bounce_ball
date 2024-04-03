using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClawbearGames
{
    public class BallIndicator : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer = null;

        private BallController ballController = null;
        private Vector2 startPoint = Vector2.zero;
        private Vector2 dragDirection = Vector2.zero;
        private Vector2 forceDirection = Vector2.zero;



        /// <summary>
        /// Determine whether the given position is overlap with this indicator.
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public bool IsOverlap(Vector2 pos)
        {
            return Vector2.Distance(pos, transform.position) <= spriteRenderer.bounds.extents.x;
        }



        /// <summary>
        /// Create the ball and setup.
        /// </summary>
        /// <param name="startPos"></param>
        public void OnMouseStart(Vector2 startPos)
        {
            startPoint = startPos;
            ballController = PoolManager.Instance.GetBallController();
            ballController.transform.position = transform.position;
            ballController.gameObject.SetActive(true);
            ballController.OnInit();
        }


        /// <summary>
        /// Update the direction where the ball heading.
        /// </summary>
        /// <param name="currentPos"></param>
        public void OnMouseUpdate(Vector2 currentPos)
        {
            float distance = Vector2.Distance(startPoint, currentPos);
            dragDirection = (startPoint - currentPos).normalized;
            forceDirection = dragDirection * distance * IngameManager.Instance.BallPushForce;

            //Disable all the projectile dots
            PoolManager.Instance.DisableAllProjectileDots();

            //Create the projectile dots
            int dotAmount = Mathf.RoundToInt(distance * 2.5f);
            float timeStamp = 0.05f;
            for (int i = 0; i <= dotAmount; i++)
            {
                Vector2 pos = Vector2.zero;
                pos.x = (transform.position.x + forceDirection.x * timeStamp);
                //pos.y = (transform.position.y + forceDirection.y * timeStamp) - (Physics2D.gravity.magnitude * ballController.Rigidbody2D.gravityScale * timeStamp * timeStamp) / 2f;
                pos.y = (transform.position.y + forceDirection.y * timeStamp);

                ProjectileDotController projectileDot = PoolManager.Instance.GetProjectileDotController();
                projectileDot.gameObject.SetActive(true);
                projectileDot.transform.position = pos;
                timeStamp += 0.05f;
            }
        }


        /// <summary>
        /// Push the ball to the aimed direction.
        /// </summary>
        public void OnMouseEnd()
        {
            ballController.Push(forceDirection);
            ballController = null;
            PoolManager.Instance.DisableAllProjectileDots();
        }
    }
}
