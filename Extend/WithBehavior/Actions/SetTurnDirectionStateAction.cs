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
    [NodeDescription(name: "Set TurnDirection State",
        story: "[Self] Set TurnDirection State [TurnDirectionState] ",
        category: "Action/StudioScor/PlayerSystem",
        id: "playersystem_setturndirectionstate")]
    public partial class SetTurnDirectionStateAction : PlayerAction
    {
        [SerializeReference] public BlackboardVariable<ETurnDirectionState> TurnDirectionState = new(ETurnDirectionState.Direction);

        protected override Status OnStart()
        {
            if (base.OnStart() == Status.Failure)
                return Status.Failure;

            _controllerSystem.SetTurnDirectionState(TurnDirectionState);

            Log($"{nameof(Self).ToBold()} Turn Direction to the {TurnDirectionState.Value.ToString().ToBold()}");

            return Status.Success;
        }
    }

}
#endif