using System.Threading;
using Cysharp.Threading.Tasks;

namespace Navigator
{
    public interface IScreenAnimation
    {
        public UniTask DoAnimation(CancellationToken token);
    }
}