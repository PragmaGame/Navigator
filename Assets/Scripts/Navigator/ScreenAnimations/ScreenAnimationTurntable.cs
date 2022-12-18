using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sirenix.Serialization;
using UnityEngine;

namespace Navigator
{
    [Serializable]
    public sealed class ScreenAnimationTurntable
    {
        [SerializeField] private bool _isPermissionOverlap;
        [SerializeReference] private IShowScreenAnimation _showScreenAnimation = new Simultaneously();
        [OdinSerialize] private IScreenAnimation[] _animations = new IScreenAnimation[0];
        
        public bool IsPermissionOverlap => _isPermissionOverlap;
        
        public IScreenAnimation[] Animations => _animations;

        public async UniTask PlayAnimations(CancellationToken token)
        {
            await _showScreenAnimation.Show(_animations.Select(animation => animation.DoAnimation(token)));
        }
    }
}