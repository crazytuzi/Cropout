using Script.Common;
using Script.CoreUObject;
using Script.Engine;

namespace Script.Game.Blueprint.Core.GameMode
{
    [IsOverride]
    public partial class BPF_Cropout_C
    {
        [IsOverride]
        public static void SteppedPosition(FVector NewParam, UObject __WorldContext, out FVector NewParam1)
        {
            NewParam1 = new FVector(UKismetMathLibrary.Round(NewParam.X / 200.0) * 200.0,
                UKismetMathLibrary.Round(NewParam.Y / 200.0) * 200.0, 0.0);
        }

        [IsOverride]
        public static void GetCropoutGI(UObject __WorldContext, out BP_GI_C GI)
        {
            GI = UGameplayStatics.GetGameInstance(__WorldContext) as BP_GI_C;
        }

        [IsOverride]
        public static void GetCropoutGM(UObject __WorldContext, out BP_GM_C GM)
        {
            GM = UGameplayStatics.GetGameMode(__WorldContext) as BP_GM_C;
        }
    }
}