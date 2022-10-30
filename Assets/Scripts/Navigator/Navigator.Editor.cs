#if UNITY_EDITOR
using System.Collections;
using ExampleScreens;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Navigator
{
    public partial class Navigator
    {
        [ValueDropdown("GetScreens")] 
        public string targetScreen;

        [Space, Header("Open Screen Settings")] 
        public bool isPopup;
        
        private BaseScreen TargetScreen => Screens.Find(s => s.name == targetScreen);

        [Button]
        private void Replace() => Replace(TargetScreen);

        [Button]
        private void Open() => Open(TargetScreen, isPopup);

        [Button]
        private void CloseScreen() => Close();

        [Button]
        private void OpenIsNeeded() => OpenIsNeeded(TargetScreen, isPopup);

        [Button]
        private void AddedToNextScreen() => AddedToNextScreen(TargetScreen);

        [Button]
        private void AddedToNextScreenIsNeeded() => AddedToNextScreenIsNeeded(TargetScreen);

        [Button]
        private void ReplaceIsNeeded() => ReplaceIsNeeded(TargetScreen);

        [Button]
        private void OpenGreen() => Open<GreenScreen>();
        
        [Button]
        private void OpenRed() => Open<RedScreen>();
        
        [Button]
        private void OpenBlue() => Open<BlueScreen>();

        private IEnumerable GetScreens => NavigatorConfig.GetDropdownOptions<BaseScreen>();
    }
}

#endif

