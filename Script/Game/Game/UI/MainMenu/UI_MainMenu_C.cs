using System;
using Script.Common;
using Script.CommonUI;
using Script.Engine;
using Script.Game.Blueprint.Core.GameMode;
using Script.Game.Blueprint.Core.Save;
using Script.Game.UI.UI_Elements;
using Script.Library;
using Script.OnlineSubsystemUtils;
using Script.UMG;

namespace Script.Game.UI.MainMenu
{
    [IsOverride]
    public partial class UI_MainMenu_C
    {
        [IsOverride]
        public override void Construct()
        {
            BTN_Continue.OnButtonBaseClicked.Add(this, OnContinueBtnClicked);

            BTN_NewGame.OnButtonBaseClicked.Add(this, OnNewGameBtnClicked);

            BTN_Quit.OnButtonBaseClicked.Add(this, OnQuitBtnClicked);

            BTN_Donate.OnButtonBaseClicked.Add(this, OnDonateBtnClicked);
        }

        [IsOverride]
        public override void Destruct()
        {
            BTN_Continue.OnButtonBaseClicked.RemoveAll(this);

            BTN_NewGame.OnButtonBaseClicked.RemoveAll(this);

            BTN_Quit.OnButtonBaseClicked.RemoveAll(this);

            BTN_Donate.OnButtonBaseClicked.RemoveAll(this);
        }

        [IsOverride]
        public override void BP_OnActivated()
        {
            /*
             * Reset focus on active (Get Desired Focus Target Is overriden function)
             */
            GetDesiredFocusTarget().SetFocus();

            /*
             * Check if save exists
             */
            var BP_GI = UGameplayStatics.GetGameInstance(this) as IBPI_GI_C;

            BP_GI.Check_h20_Save_h20_Bool(out var bSaveExist);

            bHasSave = bSaveExist;

            BTN_Continue.SetIsEnabled(bHasSave);

            /*
             * Set Donate to only visible on mobile
             */
            var PlatformName = UGameplayStatics.GetPlatformName();

            if (PlatformName == "Android" || PlatformName == "IOS")
            {
                BTN_Donate.SetVisibility(ESlateVisibility.Visible);

                /*
                 * Check If User has made in app purchase
                 */
                var InAppPurchaseQueryCallbackProxy2 =
                    UInAppPurchaseQueryCallbackProxy2.CreateProxyObjectForInAppPurchaseQuery(
                        UGameplayStatics.GetPlayerController(this, 0),
                        new TArray<FString> { "demogame_donate" });

                InAppPurchaseQueryCallbackProxy2.OnSuccess.Add(this, ReadInAppPurchaseInformation2);

                InAppPurchaseQueryCallbackProxy2.OnFailure.Add(this, ReadInAppPurchaseInformation2);
            }
            else
            {
                BTN_Donate.SetVisibility(ESlateVisibility.Collapsed);
            }
        }

        [IsOverride]
        public override UWidget BP_GetDesiredFocusTarget()
        {
            return bHasSave ? BTN_Continue : BTN_NewGame;
        }

        private void ReadInAppPurchaseInformation2(TArray<FOnlineProxyStoreOffer> InAppOfferInformation)
        {
            if (InAppOfferInformation[0].OfferId == "demogame_donate")
            {
            }
        }

        /*
         * Continue Game
         */
        private void OnContinueBtnClicked(UCommonButtonBase Button)
        {
            BPF_Cropout_C.Get_h20_Cropout_h20_GI(this, out var GI);

            GI.Open_h20_Level(Unreal.LoadObject<Village>(this));
        }

        /*
         * If save game already exists, prompt for new game
         */
        private void OnNewGameBtnClicked(UCommonButtonBase Button)
        {
            if (bHasSave)
            {
                var UI_Prompt = StackRef.BP_AddWidget(UI_Prompt_C.StaticClass()) as UI_Prompt_C;

                UI_Prompt.Prompt_h20_Question =
                    "Starting a new game will override your current save. Do you want to continue?";

                UI_Prompt.Confirm.Add(this, OnConfirmNewGame);
            }
            else
            {
                OnConfirmNewGame();
            }
        }

        private void OnConfirmNewGame()
        {
            var BP_GI = UGameplayStatics.GetGameInstance(this) as IBPI_GI_C;

            BP_GI.ClearSave(true);

            BPF_Cropout_C.Get_h20_Cropout_h20_GI(this, out var GI);

            GI.Open_h20_Level(Unreal.LoadObject<Village>(this));
        }

        /*
         * Add Prompt and set question
         */
        private void OnQuitBtnClicked(UCommonButtonBase Button)
        {
            var UI_Prompt = StackRef.BP_AddWidget(UI_Prompt_C.StaticClass()) as UI_Prompt_C;

            UI_Prompt.Prompt_h20_Question = "Are you sure you want to quit?";

            UI_Prompt.Confirm.Add(this, OnConfirmQuit);
        }

        private void OnConfirmQuit()
        {
            UKismetSystemLibrary.QuitGame(this, null, EQuitPreference.Quit, false);
        }

        /*
         * If save game already exists, prompt for new game
         */
        private void OnDonateBtnClicked(UCommonButtonBase Button)
        {
            if (bHasSave)
            {
                var UI_Prompt = StackRef.BP_AddWidget(UI_Prompt_C.StaticClass()) as UI_Prompt_C;

                UI_Prompt.Prompt_h20_Question =
                    "Would you like to donate \u00a31.99 to help make the game better? ";

                UI_Prompt.Confirm.Add(this, OnConfirmDonate);
            }
            else
            {
                OnConfirmNewGame();
            }
        }

        private void OnConfirmDonate()
        {
            ConfirmDonate();
        }

        /*
         * Donate Button (Requires setup on Platform)
         */
        private void ConfirmDonate()
        {
            var InAppPurchaseCheckoutCallbackProxy =
                UInAppPurchaseCheckoutCallbackProxy.CreateProxyObjectForInAppPurchaseCheckout(
                    UGameplayStatics.GetPlayerController(this, 0),
                    new FInAppPurchaseProductRequest2 { ProductIdentifier = "demogame_donate", bIsConsumable = false });

            InAppPurchaseCheckoutCallbackProxy.OnSuccess.Add(this, OnDonateSuccess);

            InAppPurchaseCheckoutCallbackProxy.OnFailure.Add(this, OnDonateFailure);
        }

        private void OnDonateSuccess(EInAppPurchaseStatus PurchaseStatus,
            FInAppPurchaseReceiptInfo2 InAppPurchaseReceipt)
        {
            var UI_Prompt = StackRef.BP_AddWidget(UI_Prompt_C.StaticClass()) as UI_Prompt_C;

            UI_Prompt.Prompt_h20_Question =
                "Thank you for your contribution! We have added a gold star to your main menu.";
        }

        private void OnDonateFailure(EInAppPurchaseStatus PurchaseStatus,
            FInAppPurchaseReceiptInfo2 InAppPurchaseReceipt)
        {
            var UI_Prompt = StackRef.BP_AddWidget(UI_Prompt_C.StaticClass()) as UI_Prompt_C;

            UI_Prompt.Prompt_h20_Question =
                "Unfortunately payment could not be processed";
        }

        private Boolean bHasSave;
    }
}