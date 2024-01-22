using System;
using System.Threading;
using System.Threading.Tasks;
using Script.AudioModulation;
using Script.Common;
using Script.CoreUObject;
using Script.Engine;
using Script.Game.Audio.DATA.ControlBus;
using Script.Game.Blueprint.Core.Save;
using Script.Game.Blueprint.Interactable;
using Script.Game.Blueprint.Interactable.Extras;
using Script.Game.UI;
using Script.Library;

namespace Script.Game.Blueprint.Core.GameMode
{
    [IsOverride]
    public partial class BP_GI_C
    {
        /*
         * On Begin play
         */
        [IsOverride]
        public override void ReceiveInit()
        {
            /*
             * Create Loading Screen Widget and store in GI
             */
            UI_Transition = Unreal.CreateWidget<UI_Transition_C>(this);

            LoadGame();
        }

        [IsOverride]
        public override void ReceiveShutdown()
        {
            OpenLevelTokenSource?.Cancel();

            SaveGameTokenSource?.Cancel();
        }

        /*
         * Set new level and mask with UI_Transition
         */
        public void OpenLevel(TSoftObjectPtr<UWorld> Level = null)
        {
            TransitionIn();

            OpenLevelTokenSource = new CancellationTokenSource();

            OnOpenLevel(Level);
        }

        /*
         * New Level
         */
        [IsOverride]
        public void ClearSave(Boolean Clear_h20_Seed = false)
        {
            ClearSaveRef();

            if (Clear_h20_Seed)
            {
                SaveRef.Seed = new FRandomStream(UKismetMathLibrary.RandomInteger(2147483647));
            }

            bHasSave = false;
        }

        [IsOverride]
        public void Load_h20_Level()
        {
            bHasSave = true;
        }

        /*
         * Save Game to slot
         */
        [IsOverride]
        public void Save_h20_Game()
        {
            var AsyncActionHandleSaveGame = UAsyncActionHandleSaveGame.AsyncSaveGameToSlot(this, SaveRef, "SAVE", 0);

            AsyncActionHandleSaveGame.Completed.Add(this, OnAsyncHandleSaveGame);

            AsyncActionHandleSaveGame.Activate();
        }

        /*
         * Update Interactables
         */
        [IsOverride]
        public void Update_h20_All_h20_Interactables()
        {
            SaveRef.Interactables.Empty();

            var OutActors = new TArray<AActor>();

            UGameplayStatics.GetAllActorsOfClass(this, BP_Interactable_C.StaticClass(), ref OutActors);

            foreach (var OutActor in OutActors)
            {
                SaveRef.Interactables.Add(new ST_SaveInteract
                {
                    Location = OutActor.GetTransform(),
                    Type = OutActor.GetClass(),
                    Health = ((BP_Interactable_C)OutActor).Progression_h20_State,
                    Tag = OutActor.Tags.IsValidIndex(0) ? OutActor.Tags[0] : ""
                });
            }

            Save_h20_Game();
        }

        /*
         * Update Resources. Expect this to trigger quite a bit. By adding a delay we can limit the number of times save is actually called.
         */
        [IsOverride]
        public void Update_h20_All_h20_Resources(TMap<E_ResourceType, Int32> NewParam = null)
        {
            SaveRef.Resources = NewParam;

            SaveGameTokenSource = new CancellationTokenSource();

            OnSaveGame();
        }

        /*
         * Update Villagers
         */
        [IsOverride]
        public void Update_h20_All_h20_Villagers()
        {
            SaveRef.Villagers.Empty();

            var OutActors = new TArray<AActor>();

            UGameplayStatics.GetAllActorsOfClass(this, APawn.StaticClass(), ref OutActors);

            foreach (var OutActor in OutActors)
            {
                if (OutActor as APawn != UGameplayStatics.GetPlayerPawn(this, 0))
                {
                    SaveRef.Villagers.Add(new ST_Villager
                    {
                        Location = OutActor.GetTransform().GetLocation(),
                        Task = OutActor.Tags.IsValidIndex(0) ? OutActor.Tags[0] : ""
                    });
                }
            }

            Save_h20_Game();
        }

