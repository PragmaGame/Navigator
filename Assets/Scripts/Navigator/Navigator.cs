using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Navigator
{
    public partial class Navigator : MonoBehaviour
    {
        private readonly List<Screen> _screenInstances = new List<Screen>();
        private readonly Stack<StackItem> _stack = new Stack<StackItem>();
        
        private readonly Queue<Screen> _nextScreens = new Queue<Screen>();

        [SerializeField] private NavigatorConfig config;

        private DiContainer _diContainer;

        private StackItem Current => _stack.Count > 0 ? _stack.Peek() : null;
        private List<Screen> Screens => config.Screens;
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

            if (Current?.screen == screen)
            {
                return (T)Current.screen;
            }

            screen.IsPopup = isPopup;
            
            var prevScreen = Current;

            AddScreenToStack(screen);

            if (prevScreen != null)
            {
                await prevScreen.screen.Blur();
            }

            await screen.Show();
            
            await screen.Focus();

            return screen;
        }

        public async UniTask Close()
        {
            if (!_stack.TryPop(out var item))
            {
                return;
            }

            var currentScreen = item.screen;
            
            await currentScreen.Blur();

            if (!currentScreen.IsPopup && _nextScreens.TryDequeue(out var nextScreen))
            {
                if (currentScreen.IsPermissionOverlapOnHide && nextScreen.IsPermissionOverlapOnShow)
                {
                    await UniTask.WhenAll(currentScreen.Hide(), Open(nextScreen));
                }
                else
                {
                    await currentScreen.Hide();
                    await Open(nextScreen);
                }
                
                return;
            }

            await currentScreen.Hide();

            if(Current != null)
                await Current.screen.Focus();
        }

        public async UniTask<T> Replace<T>(T screen = null) where T : Screen
        {
            screen = GetScreen(screen);
            
            if (Current == null)
            {
                return await Open(screen);
            }

            if (Current?.screen == screen)
            {
                await Current.screen.Focus();
                return (T) Current.screen;
            }

            var replacedItem = _stack.Pop();

            var replacedScreen = replacedItem.screen;
            
            await replacedScreen.Blur();

            AddScreenToStack(screen);

            if (replacedScreen.IsPermissionOverlapOnHide && screen.IsPermissionOverlapOnShow)
            {
                UniTask.WhenAll(replacedScreen.Hide(), screen.Show());
            }
            else
            {
                await replacedScreen.Hide();
                await screen.Show();
            }
            
            await screen.Focus();
            
            return (T) screen;
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
        
        public UniTaskCompletionSource<bool> WaitOpenScreen<T>(T screen = null) where T : Screen
        {
            return GetScreen(screen).ShowCompletionSource;
        }
        
        public UniTaskCompletionSource<bool> WaitCloseScreen<T>(T screen = null) where T : Screen
        {
            return GetScreen(screen).HideCompletionSource;
        }
    }
}