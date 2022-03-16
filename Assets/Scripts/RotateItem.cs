﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateItem : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (!MenuManager.Instance.GameIsPaused)
        {
            transform.Rotate(0, 2, 0, Space.World);
        }
        else
        {
            transform.Rotate(0, 0, 0, Space.World);
        }
	}
}
