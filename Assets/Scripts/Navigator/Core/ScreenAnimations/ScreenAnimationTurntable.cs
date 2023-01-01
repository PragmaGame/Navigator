using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sirenix.Serialization;
using Sirenix.Utilities;
using UnityEngine;

namespace Navigator.Core
{
    [Serializable]
    public sealed class ScreenAnimationTurntable
    {
        [SerializeField] private bool _isPermissionOverlap;
        [SerializeReference] private IShowScreenAnimation _showScreenAnimation = new Simultaneously();
        [OdinSerialize] private IScreenAnimation[] _animations = new IScreenAnimation[0];
        
        public bool IsPermissionOverlap => _isPermissionOverlap;

        public IScreenAnimation[] Animations => _animations;

        private Transform _animationObject;

        public void SetAnimationObject(Transform animationObject)
        {
            _animationObject = animationObject;

            _animations.ForEach(x => x.ScreenTransformInject = _animationObject);
        }

        public async UniTask PlayAnimations(
            CancellationToken token, 
            IShowScreenAnimation showScreenAnimation = null, 
            string[] idAnimations = null, 
            IScreenAnimation[] customAnimations = null)
        {
            showScreenAnimation ??= _showScreenAnimation;

            var selection = idAnimations == null ? _animations : _animations.Where(animation => idAnimations.Contains(animation.Id));

            var showList = new List<UniTask<bool>>();
            
            showList.AddRange(selection.Select(animation => animation.DoAnimation(token)));

            if (customAnimations != null && customAnimations.Length != 0)
            {
                customAnimations.ForEach(x => x.ScreenTransformInject = _animationObject);
                showList.AddRange(customAnimations.Select(animation => animation.DoAnimation(token)));
            }
            
            await showScreenAnimation.Show(showList);
        }

        public async UniTask PlayAnimations(CancellationToken token, ScreenAnimationBlockData screenAnimationBlockData = null)
        {
            if (screenAnimationBlockData == null)
            {
                await PlayAnimations(token, _showScreenAnimation);
            }
            else
            {
                await PlayAnimations(token, screenAnimationBlockData.ShowScreenAnimation, screenAnimationBlockData.IdAnimations,
                    screenAnimationBlockData.CustomAnimations);
            }
        }
    }
}