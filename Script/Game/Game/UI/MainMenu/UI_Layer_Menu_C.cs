using Script.CoreUObject;

namespace Script.Game.UI.MainMenu
{
    [Override]
    public partial class UI_Layer_Menu_C
    {
        [Override]
        public override void BP_OnActivated()
        {
            var UI_MainMenu = MainStack.BP_AddWidget(UI_MainMenu_C.StaticClass()) as UI_MainMenu_C;

            UI_MainMenu.StackRef = MainStack;
        }
    }
}