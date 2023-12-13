using System;
using System.Threading;
using System.Threading.Tasks;
using Script.Common;
using Script.UMG;

namespace Script.Game.UI
{
    [IsOverride]
    public partial class UI_Transition_C
    {
        [IsOverride]
        public override void Destruct()
        {
            TokenSource?.Cancel();
        }

        [IsOverride]
        public virtual void Trans_h20_In()
        {
            PlayAnimation(NewAnimation);
        }

        [IsOverride]
        public virtual void Trans_h20_Out()
        {
            PlayAnimation(NewAnimation, 0.0f, 1, EUMGSequencePlayMode.Reverse);

            TokenSource = new CancellationTokenSource();

            PlayAnimation();
        }

        private async void PlayAnimation()
        {
            while (!TokenSource.IsCancellationRequested)
            {
                await Task.Delay((Int32)NewAnimation.GetEndTime() * 1000);

                TokenSource.Cancel();

                RemoveFromParent();
            }
        }

        private CancellationTokenSource TokenSource;
    }
}