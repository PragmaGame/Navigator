using Cysharp.Threading.Tasks;

namespace Navigator
{
    public class WaitScreenItem
    {
        public WaitScreenType type;
        public UniTaskCompletionSource waitCompletionSource;
        public Screen screen;
    }
}