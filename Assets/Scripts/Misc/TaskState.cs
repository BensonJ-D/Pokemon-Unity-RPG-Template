using System.Collections;
using System.Collections.Generic;

namespace Misc
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
            TaskController.Instance.StartCoroutine(CallWrapper());
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
}