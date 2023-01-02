using System.Threading;
using Cysharp.Threading.Tasks;

namespace Navigator.Core
{
    public interface IScreen
    {
        public bool IsLazyLoad { get; }
        public Navigator Navigator { get; set; }
        public UniTask Show(CancellationToken token = default, ScreenAnimationBlockData screenAnimationBlockData = null);
        public UniTask Hide(CancellationToken token = default, ScreenAnimationBlockData screenAnimationBlockData = null);
        public UniTask Focus(CancellationToken token = default);
        public UniTask Blur(CancellationToken token = default);
    }
}