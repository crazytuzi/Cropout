using Script.Common;
using Script.CoreUObject;
using Script.Engine;
using Script.Game.Blueprint.Interactable.Building;

namespace Script.Game.Blueprint.Villagers.AI.EQS
{
    [IsOverride]
    public partial class EQC_TargetTownHall_C
    {
        [IsOverride]
        public override void ProvideSingleLocation(UObject QuerierObject, AActor QuerierActor,
            ref FVector ResultingLocation)
        {
            var OutActors = new TArray<AActor>();

            UGameplayStatics.GetAllActorsOfClass(this, BPC_TownCenter_C.StaticClass(), ref OutActors);

            if (OutActors.IsValidIndex(0))
            {
                ResultingLocation = OutActors[0].K2_GetActorLocation();
            }
        }
    }
}