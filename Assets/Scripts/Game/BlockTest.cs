using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

public class BlockTest : NetworkBehaviour
{
    private void Start()
    {
        // GetComponent<Rigidbody>().AddForceAtPosition(new Vector3(100, 100, 100), new Vector3(0, 0, 0));
    }

    private void OnCollisionEnter(Collision other)
    {
        print("Hello i am collision:))");
    }
}
