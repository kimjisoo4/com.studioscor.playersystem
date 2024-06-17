using StudioScor.Utilities;
using System;
using UnityEngine;

namespace StudioScor.PlayerSystem.Extend.TaskSystem
{
    [Serializable]
    public class CheckAffiliation : TaskActionDecorator
    {
#if SCOR_ENABLE_SERIALIZEREFERENCE
        [SerializeReference, SerializeReferenceDropdown]
#endif
        private IGameObjectVariable _target;
        [SerializeField] private EAffiliation _affiliation = EAffiliation.Hostile;

        private IControllerSystem _controllerSystem;

        protected new CheckAffiliation _original;

        public override void Setup(GameObject owner)
        {
            base.Setup(owner);

            _target.Setup(owner);

            _controllerSystem = _target.GetValue().GetController();
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
            if (_controllerSystem is null)
                return false;

            if (!target.TryGetController(out IControllerSystem targetController))
                return false;

            var aiiliation = _original is null ? _affiliation : _original._affiliation;

            return _controllerSystem.CheckAffiliation(targetController) == aiiliation;
        }
    }
}
