using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClawbearGames
{
    public class BombController : MonoBehaviour
    {
        [SerializeField] private float bombRadius = 1.3f;
        [SerializeField] private string bombID = "Bomb_0";
        [SerializeField] private LayerMask ballLayerMask = new LayerMask();

        public string BombID => bombID;
        private BallController ballController = null;
        private Vector2 spikeSize = Vector2.zero;
        private int childCount = 0;

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, bombRadius);
        }

        private void Update()
        {
            if (ballController == null) 
            {
                Collider2D ballCollider = Physics2D.OverlapCircle(transform.position, bombRadius, ballLayerMask);
                if (ballCollider != null)
                {
                    ServicesManager.Instance.SoundManager.PlaySound(ServicesManager.Instance.SoundManager.BombExploded);

                    ballController = ballCollider.gameObject.GetComponent<BallController>();

                    //Fire spikes from this bomb
                    childCount = transform.childCount;
                    spikeSize = transform.GetChild(0).GetComponent<SpriteRenderer>().bounds.size;
                    while (transform.childCount > 0)
                    {
                        StartCoroutine(CRFireSpike(transform.GetChild(0)));
                    }

                    //Wait and disable this bomb
                    StartCoroutine(CRDisabling());
                }
            }           
        }

        /// <summary>
        /// Coroutine disabling this bomb.
        /// </summary>
        /// <returns></returns>
        private IEnumerator CRDisabling()
        {
            //Scale this bomb down first
            float scaleTime = 0.5f;
            float t = 0;
            while (t < scaleTime)
            {
                t += Time.deltaTime;
                float factor = t / scaleTime;
                transform.localScale = Vector2.Lerp(Vector2.one, Vector2.zero, factor);
                transform.localEulerAngles += Vector3.forward * 500f * Time.deltaTime;
                yield return null;
            }
            transform.localEulerAngles = Vector3.zero;


            //Wait for all spikes to disable and return to parent
            while (transform.childCount < childCount)
            {
                yield return new WaitForSeconds(0.5f);
            }

            //Reset parameters and disable this bomb
            ballController = null;
            transform.localScale = Vector3.one;
            for(int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).localScale = Vector3.one;
            }
            gameObject.SetActive(false);
        }



        /// <summary>
        /// Coroutine fire the given spike
        /// </summary>
        /// <param name="spikeTrans"></param>
        /// <returns></returns>
        private IEnumerator CRFireSpike(Transform spikeTrans) 
        {
            Vector2 originalPosition = spikeTrans.localPosition;
            Vector3 originalAngles = spikeTrans.localEulerAngles;
            spikeTrans.SetParent(null);

            BalloonController[] balloonControllers = FindObjectsOfType<BalloonController>(false);

            while (spikeTrans.gameObject.activeSelf)
            {
                spikeTrans.position += spikeTrans.up * 10f * Time.deltaTime;
                yield return null;

                //Collide with obstacles -> explode this spike
                Collider2D collider2D = Physics2D.OverlapBox(spikeTrans.position, spikeSize, spikeTrans.localEulerAngles.z);
                if (collider2D != null && !collider2D.CompareTag("Player"))
                {
                    EffectManager.Instance.CreateSpikeExplodedEffect(spikeTrans.position);
                    spikeTrans.SetParent(transform);
                    spikeTrans.localPosition = originalPosition;
                    spikeTrans.localEulerAngles = originalAngles;
                    yield break;
                }

                //Check if the spike is hit a balloon -> explode the balloon
                foreach (BalloonController balloon in balloonControllers)
                {
                    if (balloon.gameObject.activeSelf)
                    {
                        float distance = Vector2.Distance(spikeTrans.position, balloon.transform.position) - 0.3f;
                        if (distance <= balloon.Radius)
                        {
                            //Explode this spike and the balllon
                            balloon.OnExplode();
                            EffectManager.Instance.CreateImpactEffect(spikeTrans.position, Color.black, -spikeTrans.up);
                        }
                    }
                }
            }
        }
    }
}
