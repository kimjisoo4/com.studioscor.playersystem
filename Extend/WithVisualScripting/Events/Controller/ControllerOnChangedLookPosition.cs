#if SCOR_ENABLE_VISUALSCRIPTING
using Unity.VisualScripting;

namespace StudioScor.PlayerSystem.VisualScripting
{

    [UnitTitle("On Controller Changed Look Position")]
    [UnitShortTitle("OnChangedLookPosition")]
    [UnitSubtitle("ControllerComponent Event")]
    [UnitCategory("Events\\StudioScor\\PlayerSystem\\Controller")]
    public class ControllerOnChangedLookPosition : ControllerDirectionEventUnit
    {
        protected override string HookName => PlayerSystemWithVisualScripting.CONTROLLER_ON_CHANGED_LOOK_POSITION;
    }
}
#endif