using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Navigator.Core
{
    public interface IScreenAnimation
    {
        public string Id { get;}
        public UniTask<bool> DoAnimation(CancellationToken token);
        public Transform ScreenTransformInject { get; set; }
    }
}