#if SCOR_ENABLE_VISUALSCRIPTING
using UnityEngine;
using Unity.VisualScripting;

namespace StudioScor.PlayerSystem.VisualScripting
{
    public class ControllerMovementInputMessageListener : MessageListener
    {
        public class MovementInputValue
        {
            public Vector3 Direction;
            public float Strength;
        }

        private MovementInputValue _MovementInputValue;

        private void Awake()
        {
            var controller = GetComponent<ControllerComponent>();
            _MovementInputValue = new();

            controller.OnStartedMovementInput += Controller_OnStartedMovementInput;
            controller.OnFinishedMovementInput += Controller_OnFinishedMovementInput;
        }
        private void OnDestroy()
        {
            _MovementInputValue = null;

            if (TryGetComponent(out ControllerComponent controller))
            {
                controller.OnStartedMovementInput -= Controller_OnStartedMovementInput;
                controller.OnFinishedMovementInput -= Controller_OnFinishedMovementInput;
            }
        }
        private void Controller_OnFinishedMovementInput(ControllerComponent controller, Vector3 direction, float strength)
        {
            _MovementInputValue.Direction = direction;
            _MovementInputValue.Strength = strength;

            EventBus.Trigger(new EventHook(PlayerSystemWithVisualScripting.CONTROLLER_ON_FINISHED_MOVEMENT_INPUT, controller), _MovementInputValue);
        }

        private void Controller_OnStartedMovementInput(ControllerComponent controller, Vector3 direction, float strength)
        {
            _MovementInputValue.Direction = direction;
            _MovementInputValue.Strength = strength;

            EventBus.Trigger(new EventHook(PlayerSystemWithVisualScripting.CONTROLLER_ON_STARTED_MOVEMENT_INPUT, controller), _MovementInputValue);
        }
    }
}
#endif