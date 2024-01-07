using Script.Common;

namespace Script.Game.UI.MainMenu
{
    [IsOverride]
    public partial class UI_Layer_Menu_C
    {
        [IsOverride]
        public override void BP_OnActivated()
        {
            var UI_MainMenu = MainStack.BP_AddWidget(UI_MainMenu_C.StaticClass()) as UI_MainMenu_C;

            UI_MainMenu.StackRef = MainStack;
        }
    }
}