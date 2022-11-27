using UnityEngine;

namespace Navigator
{
    public interface IScreenProceduralAnimation : IScreenAnimation
    {
        public RectTransform ScreenTransform { get; set; }
    }
}