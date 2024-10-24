using Script.AIModule;
using Script.CoreUObject;
using Script.Engine;

namespace Script.Game.Blueprint.Villagers.AI.Tasks
{
    [Override]
    public partial class BTT_StuckRecover_C
    {
        [Override]
        public override void ReceiveExecuteAI(AAIController OwnerController, APawn ControlledPawn)
        {
            var Position = UBTFunctionLibrary.GetBlackboardValueAsVector(this, Recovery_h20_Position);

            var SweepHitResult = new FHitResult();

            FinishExecute(ControlledPawn.K2_SetActorLocation(Position + new FVector { Z = 45.0 },
                false,
                ref SweepHitResult,
                false));
        }
    }
}