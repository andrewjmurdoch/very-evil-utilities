namespace VED.Utilities
{
    public class GameManager : SingletonMonoBehaviour<GameManager>
    {
        public StateManager StateManager => _stateManager;
        private StateManager _stateManager = null;

        protected override void Awake()
        {
            Init();
        }

        public void Init()
        {
            if (_stateManager != null) return;

            _stateManager = new StateManager();
            TimeManager.Instance.Init();
        }

        private void Update()
        {
            TimeManager.Instance.Tick();
            _stateManager.Tick();
        }

        private void LateUpdate()
        {
            _stateManager.LateTick();
        }

        private void FixedUpdate()
        {
            TimeManager.Instance.FixedTick();
            _stateManager.FixedTick();
        }
    }
}
