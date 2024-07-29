#if SCOR_ENABLE_VISUALSCRIPTING
using Unity.VisualScripting;

namespace StudioScor.PlayerSystem.VisualScripting
{
    [UnitTitle("On Changed Player Controller")]
    [UnitShortTitle("OnChangedPlayerController")]
    [UnitSubtitle("PlayerManager Event")]
    [UnitCategory("Events\\StudioScor\\PlayerSystem\\PlayerManager")]
    public class PlayerManagerOnChangedControllerEventUnit : PlayerManagerEventUnit<OnChangedPlayerControllerValue>
    {
        protected override string HookName => PlayerSystemWithVisualScripting.MANAGER_ON_CHANGED_PLAYER_CONTROLLER;

        [DoNotSerialize]
        [PortLabel("CurrentController")]
        public ValueOutput CurrentController { get; private set; }

        [DoNotSerialize]
        [PortLabel("PrevController")]
        public ValueOutput PrevController { get; private set; }

        protected override void Definition()
        {
            base.Definition();

            CurrentController = ValueOutput<ControllerSystemComponent>(nameof(CurrentController));
            PrevController = ValueOutput<ControllerSystemComponent>(nameof(PrevController));
        }
        protected override void AssignArguments(Flow flow, OnChangedPlayerControllerValue value)
        {
            flow.SetValue(CurrentController, value.CurrentController);
            flow.SetValue(PrevController, value.PrevController);
        }
    }
}
#endif