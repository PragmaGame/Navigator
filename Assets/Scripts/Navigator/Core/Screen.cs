using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace Navigator.Core
{
    [RequireComponent(typeof(Canvas), typeof(CanvasGroup))]
    [RequireComponent(typeof(GraphicRaycaster), typeof(RectTransform))]
    public abstract class Screen : SerializedMonoBehaviour, IScreen
    {
        [SerializeField] private bool _lazyLoad;

        [NonSerialized, OdinSerialize] private ScreenAnimationTurntable _screenShowAnimationTurntable = new();
        [NonSerialized, OdinSerialize] private ScreenAnimationTurntable _screenHideAnimationTurntable = new();

        private IBlurHandler[] _blurHandlers;
        private IFocusHandler[] _focusHandlers;
        private IShowHandler[] _showHandlers;
        private IShowCompletedHandler[] _showCompletedHandlers;
        private IHideHandler[] _hideHandlers;
        private IHideCompletedHandler[] _hideCompletedHandlers;

        public bool IsPermissionOverlapOnShow => _screenShowAnimationTurntable.IsPermissionOverlap;
        public bool IsPermissionOverlapOnHide => _screenHideAnimationTurntable.IsPermissionOverlap;
        
        public UniTaskCompletionSource<bool> ShowCompletionSource { get; private set; }
        public UniTaskCompletionSource<bool> HideCompletionSource { get; private set; }
        
        public bool IsPopup { get; set; }
        public bool IsLazyLoad => _lazyLoad;
        public Navigator Navigator { get; set; }

        private async UniTask DoAnimation(
            ScreenAnimationTurntable screenAnimationTurntable, 
            ScreenAnimationBlockData screenAnimationBlockData = null, 
            CancellationToken cancellationToken = default)
        {
            await screenAnimationTurntable.PlayAnimations(cancellationToken, screenAnimationBlockData);
        }

        protected virtual void Awake()
        {
            ShowCompletionSource = new UniTaskCompletionSource<bool>();
            HideCompletionSource = new UniTaskCompletionSource<bool>();

            _screenHideAnimationTurntable.SetAnimationObject(transform);
            _screenShowAnimationTurntable.SetAnimationObject(transform);
            
            CollectHandlers();
        }

        private void CollectHandlers()
        {
            _blurHandlers = GetComponentsInChildren<IBlurHandler>();
            _focusHandlers = GetComponentsInChildren<IFocusHandler>();
            _hideHandlers = GetComponentsInChildren<IHideHandler>();
            _hideCompletedHandlers = GetComponentsInChildren<IHideCompletedHandler>();
            _showHandlers = GetComponentsInChildren<IShowHandler>();
            _showCompletedHandlers = GetComponentsInChildren<IShowCompletedHandler>();
        }

        public virtual async UniTask Show(CancellationToken token = default, ScreenAnimationBlockData screenAnimationBlockData = null)
        {
            ShowCompletionSource.TrySetResult(true);
            HideCompletionSource = new UniTaskCompletionSource<bool>();
            
            gameObject.SetActive(true);

            await OnShow(token);

            _showHandlers.ForEach(x => x.OnShow());

            await DoAnimation(_screenShowAnimationTurntable, screenAnimationBlockData, token);

            await OnShowCompleted(token);

            _showCompletedHandlers.ForEach(x => x.OnShowCompleted());
        }

        public virtual async UniTask Hide(CancellationToken token = default, ScreenAnimationBlockData screenAnimationBlockData = null)
        {
            await OnHide(token);

            _hideHandlers.ForEach(x => x.OnHide());
            
            await DoAnimation(_screenHideAnimationTurntable, screenAnimationBlockData, token);

            await OnHideCompleted(token);

            _hideCompletedHandlers.ForEach(x => x.OnHideCompleted());

            gameObject.SetActive(false);

            HideCompletionSource.TrySetResult(true);
            ShowCompletionSource = new UniTaskCompletionSource<bool>();
        }
        
        public async UniTask Focus(CancellationToken token = default)
        {
            await OnFocus(token);

            _focusHandlers.ForEach(x => x.OnFocus());
        }

        public async UniTask Blur(CancellationToken token = default)
        {
            await OnBlur(token);

            _blurHandlers.ForEach(x => x.OnBlur());
        }

        public virtual bool IsNeedToOpen() => true;
        
        protected virtual UniTask OnFocus(CancellationToken token){return UniTask.CompletedTask;}
        protected virtual UniTask OnBlur(CancellationToken token){return UniTask.CompletedTask;}
        protected virtual UniTask OnShow(CancellationToken token){return UniTask.CompletedTask;}
        protected virtual UniTask OnShowCompleted(CancellationToken token){return UniTask.CompletedTask;}
        protected virtual UniTask OnHide(CancellationToken token){return UniTask.CompletedTask;}
        protected virtual UniTask OnHideCompleted(CancellationToken token){return UniTask.CompletedTask;}
    }
}