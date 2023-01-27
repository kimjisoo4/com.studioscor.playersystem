#if SCOR_ENABLE_VISUALSCRIPTING
using Unity.VisualScripting;

namespace StudioScor.PlayerSystem.VisualScripting
{
    public abstract class ControllerOnChangedStateEventUnit : ControllerEventUnit<bool>
    {
        [DoNotSerialize]
        [PortLabel("IsIgnore")]
        public ValueOutput IsIgnore { get; private set; }

        protected override void Definition()
        {
            base.Definition();

            IsIgnore = ValueOutput<bool>(nameof(IsIgnore));

            Requirement(ControllerComponent, IsIgnore);
        }

        protected override void AssignArguments(Flow flow, bool isIgnoreInput)
        {
            base.AssignArguments(flow, isIgnoreInput);

            flow.SetValue(IsIgnore, isIgnoreInput);
        }
    }
}
#endif