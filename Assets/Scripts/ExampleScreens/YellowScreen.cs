using Cysharp.Threading.Tasks;
using Navigator;

namespace ExampleScreens
{
    public class YellowScreen : Screen
    {
        protected override UniTask OnFocus()
        {
            return UniTask.CompletedTask;
        }

        protected override UniTask OnBlur()
        {
            return UniTask.CompletedTask;
        }
    }
}