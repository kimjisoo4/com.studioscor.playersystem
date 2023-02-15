using UnityEngine;
using StudioScor.Utilities;

namespace StudioScor.PlayerSystem
{
    [CreateAssetMenu(menuName ="StudioScor/PlayerSystem/new PlayerManager")]
    public class PlayerManager : BaseScriptableObject
    {
        #region Event
        public delegate void SetPlayerPawnEventHandler(PlayerManager playerManager, PawnComponent currentPawn, PawnComponent prevPawn = null);
        public delegate void SetPlayerContollerEventHandler(PlayerManager playerManager, ControllerComponent currentController, ControllerComponent prevController = null);
        #endregion

        [Header(" [ Player Manager ]")]
        [SerializeField] private ControllerComponent _DefaultPlayerController;
        [SerializeField] private PawnComponent _DefaultPlayerPawn;

        private ControllerComponent _PlayerController;
        private PawnComponent _PlayerPawn;

        public ControllerComponent PlayerController => _PlayerController;
        public PawnComponent PlayerPawn => _PlayerPawn;

        public bool HasPlayerPawn => PlayerPawn != null;
        public bool HasPlayerController => PlayerController != null;

        public event SetPlayerPawnEventHandler OnChangedPlayerPawn;
        public event SetPlayerContollerEventHandler OnChangedPlayerController;

        public void SpawnPlayerPawn(Vector3 position, Quaternion rotation)
        {
            var pawn = Instantiate(_DefaultPlayerPawn, position, rotation);

            pawn.SetStartPlayer(true);

            if (!HasPlayerController)
            {
                SpawnPlayerController();
            }
        }
        public void SpawnPlayerController()
        {
            Instantiate(_DefaultPlayerController);
        }

        public void ForceSetPlayerPawn(PawnComponent pawnComponent)
        {
            Log("Force Set Player Pawn : " + pawnComponent);
            Log("Has Player Pawn : " + HasPlayerPawn);
            Log("Current Player Pawn : " + _PlayerPawn);

            if (HasPlayerPawn && _PlayerPawn == pawnComponent)
                return;

            var prevPawn = _PlayerPawn;
            _PlayerPawn = pawnComponent;

            if (HasPlayerController)
            {
                Log(HasPlayerPawn ? "On Possess" : "Un Possess");

                _PlayerController.OnPossess(_PlayerPawn);
            }

            Callback_OnChangedPlayerPawn(prevPawn);
        }

        public void ForceSetPlayerController(ControllerComponent controller)
        {
            Log("Force Set Player Controller : " + controller);
            Log("Has Player Controller : " + HasPlayerController);
            Log("Current Player Controller : " + _PlayerController);

            if (HasPlayerController && _PlayerController == controller)
                return;

            var prevController = _PlayerController;
            _PlayerController = controller;

            if (_PlayerController)
            {
                _PlayerController.OnPossess(_PlayerPawn);
            }

            Callback_OnChangedPlayerController(prevController);
        }

        #region Callback
        protected void Callback_OnChangedPlayerController(ControllerComponent prevController)
        {
            Log("On Changed PlayerController - Current : " + PlayerController + " Prev : " + prevController);

            OnChangedPlayerController?.Invoke(this, _PlayerController, prevController);
        }
        protected void Callback_OnChangedPlayerPawn(PawnComponent prevPawn)
        {
            Log("On Changed PlayerPawn - Current :" + _PlayerPawn + " Prev : " + prevPawn);   

            OnChangedPlayerPawn?.Invoke(this, _PlayerPawn, prevPawn);
        }
        #endregion
    }

}

