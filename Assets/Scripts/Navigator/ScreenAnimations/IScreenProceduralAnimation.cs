using UnityEngine;

namespace Navigator
{
    public interface IScreenProceduralAnimation : IScreenAnimation
    {
        public Transform ScreenTransformInject { get; set; }
    }
}