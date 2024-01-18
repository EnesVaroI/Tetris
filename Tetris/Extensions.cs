using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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

        private static Dictionary<MouseButtonType, bool> buttonStates = new Dictionary<MouseButtonType, bool>();

        public static bool IsClick(this MouseState state, MouseButtonType button)
        {
            if (!buttonStates.ContainsKey(button))
            {
                buttonStates[button] = false;
            }

            ButtonState currentButtonState;

            switch (button)
            {
                case MouseButtonType.LeftButton:
                    currentButtonState = state.LeftButton;
                    break;

                case MouseButtonType.RightButton:
                    currentButtonState = state.RightButton;
                    break;

                case MouseButtonType.MiddleButton:
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

        public enum MouseButtonType
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

        public static Action<SpriteFont, string, Vector2, Color, float> DrawString(this SpriteBatch spriteBatch) =>
            (font, text, position, color, scale) =>
            {
                spriteBatch.DrawString(font, text, position, color, rotation: 0f, origin: Vector2.Zero, scale: scale, effects: SpriteEffects.None, layerDepth: 0f);
            };

        private static Dictionary<Keys, char> keyToCharMap = new Dictionary<Keys, char>(){
            { Keys.A, 'A' }, { Keys.B, 'B' }, { Keys.C, 'C' }, { Keys.D, 'D' }, { Keys.E, 'E' },
            { Keys.F, 'F' }, { Keys.G, 'G' }, { Keys.H, 'H' }, { Keys.I, 'I' }, { Keys.J, 'J' },
            { Keys.K, 'K' }, { Keys.L, 'L' }, { Keys.M, 'M' }, { Keys.N, 'N' }, { Keys.O, 'O' },
            { Keys.P, 'P' }, { Keys.Q, 'Q' }, { Keys.R, 'R' }, { Keys.S, 'S' }, { Keys.T, 'T' },
            { Keys.U, 'U' }, { Keys.V, 'V' }, { Keys.W, 'W' }, { Keys.X, 'X' }, { Keys.Y, 'Y' },
            { Keys.Z, 'Z' }, { Keys.D0, '0' }, { Keys.D1, '1' }, { Keys.D2, '2' }, { Keys.D3, '3' },
            { Keys.D4, '4' }, { Keys.D5, '5' }, { Keys.D6, '6' }, { Keys.D7, '7' }, { Keys.D8, '8' },
            { Keys.D9, '9' },
        };

        public static char? GetTypedChar(this KeyboardState state, Keys key) => keyToCharMap.TryGetValue(key, out char value) ? value : (char?)null;
    }
}