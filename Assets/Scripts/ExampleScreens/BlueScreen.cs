using System.Threading;
using Cysharp.Threading.Tasks;
using Navigator;
using Navigator.Core;

namespace ExampleScreens
{
    public class BlueScreen : Screen
    {
        protected override UniTask OnFocus(CancellationToken token)
        {
            return UniTask.CompletedTask;
        }

        protected override UniTask OnBlur(CancellationToken token)
        {
            return UniTask.CompletedTask;
        }
    }
}