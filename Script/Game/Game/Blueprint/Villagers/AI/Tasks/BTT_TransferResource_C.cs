using Script.AIModule;
using Script.Common;
using Script.Engine;
using Script.Game.Blueprint.Core.GameMode;

namespace Script.Game.Blueprint.Villagers.AI.Tasks
{
    [IsOverride]
    public partial class BTT_TransferResource_C
    {
        [IsOverride]
        public override void ReceiveExecuteAI(AAIController OwnerController, APawn ControlledPawn)
        {
            /*
             * If both values are valid, continue, otherwise return fail
             */
            var From = UBTFunctionLibrary.GetBlackboardValueAsActor(this, Take_h20_From) as IBPI_Resource_C;

            var To = UBTFunctionLibrary.GetBlackboardValueAsActor(this, Give_h20_To) as IBPI_Resource_C;

            if (From != null && To != null)
            {
                /*
                 * transfer resources
                 */
                From.Remove_h20_Resource(out var Target_h20_Resource, out var Value);

                To.Add_h20_Resource(Target_h20_Resource, Value);

                FinishExecute(true);
            }
            else
            {
                FinishExecute(false);
            }
        }
    }
}