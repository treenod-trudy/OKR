#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace PKWork
{
    public class TextureImporterObject : ScriptableObject
    {
        public TextureImporterSettings Settings;
        public TextureImporterPlatformSettings PlatformSettings;
    }
}
#endif