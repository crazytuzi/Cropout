#include "EnhancedInputComponent.h"
#include "Binding/Class/TReflectionClassBuilder.inl"
#include "Environment/FCSharpEnvironment.h"
#include "Macro/BindingMacro.h"
#include "Macro/NamespaceMacro.h"
#include "Engine/DynamicBlueprintBinding.h"
#include "EnhancedInputActionDelegateBinding.h"

BINDING_REFLECTION_CLASS(UEnhancedInputComponent);

struct FRegisterEnhancedInputComponent
{
	static void GetDynamicBindingObjectImplementation(const FGarbageCollectionHandle InThisClass,
	                                                  const FGarbageCollectionHandle InBindingClass,
	                                                  MonoObject** OutValue)
	{
		const auto ThisClass = FCSharpEnvironment::GetEnvironment().GetObject<UBlueprintGeneratedClass>(InThisClass);

		const auto BindingClass = FCSharpEnvironment::GetEnvironment().GetObject<UClass>(InBindingClass);

		if (ThisClass != nullptr && BindingClass != nullptr)
		{
			UObject* DynamicBindingObject = UBlueprintGeneratedClass::GetDynamicBindingObject(ThisClass, BindingClass);

			if (DynamicBindingObject == nullptr)
			{
				DynamicBindingObject = NewObject<UObject>(GetTransientPackage(), BindingClass);

				ThisClass->DynamicBindingObjects.Add(reinterpret_cast<UDynamicBlueprintBinding*>(DynamicBindingObject));
			}

			*OutValue = FCSharpEnvironment::GetEnvironment().Bind(DynamicBindingObject);
		}
	}

	static void BindActionImplementation(const FGarbageCollectionHandle InGarbageCollectionHandle,
	                                     const FGarbageCollectionHandle InBlueprintEnhancedInputActionBinding,
	                                     const FGarbageCollectionHandle InObjectToBindTo,
	                                     MonoObject* InFunctionNameToBind)
	{
		if (const auto FoundObject = FCSharpEnvironment::GetEnvironment().GetObject<UEnhancedInputComponent>(
			InGarbageCollectionHandle))
		{
			const auto BlueprintEnhancedInputActionBinding = *static_cast<FBlueprintEnhancedInputActionBinding*>(
				FCSharpEnvironment::GetEnvironment().GetStruct(InBlueprintEnhancedInputActionBinding));

			const auto ObjectToBindTo = FCSharpEnvironment::GetEnvironment().GetObject<UObject>(InObjectToBindTo);

			FoundObject->BindAction(BlueprintEnhancedInputActionBinding.InputAction,
			                        BlueprintEnhancedInputActionBinding.TriggerEvent,
			                        ObjectToBindTo,
			                        BlueprintEnhancedInputActionBinding.FunctionNameToBind
			);

			const auto FunctionNameToBind = FName(UTF8_TO_TCHAR(
				FCSharpEnvironment::GetEnvironment().GetDomain()->String_To_UTF8(FCSharpEnvironment::GetEnvironment().
					GetDomain()->Object_To_String(InFunctionNameToBind, nullptr))));

			BindActionFunction(ObjectToBindTo->GetClass(), FunctionNameToBind);
		}
	}

	static void BindFunction(UClass* InClass, const FName& InFunctionName,
	                         const TFunction<void(UFunction* InFunction)>& InProperty)
	{
		if (InClass == nullptr)
		{
			return;
		}

		if (InClass->FindFunctionByName(InFunctionName))
		{
			return;
		}

		auto Function = NewObject<UFunction>(InClass, InFunctionName, EObjectFlags::RF_Transient);

		Function->FunctionFlags = FUNC_BlueprintEvent;

		InProperty(Function);

		Function->Bind();

		Function->StaticLink(true);

		InClass->AddFunctionToFunctionMap(Function, InFunctionName);

		Function->Next = InClass->Children;

		InClass->Children = Function;

		Function->AddToRoot();

		FCSharpEnvironment::GetEnvironment().Bind(Function);
	}

	static void BindActionFunction(UClass* InClass, const FName& InFunctionName)
	{
		BindFunction(InClass, InFunctionName, [](UFunction* InFunction)
		{
			const auto SourceActionProperty = new FObjectProperty(InFunction, TEXT("SourceAction"),
			                                                      RF_Public | RF_Transient);

			SourceActionProperty->PropertyClass = UInputAction::StaticClass();

			SourceActionProperty->SetPropertyFlags(CPF_Parm);

			InFunction->AddCppProperty(SourceActionProperty);

			const auto TriggeredTimeProperty = new FFloatProperty(InFunction, TEXT("TriggeredTime"),
			                                                      RF_Public | RF_Transient);

			TriggeredTimeProperty->SetPropertyFlags(CPF_Parm);

			InFunction->AddCppProperty(TriggeredTimeProperty);

			const auto ElapsedTimeProperty = new FFloatProperty(InFunction, TEXT("ElapsedTime"),
			                                                    RF_Public | RF_Transient);

			ElapsedTimeProperty->SetPropertyFlags(CPF_Parm);

			InFunction->AddCppProperty(ElapsedTimeProperty);

			const auto ActionValueProperty = new FStructProperty(InFunction, TEXT("ActionValue"),
			                                                     RF_Public | RF_Transient);

			ActionValueProperty->ElementSize = FInputActionValue::StaticStruct()->GetStructureSize();

			ActionValueProperty->Struct = FInputActionValue::StaticStruct();

			ActionValueProperty->SetPropertyFlags(CPF_Parm);

			InFunction->AddCppProperty(ActionValueProperty);
		});
	}

	FRegisterEnhancedInputComponent()
	{
		TReflectionClassBuilder<UEnhancedInputComponent>(NAMESPACE_LIBRARY)
			.Function("GetDynamicBindingObject", GetDynamicBindingObjectImplementation)
			.Function("BindAction", BindActionImplementation)
			.Register();
	}
};

static FRegisterEnhancedInputComponent RegisterEnhancedInputComponent;
