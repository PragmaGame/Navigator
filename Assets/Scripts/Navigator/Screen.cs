using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Navigator
{
    [RequireComponent(typeof(Canvas), typeof(CanvasGroup), typeof(RectTransform))]
    public abstract class Screen : SerializedMonoBehaviour, IScreen
    {
        [SerializeField] private bool _lazyLoad;

        [OdinSerialize, NonSerialized] private ScreenAnimationConfig _screenShowAnimationConfig;
        [OdinSerialize, NonSerialized] private ScreenAnimationConfig _screenHideAnimationConfig;

        public bool IsPermissionOverlapOnShow => _screenShowAnimationConfig.IsPermissionOverlap;
        public bool IsPermissionOverlapOnHide => _screenHideAnimationConfig.IsPermissionOverlap;
        
        public UniTaskCompletionSource<bool> ShowCompletionSource { get; private set; }
        public UniTaskCompletionSource<bool> HideCompletionSource { get; private set; }
        
        public bool IsPopup { get; set; }
        public bool LazyLoad => _lazyLoad;

        public Navigator Navigator { get; set; }

        private async UniTask DoShowAnimation(CancellationToken cancellationToken = default) =>
            await UniTask.WhenAll(_screenShowAnimationConfig.Animations.Select(screen => screen.DoAnimation(cancellationToken)));


        private async UniTask DoHideAnimation(CancellationToken cancellationToken = default) =>
            await UniTask.WhenAll(_screenHideAnimationConfig.Animations.Select(screen => screen.DoAnimation(cancellationToken)));


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