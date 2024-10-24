using Script.AIModule;
using Script.CoreUObject;
using Script.Engine;

namespace Script.Game.Blueprint.Villagers.AI.EQS
{
    [Override]
    public partial class EQC_CollectionTarget_C
    {
        [Override]
        public override void ProvideSingleLocation(UObject QuerierObject, AActor QuerierActor,
            ref FVector ResultingLocation)
        {
            var AIController = UAIBlueprintHelperLibrary.GetAIController(QuerierActor);

            var Blackboard = UAIBlueprintHelperLibrary.GetBlackboard(AIController);

            if (Blackboard.GetValueAsObject(Key_h20_Name) is AActor Actor)
            {
                ResultingLocation = Actor.K2_GetActorLocation();
            }
        }
    }
}