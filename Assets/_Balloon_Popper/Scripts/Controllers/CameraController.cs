using System.Collections;
using UnityEngine;

namespace ClawbearGames
{
    public class CameraController : MonoBehaviour
    {
        public static CameraController Instance { private set; get; }

        [Header("Camera Configurations")]
        [SerializeField] private float smoothTime = 0.5f;
        [SerializeField] private float shakeDuration = 0.3f;
        [SerializeField] private float shakeAmount = 0.15f;
        [SerializeField] private float decreaseFactor = 1.5f;

        private Vector2 offset = Vector2.zero;
        private Vector3 velocity = Vector3.zero;
        private Vector3 targetPosition = Vector3.zero;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                DestroyImmediate(Instance.gameObject);
                Instance = this;
            }
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        private void LateUpdate()
        {
            if (IngameManager.Instance.IngameState == IngameState.Ingame_Playing)
            {
                targetPosition.x = transform.position.x;
                targetPosition.z = transform.position.z;
                transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
            }
        }



        /// <summary>
        /// Update the target position for this camera to follow.
        /// </summary>
        /// <param name="targetPos"></param>
        public void UpdateTargetPosition(Vector2 targetPos)
        {
            targetPosition = targetPos + offset;
        }




        /// <summary>
        /// Shake this camera.
        /// </summary>
        public void Shake()
        {
            StartCoroutine(CRSharking());
        }
        private IEnumerator CRSharking()
        {
            yield return new WaitForSeconds(0.15f);
            Vector3 originalPos = transform.position;
            float shakeDurationTemp = shakeDuration;
            while (shakeDurationTemp > 0)
            {
                Vector3 newPos = originalPos + Random.insideUnitSphere * shakeAmount;
                newPos.z = originalPos.z;
                transform.position = newPos;
                shakeDurationTemp -= Time.deltaTime * decreaseFactor;
                yield return null;
            }

            transform.position = originalPos;
        }
    }
}
