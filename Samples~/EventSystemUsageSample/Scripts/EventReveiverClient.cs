using Samples.Event_Testing_Sample.Scripts;
using SOBaseEvents.Impl;
using UnityEngine;

public class EventReveiverClient : MonoBehaviour
{
    [Space]
    [Header("No Arg events")]
    private NoArgsEventListener _noArgsEventListener;
    [Space]
    [Header("1 Arg events")]
    private IntEventListener _intArgsListener;
    private FloatEventListener _floatArgsListener;
    private BooleanEventListener _boolArgsListener;
    private StringEventListener _stringArgsListener;
    private CustomTypeEventListener customTypeArgsListener;
    [Space]
    [Header("2 Arg events")]
    private StringCustomTypeEventListener _stringCustomTypeArgsListener;
    [Space]
    [Header("3 Arg events")]
    private IntFloatStringEventListener _intFloatStringArgsListener;

    private void Awake()
    {
        _noArgsEventListener = GetComponent<NoArgsEventListener>();
        _intArgsListener = GetComponent<IntEventListener>();
        _floatArgsListener = GetComponent<FloatEventListener>();
        _boolArgsListener = GetComponent<BooleanEventListener>();
        _stringArgsListener = GetComponent<StringEventListener>();
        customTypeArgsListener = GetComponent<CustomTypeEventListener>();
        _stringCustomTypeArgsListener = GetComponent<StringCustomTypeEventListener>();
        _intFloatStringArgsListener = GetComponent<IntFloatStringEventListener>();
    }
    
    private void OnEnable()
    {
        _noArgsEventListener.OnEventRaised += OnNoArgsEventRaised;
        
        _intArgsListener.OnEventRaised += OnIntArgsEventRaised;
        _floatArgsListener.OnEventRaised += OnFloatArgsEventRaised;
        _boolArgsListener.OnEventRaised += OnBoolArgsEventRaised;
        _stringArgsListener.OnEventRaised += OnStringArgsEventRaised;
        customTypeArgsListener.OnEventRaised += OnCustomTypeArgsEventRaised;
        
        _stringCustomTypeArgsListener.OnEventRaised += OnStringCustomTypeArgsEventRaised;
        
        _intFloatStringArgsListener.OnEventRaised += OnIntFloatStringArgsEventRaised;
    }
    
    private void OnDisable()
    {
        _noArgsEventListener.OnEventRaised -= OnNoArgsEventRaised;
        
        _intArgsListener.OnEventRaised -= OnIntArgsEventRaised;
        _floatArgsListener.OnEventRaised -= OnFloatArgsEventRaised;
        _boolArgsListener.OnEventRaised -= OnBoolArgsEventRaised;
        _stringArgsListener.OnEventRaised -= OnStringArgsEventRaised;
        customTypeArgsListener.OnEventRaised -= OnCustomTypeArgsEventRaised;
        
        _stringCustomTypeArgsListener.OnEventRaised -= OnStringCustomTypeArgsEventRaised;
        
        _intFloatStringArgsListener.OnEventRaised -= OnIntFloatStringArgsEventRaised;
    }

    private void OnIntFloatStringArgsEventRaised(int arg1, float arg2, string arg3)
    {
        Debug.Log("** Int, Float and String Args Event Raised with values: " + arg1 + ", " + arg2 + " and " + arg3 + " **");
    }

    private void OnStringCustomTypeArgsEventRaised(string arg1, CustomType arg2)
    {
        Debug.Log("** String and SongData Args Event Raised with values: " + arg1 + " and " + arg2.Id + " by " + arg2.Value + " **");
    }

    private void OnCustomTypeArgsEventRaised(CustomType obj)
    {
        Debug.Log("** SongData Args Event Raised with value: " + obj.Id + " by " + obj.Value + " **");
    }

    private void OnStringArgsEventRaised(string obj)
    {
        Debug.Log("** String Args Event Raised with value: " + obj + " **");
    }

    private void OnBoolArgsEventRaised(bool obj)
    {
        Debug.Log("** Bool Args Event Raised with value: " + obj + " **");
    }

    private void OnFloatArgsEventRaised(float value)
    {
        Debug.Log("** Float Args Event Raised with value: " + value + " **");
    }

    private void OnNoArgsEventRaised()
    {
        Debug.Log("** No Args Event Raised **");
    }
    
    private void OnIntArgsEventRaised(int value)
    {
        Debug.Log("** Int Args Event Raised with value: " + value + " **");
    }
}
