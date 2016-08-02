using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHDY.Core
{
    public class AsyncAutoResetEvent
    {
        // 保存一个成功完成的 Task<TResult>，供重用以提高性能
        private readonly static Task s_completed = Task.FromResult(true);
        // 等待任务队列
        private readonly Queue<TaskCompletionSource<bool>> m_waits = new Queue<TaskCompletionSource<bool>>();
        // 用于跟踪 信号到达时可能没有等待者 的情况，将AsyncAutoResetEvent的初始状态设置为有信号
        private bool m_signaled;

        public Task WaitAsync()
        {
            lock (m_waits)
            {
                if (m_signaled)
                {
                    m_signaled = false;
                    return s_completed;
                }
                else
                {
                    var tcs = new TaskCompletionSource<bool>();
                    m_waits.Enqueue(tcs);
                    return tcs.Task;
                }
            }
        }

        public void Set()
        {
            TaskCompletionSource<bool> toRelease = null;
            lock (m_waits)
            {
                if (m_waits.Count > 0)
                    toRelease = m_waits.Dequeue();
                else if (!m_signaled)
                    m_signaled = true;
            }
            if (toRelease != null)
                toRelease.SetResult(true);
        }
    }
}
