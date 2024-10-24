using Script.AIModule;
using Script.CoreUObject;
using Script.Engine;
using Script.Game.Blueprint.Interactable.Building;
using Script.Game.Blueprint.Interactable.Resources;

namespace Script.Game.Blueprint.Villagers.AI.Tasks
{
    [Override]
    public partial class BTT_InitialFarmingTarget_C
    {
        [Override]
        public override void ReceiveExecuteAI(AAIController OwnerController, APawn ControlledPawn)
        {
            /*
             * Has a target resource already been set?
             */
            var Target = UBTFunctionLibrary.GetBlackboardValueAsActor(this, Key_Resource);

            if (Target == null)
            {
                var OutActors = new TArray<AActor>();

                UGameplayStatics.GetAllActorsOfClassWithTag(
                    this,
                    BP_BaseCrop_C.StaticClass(),
                    "Ready",
                    ref OutActors);

                if (OutActors.IsValidIndex(0))
                {
                    var Distance = 0.0f;

                    var NearestActor = UGameplayStatics.FindNearestActor(
                        ControlledPawn.K2_GetActorLocation(),
                        OutActors,
                        ref Distance);

                    UBTFunctionLibrary.SetBlackboardValueAsObject(this, Key_Resource, NearestActor);
                }
                else
                {
                    FinishExecute(false);

                    return;
                }

                /*
                 * Set Classes, used to find new asset types when original is nno longer available.
                 */
                UBTFunctionLibrary.SetBlackboardValueAsClass(this, Key_ResourceClass, BP_BaseCrop_C.StaticClass());

                UBTFunctionLibrary.SetBlackboardValueAsClass(this, Key_CollectionClass,
                    BP_BuildingBase_C.StaticClass());

                /*
                 * Store a backup of the town hall
                 */
                var BPC_TownCenters = new TArray<AActor>();

                UGameplayStatics.GetAllActorsOfClass(
                    this,
                    BPC_TownCenter_C.StaticClass(),
                    ref BPC_TownCenters);

                UBTFunctionLibrary.SetBlackboardValueAsObject(this, Key_TownHall, BPC_TownCenters[0]);

                FinishExecute(true);
            }
        }
    }
}