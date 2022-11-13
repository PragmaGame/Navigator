using Cysharp.Threading.Tasks;
using Navigator;

namespace ExampleScreens
{
    public class VioletScreen : Screen
    {
        public override UniTask Focus()
        {
            return UniTask.CompletedTask;
        }

        public override UniTask Blur()
        {
            return UniTask.CompletedTask;
        }
    }
}