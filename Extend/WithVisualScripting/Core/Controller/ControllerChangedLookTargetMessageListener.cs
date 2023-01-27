#if SCOR_ENABLE_VISUALSCRIPTING
using UnityEngine;
using Unity.VisualScripting;

namespace StudioScor.PlayerSystem.VisualScripting
{
    public class ControllerChangedLookTargetMessageListener : MessageListener
    {
        public class ChangedLookTarget
        {
            public Transform CurrentLookTarget;
            public Transform PrevLookTarget;

            public ChangedLookTarget(Transform currentLookTarget, Transform prevLookTarget)
            {
                CurrentLookTarget = currentLookTarget;
                PrevLookTarget = prevLookTarget;
            }
        }

        private void Awake()
        {
            var controller = GetComponent<ControllerComponent>();

            controller.OnChangedLookTarget += Controller_OnChangedLookTarget;
        }
        private void OnDestroy()
        {
            if (TryGetComponent(out ControllerComponent controller))
            {
                controller.OnChangedLookTarget -= Controller_OnChangedLookTarget;
            }
        }
        private void Controller_OnChangedLookTarget(ControllerComponent controller, Transform currentLookTarget, Transform prevLookTarget)
        {
            EventBus.Trigger(new EventHook(PlayerSystemWithVisualScripting.CONTROLLER_ON_CHANGED_LOOK_TARGET, controller), new ChangedLookTarget(currentLookTarget, prevLookTarget));
        }
    }
}
#endif