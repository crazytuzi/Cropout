using System;
using Script.AIModule;
using Script.CoreUObject;
using Script.Engine;

namespace Script.Game.Blueprint.Villagers.AI.Tasks
{
    [Override]
    public partial class BTT_FindBounds_C
    {
        [Override]
        public override void ReceiveExecuteAI(AAIController OwnerController, APawn ControlledPawn)
        {
            var TargetActor = UBTFunctionLibrary.GetBlackboardValueAsActor(this, Target);

            if (TargetActor != null)
            {
                var Origin = new FVector();

                var BoxExtent = new FVector();

                TargetActor.GetActorBounds(false, ref Origin, ref BoxExtent, false);

                UBTFunctionLibrary.SetBlackboardValueAsFloat(this, BB_h20_Bound,
                    (Single)(Math.Min(BoxExtent.X, BoxExtent.Y) + Additional_h20_Bounds));

                FinishExecute(true);
            }
            else
            {
                FinishExecute(false);
            }
        }
    }
}