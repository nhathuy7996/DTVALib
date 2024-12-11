using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using System.Collections.Concurrent;

namespace DTVA.Lib
{
    internal class UnityMainThread : MonoBehaviour
    {
        private static UnityMainThread _wkr;
        ConcurrentQueue<Action> jobs = new ConcurrentQueue<Action>();
#if UNITY_EDITOR
        [SerializeField]
        List<string> _callbackHistory = new List<string>();
        MethodInfo _trace;
#endif

        Action currentInvoke;

        internal static UnityMainThread wkr
        {
            get
            {
                if (_wkr == null)
                {
                    Debug.LogWarning(CONSTANT.Prefix + "==> Create UnitymaintThread obj<==");
                    GameObject g = new GameObject();

                    _wkr = g.AddComponent<UnityMainThread>();
                    g.name = "UnityMainTHread";

                    DontDestroyOnLoad(g);
                }

                return _wkr;
            }
        }

        protected virtual void Awake()
        {
            if (_wkr != null && _wkr.GetInstanceID() != this.GetInstanceID())
                Destroy(this);
            else
                _wkr = this.GetComponent<UnityMainThread>();

            DontDestroyOnLoad(this);
            StartCoroutine(LoopUnScale());
        }

        IEnumerator LoopUnScale()
        {
            while (true)
            {
                yield return new WaitForEndOfFrame();

                if (Time.timeScale == 0)
                    Update();
            }
        }


        void Update()
        {

            if (jobs.TryDequeue(out Action currentInvoke))
            {
                try
                {
#if UNITY_EDITOR
                    var _trace = currentInvoke.GetMethodInfo();
                    _callbackHistory.Add($"{_trace.DeclaringType}->{_trace}");
#endif
                    currentInvoke?.Invoke();
                }
                catch (Exception e)
                {
                    Debug.LogError(CONSTANT.Prefix + $"==>: {e.GetBaseException()}<==");
                }
            }
        }

        internal void AddJob(params Action[] newJobs)
        {
            lock (jobs)
            {
                foreach (Action job in newJobs)
                {
                    jobs.Enqueue(job);
                }
            }
        }

        private void OnDestroy()
        {
            while (jobs.Count > 0)
                if (jobs.TryDequeue(out Action currentInvoke))
                {
                    try
                    {
#if UNITY_EDITOR
                    var _trace = currentInvoke.GetMethodInfo();
                    _callbackHistory.Add($"{_trace.DeclaringType}->{_trace}");
#endif
                        currentInvoke?.Invoke();
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(CONSTANT.Prefix + $"==>: {e.GetBaseException()}<==");
                    }
                }
        }
    }
}