#if SCOR_ENABLE_BEHAVIOR
using StudioScor.Utilities;
using System;
using Unity.Behavior;
using UnityEngine;

namespace StudioScor.PlayerSystem.Behavior
{
    [Serializable, Unity.Properties.GeneratePropertyBag]
    [NodeDescription(name: "Get Possessed Pawn", story: "Get Possessed Pawn from [Self] and store it in [Pawn]", category: "Action/StudioScor/PlayerSystem", id: "playersystem_getpossessedpawn")]
    public partial class GetPossessedPawn : PlayerAction
    {
        [SerializeReference] public BlackboardVariable<GameObject> Pawn;

        protected override Status OnStart()
        {
            if (base.OnStart() == Status.Failure)
                return Status.Failure;

            Pawn.Value = _controllerSystem.Pawn.gameObject;

            Log($"{nameof(Pawn).ToBold()} is {(Pawn.Value ? $"{Pawn.Value.name}" : "Null").ToBold()}");

            return Status.Success;
        }
    }
}
#endif