using UnityEngine;
using UnityEngine.Events;

namespace SOBaseEvents
{
    public class FloatEventListener : MonoBehaviour
    {
        [System.Serializable]
        public class CustomUnityEvent : UnityEvent<float> { } //ARGUMENT_TYPE_LIST -> Type1, Type2, Type3...

        [SerializeField] private FloatEventScriptableObject FloatEventScriptableObject;
        [SerializeField] private CustomUnityEvent response;

        void OnEnable()
        {
            FloatEventScriptableObject.AddListener(this);
        }

        void OnDisable()
        {
            FloatEventScriptableObject.RemoveListener(this);
        }

        public void RiseEvent(float arg1) //Type1 arg1, Type2 arg2, Type3 arg3...
        {
            response.Invoke(arg1); //arg1, arg2, arg3...
        }
    }
}