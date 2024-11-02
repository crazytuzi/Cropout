using System;
using System.Threading;
using System.Threading.Tasks;
using Script.CoreUObject;
using Script.Engine;
using Script.Game.Blueprint.Interactable.Extras;

namespace Script.Game.Blueprint.Interactable.Resources
{
    /*
     * BP_Resource houses the base information for any interactable that is classed as a resource.
     * While this class won't be used directly in game, blueprints that inherit from it will.
     * We place any functions or variables that will be used across all (or most) resources here so they can be easily shared.
     */
    [Override]
    public partial class BP_Resource_C
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

            TokenSource?.Cancel();
        }

        /*
         * As resources don't require construction, we will play a small animation scaling up the mesh on begin play.
         * This can be overriden by blueprints that inherit from it if needed.
         */
        [Override]
        public void Scale_h20_UP(double Delay = 0)
        {
            Mesh.SetHiddenInGame(true);

            TokenSource = new CancellationTokenSource();

            OnScaleUP(Delay);
        }

        [Override]
        public void Timeline_0_0__UpdateFunc()
        {
            var Scale = Timeline_0_0.TheTimeline.InterpFloats[0].FloatCurve
                .GetFloatValue(Timeline_0_0.TheTimeline.Position);

            Mesh.SetRelativeScale3D(new FVector(Scale));
        }

        [Override]
        public override void Interact(out double NewParam)
        {
            base.Interact(out var NewParam1);

            NewParam = Collection_h20_Time;
        }

        [Override]
        public void Remove_h20_Resource(out E_ResourceType Target_h20_Resource, out int Value)
        {
            End_h20_Wobble();

            if (Math.Abs(Resource_h20_Amount - -1.0f) > 0.000001)
            {
                Resource_h20_Amount = Math.Max(Resource_h20_Amount - CollectionValue, 0.0);

                if (Resource_h20_Amount <= 0.0)
                {
                    Death();
                }
            }

            Target_h20_Resource = Resource_h20_Type;

            Value = CollectionValue;
        }

        private async void OnScaleUP(double Delay)
        {
            while (!TokenSource.IsCancellationRequested)
            {
                await Task.Delay((int)(Delay * 1000));

                TokenSource.Cancel();

                Mesh.SetRelativeScale3D(new FVector());

                Mesh.SetHiddenInGame(false);

                Timeline_0_0.Play();
            }
        }

        /*
         * What to do when the resource is to be destroyed
         */
        private void Death()
        {
            K2_DestroyActor();
        }

        private CancellationTokenSource TokenSource;
    }
}