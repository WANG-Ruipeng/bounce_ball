using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClawbearGames
{
    public class DoorController : MonoBehaviour
    {
        [SerializeField] private string doorID = "Door_0";
        [SerializeField] private SpriteRenderer doorButtonRender = null;
        [SerializeField] private Transform wingUpTrans = null;
        [SerializeField] private Transform wingDownTrans = null;
        [SerializeField] private LayerMask ballLayerMask = new LayerMask();

        public string DoorID => doorID;

        private bool isOpen = false;

        private void OnEnable()
        {
            doorButtonRender.color = Color.red;
            StartCoroutine(CRFlashingDoorButton());
        }

        private void Update()
        {
            if (!isOpen)
            {
                //Check collide with ball
                Collider2D ballCollider = Physics2D.OverlapBox(doorButtonRender.transform.position, doorButtonRender.bounds.size * 0.75f, doorButtonRender.transform.localEulerAngles.z, ballLayerMask);
                if (ballCollider != null)
                {
                    ServicesManager.Instance.SoundManager.PlaySound(ServicesManager.Instance.SoundManager.DoorCollided);
                    isOpen = true;
                    doorButtonRender.color = Color.green;
                    StartCoroutine(CROpenWings());
                    StartCoroutine(CRPlayDoorOpenSoundEffect());
                }
            }
        }


        /// <summary>
        /// Coroutine flashing the door button.
        /// </summary>
        /// <returns></returns>
        private IEnumerator CRFlashingDoorButton()
        {
            yield return null;
            Color startColor = doorButtonRender.color;
            Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0.5f);

            float scaleTime = 0.25f;
            float t = 0;

            while (true)
            {
                t = 0;
                while (t < scaleTime)
                {
                    t += Time.deltaTime;
                    float factor = t / scaleTime;
                    doorButtonRender.color = Color.Lerp(startColor, endColor, factor);
                    yield return null;
                    if (isOpen)
                        yield break;
                }

                t = 0;
                while (t < scaleTime)
                {
                    t += Time.deltaTime;
                    float factor = t / scaleTime;
                    doorButtonRender.color = Color.Lerp(endColor, startColor, factor);
                    yield return null;
                    if (isOpen)
                        yield break;
                }
            }
        }



        /// <summary>
        /// Coroutine open the wings of this door.
        /// </summary>
        /// <returns></returns>
        private IEnumerator CROpenWings()
        {
            //Scale down the wings
            float scaleTime = 0.5f;
            float t = 0;
            while (t < scaleTime)
            {
                t += Time.deltaTime;
                float factor = t / scaleTime;
                wingUpTrans.localScale = Vector3.Lerp(Vector3.one, new Vector3(1f, 0f, 1f), factor);
                wingDownTrans.localScale = Vector3.Lerp(Vector3.one, new Vector3(1f, 0f, 1f), factor);
                yield return null;
            }

            wingUpTrans.localScale = Vector3.zero;
            wingDownTrans.localScale = Vector3.zero;
        }


        /// <summary>
        /// Coroutine wait for an amount of time then play the door open sound effect.
        /// </summary>
        /// <returns></returns>
        private IEnumerator CRPlayDoorOpenSoundEffect()
        {
            yield return new WaitForSeconds(0.15f);
            ServicesManager.Instance.SoundManager.PlaySound(ServicesManager.Instance.SoundManager.DoorOpen);
        }
    }
}