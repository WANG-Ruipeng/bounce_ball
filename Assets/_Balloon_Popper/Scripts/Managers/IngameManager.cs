using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;


namespace ClawbearGames
{
    public class IngameManager : MonoBehaviour
    {
        public static IngameManager Instance { private set; get; }
        public static event System.Action<IngameState> IngameStateChanged = delegate { };


        [Header("Enter a number of level to test. Set back to 0 to disable this feature.")]
        [SerializeField] private int testingLevel = 0;



        [Header("Ingame Configuration")]
        [SerializeField] private float reviveWaitTime = 5f;
        [SerializeField] private float ballPushForce = 4f;
        [SerializeField] private AudioClip[] backgroundAudioClips = null;

        [Header("Ingame References")]
        [SerializeField] private BallIndicator ballIndicator = null;
        [SerializeField] private Material backgroundMaterial = null;
        [SerializeField] private ParticleSystem[] confettiEffects = null;



        public IngameState IngameState
        {
            get { return ingameState; }
            private set
            {
                if (value != ingameState)
                {
                    ingameState = value;
                    IngameStateChanged(ingameState);
                }
            }
        }

        public float ReviveWaitTime => reviveWaitTime;
        public float BallPushForce => ballPushForce;
        public int CurrentLevel { private set; get; }
        public bool IsRevived { private set; get; }



        private IngameState ingameState = IngameState.Ingame_GameOver;
        private LineController lineController = null;
        private AudioClip backgroundMusic = null;
        private LevelData levelData = null;
        private int currentBallAmount = 0;
        private int currentDrawAmount = 0;
        public int currentFadeAmount = 0;
        private bool isDrawingLine = false;
        private bool isCreatingBall = false;

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

        private void Start()
        {
            Application.targetFrameRate = 60;
            StartCoroutine(CRShowingViewWithDelay(ViewType.INGAME_VIEW, 0f));

            //Setup variables
            IsRevived = false;

            //PlayerPrefs.SetInt(PlayerPrefsKeys.PPK_SAVED_LEVEL, 1);
            //Load level parameters
            CurrentLevel = (testingLevel != 0) ? testingLevel : PlayerPrefs.GetInt(PlayerPrefsKeys.PPK_SAVED_LEVEL, 1);

            TextAsset tex = Resources.Load("Levels/" + CurrentLevel.ToString()) as TextAsset;
            levelData = JsonUtility.FromJson<LevelData>(tex.ToString().Trim());
            ballIndicator.transform.position = levelData.BallIndicatorPosition;
            backgroundMaterial.SetColor("_TopColor", Utilities.HexToColor(levelData.TopBackgroundColorHex));
            backgroundMaterial.SetColor("_BottomColor", Utilities.HexToColor(levelData.BottomBackgroundColorHex));
            currentBallAmount = levelData.BallAmount;
            currentDrawAmount = levelData.DrawAmount;
            currentFadeAmount = levelData.FadeAmount;

            //Create balloons
            foreach (BalloonData data in levelData.ListBalloonData)
            {
                BalloonController balloonController = PoolManager.Instance.GetBalloonController();
                balloonController.transform.position = data.Position;
                balloonController.transform.localScale = data.Scale;
                balloonController.gameObject.SetActive(true);
                balloonController.SpriteRenderer.color = Utilities.HexToColor(data.HexColor);
                balloonController.OnSetup();
            }


            //Create obstacles
            foreach(ObstacleData data in levelData.ListObstacleData)
            {
                ObstacleController obstacleController = PoolManager.Instance.GetObstacleController(data.ObstacleID);
                obstacleController.transform.position = data.Position;
                obstacleController.transform.localScale = data.Scale;
                obstacleController.transform.eulerAngles = data.Angles;
                obstacleController.SpriteRenderer.color = Utilities.HexToColor(data.HexColor);
                obstacleController.gameObject.SetActive(true);
            }


            //Create black holes
            foreach(BlackHoleData data in levelData.ListBlackHoleData)
            {
                BlackHoleController blackHoleController = PoolManager.Instance.GetBlackHoleController(data.BlackHoleID);
                blackHoleController.transform.position = data.Position;
                blackHoleController.gameObject.SetActive(true);
            }

            //Create spinners
            foreach(SpinnerData data in levelData.ListSpinnerData) 
            {
                SpinnerController spinnerController = PoolManager.Instance.GetSpinnerController(data.SpinnerID);
                spinnerController.transform.position = data.Position;
                spinnerController.transform.eulerAngles = data.Angles;
                spinnerController.gameObject.SetActive(true);
            }

            //Create bombs
            foreach(BombData data in levelData.ListBombData) 
            {
                BombController bombController = PoolManager.Instance.GetBombController(data.BombID);
                bombController.transform.position = data.Position;
                bombController.transform.eulerAngles = data.Angles;
                bombController.gameObject.SetActive(true);
            }

            //Create doors
            foreach(DoorData data in levelData.ListDoorData)
            {
                DoorController doorController = PoolManager.Instance.GetDoorController(data.DoorID);
                doorController.transform.position = data.Position;
                doorController.transform.eulerAngles = data.Angles;
                doorController.gameObject.SetActive(true);
            }


            backgroundMusic = backgroundAudioClips[Random.Range(0, backgroundAudioClips.Length)];
            Invoke(nameof(PlayingGame), 0.15f);
        }


