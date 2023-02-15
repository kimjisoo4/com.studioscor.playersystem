#if SCOR_ENABLE_VISUALSCRIPTING
using Unity.VisualScripting;
using System;
using StudioScor.Utilities.VisualScripting;

namespace StudioScor.PlayerSystem.VisualScripting
{
    public abstract class PlayerManagerEventUnit<T> : CustomScriptableEventUnit<PlayerManager, T>
    {
        protected override void TryAddEventBus(Data data)
        {
            if(!PlayerSystemWithVisualScripting.WasAddEventBus)
                PlayerSystemWithVisualScripting.TryAddEventBusToManager(data.Target);
        }
    }
}
#endif