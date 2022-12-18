using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace Navigator
{
    public interface IShowScreenAnimation
    {
        public UniTask<bool> Show(IEnumerable<UniTask<bool>> tasks);
    }
}