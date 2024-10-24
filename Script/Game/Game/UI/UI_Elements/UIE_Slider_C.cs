using System;
using Script.CoreUObject;
using Script.Engine;
using Script.Game.Blueprint.Core.GameMode;

namespace Script.Game.UI.UI_Elements
{
    [Override]
    public partial class UIE_Slider_C
    {
        /*
         * Set Initial Text
         */
        [Override]
        public override void PreConstruct(bool IsDesignTime)
        {
            MixDescriptor.SetText(Sound_h20_Class_h20_Title);
        }

        [Override]
        public override void Construct()
        {
            Slider_67.OnValueChanged.Add(this, OnValueChanged);
        }

        [Override]
        public override void Destruct()
        {
            Slider_67.OnValueChanged.RemoveAll(this);
        }

        /*
         * Get Stored Sound Class Volume in GI
         */
        public void UpdateSlider()
        {
            BPF_Cropout_C.GetCropoutGI(this, out var GI);

            Slider_67.SetValue((float)GI.SoundMixes[Index]);
        }

        /*
         * Update Sound Class Volume
         */
        private void OnValueChanged(Single Value)
        {
            BPF_Cropout_C.GetCropoutGI(this, out var GI);

            GI.SoundMixes[Index] = Value;

            UGameplayStatics.SetSoundMixClassOverride(this,
                In_h20_Sound_h20_Mix_h20_Modifier,
                In_h20_Sound_h20_Class, Value);
        }
    }
}