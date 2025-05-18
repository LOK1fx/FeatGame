using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LOK1game
{
    public abstract class Controller : MonoBehaviour, IApplicationUpdatable
    {
        public event Action<IPawn> OnControlledPawnChanged;
        
        public bool IsInputProcessing = true;
        public IPawn ControlledPawn { get; private set; }

        private static List<Controller> _controllers = new List<Controller>();

        protected virtual void OnEnable()
        {
            ApplicationUpdateManager.Register(this);
        }

        protected virtual void OnDisable()
        {
            ApplicationUpdateManager.Unregister(this);
        }

        protected abstract void Awake();
        public virtual void ApplicationUpdate()
        {
            if (IsInputProcessing == false)
                return;
        }
        
        public static T Create<T>(IPawn pawn = null) where T : Controller
        {
            var controllerObject = new GameObject($"{pawn}^Controller");
            var controller = controllerObject.AddComponent<T>();
            
            if(pawn != null)
                controller.SetControlledPawn(pawn);

            _controllers.Add(controller);
            
            return controller;
        }

        public static bool TryGetController<T>(out T foundController) where T : Controller
        {
            foreach (var controller in _controllers.OfType<T>())
            {
                foundController = controller;
                return true;
            }

            foundController = null;
            return false;
        }
        
        public void SetControlledPawn(IPawn pawn)
        {
            ControlledPawn = pawn;
            ControlledPawn.OnPocces(this);
            
            OnControlledPawnChanged?.Invoke(pawn);
        }
    }
}