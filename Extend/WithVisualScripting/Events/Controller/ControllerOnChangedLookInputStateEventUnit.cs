#if SCOR_ENABLE_VISUALSCRIPTING
using Unity.VisualScripting;

namespace StudioScor.PlayerSystem.VisualScripting
{
    [UnitTitle("On Controller Changed Look Input State")]
    [UnitShortTitle("OnChangedLookInputState")]
    [UnitSubtitle("ControllerComponene Event")]
    [UnitCategory("Events\\StudioScor\\PlayerSystem\\Controller")]
    public class ControllerOnChangedLookInputStateEventUnit : ControllerOnChangedStateEventUnit
    {
        protected override string HookName => PlayerSystemWithVisualScripting.CONTROLLER_ON_CHANGED_LOOK_INPUT_STATE;
    }
}
#endif