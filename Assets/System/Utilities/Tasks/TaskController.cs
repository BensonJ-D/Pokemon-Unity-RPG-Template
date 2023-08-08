using System.Collections;
using System.Runtime.CompilerServices;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace System.Utilities.Tasks
{
    public class TaskController : MonoBehaviour
    {
        #region Singleton setup

        private static TaskController _instance;
        public static TaskController Instance { 
            get => GetInstance();
            private set => _instance = value;
        }

        private static TaskController GetInstance()
        {
            if (_instance != null) return _instance;
            
            var instance = new GameObject();
            _instance = instance.AddComponent<TaskController>();
            _instance.name = "TaskController";

            return _instance;
        }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
            } else {
                _instance = this;
            }
        }
        #endregion
        
        public static TaskState CreateTask(IEnumerator coroutine) => new TaskState(coroutine);
    }
}