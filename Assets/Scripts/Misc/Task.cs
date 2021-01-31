

using System.Collections;
using UnityEngine;

namespace Misc
{
	public class Task 
	{
		public bool Running => task.Running;

		public bool Paused => task.Paused;

		public Task(IEnumerator c, bool autoStart = true)
		{
			task = TaskController.CreateTask(c);
			if(autoStart)
				Start();
		}
	
		public void Start()
		{
			task.Start();
		}

		public void Stop()
		{
			task.Stop();
		}
	
		public void Pause()
		{
			task.Pause();
		}
	
		public void Unpause()
		{
			task.Unpause();
		}

		private readonly TaskController.TaskState task;
	}

	internal class TaskController : MonoBehaviour
	{
		public class TaskState
		{
			public bool Running { get; private set; }

			public bool Paused { get; private set; }

			private readonly IEnumerator coroutine;

			public TaskState(IEnumerator c)
			{
				coroutine = c;
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
				IEnumerator e = coroutine;
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