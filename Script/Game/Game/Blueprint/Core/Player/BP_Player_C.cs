using System;
using System.Threading;
using System.Threading.Tasks;
using Script.Common;
using Script.CoreUObject;
using Script.Engine;
using Script.EnhancedInput;
using Script.Game.Blueprint.Core.Extras;
using Script.Game.Blueprint.Core.GameMode;
using Script.Game.Blueprint.Core.Player.Input;
using Script.Game.Blueprint.Core.Save;
using Script.Game.Blueprint.Interactable;
using Script.Game.Blueprint.Interactable.Extras;
using Script.Game.Blueprint.Villagers;
using Script.Game.VFX;
using Script.InputCore;
using Script.Library;
using Script.NavigationSystem;
using Script.Niagara;

namespace Script.Game.Blueprint.Core.Player
{
    [IsOverride]
    public partial class BP_Player_C
    {
        /*
         * On Begin Play start bound check
         */
        [IsOverride]
        public override void ReceiveBeginPlay()
        {
            UpdateZoom();

            MovTrackingTokenSource = new CancellationTokenSource();

            OnMovTracking();

            var PlayerController = GetPlayerController();

            var EnhancedInputLocalPlayerSubsystem = USubsystemBlueprintLibrary.GetLocalPlayerSubsystem(
                PlayerController,
                UEnhancedInputLocalPlayerSubsystem.StaticClass()
            ) as UEnhancedInputLocalPlayerSubsystem;

            EnhancedInputLocalPlayerSubsystem.AddMappingContext(Unreal.LoadObject<IMC_BaseInput>(this), 0);

            EnhancedInputLocalPlayerSubsystem.AddMappingContext(Unreal.LoadObject<IMC_Villager_Mode>(this), 0);

            var EnhancedInputComponent = InputComponent as UEnhancedInputComponent;

            /*
             * Build Mode
             */
            EnhancedInputComponent.BindAction<IA_Build_Move>(ETriggerEvent.Triggered, this, IABuildMoveTriggered);

            EnhancedInputComponent.BindAction<IA_Build_Move>(ETriggerEvent.Completed, this, IABuildMoveCompleted);

            /*
             * Villager Mode
             */
            EnhancedInputComponent.BindAction<IA_Villager>(ETriggerEvent.Triggered, this, IAVillagerTriggered);

            EnhancedInputComponent.BindAction<IA_Villager>(ETriggerEvent.Started, this, IAVillagerStarted);

            EnhancedInputComponent.BindAction<IA_Villager>(ETriggerEvent.Canceled, this, IAVillagerCanceled);

            EnhancedInputComponent.BindAction<IA_Villager>(ETriggerEvent.Completed, this, IAVillagerCompleted);

            /*
             * You can split off groups of logic into multiple Graphs. This helps keep blueprints more contained and easier to find.
             */
            EnhancedInputComponent.BindAction<IA_Move>(ETriggerEvent.Triggered, this, IAMoveTriggered);

            EnhancedInputComponent.BindAction<IA_Spin>(ETriggerEvent.Triggered, this, IASpinTriggered);

            EnhancedInputComponent.BindAction<IA_Zoom>(ETriggerEvent.Triggered, this, IAZoomTriggered);

            /*
             * Contextual Movement
             */
            EnhancedInputComponent.BindAction<IA_DragMove>(ETriggerEvent.Triggered, this, IADragMoveTriggered);
        }

        [IsOverride]
        public override void ReceiveEndPlay(EEndPlayReason EndPlayReason)
        {
            MovTrackingTokenSource?.Cancel();

            ClosestHoverCheckTokenSource?.Cancel();

            ClearHoverActorTokenSource?.Cancel();

            UpdatePathTokenSource?.Cancel();
        }

        /*
         * Create Spawnable
         */
        [IsOverride]
        public virtual void BeginBuild(TSubclassOf<BP_Interactable_C> Target_h20_Class = null,
            TMap<E_ResourceType, Int32> Resource_h20_Cost = null)
        {
            Target_h20_Spawn_h20_Class = Target_h20_Class;

            this.Resource_h20_Cost = Resource_h20_Cost;

            Spawn?.K2_DestroyActor();

            Spawn = GetWorld().SpawnActor<BP_Interactable_C>(
                Target_h20_Spawn_h20_Class.Get(),
                new FTransform
                {
                    Translation = K2_GetActorLocation(),
                    Scale3D = new FVector(1.0)
                });

            Spawn.Placement_h20_Mode();

            CreateBuildOverlay();
        }

        /*
         * Bind 'Input Switch' event from player controller to pawn function.
         * Set mapping context
         */
        [IsOverride]
        public override void ReceivePossessed(AController NewController)
        {
            (NewController as BP_PC_C)?.KeySwitch.Add(this, InputSwitch);
        }

