using System;
using Script.Common;
using Script.Engine;
using Script.Game.Blueprint.Core.GameMode;

namespace Script.Game.Blueprint.Interactable.Building
{
    [IsOverride]
    public partial class BPC_House_C
    {
        [IsOverride]
        public override void ReceiveBeginPlay()
        {
            base.ReceiveBeginPlay();
        }

        [IsOverride]
        public override void ReceiveActorBeginOverlap(AActor OtherActor)
        {
            base.ReceiveActorBeginOverlap(OtherActor);
        }

        [IsOverride]
        public override void ReceiveTick(float DeltaSeconds)
        {
            base.ReceiveTick(DeltaSeconds);
        }

        [IsOverride]
        protected override void ConstructionComplete()
        {
            base.ConstructionComplete();

            SpawnVillagers();
        }

        [IsOverride]
        protected override void Placement_h20_Mode()
        {
            base.Placement_h20_Mode();

            Nav_h20_Blocker.K2_DestroyComponent(this);
        }

        /*
         * Spawn Villagers, called when villagers finish making building.
         */
        private void SpawnVillagers()
        {
            if (bDoOnce)
            {
                BPF_Cropout_C.Get_h20_Cropout_h20_GM(this, out var GM);

                GM.Spawn_h20_Villagers(Villager_h20_Capacity);
            }
        }

        private Boolean bDoOnce = true;
    }
}