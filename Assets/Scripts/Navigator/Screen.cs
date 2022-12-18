using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace Navigator
{
    [RequireComponent(typeof(Canvas), typeof(CanvasGroup))]
    [RequireComponent(typeof(GraphicRaycaster), typeof(RectTransform))]
    public abstract class Screen : SerializedMonoBehaviour, IScreen
    {
        [SerializeField] private bool _lazyLoad;

        [OdinSerialize, NonSerialized] private ScreenAnimationTurntable _screenShowAnimationTurntable = new();
        [OdinSerialize, NonSerialized] private ScreenAnimationTurntable _screenHideAnimationTurntable = new();

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
        public bool LazyLoad => _lazyLoad;

        public Navigator Navigator { get; set; }

        private async UniTask DoAnimation(
            ScreenAnimationTurntable screenAnimationTurntable, 
            IScreenProceduralAnimation screenProceduralAnimation = null, 
            CancellationToken cancellationToken = default)
        {
            if (screenProceduralAnimation == null)
            {
                await screenAnimationTurntable.PlayAnimations(cancellationToken);
            }
            else
            {
                screenProceduralAnimation.ScreenTransformInject = gameObject.transform;
                await screenProceduralAnimation.DoAnimation(cancellationToken);
            }
        }

        protected virtual void Awake()
        {
            ShowCompletionSource = new UniTaskCompletionSource<bool>();
            HideCompletionSource = new UniTaskCompletionSource<bool>();
            
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

        public virtual async UniTask Show(IScreenProceduralAnimation screenAnimation = null)
        {
            ShowCompletionSource.TrySetResult(true);
            HideCompletionSource = new UniTaskCompletionSource<bool>();
            
            gameObject.SetActive(true);

            await OnShow();

            _showHandlers.ForEach(x => x.OnShow());

            await DoAnimation(_screenShowAnimationTurntable, screenAnimation);

            await OnShowCompleted();

            _showCompletedHandlers.ForEach(x => x.OnShowCompleted());
        }

        public virtual async UniTask Hide(IScreenProceduralAnimation screenAnimation = null)
        {
            await OnHide();

            _hideHandlers.ForEach(x => x.OnHide());
            
            await DoAnimation(_screenHideAnimationTurntable, screenAnimation);

            await OnHideCompleted();

            _hideCompletedHandlers.ForEach(x => x.OnHideCompleted());

            gameObject.SetActive(false);

            HideCompletionSource.TrySetResult(true);
            ShowCompletionSource = new UniTaskCompletionSource<bool>();
        }
        
        public async UniTask Focus()
        {
            await OnFocus();

            _focusHandlers.ForEach(x => x.OnFocus());
        }

        public async UniTask Blur()
        {
            await OnBlur();

            _blurHandlers.ForEach(x => x.OnBlur());
        }

        public virtual bool IsNeedToOpen() => true;
        
        protected virtual UniTask OnFocus(){return UniTask.CompletedTask;}
        protected virtual UniTask OnBlur(){return UniTask.CompletedTask;}
        protected virtual UniTask OnShow(){return UniTask.CompletedTask;}
        protected virtual UniTask OnShowCompleted(){return UniTask.CompletedTask;}
        protected virtual UniTask OnHide(){return UniTask.CompletedTask;}
        protected virtual UniTask OnHideCompleted(){return UniTask.CompletedTask;}
    }
}