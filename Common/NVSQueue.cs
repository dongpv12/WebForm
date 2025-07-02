using System.Collections;
using System.Threading;

namespace WebForm
{
    public class NVSQueue
    {
        private string Name;
        private AutoResetEvent autoReset;
        private Queue queue;
        private object objLock;

        private int _dequeueStep = 0;// 1: bắt đầu dequeue, 2: đề queue xong

        public NVSQueue(string name)
        {
            try
            {
                Name = name;
                autoReset = new AutoResetEvent(false);
                queue = new Queue();
                objLock = new object();
            }
            catch
            {
            }
        }

        public void Enqueue(object obj)
        {
            lock (objLock)
            {
                queue.Enqueue(obj);
                autoReset.Set();
            }
        }

        public object Dequeue()
        {
            if (queue.Count > 0)
                lock (objLock)
                {
                    return queue.Dequeue();
                }
            else
            {
                autoReset.WaitOne();
                lock (objLock)
                {
                    if (queue.Count > 0)
                        return queue.Dequeue();
                    else return null;
                }
            }
        }

        public object DequeuenoWait()
        {
            if (queue.Count > 0)
                lock (objLock)
                {
                    return queue.Dequeue();
                }
            else
            {
                return null;
            }
        }

        public void Clearqueue()
        {
            lock (objLock)
            {
                queue.Clear();
            }
        }

        public int Count()
        {
            return queue.Count;
        }

        public int DequeueStep
        {
            get
            {
                return _dequeueStep;
            }
            set
            {
                _dequeueStep = value;
            }

        }

        public void ReleaseMyQueue()
        {
            autoReset.Set();
        }

        public object ViewFirstItem()
        {
            if (queue.Count > 0)
            {
                return queue.Peek();
            }
            else
            {
                return null;
            }
        }
    }
}