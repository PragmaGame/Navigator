using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Navigator.Core
{
    [CreateAssetMenu(fileName = nameof(NavigatorConfig), menuName = "Game/Configs/" + nameof(NavigatorConfig))]
    public partial class NavigatorConfig : SerializedScriptableObject
    {
        [OdinSerialize] private List<Screen> _screens;

        public List<Screen> Screens => _screens;
    }
}