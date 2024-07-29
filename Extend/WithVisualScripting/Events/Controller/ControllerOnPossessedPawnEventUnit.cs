#if SCOR_ENABLE_VISUALSCRIPTING
using Unity.VisualScripting;

namespace StudioScor.PlayerSystem.VisualScripting
{
    [UnitTitle("On Controller Possessed Pawn")]
    [UnitShortTitle("OnPossessedPawn")]
    [UnitSubtitle("ControllerComponene Event")]
    [UnitCategory("Events\\StudioScor\\PlayerSystem\\Controller")]
    public class ControllerOnPossessedPawnEventUnit : ControllerEventUnit<PawnSystemComponent>
    {
        protected override string HookName => PlayerSystemWithVisualScripting.CONTROLLER_ON_POSSESSED_PAWN;

        [DoNotSerialize]
        [PortLabel("Pawn")]
        public ValueOutput PawnComponent { get; private set; }

        protected override void Definition()
        {
            base.Definition();

            PawnComponent = ValueOutput<PawnSystemComponent>(nameof(PawnComponent));

            Requirement(Target, PawnComponent);
        }

        protected override void AssignArguments(Flow flow, PawnSystemComponent pawn)
        {
            base.AssignArguments(flow, pawn);

            flow.SetValue(PawnComponent, pawn);
        }
    }
}
#endif