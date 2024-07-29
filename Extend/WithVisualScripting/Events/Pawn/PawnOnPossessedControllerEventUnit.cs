#if SCOR_ENABLE_VISUALSCRIPTING
using Unity.VisualScripting;

namespace StudioScor.PlayerSystem.VisualScripting
{
    [UnitTitle("On Pawn Possessed Controller")]
    [UnitShortTitle("OnPossessedController")]
    [UnitSubtitle("PawnComponent Event")]
    [UnitCategory("Events\\StudioScor\\PlayerSystem\\Pawn")]
    public class PawnOnPossessedControllerEventUnit : PawnEventUnit<ControllerSystemComponent>
    {
        protected override string HookName => PlayerSystemWithVisualScripting.PAWN_ON_POSSESSED_CONTROLLER;

        [DoNotSerialize]
        [PortLabel("Controller")]
        public ValueOutput Controller;

        protected override void Definition()
        {
            base.Definition();

            Controller = ValueOutput<ControllerSystemComponent>(nameof(Controller));
        }

        protected override void AssignArguments(Flow flow, ControllerSystemComponent controllerComponent)
        {
            base.AssignArguments(flow, controllerComponent);

            flow.SetValue(Controller, controllerComponent);
        }
    }
}
#endif