using Script.CoreUObject;
using Script.Engine;
using Script.Game.Blueprint.Core.GameMode;

namespace Script.Game.Blueprint.Interactable.Building
{
    [Override]
    public partial class BPC_EndGame_C
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

            BPF_Cropout_C.GetCropoutGM(this, out var GM);

            GM.EndGame(true);
        }
    }
}