#if SCOR_ENABLE_BEHAVIOR
using StudioScor.Utilities;
using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;

namespace StudioScor.PlayerSystem.Behavior
{

    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "Set MoveDirection To Position", story: "[Self] Set MoveDirection To [Position] ", category: "Action/StudioScor/PlayerSystem", id: "playersystem_move_position")]
    public class SetMoveDirectionToPositionAction : PlayerAction
    {
        [SerializeReference] public BlackboardVariable<Vector3> Position;
        [SerializeReference] public BlackboardVariable<float> Speed = new(1f);
        [SerializeReference] public BlackboardVariable<float> RandSpeed = new(0f);
        [SerializeReference] public BlackboardVariable<float> ReachDistance = new(5f);
        [SerializeReference] public BlackboardVariable<bool> UseVerticalDistance = new(false);  

        private Vector3 _direction;
        private float _distance;
        private float _randSpeed;

        protected override Status OnStart()
        {
            if (base.OnStart() == Status.Failure)
                return Status.Failure;

            _randSpeed = RandSpeed.Value > 0 ? UnityEngine.Random.Range(0f, _randSpeed) : 0f;

            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            if (!_controllerSystem.IsPossessed)
            {
                Log($"{nameof(Self).ToBold()} is {"UnPossessed".ToBold().ToColor(SUtility.STRING_COLOR_FAIL)}");

                return Status.Failure;
            }

            UpdateMoveDirection();

            if (CheckReachDistance())
            {
                Log($"{nameof(Self).ToBold()} is Reached {nameof(Position).ToBold()}");
                return Status.Success;
            }

            return Status.Running;
        }

        protected override void OnEnd()
        {
            base.OnEnd();

            _controllerSystem.SetMoveDirection(Vector3.zero, 0f);
        }

        private void UpdateMoveDirection()
        {
            if (UseVerticalDistance.Value)
            {
                _direction = _controllerSystem.Pawn.transform.Direction(Position.Value, false);
            }
            else
            {
                _direction = _controllerSystem.Pawn.transform.HorizontalDirection(Position.Value, false);
            }

            _distance = _direction.magnitude;
            _direction.Normalize();

            _controllerSystem.SetMoveDirection(_direction, Mathf.Min(_distance, Mathf.Clamp01(Speed.Value + _randSpeed)));
        }

        private bool CheckReachDistance()
        {
            float reachDistance = ReachDistance.Value;

            if (reachDistance > 0f)
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