namespace VED.Utilities
{
    public class GameManager : SingletonMonoBehaviour<GameManager>
    {
        public StateManager StateManager => _stateManager;
        private StateManager _stateManager = new StateManager();

        private void Update()
        {
            TimeManager.Instance.Tick();
            _stateManager.Tick();
        }

        private void FixedUpdate()
        {
            _stateManager.FixedTick();
        }
    }
}
