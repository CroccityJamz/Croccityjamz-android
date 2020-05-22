using Android.Gms.Common.Apis;
using DeepSound.Activities.Default;
using Java.Lang;

namespace DeepSound.Helpers.SocialLogins
{
    public class SignOutResultCallback : Object, IResultCallback
    {
        public LoginActivity Activity { get; set; }

        public void OnResult(Object result)
        {
            //Activity.UpdateUI(false);
        }
    }
}