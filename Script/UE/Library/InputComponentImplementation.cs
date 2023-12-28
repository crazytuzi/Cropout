using System;
using System.Runtime.CompilerServices;
using Script.Common;
using Script.Engine;

namespace Script.Library
{
    public static class InputComponentImplementation
    {
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void InputComponent_GetDynamicBindingObjectImplementation<T>(
            IntPtr InThisClass,
            IntPtr InBindingClass,
            out T OutValue)
            where T : UDynamicBlueprintBinding;

        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void InputComponent_BindActionImplementation(
            IntPtr InObject,
            IntPtr InInputActionDelegateBinding,
            IntPtr InObjectToBindTo, FName InFunctionNameToBind);

        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void InputComponent_BindAxisImplementation(
            IntPtr InObject,
            IntPtr InInputAxisDelegateBinding,
            IntPtr InObjectToBindTo, FName InFunctionNameToBind);

        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void InputComponent_BindAxisKeyImplementation(
            IntPtr InObject,
            IntPtr InInputAxisKeyDelegateBinding,
            IntPtr InObjectToBindTo, FName InFunctionNameToBind);

        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void InputComponent_BindKeyImplementation(
            IntPtr InObject,
            IntPtr InInputKeyDelegateBinding,
            IntPtr InObjectToBindTo, FName InFunctionNameToBind);

        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void InputComponent_BindTouchImplementation(
            IntPtr InObject,
            IntPtr InInputTouchDelegateBinding,
            IntPtr InObjectToBindTo, FName InFunctionNameToBind);

        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void InputComponent_BindVectorAxisImplementation(
            IntPtr InObject,
            IntPtr InInputVectorAxisDelegateBinding,
            IntPtr InObjectToBindTo, FName InFunctionNameToBind);
    }
}