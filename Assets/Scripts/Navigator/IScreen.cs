using Cysharp.Threading.Tasks;

namespace Navigator
{
    public interface IScreen
    {
        public UniTask Show(IScreenProceduralAnimation screenAnimation = null);
        public UniTask Hide(IScreenProceduralAnimation screenAnimation = null);
        public UniTask Focus();
        public UniTask Blur();
    }
}