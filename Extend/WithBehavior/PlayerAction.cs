#if SCOR_ENABLE_BEHAVIOR
using Unity.Behavior;
using UnityEngine;
using StudioScor.Utilities.UnityBehavior;

namespace StudioScor.PlayerSystem.Behavior
{
    public abstract class PlayerAction : BaseAction
    {
        [SerializeReference] public BlackboardVariable<GameObject> Self;

        protected IControllerSystem _controllerSystem;
        protected IPawnSystem Pawn => _controllerSystem.Pawn;

        protected override Status OnStart()
        {
            if (base.OnStart() == Status.Failure)
                return Status.Failure;

            var self = Self.Value;

            if (!self)
            {
                LogError($"{nameof(Self)} is Null.");
                return Status.Failure;
            }

            if(!self.TryGetControllerSystem(out _controllerSystem))
            {
                LogError($"{nameof(Self)} is Not Has {nameof(IControllerSystem)}.");
                return Status.Failure;
            }

            return Status.Running;
        }
    }
}
#endif