using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class SoftResetable : MonoBehaviour
{
    public UnityEvent onSoftReset;

    private void OnEnable()
    {
        if (onSoftReset == null)
        {
            onSoftReset = new UnityEvent();
        }
    }

    public void CallReset()
    {
        Debug.Log(name + ": I have been called. I must reset.", gameObject);
        onSoftReset.Invoke();
    }
}