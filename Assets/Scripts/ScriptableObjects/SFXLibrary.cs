using AYellowpaper.SerializedCollections;
using DefaultNamespace.Enums;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "SFXLibrary", menuName = "Audio/SFX Library", order = 0)]
    public class SFXLibrary : ScriptableObject
    {
        [SerializedDictionary("SFX Type", "Audio Clip")]
        public SerializedDictionary<SFXType, AudioClip> clips;
        
        public bool TryGetClip(SFXType type, out AudioClip clip)
        {
            return clips.TryGetValue(type, out clip);
        }
    }
}