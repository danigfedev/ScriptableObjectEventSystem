using UnityEngine;
using UnityEngine.Events;

namespace SOBaseEvents
{
    public class NoArgsEventListener : MonoBehaviour
    {
        [SerializeField] private NoArgsEventScriptableObject NoArgsEventScriptableObject;
        [SerializeField] private UnityEvent response;

        void OnEnable()
        {
            NoArgsEventScriptableObject.AddListener(this);
        }

        void OnDisable()
        {
            NoArgsEventScriptableObject.RemoveListener(this);
        }

        public void RiseEvent()
        {
            response.Invoke();
        }
    }
}