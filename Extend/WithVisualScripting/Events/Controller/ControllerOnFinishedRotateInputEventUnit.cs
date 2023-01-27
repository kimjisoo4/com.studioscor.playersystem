#if SCOR_ENABLE_VISUALSCRIPTING
using Unity.VisualScripting;
using UnityEngine;

namespace StudioScor.PlayerSystem.VisualScripting
{
    [UnitTitle("On Controller Finished Rotate Input")]
    [UnitShortTitle("OnFinishedRotateInput")]
    [UnitSubtitle("ControllerComponene Event")]
    [UnitCategory("Events\\StudioScor\\PlayerSystem\\Controller")]
    public class ControllerOnFinishedRotateInputEventUnit : ControllerDirectionEventUnit
    {
        protected override string HookName => PlayerSystemWithVisualScripting.CONTROLLER_ON_FINISHED_ROTATE_INPUT;
    }
}
#endif