        private void Update()
        {
            if (ingameState == IngameState.Ingame_Playing)
            {
                if (Input.GetMouseButtonDown(0) && Utilities.IsFinishedTutorial())
                {
                    Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    if (ballIndicator.IsOverlap(mousePos) && currentBallAmount > 0)
                    {
                        //User is touching the ball indicator to create the ball
                        isCreatingBall = true;
                        isDrawingLine = false;
                        ballIndicator.OnMouseStart(mousePos);
                    }
                    else if(currentDrawAmount > 0)
                    {
                        //User is drawing the line
                        isDrawingLine = true;
                        isCreatingBall = false;
                        lineController = PoolManager.Instance.GetLineController();
                        lineController.transform.position = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        lineController.gameObject.SetActive(true);
                        lineController.OnSetup();
                    }
                }
                else if (Input.GetMouseButton(0))
                {
                    Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    if (isDrawingLine && !ballIndicator.IsOverlap(mousePos))
                    {
                        lineController.SetPoint(mousePos);
                    }
                    else if (isCreatingBall)
                    {
                        ballIndicator.OnMouseUpdate(mousePos);
                    }
                }


                if (Input.GetMouseButtonUp(0))
                {
                    if (isDrawingLine)
                    {
                        lineController.DisableLineTemp();
                        lineController = null;
                        currentDrawAmount--;
                        ViewManager.Instance.IngameViewController.UpdateValues(currentBallAmount, currentDrawAmount, currentFadeAmount);
                    }
                    else if (isCreatingBall)
                    {
                        ballIndicator.OnMouseEnd();
                        currentBallAmount--;
                        ViewManager.Instance.IngameViewController.UpdateValues(currentBallAmount, currentDrawAmount, currentFadeAmount);
                    }

                    isCreatingBall = false;
                    isDrawingLine = false;
                }

                if (Input.GetKeyDown(KeyCode.Q) && IngameManager.Instance.currentFadeAmount >= 0)
                {
                    currentFadeAmount--;
                    int display = currentFadeAmount > 0 ? currentFadeAmount : 0;
                    ViewManager.Instance.IngameViewController.UpdateValues(currentBallAmount, currentDrawAmount, display);
                }
            }
        }



        /// <summary>
        /// Call IngameState.Ingame_Playing event and handle other actions.
        /// Actual start the game.
        /// </summary>
        private void PlayingGame()
        {
            //Fire event
            IngameState = IngameState.Ingame_Playing;
            ingameState = IngameState.Ingame_Playing;

            //Other actions
            if (IsRevived)
            {
                StartCoroutine(CRShowingViewWithDelay(ViewType.INGAME_VIEW, 0f));
                ServicesManager.Instance.SoundManager.ResumeMusic(0.5f);

                currentBallAmount = levelData.BallAmount;
                currentDrawAmount = levelData.DrawAmount;
                ViewManager.Instance.IngameViewController.UpdateValues(currentBallAmount, currentDrawAmount, currentFadeAmount, true);
            }
            else
            {
                ServicesManager.Instance.SoundManager.PlayMusic(backgroundMusic, 0.5f);
                ViewManager.Instance.IngameViewController.UpdateValues(currentBallAmount, currentDrawAmount, currentFadeAmount, true);
            }
        }


