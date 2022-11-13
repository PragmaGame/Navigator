using System.Threading;
using Cysharp.Threading.Tasks;

namespace Navigator
{
    public interface IScreenAnimation
    {
        public void ResetToDefaultView();
        
        public UniTask DoShowAnimation(CancellationToken token);
        public UniTask DoHideAnimation(CancellationToken token);
    }
}