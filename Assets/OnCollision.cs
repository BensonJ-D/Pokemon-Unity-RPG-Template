using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnCollision : MonoBehaviour
{
    public void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("OWWW");
        transform.GetChild(0).gameObject.SetActive(true);
    }
}
