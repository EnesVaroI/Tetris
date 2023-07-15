using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Tetris
{
    public static class Extensions
    {
        private static KeyboardState _lastKeyboardState;

        public static bool IsKeyReleased(this KeyboardState state, Keys key)
        {
            var isKeyUp = state.IsKeyUp(key);
            var wasKeyDown = _lastKeyboardState.IsKeyDown(key);
            _lastKeyboardState = state;

            return isKeyUp && wasKeyDown;
        }

        private static Dictionary<Keys, KeyboardState> lastStateMap = new Dictionary<Keys, KeyboardState>();

        public static bool IsClick(this KeyboardState state, Keys key)
        {
            if (!lastStateMap.TryGetValue(key, out var lastState))
                lastState = Keyboard.GetState();

            var isKeyPressed = state.IsKeyDown(key);
            var wasKeyPressed = lastState.IsKeyDown(key);
            lastStateMap[key] = state;

            return isKeyPressed && !wasKeyPressed;
        }

        private static Dictionary<MouseButton, bool> buttonStates = new Dictionary<MouseButton, bool>();

        public static bool IsClick(this MouseState state, MouseButton button)
        {
            if (!buttonStates.ContainsKey(button))
            {
                buttonStates[button] = false;
            }

            ButtonState currentButtonState;

            switch (button)
            {
                case MouseButton.LeftButton:
                    currentButtonState = state.LeftButton;
                    break;

                case MouseButton.RightButton:
                    currentButtonState = state.RightButton;
                    break;

                case MouseButton.MiddleButton:
                    currentButtonState = state.MiddleButton;
                    break;

                default:
                    throw new ArgumentException($"Invalid mouse button: {button}");
            }

            if (buttonStates[button] && currentButtonState == ButtonState.Released)
            {
                buttonStates[button] = false;
            }

            else if (!buttonStates[button] && currentButtonState == ButtonState.Pressed)
            {
                buttonStates[button] = true;
                return true;
            }

            return false;
        }

        public enum MouseButton
        {
            LeftButton,
            RightButton,
            MiddleButton
        }

        public static bool IsInRange(this MouseState state, (int, int) x, (int, int) y) => state.X >= x.Item1 && state.X <= x.Item2 && state.Y >= y.Item1 && state.Y <= y.Item2;

        private static MouseState? previousMouseState;

        public static bool IsSteady(this MouseState state)
        {
            var isCursorSteady = previousMouseState == null || state.Position == previousMouseState.Value.Position;
            previousMouseState = state;

            return isCursorSteady;
        }
    }
}