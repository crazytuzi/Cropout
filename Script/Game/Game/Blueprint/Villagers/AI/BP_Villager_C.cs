using System;
using System.Threading;
using System.Threading.Tasks;
using Script.AIModule;
using Script.AnimGraphRuntime;
using Script.Common;
using Script.CoreUObject;
using Script.Engine;
using Script.Game.Blueprint.Core.GameMode;
using Script.Game.Blueprint.Core.Save;
using Script.Game.Blueprint.Interactable.Extras;
using Script.Game.Characters.Animations;
using Script.Game.Characters.Meshes.Hair;
using Script.Game.Characters.Meshes.Tools;
using Script.Library;

namespace Script.Game.Blueprint.Villagers
{
    [IsOverride]
    public partial class BP_Villager_C
    {
        [IsOverride]
        public override void ReceiveBeginPlay()
        {
            /*
             * Offset by capsule half height
             */
            var SweepHitResult = new FHitResult();

            K2_AddActorWorldOffset(new FVector { Z = Capsule.CapsuleHalfHeight }, false, ref SweepHitResult, false);

            /*
             * Start Eat timer
             */
            EatTokenSource = new CancellationTokenSource();

            OnEat();

            /*
             * Set Current Tag as Job
             */
            Change_h20_Job("Idle");

            /*
             * Set random hair & col
             */
            Hair.SetSkeletalMeshAsset(HairPick().LoadSynchronous());

            Hair.SetCustomPrimitiveDataFloat(0, (Single)UKismetMathLibrary.RandomFloat());
        }

        [IsOverride]
        public override void ReceiveEndPlay(EEndPlayReason EndPlayReason)
        {
            EatTokenSource?.Cancel();

            ChangeJobTokenSource?.Cancel();

            PlayVillagerAnimTokenSource?.Cancel();
        }

        /*
         * Return to the default behavior tree.
         */
        [IsOverride]
        public void Return_h20_To_h20_Default_h20_BT()
        {
            Change_h20_Job("Idle");
        }

        /*
         * This is where villagers are assigned new jobs.
         * The Event Action is sent from the player and passes along the target for the villager.
         * Read the Tag from the target and try to change jobs based on that tag name.
         */
        [IsOverride]
        public void Action(AActor NewParam)
        {
            if (NewParam != null)
            {
                Target_h20_Ref = NewParam;

                if (NewParam.Tags.IsValidIndex(0))
                {
                    Change_h20_Job(NewParam.Tags[0]);

                    (UGameplayStatics.GetGameInstance(this) as IBPI_GI_C)?.Update_h20_All_h20_Villagers();
                }
            }
        }

        /*
         * Change Job
         */
        [IsOverride]
        public void Change_h20_Job(FName New_h20_Job = null)
        {
            /*
             * Check the new tag against a data table of available jobs.
             */
            this.New_h20_Job = New_h20_Job;

            ChangeJobTokenSource = new CancellationTokenSource();

            OnChangeJob();
        }

        [IsOverride]
        public virtual void Play_h20_Work_h20_Anim(Double Delay = 0)
        {
            /*
             * We got this montage from the Job table and loaded it when the villager starts the job.
             * Length here is driven by the behavior tree that gets the data from the resource.
             * The monatage is looping and gets stoped when the delay is reached
             */
            PlayVillagerAnim(Work_h20_Anim, Delay);

            /*
             * Make the Tool visible
             */
            Tool.SetStaticMesh(Target_h20_Tool);

            Tool.SetVisibility(true);
        }

        /*
         * Helper function to play a montage.
         * The length can be set.
         * It's used for the looping work animations and for putting the box down where the delay is the length of the animation.
         */
        private void PlayVillagerAnim(UAnimMontage Montage = null, Double Length = 0)
        {
            var PlayMontageCallbackProxy =
                UPlayMontageCallbackProxy.CreateProxyObjectForPlayMontage(SkeletalMesh, Montage);

            PlayMontageCallbackProxy.OnCompleted.Add(this, OnMontagePlayCompleted);

            PlayMontageCallbackProxy.OnInterrupted.Add(this, OnMontagePlayInterrupted);

            PlayVillagerAnimTokenSource = new CancellationTokenSource();

            OnPlayVillagerAnim(Length);
        }

        [IsOverride]
        public void Add_h20_Resource(E_ResourceType Resource = E_ResourceType.None, Int32 Value = 0)
        {
            /*
             * Set currently held resource and value
             */
            ResourcesHeld = Resource;

            Quantity = Value;

            /*
             * Set Box to visible
             */
            Tool.SetVisibility(true);

            Tool.SetStaticMesh(Unreal.LoadObject<Crate>(this));
        }

        private void Eat()
        {
            (UGameplayStatics.GetGameMode(this) as IBPI_Resource_C)?.Remove_h20_Target_h20_Resource(E_ResourceType.Food,
                3);
        }

        private void ResetJobState()
        {
            StopJob();

            /*
             * Remove Hat
             */
            Hat.SetSkeletalMeshAsset(null);

            /*
             * Remove Tool
             */
            Tool.SetVisibility(false);

            Tool.SetStaticMesh(null);

            Target_h20_Tool = null;
        }

