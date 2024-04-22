using StudioScor.Utilities;
using System;
using UnityEngine;

namespace StudioScor.PlayerSystem.Variable
{
    [Serializable]
    public class MoveDirectionVariable : DirectionVariable
    {
        [Header(" [ Input Direction Module ] ")]
        [SerializeField] private Vector3 _defaultDirection = Vector3.forward;
        private IPawnSystem _pawnSystem;
        private MoveDirectionVariable _original;

        public override IDirectionVariable Clone()
        {
            var copy = new MoveDirectionVariable();

            copy._original = this;

            return copy;
        }
        public override Vector3 GetValue()
        {
            Vector3 moveDirection = _pawnSystem.MoveDirection;

            if (moveDirection == Vector3.zero)
            {
                Vector3 direction = _original is null ? _defaultDirection : _original._defaultDirection;

                return Owner.transform.TransformDirection(direction);
            }

            return moveDirection;
        }

        public override void Setup(GameObject owner)
        {
            base.Setup(owner);

            _pawnSystem = Owner.GetPawnSystem();
        }
    }
}
