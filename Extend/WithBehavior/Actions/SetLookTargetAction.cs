using StudioScor.PlayerSystem.Behavior;
using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;

namespace StudioScor.Utilities.UnityBehavior
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "Set Look Target",
        story: "[Self] Set Look Target to [Target] ",
        category: "Action/StudioScor/PlayerSystem",
        id: "playersystem_setlooktarget")]
    public partial class SetLookTargetAction : PlayerAction
    {
        [SerializeReference] public BlackboardVariable<GameObject> Target;

        protected override Status OnStart()
        {
            if (base.OnStart() == Status.Failure)
                return Status.Failure;

            var target = Target.Value;

            if(target)
            {
                Log($"{nameof(Self).ToBold()} Set Look {target.name.ToBold()}");
                _controllerSystem.SetLookTarget(target.transform);
            }
            else
            {
                Log($"{nameof(Self).ToBold()} Clear Look Target");
                _controllerSystem.SetLookTarget(null);
            }

            return Status.Success;
        }
    }

}
