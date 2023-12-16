using Script.AIModule;
using Script.Common;
using Script.Engine;
using Script.Game.Blueprint.Interactable.Building;
using Script.Game.Blueprint.Interactable.Resources;

namespace Script.Game.Blueprint.Villagers.AI.Tasks
{
    [IsOverride]
    public partial class BTT_InitialCollectResource_C
    {
        [IsOverride]
        public override void ReceiveExecuteAI(AAIController OwnerController, APawn ControlledPawn)
        {
            /*
             * Check if any remaining resources available to gather
             */
            var OutActors = new TArray<AActor>();

            UGameplayStatics.GetAllActorsOfClassWithTag(
                this,
                BP_Resource_C.StaticClass(),
                ControlledPawn.Tags[0],
                ref OutActors);

            if (OutActors.Num() > 0)
            {
                UBTFunctionLibrary.SetBlackboardValueAsName(this, Key_ResourceTag, ControlledPawn.Tags[0]);

                /*
                 * Set Target Class reference
                 */
                UBTFunctionLibrary.SetBlackboardValueAsClass(this, Key_ResourceClass, BP_Resource_C.StaticClass());

                /*
                 * Check if a valid target is already stored
                 */
                if (ControlledPawn is BP_Villager_C { Target_h20_Ref: not null } BP_Villager)
                {
                    UBTFunctionLibrary.SetBlackboardValueAsObject(this, Key_Resource, BP_Villager.Target_h20_Ref);
                }

                /*
                 * Store Ref to town hall
                 */
                var BPC_TownCenters = new TArray<AActor>();

                UGameplayStatics.GetAllActorsOfClass(this, BPC_TownCenter_C.StaticClass(), ref BPC_TownCenters);

                UBTFunctionLibrary.SetBlackboardValueAsObject(this, Key_TownHall, BPC_TownCenters[0]);

                FinishExecute(true);
            }
            else
            {
                FinishExecute(false);
            }
        }
    }
}