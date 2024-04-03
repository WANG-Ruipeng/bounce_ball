using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ClawbearGames
{
    public class HomeManager : MonoBehaviour
    {
        [Header("HomeManager References")]
        [SerializeField] private Rigidbody2D ballRigidbody = null;
        [SerializeField] private ParticleSystem impactEffectPrefab = null;
        [SerializeField] private Transform[] obstacleTrans = null;
        [SerializeField] private Collider2D[] backgroundColliders = null;


        private List<ParticleSystem> listImpactEffect = new List<ParticleSystem>();


        private void Awake()
        {
            //PlayerPrefs.SetInt(PlayerPrefsKeys.PPK_SAVED_LEVEL, 1);
            //Set current level
            if (!PlayerPrefs.HasKey(PlayerPrefsKeys.PPK_SAVED_LEVEL))
            {
                PlayerPrefs.SetInt(PlayerPrefsKeys.PPK_SAVED_LEVEL, 1);
            }
        }


        private void Start()
        {
            Application.targetFrameRate = 60;
            ViewManager.Instance.OnShowView(ViewType.HOME_VIEW);

            ballRigidbody.gravityScale = 3f;
            ballRigidbody.bodyType = RigidbodyType2D.Dynamic;
            Vector2 forceDir = Random.value <= 0.5f ? (Vector2.down * 5f + Vector2.left).normalized : (Vector2.down * 5f + Vector2.right).normalized;
            ballRigidbody.AddForce(forceDir * 2f, ForceMode2D.Impulse);
            StartCoroutine(CRCheckCollision());
        }

        private void Update()
        {
            foreach(Transform trans in obstacleTrans)
            {
                trans.localEulerAngles += Vector3.forward * 50f * Time.deltaTime;
            }
        }


        private IEnumerator CRCheckCollision()
        {
            while (gameObject.activeSelf)
            {
                foreach(Collider2D collider2D in backgroundColliders)
                {
                    if (ballRigidbody.IsTouching(collider2D))
                    {
                        Vector2 contactPoint = ballRigidbody.ClosestPoint(collider2D.transform.position);
                        CreateImpactEffect(contactPoint, Color.gray, ((Vector2)ballRigidbody.transform.position - contactPoint).normalized);
                    }
                }

                yield return null;
            }
        }



        /// <summary>
        /// Create impact effect at given position with color and up direction.
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="color"></param>
        /// <param name="dir"></param>
        private void CreateImpactEffect(Vector3 pos, Color color, Vector3 dir)
        {
            //Find in the list
            ParticleSystem impactEffect = listImpactEffect.Where(a => !a.gameObject.activeSelf).FirstOrDefault();

            if (impactEffect == null)
            {
                //Didn't find one -> create new one
                impactEffect = Instantiate(impactEffectPrefab, pos, Quaternion.identity);
                impactEffect.gameObject.SetActive(false);
                listImpactEffect.Add(impactEffect);
            }

            impactEffect.transform.position = pos;
            impactEffect.transform.up = dir;
            var particleMain = impactEffect.main;
            particleMain.startColor = color;
            impactEffect.gameObject.SetActive(true);
            StartCoroutine(CRPlayParticle(impactEffect));
        }


        /// <summary>
        /// Coroutine play the given particle then disable it 
        /// </summary>
        /// <param name="par"></param>
        /// <returns></returns>
        private IEnumerator CRPlayParticle(ParticleSystem par)
        {
            par.Play();
            var main = par.main;
            yield return new WaitForSeconds(main.startLifetimeMultiplier);
            par.gameObject.SetActive(false);
        }
    }
}
