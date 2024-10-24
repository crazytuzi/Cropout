using Script.AIModule;
using Script.CoreUObject;
using Script.Engine;

namespace Script.Game.Blueprint.Villagers.AI.Tasks
{
    [Override]
    public partial class BTT_DefaultBT_C
    {
        [Override]
        public override void ReceiveExecuteAI(AAIController OwnerController, APawn ControlledPawn)
        {
            var BPI_Villager = ControlledPawn as IBPI_Villager_C;

            BPI_Villager.Return_h20_To_h20_Default_h20_BT();

            FinishExecute(true);
        }
    }
}