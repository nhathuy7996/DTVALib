
using UnityEditor;
using UnityEngine;

namespace DTVA.Lib{
public class OnPostBuildSetting 
{
    [UnityEditor.Callbacks.PostProcessBuild(0)]
    public static void OnPostProcessBuild()
    {
        if (!EditorUserBuildSettings.buildAppBundle)
        {
           return;
        }
    
    }
}
}