        /*
         * When hovering over an actor, check if that actor is the closest available actor in player collision.
         */
        [IsOverride]
        public override void ReceiveActorBeginOverlap(AActor OtherActor)
        {
            if (HoverActor == null)
            {
                HoverActor = OtherActor;

                ClosestHoverCheckTokenSource = new CancellationTokenSource();

                OnClosestHoverCheck();
            }
        }

        [IsOverride]
        public override void ReceiveActorEndOverlap(AActor OtherActor)
        {
            var OverlappingActors = new TArray<AActor>();

            GetOverlappingActors(ref OverlappingActors);

            if (OverlappingActors.Num() == 0)
            {
                ClearHoverActorTokenSource = new CancellationTokenSource();

                OnClearHoverActor();
            }
        }

        private async void OnMovTracking()
        {
            while (!MovTrackingTokenSource.IsCancellationRequested)
            {
                await Task.Delay(16);

                MoveTracking();
            }
        }

        private async void OnClosestHoverCheck()
        {
            while (!ClosestHoverCheckTokenSource.IsCancellationRequested)
            {
                await Task.Delay(10);

                ClosestHoverCheck();
            }
        }

        private async void OnClearHoverActor()
        {
            while (!ClearHoverActorTokenSource.IsCancellationRequested)
            {
                await Task.Delay(300);

                ClearHoverActorTokenSource.Cancel();

                HoverActor = null;
            }
        }

        private async void OnUpdatePath()
        {
            while (!UpdatePathTokenSource.IsCancellationRequested)
            {
                await Task.Delay(10);

                UpdatePath();
            }
        }

        [IsOverride]
        private void IABuildMoveTriggered(FInputActionValue ActionValue, Single ElapsedTime, Single TriggeredTime,
            UInputAction SourceAction)
        {
            UpdateBuildAsset();
        }

        [IsOverride]
        private void IABuildMoveCompleted(FInputActionValue ActionValue, Single ElapsedTime, Single TriggeredTime,
            UInputAction SourceAction)
        {
            /*
             * Stop Update on Spawn
             */
            BPF_Cropout_C.SteppedPosition(Spawn.K2_GetActorLocation(), this, out var NewParam1);

            var SweepHitResult = new FHitResult();

            K2_SetActorLocation(NewParam1, false, ref SweepHitResult, false);
        }

        [IsOverride]
        private void IAVillagerTriggered(FInputActionValue ActionValue, Single ElapsedTime, Single TriggeredTime,
            UInputAction SourceAction)
        {
            Villager_h20_Action = HoverActor;
        }

        [IsOverride]
        private void IAVillagerStarted(FInputActionValue ActionValue, Single ElapsedTime, Single TriggeredTime,
            UInputAction SourceAction)
        {
            /*
             * Check if multiple touch points and store current position if not.
             */
            if (SingleTouchCheck())
            {
                PositionCheck();

                /*
                 * Check if villager is selected
                 */
                var bIsVillagerOverlap = VillagerOverlapCheck(out var Villager);

                if (bIsVillagerOverlap)
                {
                    VillagerSelect(Villager);
                }
                else
                {
                    var PlayerController = GetPlayerController();

                    var EnhancedInputLocalPlayerSubsystem = USubsystemBlueprintLibrary.GetLocalPlayerSubsystem(
                        PlayerController,
                        UEnhancedInputLocalPlayerSubsystem.StaticClass()
                    ) as UEnhancedInputLocalPlayerSubsystem;

                    EnhancedInputLocalPlayerSubsystem.AddMappingContext(Unreal.LoadObject<IMC_DragMove>(this), 0);
                }
            }
        }

        [IsOverride]
        private void IAVillagerCanceled(FInputActionValue ActionValue, Single ElapsedTime, Single TriggeredTime,
            UInputAction SourceAction)
        {
            /*
             * Remove Mapping Context
             */
            var PlayerController = GetPlayerController();

            var EnhancedInputLocalPlayerSubsystem = USubsystemBlueprintLibrary.GetLocalPlayerSubsystem(
                PlayerController,
                UEnhancedInputLocalPlayerSubsystem.StaticClass()
            ) as UEnhancedInputLocalPlayerSubsystem;

            EnhancedInputLocalPlayerSubsystem.RemoveMappingContext(
                Unreal.LoadObject<IMC_DragMove>(this),
                new FModifyContextOptions
                {
                    bIgnoreAllPressedKeysUntilRelease = true,
                    bForceImmediately = true,
                    bNotifyUserSettings = false
                });

            /*
             * Send overlapping actor to villager and trigger action
             */
            if (Selected != null)
            {
                (Selected as IBPI_Villager_C)?.Action(Villager_h20_Action);

                VillagerRelease();
            }

            Villager_h20_Action = null;
        }

