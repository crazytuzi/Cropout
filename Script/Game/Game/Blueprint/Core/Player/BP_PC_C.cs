using System;
using Script.CoreUObject;
using Script.Engine;
using Script.Game.Blueprint.Core.Player.Input;
using Script.InputCore;

namespace Script.Game.Blueprint.Core.Player
{
    [Override]
    public partial class BP_PC_C
    {
        [Override]
        public override void ReceiveBeginPlay()
        {
            /*
             * Detect Input and Call a Switch Event if input changes.
             */
            // InputComponent.BindAction("KeyDetect", EInputEvent.IE_Pressed, this, KeyDetect_Pressed);

            InputComponent.BindAxis("MouseMove", this, MouseMove);

            InputComponent.BindAction("Touch Detect", EInputEvent.IE_Pressed, this, TouchDetect_Pressed);
        }

        [Override]
        private void KeyDetect_Pressed(FKey Key)
        {
            if (UKismetInputLibrary.Key_IsGamepadKey(Key))
            {
                if (InputType != E_InputType.Gamepad)
                {
                    InputType = E_InputType.Gamepad;

                    KeySwitch.Broadcast(InputType);
                }
            }
        }

        [Override]
        private void MouseMove(Single AxisValue)
        {
            if (AxisValue != 0.0)
            {
                if (InputType != E_InputType.KeyMouse)
                {
                    InputType = E_InputType.KeyMouse;

                    KeySwitch.Broadcast(InputType);
                }
            }
        }

        [Override]
        private void TouchDetect_Pressed(FKey Key)
        {
            if (InputType != E_InputType.Touch)
            {
                InputType = E_InputType.Touch;

                KeySwitch.Broadcast(InputType);
            }
        }

        public E_InputType InputType = E_InputType.Unknown;
    }
}