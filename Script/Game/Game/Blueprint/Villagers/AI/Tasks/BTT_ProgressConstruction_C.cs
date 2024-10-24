using System.Threading;
using System.Threading.Tasks;
using Script.AIModule;
using Script.CoreUObject;
using Script.Engine;
using Script.Game.Blueprint.Interactable;

namespace Script.Game.Blueprint.Villagers.AI.Tasks
{
    [Override]
    public partial class BTT_ProgressConstruction_C
    {
        [Override]
        public override void ReceiveExecuteAI(AAIController OwnerController, APawn ControlledPawn)
        {
            (ControlledPawn as IBPI_Villager_C)?.Play_h20_Work_h20_Anim(1.0);

            TokenSource = new CancellationTokenSource();

            OnPlayWorkAnim();
        }

        private async void OnPlayWorkAnim()
        {
            while (!TokenSource.IsCancellationRequested)
            {
                await Task.Delay(1000);

                var BP_Interactable = UBTFunctionLibrary.GetBlackboardValueAsActor(
                    this, TargetBuild) as BP_Interactable_C;

                BP_Interactable.Interact(out var NewParam);

                if (NewParam <= 0.0)
                {
                    FinishExecute(true);

                    UBTFunctionLibrary.SetBlackboardValueAsObject(this, TargetBuild, null);
                }
                else
                {
                    FinishExecute(true);
                }

                TokenSource.Cancel();
            }
        }

        private CancellationTokenSource TokenSource;
    }
}