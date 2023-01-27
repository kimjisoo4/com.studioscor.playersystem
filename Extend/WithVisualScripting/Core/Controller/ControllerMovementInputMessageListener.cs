#if SCOR_ENABLE_VISUALSCRIPTING
using UnityEngine;
using Unity.VisualScripting;

namespace StudioScor.PlayerSystem.VisualScripting
{
    public class ControllerMovementInputMessageListener : MessageListener
    {
        public class MovementInput
        {
            public Vector3 Direction;
            public float Strength;

            public MovementInput(Vector3 direction, float strength)
            {
                Direction = direction;
                Strength = strength;
            }
        }
        private void Awake()
        {
            var controller = GetComponent<ControllerComponent>();

            controller.OnStartedMovementInput += Controller_OnStartedMovementInput;
            controller.OnFinishedMovementInput += Controller_OnFinishedMovementInput;
        }
        private void OnDestroy()
        {
            if (TryGetComponent(out ControllerComponent controller))
            {
                controller.OnStartedMovementInput -= Controller_OnStartedMovementInput;
                controller.OnFinishedMovementInput -= Controller_OnFinishedMovementInput;
            }
        }
        private void Controller_OnFinishedMovementInput(ControllerComponent controller, Vector3 direction, float strength)
        {
            EventBus.Trigger(new EventHook(PlayerSystemWithVisualScripting.CONTROLLER_ON_FINISHED_MOVEMENT_INPUT, controller), new MovementInput(direction, strength));
        }

        private void Controller_OnStartedMovementInput(ControllerComponent controller, Vector3 direction, float strength)
        {
            EventBus.Trigger(new EventHook(PlayerSystemWithVisualScripting.CONTROLLER_ON_STARTED_MOVEMENT_INPUT, controller), new MovementInput(direction, strength));
        }
    }
}
#endif