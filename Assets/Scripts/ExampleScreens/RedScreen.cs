using Cysharp.Threading.Tasks;
using Navigator;
using UnityEngine;
using UnityEngine.UI;
using Screen = Navigator.Screen;

namespace ExampleScreens
{
    public class RedScreen : Screen
    {
        [SerializeField] private Button _button;
        
        public override UniTask Focus()
        {
            Debug.Log("Red Focus");
            _button.onClick.AddListener(ClickHandler);
            
            return UniTask.CompletedTask;
        }

        public override UniTask Blur()
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