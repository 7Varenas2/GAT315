using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Events/EventRouter")]
public class EventRouter : ScriptableObject
{
    // Allow observers to subscribe on event (onEvent +=)
    public UnityAction onEvent;

    public void Notify()
    {
        onEvent?.Invoke();
    }
}
