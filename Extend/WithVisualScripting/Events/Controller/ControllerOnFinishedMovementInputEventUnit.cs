#if SCOR_ENABLE_VISUALSCRIPTING
using Unity.VisualScripting;

namespace StudioScor.PlayerSystem.VisualScripting
{
    [UnitTitle("On Controller Finished Movement Input")]
    [UnitShortTitle("OnFinishedMovementInput")]
    [UnitSubtitle("ControllerComponene Event")]
    [UnitCategory("Events\\StudioScor\\PlayerSystem\\Controller")]
    public class ControllerOnFinishedMovementInputEventUnit : ControllerOnStartedMovementInputEventUnit
    {
        protected override string HookName => PlayerSystemWithVisualScripting.CONTROLLER_ON_FINISHED_MOVEMENT_INPUT;
    }
}
#endif