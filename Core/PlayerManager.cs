using UnityEngine;
using StudioScor.Utilities;

namespace StudioScor.PlayerSystem
{
    [CreateAssetMenu(menuName ="StudioScor/PlayerSystem/new PlayerManager")]
    public class PlayerManager : BaseScriptableObject
    {
        #region Event
        public delegate void SetPlayerPawnEventHandler(PlayerManager playerManager, IPawnSystem currentPawn, IPawnSystem prevPawn = null);
        public delegate void SetPlayerContollerEventHandler(PlayerManager playerManager, IControllerSystem currentController, IControllerSystem prevController = null);
        #endregion

        [Header(" [ Player Manager ]")]
        [SerializeField] private GameObject _defaultPlayerController;
        [SerializeField] private GameObject _defaultPlayerPawn;

        [SerializeField][SReadOnly] private GameObject _currentPlayerController;
        [SerializeField][SReadOnly] private GameObject _currentPlayerPawn;
        private IControllerSystem _playerController;
        private IPawnSystem _playerPawn;

        public IControllerSystem PlayerController => _playerController;
        public IPawnSystem PlayerPawn => _playerPawn;

        public bool HasPlayerPawn => (Object)PlayerPawn;
        public bool HasPlayerController => (Object)PlayerController;

        public event SetPlayerPawnEventHandler OnChangedPlayerPawn;
        public event SetPlayerContollerEventHandler OnChangedPlayerController;

        protected override void OnReset()
        {
            base.OnReset();

            _playerController = null;
            _playerPawn = null;

            _currentPlayerController = null;
            _currentPlayerPawn = null;
        }

        public void SpawnPlayerPawn(Vector3 position, Quaternion rotation)
        {
            var instance = Instantiate(_defaultPlayerPawn, position, rotation);

            if (!instance.TryGetPawnSystem(out IPawnSystem pawnSystem))
                LogError($"{instance} is Not Has IPawnSystem");

            pawnSystem.SetStartPlayer(true);

            if (!HasPlayerController)
            {
                SpawnPlayerController();
            }
        }

        public void SpawnPlayerController()
        {
            Instantiate(_defaultPlayerController);
        }

        public void ForceSetPlayerPawn(IPawnSystem pawnSystem)
        {
            Log("Force Set Player Pawn : " + pawnSystem);
            Log("Has Player Pawn : " + HasPlayerPawn);
            Log("Current Player Pawn : " + _playerPawn);

            if (HasPlayerPawn && _playerPawn == pawnSystem)
                return;

            var prevPawn = _playerPawn;
            _playerPawn = pawnSystem;

            _currentPlayerPawn = _playerPawn is null ? null : _playerPawn.gameObject;

            if (HasPlayerController)
            {
                Log(HasPlayerPawn ? "On Possess" : "Un Possess");

                _playerController.Possess(_playerPawn);
            }

            Invoke_OnChangedPlayerPawn(prevPawn);
        }

        public void ForceSetPlayerController(IControllerSystem controller)
        {
            Log("Force Set Player Controller : " + controller);
            Log("Has Player Controller : " + HasPlayerController);
            Log("Current Player Controller : " + _playerController);

            if (HasPlayerController && _playerController == controller)
                return;

            var prevController = _playerController;
            _playerController = controller;

            _currentPlayerController = _playerController.gameObject;

            if (_playerController is not null)
            {
                _playerController.Possess(_playerPawn);
            }

            Invoke_OnChangedPlayerController(prevController);
        }

        #region Invoke
        protected void Invoke_OnChangedPlayerController(IControllerSystem prevController)
        {
            Log($"{nameof(OnChangedPlayerController)} - Current : " + PlayerController + " Prev : " + prevController);

            OnChangedPlayerController?.Invoke(this, _playerController, prevController);
        }
        protected void Invoke_OnChangedPlayerPawn(IPawnSystem prevPawn)
        {
            Log($"{nameof(OnChangedPlayerPawn)} - Current :" + _playerPawn + " Prev : " + prevPawn);   

            OnChangedPlayerPawn?.Invoke(this, _playerPawn, prevPawn);
        }
        #endregion
    }

}