        private void TransitionIn()
        {
            if (!UI_Transition.IsInViewport())
            {
                UI_Transition.AddToViewport();
            }

            UI_Transition.TransIn();
        }

        [IsOverride]
        public void TransitionOut()
        {
            if (!UI_Transition.IsInViewport())
            {
                UI_Transition.AddToViewport();
            }

            UI_Transition.TransOut();
        }

        /*
         * Check if save already exists
         */
        private void LoadGame()
        {
            /*
             * Check if save already exists
             */
            bHasSave = UGameplayStatics.DoesSaveGameExist("SAVE", 0);

            if (bHasSave)
            {
                /*
                 * Load save data if true
                 */
                SaveRef = UGameplayStatics.LoadGameFromSlot("SAVE", 0) as BP_SaveGM_C;
            }
            else
            {
                /*
                 * Create Save if False
                 */
                SaveRef = UGameplayStatics.CreateSaveGameObject(BP_SaveGM_C.StaticClass()) as BP_SaveGM_C;

                bMusicPlaying = false;
            }
        }

        private void ClearSaveRef()
        {
            SaveRef.Play_h20_Time = 0.0;

            SaveRef.Villagers.Empty();

            SaveRef.Interactables.Empty();

            SaveRef.Resources.Empty();

            SaveRef.Resources = new TMap<E_ResourceType, Int32>
            {
                { E_ResourceType.Food, 100 }
            };

            bMusicPlaying = false;
        }

        [IsOverride]
        public void Play_h20_Music(USoundBase Audio = null, Single Volume = 1.000000f, Boolean Persist = true)
        {
            if (!bMusicPlaying)
            {
                UAudioModulationStatics.SetGlobalBusMixValue(
                    this,
                    Unreal.LoadObject<Cropout_Music_WinLose>(this), 0.5f,
                    0.0f);

                UAudioModulationStatics.SetGlobalBusMixValue(
                    this,
                    Unreal.LoadObject<Cropout_Music_Stop>(this),
                    0.0f,
                    0.0f);

                this.Audio = UGameplayStatics.CreateSound2D(
                    this,
                    Audio,
                    Volume,
                    1.0f,
                    0.0f,
                    null,
                    Persist);

                this.Audio.Play();

                bMusicPlaying = true;
            }
        }

        public void StopMusic()
        {
            bMusicPlaying = false;

            UAudioModulationStatics.SetGlobalBusMixValue(
                this,
                Unreal.LoadObject<Cropout_Music_Piano_Vol>(this),
                1.0f,
                0.0f);
        }

        [IsOverride]
        public void Get_h20_Save(out BP_SaveGM_C Save_h20_Data)
        {
            Save_h20_Data = SaveRef;
        }

        [IsOverride]
        public void Get_h20_All_h20_Interactables(out TArray<ST_SaveInteract> NewParam)
        {
            NewParam = SaveRef.Interactables;
        }

        [IsOverride]
        public void Get_h20_Seed(out FRandomStream Seed)
        {
            Seed = SaveRef.Seed;
        }

        [IsOverride]
        public void Check_h20_Save_h20_Bool(out Boolean Save_h20_Exist)
        {
            Save_h20_Exist = bHasSave;
        }

        [IsOverride]
        public void Island_h20_Seed(out FRandomStream Seed)
        {
            Seed = SaveRef.Seed;
        }

        private async void OnOpenLevel(TSoftObjectPtr<UWorld> Level = null)
        {
            while (!OpenLevelTokenSource.IsCancellationRequested)
            {
                await Task.Delay(1100);

                OpenLevelTokenSource.Cancel();

                UGameplayStatics.OpenLevelBySoftObjectPtr(this, Level);
            }
        }

        private void OnAsyncHandleSaveGame(USaveGame SaveGame, Boolean bSuccess)
        {
            bHasSave = true;
        }

        private async void OnSaveGame()
        {
            while (!SaveGameTokenSource.IsCancellationRequested)
            {
                await Task.Delay(5000);

                SaveGameTokenSource.Cancel();

                Save_h20_Game();
            }
        }

        private CancellationTokenSource OpenLevelTokenSource;

        private CancellationTokenSource SaveGameTokenSource;

        private Boolean bMusicPlaying;

        private Boolean bHasSave;
    }
}