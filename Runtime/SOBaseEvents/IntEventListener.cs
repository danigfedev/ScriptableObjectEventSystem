using UnityEngine;
using UnityEngine.Events;

public class IntEventListener : MonoBehaviour
{
    [System.Serializable]
    public class CustomUnityEvent : UnityEvent<int> { } //ARGUMENT_TYPE_LIST -> Type1, Type2, Type3...

    [SerializeField] private IntEventScriptableObject IntEventScriptableObject;
    [SerializeField] private CustomUnityEvent response;

    void OnEnable()
    {
        IntEventScriptableObject.AddListener(this);
    }

    void OnDisable()
    {
        IntEventScriptableObject.RemoveListener(this);
    }

    public void RiseEvent(int arg1) //Type1 arg1, Type2 arg2, Type3 arg3...
    {
        response.Invoke(arg1); //arg1, arg2, arg3...
    }
}
