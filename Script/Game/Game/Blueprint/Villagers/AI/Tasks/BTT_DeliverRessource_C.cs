using System;
using System.Threading;
using System.Threading.Tasks;
using Script.AIModule;
using Script.Common;
using Script.Engine;
using Script.Game.Blueprint.Core.GameMode;

namespace Script.Game.Blueprint.Villagers.AI.Tasks
{
    [IsOverride]
    public partial class BTT_DeliverRessource_C
    {
        /*
         * If both values are valid, continue, otherwise return fail
         */
        [IsOverride]
        public override void ReceiveExecuteAI(AAIController OwnerController, APawn ControlledPawn)
        {
            var From = UBTFunctionLibrary.GetBlackboardValueAsActor(this, Take_h20_From);

            var To = UBTFunctionLibrary.GetBlackboardValueAsActor(this, Give_h20_To);

            if (From != null && To != null)
            {
                /*
                 * Play transfer event, wait for delay, transfer resources
                 */
                var BPI_Villager = ControlledPawn as IBPI_Villager_C;

                BPI_Villager.Play_h20_Deliver_h20_Anim(out var Delay);

                TokenSource = new CancellationTokenSource();

                OnPlayDeliverAnim(Delay);
            }
            else
            {
                FinishExecute(false);
            }
        }

        private async void OnPlayDeliverAnim(Double Delay)
        {
            while (!TokenSource.IsCancellationRequested)
            {
                await Task.Delay((Int32)Delay * 1000);

                TokenSource.Cancel();

                var From = UBTFunctionLibrary.GetBlackboardValueAsActor(this, Take_h20_From) as IBPI_Resource_C;

                From.Remove_h20_Resource(out var Target_h20_Resource, out var Value);

                var BPI_Resource = UGameplayStatics.GetGameMode(this) as IBPI_Resource_C;

                BPI_Resource.Add_h20_Resource(Target_h20_Resource, Value);

                FinishExecute(true);
            }
        }

        private CancellationTokenSource TokenSource;
    }
}