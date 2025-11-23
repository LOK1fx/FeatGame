using System;
using UnityEngine;

namespace LOK1game
{
    public class PlayerCharacterInputContext
    {
        public Vector2 MovementInput;
        public Vector2 LookInput;

        public event Action OnCrouchButtonDown;
        public void FireCrouchButtonDown() => OnCrouchButtonDown?.Invoke();


        public event Action OnCrouchButtonUp;
        public void FireCrouchButtonUp() => OnCrouchButtonUp?.Invoke();


        public event Action OnSprintButtonDown;
        public void FireSprintButtonDown() => OnSprintButtonDown?.Invoke();


        public event Action OnSprintButtonUp;
        public void FireSprintButtonUp() => OnSprintButtonUp?.Invoke();


        public event Action OnFireButtonDown;
        public void FireFireButtonDown() => OnFireButtonDown?.Invoke();


        public event Action OnFireButtonUp;
        public void FireFireButtonUp() => OnFireButtonUp?.Invoke();


        public event Action OnAltFireButtonDown;
        public void FireAltFireButtonDown() => OnAltFireButtonDown?.Invoke();


        public event Action OnAltFireButtonUp;
        public void FireAltFireButtonUp() => OnAltFireButtonUp?.Invoke();


        public event Action OnJumpButtonDown;
        public void FireJumpButtonDown() => OnJumpButtonDown?.Invoke();


        public event Action OnInteractionButtonDown;
        public void FireInteractionButtonDown() => OnInteractionButtonDown?.Invoke();
    }
}
