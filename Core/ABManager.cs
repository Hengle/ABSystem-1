using UnityEngine;

namespace ABSystem
{
    public class ABManager : MonoBehaviour
    {

        public ABUpdater Updater { get; private set; }

        public ABUpdater.ABRemoteSetting RemoteSetting;
        public ABUpdater.ABLocalSetting LocalSetting;
        public bool AutoUpdate;

        public static ABManager Instance;

        private void Awake()
        {
            Instance = this;
            Updater = new ABUpdater(RemoteSetting, LocalSetting);
        }

        void Start()
        {
            if (AutoUpdate)
            {
                Updater.Check();
                if (Updater.HasNewVersion)
                {
                    Updater.StartUpdate();
                }
            }
        }

    }

}


