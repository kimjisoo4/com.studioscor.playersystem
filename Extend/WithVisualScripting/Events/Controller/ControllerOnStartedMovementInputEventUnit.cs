#if SCOR_ENABLE_VISUALSCRIPTING
using System;
using Unity.VisualScripting;
using UnityEngine;

namespace StudioScor.PlayerSystem.VisualScripting
{
    [UnitTitle("On Controller Started Movement Input")]
    [UnitShortTitle("OnStartedMovementInput")]
    [UnitSubtitle("ControllerComponene Event")]
    [UnitCategory("Events\\StudioScor\\PlayerSystem\\Controller")]
    public class ControllerOnStartedMovementInputEventUnit : ControllerEventUnit<MoveDirectionEvent>
    {
        protected override string HookName => PlayerSystemWithVisualScripting.CONTROLLER_ON_STARTED_MOVEMENT_INPUT;

        [DoNotSerialize]
        [PortLabel("Direction")]
        public ValueOutput Direction { get; private set; }
        [DoNotSerialize]
        [PortLabel("Strength")]
        public ValueOutput Strength { get; private set; }

        protected override void Definition()
        {
            base.Definition();

            Direction = ValueOutput<Vector3>(nameof(Direction));
            Strength = ValueOutput<float>(nameof(Strength));

            Requirement(Target, Direction);
            Requirement(Target, Strength);
        }

        protected override void AssignArguments(Flow flow, MoveDirectionEvent movementInput)
        {
            base.AssignArguments(flow, movementInput);

            flow.SetValue(Direction, movementInput.Direction);
            flow.SetValue(Strength, movementInput.Strength);
        }
    }
}
#endif