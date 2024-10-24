using Script.CoreUObject;
using Script.Engine;

namespace Script.Game.UI.Common
{
    [Override]
    public partial class CUI_Button_C
    {
        [Override]
        public override void PreConstruct(bool IsDesignTime)
        {
            ButtonTitle.SetText(Button_h20_Text);

            var Height = UGameplayStatics.GetPlatformName() == "Android" || UGameplayStatics.GetPlatformName() == "IOS"
                ? MinHeight * 1.5
                : MinHeight;

            SizeBox_42.SetMinDesiredHeight((float)Height);

            GamepadIcon.SetInputAction(TriggeringInputAction);
        }
    }
}