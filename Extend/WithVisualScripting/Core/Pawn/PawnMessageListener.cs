#if SCOR_ENABLE_VISUALSCRIPTING
using System.Collections;
using System.Collections.Generic;
using System;
using Unity.VisualScripting;
using UnityEngine;

namespace StudioScor.PlayerSystem.VisualScripting
{
    public class PawnMessageListener : MessageListener
    {
        private void Awake()
        {
            var pawn = GetComponent<PawnComponent>();

            pawn.OnChangedMovementInputState += Pawn_OnChangedIgnoreMovementInput;
            pawn.OnChangedRotateInputState += Pawn_OnChangedIgnoreRotateInput;
            pawn.OnUnPossessedController += Pawn_OnUnPossessedController;
            pawn.OnPossessedController += Pawn_OnPossessedController;
        }
        private void OnDestroy()
        {
            if (TryGetComponent(out PawnComponent pawn))
            {
                pawn.OnChangedMovementInputState -= Pawn_OnChangedIgnoreMovementInput;
                pawn.OnChangedRotateInputState -= Pawn_OnChangedIgnoreRotateInput;
                pawn.OnUnPossessedController -= Pawn_OnUnPossessedController;
                pawn.OnPossessedController -= Pawn_OnPossessedController;
            }
        }
        private void Pawn_OnChangedIgnoreRotateInput(PawnComponent pawn, bool ignore)
        {
            EventBus.Trigger(new EventHook(PlayerSystemWithVisualScripting.PAWN_ON_CHANGED_ROTATE_INPUT_STATE, pawn), ignore);
        }

        private void Pawn_OnChangedIgnoreMovementInput(PawnComponent pawn, bool ignore)
        {
            EventBus.Trigger(new EventHook(PlayerSystemWithVisualScripting.PAWN_ON_CHANGED_MOVEMENT_INPUT_STATE, pawn), ignore);
        }

        private void Pawn_OnUnPossessedController(PawnComponent pawn, ControllerComponent controller)
        {
            EventBus.Trigger(new EventHook(PlayerSystemWithVisualScripting.PAWN_ON_UNPOSSESSED_CONTROLLER, pawn), controller);
        }

        private void Pawn_OnPossessedController(PawnComponent pawn, ControllerComponent controller)
        {
            EventBus.Trigger(new EventHook(PlayerSystemWithVisualScripting.PAWN_ON_POSSESSED_CONTROLLER, pawn), controller);
        }
    }
}
#endif