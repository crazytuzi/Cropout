using System;
using System.Threading;
using System.Threading.Tasks;
using Script.AIModule;
using Script.CoreUObject;
using Script.Engine;
using Script.Game.Blueprint.Core.GameMode;
using Script.Game.Blueprint.Interactable;

namespace Script.Game.Blueprint.Villagers.AI.Tasks
{
    [Override]
    public partial class BTT_Work_C
    {
        [Override]
        public override void ReceiveExecuteAI(AAIController OwnerController, APawn ControlledPawn)
        {
            /*
             * If both values are valid, continue, otherwise return fail
             */
            var From = UBTFunctionLibrary.GetBlackboardValueAsActor(this, Take_h20_From);

            var To = UBTFunctionLibrary.GetBlackboardValueAsActor(this, Give_h20_To);

            if (From != null && To != null)
            {
                /*
                 * Play transfer event, wait for delay, transfer resources
                 */
                As_h20_BP_h20_Interactable = From as BP_Interactable_C;

                As_h20_BP_h20_Interactable.Play_h20_Wobble(ControlledPawn.K2_GetActorLocation());

                As_h20_BP_h20_Interactable.Interact(out var NewParam);

                (ControlledPawn as IBPI_Villager_C)?.Play_h20_Work_h20_Anim(NewParam);

                TokenSource = new CancellationTokenSource();

                OnPlayWorkAnim(NewParam);
            }
            else
            {
                FinishExecute(false);
            }
        }

        [Override]
        public override void ReceiveAbortAI(AAIController OwnerController, APawn ControlledPawn)
        {
            As_h20_BP_h20_Interactable.End_h20_Wobble();

            FinishExecute(false);
        }

        private async void OnPlayWorkAnim(Double Delay)
        {
            while (!TokenSource.IsCancellationRequested)
            {
                await Task.Delay((Int32)Delay * 1000);

                TokenSource.Cancel();

                var From = UBTFunctionLibrary.GetBlackboardValueAsActor(this, Take_h20_From) as IBPI_Resource_C;

                From.Remove_h20_Resource(out var Target_h20_Resource, out var Value);

                var To = UBTFunctionLibrary.GetBlackboardValueAsActor(this, Give_h20_To) as IBPI_Resource_C;
                ;

                To.Add_h20_Resource(Target_h20_Resource, Value);

                FinishExecute(true);
            }
        }

        private CancellationTokenSource TokenSource;
    }
}