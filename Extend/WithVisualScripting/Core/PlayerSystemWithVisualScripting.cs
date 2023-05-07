﻿#if SCOR_ENABLE_VISUALSCRIPTING
using Unity.VisualScripting;

namespace StudioScor.PlayerSystem.VisualScripting
{
    public static class PlayerSystemWithVisualScripting
    {
        //Manager
        public const string MANAGER_ON_CHANGED_PLAYER_CONTROLLER = "PlayerManager_OnChangedPlayerController";
        public const string MANAGER_ON_CHANGED_PLAYER_PAWN = "PlayerManager_OnChangedPlayerPawn";


        public static bool WasAddEventBus = false;
        private static OnChangedPlayerPawnValue OnChangedPlayerPawnValue;
        private static OnChangedPlayerControllerValue OnChangedPlayerControllerValue;
        public static void TryAddEventBusToManager(PlayerManager playerManager)
        {
            if (WasAddEventBus)
                return;

            WasAddEventBus = true;

            playerManager.OnChangedPlayerController += PlayerManager_OnChangedPlayerController;
            playerManager.OnChangedPlayerPawn += PlayerManager_OnChangedPlayerPawn;
        }

        private static void PlayerManager_OnChangedPlayerPawn(PlayerManager playerManager, PawnComponent currentPawn, PawnComponent prevPawn = null)
        {
            if (OnChangedPlayerPawnValue is null)
                OnChangedPlayerPawnValue = new();

            OnChangedPlayerPawnValue.CurrentPawn = currentPawn;
            OnChangedPlayerPawnValue.PrevPawn = prevPawn;

            EventBus.Trigger(new EventHook(MANAGER_ON_CHANGED_PLAYER_PAWN, playerManager), OnChangedPlayerPawnValue);

            OnChangedPlayerPawnValue.CurrentPawn = null;
            OnChangedPlayerPawnValue.PrevPawn = null;
        }

        private static void PlayerManager_OnChangedPlayerController(PlayerManager playerManager, ControllerComponent currentController, ControllerComponent prevController = null)
        {
            if (OnChangedPlayerControllerValue is null)
                OnChangedPlayerControllerValue = new();

            OnChangedPlayerControllerValue.CurrentController = currentController;
            OnChangedPlayerControllerValue.PrevController = prevController;

            EventBus.Trigger(new EventHook(MANAGER_ON_CHANGED_PLAYER_CONTROLLER, playerManager), OnChangedPlayerControllerValue);

            OnChangedPlayerControllerValue.CurrentController = null;
            OnChangedPlayerControllerValue.PrevController = null;
        }


        //Pawn
        public const string PAWN_ON_CHANGED_MOVEMENT_INPUT_STATE = "OnChangedMovementInputState";
        public const string PAWN_ON_CHANGED_ROTATE_INPUT_STATE = "OnChangedRotateInputState";
        public const string PAWN_ON_POSSESSED_CONTROLLER = "OnPossessedController";
        public const string PAWN_ON_UNPOSSESSED_CONTROLLER = "UnPossessedController";

        //Controller
        public const string CONTROLLER_ON_POSSESSED_PAWN = "OnChangedPossessedPawn";
        public const string CONTROLLER_ON_UNPOSSESSED_PAWN = "OnUnPossessedPawn";
        public const string CONTROLLER_ON_CHANGED_MOVEMENT_INPUT_STATE = "OnChnagedMovementInputState";
        public const string CONTROLLER_ON_CHANGED_ROTATE_INPUT_STATE = "OnChangedRotateInputState";
        public const string CONTROLLER_ON_CHANGED_LOOK_INPUT_STATE = "OnChangedLookInputState";
        

        public const string CONTROLLER_ON_STARTED_MOVEMENT_INPUT = "OnStatedMovementInput";
        public const string CONTROLLER_ON_FINISHED_MOVEMENT_INPUT = "OnFinishedMovementInput";
        public const string CONTROLLER_ON_STARTED_ROTATE_INPUT = "OnStatedRotateInput";
        public const string CONTROLLER_ON_FINISHED_ROTATE_INPUT = "OnFinishedRotateInput";

        public const string CONTROLLER_ON_CHANGED_LOOK_POSITION = "OnChangedLookPosition";
        public const string CONTROLLER_ON_CHANGED_LOOK_TARGET = "OnChangedLookTarget";

    }
}
#endif