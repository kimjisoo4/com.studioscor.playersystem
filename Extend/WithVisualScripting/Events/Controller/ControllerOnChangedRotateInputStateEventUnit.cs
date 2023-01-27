#if SCOR_ENABLE_VISUALSCRIPTING
using Unity.VisualScripting;

namespace StudioScor.PlayerSystem.VisualScripting
{
    [UnitTitle("On Controller Changed Rotate Input State")]
    [UnitShortTitle("OnChangedRotateInputState")]
    [UnitSubtitle("ControllerComponene Event")]
    [UnitCategory("Events\\StudioScor\\PlayerSystem\\Controller")]
    public class ControllerOnChangedRotateInputStateEventUnit : ControllerOnChangedStateEventUnit
    {
        protected override string HookName => PlayerSystemWithVisualScripting.CONTROLLER_ON_CHANGED_ROTATE_INPUT_STATE;
    }
}
#endif