using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static UnityEngine.Networking.UnityWebRequest;

namespace ClawbearGames
{
    public class PoolManager : MonoBehaviour
    {
        public static PoolManager Instance { private set; get; }

        [SerializeField] private BallController ballControllerPrefab = null;
        [SerializeField] private LineController lineControllerPrefab = null;
        [SerializeField] private ProjectileDotController projectileDotControllerPrefab = null;
        [SerializeField] private BalloonController balloonControllerPrefab = null;
        [SerializeField] private ObstacleController[] obstacleControllerPrefabs = null;
        [SerializeField] private BlackHoleController[] blackHoleControllerPrefabs = null;
        [SerializeField] private SpinnerController[] spinnerControllerPrefabs = null;
        [SerializeField] private BombController[] bombControllerPrefabs = null;
        [SerializeField] private DoorController[] doorControllerPrefabs = null;

        private List<BallController> listBallController = new List<BallController>();
        private List<LineController> listLineController = new List<LineController>();
        private List<ProjectileDotController> listProjectileDotController = new List<ProjectileDotController>();
        private List<BalloonController> listBalloonController = new List<BalloonController>();
        private List<ObstacleController> listObstacleController = new List<ObstacleController>();
        private List<BlackHoleController> listBlackHoleController = new List<BlackHoleController>();
        private List<SpinnerController> listSpinnerController = new List<SpinnerController>();
        private List<BombController> listBombController = new List<BombController>();
        private List<DoorController> listDoorController = new List<DoorController>();

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
        /// Disable all projectile dots.
        /// </summary>
        public void DisableAllProjectileDots()
        {
            foreach(ProjectileDotController o in listProjectileDotController)
            {
                o.gameObject.SetActive(false);
            }
        }



        /// <summary>
        /// Determine whether all the balloons are disable.
        /// </summary>
        /// <returns></returns>
        public bool AreAllBalloonsDisable()
        {
            bool result = true;
            foreach(BalloonController balloon in listBalloonController)
            {
                if (balloon.gameObject.activeSelf)
                {
                    result = false;
                    break;
                }
            }

            return result;
        }


        /// <summary>
        /// Determine whether all the balls are disable.
        /// </summary>
        /// <returns></returns>
        public bool AreAllBallsDisable()
        {
            bool result = true;
            foreach (BallController ball in listBallController)
            {
                if (ball.gameObject.activeSelf)
                {
                    result = false;
                    break;
                }
            }

            return result;
        }


        /// <summary>
        /// Get an inactive BallController object.
        /// </summary>
        /// <returns></returns>
        public BallController GetBallController()
        {
            //Find in the list
            BallController ballController = listBallController.Where(a => !a.gameObject.activeSelf).FirstOrDefault();

            if (ballController == null)
            {
                //Didn't find one -> create new one
                ballController = Instantiate(ballControllerPrefab, Vector3.zero, Quaternion.identity);
                ballController.gameObject.SetActive(false);
                listBallController.Add(ballController);
            }

            return ballController;
        }



        /// <summary>
        /// Get an inactive ProjectileDotController object.
        /// </summary>
        /// <returns></returns>
        public ProjectileDotController GetProjectileDotController()
        {
            //Find in the list
            ProjectileDotController projectileDotController = listProjectileDotController.Where(a => !a.gameObject.activeSelf).FirstOrDefault();

            if (projectileDotController == null)
            {
                //Didn't find one -> create new one
                projectileDotController = Instantiate(projectileDotControllerPrefab, Vector3.zero, Quaternion.identity);
                projectileDotController.gameObject.SetActive(false);
                listProjectileDotController.Add(projectileDotController);
            }

            return projectileDotController;
        }



        /// <summary>
        /// Get an inactive LineController object.
        /// </summary>
        /// <returns></returns>
        public LineController GetLineController()
        {
            //Find in the list
            LineController lineController = listLineController.Where(a => !a.gameObject.activeSelf).FirstOrDefault();

            if (lineController == null)
            {
                //Didn't find one -> create new one
                lineController = Instantiate(lineControllerPrefab, Vector3.zero, Quaternion.identity);
                lineController.gameObject.SetActive(false);
                listLineController.Add(lineController);
            }

            return lineController;
        }



