using System;
using Script.AudioModulation;
using Script.Common;
using Script.CoreUObject;
using Script.Engine;
using Script.Game.Audio.DATA.ControlBus;
using Script.Game.Audio.MUSIC;
using Script.Game.Blueprint.Core.Extras;
using Script.Game.Blueprint.Core.GameMode;
using Script.Game.UI.MainMenu;
using Script.Library;
using Script.OnlineSubsystemUtils;

namespace Script.Game.Blueprint.Core.MainMenu
{
    [IsOverride]
    public partial class BP_MainMenuGM_C
    {
        [IsOverride]
        public override void ReceiveBeginPlay()
        {
            /*
             * Create menu UI and add to viewport
             */
            var PlayerController = UGameplayStatics.GetPlayerController(this, 0);

            var UI_Layer_Menu = Unreal.CreateWidget<UI_Layer_Menu_C>(PlayerController);

            UI_Layer_Menu.AddToViewport();

            UI_Layer_Menu.ActivateWidget();

            var BP_GI = UGameplayStatics.GetGameInstance(this) as BP_GI_C;

            BP_GI.TransitionOut();

            UKismetRenderingLibrary.ClearRenderTarget2D(this,
                Unreal.LoadObject<RT_GrassMove>(this),
                FLinearColor.Black);

            /*
             * Mobile Only
             */
            var ShowLoginUICallbackProxy = UShowLoginUICallbackProxy.ShowExternalLoginUI(this, PlayerController);

            ShowLoginUICallbackProxy.OnSuccess.Add(this, OnSuccess);

            /*
             * Set Up Music Controls
             */
            UAudioModulationStatics.SetGlobalBusMixValue(this,
                Unreal.LoadObject<Cropout_Music_Piano_Vol>(this),
                0.0f,
                1.0f);

            UAudioModulationStatics.SetGlobalBusMixValue(this,
                Unreal.LoadObject<Cropout_Music_Perc_Vol>(this),
                0.0f,
                1.0f);

            UAudioModulationStatics.SetGlobalBusMixValue(this,
                Unreal.LoadObject<Cropout_Music_Strings_Delay>(this),
                0.5f,
                1.0f);

            UAudioModulationStatics.SetGlobalBusMixValue(this,
                Unreal.LoadObject<Cropout_Music_WinLose>(this),
                0.5f,
                10.0f);

            BP_GI.PlayMusic(Unreal.LoadObject<MUS_Main_MSS>(this));
        }

        private void OnSuccess(APlayerController PlayerController)
        {
            Console.WriteLine("Show login success");

            Console.WriteLine(UKismetSystemLibrary.GetDisplayName(PlayerController));
        }
    }
}