        [IsOverride]
        private void IAVillagerCompleted(FInputActionValue ActionValue, Single ElapsedTime, Single TriggeredTime,
            UInputAction SourceAction)
        {
            /*
             * Remove Mapping Context
             */
            var PlayerController = GetPlayerController();

            var EnhancedInputLocalPlayerSubsystem = USubsystemBlueprintLibrary.GetLocalPlayerSubsystem(
                PlayerController,
                UEnhancedInputLocalPlayerSubsystem.StaticClass()
            ) as UEnhancedInputLocalPlayerSubsystem;

            EnhancedInputLocalPlayerSubsystem.RemoveMappingContext(
                Unreal.LoadObject<IMC_DragMove>(this),
                new FModifyContextOptions
                {
                    bIgnoreAllPressedKeysUntilRelease = true,
                    bForceImmediately = true,
                    bNotifyUserSettings = false
                });

            /*
             * Send overlapping actor to villager and trigger action
             */
            if (Selected != null)
            {
                (Selected as IBPI_Villager_C)?.Action(Villager_h20_Action);

                VillagerRelease();
            }

            Villager_h20_Action = null;
        }

        /*
         * Movement and Rotation using WASD,QE or Controller
         */
        [IsOverride]
        private void IAMoveTriggered(FInputActionValue ActionValue, Single ElapsedTime, Single TriggeredTime,
            UInputAction SourceAction)
        {
            var Vector2D = UEnhancedInputLibrary.Conv_InputActionValueToAxis2D(ActionValue);

            AddMovementInput(GetActorForwardVector(), (Single)Vector2D.Y);

            AddMovementInput(GetActorRightVector(), (Single)Vector2D.X);
        }

        [IsOverride]
        private void IASpinTriggered(FInputActionValue ActionValue, Single ElapsedTime, Single TriggeredTime,
            UInputAction SourceAction)
        {
            var SweepHitResult = new FHitResult();

            K2_AddActorLocalRotation(new FRotator
                {
                    Roll = 0.0,
                    Pitch = 0.0,
                    Yaw = UEnhancedInputLibrary.Conv_InputActionValueToAxis1D(ActionValue)
                },
                false,
                ref SweepHitResult,
                false);
        }

        /*
         * Zoom (Also effects player move speed)
         */
        [IsOverride]
        private void IAZoomTriggered(FInputActionValue ActionValue, Single ElapsedTime, Single TriggeredTime,
            UInputAction SourceAction)
        {
            ZoomDirection = UEnhancedInputLibrary.Conv_InputActionValueToAxis1D(ActionValue);

            UpdateZoom();

            Dof();
        }

        /*
         * Drag move
         */
        [IsOverride]
        private void IADragMoveTriggered(FInputActionValue ActionValue, Single ElapsedTime, Single TriggeredTime,
            UInputAction SourceAction)
        {
            if (SingleTouchCheck())
            {
                TrackMove();
            }
            else
            {
                var PlayerController = GetPlayerController();

                var EnhancedInputLocalPlayerSubsystem = USubsystemBlueprintLibrary.GetLocalPlayerSubsystem(
                    PlayerController,
                    UEnhancedInputLocalPlayerSubsystem.StaticClass()
                ) as UEnhancedInputLocalPlayerSubsystem;

                EnhancedInputLocalPlayerSubsystem.RemoveMappingContext(
                    Unreal.LoadObject<IMC_DragMove>(this),
                    new FModifyContextOptions
                    {
                        bIgnoreAllPressedKeysUntilRelease = true,
                        bForceImmediately = true,
                        bNotifyUserSettings = false
                    });
            }
        }

        /*
         * Set Target to Hover Actor if valid, otherwise use Collision component.
         */
        private void UpdateCursorPosition()
        {
            if (InputType == E_InputType.KeyMouse || InputType == E_InputType.Gamepad)
            {
                FTransform TargetTransform;

                if (HoverActor != null)
                {
                    var Origin = new FVector();

                    var BoxExtent = new FVector();

                    HoverActor.GetActorBounds(true, ref Origin, ref BoxExtent, false);

                    /*
                     * Scale Indicator up and down
                     */
                    var Scale = new FVector2D(BoxExtent.X, BoxExtent.Y).GetAbsMax() / 50.0 +
                                UKismetMathLibrary.Sin(UGameplayStatics.GetWorldDeltaSeconds(this) * 5.0) * 0.25 +
                                1.0;

                    TargetTransform = new FTransform
                    {
                        Translation = new FVector
                        {
                            X = Origin.X,
                            Y = Origin.Y,
                            Z = 20.0
                        },
                        Rotation = new FQuat(),
                        Scale3D = new FVector
                        {
                            X = Scale,
                            Y = Scale,
                            Z = 1.0
                        }
                    };
                }
                else
                {
                    var WorldTransform = Collision.K2_GetComponentToWorld();

                    TargetTransform = new FTransform
                    {
                        Translation = WorldTransform.Translation,
                        Rotation = WorldTransform.Rotation,
                        Scale3D = new FVector
                        {
                            X = 2.0,
                            Y = 2.0,
                            Z = 1.0
                        }
                    };
                }

                /*
                 * Interp Cursor Position to target
                 */
                var SweepHitResult = new FHitResult();

                Cursor.K2_SetWorldTransform(UKismetMathLibrary.TInterpTo(
                        Cursor.K2_GetComponentToWorld(),
                        TargetTransform,
                        (Single)UGameplayStatics.GetWorldDeltaSeconds(this),
                        12.0f
                    ),
                    false,
                    ref SweepHitResult,
                    false);
            }
        }

