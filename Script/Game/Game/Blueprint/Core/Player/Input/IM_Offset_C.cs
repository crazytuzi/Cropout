using System;
using Script.Common;
using Script.EnhancedInput;

namespace Script.Game.Blueprint.Core.Player.Input
{
    [IsOverride]
    public partial class IM_Offset_C
    {
        [IsOverride]
        public override FInputActionValue ModifyRaw(UEnhancedPlayerInput PlayerInput, FInputActionValue CurrentValue, Single DeltaTime)
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