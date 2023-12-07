using System;
using Script.Common;
using Script.CommonUI;
using Script.CoreUObject;
using Script.Engine;
using Script.Game.Blueprint.Core.Player;
using Script.Game.Blueprint.Core.Player.Input;
using Script.SlateCore;
using Script.UMG;

namespace Script.Game.UI.UI_Elements
{
    [IsOverride]
    public partial class UI_BuildConfirm_C
    {
        [IsOverride]
        public override void Construct()
        {
            /*
             * Button inputs
             */
            BTN_Pos.OnButtonBaseClicked.Add(this, OnPlaceBtnClicked);

            BTN_Pos_1.OnButtonBaseClicked.Add(this, OnRotateBtnClicked);

            BTN_Neg.OnButtonBaseClicked.Add(this, OnCancelBtnClicked);
        }

        [IsOverride]
        public override void Destruct()
        {
            BTN_Pos.OnButtonBaseClicked.RemoveAll(this);

            BTN_Pos_1.OnButtonBaseClicked.RemoveAll(this);

            BTN_Neg.OnButtonBaseClicked.RemoveAll(this);
        }

        /*
         * Position the UI below the Spawned actor
         */
        [IsOverride]
        public override void Tick(FGeometry MyGeometry, float InDeltaTime)
        {
            var BP_PC = UGameplayStatics.GetPlayerController(this, 0) as BP_PC_C;

            var BP_Player = BP_PC.K2_GetPawn() as BP_Player_C;

            var Position = GetPosition();

            if (BP_Player.Spawn != null && BP_Player.Spawn.IsValid())
            {
                var SpringState = Spring_h20_State;

                var InterpVector = UKismetMathLibrary.VectorSpringInterp(
                    new FVector(CommonBorder_1.RenderTransform.Translation.X,
                        CommonBorder_1.RenderTransform.Translation.Y, 0.0),
                    new FVector(Position.X, Position.Y, 0.0),
                    ref SpringState,
                    50.0f,
                    0.9f,
                    (float)UGameplayStatics.GetWorldDeltaSeconds(this),
                    1.0f,
                    0.75f,
                    false,
                    new FVector(-1.0, -1.0, -1.0),
                    new FVector(1.0, 1.0, 1.0),
                    false
                );

                Spring_h20_State = SpringState;

                CommonBorder_1.SetRenderTransform(new FWidgetTransform
                {
                    Translation = new FVector2D(InterpVector.X, InterpVector.Y),
                    Scale = new FVector2D(1.0, 1.0),
                    Shear = new FVector2D(),
                    Angle = 0.0f
                });
            }
        }

        /*
         * Set input mode and Intial Position
         */
        [IsOverride]
        public override void BP_OnActivated()
        {
            var BP_PC = UGameplayStatics.GetPlayerController(this, 0) as BP_PC_C;

            UIGameMainAutoGenFunc(BP_PC.InputType);

            BP_PC.KeySwitch.Add(this, OnKeySwitch);

            var Pawn = BP_PC.K2_GetPawn();

            Pawn.EnableInput(BP_PC);

            Pawn.SetActorTickEnabled(true);

            UWidgetBlueprintLibrary.SetFocusToGameViewport();

            CommonBorder_1.SetRenderTransform(new FWidgetTransform
            {
                Translation = GetPosition(),
                Scale = new FVector2D(1.0, 1.0),
                Shear = new FVector2D(),
                Angle = 0.0f
            });
        }

        [IsOverride]
        public override void BP_OnDeactivated()
        {
            var BP_PC = UGameplayStatics.GetPlayerController(this, 0) as BP_PC_C;

            if (BP_PC != null && BP_PC.IsValid())
            {
                BP_PC.KeySwitch.RemoveAll(this);
            }
        }

        private void OnKeySwitch(E_InputType NewType)
        {
            UIGameMainAutoGenFunc(NewType);
        }

        private void UIGameMainAutoGenFunc(E_InputType NewInput = E_InputType.Unknown)
        {
            var PlayerController = UGameplayStatics.GetPlayerController(this, 0);

            PlayerController.bShowMouseCursor = false;

            if (NewInput == E_InputType.Unknown)
            {
                UWidgetBlueprintLibrary.SetInputMode_GameAndUIEx(PlayerController, null, EMouseLockMode.DoNotLock,
                    false);
            }
            else if (NewInput == E_InputType.KeyMouse)
            {
                PlayerController.bShowMouseCursor = true;

                UWidgetBlueprintLibrary.SetInputMode_GameAndUIEx(PlayerController, null, EMouseLockMode.DoNotLock,
                    false);
            }
            else if (NewInput == E_InputType.Gamepad)
            {
                UWidgetBlueprintLibrary.SetInputMode_GameOnly(PlayerController, true);
            }
            else if (NewInput == E_InputType.Touch)
            {
                UWidgetBlueprintLibrary.SetInputMode_GameAndUIEx(PlayerController, null, EMouseLockMode.DoNotLock,
                    false);
            }

            UWidgetBlueprintLibrary.SetFocusToGameViewport();
        }

        private void OnPlaceBtnClicked(UCommonButtonBase Button)
        {
            var BP_PC = UGameplayStatics.GetPlayerController(this, 0) as BP_PC_C;

            var BP_Player = BP_PC.K2_GetPawn() as BP_Player_C;

            BP_Player.Spawn_h20_Build_h20_Target();
        }

        private void OnRotateBtnClicked(UCommonButtonBase Button)
        {
            var BP_PC = UGameplayStatics.GetPlayerController(this, 0) as BP_PC_C;

            var BP_Player = BP_PC.K2_GetPawn() as BP_Player_C;

            BP_Player.Rotate_h20_Spawn();
        }

        private void OnCancelBtnClicked(UCommonButtonBase Button)
        {
            var BP_PC = UGameplayStatics.GetPlayerController(this, 0) as BP_PC_C;

            var BP_Player = BP_PC.K2_GetPawn() as BP_Player_C;

            BP_Player.Destroy_h20_Spawn();

            DeactivateWidget();
        }

        FVector2D GetPosition()
        {
            /*
             * Target position relative to viewport scaled viewport size
             */
            var BP_PC = UGameplayStatics.GetPlayerController(this, 0) as BP_PC_C;

            var BP_Player = BP_PC.K2_GetPawn() as BP_Player_C;

            if (BP_Player.Spawn == null || !BP_Player.Spawn.IsValid())
            {
                return FVector2D.ZeroVector;
            }

            var Location = BP_Player.Spawn.K2_GetActorLocation();

            /*
             * Lock range to keep on screen
             */
            var ScreenPosition = new FVector2D();

            UGameplayStatics.ProjectWorldToScreen(BP_PC, Location, ref ScreenPosition, true);

            var ViewportScale = UWidgetLayoutLibrary.GetViewportScale(this);

            var ViewportSize = UWidgetLayoutLibrary.GetViewportSize(this);

            return new FVector2D(
                Math.Clamp((ScreenPosition / ViewportScale).X, 150.0, ViewportSize.X / ViewportScale - 150.0),
                Math.Clamp((ScreenPosition / ViewportScale).Y, 0.0, ViewportSize.Y / ViewportScale - 350.0)
            );
        }
    }
}