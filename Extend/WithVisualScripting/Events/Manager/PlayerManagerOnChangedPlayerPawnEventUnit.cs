#if SCOR_ENABLE_VISUALSCRIPTING
using Unity.VisualScripting;

namespace StudioScor.PlayerSystem.VisualScripting
{
    [UnitTitle("On Changed Player Pawn")]
    [UnitShortTitle("OnChangedPlayerPawn")]
    [UnitSubtitle("PlayerManager Event")]
    [UnitCategory("Events\\StudioScor\\PlayerSystem\\PlayerManager")]
    public class PlayerManagerOnChangedPlayerPawnEventUnit : PlayerManagerEventUnit<OnChangedPlayerPawnValue>
    {
        protected override string HookName => PlayerSystemWithVisualScripting.MANAGER_ON_CHANGED_PLAYER_PAWN;

        [DoNotSerialize]
        [PortLabel("CurrentPawn")]
        public ValueOutput CurrentPawn { get; private set; }

        [DoNotSerialize]
        [PortLabel("PrevPawn")]
        public ValueOutput PrevPawn { get; private set; }

        protected override void Definition()
        {
            base.Definition();

            CurrentPawn = ValueOutput<PawnSystemComponent>(nameof(CurrentPawn));
            PrevPawn = ValueOutput<PawnSystemComponent>(nameof(PrevPawn));
        }
        protected override void AssignArguments(Flow flow, OnChangedPlayerPawnValue value)
        {
            flow.SetValue(CurrentPawn, value.CurrentPawn);
            flow.SetValue(PrevPawn, value.PrevPawn);
        }
    }
}
#endif