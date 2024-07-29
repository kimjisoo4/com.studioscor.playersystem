#if SCOR_ENABLE_VISUALSCRIPTING
using Unity.VisualScripting;

namespace StudioScor.PlayerSystem.VisualScripting
{
    [UnitTitle("On Pawn UnPossessed Controller")]
    [UnitShortTitle("OnUnPossessedController")]
    [UnitSubtitle("PawnComponent Event")]
    [UnitCategory("Events\\StudioScor\\PlayerSystem\\Pawn")]
    public class PawnOnUnPossessedControllerEventUnit : PawnEventUnit<ControllerSystemComponent>
    {
        protected override string HookName => PlayerSystemWithVisualScripting.PAWN_ON_UNPOSSESSED_CONTROLLER;

        [DoNotSerialize]
        [PortLabel("Prev Controller")]
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