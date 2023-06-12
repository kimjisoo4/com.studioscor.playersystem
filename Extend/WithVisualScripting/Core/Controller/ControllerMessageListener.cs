﻿#if SCOR_ENABLE_VISUALSCRIPTING
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
        private void Awake()
        {
            var controller = GetComponent<IControllerEvent>();

            controller.OnPossessedPawn += Controller_OnPossessedPawn;
            controller.OnUnPossessedPawn += Controller_OnUnPossessedPawn;
        }



        private void OnDestroy()
        {
            if (TryGetComponent(out IControllerEvent controller))
            {
                controller.OnPossessedPawn -= Controller_OnPossessedPawn;
                controller.OnUnPossessedPawn -= Controller_OnUnPossessedPawn;
            }
        }

        private void Controller_OnUnPossessedPawn(IControllerEvent controller, IPawnSystem pawn)
        {
            EventBus.Trigger(new EventHook(PlayerSystemWithVisualScripting.CONTROLLER_ON_UNPOSSESSED_PAWN, controller), pawn);
        }

        private void Controller_OnPossessedPawn(IControllerEvent controller, IPawnSystem pawn)
        {
            EventBus.Trigger(new EventHook(PlayerSystemWithVisualScripting.CONTROLLER_ON_POSSESSED_PAWN, controller), pawn);
        }
    }
}
#endif