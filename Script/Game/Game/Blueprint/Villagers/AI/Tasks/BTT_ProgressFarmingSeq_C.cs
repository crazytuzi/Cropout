using System;
using System.Threading;
using System.Threading.Tasks;
using Script.AIModule;
using Script.CoreUObject;
using Script.Engine;
using Script.Game.Blueprint.Interactable;

namespace Script.Game.Blueprint.Villagers.AI.Tasks
{
    [Override]
    public partial class BTT_ProgressFarmingSeq_C
    {
        [Override]
        public override void ReceiveExecuteAI(AAIController OwnerController, APawn ControlledPawn)
        {
            /*
             * Progress Farm Plot
             */
            var Actor = UBTFunctionLibrary.GetBlackboardValueAsActor(this, Crop);

            if (Actor.ActorHasTag("Ready") || Actor.ActorHasTag("Harvest"))
            {
                Tag_h20_State = Actor.Tags[1];

                var BP_Interactable = Actor as BP_Interactable_C;

                BP_Interactable.Interact(out var NewParam);

                (ControlledPawn as IBPI_Villager_C)?.Play_h20_Work_h20_Anim(NewParam);

                TokenSource = new CancellationTokenSource();

                OnPlayWorkAnim(NewParam);
            }
            else
            {
                /*
                 * Find new crop
                 */
                UBTFunctionLibrary.SetBlackboardValueAsObject(this, Crop, null);

                FinishExecute(true);
            }
        }

        private async void OnPlayWorkAnim(Double NewParam)
        {
            while (!TokenSource.IsCancellationRequested)
            {
                await Task.Delay((Int32)(NewParam * 1000));

                if (Tag_h20_State == "Harvest")
                {
                    FinishExecute(true);
                }
                else
                {
                    /*
                     * Find new crop
                     */
                    UBTFunctionLibrary.SetBlackboardValueAsObject(this, Crop, null);

                    FinishExecute(true);
                }

                TokenSource.Cancel();
            }
        }

        private CancellationTokenSource TokenSource;
    }
}