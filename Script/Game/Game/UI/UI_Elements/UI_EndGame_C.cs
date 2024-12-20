using Script.AudioModulation;
using Script.CommonUI;
using Script.CoreUObject;
using Script.Engine;
using Script.Game.Audio.DATA.ControlBus;
using Script.Game.Blueprint.Core.GameMode;
using Script.UMG;

namespace Script.Game.UI.UI_Elements
{
    [Override]
    public partial class UI_EndGame_C
    {
        [Override]
        public override void Construct()
        {
            /*
             * Button Response
             */
            BTN_Retry.OnButtonBaseClicked.Add(this, OnPlayAgainBtnClicked);

            BTN_Continue.OnButtonBaseClicked.Add(this, OnContinueBtnClicked);

            BTN_MainMenu.OnButtonBaseClicked.Add(this, OnMainMenuBtnClicked);
        }

        [Override]
        public override void Destruct()
        {
            BTN_Retry.OnButtonBaseClicked.RemoveAll(this);

            BTN_Continue.OnButtonBaseClicked.RemoveAll(this);

            BTN_MainMenu.OnButtonBaseClicked.RemoveAll(this);
        }

        [Override]
        public override void BP_OnActivated()
        {
            var PlayerController = UGameplayStatics.GetPlayerController(this, 0);

            UWidgetBlueprintLibrary.SetInputMode_UIOnlyEx(PlayerController, BP_GetDesiredFocusTarget());

            PlayerController.K2_GetPawn().DisableInput(PlayerController);

            UGameplayStatics.SetGamePaused(this, true);
        }

        [Override]
        public override UWidget GetDesiredFocusTarget()
        {
            return BTN_Continue;
        }

        /*
         * End Game Variance
         */
        public void EndGame(bool bIsWin = false)
        {
            WIN = bIsWin;

            if (WIN)
            {
                MainText.SetText(Win_h20_Text);

                UAudioModulationStatics.SetGlobalBusMixValue(this, Unreal.LoadObject<Cropout_Music_WinLose>(this), 1.0f,
                    0.0f);
            }
            else
            {
                MainText.SetText(Lose_h20_Text);

                UAudioModulationStatics.SetGlobalBusMixValue(this, Unreal.LoadObject<Cropout_Music_WinLose>(this), 0.0f,
                    0.0f);
            }

            BPF_Cropout_C.GetCropoutGI(this, out var GI);

            GI.StopMusic();
        }

        private void OnPlayAgainBtnClicked(UCommonButtonBase Button)
        {
            BPF_Cropout_C.GetCropoutGI(this, out var GI);

            GI.ClearSave();

            GI.OpenLevel(Unreal.LoadObject<Village>(this));

            UGameplayStatics.SetGamePaused(this, false);
        }

        private void OnContinueBtnClicked(UCommonButtonBase Button)
        {
            UGameplayStatics.SetGamePaused(this, false);

            DeactivateWidget();
        }

        private void OnMainMenuBtnClicked(UCommonButtonBase Button)
        {
            BPF_Cropout_C.GetCropoutGI(this, out var GI);

            GI.OpenLevel(Unreal.LoadObject<Script.Game.MainMenu>(this));

            UGameplayStatics.SetGamePaused(this, false);
        }
    }
}