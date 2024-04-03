using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace ClawbearGames
{
    public class EffectManager : MonoBehaviour
    {

        public static EffectManager Instance { private set; get; }

        [SerializeField] private ParticleSystem impactEffectPrefab = null;
        [SerializeField] private ParticleSystem spikeExplodedEffectPrefab = null;
        [SerializeField] private SpriteFader balloonExplodedEffectPrefab = null;

        private List<ParticleSystem> listImpactEffect = new List<ParticleSystem>();
        private List<ParticleSystem> listSpikeExplodedEffect = new List<ParticleSystem>();
        private List<SpriteFader> listballoonExplodedEffect = new List<SpriteFader>();

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



        /// <summary>
        /// Create impact effect at given position with color and up direction.
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="color"></param>
        /// <param name="dir"></param>
        public void CreateImpactEffect(Vector3 pos, Color color, Vector3 dir)
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
        /// Create spike exploded effect at given position.
        /// </summary>
        /// <param name="pos"></param>
        public void CreateSpikeExplodedEffect(Vector3 pos)
        {
            //Find in the list
            ParticleSystem spikeExplodedEffect = listSpikeExplodedEffect.Where(a => !a.gameObject.activeSelf).FirstOrDefault();

            if (spikeExplodedEffect == null)
            {
                //Didn't find one -> create new one
                spikeExplodedEffect = Instantiate(spikeExplodedEffectPrefab, pos, Quaternion.identity);
                spikeExplodedEffect.gameObject.SetActive(false);
                listSpikeExplodedEffect.Add(spikeExplodedEffect);
            }

            spikeExplodedEffect.transform.position = pos;
            spikeExplodedEffect.gameObject.SetActive(true);
            StartCoroutine(CRPlayParticle(spikeExplodedEffect));
        }


        /// <summary>
        /// Create a balloon exploded effect at given position.
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="color"></param>
        /// <param name="scale"></param>
        public void CreateBalloonExplodedEffect(Vector3 pos, Color color, float scale)
        {
            //Find in the list
            SpriteFader balloonExplodedEffect = listballoonExplodedEffect.Where(a => !a.gameObject.activeSelf).FirstOrDefault();

            if (balloonExplodedEffect == null)
            {
                //Didn't find one -> create new one
                balloonExplodedEffect = Instantiate(balloonExplodedEffectPrefab, pos, Quaternion.identity);
                balloonExplodedEffect.gameObject.SetActive(false);
                listballoonExplodedEffect.Add(balloonExplodedEffect);
            }

            balloonExplodedEffect.transform.position = pos;
            balloonExplodedEffect.gameObject.SetActive(true);
            balloonExplodedEffect.StartFade(color, scale);
        }
    }
}