#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor;

namespace Navigator.Core
{
    public partial class NavigatorConfig
    {
        private void OnValidate()
        {
            ForceUpdate = true;
        }

        private static bool ForceUpdate;

        private static IEnumerable DropdownOptions { get; set; }

        private static void UpdateDropdownOptions<T>()
        {
            DropdownOptions = AssetDatabase.FindAssets(nameof(NavigatorConfig) + " t:ScriptableObject")
                                           .Select(AssetDatabase.GUIDToAssetPath)
                                           .Select(AssetDatabase.LoadAssetAtPath<NavigatorConfig>)
                                           .SelectMany(GetItems<T>);
        }

        private static IEnumerable<ValueDropdownItem> GetItems<T>(NavigatorConfig asset)
        {
            return asset.Screens.Where(item => item is T).Select((item) => new ValueDropdownItem(item.name, item.name));
        }

        public static IEnumerable GetDropdownOptions<T>()
        {
            if (!EditorApplication.isCompiling && (DropdownOptions == null || ForceUpdate))
            {
                ForceUpdate = false;
                UpdateDropdownOptions<T>();
            }

            return DropdownOptions;
        }
    }
}
#endif