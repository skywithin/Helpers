// Copyright 2013 Jon Skeet. All rights reserved. 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Helpers
{
    public static class TaskExtensions
    {
        /// <summary>
        /// Orders a list of tasks by completion.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IEnumerable<Task<T>> InCompletionOrder<T>(this IEnumerable<Task<T>> source)
        {
            var inputs = source.ToList();
            var boxes = inputs.Select(x => new TaskCompletionSource<T>()).ToList();

            int currentIndex = -1;

            foreach(var task in inputs)
            {
                task.ContinueWith(
                    completed =>
                        {
                            var nextBox = boxes[Interlocked.Increment(ref currentIndex)];
                            PropagateResult(completed, nextBox);
                        }, 
                    TaskContinuationOptions.ExecuteSynchronously);
            }
            return boxes.Select(box => box.Task);
        }

        private static void PropagateResult<T>(
            Task<T> completedTask, 
            TaskCompletionSource<T> completionSource)
        {
            switch (completedTask.Status)
            {
                case TaskStatus.Canceled:
                    completionSource.TrySetCanceled();
                    break;
                case TaskStatus.Faulted:
                    completionSource.TrySetException(completedTask.Exception.InnerException);
                    break;
                case TaskStatus.RanToCompletion:
                    completionSource.TrySetResult(completedTask.Result);
                    break;
                default:
                    throw new ArgumentException("Task was not completed");
            }
        }
    }
}
