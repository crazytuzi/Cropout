using System;
using Script.Common;
using Script.CoreUObject;
using Script.Engine;
using Script.Game.Blueprint.Core.GameMode;
using Script.Game.Blueprint.Core.Player;
using Script.Game.Blueprint.Interactable.Extras;
using Script.Game.UI.UI_Elements;
using Script.Library;
using Script.UMG;

namespace Script.Game.UI.Common
{
    [IsOverride]
    public partial class CUI_BuildItem_C
    {
        [IsOverride]
        public override void PreConstruct(bool IsDesignTime)
        {
            /*
             * Set Title and Image
             */
            Txt_Title.SetText(TableData.Title);

            Img_Icon.SetBrushFromTexture(TableData.UI_h20_Icon.LoadSynchronous());

            /*
             * Async Get Class
             */
            HardClassRef = TableData.Target_h20_Class.LoadSynchronous();

            BG.SetBrushColor(new FLinearColor(TableData.Tab_h20_Col));

            /*
             * Set Resource Costs
             */
            CostContainer.ClearChildren();

            foreach (var ResourceCost in TableData.Cost)
            {
                var UIE_Cost = Unreal.CreateWidget<UIE_Cost_C>(UGameplayStatics.GetPlayerController(this, 0));

                UIE_Cost.Cost = ResourceCost.Value;

                UIE_Cost.Resource = ResourceCost.Key;

                var HorizontalBoxSlot = CostContainer.AddChildToHorizontalBox(UIE_Cost);

                HorizontalBoxSlot.SetSize(new FSlateChildSize { Value = 1.0f, SizeRule = ESlateSizeRule.Fill });
            }
        }

        /*
         * Refresh Button activation based on available resources
         */
        [IsOverride]
        public override void Construct()
        {
            SetIsInteractionEnabled(ResourceCheck());

            var BP_GM = UGameplayStatics.GetGameMode(this) as BP_GM_C;

            BP_GM.Update_h20_Resources.Add(this, OnUpdateResources);
        }

        [IsOverride]
        public override void Destruct()
        {
            var BP_GM = UGameplayStatics.GetGameMode(this) as BP_GM_C;

            BP_GM.Update_h20_Resources.RemoveAll(this);
        }

        /*
         * On pressed get player and trigger Begin Build, set focus back to game
         */
        [IsOverride]
        public override void BP_OnClicked()
        {
            (GetOwningPlayer().K2_GetPawn() as IBPI_Player_C)?.BeginBuild(HardClassRef, TableData.Cost);

            (UGameplayStatics.GetGameMode(this) as IBPI_Player_C)?.Add_h20_UI(UI_BuildConfirm_C.StaticClass());
        }

        [IsOverride]
        public override void BP_OnHovered()
        {
            BaseSize.SetMinDesiredHeight(300.0f);

            PlayAnimation(Loop_Hover, 0.0f, 0);

            PlayAnimation(Highlight_In);
        }

        [IsOverride]
        public override void BP_OnUnhovered()
        {
            BaseSize.SetMinDesiredHeight(250.0f);

            StopAnimation(Loop_Hover);

            PlayAnimation(Hightlight_Out);
        }

        private bool ResourceCheck()
        {
            var BPI_Resource = UGameplayStatics.GetGameMode(this) as IBPI_Resource_C;

            foreach (var ResourceCost in TableData.Cost)
            {
                BPI_Resource.Check_h20_Resource(ResourceCost.Key, out var NewParam, out var NewParam3);

                if (!NewParam || NewParam3 < ResourceCost.Value)
                {
                    return false;
                }
            }

            return true;
        }

        private void OnUpdateResources(E_ResourceType Resource, Int32 Value)
        {
            SetIsInteractionEnabled(ResourceCheck());
        }
    }
}