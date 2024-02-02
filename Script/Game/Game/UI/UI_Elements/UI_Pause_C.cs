using Script.Common;
using Script.CommonUI;
using Script.Engine;
using Script.Game.Blueprint.Core.GameMode;
using Script.Game.Blueprint.Core.Save;
using Script.Library;
using Script.UMG;

namespace Script.Game.UI.UI_Elements
{
    [IsOverride]
    public partial class UI_Pause_C
    {
        [IsOverride]
        public override void Construct()
        {
            BTN_Resume.OnButtonBaseClicked.Add(this, OnResumeBtnClicked);

            BTN_Restart.OnButtonBaseClicked.Add(this, OnRestartBtnClicked);

            BTN_MainMenu.OnButtonBaseClicked.Add(this, OnMainMenuBtnClicked);
        }

        [IsOverride]
        public override void Destruct()
        {
            BTN_Resume.OnButtonBaseClicked.RemoveAll(this);

            BTN_Restart.OnButtonBaseClicked.RemoveAll(this);

            BTN_MainMenu.OnButtonBaseClicked.RemoveAll(this);
        }

        [IsOverride]
        public override void BP_OnActivated()
        {
            UWidgetBlueprintLibrary.SetInputMode_UIOnlyEx(
                UGameplayStatics.GetPlayerController(this, 0),
                GetDesiredFocusTarget());

            UGameplayStatics.SetGamePaused(this, true);

            Slider_Music.UpdateSlider();

            Slider_SFX.UpdateSlider();
        }

        [IsOverride]
        public override UWidget GetDesiredFocusTarget()
        {
            return BTN_Resume;
        }

        private void OnResumeBtnClicked(UCommonButtonBase Button)
        {
            UGameplayStatics.SetGamePaused(this, false);

            UWidgetBlueprintLibrary.SetFocusToGameViewport();

            DeactivateWidget();
        }

        private void OnRestartBtnClicked(UCommonButtonBase Button)
        {
            UGameplayStatics.SetGamePaused(this, false);

            BPF_Cropout_C.GetCropoutGI(this, out var GI);

            (GI as IBPI_GI_C).ClearSave();

            GI.OpenLevel(Unreal.LoadObject<Village>(this));
        }

        private void OnMainMenuBtnClicked(UCommonButtonBase Button)
        {
            UGameplayStatics.SetGamePaused(this, false);

            BPF_Cropout_C.GetCropoutGI(this, out var GI);

            GI.OpenLevel(Unreal.LoadObject<Script.Game.MainMenu>(this));
        }
    }
}