
using UnityEngine;

namespace DTVA.Lib
{
    public class MemoryChecker : MonoBehaviour
    {
#if !UNITY_EDITOR
        static AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");

        static AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        static AndroidJavaObject context = activity.Call<AndroidJavaObject>("getApplicationContext");

        static AndroidJavaObject activityManager = context.Call<AndroidJavaObject>("getSystemService", "activity");


        static AndroidJavaObject memoryInfo = new AndroidJavaObject("android.app.ActivityManager$MemoryInfo");
        public static long GetUsedRAM()
        {

            if (Application.platform != RuntimePlatform.Android)
            {
                return 0;
            }

            activityManager.Call("getMemoryInfo", memoryInfo);


            long availableMem = memoryInfo.Get<long>("availMem");
            long totalMem = memoryInfo.Get<long>("totalMem");


            long usedMem = totalMem - availableMem;

            return (usedMem / (1024 * 1024)) / 10;
        }
#else

    public static long GetUsedRAM()
    {
        
       return 0;
    }
#endif
    }
}
