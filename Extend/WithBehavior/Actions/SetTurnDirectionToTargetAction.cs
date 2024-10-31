using StudioScor.PlayerSystem.Behavior;
using StudioScor.Utilities;
using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

namespace StudioScor.Utilities.UnityBehavior
{

    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "Set TurnDirection To Target", story: "[Self] Set TurnDirection To [Target]", category: "Action/StudioScor/PlayerSystem", id: "playersystem_setturndirectiontotarget")]
    public partial class SetTurnDirectionToTargetAction : PlayerAction
    {
        [SerializeReference] public BlackboardVariable<GameObject> Target;
        [SerializeReference] public BlackboardVariable<float> ReachAngle = new(5f);

        private Vector3 _direction;


        protected override Status OnStart()
        {
            if (base.OnStart() == Status.Failure)
                return Status.Failure;

            var target = Target.Value;

            if (!target)
                return Status.Failure;

            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            if (!Target.Value)
            {
                Log($"{nameof(Target).ToBold()} is {"Null".ToBold().ToColor(SUtility.STRING_COLOR_FAIL)}");

                return Status.Failure;
            }

            if (!_controllerSystem.IsPossessed)
            {
                Log($"{nameof(Self).ToBold()} is {"UnPossessed".ToBold().ToColor(SUtility.STRING_COLOR_FAIL)}");

                return Status.Failure;
            }


            UpdateTurnDirection();

            if (CheckReachAngle())
            {
                return Status.Success;
            }

            return Status.Running;
        }

        protected override void OnEnd()
        {
            _controllerSystem.SetTurnDirection(Vector3.zero);
        }

        private void UpdateTurnDirection()
        {
            _direction = _controllerSystem.Pawn.transform.Direction(Target.Value.transform);

            _controllerSystem.SetTurnDirection(_direction);
        }

        private bool CheckReachAngle()
        {
            if (_direction.SafeEquals(Vector3.zero))
            {
                return false;
            }

            float reachAngle = ReachAngle.Value;

            if (reachAngle >= 0f)
            {
                var currentYaw = _controllerSystem.Pawn.transform.eulerAngles.y;
                var targetYaw = Quaternion.LookRotation(_direction, _controllerSystem.transform.up).eulerAngles.y;

                var deltaAngle = Mathf.DeltaAngle(currentYaw, targetYaw);

                return deltaAngle.InRange(-reachAngle, reachAngle);
            }
            else
            {
                return false;
            }
        }
    }

}
