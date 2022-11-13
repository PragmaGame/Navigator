using System.Collections.Generic;
using UnityEngine;

namespace Navigator
{
    [CreateAssetMenu(fileName = nameof(NavigatorConfig), menuName = "Game/Configs/" + nameof(NavigatorConfig))]
    public partial class NavigatorConfig : ScriptableObject
    {
        [SerializeField] private List<Screen> screens;

        public List<Screen> Screens => screens;
    }
}