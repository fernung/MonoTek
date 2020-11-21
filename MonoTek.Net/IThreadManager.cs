using System;
using System.Collections.Generic;
using System.Text;

namespace MonoTek.Net
{
    public interface IThreadManager
    {
        void ExecuteOnMainThread(Action action);
        void UpdateMain();
    }

    public class ThreadManager : IThreadManager
    {
        private readonly List<Action> _executeOnMainThread = new List<Action>();
        private readonly List<Action> _executeCopiedOnMainThread = new List<Action>();
        private bool _actionToExecuteOnMainThread = false;

        public void ExecuteOnMainThread(Action action)
        {
            if (action == null)
            {
                Console.WriteLine("No action to execute on main thread!");
                return;
            }

            lock (_executeOnMainThread)
            {
                _executeOnMainThread.Add(action);
                _actionToExecuteOnMainThread = true;
            }
        }

        public void UpdateMain()
        {
            if (_actionToExecuteOnMainThread)
            {
                _executeCopiedOnMainThread.Clear();
                lock (_executeOnMainThread)
                {
                    _executeCopiedOnMainThread.AddRange(_executeOnMainThread);
                    _executeOnMainThread.Clear();
                    _actionToExecuteOnMainThread = false;
                }

                for (int i = 0; i < _executeCopiedOnMainThread.Count; i++)
                {
                    _executeCopiedOnMainThread[i]();
                }
            }
        }
    }
}