        private void TrackMove()
        {
            /*
             * Include offset between camera and spring arm end if using lag. Alternatively, disable camera lag while moving
             */
            var Offset = new FVector
                         {
                             X = SpringArm.GetForwardVector().X *
                                 (SpringArm.TargetArmLength - SpringArm.SocketOffset.X) * -1.0,
                             Y = SpringArm.GetForwardVector().Y *
                                 (SpringArm.TargetArmLength - SpringArm.SocketOffset.X) * -1.0,
                             Z = SpringArm.GetForwardVector().Z *
                                 (SpringArm.TargetArmLength - SpringArm.SocketOffset.X) * -1.0
                         } +
                         new FVector
                         {
                             X = SpringArm.GetUpVector().X * SpringArm.SocketOffset.Z,
                             Y = SpringArm.GetUpVector().Y * SpringArm.SocketOffset.Z,
                             Z = SpringArm.GetUpVector().Z * SpringArm.SocketOffset.Z,
                         } +
                         SpringArm.K2_GetComponentLocation() -
                         Camera.K2_GetComponentLocation();

            if (ProjectMouseOrTouch1ToGroundPlane(out var ScreenPos, out var Intersection))
            {
                Stored_h20_Move = TargetHandle - Intersection - Offset;

                var SweepHitResult = new FHitResult();

                K2_AddActorWorldOffset(new FVector { X = Stored_h20_Move.X, Y = Stored_h20_Move.Y }, false,
                    ref SweepHitResult, false);
            }
        }

        private void UpdateZoom()
        {
            ZoomValue = (Single)UKismetMathLibrary.FClamp(ZoomDirection * 0.01 + ZoomValue, 0.0, 1.0);

            var CurveValue = ZoomCurve.GetFloatValue(ZoomValue);

            /*
             * B originally = 8000
             */
            SpringArm.TargetArmLength = (Single)UKismetMathLibrary.Lerp(800.0, 40000.0, CurveValue);

            var SweepHitResult = new FHitResult();

            SpringArm.K2_SetRelativeRotation(new FRotator { Pitch = UKismetMathLibrary.Lerp(-40.0, -55.0, CurveValue) },
                false, ref SweepHitResult, false);

            FloatingPawnMovement.MaxSpeed = (Single)UKismetMathLibrary.Lerp(1000.0, 6000.0, CurveValue);

            Dof();

            Camera.SetFieldOfView((Single)UKismetMathLibrary.Lerp(20.0, 15.0, CurveValue));
        }

        private void MoveTracking()
        {
            /*
             * Keep player within playspace
             */
            var ActorLocation = K2_GetActorLocation();

            ActorLocation.Normalize(0.0001);

            AddMovementInput(new FVector
                (
                    ActorLocation.X * -1.0,
                    ActorLocation.Y * -1.0,
                    0.0
                ),
                (Single)UKismetMathLibrary.FMax((K2_GetActorLocation().Length() - 9000.0) / 5000.0, 0.0));

            /*
             * Syncs 3D Cursor and Collision Position
             */
            UpdateCursorPosition();

            /*
             * Edge Of Screen Movement
             */
            EdgeMove(out var Direction, out var Strength);

            AddMovementInput(Direction, (Single)Strength);

            /*
             * Position Collision On Ground Plane Projection
             */
            ProjectMouseOrTouch1ToGroundPlane(out var ScreenPos, out var Intersection);

            var SweepHitResult = new FHitResult();

            Collision.K2_SetWorldLocation(Intersection + new FVector { Z = 10.0 }, false, ref SweepHitResult, false);
        }

