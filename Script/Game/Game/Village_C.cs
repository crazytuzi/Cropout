using Script.AudioModulation;
using Script.Common;
using Script.Engine;
using Script.Game.Audio.DATA.ControlBus;
using Script.Game.Audio.MUSIC;
using Script.Game.Blueprint.Core.GameMode;
using Script.Library;

namespace Script.Game
{
    [IsOverride]
    public partial class Village_C
    {
        [IsOverride]
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
    }
}