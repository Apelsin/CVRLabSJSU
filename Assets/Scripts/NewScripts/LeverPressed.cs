﻿using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class LeverPressed : MonoBehaviour
{
    //public AudioSource buttonSound;
    //public LeverAnimation pressLeverAnimation;

    [SerializeField]
    private UnityEvent _Pressed;

    public UnityEvent Pressed
    {
        get { return _Pressed; }
    }

    private void Start()
    {
    }

    private void Update()
    {
    }

    public void OnPress()
    {
        Pressed.Invoke();
    }
}