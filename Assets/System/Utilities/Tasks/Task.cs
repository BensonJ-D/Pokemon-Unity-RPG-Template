using System.Collections;
using UnityEngine;

namespace System.Utilities.Tasks
{
	public class Task 
	{
		public bool Running => _task.Running;

		public bool Paused => _task.Paused;

		public Task(IEnumerator c, bool autoStart = true)
		{
			_task = TaskController.CreateTask(c);
			if(autoStart)
				Start();
		}

		private void Start()
		{
			_task.Start();
		}

		public void Stop()
		{
			_task.Stop();
		}
	
		public void Pause()
		{
			_task.Pause();
		}
	
		public void Unpause()
		{
			_task.Unpause();
		}

		private readonly TaskState _task;

		public Task QueueTask(IEnumerator newTask) => new Task(QueuedTask(newTask));

		private IEnumerator QueuedTask(IEnumerator newTask)
		{
			yield return new WaitWhile(() => Running);
			yield return newTask;
		}
		
		public static Task EmptyTask => new Task(EmptyTaskFn());
		private static IEnumerator EmptyTaskFn()
		{
			yield break;
		}
	}
}