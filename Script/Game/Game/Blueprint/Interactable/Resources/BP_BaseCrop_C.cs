using System;
using System.Threading;
using System.Threading.Tasks;
using Script.CoreUObject;
using Script.Engine;
using Script.Game.Blueprint.Core.Save;

namespace Script.Game.Blueprint.Interactable.Resources
{
    [Override]
    public partial class BP_BaseCrop_C
    {
        [Override]
        public override void ReceiveBeginPlay()
        {
            base.ReceiveBeginPlay();
        }

        [Override]
        public override void ReceiveEndPlay(EEndPlayReason EndPlayReason)
        {
            base.ReceiveEndPlay(EndPlayReason);

            SwitchStageTokenSource?.Cancel();

            SetReadyTokenSource?.Cancel();
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
        public override void Set_h20_Progressions_h20_State(Double Progression = 0)
        {
            base.Set_h20_Progressions_h20_State(Progression);

            SetReady();
        }

        [Override]
        public override void Interact(out Double NewParam)
        {
            base.Interact(out var NewParam1);

            NewParam = FarmingProgress();
        }

        private Double FarmingProgress()
        {
            Tags.RemoveAt(1, 1);

            SwitchStageTokenSource = new CancellationTokenSource();

            OnSwitchStage();

            Progression_h20_State = (Int32)Math.Truncate(Progression_h20_State) >= Mesh_h20_List.Num() - 1
                ? 0
                : Math.Truncate(Progression_h20_State) + 1;

            return Collection_h20_Time;
        }

        private async void OnSwitchStage()
        {
            while (!SwitchStageTokenSource.IsCancellationRequested)
            {
                await Task.Delay((Int32)Collection_h20_Time * 1000);

                SwitchStageTokenSource.Cancel();

                SwitchStage();
            }
        }

        private void SwitchStage()
        {
            if (Math.Floor(Progression_h20_State) == 0)
            {
                SetReady();
            }
            else
            {
                SetReadyTokenSource = new CancellationTokenSource();

                OnSetReady();
            }
        }

        private async void OnSetReady()
        {
            while (!SetReadyTokenSource.IsCancellationRequested)
            {
                await Task.Delay((Int32)Collection_h20_Time * 1000);

                SetReadyTokenSource.Cancel();

                SetReady();
            }
        }

        private void SetReady()
        {
            var Tag = Mesh_h20_List.Num() - 1 == (Int32)Math.Floor(Progression_h20_State) ? "Harvest" : "Ready";

            if (Tags.Num() >= 2)
            {
                Tags[1] = Tag;
            }
            else
            {
                Tags.Add(Tag);
            }

            Mesh.SetStaticMesh(Mesh_h20_List[(Int32)Math.Truncate(Progression_h20_State)]);

            (UGameplayStatics.GetGameInstance(this) as IBPI_GI_C)?.Update_h20_All_h20_Interactables();

            PopFarmPlot();
        }

        private void PopFarmPlot()
        {
            Pop_h20_Plot.PlayFromStart();
        }

        [Override]
        public void Pop_h20_Plot__UpdateFunc()
        {
            var Plot = Pop_h20_Plot.TheTimeline.InterpFloats[0].FloatCurve
                .GetFloatValue(Pop_h20_Plot.TheTimeline.Position);

            Mesh.SetRelativeScale3D(new FVector { X = 1.0f, Y = 1.0f, Z = Plot });
        }

        private CancellationTokenSource SwitchStageTokenSource;

        private CancellationTokenSource SetReadyTokenSource;
    }
}