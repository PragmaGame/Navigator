using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Navigator
{
    public partial class Navigator : MonoBehaviour
    {
        private readonly List<Screen> _screenInstances = new();
        private readonly Stack<StackItem> _stack = new();
        
        private readonly Queue<Screen> _nextScreens = new();
        private readonly List<WaitScreenItem> _waitScreen = new();

        [SerializeField] private NavigatorConfig config;

        private DiContainer _diContainer;

        private Screen Current => _stack.Count > 0 ? _stack.Peek().screen : null;
        private List<Screen> Screens => config.Screens;
        private int Count => _stack.Count;

        public string CurrentScreenName => Current.name;

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

        private T CreateScreen<T>(T prefab) where T : Screen
        {
            var screen = _diContainer.InstantiatePrefabForComponent<Screen>(prefab, transform);
            screen.Navigator = this;
            _screenInstances.Add(screen);

            return (T)screen;
        }

        private T GetScreen<T>(T screen = null) where T : Screen
        {
            if (screen != null)
            {
                return (T)GetScreenByInstance(screen);
            }

            return GetScreenByType<T>();
        }

        private Screen GetScreenByInstance(Screen screen)
        {
            if (_screenInstances.Contains(screen))
            {
                return screen;
            }

            var type = screen.GetType();

            var instance = _screenInstances.Find(s => s.GetType() == type);

            return instance != null ? instance : CreateScreen(screen);
        }

        private T GetScreenByType<T>() where T : Screen
        {
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

        private void AddScreenToStack(Screen screen)
        {
            var key = Guid.NewGuid().ToString();
            var item = new StackItem(key, screen);

            var rectTransform = item.screen.GetComponent<RectTransform>();
            rectTransform.SetSiblingIndex(Count);

            _stack.Push(item);
        }

        public UniTask<T> Open<T>(string screenName, bool isPopup = false) where T : Screen
        {
            var screen = Screens.Find(s => s.name == screenName);
        
            return Open((T) screen, isPopup);
        }

        public async UniTask<T> Open<T>(T screen = null, bool isPopup = false) where T : Screen
        {
            screen = GetScreen(screen);

            if (Current == screen)
            {
                return (T)Current;
            }

            screen.IsPopup = isPopup;
            
            var prevScreen = Current;

            AddScreenToStack(screen);

            if (prevScreen != null)
            {
                await prevScreen.Blur();
            }

            await screen.Show();
            
            await screen.Focus();
            
            SendWaitScreen(WaitScreenType.Open, screen);

            return screen;
        }

        public async UniTask Close() => await Close(true);

        public async UniTask Close(bool isTryOpenNextScreen)
        {
            if (!_stack.TryPop(out var item))
            {
                return;
            }

            var currentScreen = item.screen;
            
            await currentScreen.Blur();

            if (isTryOpenNextScreen && !currentScreen.IsPopup && _nextScreens.TryDequeue(out var nextScreen))
            {
                if (currentScreen.IsPermissionOverlapOnHide && nextScreen.IsPermissionOverlapOnShow)
                {
                    SendWaitScreen(WaitScreenType.Close, currentScreen);
                    await UniTask.WhenAll(currentScreen.Hide(), Open(nextScreen));
                }
                else
                {
                    await currentScreen.Hide();
                    SendWaitScreen(WaitScreenType.Close, currentScreen);
                    await Open(nextScreen);
                }
                
                return;
            }

            SendWaitScreen(WaitScreenType.Close, currentScreen);
            await currentScreen.Hide();

            if(Current != null)
                await Current.Focus();
        }

        public async UniTask<T> Replace<T>(T screen = null, bool isPopup = false) where T : Screen
        {
            await Close(false);
            
            return await Open(screen, isPopup);
        }

        public UniTask<T> OpenIsNeeded<T>(T screen = null, bool isPopup = false) where T : Screen
        {
            screen = GetScreen(screen);

            screen.IsPopup = isPopup;

            return screen.IsNeedToOpen() ? Open(screen) : UniTask.FromResult<T>(null);
        }
        
        public UniTask<T> ReplaceIsNeeded<T>(T screen = null) where T : Screen
        {
            screen = GetScreen<T>(screen);
            
            return screen.IsNeedToOpen() ? Replace<T>() : UniTask.FromResult<T>(null);
        }
        
        public void AddedToNextScreenIsNeeded<T>(T screen = null) where T : Screen
        {
            screen = GetScreen<T>(screen);

            if (screen.IsNeedToOpen())
            {
                AddedToNextScreen(screen);
            }
        }
        
        public void AddedToNextScreen<T>(T screen = null) where T : Screen
        {
            screen = GetScreen<T>(screen);
            
            if (Current == null)
            {
                Open(screen);
                return;
            }
                
            _nextScreens.Enqueue(screen);
        }

        public void SendWaitScreen<T>(WaitScreenType waitType, T screen = null) where T : Screen
        {
            screen = GetScreen(screen);
            
            var waitScreenItem = _waitScreen.Find(item => item.screen == screen && item.type == waitType);

            if (waitScreenItem == null)
            {
                return;
            }

            waitScreenItem.waitCompletionSource.TrySetResult();

            _waitScreen.Remove(waitScreenItem);
        }

        public UniTask WaitScreen<T>(T screen = null, WaitScreenType waitType = WaitScreenType.Open) where T : Screen
        {
            screen = GetScreen(screen);

            var waitScreenItem = _waitScreen.Find(item => item.screen == screen && item.type == waitType);

            if (waitScreenItem == null)
            {
                var source = new UniTaskCompletionSource();
                
                _waitScreen.Add(new WaitScreenItem()
                {
                    waitCompletionSource = source,
                    screen = screen,
                });

                return source.Task;
            }

            return waitScreenItem.waitCompletionSource.Task;
        }
    }
}