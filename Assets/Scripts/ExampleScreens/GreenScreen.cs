using System.Threading;
using Cysharp.Threading.Tasks;
using Navigator;
using UnityEngine;
using UnityEngine.UI;
using Screen = Navigator.Core.Screen;

namespace ExampleScreens
{
    public class GreenScreen : Navigator.Core.Screen
    {
        [SerializeField] private Button _button;
        
        protected override UniTask OnFocus(CancellationToken token)
        {
            Debug.Log("Green Focus");
           _button.onClick.AddListener(ClickHandler);
           
           return UniTask.CompletedTask;
        }

        protected override UniTask OnBlur(CancellationToken token)
        {
            Debug.Log("Green Blur");
            _button.onClick.RemoveListener(ClickHandler);
            
            return UniTask.CompletedTask;
        }

        private void ClickHandler()
        {
            Debug.Log($"Click GreenScreen");
        }
    }
}