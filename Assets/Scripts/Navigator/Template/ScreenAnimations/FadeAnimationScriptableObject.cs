using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Navigator.Core;
using UnityEngine;

namespace Navigator.Template.ScreenAnimations
{
    [CreateAssetMenu(fileName = nameof(FadeAnimationScriptableObject), menuName = "Navigator/Animations/" + nameof(FadeAnimationScriptableObject))]
    public class FadeAnimationScriptableObject : ScriptableObject, IScreenAnimation
    {
        [SerializeField] private CanvasGroup _group;

        [SerializeField] private float _duration;
        [SerializeField] private AnimationCurve _curve;
        [SerializeField] private float _startValue;
        [SerializeField] private float _endValue;
        
        [field:SerializeField]
        public string Id { get; }
        public Transform ScreenTransformInject { get; set; }

        public async UniTask<bool> DoAnimation(CancellationToken token)
        {
            RewindToFirstFrame();
            
            await _group.DOFade(_endValue, _duration).SetEase(_curve).ToUniTask(cancellationToken: token);

            return !token.IsCancellationRequested;
        }

        public void RewindToFirstFrame()
        {
            _group.alpha = _startValue;
        }

        public void RewindToLastFrame()
        {
            _group.alpha = _endValue;
        }
    }
}