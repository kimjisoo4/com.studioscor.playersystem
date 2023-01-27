#if SCOR_ENABLE_VISUALSCRIPTING
using UnityEngine;
using Unity.VisualScripting;

namespace StudioScor.PlayerSystem.VisualScripting
{

    public class ControllerMessageListener : MessageListener
    {
        private void Awake()
        {
            var controller = GetComponent<ControllerComponent>();

            controller.OnPossessedPawn += Controller_OnPossessedPawn;
            controller.OnUnPossessedPawn += Controller_OnUnPossessedPawn;

            controller.OnChangedMovementInputState += Controller_OnChangedMovementInputState;
            controller.OnChangedRotateInputState += Controller_OnChangedRotateInputState;
            controller.OnChangedLookInputState += Controller_OnChangedLookInputState;

            controller.OnStartedLookInput += Controller_OnStartedLookInput;
            controller.OnFinishedLookInput += Controller_OnFinishedLookInput;
            
            controller.OnStartedRotatetInput += Controller_OnStartedRotatetInput;
            controller.OnFinishedRotateInput += Controller_OnFinishedRotateInput;
        }

        private void OnDestroy()
        {
            if (TryGetComponent(out ControllerComponent controller))
            {
                controller.OnPossessedPawn -= Controller_OnPossessedPawn;
                controller.OnUnPossessedPawn -= Controller_OnUnPossessedPawn;

                controller.OnChangedMovementInputState -= Controller_OnChangedMovementInputState;
                controller.OnChangedRotateInputState -= Controller_OnChangedRotateInputState;
                controller.OnChangedLookInputState -= Controller_OnChangedLookInputState;

                controller.OnStartedLookInput -= Controller_OnStartedLookInput;
                controller.OnFinishedLookInput -= Controller_OnFinishedLookInput;

                controller.OnStartedRotatetInput -= Controller_OnStartedRotatetInput;
                controller.OnFinishedRotateInput -= Controller_OnFinishedRotateInput;
            }
        }
        private void Controller_OnFinishedRotateInput(ControllerComponent controller, Vector3 direction)
        {
            EventBus.Trigger(new EventHook(PlayerSystemWithVisualScripting.CONTROLLER_ON_FINISHED_ROTATE_INPUT, controller), direction);
        }
        private void Controller_OnStartedRotatetInput(ControllerComponent controller, Vector3 direction)
        {
            EventBus.Trigger(new EventHook(PlayerSystemWithVisualScripting.CONTROLLER_ON_STARTED_ROTATE_INPUT, controller), direction);
        }
        private void Controller_OnFinishedLookInput(ControllerComponent controller, Vector3 direction)
        {
            EventBus.Trigger(new EventHook(PlayerSystemWithVisualScripting.CONTROLLER_ON_FINISHED_LOOK_INPUT, controller), direction);
        }
        private void Controller_OnStartedLookInput(ControllerComponent controller, Vector3 direction)
        {
            EventBus.Trigger(new EventHook(PlayerSystemWithVisualScripting.CONTROLLER_ON_STARTED_LOOK_INPUT, controller), direction);
        }

        private void Controller_OnChangedLookInputState(ControllerComponent controller, bool isUsed)
        {
            EventBus.Trigger(new EventHook(PlayerSystemWithVisualScripting.CONTROLLER_ON_CHANGED_LOOK_INPUT_STATE, controller), isUsed);
        }

        private void Controller_OnChangedRotateInputState(ControllerComponent controller, bool isUsed)
        {
            EventBus.Trigger(new EventHook(PlayerSystemWithVisualScripting.CONTROLLER_ON_CHANGED_ROTATE_INPUT_STATE, controller), isUsed);
        }

        private void Controller_OnChangedMovementInputState(ControllerComponent controller, bool isUsed)
        {
            EventBus.Trigger(new EventHook(PlayerSystemWithVisualScripting.CONTROLLER_ON_CHANGED_MOVEMENT_INPUT_STATE, controller), isUsed);
        }
        private void Controller_OnUnPossessedPawn(ControllerComponent controller, PawnComponent pawn)
        {
            EventBus.Trigger(new EventHook(PlayerSystemWithVisualScripting.CONTROLLER_ON_UNPOSSESSED_PAWN, controller), pawn);
        }

        private void Controller_OnPossessedPawn(ControllerComponent controller, PawnComponent pawn)
        {
            EventBus.Trigger(new EventHook(PlayerSystemWithVisualScripting.CONTROLLER_ON_POSSESSED_PAWN, controller), pawn);
        }
    }
}
#endif