using System.Collections;
using GameSystem.Utilities.Tasks;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start() {
        var task = new Task(TestTask());
    }

    // Update is called once per frame
    private void Update() { }

    private IEnumerator TestTask() {
        Debug.Log("Test");
        yield return null;
    }
}