#if SCOR_ENABLE_VISUALSCRIPTING
using Unity.VisualScripting;

namespace StudioScor.PlayerSystem.VisualScripting
{
    [UnitTitle("On Controller UnPossessed Pawn")]
    [UnitShortTitle("OnUnPossessedPawn")]
    [UnitSubtitle("ControllerComponene Event")]
    [UnitCategory("Events\\StudioScor\\PlayerSystem\\Controller")]
    public class ControllerOnUnPossessedPawnEventUnit : ControllerOnPossessedPawnEventUnit
    {
        protected override string HookName => PlayerSystemWithVisualScripting.CONTROLLER_ON_UNPOSSESSED_PAWN;
    }
}
#endif