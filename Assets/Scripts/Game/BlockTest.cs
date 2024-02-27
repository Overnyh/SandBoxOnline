using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object.Synchronizing;
using UnityEngine;

public class BlockTest : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        print("Hello i am collision:))");
    }
}
