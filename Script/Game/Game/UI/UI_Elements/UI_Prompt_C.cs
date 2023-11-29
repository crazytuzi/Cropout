using Script.Common;
using Script.CommonUI;
using Script.UMG;

namespace Script.Game.UI.UI_Elements
{
    [IsOverride]
    public partial class UI_Prompt_C
    {
        [IsOverride]
        public override void Construct()
        {
            BTN_Pos.OnButtonBaseClicked.Add(this, OnConfirmBtnClicked);

            BTN_Neg.OnButtonBaseClicked.Add(this, OnBackBtnClicked);
        }

        [IsOverride]
        public override void Destruct()
        {
            BTN_Pos.OnButtonBaseClicked.RemoveAll(this);

            BTN_Neg.OnButtonBaseClicked.RemoveAll(this);
        }

        /*
         * Set Text & Focus
         */
        [IsOverride]
        public override void BP_OnActivated()
        {
            GetDesiredFocusTarget().SetFocus();

            Title.SetText(Prompt_h20_Question);
        }

        [IsOverride]
        public override UWidget BP_GetDesiredFocusTarget()
        {
            return BTN_Neg;
        }

        /*
         * Event Dispatch, clear bindings and widget
         */
        private void OnConfirmBtnClicked(UCommonButtonBase Button)
        {
            Confirm.Broadcast();

            Confirm.Clear();

            Back.Clear();

            DeactivateWidget();
        }

        /*
         * Event Dispatch, clear bindings and widget
         */
        private void OnBackBtnClicked(UCommonButtonBase Button)
        {
            Back.Broadcast();

            Confirm.Clear();

            Back.Clear();

            DeactivateWidget();
        }
    }
}