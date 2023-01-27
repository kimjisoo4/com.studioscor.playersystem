#if SCOR_ENABLE_VISUALSCRIPTING
using Unity.VisualScripting;

namespace StudioScor.PlayerSystem.VisualScripting
{
    [UnitTitle("On Pawn Changed Movement Input State")]
    [UnitShortTitle("OnChangedMovementInputState")]
    [UnitSubtitle("PawnComponent Event")]
    [UnitCategory("Events\\StudioScor\\PlayerSystem\\Pawn")]
    public class PawnOnChangedMovementInputStateEventUnit : PawnEventUnit<bool>
    {
        protected override string HookName => PlayerSystemWithVisualScripting.PAWN_ON_CHANGED_MOVEMENT_INPUT_STATE;

        [DoNotSerialize]
        [PortLabel("IsIgnore")]
        public ValueOutput IsIgnoreInput;

        protected override void Definition()
        {
            base.Definition();

            IsIgnoreInput = ValueOutput<bool>(nameof(IsIgnoreInput));
        }

        protected override void AssignArguments(Flow flow, bool isIgnoreInput)
        {
            base.AssignArguments(flow, isIgnoreInput);

            flow.SetValue(IsIgnoreInput, isIgnoreInput);
        }
    }
}
#endif