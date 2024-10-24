using Script.AudioModulation;
using Script.CoreUObject;
using Script.Engine;
using Script.Game.Audio.DATA.ControlBus;
using Script.Game.Audio.MUSIC;
using Script.Game.Blueprint.Core.GameMode;
using Script.Library;

namespace Script.Game
{
    [PathName("/Game/Village.Village_C")]
    public partial class Village_C : ALevelScriptActor, IStaticClass
    {
        public new static UClass StaticClass()
        {
            return StaticClassSingleton ??=
                UObjectImplementation.UObject_StaticClassImplementation("/Game/Village.Village_C");
        }

        [Override]
        public override void ReceiveBeginPlay()
        {
            var BP_GI = UGameplayStatics.GetGameInstance(this) as BP_GI_C;

            UAudioModulationStatics.SetGlobalBusMixValue(this,
                Unreal.LoadObject<Cropout_Music_Piano_Vol>(this),
                1.0f,
                1.0f);

            UAudioModulationStatics.SetGlobalBusMixValue(this,
                Unreal.LoadObject<Cropout_Music_Perc_Vol>(this),
                1.0f,
                1.0f);

            UAudioModulationStatics.SetGlobalBusMixValue(this,
                Unreal.LoadObject<Cropout_Music_Strings_Delay>(this),
                0.0f,
                1.0f);

            BP_GI.PlayMusic(Unreal.LoadObject<MUS_Main_MSS>(this));
        }

        private static UClass StaticClassSingleton { get; set; }
    }
}