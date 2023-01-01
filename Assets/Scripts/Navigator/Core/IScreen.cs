using Cysharp.Threading.Tasks;

namespace Navigator.Core
{
    public interface IScreen
    {
        public UniTask Show(ScreenAnimationBlockData screenAnimationBlockData = null);
        public UniTask Hide(ScreenAnimationBlockData screenAnimationBlockData = null);
        public UniTask Focus();
        public UniTask Blur();
    }
}