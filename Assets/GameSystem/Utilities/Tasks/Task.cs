using System.Collections;
using UnityEngine;

namespace GameSystem.Utilities.Tasks
{
    public class Task
    {
        private readonly TaskState _task;

        public Task(IEnumerator c, bool autoStart = true) {
            _task = TaskController.CreateTask(c);
            if (autoStart)
                Start();
        }

        public bool Running => _task.Running;

        public bool Paused => _task.Paused;

        public static Task EmptyTask => new Task(EmptyTaskFn());

        private void Start() {
            _task.Start();
        }

        public void Stop() {
            _task.Stop();
        }

        public void Pause() {
            _task.Pause();
        }

        public void Unpause() {
            _task.Unpause();
        }

        public Task QueueTask(IEnumerator newTask) {
            return new Task(QueuedTask(newTask));
        }

        private IEnumerator QueuedTask(IEnumerator newTask) {
            yield return new WaitWhile(() => Running);
            yield return newTask;
        }

        private static IEnumerator EmptyTaskFn() {
            yield break;
        }
    }
}