        /// <summary>
        /// Call IngameState.Ingame_Revive event and handle other actions.
        /// </summary>
        public void Revive()
        {
            //Fire event
            IngameState = IngameState.Ingame_Revive;
            ingameState = IngameState.Ingame_Revive;

            //Add another actions here
            StartCoroutine(CRShowingViewWithDelay(ViewType.REVIVE_VIEW, 0.5f));
            ServicesManager.Instance.SoundManager.PauseMusic(0.5f);
        }


        /// <summary>
        /// Call IngameState.Ingame_GameOver event and handle other actions.
        /// </summary>
        public void GameOver()
        {
            //Fire event
            IngameState = IngameState.Ingame_GameOver;
            ingameState = IngameState.Ingame_GameOver;

            //Add another actions here
            StartCoroutine(CRShowingViewWithDelay(ViewType.ENDGAME_VIEW, 0.25f));
            ServicesManager.Instance.SoundManager.StopMusic(0.5f);
            ServicesManager.Instance.SoundManager.PlaySound(ServicesManager.Instance.SoundManager.LevelFailed);
        }


        /// <summary>
        /// Call IngameState.Ingame_CompleteLevel event and handle other actions.
        /// </summary>
        public void CompletedLevel()
        {
            //Fire event
            IngameState = IngameState.Ingame_CompleteLevel;
            ingameState = IngameState.Ingame_CompleteLevel;

            //Other actions
            StartCoroutine(CRShowingViewWithDelay(ViewType.ENDGAME_VIEW, 1f));
            ServicesManager.Instance.SoundManager.StopMusic(0.5f);
            ServicesManager.Instance.SoundManager.PlaySound(ServicesManager.Instance.SoundManager.LevelCompleted);
            foreach (ParticleSystem o in confettiEffects)
            {
                o.Play();
            }

            //Save level
            if (testingLevel == 0)
            {
                PlayerPrefs.SetInt(PlayerPrefsKeys.PPK_SAVED_LEVEL, PlayerPrefs.GetInt(PlayerPrefsKeys.PPK_SAVED_LEVEL) + 1);

                //Report level to leaderboard
                string username = PlayerPrefs.GetString(PlayerPrefsKeys.PPK_SAVED_USER_NAME);
                if (!string.IsNullOrEmpty(username))
                {
                    ServicesManager.Instance.LeaderboardManager.SetPlayerLeaderboardData();
                }
            }
        }

        /// <summary>
        /// Delay for an amount of time then show the view with given viewType.
        /// </summary>
        /// <param name="viewType"></param>
        /// <param name="delay"></param>
        /// <returns></returns>
        private IEnumerator CRShowingViewWithDelay(ViewType viewType, float delay)
        {
            yield return new WaitForSeconds(delay);
            ViewManager.Instance.OnShowView(viewType);
        }



        //////////////////////////////////////Publish functions



        /// <summary>
        /// Continue the game
        /// </summary>
        public void SetContinueGame()
        {
            IsRevived = true;
            Invoke(nameof(PlayingGame), 0.05f);
        }


        /// <summary>
        /// Handle action when a balloon exploded.
        /// </summary>
        public void OnBalloonExploded()
        {
            if (PoolManager.Instance.AreAllBalloonsDisable())
            {
                CompletedLevel();
            }
        }


        /// <summary>
        /// Handle action when a ball disable.
        /// </summary>
        public void OnBallDisable()
        {
            if (currentBallAmount == 0 && !PoolManager.Instance.AreAllBalloonsDisable() && PoolManager.Instance.AreAllBallsDisable())
            {
                if (IsRevived || !ServicesManager.Instance.AdManager.IsRewardedAdReady())
                {
                    //Fire event
                    IngameState = IngameState.Ingame_GameOver;
                    ingameState = IngameState.Ingame_GameOver;

                    //Add another actions here
                    StartCoroutine(CRShowingViewWithDelay(ViewType.ENDGAME_VIEW, 1f));
                    ServicesManager.Instance.SoundManager.StopMusic(0.5f);
                    ServicesManager.Instance.SoundManager.PlaySound(ServicesManager.Instance.SoundManager.LevelFailed);
                }
                else
                {
                    Revive();
                }
            }
        }
    }
}
