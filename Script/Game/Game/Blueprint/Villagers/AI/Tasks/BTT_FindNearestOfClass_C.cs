using Script.AIModule;
using Script.Common;
using Script.Engine;
using Script.NavigationSystem;

namespace Script.Game.Blueprint.Villagers.AI.Tasks
{
    [IsOverride]
    public partial class BTT_FindNearestOfClass_C
    {
        [IsOverride]
        public override void ReceiveExecute(AActor OwnerActor)
        {
            /*
             * Is Target Already Set and Valid?
             */
            var TargetActor = UBTFunctionLibrary.GetBlackboardValueAsActor(this, Target);

            if (TargetActor != null)
            {
                FinishExecute(true);
            }
            else
            {
                /*
                 * Get Target Class and Switch if a tag filter is present
                 */
                var Tag = Use_h20_Blackboard_h20_Tag
                    ? UBTFunctionLibrary.GetBlackboardValueAsName(this, Blackboard_h20_Tag)
                    : Tag_h20_Filter;

                var TargetClass = UBTFunctionLibrary.GetBlackboardValueAsClass(this, Target_h20_Class);

                var Class = Use_h20_Blackboard_h20_Class ? TargetClass : Manual_h20_Class;

                if (Tag == "")
                {
                    /*
                     * Find nearest actor of class type and set to BB
                     */
                    var OutActors = new TArray<AActor>();

                    UGameplayStatics.GetAllActorsOfClass(
                        this,
                        new TSubclassOf<AActor>(Class.Get()),
                        ref OutActors);

                    PossibleActors = OutActors;

                    if (OutActors.Num() == 0)
                    {
                        /*
                         * If no available actors, finish execute with fail
                         */
                        FinishExecute(false);

                        return;
                    }
                }
                else
                {
                    /*
                     * Find nearest actor of class type with tag and set to BB
                     */
                    var OutActors = new TArray<AActor>();

                    UGameplayStatics.GetAllActorsOfClassWithTag(
                        this,
                        new TSubclassOf<AActor>(Class.Get()),
                        Tag,
                        ref OutActors);

                    PossibleActors = OutActors;

                    if (OutActors.Num() == 0)
                    {
                        /*
                         * If no available actors, finish execute with fail
                         */
                        FinishExecute(false);

                        return;
                    }
                }

                Path_h20_Found = false;

                /*
                 * Loop through actor array and check if villager can reach it
                 */
                while (PossibleActors.Num() > 0)
                {
                    /*
                     * Check Path from villager to nearest actor
                     */
                    var NearestTo = UBTFunctionLibrary.GetBlackboardValueAsActor(this, Nearest_h20_To);

                    var Distance = 0.0f;

                    var NearestActor = UGameplayStatics.FindNearestActor(
                        NearestTo.K2_GetActorLocation(),
                        PossibleActors,
                        ref Distance);

                    var ActorSynchronously = UNavigationSystemV1.FindPathToActorSynchronously(
                        this,
                        NearestTo.K2_GetActorLocation(),
                        NearestActor,
                        100.0f);

                    if (ActorSynchronously.IsPartial())
                    {
                        /*
                         * If Path is only partial, remove actor from array
                         */
                        PossibleActors.Remove(NearestTo);
                    }
                    else
                    {
                        /*
                         * If Path is available, set target and stop looking
                         */
                        Path_h20_Found = true;

                        New_h20_Target = NearestActor;

                        /*
                         * Once available actor is found, don't bother  checking the rest
                         */
                        break;
                    }
                }

                /*
                 * Set Target to Blackboard if found and Execute Success, Otherwise, Fail
                 */
                if (Path_h20_Found)
                {
                    UBTFunctionLibrary.SetBlackboardValueAsObject(this, Target, New_h20_Target);

                    FinishExecute(true);
                }
                else
                {
                    FinishExecute(false);
                }
            }
        }
    }
}