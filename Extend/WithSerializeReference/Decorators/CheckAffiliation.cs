#if SCOR_ENABLE_SERIALIZEREFERENCE
using StudioScor.Utilities;
using System;
using UnityEngine;

namespace StudioScor.PlayerSystem.Extend.TaskSystem
{
    [Serializable]
    public class CheckAffiliation : TaskActionDecorator
    {
        [SerializeReference, SerializeReferenceDropdown]
        private IGameObjectVariable _target;
        [SerializeField] private ERelationship _affiliation = ERelationship.Hostile;

        private IRelationshipSystem _relationshipSystem;

        protected new CheckAffiliation _original;

        public override void Setup(GameObject owner)
        {
            base.Setup(owner);

            _target.Setup(owner);

            _relationshipSystem = _target.GetValue().GetRelationshipSystem();
        }

        public override ITaskActionDecorator Clone()
        {
            var clone = new CheckAffiliation();

            clone._original = this;
            clone._target = _target.Clone();

            return clone;
        }

        protected override bool PerformConditionCheck(GameObject target)
        {
            if (_relationshipSystem is null)
                return false;

            if (!target.TryGetReleationshipSystem(out IRelationshipSystem targetRelationship))
                return false;

            var aiiliation = _original is null ? _affiliation : _original._affiliation;

            return _relationshipSystem.CheckRelationship(targetRelationship) == aiiliation;
        }
    }
}
#endif
