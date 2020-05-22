namespace DeepSound
{
    //For the accuracy of the icon and logo, please use this website " http://nsimage.brosteins.com " and add images according to size in folders " mipmap " 

    public static class AppSettings
    {
        //Main Settings >>>>>
        public static string Cert = "NFYnA2qriwLLUe74dlNM96SrVvtvb5awCQo2qxD7iZhHUUrbI5ZTfVixH5ZlwQUq+hHW6rkwmgJvXbFpT0KwfqAP4YQoZn+1rLxznXrUjp+KzzupJ/LtvFvs3cIVkalzPN9vQqM9JB2TKxdS8eanqU2PXLXPFnaHa/c5CrQTR9P0jldqnNyUVRi9tLcvpPnBGfrEb9hkGpYO9ZG7y6+TLJC2UE/+vIUnEsTFOi/QcRq9qBCLkwupfgdUUpOWdqWDMkNeQUVz3wiDySOOw4MYwg==";
        //*********************************************************

        public static string ApplicationName = "Croccityjamz";
        public static string Version = "1.4";

        //Main Colors >>
        //*********************************************************
        public static string MainColor = "#ffa142";

        //Language Settings >> http://www.lingoes.net/en/translator/langcode.htm
        //*********************************************************
        public static bool FlowDirectionRightToLeft = false;
        public static string Lang = "en_US"; //Default language ar_AE

        //Error Report Mode
        //*********************************************************
        public static bool SetApisReportMode = false; 

        //Notification Settings >>
        //*********************************************************
        public static string OneSignalAppId = "bfc2e195-6c3c-4123-97ce-1e122f5809a9";

        public static bool ShowNotification = true;
        public static int RefreshGetNotification = 60000; // 1 minute

        //AdMob >> Please add the code ads in the Here and analytic.xml 
        //*********************************************************
        public static bool ShowAdMobBanner = true;
        public static bool ShowAdMobInterstitial = true;
        public static bool ShowAdMobRewardVideo = true;
        public static bool ShowAdMobNative = true; 

        public static string AdInterstitialKey = "ca-app-pub-5135691635931982/5807300922";
        public static string AdRewardVideoKey = "ca-app-pub-5135691635931982/9993834236";
        public static string AdAdMobNativeKey = "ca-app-pub-5135691635931982/1307424613"; 

        //Three times after entering the ad is displayed
        public static int ShowAdMobInterstitialCount = 3;
        public static int ShowAdMobRewardedVideoCount = 3;
        //*********************************************************

        //Social Logins >>
        //If you want login with facebook or google you should change id key in the analytic.xml file  
        //Facebook >> ../values/analytic.xml .. line 8 - 9 
        //Google >> ../values/analytic.xml .. line 13
        //*********************************************************
        public static bool ShowFacebookLogin = true;
        public static bool ShowGoogleLogin = true;

        public static string ClientId = "104516058316-9vjdctmsk63o35nbpp872is04qqa84vc.apps.googleusercontent.com";
        public static string ClientSecret = "EP7_0AtIm8eEszTpNPfGJsJ2";

        //*********************************************************
        public static bool ShowPrice = true;
        public static bool ShowSkipButton = true; 

        //Set Theme Full Screen App
        //*********************************************************
        public static bool EnableFullScreenApp = false;

        //Set Blur Screen Comment
        //*********************************************************
        public static bool EnableBlurBackgroundComment = true;

        //Set the radius of the Blur. Supported range 0 < radius <= 25
        public static readonly float BlurRadiusComment = 25f;

        //Import && Upload Videos >>  
        //*********************************************************
        public static bool ShowButtonUploadSingleSong { get; set; } = true;
        public static bool ShowButtonUploadAlbum { get; set; } = false;// >> Next Version
         
        //Offline Sound >>  
        //*********************************************************
        public static bool AllowOfflineDownload = true;
         
        //Profile >>  
        //*********************************************************
        public static bool ShowEmail = true; //#New

        /// <summary>
        /// true :  use Download Manager you can see file song in folder app  
        /// false :  default used normal download and you can see i folder "Dcim"
        /// </summary>
        public static bool SetDownloadManager = true; 

        public static bool ShowForwardTrack = true; 
        public static bool ShowBackwardTrack = true; 

        //Settings Page >>  
        //*********************************************************
        public static bool ShowEditPassword = true; 
        public static bool ShowWithdrawals = true; 
        public static bool ShowGoPro = true; 
        public static bool ShowBlockedUsers = true; 
         
         
        //Set Theme App >> Color - Tab
        //*********************************************************
        public static bool SetTabLightTheme = true;
        public static bool SetTabColoredTheme = false;
        public static bool SetTabDarkTheme = false;
        public static string TabColoredColor = MainColor;

        public static bool SetTabIsTitledWithText = true;

        //Bypass Web Erros 
        ///*********************************************************
        public static bool TurnTrustFailureOnWebException = true;
        public static bool TurnSecurityProtocolType3072On = true;

        //*********************************************************
        public static bool RenderPriorityFastPostLoad = true;

    }
} 