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
            EnhancedInputComponentImplementation
                .EnhancedInputComponent_GetDynamicBindingObjectImplementation<UEnhancedInputActionDelegateBinding>(
                    InObject.GetClass().GetHandle(),
                    UEnhancedInputActionDelegateBinding.StaticClass().GetHandle(),
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

                EnhancedInputComponentImplementation.EnhancedInputComponent_BindActionImplementation(
                    GetHandle(), Binding.GetHandle(), InObject.GetHandle(), Binding.FunctionNameToBind);
            }
        }
    }
}