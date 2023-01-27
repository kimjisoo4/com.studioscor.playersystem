#if SCOR_ENABLE_VISUALSCRIPTING
using Unity.VisualScripting;
using UnityEngine;

namespace StudioScor.PlayerSystem.VisualScripting
{
    [UnitTitle("On Controller Started Rotate Input")]
    [UnitShortTitle("OnStartedRotateInput")]
    [UnitSubtitle("ControllerComponene Event")]
    [UnitCategory("Events\\StudioScor\\PlayerSystem\\Controller")]
    public class ControllerOnStartedRotateInputEventUnit : ControllerDirectionEventUnit
    {
        protected override string HookName => PlayerSystemWithVisualScripting.CONTROLLER_ON_STARTED_ROTATE_INPUT;
    }
}
#endif