

using System.Collections;
using UnityEngine;

namespace Misc
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

		private readonly TaskController.TaskState _task;
	}

	internal class TaskController : MonoBehaviour
	{
		public class TaskState
		{
			public bool Running { get; private set; }

			public bool Paused { get; private set; }

			private readonly IEnumerator _coroutine;

			public TaskState(IEnumerator c)
			{
				_coroutine = c;
			}
		
			public void Pause()
			{
				Paused = true;
			}
		
			public void Unpause()
			{
				Paused = false;
			}
		
			public void Start()
			{
				Running = true;
				_singleton.StartCoroutine(CallWrapper());
			}
		
			public void Stop()
			{
				Running = false;
			}

			private IEnumerator CallWrapper()
			{
				yield return null;
				IEnumerator e = _coroutine;
				while(Running) {
					if(Paused)
						yield return null;
					else {
						if(e != null && e.MoveNext()) {
							yield return e.Current;
						}
						else {
							Running = false;
						}
					}
				}
			}
		}

		private static TaskController _singleton;

		public static TaskState CreateTask(IEnumerator coroutine)
		{
			if (!(_singleton is null)) return new TaskState(coroutine);
			GameObject go = new GameObject("TaskController");
			_singleton = go.AddComponent<TaskController>();
		
			return new TaskState(coroutine);
		}
	}
}