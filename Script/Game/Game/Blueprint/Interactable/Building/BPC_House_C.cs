using System;
using Script.CoreUObject;
using Script.Engine;
using Script.Game.Blueprint.Core.GameMode;

namespace Script.Game.Blueprint.Interactable.Building
{
    [Override]
    public partial class BPC_House_C
    {
        [Override]
        public override void ReceiveBeginPlay()
        {
            base.ReceiveBeginPlay();
        }

        [Override]
        public override void ReceiveActorBeginOverlap(AActor OtherActor)
        {
            base.ReceiveActorBeginOverlap(OtherActor);
        }

        [Override]
        public override void ReceiveTick(float DeltaSeconds)
        {
            base.ReceiveTick(DeltaSeconds);
        }

        [Override]
        protected override void ConstructionComplete()
        {
            base.ConstructionComplete();

            SpawnVillagers();
        }

        [Override]
        public override void Placement_h20_Mode()
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
                BPF_Cropout_C.GetCropoutGM(this, out var GM);

                GM.SpawnVillagers(Villager_h20_Capacity);
            }
        }

        private Boolean bDoOnce = true;
    }
}