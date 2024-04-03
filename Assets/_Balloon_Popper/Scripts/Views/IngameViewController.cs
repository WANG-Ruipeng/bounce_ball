using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace ClawbearGames
{
    public class IngameViewController : BaseViewController
    {
        [SerializeField] private RectTransform topPanelTrans = null;
        [SerializeField] private RectTransform tutorialPanelTrans = null;
        [SerializeField] private Text currentLevelText = null;
        [SerializeField] private Text ballAmountText = null;
        [SerializeField] private Text drawAmountText = null;
        [SerializeField] private Text fadeAmountText = null;

        private int maxBallAmount = 0;
        private int maxDrawAmount = 0;
        private int maxFadeAmount = 0;

        /// <summary>
        ////////////////////////////////////////////////// Public Functions
        /// </summary>


        public override void OnShow()
        {
            MoveRectTransform(topPanelTrans, topPanelTrans.anchoredPosition, new Vector2(topPanelTrans.anchoredPosition.x, 0f), 0.5f);
            tutorialPanelTrans.gameObject.SetActive(!Utilities.IsFinishedTutorial());

            ////Update texts and other fields, parameters
            currentLevelText.text = "Level: " + IngameManager.Instance.CurrentLevel.ToString();
        }

        public override void OnClose()
        {
            topPanelTrans.anchoredPosition = new Vector2(topPanelTrans.anchoredPosition.x, 200f);
            gameObject.SetActive(false);
        }


        /// <summary>
        /// Update the ball amount value and draw amount value.
        /// </summary>
        /// <param name="ballAmount"></param>
        /// <param name="drawAmount"></param>
        /// <param name="fadeAmount"></param>
        /// <param name="isInitValues"></param>
        public void UpdateValues(int ballAmount, int drawAmount, int fadeAmount, bool isInitValues = false)
        {
            if (isInitValues)
            {
                maxBallAmount = ballAmount;
                maxDrawAmount = drawAmount;
                maxFadeAmount = fadeAmount;
            }

            ballAmountText.text = ballAmount.ToString() + " / " + maxBallAmount.ToString();
            drawAmountText.text = drawAmount.ToString() + " / " + maxDrawAmount.ToString();
            fadeAmountText.text = fadeAmount.ToString() + " / " + maxFadeAmount.ToString();
        }


        /// <summary>
        ////////////////////////////////////////////////// UI Buttons
        /// </summary>



        public void OnClickRestartButton()
        {
            ServicesManager.Instance.SoundManager.PlaySound(ServicesManager.Instance.SoundManager.Button);
            ServicesManager.Instance.AdManager.HandlePlayerRestartLevel();
            LoadScene("Ingame", 0.25f);
        }


        public void CloseTutorialButton()
        {
            PlayerPrefs.SetInt(PlayerPrefsKeys.PPK_TUTORIAL, 1);
            tutorialPanelTrans.gameObject.SetActive(false);
        }
    }
}
