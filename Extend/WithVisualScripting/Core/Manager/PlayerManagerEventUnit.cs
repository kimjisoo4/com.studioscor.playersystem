#if SCOR_ENABLE_VISUALSCRIPTING
using Unity.VisualScripting;
using System;

namespace StudioScor.PlayerSystem.VisualScripting
{
    public abstract class PlayerManagerEventUnit<T> : EventUnit<T>
    {
        protected override bool register => true;
        protected virtual Type MessageListenerType => typeof(PlayerManagerMessageListner);
        protected abstract string HookName { get; }
        public override EventHook GetHook(GraphReference reference)
        {
            return new EventHook(HookName, PlayerManager.Instance);
        }

        public override void StartListening(GraphStack stack)
        {
            if (UnityThread.allowsAPI)
            {
                if (MessageListenerType != null)
                    MessageListener.AddTo(MessageListenerType, PlayerManager.Instance.gameObject);
            }

            base.StartListening(stack);
        }
    }
}
#endif