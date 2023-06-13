#if SCOR_ENABLE_VISUALSCRIPTING

namespace StudioScor.PlayerSystem.VisualScripting
{
    public class OnChangedPlayerControllerValue
    {
        public IControllerSystem CurrentController;
        public IControllerSystem PrevController;
    }
}
#endif