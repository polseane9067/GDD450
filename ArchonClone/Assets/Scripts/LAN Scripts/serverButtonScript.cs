﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class serverButtonScript : MonoBehaviour {

    public HostData hostData;
    public Text buttonText;

	// Use this for initialization
	void Start () {
        this.GetComponent<Button>().onClick.AddListener(() => { Network.Connect(hostData); });
	}
}
