#if SCOR_ENABLE_BEHAVIOR
using StudioScor.Utilities;
using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;

namespace StudioScor.PlayerSystem.Behavior
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "Set MoveDirection", story: "[Self] Set MoveDirection To [Direction] in [Space] ", category: "Action/StudioScor/PlayerSystem", id: "playersystem_move_direction")]
    public class SetMoveDirectionAction : PlayerAction
    {
        public enum EDirection
        {
            World,
            Local,
            Target,
        }

        [SerializeReference] public BlackboardVariable<Vector3> Direction;
        [SerializeReference] public BlackboardVariable<float> Speed = new(1f);
        [SerializeReference] public BlackboardVariable<float> RandSpeed = new(0f);
        [SerializeReference] public BlackboardVariable<EDirection> Space = new(EDirection.Local);
        [SerializeReference] public BlackboardVariable<GameObject> Target;
        [SerializeReference] public BlackboardVariable<float> Duration = new(0f);
        [SerializeReference] public BlackboardVariable<float> RandDuration = new(0f);

        private bool _useDuration;
        private float _randSpeed;
        private float _remainTime;

        protected override Status OnStart()
        {
            if(base.OnStart() == Status.Failure)
                return Status.Failure;

            _useDuration = Duration.Value >= 0f;

            if(_useDuration)
            {
                _remainTime = Duration.Value + UnityEngine.Random.Range(0f, RandDuration.Value);
            }

            _randSpeed = RandSpeed.Value > 0f ? UnityEngine.Random.Range(0f, RandSpeed.Value) : 0f;
            
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

            if (_useDuration)
            {
                _remainTime -= Time.deltaTime;

                if (_remainTime <= 0f)
                    return Status.Success;
                else
                    return Status.Running;
            }
            else
            {
                return Status.Running;
            }
        }

        protected override void OnEnd()
        {
            base.OnEnd();

            _controllerSystem.SetMoveDirection(Vector3.zero, 0f);
        }

        public void UpdateMoveDirection()
        {
            Vector3 direction = Direction.Value;

            switch (Space.Value)
            {
                case EDirection.World:
                    break;
                case EDirection.Local:
                    direction = direction.TurnDirectionFromY(Pawn.transform);
                    break;
                case EDirection.Target:
                    Quaternion rotation = Quaternion.LookRotation(Pawn.transform.HorizontalDirection(Target.Value.transform), Vector3.up);

                    direction = Quaternion.Euler(0, rotation.eulerAngles.y, 0) * direction;
                    break;
                default:
                    break;
            }

            _controllerSystem.SetMoveDirection(direction, Mathf.Clamp01(Speed.Value + _randSpeed));

            
        }

    }
}
#endif