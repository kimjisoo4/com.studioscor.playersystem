#if SCOR_ENABLE_VISUALSCRIPTING
using Unity.VisualScripting;

namespace StudioScor.PlayerSystem.VisualScripting
{

    [UnitTitle("On Controller Stated Look Input")]
    [UnitShortTitle("OnStartedLookInput")]
    [UnitSubtitle("ControllerComponene Event")]
    [UnitCategory("Events\\StudioScor\\PlayerSystem\\Controller")]
    public class ControllerOnStartedLookInputEventUnit : ControllerDirectionEventUnit
    {
        protected override string HookName => PlayerSystemWithVisualScripting.CONTROLLER_ON_STARTED_LOOK_INPUT;
    }
}
#endif