        /// <summary>
        /// Get an inactive BalloonController object.
        /// </summary>
        /// <returns></returns>
        public BalloonController GetBalloonController()
        {
            //Find in the list
            BalloonController balloonController = listBalloonController.Where(a => !a.gameObject.activeSelf).FirstOrDefault();

            if (balloonController == null)
            {
                //Didn't find one -> create new one
                balloonController = Instantiate(balloonControllerPrefab, Vector3.zero, Quaternion.identity);
                balloonController.gameObject.SetActive(false);
                listBalloonController.Add(balloonController);
            }

            return balloonController;
        }


        /// <summary>
        /// Get an inactive ObstacleController object.
        /// </summary>
        /// <param name="obstacleID"></param>
        /// <returns></returns>
        public ObstacleController GetObstacleController(string obstacleID)
        {
            //Find in the list
            ObstacleController obstacleController = listObstacleController.Where(a => !a.gameObject.activeSelf && a.ObstacleID.Equals(a.ObstacleID)).FirstOrDefault();

            if (obstacleController == null)
            {
                //Didn't find one -> create new one
                ObstacleController prefab = obstacleControllerPrefabs.Where(a => a.ObstacleID.Equals(obstacleID)).FirstOrDefault();
                obstacleController = Instantiate(prefab, Vector3.zero, Quaternion.identity);
                obstacleController.gameObject.SetActive(false);
                listObstacleController.Add(obstacleController);
            }

            return obstacleController;
        }



        /// <summary>
        ///  Get an inactive BlackHoleController object.
        /// </summary>
        /// <param name="blackHoleID"></param>
        /// <returns></returns>
        public BlackHoleController GetBlackHoleController(string blackHoleID)
        {
            //Find in the list
            BlackHoleController blackHoleController = listBlackHoleController.Where(a => !a.gameObject.activeSelf && a.BlackHoleID.Equals(a.BlackHoleID)).FirstOrDefault();

            if (blackHoleController == null)
            {
                //Didn't find one -> create new one
                BlackHoleController prefab = blackHoleControllerPrefabs.Where(a => a.BlackHoleID.Equals(blackHoleID)).FirstOrDefault();
                blackHoleController = Instantiate(prefab, Vector3.zero, Quaternion.identity);
                blackHoleController.gameObject.SetActive(false);
                listBlackHoleController.Add(blackHoleController);
            }

            return blackHoleController;
        }


        /// <summary>
        /// Get an inactive SpinnerController object.
        /// </summary>
        /// <param name="spinnerID"></param>
        /// <returns></returns>
        public SpinnerController GetSpinnerController(string spinnerID)
        {
            //Find in the list
            SpinnerController spinnerController = listSpinnerController.Where(a => !a.gameObject.activeSelf && a.SpinnerID.Equals(a.SpinnerID)).FirstOrDefault();

            if (spinnerController == null)
            {
                //Didn't find one -> create new one
                SpinnerController prefab = spinnerControllerPrefabs.Where(a => a.SpinnerID.Equals(spinnerID)).FirstOrDefault();
                spinnerController = Instantiate(prefab, Vector3.zero, Quaternion.identity);
                spinnerController.gameObject.SetActive(false);
                listSpinnerController.Add(spinnerController);
            }

            return spinnerController;
        }


        /// <summary>
        /// Get an inactive BombController object.
        /// </summary>
        /// <param name="bombID"></param>
        /// <returns></returns>
        public BombController GetBombController(string bombID)
        {
            //Find in the list
            BombController bombController = listBombController.Where(a => !a.gameObject.activeSelf && a.BombID.Equals(a.BombID)).FirstOrDefault();

            if (bombController == null)
            {
                //Didn't find one -> create new one
                BombController prefab = bombControllerPrefabs.Where(a => a.BombID.Equals(bombID)).FirstOrDefault();
                bombController = Instantiate(prefab, Vector3.zero, Quaternion.identity);
                bombController.gameObject.SetActive(false);
                listBombController.Add(bombController);
            }

            return bombController;
        }


        /// <summary>
        /// Get an inactive DoorController object.
        /// </summary>
        /// <param name="doorID"></param>
        /// <returns></returns>
        public DoorController GetDoorController(string doorID)
        {
            //Find in the list
            DoorController doorController = listDoorController.Where(a => !a.gameObject.activeSelf && a.DoorID.Equals(a.DoorID)).FirstOrDefault();

            if (doorController == null)
            {
                //Didn't find one -> create new one
                DoorController prefab = doorControllerPrefabs.Where(a => a.DoorID.Equals(doorID)).FirstOrDefault();
                doorController = Instantiate(prefab, Vector3.zero, Quaternion.identity);
                doorController.gameObject.SetActive(false);
                listDoorController.Add(doorController);
            }

            return doorController;
        }
    }
}
