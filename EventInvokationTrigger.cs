using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class EventInvokationTrigger : MonoBehaviour
{

    public bool EventInvokationTriggerActive = true;

    [Header("Leave empty detect all collisions")]
    public string ColliderTag = null;

    [Space(5)]
    public UnityEvent OnTriggerEnterEvent;
    [Space(5)]
    public UnityEvent OnTriggerStayEvent;
    [Space(5)]
    public UnityEvent OnTriggerExitEvent;


    private void OnTriggerEnter(Collider other)

    {

        if ((ColliderTag == "") || other.tag == (ColliderTag))
        {
            OnTriggerEnterEvent.Invoke();
        }



    }

    private void OnTriggerStay(Collider other)
    {
        if ((ColliderTag == "") || other.tag == (ColliderTag))
        {
            OnTriggerStayEvent.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if ((ColliderTag == "") || other.tag == (ColliderTag))
        {
            OnTriggerExitEvent.Invoke();
        }
    }

}
