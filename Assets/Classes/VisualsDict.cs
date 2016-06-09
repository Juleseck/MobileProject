using UnityEngine;
using UnityEngine.UI;
using System;
using Assets.Enums;

namespace Assets.Classes
{
    [Serializable]
    public class VisualsDict
    {

        [SerializeField]
        public Visuals_Enum VisualTheme;

        [SerializeField]
        public Texture[] Textures;
    }
}
