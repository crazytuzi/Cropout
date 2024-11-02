using System.Threading;
using System.Threading.Tasks;
using Script.CommonUI;
using Script.CoreUObject;
using Script.Engine;
using Script.Game.Blueprint.Core.GameMode;
using Script.Game.Blueprint.Core.Player;
using Script.Game.Blueprint.Core.Player.Input;
using Script.Game.Blueprint.Interactable.Extras;
using Script.Game.UI.UI_Elements;

namespace Script.Game.UI.Game
{
    [Override]
    public partial class UI_Layer_Game_C
    {
        [Override]
        public override void Construct()
        {
            Resources = E_ResourceType.Food;

            /*
             * Get All Resources in Resource Enum and add widget to UI
             */
            ResourceContainer.ClearChildren();

            TokenSource = new CancellationTokenSource();

            AddResource();

            /*
             * Bind to Villager Count
             */
            var BP_GM = UGameplayStatics.GetGameMode(this) as BP_GM_C;

            BP_GM.Update_h20_Villagers.Add(this, UpdateVillagerDetails);

            var BP_PC = UGameplayStatics.GetPlayerController(this, 0) as BP_PC_C;

            BP_PC.KeySwitch.Add(this, OnKeySwitch);

            BTN_Pause.OnButtonBaseClicked.Add(this, OnPauseBtnClicked);
        }

        [Override]
        public override void Destruct()
        {
            BTN_Pause.OnButtonBaseClicked.RemoveAll(this);

            TokenSource?.Cancel();
        }

        public void AddStackItem(TSubclassOf<UCommonActivatableWidget> ActivatableWidgetClass)
        {
            MainStack.BP_AddWidget(ActivatableWidgetClass);
        }

        public void PullCurrentActiveWidget()
        {
            MainStack.RemoveWidget(MainStack.GetActiveWidget());
        }

        public void EndGame(bool Win = false)
        {
            var UI_EndGame = MainStack.BP_AddWidget(UI_EndGame_C.StaticClass()) as UI_EndGame_C;

            UI_EndGame.EndGame(Win);

            UI_EndGame.ActivateWidget();
        }

        private async void AddResource()
        {
            while (!TokenSource.IsCancellationRequested)
            {
                await Task.Delay(100);

                /*
                 * Check if end of enum is reached
                 */
                if (Resources > E_ResourceType.Stone)
                {
                    TokenSource?.Cancel();
                }
                else
                {
                    var UIE_Resource =
                        Unreal.CreateWidget<UIE_Resource_C>(UGameplayStatics.GetPlayerController(this, 0));

                    UIE_Resource.ResourceType = Resources;

                    ResourceContainer.AddChild(UIE_Resource);

                    Resources += 1;
                }
            }
        }

        private void UpdateVillagerDetails(int VillagerCount)
        {
            VillagerCounter.SetText(VillagerCount.ToString());
        }

        private void OnKeySwitch(E_InputType NewType)
        {
            BTN_Pause.SetRenderOpacity(NewType == E_InputType.Gamepad ? 0.0f : 1.0f);
        }

        private void OnPauseBtnClicked(UCommonButtonBase Button)
        {
            MainStack.BP_AddWidget(UI_Pause_C.StaticClass());
        }

        private CancellationTokenSource TokenSource;

        private E_ResourceType Resources;
    }
}