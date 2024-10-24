using System;
using System.Runtime.CompilerServices;
using Script.Engine;

namespace Script.Library
{
    public static partial class UEnhancedInputComponentImplementation
    {
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void UEnhancedInputComponent_GetDynamicBindingObjectImplementation<T>(
            IntPtr InThisClass,
            IntPtr InBindingClass,
            out T OutValue)
            where T : UDynamicBlueprintBinding;

        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void UEnhancedInputComponent_BindActionImplementation(
            IntPtr InObject,
            IntPtr InBlueprintEnhancedInputActionBinding,
            IntPtr InObjectToBindTo,
            IntPtr InFunctionNameToBind);
    }
}