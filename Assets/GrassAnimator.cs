// This C# function can be called by an Animation Event
using UnityEngine;
using System.Collections;


public class GrassAnimator : MonoBehaviour {
    public void DisableSelf() {
        transform.gameObject.SetActive(false);
    }
}