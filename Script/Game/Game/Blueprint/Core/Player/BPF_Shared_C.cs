using Script.CoreUObject;
using Script.Engine;

namespace Script.Game.Blueprint.Core.Player
{
    [Override]
    public partial class BPF_Shared_C
    {
        [Override]
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