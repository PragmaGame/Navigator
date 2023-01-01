using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Navigator.Core
{
    [Serializable]
    public class SequenceDelayedAnimation : IShowScreenAnimation
    {
        [SerializeField] private float _delay;

        public async UniTask<bool> Show(IEnumerable<UniTask<bool>> tasks)
        {
            var runTasks = new List<UniTask<bool>>();
            
            foreach (var task in tasks)
            {
                runTasks.Add(TaskRun(task));

                await UniTask.Delay(TimeSpan.FromSeconds(_delay));
            }
            
            var results = await UniTask.WhenAll(runTasks);
            
            return results.All(x => x);
        }

        public async UniTask<bool> TaskRun(UniTask<bool> task)
        {
            return await task;
        }
    }
}