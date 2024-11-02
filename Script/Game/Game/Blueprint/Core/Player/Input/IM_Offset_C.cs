using Script.CoreUObject;
using Script.EnhancedInput;

namespace Script.Game.Blueprint.Core.Player.Input
{
    [Override]
    public partial class IM_Offset_C
    {
        [Override]
        public override FInputActionValue ModifyRaw(UEnhancedPlayerInput PlayerInput, FInputActionValue CurrentValue, float DeltaTime)
        {
            var X = 0.0;
            
            var Y = 0.0;

            var Z = 0.0;

            var Type = EInputActionValueType.Boolean;

            UEnhancedInputLibrary.BreakInputActionValue(CurrentValue, ref X, ref Y, ref Z, ref Type);
            
            return base.ModifyRaw(null, UEnhancedInputLibrary.MakeInputActionValueOfType(X + Offset, Y + Offset, Z + Offset, Type), 0.0f);
        }
    }
}