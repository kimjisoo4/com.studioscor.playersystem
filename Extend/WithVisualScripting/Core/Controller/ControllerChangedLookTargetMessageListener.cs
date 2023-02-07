#if SCOR_ENABLE_VISUALSCRIPTING
using UnityEngine;
using Unity.VisualScripting;

namespace StudioScor.PlayerSystem.VisualScripting
{
    public class ControllerChangedLookTargetMessageListener : MessageListener
    {
        public class ChangedLookTargetValue
        {
            public Transform CurrentLookTarget;
            public Transform PrevLookTarget;
        }

        private ChangedLookTargetValue _ChangedLookTargetValue;


        private void Awake()
        {
            var controller = GetComponent<ControllerComponent>();
            _ChangedLookTargetValue = new();

            controller.OnChangedLookTarget += Controller_OnChangedLookTarget;
        }
        private void OnDestroy()
        {
            _ChangedLookTargetValue = null;

            if (TryGetComponent(out ControllerComponent controller))
            {
                controller.OnChangedLookTarget -= Controller_OnChangedLookTarget;
            }
        }

        private void Controller_OnChangedLookTarget(ControllerComponent controller, Transform currentLookTarget, Transform prevLookTarget)
        {
            _ChangedLookTargetValue.CurrentLookTarget = currentLookTarget;
            _ChangedLookTargetValue.PrevLookTarget = prevLookTarget;

            EventBus.Trigger(new EventHook(PlayerSystemWithVisualScripting.CONTROLLER_ON_CHANGED_LOOK_TARGET, controller), _ChangedLookTargetValue);
        }
    }
}
#endif