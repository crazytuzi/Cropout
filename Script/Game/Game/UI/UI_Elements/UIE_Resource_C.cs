using Script.CoreUObject;
using Script.Engine;
using Script.Game.Blueprint.Core.GameMode;
using Script.Game.Blueprint.Interactable.Extras;
using Script.Game.Environment.Materials.Textures;
using Script.Game.UI.Materials.Textures;

namespace Script.Game.UI.UI_Elements
{
    [Override]
    public partial class UIE_Resource_C
    {
        [Override]
        public override void OnInitialized()
        {
            var BP_GM = UGameplayStatics.GetGameMode(this) as BP_GM_C;

            BP_GM.Update_h20_Resources.Add(this, OnUpdateResources);
        }

        [Override]
        public override void PreConstruct(bool IsDesignTime)
        {
            UTexture2D SoftTexture = null;

            if (ResourceType == E_ResourceType.None)
            {
                SoftTexture = Unreal.LoadObject<_h01_127grey_VT>(this);
            }
            else if (ResourceType == E_ResourceType.Food)
            {
                SoftTexture = Unreal.LoadObject<T_Rsrc_Food_DA>(this);
            }
            else if (ResourceType == E_ResourceType.Wood)
            {
                SoftTexture = Unreal.LoadObject<T_Rsrc_Wood_DA>(this);
            }
            else if (ResourceType == E_ResourceType.Stone)
            {
                SoftTexture = Unreal.LoadObject<T_Rsrc_Stone_DA>(this);
            }

            Image_24.SetBrushFromTexture(SoftTexture);
        }

        [Override]
        public override void Construct()
        {
            var BPI_Resource = UGameplayStatics.GetGameMode(this) as IBPI_Resource_C;

            BPI_Resource.Check_h20_Resource(ResourceType, out var NewParam, out var NewParam3);

            Value = NewParam3;
        }

        private void OnUpdateResources(E_ResourceType Resource, int Value)
        {
            if (ResourceType == Resource)
            {
                PlayAnimation(Value > this.Value ? Increase : Reduce);

                this.Value = Value;
            }
        }

        public E_ResourceType ResourceType;
    }
}