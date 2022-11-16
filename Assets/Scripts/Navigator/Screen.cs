using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Navigator
{
    [RequireComponent(typeof(Canvas), typeof(CanvasGroup), typeof(RectTransform))]
    public abstract class Screen : MonoBehaviour, IScreen
    {
        [SerializeField] private bool _lazyLoad;

        [SerializeReference] private IScreenAnimation[] _screenShowAnimations;
        [SerializeReference] private IScreenAnimation[] _screenHideAnimations;
        
        [field:SerializeField] public bool IsPermissionOverlapOnShow { get; private set; }
        [field:SerializeField] public bool IsPermissionOverlapOnHide { get; private set; }
        
        public UniTaskCompletionSource<bool> ShowCompletionSource { get; private set; }
        public UniTaskCompletionSource<bool> HideCompletionSource { get; private set; }
        
        public bool IsPopup { get; set; }
        public bool LazyLoad => _lazyLoad;

        public Navigator Navigator { get; set; }

        private async UniTask DoShowAnimation(CancellationToken cancellationToken = default) =>
            await UniTask.WhenAll(_screenShowAnimations.Select(screen => screen.DoAnimation(cancellationToken)));


        private async UniTask DoHideAnimation(CancellationToken cancellationToken = default) =>
            await UniTask.WhenAll(_screenHideAnimations.Select(screen => screen.DoAnimation(cancellationToken)));


        private void Awake()
        {
            ShowCompletionSource = new UniTaskCompletionSource<bool>();
            HideCompletionSource = new UniTaskCompletionSource<bool>();
        }

        public virtual async UniTask Show()
        {
            ShowCompletionSource.TrySetResult(true);
            HideCompletionSource = new UniTaskCompletionSource<bool>();
            
            gameObject.SetActive(true);

            await DoShowAnimation();
        }

        public virtual async UniTask Hide()
        {
            await DoHideAnimation();

            gameObject.SetActive(false);

            HideCompletionSource.TrySetResult(true);
            ShowCompletionSource = new UniTaskCompletionSource<bool>();
        }

        public virtual bool IsNeedToOpen() => true;

        public abstract UniTask Focus();

        public abstract UniTask Blur();
    }
}