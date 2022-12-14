#if UNITY_EDITOR
using System.Collections;
using ExampleScreens;
using Sirenix.OdinInspector;
using UnityEngine;

#pragma warning disable CS4014

namespace Navigator.Core
{
    public partial class Navigator
    {
        [ValueDropdown("GetScreens")] 
        public string targetScreen;

        [Space, Header("Open Screen Settings")] 
        public bool isPopupTarget;
        public bool isAllowedMultiOperationTarget;
        
        private Screen TargetScreen => Screens.Find(s => s.name == targetScreen);

        [Button]
        private void Replace() => Replace(TargetScreen);

        [Button]
        private void Open() => Open(TargetScreen, isPopupTarget, isAllowedMultiOperation:isAllowedMultiOperationTarget);

        [Button]
        private void CloseScreen() => Close();

        [Button]
        private void OpenIsNeeded() => OpenIsNeeded(TargetScreen, isPopupTarget);

        [Button]
        private void AddedToNextScreen() => AddedToNextScreen(TargetScreen);

        [Button]
        private void AddedToNextScreenIsNeeded() => AddedToNextScreenIsNeeded(TargetScreen);

        [Button]
        private void ReplaceIsNeeded() => ReplaceIsNeeded(TargetScreen);

        
        [ButtonGroup("Temporary"),Button]
        private void OpenGreen() => Open<GreenScreen>(isPopup : isPopupTarget, isAllowedMultiOperation: isAllowedMultiOperationTarget);
        
        [ButtonGroup("Temporary"),Button]
        private void OpenRed() => Open<RedScreen>(isPopup : isPopupTarget, isAllowedMultiOperation: isAllowedMultiOperationTarget);
        
        [ButtonGroup("Temporary"),Button]
        private void OpenBlue() => Open<BlueScreen>(isPopup : isPopupTarget, isAllowedMultiOperation: isAllowedMultiOperationTarget);

        private IEnumerable GetScreens => NavigatorConfig.GetDropdownOptions<Screen>();
    }
}

#endif

