using System;
using System.Collections;
using System.Collections.Generic;
using System.Utilities.Tasks;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var task = new Task(TestTask());
    }

    private IEnumerator TestTask() {
        Debug.Log("Test");
        yield return null;
    } 
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
