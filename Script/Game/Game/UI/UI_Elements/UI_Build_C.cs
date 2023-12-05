using Script.Common;
using Script.CommonUI;
using Script.Engine;
using Script.Game.Blueprint.Core.Player;
using Script.Game.Blueprint.Interactable.Extras;
using Script.Game.UI.Common;
using Script.Library;
using Script.SlateCore;
using Script.UMG;

namespace Script.Game.UI.UI_Elements
{
    [IsOverride]
    public partial class UI_Build_C
    {
        /*
         * Get our placeable actors from the Data Table DT_Placeable
         */
        [IsOverride]
        public override void PreConstruct(bool IsDesignTime)
        {
            Container.ClearChildren();

            var OutRowNames = new TArray<FName>();

            var Buidables = Unreal.LoadObject<DT_Buidables>(this);

            UDataTableFunctionLibrary.GetDataTableRowNames(Buidables, ref OutRowNames);

            /*
             * For each data table entry, make a new UI element and add it to the container
             */
            foreach (var RowName in OutRowNames)
            {
                UDataTableFunctionLibrary.GetDataTableRowFromName<ST_Resource>(Buidables, RowName, out var OutRow);

                var CUI_BuildItem = Unreal.CreateWidget<CUI_BuildItem_C>(UGameplayStatics.GetPlayerController(this, 0));

                CUI_BuildItem.TableData = OutRow;

                CUI_BuildItem.Style = CUI_Style_Build_C.StaticClass();

                CUI_BuildItem.bApplyAlphaOnDisable = true;

                CUI_BuildItem.bLocked = false;

                CUI_BuildItem.bSelectable = false;

                CUI_BuildItem.bShouldSelectUponReceivingFocus = true;

                CUI_BuildItem.bInteractableWhenSelected = true;

                CUI_BuildItem.bToggleable = false;

                CUI_BuildItem.bTriggerClickedAfterSelection = false;

                CUI_BuildItem.bDisplayInputActionWhenNotInteractable = true;

                CUI_BuildItem.bHideInputActionWithKeyboard = false;

                CUI_BuildItem.bShouldUseFallbackDefaultInputAction = true;

                CUI_BuildItem.bRequiresHold = false;

                CUI_BuildItem.InputPriority = 0;

                Container.AddChildToHorizontalBox(CUI_BuildItem);

                (CUI_BuildItem.Slot as UHorizontalBoxSlot)?.SetVerticalAlignment(EVerticalAlignment.VAlign_Bottom);

                CUI_BuildItem.SetPadding(new FMargin { Left = 8.0f, Top = 0.0f, Right = 8.0f, Bottom = 5.0f });
            }
        }

        [IsOverride]
        public override void Construct()
        {
            BTN_Back.OnButtonBaseClicked.Add(this, OnBackBtnClicked);
        }

        [IsOverride]
        public override void Destruct()
        {
            BTN_Back.OnButtonBaseClicked.RemoveAll(this);
        }

        /*
         * Set Default Focus, Disable player input so background game doesn't move
         */
        [IsOverride]
        public override void BP_OnActivated()
        {
            var PlayerController = UGameplayStatics.GetPlayerController(this, 0);

            UWidgetBlueprintLibrary.SetInputMode_UIOnlyEx(PlayerController);

            GetDesiredFocusTarget().SetFocus();

            var Pawn = PlayerController.K2_GetPawn();

            Pawn.SetActorTickEnabled(false);

            Pawn.DisableInput(PlayerController);

            (Pawn as IBPI_Player_C)?.Switch_h20_Build_h20_Mode(true);
        }

        [IsOverride]
        public override UWidget GetDesiredFocusTarget()
        {
            return BTN_Back;
        }

        /*
         * On Back
         */
        private void OnBackBtnClicked(UCommonButtonBase Button)
        {
            var PlayerController = UGameplayStatics.GetPlayerController(this, 0);

            var Pawn = PlayerController.K2_GetPawn();

            Pawn.EnableInput(PlayerController);

            (Pawn as IBPI_Player_C)?.Switch_h20_Build_h20_Mode();

            DeactivateWidget();
        }
    }
}