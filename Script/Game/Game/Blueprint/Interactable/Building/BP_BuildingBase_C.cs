using System;
using Script.CoreUObject;
using Script.Engine;
using Script.Game.Blueprint.Core.Save;

namespace Script.Game.Blueprint.Interactable.Building
{
    public partial class BP_BuildingBase_C
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
        public override void Interact(out double NewParam)
        {
            base.Interact(out var OutNewParam);

            NewParam = ProgressConstruct(0.4);
        }

        private double ProgressConstruct(double Invested_h20_Time)
        {
            /*
             * Update Current Build Progression
             */
            Progression_h20_State += Invested_h20_Time / Build_h20_Difficulty;

            /*
             * Is build complete?
             */
            if (Progression_h20_State >= Mesh_h20_List.Num() - 1)
            {
                /*
                 * If build is complete, trigger construction complete and set final mesh
                 */
                ConstructionComplete();

                Mesh.SetStaticMesh(Mesh_h20_List[Mesh_h20_List.Num() - 1]);

                (UGameplayStatics.GetGameInstance(this) as IBPI_GI_C)?.Update_h20_All_h20_Interactables();
            }
            else
            {
                /*
                 * If Build
                 */
                var State = (int)Math.Floor(Progression_h20_State);

                if (State > CurrentStage)
                {
                    CurrentStage = State;

                    if (Mesh_h20_List.IsValidIndex(CurrentStage))
                    {
                        Mesh.SetStaticMesh(Mesh_h20_List[CurrentStage]);

                        (UGameplayStatics.GetGameInstance(this) as IBPI_GI_C)?.Update_h20_All_h20_Interactables();
                    }
                }
            }

            return Mesh_h20_List.Num() - 1 - Progression_h20_State;
        }

        public virtual void SpawnBuildMode(double Progression = 0)
        {
            Progression_h20_State = Progression;

            Tags.Add("Build");

            var Index = (int)Math.Truncate(Progression_h20_State * Mesh_h20_List.Num());

            if (Mesh_h20_List.IsValidIndex(Index))
            {
                Mesh.SetStaticMesh(Mesh_h20_List[Index]);

                UKismetSystemLibrary.PrintString(this, Index.ToString());
            }
        }

        protected virtual void ConstructionComplete()
        {
            /*
             * Once the building is constructed we can remove the build tag
             */
            Tags.Remove("Build");

            (UGameplayStatics.GetGameInstance(this) as IBPI_GI_C)?.Update_h20_All_h20_Interactables();

            /*
             * We could also trigger any changes in logic now the build is complete.
             */
        }

        private int CurrentStage;
    }
}