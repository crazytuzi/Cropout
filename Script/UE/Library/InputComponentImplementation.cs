using System;
using System.Runtime.CompilerServices;
using Script.Engine;

namespace Script.Library
{
    public static partial class UInputComponentImplementation
    {
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void UInputComponent_GetDynamicBindingObjectImplementation<T>(
            IntPtr InThisClass,
            IntPtr InBindingClass,
            out T OutValue)
            where T : UDynamicBlueprintBinding;

        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void UInputComponent_BindActionImplementation(
            IntPtr InObject,
            IntPtr InInputActionDelegateBinding,
            IntPtr InObjectToBindTo,
            IntPtr InFunctionNameToBind);

        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void UInputComponent_BindAxisImplementation(
            IntPtr InObject,
            IntPtr InInputAxisDelegateBinding,
            IntPtr InObjectToBindTo,
            IntPtr InFunctionNameToBind);

        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void UInputComponent_BindAxisKeyImplementation(
            IntPtr InObject,
            IntPtr InInputAxisKeyDelegateBinding,
            IntPtr InObjectToBindTo,
            IntPtr InFunctionNameToBind);

        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void UInputComponent_BindKeyImplementation(
            IntPtr InObject,
            IntPtr InInputKeyDelegateBinding,
            IntPtr InObjectToBindTo,
            IntPtr InFunctionNameToBind);

        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void UInputComponent_BindTouchImplementation(
            IntPtr InObject,
            IntPtr InInputTouchDelegateBinding,
            IntPtr InObjectToBindTo,
            IntPtr InFunctionNameToBind);

        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void UInputComponent_BindVectorAxisImplementation(
            IntPtr InObject,
            IntPtr InInputVectorAxisDelegateBinding,
            IntPtr InObjectToBindTo,
            IntPtr InFunctionNameToBind);
    }
}