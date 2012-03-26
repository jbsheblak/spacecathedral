using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace OuterSpaceCathedral
{
    static internal class ControllerInput
    {
        public enum ControllerType
        {
            Keyboard,
            GamePad,
            Null
        };

        public enum ButtonAction
        {   
            FrontEnd_Select,

            Game_Fire,
            Game_Spawn,

            Debug_Back,
            Debug_SwapControl,
        };

        public enum AnalogAction
        {
            FrontEnd_Move,

            Game_Move,
            Game_StreamControl,
        }

        public interface IController
        {
            ControllerType GetControllerType();
            ButtonState GetButtonState(ButtonAction action);
            Vector2     GetAnalogDirection(AnalogAction action);
        }
        
        public static bool Initialize()
        {
            bool hasKeyboard = false;

            for ( int i = 0; i < 4; ++i )
            {
                PlayerIndex idx = (PlayerIndex)i;
                GamePadCapabilities caps = GamePad.GetCapabilities(idx);
                if ( caps.GamePadType == GamePadType.GamePad )
                {
                    sControllers.Add(new GamePadController(idx));
                }
                else
                {
                    if ( !hasKeyboard )
                    {
                        sControllers.Add(new KeyboardController(idx));
                        hasKeyboard = true;
                    }
                    else
                    {
                        sControllers.Add(new NullController(idx));
                    }
                }
            }

            return true;
        }

        public static IController GetController(PlayerIndex idx)
        {
            return sControllers[ (int)idx ];
        }

        /// <summary>
        /// XBox GamePad controller implementation.
        /// </summary>
        private class GamePadController : IController
        {
            private PlayerIndex mPlayerIndex;

            public GamePadController(PlayerIndex playerIndex)
            {
                mPlayerIndex = playerIndex;
            }

            public ControllerType GetControllerType()
            {
                return ControllerType.GamePad;
            }

            public ButtonState GetButtonState(ButtonAction action)
            {
                GamePadState state = GamePad.GetState(mPlayerIndex);

                switch ( action )
                {  
                    case ButtonAction.FrontEnd_Select:   
                        return state.Buttons.A;

                    case ButtonAction.Game_Fire:
                        return state.Buttons.A;

                    case ButtonAction.Game_Spawn:
                        return state.Buttons.Start;

                    case ButtonAction.Debug_Back:
                        return state.Buttons.Back;

                    case ButtonAction.Debug_SwapControl:
                        return state.Buttons.RightShoulder;

                    default:
                        throw new Exception("Unhandled button");
                }
            }

            public Vector2 GetAnalogDirection(AnalogAction action)
            {
                GamePadState state = GamePad.GetState(mPlayerIndex);

                switch ( action )
                {
                    case AnalogAction.FrontEnd_Move:
                        return GetMovementDirection(state);

                    case AnalogAction.Game_Move:
                        return GetMovementDirection(state);

                    case AnalogAction.Game_StreamControl:
                        return new Vector2(state.Triggers.Right, 0);

                    default:
                        throw new Exception("Unhandled button");
                }
            }
                        
            /// Gather a movement direction from the DPAD and Analog inputs.
            private Vector2 GetMovementDirection(GamePadState state)
            {
                float x = state.ThumbSticks.Left.X;
                float y = state.ThumbSticks.Left.Y;

                if ( state.DPad.Up == ButtonState.Pressed )
                {
                    y = +1;
                }

                if ( state.DPad.Down == ButtonState.Pressed )
                {
                    y = -1;
                }

                if ( state.DPad.Left == ButtonState.Pressed )
                {
                    x = -1;
                }

                if ( state.DPad.Right == ButtonState.Pressed )
                {
                    x = +1;
                }

                Vector2 dir = new Vector2(x,y);
                if ( dir.LengthSquared() != 0.0f )
                {
                    dir.Normalize();
                }

                return dir;
            }
        }
        
        /// <summary>
        /// Keyboard Controller Implementation.
        /// </summary>
        private class KeyboardController : IController
        {
            private PlayerIndex mPlayerIndex;

            public KeyboardController(PlayerIndex playerIndex)
            {
                mPlayerIndex = playerIndex;
            }

            public ControllerType GetControllerType()
            {
                return ControllerType.Keyboard;
            }

            public ButtonState GetButtonState(ButtonAction action)
            {
                KeyboardState state = Keyboard.GetState();

                switch ( action )
                {  
                    case ButtonAction.FrontEnd_Select:
                        return GetButtonStateforKey(state, Keys.Enter);

                    case ButtonAction.Game_Fire:
                        return GetButtonStateforKey(state, Keys.Space);

                    case ButtonAction.Game_Spawn:
                        return GetButtonStateforKey(state, Keys.Enter);

                    case ButtonAction.Debug_Back:
                        return GetButtonStateforKey(state, Keys.OemTilde);

                    case ButtonAction.Debug_SwapControl:
                        return GetButtonStateforKey(state, Keys.P);

                    default:
                        throw new Exception("Unhandled button");
                }
            }

            public Vector2 GetAnalogDirection(AnalogAction action)
            {
                KeyboardState state = Keyboard.GetState();

                switch ( action )
                {
                    case AnalogAction.FrontEnd_Move:
                        return GetMovementDirection(state);

                    case AnalogAction.Game_Move:
                        return GetMovementDirection(state);

                    case AnalogAction.Game_StreamControl:
                        return state.IsKeyDown(Keys.F) ? new Vector2(1,0) : new Vector2(0,0);

                    default:
                        throw new Exception("Unhandled button");
                }
            }
            
            ButtonState GetButtonStateforKey(KeyboardState state, Keys k)
            {
                return state.IsKeyDown(k) ? ButtonState.Pressed : ButtonState.Released;
            }

            Vector2 GetMovementDirection(KeyboardState state)
            {
                float x = 0;
                float y = 0;

                if ( state.IsKeyDown(Keys.Left) )
                {
                    x += -1;
                }

                if ( state.IsKeyDown(Keys.Right) )
                {
                    x += +1;
                }

                if ( state.IsKeyDown(Keys.Down) )
                {
                    y += -1;
                }

                if ( state.IsKeyDown(Keys.Up) )
                {
                    y += +1;
                }

                Vector2 dir = new Vector2(x,y);
                if ( dir.LengthSquared() != 0.0f )
                {
                    dir.Normalize();
                }

                return dir;
            }
        }

        /// <summary>
        /// Null Controller Implementation.
        /// </summary>
        private class NullController : IController
        {
            public NullController(PlayerIndex playerIndex)
            {   
            }

            public ControllerType GetControllerType()
            {
                return ControllerType.Null;
            }

            public ButtonState GetButtonState(ButtonAction action)
            {
                return ButtonState.Released;
            }

            public Vector2 GetAnalogDirection(AnalogAction action)
            {
                return new Vector2(0,0);
            }
        }

        private static List<IController> sControllers = new List<IController>();
    }
}
