using System;
using System.Runtime.CompilerServices;
using Script.CoreUObject;
using Script.Engine;

namespace Script.Library
{
    public static partial class UEnhancedInputComponentImplementation
    {
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern T UEnhancedInputComponent_GetDynamicBindingObjectImplementation<T>(
            IntPtr InThisClass,
            IntPtr InBindingClass)
            where T : UDynamicBlueprintBinding;

        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern FEnhancedInputActionEventBinding UEnhancedInputComponent_BindActionImplementation(
            IntPtr InObject,
            IntPtr InBlueprintEnhancedInputActionBinding,
            IntPtr InObjectToBindTo,
            IntPtr InFunctionNameToBind);

        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void UEnhancedInputComponent_RemoveActionImplementation(
            IntPtr InObject,
            IntPtr InEnhancedInputActionEventBinding);
    }
}