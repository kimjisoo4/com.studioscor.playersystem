using StudioScor.Utilities;
using System;
using UnityEngine;

namespace StudioScor.PlayerSystem.Variable
{
    [Serializable]
    public class LookDirectionVariable : DirectionVariable
    {
        private IPawnSystem _pawnSystem;

        public override void Setup(GameObject owner)
        {
            base.Setup(owner);

            _pawnSystem = Owner.GetPawnSystem();
        }
        public override IDirectionVariable Clone()
        {
            var clone = new LookDirectionVariable();

            return clone;
        }

        public override Vector3 GetValue()
        {
            return _pawnSystem.LookDirection;
        }
    }
}
