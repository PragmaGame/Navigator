using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Navigator
{
    [RequireComponent(typeof(Canvas), typeof(CanvasGroup), typeof(RectTransform))]
    public abstract class BaseScreen : MonoBehaviour, IScreen
    {
        [SerializeField] private bool lazyLoad;
        
        [SerializeField] private float showAnimationDuration = 0.3f;
        [SerializeField] private float hideAnimationDuration = 0.2f;

        public UniTaskCompletionSource<bool> ShowCompletionSource { get; private set; }
        public UniTaskCompletionSource<bool> HideCompletionSource { get; private set; }
        
        public bool IsPopup { get; set; }
        public bool LazyLoad => lazyLoad;

        private CanvasGroup _group;

        public Navigator Navigator { get; set; }

        protected virtual void Awake()
        {
            _group = GetComponent<CanvasGroup>();
        }

        private async UniTask DoAnimation(float to, float duration)
        {
            await _group.DOFade(to, duration).ToUniTask();
        }

        public virtual async UniTask Show()
        {
            ShowCompletionSource.TrySetResult(true);
            HideCompletionSource = new UniTaskCompletionSource<bool>();
            
            gameObject.SetActive(true);
            
            _group.alpha = 0;

            await DoAnimation(1, showAnimationDuration);
        }

        public virtual async UniTask Hide()
        {
            await DoAnimation(0, hideAnimationDuration);

            gameObject.SetActive(false);

            HideCompletionSource.TrySetResult(true);
            ShowCompletionSource = new UniTaskCompletionSource<bool>();
        }

        public virtual bool IsNeedToOpen() => true;

        public abstract UniTask Focus();

        public abstract UniTask Blur();
    }
}