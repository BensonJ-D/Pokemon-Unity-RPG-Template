using System.Collections;
using UnityEngine;

namespace GameSystem.Utilities.Tasks
{
    public class TaskController : MonoBehaviour
    {
        public static TaskState CreateTask(IEnumerator coroutine) {
            return new TaskState(coroutine);
        }

        #region Singleton setup

        private static TaskController _instance;
        public static TaskController Instance => GetInstance();

        private static TaskController GetInstance() {
            if (_instance != null) return _instance;

            var instance = new GameObject();
            _instance = instance.AddComponent<TaskController>();
            _instance.name = "TaskController";

            return _instance;
        }

        private void Awake() {
            if (_instance != null && _instance != this)
                Destroy(gameObject);
            else
                _instance = this;
        }

        #endregion
    }
}