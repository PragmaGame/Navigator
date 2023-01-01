using Cysharp.Threading.Tasks;

namespace Navigator.Core
{
    public class WaitScreenItem
    {
        public WaitScreenType type;
        public UniTaskCompletionSource waitCompletionSource;
        public Screen screen;
    }
}