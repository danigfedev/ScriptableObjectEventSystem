using UnityEngine;
using UnityEngine.Events;

public class BooleanEventListener : MonoBehaviour
{
    [System.Serializable]
    public class CustomUnityEvent : UnityEvent<bool> { } //ARGUMENT_TYPE_LIST -> Type1, Type2, Type3...

    [SerializeField] private BooleanEventScriptableObject BooleanEventScriptableObject;
    [SerializeField] private CustomUnityEvent response;

    void OnEnable()
    {
        BooleanEventScriptableObject.AddListener(this);
    }

    void OnDisable()
    {
        BooleanEventScriptableObject.RemoveListener(this);
    }

    public void RiseEvent(bool arg1) //Type1 arg1, Type2 arg2, Type3 arg3...
    {
        response.Invoke(arg1); //arg1, arg2, arg3...
    }
}