        /*
         * Update Target Spawn Position
         */
        private void UpdateBuildAsset()
        {
            if (ProjectMouseOrTouch1ToGroundPlane(out var ScreenPos, out var Intersection))
            {
                if (Spawn != null)
                {
                    BPF_Shared_C.ConvertToSteppedPos(Intersection, this, out var NewParam);

                    var InterpVector = UKismetMathLibrary.VInterpTo(Spawn.K2_GetActorLocation(), NewParam,
                        (Single)UGameplayStatics.GetWorldDeltaSeconds(this), 10.0f);

                    var SweepHitResult = new FHitResult();

                    K2_SetActorLocation(InterpVector, false, ref SweepHitResult, false);

                    /*
                     * If not overlapping anything and within bounds, asset can be spawned
                     */
                    var OverlappingActors = new TArray<AActor>();

                    Spawn.GetOverlappingActors(ref OverlappingActors, BP_Interactable_C.StaticClass());

                    if (OverlappingActors.Num() == 0)
                    {
                        Can_h20_Drop = CornersInNav();
                    }
                    else
                    {
                        Can_h20_Drop = false;
                    }

                    UKismetMaterialLibrary.SetVectorParameterValue(
                        this,
                        Unreal.LoadObject<MPC_Cropout>(this),
                        "Target Position",
                        new FLinearColor
                        {
                            R = (Single)InterpVector.X,
                            G = (Single)InterpVector.Y,
                            B = (Single)InterpVector.Z,
                            A = Can_h20_Drop ? 1.0f : 0.0f
                        });
                }
            }
        }

