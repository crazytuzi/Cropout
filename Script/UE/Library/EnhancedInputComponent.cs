using System;
using Script.CoreUObject;
using Script.Library;

namespace Script.EnhancedInput
{
    public partial class UEnhancedInputComponent
    {
        public void BindAction<T>(ETriggerEvent InTriggerEvent,
            UObject InObject,
            Action<FInputActionValue, Single, Single, UInputAction> InAction)
            where T : UInputAction, IStaticClass
        {
            BindAction(Unreal.LoadObject<T>(this), InTriggerEvent, InObject, InAction);
        }

        public void BindAction(UInputAction InInputAction, ETriggerEvent InTriggerEvent,
            UObject InObject, Action<FInputActionValue, Single, Single, UInputAction> InAction)
        {
            UEnhancedInputComponentImplementation
                .UEnhancedInputComponent_GetDynamicBindingObjectImplementation<UEnhancedInputActionDelegateBinding>(
                    InObject.GetClass().GarbageCollectionHandle,
                    UEnhancedInputActionDelegateBinding.StaticClass().GarbageCollectionHandle,
                    out var EnhancedInputActionDelegateBinding
                );

            if (EnhancedInputActionDelegateBinding != null)
            {
                foreach (var InputActionDelegate in EnhancedInputActionDelegateBinding.InputActionDelegateBindings)
                {
                    if (InputActionDelegate.FunctionNameToBind.ToString() == InAction.Method.Name)
                    {
                        return;
                    }
                }

                var Binding = new FBlueprintEnhancedInputActionBinding
                {
                    InputAction = InInputAction,
                    TriggerEvent = InTriggerEvent,
                    FunctionNameToBind = InAction.Method.Name
                };

                EnhancedInputActionDelegateBinding.InputActionDelegateBindings.Add(Binding);

                UEnhancedInputComponentImplementation.UEnhancedInputComponent_BindActionImplementation(
                    GarbageCollectionHandle, Binding.GarbageCollectionHandle, InObject.GarbageCollectionHandle,
                    Binding.FunctionNameToBind.GarbageCollectionHandle);
            }
        }
    }
}