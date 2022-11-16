using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Navigator.BaseScreenAnimations
{
    [Serializable]
    public class FadeAnimation : IScreenAnimation
    {
        [SerializeField] private CanvasGroup _group;

        [SerializeField] private float _duration;
        [SerializeField] private AnimationCurve _curve;
        [SerializeField] private float _endValue;

        public async UniTask DoAnimation(CancellationToken token)
        {
            await _group.DOFade(_endValue, _duration).SetEase(_curve).ToUniTask(cancellationToken: token);
        }
    }
}