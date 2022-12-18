using Cysharp.Threading.Tasks;
using Navigator;
using UnityEngine;
using UnityEngine.UI;
using Screen = Navigator.Screen;

namespace ExampleScreens
{
    public class GreenScreen : Screen
    {
        [SerializeField] private Button _button;
        
        protected override UniTask OnFocus()
        {
            Debug.Log("Green Focus");
           _button.onClick.AddListener(ClickHandler);
           
           return UniTask.CompletedTask;
        }

        protected override UniTask OnBlur()
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