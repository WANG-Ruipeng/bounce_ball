using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace ClawbearGames
{
    public class EndGameViewController : BaseViewController
    {
        [SerializeField] private CanvasGroup mainCanvasGroup = null;
        [SerializeField] private RectTransform topPanelTrans = null;
        [SerializeField] private RectTransform mainButtonsPanelTrans = null;
        [SerializeField] private RectTransform bottonPanelTrans = null;
        [SerializeField] private RectTransform levelCompletedTextTrans = null;
        [SerializeField] private RectTransform levelFailedTextTrans = null;
        [SerializeField] private RectTransform nextLevelButtonTrans = null;
        [SerializeField] private RectTransform replayLevelButtonTrans = null;


        /// <summary>
        ////////////////////////////////////////////////// Public Functions
        /// </summary>


        public override void OnShow()
        {
            FadeInCanvasGroup(mainCanvasGroup, 1f);
            MoveRectTransform(topPanelTrans, topPanelTrans.anchoredPosition, new Vector2(topPanelTrans.anchoredPosition.x, 0f), 0.75f);
            MoveRectTransform(bottonPanelTrans, bottonPanelTrans.anchoredPosition, new Vector2(bottonPanelTrans.anchoredPosition.x, 0f), 0.75f);
            ScaleRectTransform(mainButtonsPanelTrans, Vector3.zero, Vector3.one, 0.75f);

            if (IngameManager.Instance.IngameState == IngameState.Ingame_CompleteLevel)
            {
                levelCompletedTextTrans.gameObject.SetActive(true);
                levelFailedTextTrans.gameObject.SetActive(false);

                nextLevelButtonTrans.gameObject.SetActive(true);
                replayLevelButtonTrans.gameObject.SetActive(false);
            }
            else
            {
                levelFailedTextTrans.gameObject.SetActive(true);
                levelCompletedTextTrans.gameObject.SetActive(false);

                replayLevelButtonTrans.gameObject.SetActive(true);
                nextLevelButtonTrans.gameObject.SetActive(false);
            }
        }

        public override void OnClose()
        {
            mainCanvasGroup.alpha = 0;
            topPanelTrans.anchoredPosition = new Vector2(topPanelTrans.anchoredPosition.x, 200f);
            bottonPanelTrans.anchoredPosition = new Vector2(bottonPanelTrans.anchoredPosition.x, -200f);
            mainButtonsPanelTrans.localScale = Vector3.zero;
            gameObject.SetActive(false);
        }

        /// <summary>
        ////////////////////////////////////////////////// UI Buttons
        /// </summary>  

        public void OnClickPlayButton()
        {
            ServicesManager.Instance.SoundManager.PlaySound(ServicesManager.Instance.SoundManager.Button);
            LoadScene("Ingame", 0.25f);
        }

        public void OnClickShareButton()
        {
            ServicesManager.Instance.SoundManager.PlaySound(ServicesManager.Instance.SoundManager.Button);
            ServicesManager.Instance.ShareManager.NativeShare();
        }

        public void OnClickRateAppButton()
        {
            ServicesManager.Instance.SoundManager.PlaySound(ServicesManager.Instance.SoundManager.Button);
            Application.OpenURL(ServicesManager.Instance.ShareManager.AppUrl);
        }

        public void OnClickHomeButton()
        {
            ServicesManager.Instance.SoundManager.PlaySound(ServicesManager.Instance.SoundManager.Button);
            LoadScene("Home", 0.25f);
        }
    }
}
