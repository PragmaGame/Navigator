using Cysharp.Threading.Tasks;

namespace Navigator
{
    public interface IScreen
    {
        public UniTask Show();
        public UniTask Hide();
        public UniTask Focus();
        public UniTask Blur();
    }
}