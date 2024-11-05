#if SCOR_ENABLE_BEHAVIOR
using Unity.Behavior;
using UnityEngine;
using StudioScor.Utilities;
using StudioScor.Utilities.UnityBehavior;

namespace StudioScor.PlayerSystem.Behavior
{
    public abstract class PlayerCondition : BaseCondition
    {
        [SerializeReference] public BlackboardVariable<GameObject> Self;

        protected IControllerSystem _controllerSystem;

        public override void OnStart()
        {
            base.OnStart();

            var self = Self.Value;

            if (!self)
            {
                _controllerSystem = null;
                LogError($"{nameof(Self).ToBold()} is Null.");
                return;
            }

            if (!self.TryGetControllerSystem(out _controllerSystem))
            {
                LogError($"{nameof(Self).ToBold()} is Not Has {nameof(IControllerSystem).ToBold()}.");
                return;
            }
        }

        public override bool IsTrue()
        {
            var result = _controllerSystem is not null;

            Log($"{nameof(Self).ToBold()} {(result ? $"Has {nameof(IControllerSystem)}".ToColor(SUtility.STRING_COLOR_SUCCESS) : $"Not Has {nameof(IControllerSystem).ToColor(SUtility.STRING_COLOR_FAIL)}").ToBold()}");

            return result;
        }
    }
}
#endif