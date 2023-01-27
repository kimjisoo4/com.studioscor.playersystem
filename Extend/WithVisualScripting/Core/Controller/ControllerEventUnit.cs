#if SCOR_ENABLE_VISUALSCRIPTING
using Unity.VisualScripting;
using System;

namespace StudioScor.PlayerSystem.VisualScripting
{
    public abstract class ControllerEventUnit<T> : EventUnit<T>
    {
        public virtual Type MessageListenerType => typeof(ControllerMessageListener);
        protected override bool register => true;
        protected abstract string HookName { get; }
        public override EventHook GetHook(GraphReference reference)
        {
            if (!reference.hasData)
            {
                return HookName;
            }

            var data = reference.GetElementData<Data>(this);

            return new EventHook(HookName, data.ControllerComponent);
        }

        [DoNotSerialize]
        [PortLabel("PawnComponent")]
        [NullMeansSelf]
        [PortLabelHidden]
        public ValueInput ControllerComponent { get; private set; }

        public override IGraphElementData CreateData()
        {
            return new Data();
        }
        public new class Data : EventUnit<T>.Data
        {
            public ControllerComponent ControllerComponent;
        }

        protected override void Definition()
        {
            base.Definition();

            ControllerComponent = ValueInput<ControllerComponent>(nameof(ControllerComponent), null).NullMeansSelf();
        }

        private void UpdateTarget(GraphStack stack)
        {
            var data = stack.GetElementData<Data>(this);

            var wasListening = data.isListening;

            var controllerComponent = Flow.FetchValue<ControllerComponent>(ControllerComponent, stack.ToReference());

            if (controllerComponent != data.ControllerComponent)
            {
                if (wasListening)
                {
                    StopListening(stack);
                }

                data.ControllerComponent = controllerComponent;

                if (wasListening)
                {
                    StartListening(stack, false);
                }
            }
        }

        protected void StartListening(GraphStack stack, bool updateTarget)
        {
            if (updateTarget)
            {
                UpdateTarget(stack);
            }

            var data = stack.GetElementData<Data>(this);

            if (data.ControllerComponent is null)
            {
                return;
            }

            if (UnityThread.allowsAPI)
            {
                if(MessageListenerType != null)
                    MessageListener.AddTo(MessageListenerType, data.ControllerComponent.gameObject);
            }

            base.StartListening(stack);
        }

        public override void StartListening(GraphStack stack)
        {
            StartListening(stack, true);
        }
    }
}
#endif