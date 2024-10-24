using System;
using System.Threading;
using System.Threading.Tasks;
using Script.CoreUObject;
using Script.Engine;
using Script.Game.Environment.Materials;

namespace Script.Game.Blueprint.Interactable
{
    /*
     * BP_Interactable is the base class for interactable, stationary actors in the world.
     * It acts as a container for basic information that all interactable items will have,
     * like their display name, spacing overide for the navmesh and meshes to use.
     * There is also Box collider, the sizing is set in construction script.
     */
    [Override]
    public partial class BP_Interactable_C
    {
        [Override]
        public override void ReceiveBeginPlay()
        {
            TokenSource = new CancellationTokenSource();

            OnNextTick();
        }

        [Override]
        public override void ReceiveEndPlay(EEndPlayReason EndPlayReason)
        {
            TokenSource?.Cancel();
        }

        [Override]
        public virtual void Set_h20_Progressions_h20_State(Double Progression = 0)
        {
            /*
             * Set progression state
             */
            Progression_h20_State = Progression;

            /*
             * If this interactable requires building before use, apply the Build tag and set starting mesh from mesh list
             */
            if (Require_h20_Build)
            {
                Tags.Add("Build");

                var Index = (Int32)Math.Floor(Progression_h20_State);

                if (Mesh_h20_List.IsValidIndex(Index))
                {
                    Mesh.SetStaticMesh(Mesh_h20_List[Index]);
                }
            }
        }

        [Override]
        public virtual void Placement_h20_Mode()
        {
            Enable_h20_Ground_h20_Blend = false;

            Mesh.SetStaticMesh(Mesh_h20_List[0]);

            Tags.Add("PlacementMode");
        }

        [Override]
        public virtual void Interact(out Double NewParam)
        {
            NewParam = 0.0;
        }

        [Override]
        public virtual void Timeline_0__UpdateFunc()
        {
            var Wobble = Timeline_0.TheTimeline.InterpFloats[0].FloatCurve
                .GetFloatValue(Timeline_0.TheTimeline.Position);

            Mesh.SetScalarParameterValueOnMaterials("Wobble", Wobble);
        }

        /*
         * Wobble Mesh
         */
        public void Play_h20_Wobble(FVector NewParam = null)
        {
            var Param = K2_GetActorLocation() - NewParam;

            Param.Normalize(0.0001);

            Mesh.SetVectorParameterValueOnMaterials(" Wobble Vector", Param);

            Timeline_0.Play();
        }

        /*
         * Wobble Mesh
         */
        public void End_h20_Wobble()
        {
            Timeline_0.Reverse();
        }

        private async void OnNextTick()
        {
            while (!TokenSource.IsCancellationRequested)
            {
                await Task.Delay(100);

                TokenSource.Cancel();

                /*
                 * Write position to world space RT, used to remove grass from area and paint dirt around asset.
                 */
                if (Enable_h20_Ground_h20_Blend)
                {
                    var Canvas = Unreal.NewObject<UCanvas>(this);

                    var Size = new FVector2D();

                    var Context = new FDrawToRenderTargetContext();

                    UKismetRenderingLibrary.BeginDrawCanvasToRenderTarget(
                        this,
                        RT_Draw,
                        ref Canvas,
                        ref Size,
                        ref Context);

                    TransformToTexture(Size, out var ReturnValue, out var ReturnValue2);

                    Canvas.K2_DrawMaterial(
                        Unreal.LoadObject<M_ShapeDraw>(this),
                        ReturnValue,
                        ReturnValue2,
                        new FVector2D(),
                        new FVector2D(1.0, 1.0),
                        0.0f,
                        new FVector2D(0.5, 0.5)
                    );

                    UKismetRenderingLibrary.EndDrawCanvasToRenderTarget(this, Context);
                }

                /*
                 * Remove if exactly overlapping another interactable.
                 * This will not be needed once Spawner is replaced by PCG.
                 */
                var OverlappingActors = new TArray<AActor>();

                GetOverlappingActors(ref OverlappingActors, StaticClass());

                foreach (var OverlappingActor in OverlappingActors)
                {
                    var bEqual = OverlappingActor.K2_GetActorLocation().Equals(K2_GetActorLocation(), 5.0);

                    var bHasTag = ActorHasTag("PlacementMode");

                    if (bEqual && !bHasTag)
                    {
                        K2_DestroyActor();

                        break;
                    }
                }
            }
        }

        private void TransformToTexture(FVector2D InVec, out FVector2D ReturnValue, out FVector2D ReturnValue2)
        {
            var Origin = new FVector();

            var BoxExtent = new FVector();

            GetActorBounds(false, ref Origin, ref BoxExtent, false);

            var Min = Math.Min(BoxExtent.X, BoxExtent.Y);

            var Outline = Min / 10000.0 * InVec.X * OutlineDraw;

            ReturnValue2 = new FVector2D(Outline);

            ReturnValue = new FVector2D((K2_GetActorLocation()
                                         + new FVector(10000.0))
                                        / new FVector(20000.0)
                                        * new FVector(InVec.X)
                                        - new FVector(Outline / 2.0));
        }

        private CancellationTokenSource TokenSource;
    }
}