using System.Threading;
using Cysharp.Threading.Tasks;
using Navigator;
using UnityEngine;
using UnityEngine.UI;
using Screen = Navigator.Core.Screen;

namespace ExampleScreens
{
    public class RedScreen : Navigator.Core.Screen
    {
        [SerializeField] private Button _button;
        
        protected override UniTask OnFocus(CancellationToken token)
        {
            Debug.Log("Red Focus");
            _button.onClick.AddListener(ClickHandler);
            
            return UniTask.CompletedTask;
        }

        protected override UniTask OnBlur(CancellationToken token)
        {
            Debug.Log("Red Blur");
            _button.onClick.RemoveListener(ClickHandler);
            
            return UniTask.CompletedTask;
        }

        private void ClickHandler()
        {
            Debug.Log($"Click RedScreen");
        }
    }
}