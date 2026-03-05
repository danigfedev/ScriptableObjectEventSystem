using SOBaseEvents;
using UnityEngine;
using Samples.Event_Testing_Sample.Scripts;

[CreateAssetMenu(fileName ="StringCustomTypeEventScriptableObject", menuName ="EspidiGames/SO Events/StringCustomTypeEventScriptableObject", order = 20)]
public class StringCustomTypeEventScriptableObject : EventSOBase<string, CustomType>
{

}