#if SCOR_ENABLE_BEHAVIOR
using StudioScor.PlayerSystem.Behavior;
using System;
using Unity.Behavior;

namespace StudioScor.Utilities.UnityBehavior
{
    [Serializable, Unity.Properties.GeneratePropertyBag]
    [Condition(name: "Controller Is Possessed Pawn", 
        story: "[Self] is IsPossessed Pawn ( UseDebug [UseDebugKey] )", 
        category: "Conditions/StudioScor/PlayerSystem", 
        id: "playersystem_controllerispossessedpawn")]
    public partial class ControllerIsPossessedPawnCondition : PlayerCondition
    {
        public override bool IsTrue()
        {
            if (!base.IsTrue())
                return false;

            var result = _controllerSystem.IsPossessed;

            Log($"{nameof(Self).ToBold()} Is {(result ? "Possessed".ToColor(SUtility.STRING_COLOR_SUCCESS) : "UnPossessed".ToColor(SUtility.STRING_COLOR_FAIL)).ToBold()}");

            return result;
        }
    }
}
#endif