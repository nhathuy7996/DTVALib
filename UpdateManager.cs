using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_ANDROID && UPDATE_IMPLEMENT
using Google.Play.AppUpdate;
using Google.Play.Common;
#endif
using UnityEngine;

namespace DTVA.Lib
{
    public class UpdateManager : Singleton<UpdateManager>
    {
#if UNITY_ANDROID && UPDATE_IMPLEMENT
    private AppUpdateManager appUpdateManager;
 
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        if (Application.platform == RuntimePlatform.Android)
        {
            this.appUpdateManager = new AppUpdateManager();
        }
    } 
 
    private void Start()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
           StartCoroutine(CheckForUpdate());
        }
        
    }
 
    IEnumerator CheckForUpdate()
    {
        Debug.Log(CONSTANT.Prefix+"==> Start check update");
        PlayAsyncOperation<AppUpdateInfo, AppUpdateErrorCode> appUpdateInfoOperation =
          appUpdateManager.GetAppUpdateInfo();
 
        yield return appUpdateInfoOperation;
 
 
        if(appUpdateInfoOperation.Error == AppUpdateErrorCode.ErrorUnknown)
        {
            Debug.LogError(CONSTANT.Prefix+"==> There is some errors on update");
        }
       
        Debug.Log(CONSTANT.Prefix+"==> Get update Info success");
        if (appUpdateInfoOperation.IsSuccessful)
        {
            var appUpdateInfoResult = appUpdateInfoOperation.GetResult();
 
            if (appUpdateInfoResult.UpdateAvailability == UpdateAvailability.UpdateAvailable )
            {
                 
                if(appUpdateInfoResult.IsUpdateTypeAllowed(AppUpdateOptions.ImmediateAppUpdateOptions())){ 
                    StartCoroutine(StartImmediateUpdate(appUpdateInfoResult, AppUpdateOptions.ImmediateAppUpdateOptions()));
                }else

                if(appUpdateInfoResult.IsUpdateTypeAllowed(AppUpdateOptions.FlexibleAppUpdateOptions())){ 
                    StartCoroutine(StartFlexibleUpdate(appUpdateInfoResult, AppUpdateOptions.FlexibleAppUpdateOptions()));
                }
            }
        }
        else
        {
             Debug.Log(CONSTANT.Prefix+"==> There is no update for now!");
        }
    }
 
    IEnumerator StartImmediateUpdate(AppUpdateInfo appUpdateInfo_i, AppUpdateOptions appUpdateOptions_i)
    {
         Debug.Log(CONSTANT.Prefix+"==> Start Immediate Update ");
        var startUpdateRequest = appUpdateManager.StartUpdate(appUpdateInfo_i, appUpdateOptions_i);
        yield return startUpdateRequest;
    }

    IEnumerator StartFlexibleUpdate(AppUpdateInfo appUpdateInfoResult,AppUpdateOptions appUpdateOptions)
    { 
         Debug.Log(CONSTANT.Prefix+"==> Start Flexible Update ");
        var startUpdateRequest = appUpdateManager.StartUpdate( appUpdateInfoResult,  appUpdateOptions);

        while (!startUpdateRequest.IsDone)
        { 
            Debug.Log(CONSTANT.Prefix+$"==> Flexible Update {startUpdateRequest.DownloadProgress * 100}%");
            yield return null;
        }

    }
#endif
    }

}

