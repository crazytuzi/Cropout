using Script.CommonUI;
using Script.CoreUObject;
using Script.Engine;
using Script.Game.Blueprint.Core.Player;
using Script.Game.Blueprint.Core.Player.Input;
using Script.Game.UI.UI_Elements;
using Script.UMG;

namespace Script.Game.UI.Game
{
    [Override]
    public partial class UI_GameMain_C
    {
        [Override]
        public override void Construct()
        {
            CUI_Button_55.OnButtonBaseClicked.Add(this, OnCUIBtnClicked);
        }

        [Override]
        public override void Destruct()
        {
            CUI_Button_55.OnButtonBaseClicked.RemoveAll(this);
        }

        /*
         * Set UI input mode.
         * Switch between Game and UI and UI only to avoid common UI eating gamepad input.
         */
        [Override]
        public override void BP_OnActivated()
        {
            var BP_PC = UGameplayStatics.GetPlayerController(this, 0) as BP_PC_C;

            UIGameMainAutoGeneratorFunction(BP_PC.InputType);

            BP_PC.KeySwitch.Add(this, OnKeySwitch);

            var Pawn = BP_PC.K2_GetPawn();

            Pawn.EnableInput(BP_PC);

            Pawn.SetActorTickEnabled(true);

            UWidgetBlueprintLibrary.SetFocusToGameViewport();
        }

        private void UIGameMainAutoGeneratorFunction(E_InputType NewInputType = E_InputType.Unknown)
        {
            var PlayerController = UGameplayStatics.GetPlayerController(this, 0);

            PlayerController.bShowMouseCursor = false;

            if (NewInputType == E_InputType.Unknown)
            {
                UWidgetBlueprintLibrary.SetInputMode_GameAndUIEx(PlayerController, null, EMouseLockMode.DoNotLock,
                    false);
            }
            else if (NewInputType == E_InputType.KeyMouse)
            {
                PlayerController.bShowMouseCursor = true;

                UWidgetBlueprintLibrary.SetInputMode_GameAndUIEx(PlayerController, null, EMouseLockMode.DoNotLock,
                    false);
            }
            else if (NewInputType == E_InputType.Gamepad)
            {
                UWidgetBlueprintLibrary.SetInputMode_GameOnly(PlayerController, true);
            }
            else if (NewInputType == E_InputType.Touch)
            {
                UWidgetBlueprintLibrary.SetInputMode_GameAndUIEx(PlayerController, null, EMouseLockMode.DoNotLock,
                    false);
            }

            UWidgetBlueprintLibrary.SetFocusToGameViewport();
        }

        private void OnKeySwitch(E_InputType NewType)
        {
            UIGameMainAutoGeneratorFunction(NewType);
        }

        /*
         * Button Interactions
         */
        private void OnCUIBtnClicked(UCommonButtonBase Button)
        {
            var BPI_Player = UGameplayStatics.GetGameMode(this) as IBPI_Player_C;

            BPI_Player.Add_h20_UI(UI_Build_C.StaticClass());
        }
    }
}