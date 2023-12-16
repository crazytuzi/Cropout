using Script.AIModule;
using Script.Common;
using Script.CoreUObject;
using Script.Engine;
using Script.Niagara;

namespace Script.Game.Blueprint.Villagers.AI.Tasks
{
    [IsOverride]
    public partial class BTT_PlayNiagara_C
    {
        [IsOverride]
        public override void ReceiveExecuteAI(AAIController OwnerController, APawn ControlledPawn)
        {
            UNiagaraFunctionLibrary.SpawnSystemAttached(System,
                ControlledPawn.RootComponent,
                "",
                new FVector(),
                new FRotator(),
                EAttachLocation.KeepRelativeOffset,
                false);

            FinishExecute(true);
        }
    }
}