using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector.Editor.Drawers;
using UnityEngine;
using Zenject;

namespace Navigator
{
    public partial class Navigator : MonoBehaviour
    {
        private readonly List<BaseScreen> _screenInstances = new List<BaseScreen>();
        private readonly Stack<StackItem> _stack = new Stack<StackItem>();
        
        private readonly Queue<BaseScreen> _nextScreens = new Queue<BaseScreen>();

        [SerializeField] private NavigatorConfig config;

        private DiContainer _diContainer;

        private StackItem Current => _stack.Count > 0 ? _stack.Peek() : null;
        private List<BaseScreen> Screens => config.Screens;
        private int Count => _stack.Count;

        public string CurrentScreenName => Current.screen.name;

        [Inject]
        private void Construct(DiContainer container)
        {
            _diContainer = container;
        }

        private void Awake()
        {
            Screens.ForEach(s =>
            {
                if (s.LazyLoad) return;

                CreateScreen(s);
            });

            _screenInstances.ForEach(i => i.gameObject.SetActive(false));
        }

        private T CreateScreen<T>(T prefab) where T : BaseScreen
        {
            var screen = _diContainer.InstantiatePrefabForComponent<BaseScreen>(prefab, transform);
            screen.Navigator = this;
            _screenInstances.Add(screen);

            return (T)screen;
        }

        private T GetScreen<T>(T screen = null) where T : BaseScreen
        {
            Debug.Log(typeof(T));
            
            if (screen != null)
            {
                if (_screenInstances.Contains(screen))
                {
                    return screen;
                }

                var type = screen.GetType();

                var instance = _screenInstances.Find(s => s.GetType() == type);

                if (instance != null)
                {
                    return (T)instance;
                }

                return CreateScreen(screen);
            }

            var screenInstance = _screenInstances.Find(s => s is T);

            if (screenInstance == null)
            {
                var screenPrefab = Screens.Find(s => s is T);

                if (screenPrefab != null)
                {
                    screenInstance = CreateScreen(screenPrefab);
                }
                else
                    Debug.LogError("Screen instance " + typeof(T) + " can't created!");
            }

            return (T) screenInstance;
        }

        private void AddScreenToStack(BaseScreen screen)
        {
            var key = Guid.NewGuid().ToString();
            var item = new StackItem(key, screen);

            var rectTransform = item.screen.GetComponent<RectTransform>();
            rectTransform.SetSiblingIndex(Count);

            _stack.Push(item);
        }

        public UniTask<T> Open<T>(string screenName, bool isPopup = false) where T : BaseScreen
        {
            var screen = Screens.Find(s => s.name == screenName);
        
            return Open((T) screen, isPopup);
        }

        public async UniTask<T> Open<T>(T screen = null, bool isPopup = false) where T : BaseScreen
        {
            screen = GetScreen(screen);

            if (Current?.screen == screen)
            {
                return (T)Current.screen;
            }

            screen.IsPopup = isPopup;
            
            var prevScreen = Current;

            AddScreenToStack(screen);

            if(prevScreen != null)
                await prevScreen.screen.Blur();
                
            await screen.Show();
            
            await screen.Focus();

            return screen;
        }

        public async UniTask Close()
        {
            var stackItem = _stack.Pop();

            await stackItem.screen.Hide();
            
            await stackItem.screen.Blur();

            if (!stackItem.screen.IsPopup && await TryOpenNextScreen())
            {
                return;
            }

            if(Current != null)
                await Current.screen.Focus();
        }
        
        private async UniTask<bool> TryOpenNextScreen()
        {
            if (_nextScreens.Count > 0)
            {
                await Open(_nextScreens.Dequeue());

                return true;
            }

            return false;
        }

        public async UniTask<T> Replace<T>(T screen = null) where T : BaseScreen
        {
            if (Current == null)
            {
                return await Open(screen);
            }
            
            if (Current?.screen is T)
            {
                await Current.screen.Focus();
                return (T) Current.screen;
            }

            var replacedItem = _stack.Pop();
            
            await replacedItem.screen.Hide();
            
            await replacedItem.screen.Blur();

            screen = GetScreen<T>();

            AddScreenToStack(screen);

            await screen.Show();

            await screen.Focus();
            
            return (T) screen;
        }

        public UniTask<T> OpenIsNeeded<T>(T screen = null, bool isPopup = false) where T : BaseScreen
        {
            screen = GetScreen(screen);

            screen.IsPopup = isPopup;

            return screen.IsNeedToOpen() ? Open(screen) : UniTask.FromResult<T>(null);
        }
        
        public UniTask<T> ReplaceIsNeeded<T>(T screen = null) where T : BaseScreen
        {
            screen = GetScreen<T>(screen);
            
            return screen.IsNeedToOpen() ? Replace<T>() : UniTask.FromResult<T>(null);
        }
        
        public void AddedToNextScreenIsNeeded<T>(T screen = null) where T : BaseScreen
        {
            screen = GetScreen<T>(screen);

            if (screen.IsNeedToOpen())
            {
                AddedToNextScreen(screen);
            }
        }
        
        public void AddedToNextScreen<T>(T screen = null) where T : BaseScreen
        {
            screen = GetScreen<T>(screen);
            
            if (Current == null)
            {
                Open(screen);
                return;
            }
                
            _nextScreens.Enqueue(screen);
        }
        
        public UniTaskCompletionSource<bool> WaitOpenScreen<T>(T screen = null) where T : BaseScreen
        {
            return GetScreen(screen).ShowCompletionSource;
        }
        
        public UniTaskCompletionSource<bool> WaitCloseScreen<T>(T screen = null) where T : BaseScreen
        {
            return GetScreen(screen).HideCompletionSource;
        }
    }
}