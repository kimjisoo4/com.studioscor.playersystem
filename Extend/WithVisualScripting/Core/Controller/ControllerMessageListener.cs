#if SCOR_ENABLE_VISUALSCRIPTING
using UnityEngine;
using Unity.VisualScripting;

namespace StudioScor.PlayerSystem.VisualScripting
{
    public class MoveDirectionEvent
    {
        public Vector3 Direction;
        public float Strength;
    }
    public class ChangeLookTargetEvent
    {
        public Transform Current;
        public Transform Prev;
    }
    public class LookPositionEvent
    {
        public Vector3 Current;
        public Vector3 Prev;
    }

    public class ControllerMessageListener : MessageListener
    {
        private readonly MoveDirectionEvent _MoveDirection = new();
        private readonly ChangeLookTargetEvent _LookTarget = new();
        private readonly LookPositionEvent _LookPosition = new();

        private void Awake()
        {
            var controller = GetComponent<IControllerEvent>();

            controller.OnPossessedPawn += Controller_OnPossessedPawn;
            controller.OnUnPossessedPawn += Controller_OnUnPossessedPawn;

            controller.OnChangedMovementInputState += Controller_OnChangedMovementInputState;
            controller.OnStartedMovementInput += Controller_OnStartedMovementInput;
            controller.OnFinishedMovementInput += Controller_OnFinishedMovementInput;

            controller.OnChangedRotateInputState += Controller_OnChangedRotateInputState;
            controller.OnStartedRotatetInput += Controller_OnStartedRotatetInput;
            controller.OnFinishedRotateInput += Controller_OnFinishedRotateInput;

            controller.OnChangedLookInputState += Controller_OnChangedLookInputState;
            controller.OnChangedLookPosition += Controller_OnChangedLookPosition;
            controller.OnChangedLookTarget += Controller_OnChangedLookTarget;
        }



        private void OnDestroy()
        {
            if (TryGetComponent(out IControllerEvent controller))
            {
                controller.OnPossessedPawn -= Controller_OnPossessedPawn;
                controller.OnUnPossessedPawn -= Controller_OnUnPossessedPawn;

                controller.OnChangedMovementInputState -= Controller_OnChangedMovementInputState;
                controller.OnStartedMovementInput -= Controller_OnStartedMovementInput;
                controller.OnFinishedMovementInput -= Controller_OnFinishedMovementInput;

                controller.OnChangedRotateInputState -= Controller_OnChangedRotateInputState;
                controller.OnStartedRotatetInput -= Controller_OnStartedRotatetInput;
                controller.OnFinishedRotateInput -= Controller_OnFinishedRotateInput;

                controller.OnChangedLookInputState -= Controller_OnChangedLookInputState;
                controller.OnChangedLookPosition -= Controller_OnChangedLookPosition;
                controller.OnChangedLookTarget -= Controller_OnChangedLookTarget;
            }
        }

        private void Controller_OnChangedLookPosition(IControllerEvent controller, Vector3 position, Vector3 prevPosition)
        {
            _LookPosition.Current = position;
            _LookPosition.Prev = prevPosition;

            EventBus.Trigger(new EventHook(PlayerSystemWithVisualScripting.CONTROLLER_ON_CHANGED_LOOK_POSITION, controller), _LookPosition);

            _LookPosition.Current = default;
            _LookPosition.Prev = default;
        }
        private void Controller_OnChangedLookTarget(IControllerEvent controller, Transform currentLookTarget, Transform prevLookTarget)
        {
            _LookTarget.Current = currentLookTarget;
            _LookTarget.Prev = prevLookTarget;

            EventBus.Trigger(new EventHook(PlayerSystemWithVisualScripting.CONTROLLER_ON_CHANGED_LOOK_TARGET, controller), _LookTarget);

            _LookTarget.Current = null;
            _LookTarget.Prev = null;
        }

        private void Controller_OnFinishedRotateInput(IControllerEvent controller, Vector3 direction)
        {
            EventBus.Trigger(new EventHook(PlayerSystemWithVisualScripting.CONTROLLER_ON_FINISHED_ROTATE_INPUT, controller), direction);
        }
        private void Controller_OnStartedRotatetInput(IControllerEvent controller, Vector3 direction)
        {
            EventBus.Trigger(new EventHook(PlayerSystemWithVisualScripting.CONTROLLER_ON_STARTED_ROTATE_INPUT, controller), direction);
        }

        private void Controller_OnChangedLookInputState(IControllerEvent controller, bool isUsed)
        {
            EventBus.Trigger(new EventHook(PlayerSystemWithVisualScripting.CONTROLLER_ON_CHANGED_LOOK_INPUT_STATE, controller), isUsed);
        }

        private void Controller_OnChangedRotateInputState(IControllerEvent controller, bool isUsed)
        {
            EventBus.Trigger(new EventHook(PlayerSystemWithVisualScripting.CONTROLLER_ON_CHANGED_ROTATE_INPUT_STATE, controller), isUsed);
        }

        private void Controller_OnChangedMovementInputState(IControllerEvent controller, bool isUsed)
        {
            EventBus.Trigger(new EventHook(PlayerSystemWithVisualScripting.CONTROLLER_ON_CHANGED_MOVEMENT_INPUT_STATE, controller), isUsed);
        }

        private void Controller_OnStartedMovementInput(IControllerEvent controller, Vector3 direction, float strength)
        {
            _MoveDirection.Direction = direction;
            _MoveDirection.Strength = strength;

            EventBus.Trigger(new EventHook(PlayerSystemWithVisualScripting.CONTROLLER_ON_STARTED_MOVEMENT_INPUT, controller), _MoveDirection);

            _MoveDirection.Direction = default;
            _MoveDirection.Strength = default;
        }
        private void Controller_OnFinishedMovementInput(IControllerEvent controller, Vector3 direction, float strength)
        {
            _MoveDirection.Direction = direction;
            _MoveDirection.Strength = strength;

            EventBus.Trigger(new EventHook(PlayerSystemWithVisualScripting.CONTROLLER_ON_FINISHED_MOVEMENT_INPUT, controller), _MoveDirection);

            _MoveDirection.Direction = default;
            _MoveDirection.Strength = default;
        }


        private void Controller_OnUnPossessedPawn(IControllerEvent controller, PawnComponent pawn)
        {
            EventBus.Trigger(new EventHook(PlayerSystemWithVisualScripting.CONTROLLER_ON_UNPOSSESSED_PAWN, controller), pawn);
        }

        private void Controller_OnPossessedPawn(IControllerEvent controller, PawnComponent pawn)
        {
            EventBus.Trigger(new EventHook(PlayerSystemWithVisualScripting.CONTROLLER_ON_POSSESSED_PAWN, controller), pawn);
        }
    }
}
#endif