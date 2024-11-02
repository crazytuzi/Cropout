using Script.CoreUObject;
using Script.Engine;
using Script.EnhancedInput;
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

            var EnhancedInputLocalPlayerSubsystem = USubsystemBlueprintLibrary.GetLocalPlayerSubsystem(
                this,
                UEnhancedInputLocalPlayerSubsystem.StaticClass()
            ) as UEnhancedInputLocalPlayerSubsystem;

            EnhancedInputLocalPlayerSubsystem.AddMappingContext(Unreal.LoadObject<IMC_BaseInput>(this), 0);

            EnhancedInputLocalPlayerSubsystem.AddMappingContext(Unreal.LoadObject<IMC_Villager_Mode>(this), 0);

            var EnhancedInputComponent = InputComponent as UEnhancedInputComponent;

            /*
             * Build Mode
             */
            Build_Move_Triggered =
                EnhancedInputComponent.BindAction<IA_Build_Move>(ETriggerEvent.Triggered, this, IABuildMoveTriggered);

            Build_Move_Completed =
                EnhancedInputComponent.BindAction<IA_Build_Move>(ETriggerEvent.Completed, this, IABuildMoveCompleted);

            /*
             * Villager Mode
             */
            Villager_Triggered = Villager_Triggered =
                EnhancedInputComponent.BindAction<IA_Villager>(ETriggerEvent.Triggered, this, IAVillagerTriggered);

            Villager_Started =
                EnhancedInputComponent.BindAction<IA_Villager>(ETriggerEvent.Started, this, IAVillagerStarted);

            Villager_Canceled =
                EnhancedInputComponent.BindAction<IA_Villager>(ETriggerEvent.Canceled, this, IAVillagerCanceled);

            Villager_Completed =
                EnhancedInputComponent.BindAction<IA_Villager>(ETriggerEvent.Completed, this, IAVillagerCompleted);

            /*
             * You can split off groups of logic into multiple Graphs. This helps keep blueprints more contained and easier to find.
             */
            Move_Triggered = EnhancedInputComponent.BindAction<IA_Move>(ETriggerEvent.Triggered, this, IAMoveTriggered);

            Spin_Triggered = EnhancedInputComponent.BindAction<IA_Spin>(ETriggerEvent.Triggered, this, IASpinTriggered);

            Zoom_Triggered = EnhancedInputComponent.BindAction<IA_Zoom>(ETriggerEvent.Triggered, this, IAZoomTriggered);

            /*
             * Contextual Movement
             */
            DragMove_Triggered =
                EnhancedInputComponent.BindAction<IA_DragMove>(ETriggerEvent.Triggered, this, IADragMoveTriggered);
        }

        [Override]
        public override void ReceiveEndPlay(EEndPlayReason EndPlayReason)
        {
            InputComponent.RemoveAxisBinding(this, "MouseMove", MouseMove);

            InputComponent.RemoveActionBinding(this, "Touch Detect", EInputEvent.IE_Pressed, TouchDetect_Pressed);

            var EnhancedInputComponent = InputComponent as UEnhancedInputComponent;

            EnhancedInputComponent.RemoveAction(this, Build_Move_Triggered, IABuildMoveTriggered);

            EnhancedInputComponent.RemoveAction(this, Build_Move_Completed, IABuildMoveCompleted);

            EnhancedInputComponent.RemoveAction(this, Villager_Triggered, IAVillagerTriggered);

            EnhancedInputComponent.RemoveAction(this, Villager_Started, IAVillagerStarted);

            EnhancedInputComponent.RemoveAction(this, Villager_Canceled, IAVillagerCanceled);

            EnhancedInputComponent.RemoveAction(this, Villager_Completed, IAVillagerCompleted);

            EnhancedInputComponent.RemoveAction(this, Move_Triggered, IAMoveTriggered);

            EnhancedInputComponent.RemoveAction(this, Spin_Triggered, IASpinTriggered);

            EnhancedInputComponent.RemoveAction(this, Zoom_Triggered, IAZoomTriggered);

            EnhancedInputComponent.RemoveAction(this, DragMove_Triggered, IADragMoveTriggered);
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
        private void MouseMove(float AxisValue)
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

        [Override]
        private void IABuildMoveTriggered(FInputActionValue ActionValue, float ElapsedTime, float TriggeredTime,
            UInputAction SourceAction)
        {
            (Pawn as BP_Player_C)?.IABuildMoveTriggered(ActionValue, ElapsedTime, TriggeredTime, SourceAction);
        }

        [Override]
        private void IABuildMoveCompleted(FInputActionValue ActionValue, float ElapsedTime, float TriggeredTime,
            UInputAction SourceAction)
        {
            (Pawn as BP_Player_C)?.IABuildMoveCompleted(ActionValue, ElapsedTime, TriggeredTime, SourceAction);
        }

        [Override]
        private void IAVillagerTriggered(FInputActionValue ActionValue, float ElapsedTime, float TriggeredTime,
            UInputAction SourceAction)
        {
            (Pawn as BP_Player_C)?.IAVillagerTriggered(ActionValue, ElapsedTime, TriggeredTime, SourceAction);
        }

        [Override]
        private void IAVillagerStarted(FInputActionValue ActionValue, float ElapsedTime, float TriggeredTime,
            UInputAction SourceAction)
        {
            (Pawn as BP_Player_C)?.IAVillagerStarted(ActionValue, ElapsedTime, TriggeredTime, SourceAction);
        }

        [Override]
        private void IAVillagerCanceled(FInputActionValue ActionValue, float ElapsedTime, float TriggeredTime,
            UInputAction SourceAction)
        {
            (Pawn as BP_Player_C)?.IAVillagerCanceled(ActionValue, ElapsedTime, TriggeredTime, SourceAction);
        }

        [Override]
        private void IAVillagerCompleted(FInputActionValue ActionValue, float ElapsedTime, float TriggeredTime,
            UInputAction SourceAction)
        {
            (Pawn as BP_Player_C)?.IAVillagerCompleted(ActionValue, ElapsedTime, TriggeredTime, SourceAction);
        }

        [Override]
        private void IAMoveTriggered(FInputActionValue ActionValue, float ElapsedTime, float TriggeredTime,
            UInputAction SourceAction)
        {
            (Pawn as BP_Player_C)?.IAMoveTriggered(ActionValue, ElapsedTime, TriggeredTime, SourceAction);
        }

        [Override]
        private void IASpinTriggered(FInputActionValue ActionValue, float ElapsedTime, float TriggeredTime,
            UInputAction SourceAction)
        {
            (Pawn as BP_Player_C)?.IASpinTriggered(ActionValue, ElapsedTime, TriggeredTime, SourceAction);
        }

        [Override]
        private void IAZoomTriggered(FInputActionValue ActionValue, float ElapsedTime, float TriggeredTime,
            UInputAction SourceAction)
        {
            (Pawn as BP_Player_C)?.IAZoomTriggered(ActionValue, ElapsedTime, TriggeredTime, SourceAction);
        }

        [Override]
        private void IADragMoveTriggered(FInputActionValue ActionValue, float ElapsedTime, float TriggeredTime,
            UInputAction SourceAction)
        {
            (Pawn as BP_Player_C)?.IADragMoveTriggered(ActionValue, ElapsedTime, TriggeredTime, SourceAction);
        }

        public E_InputType InputType = E_InputType.Unknown;

        private FEnhancedInputActionEventBinding Build_Move_Triggered;

        private FEnhancedInputActionEventBinding Build_Move_Completed;

        private FEnhancedInputActionEventBinding Villager_Triggered;

        private FEnhancedInputActionEventBinding Villager_Started;

        private FEnhancedInputActionEventBinding Villager_Canceled;

        private FEnhancedInputActionEventBinding Villager_Completed;

        private FEnhancedInputActionEventBinding Move_Triggered;

        private FEnhancedInputActionEventBinding Spin_Triggered;

        private FEnhancedInputActionEventBinding Zoom_Triggered;

        private FEnhancedInputActionEventBinding DragMove_Triggered;
    }
}