#if SCOR_ENABLE_VISUALSCRIPTING
using System;
using Unity.VisualScripting;
using UnityEngine;

namespace StudioScor.PlayerSystem.VisualScripting
{
    [UnitTitle("On Controller Changed Look Target")]
    [UnitShortTitle("OnControllerChangedLookTarget")]
    [UnitSubtitle("ControllerComponene Event")]
    [UnitCategory("Events\\StudioScor\\PlayerSystem\\Controller")]
    public class ControllerOnChangedLookTargetEventUnit : ControllerEventUnit<ChangeLookTargetEvent>
    {
        protected override string HookName => PlayerSystemWithVisualScripting.CONTROLLER_ON_CHANGED_LOOK_TARGET;

        [DoNotSerialize]
        [PortLabel("CurrentTarget")]
        public ValueOutput CurrentTarget { get; private set; }
        [DoNotSerialize]
        [PortLabel("PrevTarget")]
        public ValueOutput PrevTarget { get; private set; }

        protected override void Definition()
        {
            base.Definition();

            CurrentTarget = ValueOutput<Transform>(nameof(CurrentTarget));
            PrevTarget = ValueOutput<Transform>(nameof(PrevTarget));

            Requirement(Target, CurrentTarget);
            Requirement(Target, PrevTarget);
        }

        protected override void AssignArguments(Flow flow, ChangeLookTargetEvent changedLookTarget)
        {
            base.AssignArguments(flow, changedLookTarget);

            flow.SetValue(CurrentTarget, changedLookTarget.Current);
            flow.SetValue(PrevTarget, changedLookTarget.Prev);
        }
    }
}
#endif