using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClawbearGames
{
    public class BlackHoleController : MonoBehaviour
    {
        [SerializeField] private float blackHoleRadius = 1.3f;
        [SerializeField] private string blackHoleID = "Black_Hole_0";
        [SerializeField] private LayerMask ballLayerMask = new LayerMask();

        public string BlackHoleID => blackHoleID;
        private BallController ballController = null;

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, blackHoleRadius);
        }

        private void Update()
        {
            Collider2D ballCollider = Physics2D.OverlapCircle(transform.position, blackHoleRadius, ballLayerMask);
            if (ballCollider != null && ballController == null)
            {
                float forceMagnitude = 100.0f;
                if (blackHoleID == "Black_Hole_2")
                {
                    forceMagnitude = 500.0f;
                }
                else if (blackHoleID == "Black_Hole_3")
                {
                    forceMagnitude = -100.0f;
                }
                else
                {

                }
                ballController = ballCollider.gameObject.GetComponent<BallController>();
                Vector2 forceDirection = (Vector2)transform.position - ballCollider.attachedRigidbody.position;
                ballController.Rigidbody2D.AddForce(forceDirection.normalized * forceMagnitude);
                ballController = null;
            }
        }



        /// <summary>
        /// Determine whether the given position is overlap with this black hole.
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public bool IsOverlap(Vector2 pos)
        {
            return Vector2.Distance(transform.position, pos) <= blackHoleRadius + 0.1f;
        }
    }
}
