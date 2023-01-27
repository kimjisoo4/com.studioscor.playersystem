#if SCOR_ENABLE_VISUALSCRIPTING
using Unity.VisualScripting;

namespace StudioScor.PlayerSystem.VisualScripting
{

    [UnitTitle("On Controller Changed Movement Input State")]
    [UnitShortTitle("OnChangedMovementInputState")]
    [UnitSubtitle("ControllerComponene Event")]
    [UnitCategory("Events\\StudioScor\\PlayerSystem\\Controller")]
    public class ControllerOnChangedMovementInputStateEventUnit : ControllerOnChangedStateEventUnit
    {
        protected override string HookName => PlayerSystemWithVisualScripting.CONTROLLER_ON_CHANGED_MOVEMENT_INPUT_STATE;
    }
}
#endif