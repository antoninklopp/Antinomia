
// Copyright (c) 2017-2018 Antonin KLOPP-TOSSER
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickManager {

    public float MaxTimeToClick;
    public float MinTimeToClick;
    public bool IsDebug;

    private float _maxTimeToClick = 0.4f;
    private float _minTimeToClick = 0.05f;
    private bool _Isdebug = false;

    private float _minCurrentTime;
    private float _maxCurrentTime; 

    public bool DoubleClick() {

        if (Time.time >= _minCurrentTime && Time.time <= _maxCurrentTime) {
            if (_Isdebug) {
                Debug.Log("Double Click");
                _minCurrentTime = 0;
                _maxCurrentTime = 0;
                return true; 
            }
        }

        _minCurrentTime = Time.time + MinTimeToClick;
        _maxCurrentTime = Time.time + MaxTimeToClick;
        return false; 

    }


}
