#if SCOR_ENABLE_BEHAVIOR
using StudioScor.PlayerSystem;
using StudioScor.PlayerSystem.Behavior;
using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;

namespace StudioScor.Utilities.UnityBehavior
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "UnPossess Pawn",
        story: "[Self] UnPossess to [Target] ",
        category: "Action/StudioScor/PlayerSystem",
        id: "playersystem_unpossesspawn")]
    public partial class UnPossessPawnAction : PlayerAction
    {
        [SerializeReference] public BlackboardVariable<GameObject> Target;

        protected override Status OnStart()
        {
            if (base.OnStart() == Status.Failure)
                return Status.Failure;

            var target = Target.Value;

            if (!target)
            {
                Log($"{nameof(Target).ToBold()} is {"Null".ToBold().ToColor(SUtility.STRING_COLOR_FAIL)}");

                return Status.Failure;
            }
            else if (!target.TryGetPawnSystem(out IPawnSystem pawn))
            {
                Log($"{nameof(Target.Value).ToBold()} is {$"Not Has {nameof(IPawnSystem)}".ToBold().ToColor(SUtility.STRING_COLOR_FAIL)}");

                return Status.Failure;
            }
            else
            {
                _controllerSystem.UnPossess(pawn);
                return Status.Success;
            }
        }
    }
}
#endif