        private Boolean CornersInNav()
        {
            var Origin = new FVector();

            var BoxExtent = new FVector();

            var SphereRadius = 0.0f;

            UKismetSystemLibrary.GetComponentBounds(Spawn.Box, ref Origin, ref BoxExtent, ref SphereRadius);

            var Box1 = Origin + new FVector(BoxExtent.X * 1.05, BoxExtent.Y * 1.05, 0.0);

            var ActorsToIgnore = new TArray<AActor>
            {
                Spawn
            };

            var OutHits1 = new TArray<FHitResult>();

            if (UKismetSystemLibrary.LineTraceMulti(
                    this,
                    new FVector(Box1.X, Box1.Y, 100.0),
                    new FVector(Box1.X, Box1.Y, -1.0),
                    (ETraceTypeQuery)ECollisionChannel.Visibility,
                    false,
                    ActorsToIgnore,
                    EDrawDebugTrace.None,
                    ref OutHits1,
                    true,
                    FLinearColor.Red,
                    FLinearColor.Green,
                    5.0f
                ))
            {
                var Box2 = Origin + new FVector(-BoxExtent.X * 1.05, BoxExtent.Y * 1.05, 0.0);

                var OutHits2 = new TArray<FHitResult>();

                if (UKismetSystemLibrary.LineTraceMulti(
                        this,
                        new FVector(Box2.X, Box2.Y, 100.0),
                        new FVector(Box2.X, Box2.Y, -1.0),
                        (ETraceTypeQuery)ECollisionChannel.Visibility,
                        false,
                        ActorsToIgnore,
                        EDrawDebugTrace.None,
                        ref OutHits2,
                        true,
                        FLinearColor.Red,
                        FLinearColor.Green,
                        5.0f
                    ))
                {
                    var Box3 = Origin + new FVector(BoxExtent.X * 1.05, -BoxExtent.Y * 1.05, 0.0);

                    var OutHits3 = new TArray<FHitResult>();

                    if (UKismetSystemLibrary.LineTraceMulti(
                            this,
                            new FVector(Box3.X, Box3.Y, 100.0),
                            new FVector(Box3.X, Box3.Y, -1.0),
                            (ETraceTypeQuery)ECollisionChannel.Visibility,
                            false,
                            ActorsToIgnore,
                            EDrawDebugTrace.None,
                            ref OutHits3,
                            true,
                            FLinearColor.Red,
                            FLinearColor.Green,
                            5.0f
                        ))
                    {
                        var Box4 = Origin + new FVector(-BoxExtent.X * 1.05, -BoxExtent.Y * 1.05, 0.0);

                        var OutHits4 = new TArray<FHitResult>();

                        if (UKismetSystemLibrary.LineTraceMulti(
                                this,
                                new FVector(Box4.X, Box4.Y, 100.0),
                                new FVector(Box4.X, Box4.Y, -1.0),
                                (ETraceTypeQuery)ECollisionChannel.Visibility,
                                false,
                                ActorsToIgnore,
                                EDrawDebugTrace.None,
                                ref OutHits4,
                                true,
                                FLinearColor.Red,
                                FLinearColor.Green,
                                5.0f
                            ))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        private void CreateBuildOverlay()
        {
            if (SpawnOverlay == null)
            {
                var Origin = new FVector();

                var BoxExtent = new FVector();

                Spawn.GetActorBounds(false, ref Origin, ref BoxExtent, false);

                SpawnOverlay = AddComponentByClass(
                    UStaticMeshComponent.StaticClass(),
                    true,
                    new FTransform
                    {
                        Scale3D = BoxExtent / new FVector(50.0)
                    },
                    false
                ) as UStaticMeshComponent;

                SpawnOverlay.K2_AttachToComponent(
                    Spawn.Mesh,
                    "None",
                    EAttachmentRule.SnapToTarget,
                    EAttachmentRule.KeepWorld,
                    EAttachmentRule.KeepWorld,
                    true);

                UpdateBuildAsset();
            }
        }

        public void DestroySpawn()
        {
            Spawn.K2_DestroyActor();

            SpawnOverlay.K2_DestroyComponent(SpawnOverlay);
        }

        public void RotateSpawn()
        {
            Spawn.K2_SetActorRotation(
                UKismetMathLibrary.ComposeRotators(Spawn.K2_GetActorRotation(), new FRotator { Yaw = 90.0 }),
                false);
        }

        /*
         * Remove Resources
         */
        private void RemoveResources()
        {
            var BPI_Resource_C = UGameplayStatics.GetGameMode(this) as IBPI_Resource_C;

            foreach (var Resource in Resource_h20_Cost)
            {
                BPI_Resource_C.Remove_h20_Target_h20_Resource(Resource.Key, Resource.Value);
            }

            /*
             * Check if we have enough resources to place again
             */
            BPI_Resource_C.Get_h20_Current_h20_Resources(out var Resources);

            foreach (var Resource in Resource_h20_Cost)
            {
                if (Resource.Value > Resources[Resource.Key])
                {
                    (UGameplayStatics.GetGameMode(this) as IBPI_Player_C)?.Remove_h20_Current_h20_UI_h20_Layer();

                    DestroySpawn();

                    break;
                }
            }
        }

        public void SpawnBuildTarget()
        {
            if (Can_h20_Drop)
            {
                var BP_Interactable = GetWorld()
                    .SpawnActor<BP_Interactable_C>(Target_h20_Spawn_h20_Class.Get(), Spawn.GetTransform());

                BP_Interactable.Progression_h20_State = 0.0;

                BP_Interactable.Set_h20_Progressions_h20_State();

                RemoveResources();

                (UGameplayStatics.GetGameInstance(this) as IBPI_GI_C)?.Update_h20_All_h20_Interactables();

                UpdateBuildAsset();
            }
        }

        private void UpdatePath()
        {
            if (Selected != null)
            {
                var Path = UNavigationSystemV1.FindPathToLocationSynchronously(
                    this,
                    Collision.K2_GetComponentLocation(),
                    Selected.K2_GetActorLocation());

                if (Path.PathPoints.Num() > 0)
                {
                    Path_h20_Points = Path.PathPoints;

                    Path_h20_Points[0] = Collision.K2_GetComponentLocation();

                    Path_h20_Points[Path_h20_Points.Num() - 1] = Selected.K2_GetActorLocation();

                    UNiagaraDataInterfaceArrayFunctionLibrary.SetNiagaraArrayVector(NS_Path, "TargetPath",
                        Path_h20_Points);
                }
            }
        }

        private void VillagerSelect(AActor Selected)
        {
            this.Selected = Selected;

            NS_Path = UNiagaraFunctionLibrary.SpawnSystemAttached(
                Unreal.LoadObject<NS_Target>(this),
                DefaultSceneRoot,
                "None",
                new FVector(),
                new FRotator(),
                EAttachLocation.SnapToTarget,
                false);

            UpdatePathTokenSource = new CancellationTokenSource();

            OnUpdatePath();
        }

        private void VillagerRelease()
        {
            UpdatePathTokenSource?.Cancel();

            NS_Path.K2_DestroyComponent(this);

            Selected = null;
        }

        private void Dof()
        {
            Camera.PostProcessSettings = new FPostProcessSettings
            {
                DepthOfFieldFstop = 3.0f,
                DepthOfFieldSensorWidth = 150.0f,
                DepthOfFieldFocalDistance = SpringArm.TargetArmLength
            };
        }

        private void ClosestHoverCheck()
        {
            /*
             * End Hover Check if not overlapping anything
             */
            var OverlappingActors = new TArray<AActor>();

            Collision.GetOverlappingActors(ref OverlappingActors, AActor.StaticClass());

            if (OverlappingActors.Num() > 0)
            {
                /*
                 * Check distance of all overlapping actors and find closest one
                 */
                var Hover = OverlappingActors[0];

                for (var i = 1; i < OverlappingActors.Num(); i++)
                {
                    if (UKismetMathLibrary.Vector_Distance(OverlappingActors[i].K2_GetActorLocation(),
                            Collision.K2_GetComponentLocation()) <
                        UKismetMathLibrary.Vector_Distance(Collision.K2_GetComponentLocation(),
                            Hover.K2_GetActorLocation()))
                    {
                        Hover = OverlappingActors[i];
                    }
                }

                /*
                 * If closest overlapping actor is the currently hovered asset, do nothing.
                 */
                if (HoverActor != Hover)
                {
                    HoverActor = Hover;
                }
            }
            else
            {
                ClosestHoverCheckTokenSource?.Cancel();
            }
        }

        private void PositionCheck()
        {
            ProjectMouseOrTouch1ToGroundPlane(out var ScreenPos, out var Intersection);

            TargetHandle = Intersection;

            if (InputType == E_InputType.Touch)
            {
                var SweepHitResult = new FHitResult();

                Collision.K2_SetWorldLocation(TargetHandle, false, ref SweepHitResult, false);
            }
        }

        private APlayerController GetPlayerController()
        {
            return GetController() as APlayerController;
        }

        [IsOverride]
        public virtual void Switch_h20_Build_h20_Mode(Boolean Switch_h20_To_h20_Build_h20_Mode_h3F_ = false)
        {
            var PlayerController = GetPlayerController();

            var EnhancedInputLocalPlayerSubsystem = USubsystemBlueprintLibrary.GetLocalPlayerSubsystem(
                PlayerController,
                UEnhancedInputLocalPlayerSubsystem.StaticClass()
            ) as UEnhancedInputLocalPlayerSubsystem;

            if (Switch_h20_To_h20_Build_h20_Mode_h3F_)
            {
                EnhancedInputLocalPlayerSubsystem.RemoveMappingContext(Unreal.LoadObject<IMC_Villager_Mode>(this));

                EnhancedInputLocalPlayerSubsystem.AddMappingContext(Unreal.LoadObject<IMC_BuildMode>(this), 0);
            }
            else
            {
                EnhancedInputLocalPlayerSubsystem.RemoveMappingContext(Unreal.LoadObject<IMC_BuildMode>(this));

                EnhancedInputLocalPlayerSubsystem.AddMappingContext(Unreal.LoadObject<IMC_Villager_Mode>(this), 0);
            }
        }

        private Boolean SingleTouchCheck()
        {
            var LocationX = 0.0f;

            var LocationY = 0.0f;

            var bIsCurrentlyPressed = false;

            var PlayerController = GetPlayerController();

            PlayerController.GetInputTouchState(ETouchIndex.Touch2, ref LocationX, ref LocationY,
                ref bIsCurrentlyPressed);

            return !bIsCurrentlyPressed;
        }

        private Boolean VillagerOverlapCheck(out AActor Villager)
        {
            var OverlappingActors = new TArray<AActor>();

            GetOverlappingActors(ref OverlappingActors, APawn.StaticClass());

            if (OverlappingActors.Num() > 0)
            {
                Villager = OverlappingActors[0];

                return true;
            }

            Villager = null;

            return false;
        }

        private void InputSwitch(E_InputType NewInputType = E_InputType.Unknown)
        {
            InputType = NewInputType;

            /*
             * Get new Input Type
             */
            if (NewInputType == E_InputType.KeyMouse)
            {
                Cursor.SetHiddenInGame(false, true);
            }
            else if (NewInputType == E_InputType.Gamepad)
            {
                /*
                 * If Gamepad, reset Cursor Position, hide mouse and refocus to Game viewport
                 */
                var SweepHitResult = new FHitResult();

                Collision.K2_SetRelativeLocation(new FVector { Z = 10.0 }, false, ref SweepHitResult, false);

                Cursor.SetHiddenInGame(false, true);
            }
            else if (NewInputType == E_InputType.Touch)
            {
                /*
                 * If Touch, we only want to use the cursor when it's being pressed
                 */
                Cursor.SetHiddenInGame(true, true);

                var SweepHitResult = new FHitResult();

                Collision.K2_SetRelativeLocation(new FVector { Z = -500.0 }, false, ref SweepHitResult, false);
            }
        }

        private Boolean ProjectMouseOrTouch1ToGroundPlane(out FVector2D ScreenPos, out FVector Intersection)
        {
            var PlayerController = GetPlayerController();

            /*
             * Lock Projection to center screen
             */
            var SizeX = 0;

            var SizeY = 0;

            PlayerController.GetViewportSize(ref SizeX, ref SizeY);

            var LockProjection = new FVector2D(SizeX, SizeY) / new FVector2D(2.0);

            /*
             * Get Mouse Viewport position
             */
            var MouseLocationX = 0.0f;

            var MouseLocationY = 0.0f;

            var bAssociatedMouseDevice = PlayerController.GetMousePosition(ref MouseLocationX, ref MouseLocationY);

            /*
             * Get Touch if touch event detected
             */
            var TouchLocationX = 0.0f;

            var TouchLocationY = 0.0f;

            var bIsCurrentlyPressed = false;

            PlayerController.GetInputTouchState(ETouchIndex.Touch1, ref TouchLocationX, ref TouchLocationY,
                ref bIsCurrentlyPressed);

            /*
             * Select based on input
             */
            FVector2D ScreenPosition;

            if (InputType == E_InputType.Unknown)
            {
                ScreenPosition = LockProjection;
            }
            else if (InputType == E_InputType.KeyMouse)
            {
                ScreenPosition = bAssociatedMouseDevice
                    ? new FVector2D(MouseLocationX, MouseLocationY)
                    : LockProjection;
            }
            else if (InputType == E_InputType.Gamepad)
            {
                ScreenPosition = LockProjection;
            }
            else
            {
                ScreenPosition = bIsCurrentlyPressed
                    ? new FVector2D(TouchLocationX, TouchLocationY)
                    : LockProjection;
            }

            ScreenPos = ScreenPosition;

            /*
             * Project Screen Position to Game Plane
             */
            var WorldPosition = new FVector();

            var WorldDirection = new FVector();

            UGameplayStatics.DeprojectScreenToWorld(
                UGameplayStatics.GetPlayerController(this, 0),
                ScreenPosition,
                ref WorldPosition,
                ref WorldDirection
            );

            var T = 0.0f;

            var RefIntersection = new FVector();

            UKismetMathLibrary.LinePlaneIntersection(
                WorldPosition,
                WorldPosition + new FVector
                (
                    WorldDirection.X * 100000.0,
                    WorldDirection.Y * 100000.0,
                    WorldDirection.Z * 100000.0
                ),
                UKismetMathLibrary.MakePlaneFromPointAndNormal(new FVector(), new FVector { Z = 1.0 }),
                ref T,
                ref RefIntersection);

            Intersection = RefIntersection + new FVector
            {
                Z = InputType == E_InputType.Touch
                    ? bIsCurrentlyPressed ? 0.0f : -500.0f
                    : 0.0f
            };

            if (InputType == E_InputType.Unknown)
            {
                return false;
            }
            else if (InputType == E_InputType.KeyMouse)
            {
                return bAssociatedMouseDevice;
            }
            else if (InputType == E_InputType.Gamepad)
            {
                return true;
            }
            else
            {
                return bIsCurrentlyPressed;
            }
        }

        private void EdgeMove(out FVector Direction, out Double Strength)
        {
            /*
             * Get viewport center
             */
            var PlayerController = GetPlayerController();

            var SizeX = 0;

            var SizeY = 0;

            PlayerController.GetViewportSize(ref SizeX, ref SizeY);

            ProjectMouseOrTouch1ToGroundPlane(out var ScreenPos, out var Intersection);

            CursorDistFromViewportCenter(ScreenPos - new FVector2D(SizeX, SizeY) / new FVector2D(2.0),
                out var OutDirection, out var OutStrength);

            Direction = UKismetMathLibrary.TransformDirection(GetTransform(), OutDirection);

            Strength = OutStrength;
        }

        private void CursorDistFromViewportCenter(FVector2D A, out FVector Direction, out Double Strength)
        {
            /*
             * Viewport Half Size - Edge Detect Distance
             */
            var PlayerController = GetPlayerController();

            var SizeX = 0;

            var SizeY = 0;

            PlayerController.GetViewportSize(ref SizeX, ref SizeY);

            Double Distance = 0.0f;

            if (InputType == E_InputType.Unknown)
            {
                Distance = 0.0f;
            }
            else if (InputType == E_InputType.KeyMouse)
            {
                Distance = 1.0f;
            }
            else if (InputType == E_InputType.Gamepad)
            {
                Distance = 2.0f;
            }
            else if (InputType == E_InputType.Touch)
            {
                Distance = 2.0f;
            }

            /*
             * Cursor Position offset by center screen position
             */

            /*
             * Account for negative direction
             */

            /*
             * Offset tracked mouse position to create dead zone
             */

            Direction = new FVector
            {
                X = UKismetMathLibrary.SignOfFloat(A.Y) *
                    (UKismetMathLibrary.FMax(
                         UKismetMathLibrary.Abs(A.Y) - (SizeY / 2.0 - Edge_h20_Move_h20_Distance * Distance), 0.0) /
                     Edge_h20_Move_h20_Distance) * -1.0,
                Y = UKismetMathLibrary.SignOfFloat(A.X) *
                    (UKismetMathLibrary.FMax(
                         UKismetMathLibrary.Abs(A.X) - (SizeX / 2.0 - Edge_h20_Move_h20_Distance * Distance), 0.0) /
                     Edge_h20_Move_h20_Distance)
            };

            Strength = 1.0;
        }

        private CancellationTokenSource MovTrackingTokenSource;

        private CancellationTokenSource ClosestHoverCheckTokenSource;

        private CancellationTokenSource ClearHoverActorTokenSource;

        private CancellationTokenSource UpdatePathTokenSource;
    }
}