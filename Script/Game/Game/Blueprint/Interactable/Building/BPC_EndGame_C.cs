using Script.Common;
using Script.Engine;
using Script.Game.Blueprint.Core.GameMode;

namespace Script.Game.Blueprint.Interactable.Building
{
    [IsOverride]
    public partial class BPC_EndGame_C
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

            BPF_Cropout_C.Get_h20_Cropout_h20_GM(this, out var GM);

            GM.EndGame(true);
        }
    }
}