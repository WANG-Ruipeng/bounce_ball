using UnityEngine;

namespace ClawbearGames
{
    public class ServicesManager : MonoBehaviour
    {
        public static ServicesManager Instance { private set; get; }

        [SerializeField] private AdManager adManager = null;
        [SerializeField] private SoundManager soundManager = null;
        [SerializeField] private ShareManager shareManager = null;
        [SerializeField] private LeaderboardManager leaderboardManager = null;
        [SerializeField] private NotificationManager notificationManager = null;


        public SoundManager SoundManager { get { return soundManager; } }
        public ShareManager ShareManager { get { return shareManager; } }
        public LeaderboardManager LeaderboardManager { get { return leaderboardManager; } }
        public NotificationManager NotificationManager { get { return notificationManager; } }
        public AdManager AdManager { get { return adManager; } }

        private void Awake()
        {
            if (Instance)
            {
                DestroyImmediate(gameObject);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }
    }
}

