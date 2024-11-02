using Script.CoreUObject;
using Script.Engine;
using Script.Game.Blueprint.Interactable.Extras;
using Script.Game.Environment.Materials.Textures;
using Script.Game.UI.Materials.Textures;

namespace Script.Game.UI.UI_Elements
{
    [Override]
    public partial class UIE_Cost_C
    {
        [Override]
        public override void PreConstruct(bool IsDesignTime)
        {
            C_Cost.SetText(Cost.ToString());

            UTexture2D SoftTexture = null;

            if (Resource == E_ResourceType.None)
            {
                SoftTexture = Unreal.LoadObject<_h01_127grey_VT>(this);
            }
            else if (Resource == E_ResourceType.Food)
            {
                SoftTexture = Unreal.LoadObject<T_Rsrc_Food_DA>(this);
            }
            else if (Resource == E_ResourceType.Wood)
            {
                SoftTexture = Unreal.LoadObject<T_Rsrc_Wood_DA>(this);
            }
            else if (Resource == E_ResourceType.Stone)
            {
                SoftTexture = Unreal.LoadObject<T_Rsrc_Stone_DA>(this);
            }

            Image_17.SetBrushFromTexture(SoftTexture);
        }

        public int Cost;

        public E_ResourceType Resource;
    }
}