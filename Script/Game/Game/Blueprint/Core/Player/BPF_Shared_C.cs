using Script.Common;
using Script.CoreUObject;
using Script.Engine;

namespace Script.Game.Blueprint.Core.Player
{
    [IsOverride]
    public partial class BPF_Shared_C
    {
        [IsOverride]
        public static void ConvertToSteppedPos(FVector A, UObject __WorldContext, out FVector NewParam)
        {
            NewParam = new FVector
            {
                X = UKismetMathLibrary.Round(A.X / 200.0) * 200.0,
                Y = UKismetMathLibrary.Round(A.Y / 200.0) * 200.0
            };
        }
    }
}