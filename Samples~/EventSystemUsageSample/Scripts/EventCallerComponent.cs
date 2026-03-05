using Samples.Event_Testing_Sample.Scripts;
using SOBaseEvents;
using SOBaseEvents.Impl;
using UnityEngine;

public class EventCallerComponent : MonoBehaviour
{
    //Interfaces are not serialized. I would need to use Odin Inspector.
    public ISOEventRaiser _norArgsEvent;
    public ISOEventRaiser<int> _intArgsEvent;
    public ISOEventRaiser<bool> _boolArgsEvent;
    
    [Header("Helper data")]
    public CustomType customType;
    
    [Space]
    [Header("No Arg events")]
    public NoArgsEventScriptableObject _noArgsEvent;
    [Space]
    [Header("1 Arg events")]
    public IntEventScriptableObject _intArgsEventSO;
    public FloatEventScriptableObject _floatArgsEventSO;
    public BooleanEventScriptableObject _boolArgsEventSO;
    public StringEventScriptableObject _stringArgsEventSO;
    public CustomTypeEventScriptableObject customTypeArgsEventSo;
    [Space]
    [Header("2 Arg events")]
    public StringCustomTypeEventScriptableObject _stringCustomTypeArgsEventSO;
    [Space]
    [Header("3 Arg events")]
    public IntFloatStringEventScriptableObject _intFloatStringArgsEventSO;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _noArgsEvent.RaiseEvent();
            
            _intArgsEventSO.RaiseEvent(100);
            _floatArgsEventSO.RaiseEvent(3.14f);
            _boolArgsEventSO.RaiseEvent(true);
            _stringArgsEventSO.RaiseEvent("Hello World");
            customTypeArgsEventSo.RaiseEvent(customType);
            
            _stringCustomTypeArgsEventSO.RaiseEvent("Testing", customType);
            
            _intFloatStringArgsEventSO.RaiseEvent(100, 6.25f, "Testing 3 args");
        }
    }
}
