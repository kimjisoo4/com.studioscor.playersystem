#if SCOR_ENABLE_BEHAVIOR
using StudioScor.Utilities;
using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;

namespace StudioScor.PlayerSystem.Behavior
{

    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "Set MoveDirection To Target", story: "[Self] Set MoveDirection To [Target]", category: "Action/StudioScor/PlayerSystem", id: "playersystem_setmovedirectiontotarget")]
    public partial class SetMoveDirectionToTargetAction : PlayerAction
    {
        [SerializeReference] public BlackboardVariable<GameObject> Target;

        [SerializeReference] public BlackboardVariable<float> Speed = new(1f);
        [SerializeReference] public BlackboardVariable<float> ReachDistance = new(5f);

        [SerializeReference] public BlackboardVariable<bool> UseVerticalDistance = new(false);

        private Vector3 _direction;
        private float _distance;

        protected override Status OnStart()
        {
            if (base.OnStart() == Status.Failure)
                return Status.Failure;
            
            var target = Target.Value;

            if (!target)
            {
                return Status.Failure;
            }

            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            if (!Target.Value)
            {
                Log($"{nameof(Target).ToBold()} is {"Null".ToBold().ToColor(SUtility.STRING_COLOR_FAIL)}");

                return Status.Failure;
            }

            if(!_controllerSystem.IsPossessed)
            {
                Log($"{nameof(Self).ToBold()} is {"UnPossessed".ToBold().ToColor(SUtility.STRING_COLOR_FAIL)}");

                return Status.Failure;
            }

            UpdateMoveDirection();

            if (CheckReachDistance())
            {
                Log($"{nameof(Self).ToBold()} is Reached {nameof(Target).ToBold()}");
                return Status.Success;
            }

            return Status.Running;
        }

        protected override void OnEnd()
        {
            _controllerSystem.SetMoveDirection(Vector3.zero, 0f);
        }

        private void UpdateMoveDirection()
        {
            if(UseVerticalDistance.Value)
            {
                _direction = _controllerSystem.Pawn.transform.Direction(Target.Value.transform, false);
            }
            else
            {
                _direction = _controllerSystem.Pawn.transform.HorizontalDirection(Target.Value.transform, false);
            }
            
            _distance = _direction.magnitude;
            _direction.Normalize();

            _controllerSystem.SetMoveDirection(_direction, _distance * Speed.Value);
        }

        private bool CheckReachDistance()
        {
            float reachDistance = ReachDistance.Value;

            if(reachDistance > 0f)
            {
                return _distance <= reachDistance;
            }
            else
            {
                return false;
            }
        }
    }
}
#endif