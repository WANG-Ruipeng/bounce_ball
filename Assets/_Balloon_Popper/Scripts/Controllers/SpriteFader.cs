using System.Collections;
using UnityEngine;

namespace ClawbearGames
{
    public class SpriteFader : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer = null;
        [SerializeField] private Sprite balloonSprite = null;


        private Sprite originalSprite = null;
        private Color balloonColor = Color.white;

        /// <summary>
        /// Fade out this sprite.
        /// </summary>
        /// <param name="color"></param>
        /// <param name="scale"></param>
        public void StartFade(Color color, float scale)
        {
            transform.localScale = new Vector3(scale, scale, 1);
            transform.localEulerAngles = new Vector3(0, 0, Random.Range(0, 360));

            originalSprite = spriteRenderer.sprite;
            spriteRenderer.sprite = balloonSprite;
            spriteRenderer.color = Color.white;
            balloonColor = color;
            StartCoroutine(CRFadingAndScale());
        }
        private IEnumerator CRFadingAndScale()
        {
            yield return new WaitForSeconds(0.05f);
            spriteRenderer.sprite = originalSprite;
            spriteRenderer.color = balloonColor;

            yield return new WaitForSeconds(Random.Range(1f, 2f));
            float t = 0;
            float fadingTime = Random.Range(1f, 2f);
            Color startColor = spriteRenderer.color;
            Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0);
            while (t < fadingTime)
            {
                t += Time.deltaTime;
                float factor = t / fadingTime;
                spriteRenderer.color = Color.Lerp(startColor, endColor, factor);
                yield return null;
            }

            transform.localScale = Vector3.one;
            spriteRenderer.color = startColor;
            transform.SetParent(null);
            gameObject.SetActive(false);
        }
    }
}
