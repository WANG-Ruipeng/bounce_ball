using UnityEngine;
using UnityEngine.UI;

namespace ClawbearGames
{
    public class PlayingViewController : MonoBehaviour
    {

        [SerializeField] private RectTransform levelProgressTrans = null;
        [SerializeField] private Image levelProgressFilterImg = null;
        [SerializeField] private Text currentLevelTxt = null;
        [SerializeField] private Text nextLevelTxt = null;

        public void OnShow()
        {
            //ViewManager.Instance.MoveRect(levelProgressTrans, levelProgressTrans.anchoredPosition, new Vector2(levelProgressTrans.anchoredPosition.x, 150), 0.5f);
            currentLevelTxt.text = IngameManager.Instance.CurrentLevel.ToString();
            nextLevelTxt.text = (IngameManager.Instance.CurrentLevel + 1).ToString();

            if (!IngameManager.Instance.IsRevived)
            {
                levelProgressFilterImg.fillAmount = 0;
            }
        }

        private void OnDisable()
        {
            levelProgressTrans.anchoredPosition = new Vector2(levelProgressTrans.anchoredPosition.x, -200);
        }
    }
}
