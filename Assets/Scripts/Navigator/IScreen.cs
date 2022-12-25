using Cysharp.Threading.Tasks;

namespace Navigator
{
    public interface IScreen
    {
        public UniTask Show(ScreenAnimationData screenAnimationData = null);
        public UniTask Hide(ScreenAnimationData screenAnimationData = null);
        public UniTask Focus();
        public UniTask Blur();
    }
}