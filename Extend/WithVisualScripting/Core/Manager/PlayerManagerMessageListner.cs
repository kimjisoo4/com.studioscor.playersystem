#if SCOR_ENABLE_VISUALSCRIPTING
using Unity.VisualScripting;

namespace StudioScor.PlayerSystem.VisualScripting
{
    public class PlayerManagerMessageListner : MessageListener
    {
        public class OnChangedPlayerControllerValue
        {
            public ControllerComponent CurrentController;
            public ControllerComponent PrevController;
        }
        public class OnChangedPlayerPawnValue
        {
            public PawnComponent CurrentPawn;
            public PawnComponent PrevPawn;
        }

        private OnChangedPlayerControllerValue _OnChangedPlayerControllerValue;
        private OnChangedPlayerPawnValue _OnChangedPlayerPawnValue;

        private void Awake()
        {
            PlayerManager.Instance.OnChangedPlayerController += Instance_OnChangedPlayerController;
            PlayerManager.Instance.OnChangedPlayerPawn += Instance_OnChangedPlayerPawn;
        }
        private void OnDestroy()
        {
            PlayerManager.Instance.OnChangedPlayerController -= Instance_OnChangedPlayerController;
            PlayerManager.Instance.OnChangedPlayerPawn -= Instance_OnChangedPlayerPawn;
        }

        private void Instance_OnChangedPlayerPawn(PlayerManager playerManager, PawnComponent currentPawn, PawnComponent prevPawn = null)
        {
            if (_OnChangedPlayerPawnValue is null)
                _OnChangedPlayerPawnValue = new();

            _OnChangedPlayerPawnValue.PrevPawn = prevPawn;
            _OnChangedPlayerPawnValue.CurrentPawn = currentPawn;

            EventBus.Trigger(new EventHook(PlayerSystemWithVisualScripting.MANAGER_ON_CHANGED_PLAYER_PAWN, playerManager), _OnChangedPlayerPawnValue);

            _OnChangedPlayerPawnValue.PrevPawn = null;
            _OnChangedPlayerPawnValue.CurrentPawn = null;
        }

        private void Instance_OnChangedPlayerController(PlayerManager playerManager, ControllerComponent currentController, ControllerComponent prevController = null)
        {
            if (_OnChangedPlayerControllerValue is null)
                _OnChangedPlayerControllerValue = new();

            _OnChangedPlayerControllerValue.PrevController = prevController;
            _OnChangedPlayerControllerValue.CurrentController = currentController;

            EventBus.Trigger(new EventHook(PlayerSystemWithVisualScripting.MANAGER_ON_CHANGED_PLAYER_CONTROLLER, playerManager), _OnChangedPlayerControllerValue);

            _OnChangedPlayerControllerValue.PrevController = null;
            _OnChangedPlayerControllerValue.CurrentController = null;
        }
    }
}
#endif