#if SCOR_ENABLE_BEHAVIOR
using StudioScor.Utilities;
using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using UnityEngine.AI;

namespace StudioScor.PlayerSystem.Behavior
{

    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "Set MoveDirection To Target", story: "[Self] Set MoveDirection To [Target]", category: "Action/StudioScor/PlayerSystem", id: "playersystem_move_target")]
    public partial class SetMoveDirectionToTargetAction : PlayerAction
    {
        [SerializeReference] public BlackboardVariable<GameObject> Target;

        [SerializeReference] public BlackboardVariable<float> Speed = new(1f);
        [SerializeReference] public BlackboardVariable<float> RandSpeed = new(0f);

        [SerializeReference] public BlackboardVariable<float> ReachDistance = new(5f);
        [SerializeReference] public BlackboardVariable<float> RandDistance = new(1f);

        [SerializeReference] public BlackboardVariable<bool> UseVerticalDistance = new(false);

        [SerializeReference] public BlackboardVariable<bool> UseNav = new(true);
        [SerializeReference] public BlackboardVariable<float> NavUpdateInterval = new(0.5f);


        private Vector3 _direction;
        private float _distance;
        private float _randSpeed;
        private float _randDistance;

        private bool _useNav;
        private int _pathIndex;
        private float _remainUpdateNavInterval;
        private NavMeshAgent _navMeshAgent;
        private NavMeshPath _path;

        protected override Status OnStart()
        {
            if (base.OnStart() == Status.Failure)
                return Status.Failure;
            
            var target = Target.Value;

            if (!target)
            {
                return Status.Failure;
            }
            _randSpeed = RandSpeed.Value > 0f ? UnityEngine.Random.Range(0f, RandSpeed.Value) : 0f;
            _randDistance = RandDistance.Value > 0f ? UnityEngine.Random.Range(0f, RandDistance.Value) : 0f;
            _useNav = UseNav.Value;
            if(_useNav)
            {
                if (!Pawn.gameObject.TryGetComponent(out _navMeshAgent))
                {
                    Log($"{_navMeshAgent.GetType().Name.ToBold()} is {"Null".ToBold().ToColor(SUtility.STRING_COLOR_FAIL)}");
                    return Status.Failure;
                }

                _remainUpdateNavInterval = 0f;
                _pathIndex = 0;

                if (_path is null)
                    _path = new();
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

            if (!_controllerSystem.IsPossessed)
            {
                Log($"{nameof(Self).ToBold()} is {"UnPossessed".ToBold().ToColor(SUtility.STRING_COLOR_FAIL)}");

                return Status.Failure;
            }

            if (_useNav)
                UpdateMoveDirectionUseNav();
            else
                UpdateMoveDirection();


            if (CheckReachDistance())
            {
                Log($"{nameof(Self).ToBold()} is Reached {nameof(Target).ToBold()}");

                return Status.Success;
            }
            else
            {
                _controllerSystem.SetMoveDirection(_direction, Mathf.Min(_distance, Mathf.Clamp01(Speed.Value + _randSpeed)));

                return Status.Running;
            }
        }

        protected override void OnEnd()
        {
            base.OnEnd();

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
        }


        private void UpdateMoveDirectionUseNav()
        {
            var target = Target.Value;

            if (_remainUpdateNavInterval <= 0f)
            {
                _remainUpdateNavInterval = NavUpdateInterval.Value;
                _pathIndex = 1;
                NavMesh.CalculatePath(Pawn.transform.position, target.transform.position, _navMeshAgent.areaMask, _path);
            }
            else
            {
                _remainUpdateNavInterval -= Time.deltaTime;
            }

            Vector3 direction;

            if (_path.status != NavMeshPathStatus.PathComplete || _path.corners.Length <= 2)
            {
                direction = Pawn.transform.HorizontalDirection(target.transform);
            }
            else
            {
                Vector3 targetPosition = _path.corners[_pathIndex];

                if (Pawn.transform.HorizontalDistance(targetPosition) <= 0.1f)
                {
                    _pathIndex++;

                    targetPosition = _path.corners[_pathIndex];
                }

                direction = Pawn.transform.HorizontalDirection(targetPosition);
            }

            _direction = direction;
            _distance = Pawn.transform.HorizontalDistance(target.transform.position);
        }

        private bool CheckReachDistance()
        {
            float reachDistance = ReachDistance.Value;

            if(reachDistance > 0f)
            {
                return _distance <= reachDistance + _randDistance;
            }
            else
            {
                return false;
            }
        }
    }
}
#endif