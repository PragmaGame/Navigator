using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace Navigator.Core
{
    public class SequenceAnimation : IShowScreenAnimation
    {
        public async UniTask<bool> Show(IEnumerable<UniTask<bool>> tasks)
        {
            foreach (var task in tasks)
            {
                if (!await task)
                {
                    return false;
                }
            }

            return true;
        }
    }
}