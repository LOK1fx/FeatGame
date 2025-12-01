using FishNet.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LOK1game
{
    public abstract class Controller<Pawntype> : NetworkBehaviour, IApplicationUpdatable where Pawntype : IPawn
    {
        public event Action<Pawntype> OnControlledPawnChanged;
        
        public bool IsInputProcessing = true;
        public Pawntype ControlledPawn { get; private set; }
        public PlayerCharacterInputContext InputContext { get; private set; }

        private static List<Controller<Pawntype>> _controllers = new();

        protected virtual void OnEnable()
        {
            ApplicationUpdateManager.Register(this);
        }

        protected virtual void OnDisable()
        {
            ApplicationUpdateManager.Unregister(this);
        }

        protected virtual void Awake()
        {

        }
        public abstract void ApplicationUpdate();
        
        public static T Create<T>(Pawntype pawn, bool locallyControlled = false) where T : Controller<Pawntype>
        {
            var controllerObject = new GameObject($"{pawn}^Controller");
            var controller = controllerObject.AddComponent<T>();
            
            if(pawn != null)
                controller.SetControlledPawn(pawn, locallyControlled);

            _controllers.Add(controller);
            
            return controller;
        }

        public static bool TryGetController<T>(out T foundController) where T : Controller<Pawntype>
        {
            foreach (var controller in _controllers.OfType<T>())
            {
                foundController = controller;
                return true;
            }

            foundController = null;
            return false;
        }
        
        public void SetControlledPawn(Pawntype pawn, bool locallyControlled)
        {
            InputContext = new PlayerCharacterInputContext();

            ControlledPawn = pawn;

            if (ControlledPawn != null && ControlledPawn as Pawn)
                (ControlledPawn as Pawn).SetLocal(IsOwner);

            ControlledPawn?.OnPocces(this, InputContext);
            
            OnControlledPawnChanged?.Invoke(pawn);
            OnPawnChanged(pawn);
        }

        protected virtual void OnPawnChanged(Pawntype newPawn) { }
    }
}