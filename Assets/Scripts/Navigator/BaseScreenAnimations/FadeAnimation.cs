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

        [SerializeField] private float _showDuration;
        [SerializeField] private AnimationCurve _showCurve;
        [SerializeField] private float _showValue = 1; 
        
        [SerializeField] private float _hideDuration;
        [SerializeField] private AnimationCurve _hideCurve;
        [SerializeField] private float _hideValue = 0; 
        
        public void ResetToDefaultView()
        {
            _group.alpha = 1f;
        }

        public async UniTask DoShowAnimation(CancellationToken token)
        {
            await _group.DOFade(_showValue, _showDuration).SetEase(_showCurve).ToUniTask(cancellationToken: token);
        }

        public async UniTask DoHideAnimation(CancellationToken token)
        {
            await _group.DOFade(_hideValue, _hideDuration).SetEase(_hideCurve).ToUniTask(cancellationToken: token);
        }
    }
}