#if SCOR_ENABLE_VISUALSCRIPTING
using Unity.VisualScripting;
using System;
using StudioScor.Utilities.VisualScripting;

namespace StudioScor.PlayerSystem.VisualScripting
{
    public abstract class ControllerEventUnit<T> : CustomInterfaceEventUnit<IControllerSystem, T>
    {
        public override Type MessageListenerType => typeof(ControllerMessageListener);
    }
}
#endif