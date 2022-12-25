using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sirenix.Serialization;
using Sirenix.Utilities;
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

        [HideInInspector] public Transform AnimationObject;

        public async UniTask PlayAnimations(
            CancellationToken token, 
            IShowScreenAnimation showScreenAnimation = null, 
            string[] idAnimations = null, 
            IScreenProceduralAnimation[] proceduralAnimations = null)
        {
            showScreenAnimation ??= _showScreenAnimation;

            IEnumerable<IScreenAnimation> selection;
            
            if (idAnimations == null || idAnimations.Length == 0)
            {
                selection = _animations;
            }
            else
            {
                selection = _animations.Where(animation => idAnimations.Contains(animation.Id));
            }

            var showList = new List<UniTask<bool>>();
            
            showList.AddRange(selection.Select(animation => animation.DoAnimation(token)));

            if (proceduralAnimations != null && proceduralAnimations.Length != 0)
            {
                proceduralAnimations.ForEach(x => x.ScreenTransformInject = AnimationObject);
                showList.AddRange(proceduralAnimations.Select(animation => animation.DoAnimation(token)));
            }
            
            await showScreenAnimation.Show(showList);
        }

        public async UniTask PlayAnimations(CancellationToken token, ScreenAnimationData screenAnimationData)
        {
            await PlayAnimations(token, screenAnimationData.ShowScreenAnimation, screenAnimationData.idAnimations,
                screenAnimationData.ProceduralAnimation);
        }
    }
}