using System;
using System.Collections.Generic;
using UnityEngine;

namespace Iguagile
{
    public class IguagileDispatcher : MonoBehaviour
    {
        internal static IguagileDispatcher Dispatcher;
        
        private readonly Queue<Action> _queue = new Queue<Action>();

        void Update()
        {
            while (_queue.Count > 0)
            {
                _queue.Dequeue()();
            }
        }

        private void Invoke(Action action)
        {
            _queue.Enqueue(action);
        }

        public static void BeginInvoke(Action action)
        {
            Dispatcher.Invoke(action);
        }
    }
}