        private void StopJob()
        {
            Tool.SetVisibility(false);

            SkeletalMesh.GetAnimInstance()?.Montage_StopGroupByName(0.0f, "DefaultGroup");

            UAIBlueprintHelperLibrary.GetAIController(this)?.StopMovement();

            Quantity = 0;
        }

        private TSoftObjectPtr<USkeletalMesh> HairPick()
        {
            var Index = UKismetMathLibrary.RandomIntegerInRange(0, 5);

            if (Index == 0)
            {
                return new TSoftObjectPtr<USkeletalMesh>(Unreal.LoadObject<SKM_Hair01>(this));
            }
            else if (Index == 1)
            {
                return new TSoftObjectPtr<USkeletalMesh>(Unreal.LoadObject<SKM_Hair02>(this));
            }
            else if (Index == 2)
            {
                return new TSoftObjectPtr<USkeletalMesh>(Unreal.LoadObject<SKM_Hair03>(this));
            }
            else if (Index == 3)
            {
                return new TSoftObjectPtr<USkeletalMesh>(Unreal.LoadObject<SKM_Hair04>(this));
            }
            else if (Index == 4)
            {
                return new TSoftObjectPtr<USkeletalMesh>(Unreal.LoadObject<SKM_Hair05>(this));
            }
            else if (Index == 5)
            {
                return new TSoftObjectPtr<USkeletalMesh>(Unreal.LoadObject<SKM_Hair06>(this));
            }

            return null;
        }

        [IsOverride]
        public void Remove_h20_Resource(out E_ResourceType Target_h20_Resource, out Int32 Value)
        {
            /*
             * Store Resource type and value locally
             */
            var CacheResource = ResourcesHeld;

            var CacheValue = Quantity;

            /*
             * Clear resource and value
             */
            ResourcesHeld = E_ResourceType.None;

            Quantity = 0;

            /*
             * Hide Crate
             */
            Tool.SetVisibility(false);

            Tool.SetStaticMesh(null);

            /*
             * Send locally stored values
             */
            Target_h20_Resource = CacheResource;

            Value = CacheValue;
        }

        [IsOverride]
        public void Play_h20_Deliver_h20_Anim(out Double Delay)
        {
            PlayVillagerAnim(Unreal.LoadObject<AM_PutDown>(this), 1.0f);

            Delay = 1.0;
        }

        private async void OnEat()
        {
            while (!EatTokenSource.IsCancellationRequested)
            {
                await Task.Delay(24000);

                Eat();
            }
        }

        private async void OnChangeJob()
        {
            while (!ChangeJobTokenSource.IsCancellationRequested)
            {
                await Task.Delay(400);

                ChangeJobTokenSource.Cancel();

                if (UDataTableFunctionLibrary.GetDataTableRowFromName<ST_Job>(
                        Unreal.LoadObject<DT_Jobs>(this),
                        New_h20_Job,
                        out var OutRow))
                {
                    /*
                     * Using Tags makes it much easier to quickly find actors with set properties without having to explicitly cast.
                     */
                    if (!Tags.Contains(New_h20_Job))
                    {
                        Tags = new TArray<FName>
                        {
                            New_h20_Job
                        };

                        ResetJobState();

                        /*
                         * The data table only stores soft references, which avoids loading in every possible behavior tree, tool and hat.
                         * This means we can store every job type in a single graph and not worry about loading in content that won't be used on the map.
                         */

                        /*
                         * Async Load Behavior Tree
                         */
                        var AIController = UAIBlueprintHelperLibrary.GetAIController(this);

                        var BehaviourTree = OutRow.BehaviourTree.LoadSynchronous();

                        AIController.RunBehaviorTree(BehaviourTree);

                        Active_h20__h20_Behavior = BehaviourTree;

                        if (Target_h20_Ref != null)
                        {
                            var Blackboard = UAIBlueprintHelperLibrary.GetBlackboard(AIController);

                            Blackboard.SetValueAsObject("Target", Target_h20_Ref);
                        }

                        /*
                         * Load and Store Accessories
                         */
                        Work_h20_Anim = OutRow.WorkAnim.LoadSynchronous();

                        Hat.SetSkeletalMeshAsset(OutRow.Hat.LoadSynchronous());

                        Hat.SetVisibility(true);

                        Target_h20_Tool = OutRow.Tool.LoadSynchronous();
                    }
                }
                else
                {
                    UKismetSystemLibrary.PrintString(this, "ERROR: Failed to Load Job");
                }
            }
        }

        private async void OnPlayVillagerAnim(Double Length)
        {
            while (!PlayVillagerAnimTokenSource.IsCancellationRequested)
            {
                await Task.Delay((Int32)(Length * 1000));

                PlayVillagerAnimTokenSource.Cancel();

                SkeletalMesh.GetAnimInstance()?.Montage_StopGroupByName(0.0f, "DefaultGroup");
            }
        }

        void OnMontagePlayCompleted(FName NotifyName)
        {
            Tool.SetVisibility(false);
        }

        void OnMontagePlayInterrupted(FName NotifyName)
        {
            Tool.SetVisibility(false);
        }

        private CancellationTokenSource EatTokenSource;

        private CancellationTokenSource ChangeJobTokenSource;

        private CancellationTokenSource PlayVillagerAnimTokenSource;
    }
}