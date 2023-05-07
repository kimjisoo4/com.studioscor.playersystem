#if SCOR_ENABLE_VISUALSCRIPTING
using Unity.VisualScripting;
using UnityEngine;

namespace StudioScor.PlayerSystem.VisualScripting
{
    public abstract class ControllerDirectionEventUnit : ControllerEventUnit<Vector3>
    {
        [DoNotSerialize]
        [PortLabel("Direction")]
        public ValueOutput Direction { get; private set; }

        protected override void Definition()
        {
            base.Definition();

            Direction = ValueOutput<Vector3>(nameof(Direction));

            Requirement(Target, Direction);
        }

        protected override void AssignArguments(Flow flow, Vector3 movementInput)
        {
            base.AssignArguments(flow, movementInput);

            flow.SetValue(Direction, movementInput);
        }
    }
}
#endif