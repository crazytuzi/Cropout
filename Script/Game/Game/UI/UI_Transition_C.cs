using System;
using System.Threading;
using System.Threading.Tasks;
using Script.CoreUObject;
using Script.UMG;

namespace Script.Game.UI
{
    [Override]
    public partial class UI_Transition_C
    {
        [Override]
        public override void Destruct()
        {
            TokenSource?.Cancel();
        }

        public void TransIn()
        {
            PlayAnimation(NewAnimation);
        }

        public void TransOut()
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