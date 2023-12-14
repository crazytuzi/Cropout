using Script.Common;
using Script.CoreUObject;
using Script.Engine;

namespace Script.Game.Blueprint.Core.Player
{
    [IsOverride]
    public partial class BPF_Shared_C
    {
        [IsOverride]
        public static void Convert_h20_To_h20_Stepped_h20_Pos(FVector A, UObject __WorldContext, out FVector NewParam)
        {
            NewParam = new FVector
            {
                X = UKismetMathLibrary.Round(A.X / 200.0) * 200.0,
                Y = UKismetMathLibrary.Round(A.Y / 200.0) * 200.0
            };
        }
    }
}