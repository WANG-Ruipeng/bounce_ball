using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;

namespace ClawbearGames
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private BallIndicator ballIndicatorPrefab = null;
        [SerializeField] private BalloonController balloonControllerPrefab = null;
        [SerializeField] private ObstacleController[] obstacleControllerPrefabs = null;
        [SerializeField] private BlackHoleController[] blackHoleControllerPrefabs = null;
        [SerializeField] private SpinnerController[] spinnerControllerPrefabs = null;
        [SerializeField] private BombController[] bombControllerPrefabs = null;
        [SerializeField] private DoorController[] doorControllerPrefabs = null;
        public Material backgroundMaterial = null;

        [HideInInspector] public int BallAmount = 3;
        [HideInInspector] public int DrawAmount = 2;
        [HideInInspector] public int FadeAmount = 3;
        [HideInInspector] public Color TopBackgroundColor = Color.blue;
        [HideInInspector] public Color BottomBackgroundColor = Color.gray;

        public const int ScreenshotWidth = 500;
        public const int ScreenshotHeight = 281;




        /// <summary>
        /// Get the json path of the level's data.
        /// </summary>
        /// <param name="levelNumber"></param>
        /// <returns></returns>
        private string JsonPath(int levelNumber)
        {
            return "Assets/_Balloon_Popper/Resources/Levels/" + levelNumber.ToString() + ".json";
        }


        /// <summary>
        /// Get the screenshot path of the level's image.
        /// </summary>
        /// <param name="levelNumber"></param>
        /// <returns></returns>
        public static string ScreenshotPath(int levelNumber)
        {
            string path = "Assets/_Balloon_Popper/Sprites/Levels/" + levelNumber.ToString() + ".png";
            return path;
        }


        /// <summary>
        /// Get the total levels.
        /// </summary>
        /// <returns></returns>
        public int TotalLevel()
        {
            return Resources.LoadAll("Levels").Length;
        }



        /// <summary>
        /// Create the level json file. 
        /// </summary>
        public void CreateLevel()
        {
            FileStream fs = new FileStream(JsonPath(TotalLevel() + 1), FileMode.Create);
            fs.Close();
#if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh();
#endif
        }



        /// <summary>
        /// Save the given level.
        /// </summary>
        /// <param name="levelNumber"></param>
        public void SaveLevel(int levelNumber)
        {
            StartCoroutine(CRSnapshotLevel(levelNumber));
            File.WriteAllText(JsonPath(levelNumber), GetLevelData(levelNumber));
#if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh();
#endif
        }



        /// <summary>
        /// Lad the given level.
        /// </summary>
        /// <param name="levelNumber"></param>
        public void LoadLevel(int levelNumber)
        {
            ClearScene();
            TextAsset tex = Resources.Load("Levels/" + levelNumber.ToString()) as TextAsset;
            LevelData levelData = JsonUtility.FromJson<LevelData>(tex.ToString().Trim());

            BallAmount = levelData.BallAmount;
            DrawAmount = levelData.DrawAmount;
            FadeAmount = levelData.FadeAmount;
            TopBackgroundColor = Utilities.HexToColor(levelData.TopBackgroundColorHex);
            BottomBackgroundColor = Utilities.HexToColor(levelData.BottomBackgroundColorHex);

            //Create ball indicator object
            Instantiate(ballIndicatorPrefab, levelData.BallIndicatorPosition, Quaternion.identity);

            //Create balloon objects
            foreach(BalloonData data in levelData.ListBalloonData)
            {
                BalloonController balloonController = Instantiate(balloonControllerPrefab, data.Position, Quaternion.identity);
                balloonController.transform.localScale = data.Scale;
                balloonController.SpriteRenderer.color = Utilities.HexToColor(data.HexColor);
            }

            //Create obstacle objects
            foreach(ObstacleData data in levelData.ListObstacleData)
            {
                ObstacleController prefab = obstacleControllerPrefabs.Where(a => a.ObstacleID.Equals(data.ObstacleID)).FirstOrDefault();
                ObstacleController obstacleController = Instantiate(prefab, data.Position, Quaternion.Euler(data.Angles));
                obstacleController.transform.localScale = data.Scale;
                obstacleController.SpriteRenderer.color = Utilities.HexToColor(data.HexColor);
            }

            //Create black hole objects
            foreach (BlackHoleData data in levelData.ListBlackHoleData)
            {
                BlackHoleController prefab = blackHoleControllerPrefabs.Where(a => a.BlackHoleID.Equals(data.BlackHoleID)).FirstOrDefault();
                BlackHoleController blackHoleController = Instantiate(prefab, data.Position, Quaternion.identity);
                blackHoleController.GetComponent<ParticleSystem>().Play();
            }

            //Create spinner objects
            foreach (SpinnerData data in levelData.ListSpinnerData)
            {
                SpinnerController prefab = spinnerControllerPrefabs.Where(a => a.SpinnerID.Equals(data.SpinnerID)).FirstOrDefault();
                SpinnerController spinnerController = Instantiate(prefab, data.Position, Quaternion.Euler(data.Angles));
                spinnerController.transform.localScale = data.Scale;
            }

            //Create bomb objects
            foreach (BombData data in levelData.ListBombData)
            {
                BombController prefab = bombControllerPrefabs.Where(a => a.BombID.Equals(data.BombID)).FirstOrDefault();
                Instantiate(prefab, data.Position, Quaternion.Euler(data.Angles));
            }

            //Create door objects
            foreach (DoorData data in levelData.ListDoorData)
            {
                DoorController prefab = doorControllerPrefabs.Where(a => a.DoorID.Equals(data.DoorID)).FirstOrDefault();
                Instantiate(prefab, data.Position, Quaternion.Euler(data.Angles));
            }
        }


        /// <summary>
        /// Clear the scene.
        /// </summary>
        public void ClearScene()
        {
            BallIndicator ballIndicator = FindObjectOfType<BallIndicator>();
            if (ballIndicator != null)
                DestroyImmediate(ballIndicator.gameObject);

            BalloonController[] balloonControllers = FindObjectsOfType<BalloonController>();
            foreach (BalloonController o in balloonControllers)
            {
                DestroyImmediate(o.gameObject);
            }

            ObstacleController[] obstacleControllers = FindObjectsOfType<ObstacleController>();
            foreach (ObstacleController o in obstacleControllers)
            {
                DestroyImmediate(o.gameObject);
            }

            BlackHoleController[] blackHoleControllers = FindObjectsOfType<BlackHoleController>();
            foreach (BlackHoleController o in blackHoleControllers)
            {
                DestroyImmediate(o.gameObject);
            }

            SpinnerController[] spinnerControllers = FindObjectsOfType<SpinnerController>();
            foreach (SpinnerController o in spinnerControllers)
            {
                DestroyImmediate(o.gameObject);
            }

            BombController[] bombControllers = FindObjectsOfType<BombController>();
            foreach (BombController o in bombControllers)
            {
                DestroyImmediate(o.gameObject);
            }

            DoorController[] doorControllers = FindObjectsOfType<DoorController>();
            foreach (DoorController o in doorControllers)
            {
                DestroyImmediate(o.gameObject);
            }
        }


        /// <summary>
        /// determine whether the given level's data is null.
        /// </summary>
        /// <param name="levelNumber"></param>
        /// <returns></returns>
        public bool IsLevelNullData(int levelNumber)
        {
            string data = File.ReadAllText(JsonPath(levelNumber)).Trim();
            return string.IsNullOrEmpty(data);
        }



        /// <summary>
        /// Get the LevelData in string.
        /// </summary>
        /// <param name="levelNumber"></param>
        /// <returns></returns>
        private string GetLevelData(int levelNumber)
        {
            LevelData levelData = new LevelData();
            levelData.LevelNumber = levelNumber;

            //Set parameters
            levelData.BallAmount = BallAmount;
            levelData.DrawAmount = DrawAmount;
            levelData.FadeAmount = FadeAmount;
            levelData.TopBackgroundColorHex = Utilities.ColorToHex(TopBackgroundColor);
            levelData.BottomBackgroundColorHex = Utilities.ColorToHex(BottomBackgroundColor);
            levelData.BallIndicatorPosition = FindObjectOfType<BallIndicator>().transform.position;

            //Set ListBalloonData
            levelData.ListBalloonData = new List<BalloonData>();
            BalloonController[] balloonControllers = FindObjectsOfType<BalloonController>();
            foreach (BalloonController balloon in balloonControllers)
            {
                BalloonData data = new BalloonData();
                data.Position = balloon.transform.position;
                data.Scale = balloon.transform.localScale;
                data.HexColor = Utilities.ColorToHex(balloon.SpriteRenderer.color);
                levelData.ListBalloonData.Add(data);
            }


            //Set ListObstacleData
            levelData.ListObstacleData = new List<ObstacleData>();
            ObstacleController[] obstacleControllers = FindObjectsOfType<ObstacleController>();
            foreach (ObstacleController obstacle in obstacleControllers)
            {
                ObstacleData data = new ObstacleData();
                data.ObstacleID = obstacle.ObstacleID;
                data.Position = obstacle.transform.position;
                data.Angles = obstacle.transform.localEulerAngles;
                data.Scale = obstacle.transform.localScale;
                data.HexColor = Utilities.ColorToHex(obstacle.SpriteRenderer.color);
                levelData.ListObstacleData.Add(data);
            }


            //Set ListBlackHoleData
            levelData.ListBlackHoleData = new List<BlackHoleData>();
            BlackHoleController[] blackHoleControllers = FindObjectsOfType<BlackHoleController>();
            foreach (BlackHoleController obstacle in blackHoleControllers)
            {
                BlackHoleData data = new BlackHoleData();
                data.BlackHoleID = obstacle.BlackHoleID;
                data.Position = obstacle.transform.position;
                levelData.ListBlackHoleData.Add(data);
            }


            //Set ListBombData
            levelData.ListBombData = new List<BombData>();
            BombController[] bombControllers = FindObjectsOfType<BombController>();
            foreach (BombController bomb in bombControllers)
            {
                BombData data = new BombData();
                data.BombID = bomb.BombID;
                data.Position = bomb.transform.position;
                data.Angles = bomb.transform.localEulerAngles;
                levelData.ListBombData.Add(data);
            }

            //Set ListDoorData
            levelData.ListDoorData = new List<DoorData>();
            DoorController[] doorControllers = FindObjectsOfType<DoorController>();
            foreach (DoorController door in doorControllers)
            {
                DoorData data = new DoorData();
                data.DoorID = door.DoorID;
                data.Position = door.transform.position;
                data.Angles = door.transform.localEulerAngles;
                levelData.ListDoorData.Add(data);
            }

            //Set ListSpinnerData
            levelData.ListSpinnerData = new List<SpinnerData>();
            SpinnerController[] spinnerControllers = FindObjectsOfType<SpinnerController>();
            foreach (SpinnerController spinner in spinnerControllers)
            {
                SpinnerData data = new SpinnerData();
                data.SpinnerID = spinner.SpinnerID;
                data.Position = spinner.transform.position;
                data.Angles = spinner.transform.localEulerAngles;
                data.Scale = spinner.transform.localScale;
                levelData.ListSpinnerData.Add(data);
            }

            return JsonUtility.ToJson(levelData);
        }



        /// <summary>
        /// coroutine snapshot the level's image.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="levelNumber"></param>
        /// <returns></returns>
        private IEnumerator CRSnapshotLevel(int levelNumber)
        {
            // Capture screenshot using render texture
            RenderTexture rt = new RenderTexture(ScreenshotWidth, ScreenshotHeight, 32, RenderTextureFormat.ARGB32);
            yield return null;
            Camera.main.targetTexture = rt;
            Camera.main.Render();
            Camera.main.targetTexture = null;
            RenderTexture.active = rt;
            Texture2D tx = new Texture2D(ScreenshotWidth, ScreenshotHeight, TextureFormat.ARGB32, false);
            tx.ReadPixels(new Rect(0, 0, ScreenshotWidth, ScreenshotHeight), 0, 0);
            tx.Apply();
            RenderTexture.active = null;
            DestroyImmediate(rt);
            byte[] bytes = tx.EncodeToPNG();
            File.WriteAllBytes(ScreenshotPath(levelNumber), bytes);
#if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh();
#endif
        }
    }
}
