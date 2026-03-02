using UnityEngine;

namespace SOBaseEvents.Refactor.Impl
{
    public abstract class EventListenerBase : MonoBehaviour
    {
        [SerializeField] protected ScriptableObject _baseEvent;
        
        public ScriptableObject EventAsset => _baseEvent; // To be used by the tool
    }
}