using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;

namespace Navigator.Core
{
    public class Simultaneously : IShowScreenAnimation
    {
        public async UniTask<bool> Show(IEnumerable<UniTask<bool>> tasks)
        {
            var result = await UniTask.WhenAll(tasks);
            
            return result.All(x => x);
        }
    }
}