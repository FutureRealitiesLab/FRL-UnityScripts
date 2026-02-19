using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventInvokationKeyCode : MonoBehaviour
{
    public bool EventInvokationKeyCodeActive = true;
    public KeyCode ActivationKey;
    [Space(5)]
    public UnityEvent OnKeyDownEvent;
    [Space(5)]
    public UnityEvent OnKeyHoldEvent;
    [Space(5)]
    public UnityEvent OnKeyUpEvent;

    // Update is called once per frame
    void Update()
    {
       if (Input.GetKeyDown(ActivationKey))
        {
            OnKeyDownEvent.Invoke();
        }

        if (Input.GetKey(ActivationKey))
        {
            OnKeyHoldEvent.Invoke();
        }

        if (Input.GetKeyUp(ActivationKey))
        {
            OnKeyUpEvent.Invoke();
        }
    }
}
