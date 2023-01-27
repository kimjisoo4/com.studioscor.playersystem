#if SCOR_ENABLE_VISUALSCRIPTING
using Unity.VisualScripting;
using System;
namespace StudioScor.PlayerSystem.VisualScripting
{
    public abstract class PawnEventUnit<T> : EventUnit<T>
    {
        protected virtual Type MessageListenerType => typeof(PawnMessageListener);
        protected override bool register => true;
        protected abstract string HookName { get; }
        public override EventHook GetHook(GraphReference reference)
        {
            if (!reference.hasData)
            {
                return HookName;
            }

            var data = reference.GetElementData<Data>(this);

            return new EventHook(HookName, data.PawnComponent);
        }

        [DoNotSerialize]
        [PortLabel("PawnComponent")]
        [NullMeansSelf]
        [PortLabelHidden]
        public ValueInput PawnComponent { get; private set; }

        public override IGraphElementData CreateData()
        {
            return new Data();
        }
        public new class Data : EventUnit<T>.Data
        {
            public PawnComponent PawnComponent;
        }

        protected override void Definition()
        {
            base.Definition();

            PawnComponent = ValueInput<PawnComponent>(nameof(PawnComponent), null).NullMeansSelf();
        }

        private void UpdateTarget(GraphStack stack)
        {
            var data = stack.GetElementData<Data>(this);

            var wasListening = data.isListening;

            var pawnComponent = Flow.FetchValue<PawnComponent>(PawnComponent, stack.ToReference());

            if (pawnComponent != data.PawnComponent)
            {
                if (wasListening)
                {
                    StopListening(stack);
                }

                data.PawnComponent = pawnComponent;

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

            if (data.PawnComponent is null)
            {
                return;
            }

            if (UnityThread.allowsAPI)
            {
                MessageListener.AddTo(MessageListenerType, data.PawnComponent.gameObject);
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