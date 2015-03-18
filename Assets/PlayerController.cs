﻿using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

    // Use this for initialization
    void Start() {
        netChar = GetComponent<NetworkCharacter>();
    }

    // Update is called once per frame
    void Update() {
        netChar.Direction = transform.rotation * new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        if (netChar.Direction.magnitude > 1f)
            netChar.Direction = netChar.Direction.normalized;

        // Handle jumping
        if (Input.GetButton("Jump")) {
            netChar.IsJumping = true;
        } else {
            netChar.IsJumping = false;
        }
    }

    NetworkCharacter netChar;
}
