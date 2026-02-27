using UnityEngine;
using UnityEngine.Events;

namespace SOBaseEvents
{
    public class StringEventListener : MonoBehaviour
    {
        [System.Serializable]
        public class CustomUnityEvent : UnityEvent<string> { } //ARGUMENT_TYPE_LIST -> Type1, Type2, Type3...

        [SerializeField] private StringEventScriptableObject StringEventScriptableObject;
        [SerializeField] private CustomUnityEvent response;

        void OnEnable()
        {
            StringEventScriptableObject.AddListener(this);
        }

        void OnDisable()
        {
            StringEventScriptableObject.RemoveListener(this);
        }

        public void RiseEvent(string arg1) //Type1 arg1, Type2 arg2, Type3 arg3...
        {
            response.Invoke(arg1); //arg1, arg2, arg3...
        }
    }
}