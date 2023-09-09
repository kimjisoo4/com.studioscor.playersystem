#if SCOR_ENABLE_VISUALSCRIPTING
using System.Collections;
using System.Collections.Generic;
using System;
using Unity.VisualScripting;
using UnityEngine;
using StudioScor.PlayerSystem;

namespace StudioScor.PlayerSystem.VisualScripting
{
    public class PawnMessageListener : MessageListener
    {
        private void Awake()
        {
            var pawnSystem = gameObject.GetPawnSystem();

            pawnSystem.OnChangedMovementInputState += Pawn_OnChangedIgnoreMovementInput;
            pawnSystem.OnChangedRotateInputState += Pawn_OnChangedIgnoreRotateInput;
            pawnSystem.OnUnPossessedController += Pawn_OnUnPossessedController;
            pawnSystem.OnPossessedController += Pawn_OnPossessedController;
        }
        private void OnDestroy()
        {
            if (gameObject.TryGetPawnSystem(out IPawnSystem pawnSystem))
            {
                pawnSystem.OnChangedMovementInputState -= Pawn_OnChangedIgnoreMovementInput;
                pawnSystem.OnChangedRotateInputState -= Pawn_OnChangedIgnoreRotateInput;
                pawnSystem.OnUnPossessedController -= Pawn_OnUnPossessedController;
                pawnSystem.OnPossessedController -= Pawn_OnPossessedController;
            }
        }
        private void Pawn_OnChangedIgnoreRotateInput(IPawnSystem pawn, bool ignore)
        {
            EventBus.Trigger(new EventHook(PlayerSystemWithVisualScripting.PAWN_ON_CHANGED_ROTATE_INPUT_STATE, pawn), ignore);
        }

        private void Pawn_OnChangedIgnoreMovementInput(IPawnSystem pawn, bool ignore)
        {
            EventBus.Trigger(new EventHook(PlayerSystemWithVisualScripting.PAWN_ON_CHANGED_MOVEMENT_INPUT_STATE, pawn), ignore);
        }

        private void Pawn_OnUnPossessedController(IPawnSystem pawn, IControllerSystem controller)
        {
            EventBus.Trigger(new EventHook(PlayerSystemWithVisualScripting.PAWN_ON_UNPOSSESSED_CONTROLLER, pawn), controller);
        }

        private void Pawn_OnPossessedController(IPawnSystem pawn, IControllerSystem controller)
        {
            EventBus.Trigger(new EventHook(PlayerSystemWithVisualScripting.PAWN_ON_POSSESSED_CONTROLLER, pawn), controller);
        }
    }
}
#endif