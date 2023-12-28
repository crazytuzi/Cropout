using System;
using System.Runtime.CompilerServices;
using Script.Common;
using Script.Engine;

namespace Script.Library
{
    public static class EnhancedInputComponentImplementation
    {
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void EnhancedInputComponent_GetDynamicBindingObjectImplementation<T>(
            IntPtr InThisClass,
            IntPtr InBindingClass,
            out T OutValue)
            where T : UDynamicBlueprintBinding;

        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void EnhancedInputComponent_BindActionImplementation(
            IntPtr InObject,
            IntPtr InBlueprintEnhancedInputActionBinding,
            IntPtr InObjectToBindTo,
            FName InFunctionNameToBind);
    }
}