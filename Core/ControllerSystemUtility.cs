using UnityEngine;

namespace StudioScor.PlayerSystem
{
    public static class ControllerSystemUtility
    {
        #region Get Controller

        public static bool TryGetController(this GameObject gameObject, out IControllerSystem controllerSystem)
        {
            controllerSystem = gameObject.GetController();

            return controllerSystem is not null;
        }
        public static bool TryGetController(this Component component, out IControllerSystem controllerSystem)
        {
            controllerSystem = component.gameObject.GetController();

            return controllerSystem is not null;
        }

        public static IControllerSystem GetController(this GameObject gameObject)
        {
            var controllerSystem = gameObject.GetControllerSystem();

            if (controllerSystem is null)
            {
                if(gameObject.TryGetPawnSystem(out IPawnSystem pawnSystem))
                {
                    controllerSystem = pawnSystem.Controller;
                }
            }

            return controllerSystem;
        }
        public static IControllerSystem GetController(this Component component)
        {
            return component.gameObject.GetController();
        }


        public static bool TryGetPawn(this GameObject gameObject, out IPawnSystem pawnSystem)
        {
            pawnSystem = gameObject.GetPawn();

            return pawnSystem is not null;
        }
        public static bool TryGetPawn(this Component component, out IPawnSystem pawnSystem)
        {
            pawnSystem = component.gameObject.GetPawn();

            return pawnSystem is not null;
        }

        public static IPawnSystem GetPawn(this GameObject gameObject)
        {
            var pawnSystem = gameObject.GetPawnSystem();

            if(pawnSystem is null)
            {
                if(gameObject.TryGetControllerSystem(out IControllerSystem controllerSystem))
                {
                    pawnSystem = controllerSystem.Pawn;
                }
            }

            return pawnSystem;
        }
        public static IPawnSystem GetPawn(this Component component)
        {
            return component.gameObject.GetPawn();
        }
        #endregion

        #region Get Controller System
        public static IControllerSystem GetControllerSystem(this GameObject gameObject)
        {
            return gameObject.GetComponent<IControllerSystem>();
        }
        public static IControllerSystem GetControllerSystem(this Component component)
        {
            var controller = component as IControllerSystem;

            if (controller is not null)
                return controller;

            return component.gameObject.GetComponent<IControllerSystem>();
        }
        public static bool TryGetControllerSystem(this GameObject gameObject, out IControllerSystem controllerSystem)
        {
            return gameObject.TryGetComponent(out controllerSystem);
        }
        public static bool TryGetControllerSystem(this Component component, out IControllerSystem controllerSystem)
        {
            controllerSystem = component as IControllerSystem;

            if (controllerSystem is not null)
                return true;

            return component.TryGetComponent(out controllerSystem);
        }
        #endregion
        #region Get Pawn System
        public static IPawnSystem GetPawnSystem(this GameObject gameObject)
        {
            return gameObject.GetComponent<IPawnSystem>();
        }
        public static IPawnSystem GetPawnSystem(this Component component)
        {
            var pawnSystem = component as IPawnSystem;

            if (pawnSystem is not null)
                return pawnSystem;

            return component.GetComponent<IPawnSystem>();
        }
        public static bool TryGetPawnSystem(this GameObject gameObject, out IPawnSystem pawnSystem)
        {
            return gameObject.TryGetComponent(out pawnSystem);
        }
        public static bool TryGetPawnSystem(this Component component, out IPawnSystem pawnSystem)
        {
            pawnSystem = component as IPawnSystem;

            if (pawnSystem is not null)
                return true;

            return component.TryGetComponent(out pawnSystem);
        }
        #endregion

    }

}

