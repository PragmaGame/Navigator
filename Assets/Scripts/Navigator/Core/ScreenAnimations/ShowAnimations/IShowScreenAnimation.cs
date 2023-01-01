using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace Navigator.Core
{
    public interface IShowScreenAnimation
    {
        public UniTask<bool> Show(IEnumerable<UniTask<bool>> tasks);
    }
}