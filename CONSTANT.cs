
namespace DTVA.Lib
{
        public static class CONSTANT
        {
#if UNITY_EDITOR
        public const string Prefix = "<color=cyan>[Huynn3rdLib]</color>";
#else
                public const string Prefix = "[Huynn3rdLib]";
#endif


                #region MiraiSDK key
                public const string SdkUrl = "https://github.com/nhathuy796/nhathuy796/releases/download/v6.0/MiraiSuperSDK_withPangle.unitypackage";
                public const string FirebaseX86Url = "https://github.com/nhathuy796/nhathuy796/releases/download/FBx86/FirebaseX86.unitypackage";

                public const string GoogleSheetUrl = "https://sheets.googleapis.com/v4/spreadsheets/{0}/values/{1}?key=AIzaSyD0jUG-gWRKuDBfcCikoIp9nssmCU2bJ3k";

                public const string AdUnitClassName = "DVAH.AdUnitModule{0}_{1}";
                #endregion

                #region Custom
                public const string Check_rev = "CHECK_REV";
                public const string REMOTE_DEFAULT = "REMOTE_DEFAULT";
                #endregion
        }
}
