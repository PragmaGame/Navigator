using Cysharp.Threading.Tasks;
using Navigator;
using UnityEngine;
using UnityEngine.UI;

namespace ExampleScreens
{
    public class RedScreen : BaseScreen
    {
        [SerializeField] private Button _button;
        
        public override async UniTask Focus()
        {
            Debug.Log("Red Focus");
            _button.onClick.AddListener(ClickHandler);
        }

        public override async UniTask Blur()
        {
            Debug.Log("Red Blur");
            _button.onClick.RemoveListener(ClickHandler);
        }

        private void ClickHandler()
        {
            Debug.Log($"Click RedScreen");
        }
    }
}