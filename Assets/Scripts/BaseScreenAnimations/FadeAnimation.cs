using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Navigator;
using UnityEngine;

namespace BaseScreenAnimations
{
    [Serializable]
    public class FadeAnimation : IScreenAnimation
    {
        [SerializeField] private CanvasGroup _group;

        [SerializeField] private float _duration;
        [SerializeField] private AnimationCurve _curve;
        [SerializeField] private float _endValue;
        
        [field:SerializeField]
        public string Id { get; }

        public async UniTask<bool> DoAnimation(CancellationToken token)
        {
            return await _group.DOFade(_endValue, _duration).SetEase(_curve).ToUniTask(cancellationToken: token).SuppressCancellationThrow();
        }
    }
}