using System.Threading;
using Cysharp.Threading.Tasks;

namespace Navigator
{
    public interface IScreenAnimation
    {
        public string Id { get;}
        public UniTask<bool> DoAnimation(CancellationToken token);
    }
}