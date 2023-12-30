using Script.Common;
using Script.CoreUObject;
using Script.Engine;
using Script.Game.Blueprint.Core.GameMode;
using Script.Game.Blueprint.Interactable.Extras;
using Script.InputCore;

namespace Script.Game.Blueprint.Core.Extras
{
    [IsOverride]
    public partial class BPC_Cheats_C
    {
        [IsOverride]
        public override void ReceiveBeginPlay()
        {
            /*
             * Detect Input and Call a Switch Event if input changes.
             */
            GetOwner().InputComponent.BindKey(EKeys.One, EInputEvent.IE_Pressed, this, One_Pressed);

            GetOwner().InputComponent.BindKey(EKeys.Two, EInputEvent.IE_Pressed, this, Two_Pressed);

            GetOwner().InputComponent.BindKey(EKeys.Three, EInputEvent.IE_Pressed, this, Three_Pressed);

            GetOwner().InputComponent.BindKey(EKeys.Four, EInputEvent.IE_Pressed, this, Four_Pressed);

            GetOwner().InputComponent.BindKey(EKeys.Five, EInputEvent.IE_Pressed, this, Five_Pressed);

            GetOwner().InputComponent.BindKey(EKeys.Six, EInputEvent.IE_Pressed, this, Six_Pressed);
        }

        [IsOverride]
        private void One_Pressed(FKey Key)
        {
            (UGameplayStatics.GetGameMode(this) as IBPI_Resource_C)?.Add_h20_Resource(
                E_ResourceType.Food, 15);
        }

        [IsOverride]
        private void Two_Pressed(FKey Key)
        {
            (UGameplayStatics.GetGameMode(this) as IBPI_Resource_C)?.Add_h20_Resource(
                E_ResourceType.Wood, 15);
        }

        [IsOverride]
        private void Three_Pressed(FKey Key)
        {
            (UGameplayStatics.GetGameMode(this) as IBPI_Resource_C)?.Add_h20_Resource(
                E_ResourceType.Stone, 15);
        }

        [IsOverride]
        private void Four_Pressed(FKey Key)
        {
            (UGameplayStatics.GetGameMode(this) as IBPI_Resource_C)?.Remove_h20_Target_h20_Resource(
                E_ResourceType.Food, 15);
        }

        [IsOverride]
        private void Five_Pressed(FKey Key)
        {
            (UGameplayStatics.GetGameMode(this) as IBPI_Resource_C)?.Remove_h20_Target_h20_Resource(
                E_ResourceType.Wood, 15);
        }

        [IsOverride]
        private void Six_Pressed(FKey Key)
        {
            (UGameplayStatics.GetGameMode(this) as IBPI_Resource_C)?.Remove_h20_Target_h20_Resource(
                E_ResourceType.Stone, 15);
        }
    }
}