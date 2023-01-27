#if SCOR_ENABLE_VISUALSCRIPTING

namespace StudioScor.PlayerSystem.VisualScripting
{
    public static class PlayerSystemWithVisualScripting
    {
        public const string PAWN_ON_CHANGED_MOVEMENT_INPUT_STATE = "OnChangedMovementInputState";
        public const string PAWN_ON_CHANGED_ROTATE_INPUT_STATE = "OnChangedRotateInputState";
        public const string PAWN_ON_POSSESSED_CONTROLLER = "OnPossessedController";
        public const string PAWN_ON_UNPOSSESSED_CONTROLLER = "UnPossessedController";

        public const string CONTROLLER_ON_POSSESSED_PAWN = "OnChangedPossessedPawn";
        public const string CONTROLLER_ON_UNPOSSESSED_PAWN = "OnUnPossessedPawn";
        public const string CONTROLLER_ON_CHANGED_MOVEMENT_INPUT_STATE = "OnChnagedMovementInputState";
        public const string CONTROLLER_ON_CHANGED_ROTATE_INPUT_STATE = "OnChangedRotateInputState";
        public const string CONTROLLER_ON_CHANGED_LOOK_INPUT_STATE = "OnChangedLookInputState";
        
        public const string CONTROLLER_ON_CHANGED_LOOK_TARGET = "OnChangedLookTarget";

        public const string CONTROLLER_ON_STARTED_MOVEMENT_INPUT = "OnStatedMovementInput";
        public const string CONTROLLER_ON_FINISHED_MOVEMENT_INPUT = "OnFinishedMovementInput";
        public const string CONTROLLER_ON_STARTED_ROTATE_INPUT = "OnStatedRotateInput";
        public const string CONTROLLER_ON_FINISHED_ROTATE_INPUT = "OnFinishedRotateInput";
        public const string CONTROLLER_ON_STARTED_LOOK_INPUT = "OnStatedLookInput";
        public const string CONTROLLER_ON_FINISHED_LOOK_INPUT = "OnFinishedLookInput";

    }
}
#endif