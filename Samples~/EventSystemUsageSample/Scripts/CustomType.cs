using UnityEngine;

namespace Samples.Event_Testing_Sample.Scripts
{
    [CreateAssetMenu(fileName = "CustomType", menuName = "Scriptable Objects/CustomType")]
    public class CustomType : ScriptableObject
    {
        public string Id;
        public int Value;
    }
}
