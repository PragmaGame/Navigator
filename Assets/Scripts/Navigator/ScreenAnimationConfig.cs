using System;
using Sirenix.Serialization;
using UnityEngine;

namespace Navigator
{
    [Serializable]
    public sealed class ScreenAnimationConfig
    {
        [SerializeField] private bool _isPermissionOverlap;
        [OdinSerialize] private IScreenAnimation[] animations;

        public bool IsPermissionOverlap => _isPermissionOverlap;

        public IScreenAnimation[] Animations => animations;
    }
}