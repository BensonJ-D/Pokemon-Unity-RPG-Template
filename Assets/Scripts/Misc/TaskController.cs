using System.Collections;
using UnityEngine;

namespace Misc
{
    public class TaskController : MonoBehaviour
    {
        #region Singleton setup
        public static TaskController Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            } else {
                Instance = this;
            }
        }
        #endregion
        
        public static TaskState CreateTask(IEnumerator coroutine) => new TaskState(coroutine);
    }
}