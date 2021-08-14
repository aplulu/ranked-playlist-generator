using System;
using System.Runtime.CompilerServices;
using UnityEngine.Networking;

namespace RankedPlaylistGenerator.Managers
{
    public static class UnityWebRequestAsyncOperationExtension
    {
        public static UnityWebRequestAsyncOperationAwaiter GetAwaiter(this UnityWebRequestAsyncOperation asyncOperation)
        {
            return new UnityWebRequestAsyncOperationAwaiter(asyncOperation);
        }
    }
    public class UnityWebRequestAsyncOperationAwaiter : INotifyCompletion
    {
        UnityWebRequestAsyncOperation _asyncOperation;

        public bool IsCompleted => _asyncOperation.isDone;

        public UnityWebRequestAsyncOperationAwaiter(UnityWebRequestAsyncOperation asyncOperation)
        {
            _asyncOperation = asyncOperation;
        }

        public void GetResult() { }

        public void OnCompleted(Action continuation)
        {
            _asyncOperation.completed += _ => { continuation(); };
        }
    }
}