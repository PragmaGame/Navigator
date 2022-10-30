using Cysharp.Threading.Tasks;
using Navigator;
using UnityEngine;
using UnityEngine.UI;

namespace ExampleScreens
{
    public class GreenScreen : BaseScreen
    {
        [SerializeField] private Button _button;
        
        public override async UniTask Focus()
        {
            Debug.Log("Green Focus");
           _button.onClick.AddListener(ClickHandler);
        }

        public override async UniTask Blur()
        {
            Debug.Log("Green Blur");
            _button.onClick.RemoveListener(ClickHandler);
        }

        private void ClickHandler()
        {
            Debug.Log($"Click GreenScreen");
        }
    }
}