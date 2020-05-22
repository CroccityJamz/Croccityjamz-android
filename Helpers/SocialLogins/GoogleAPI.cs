using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using DeepSound.Properties;

namespace DeepSound.Helpers.SocialLogins
{
    public sealed class GoogleApi : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private GoogleProfile Profile;
        private readonly GoogleServices GoogleServices;

        private GoogleProfile GoogleProfile
        {
            set
            {
                Profile = value;
                OnPropertyChanged();
            }
        }

        public GoogleApi()
        {
            try
            {
                Console.WriteLine(Profile);
                GoogleServices = new GoogleServices();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public async Task<string> GetAccessTokenAsync(string code)
        {
            try
            {
                var dataGoogle = await GoogleServices.GetAccessTokenAsync(code);
                return dataGoogle.AccessToken;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public async Task SetGoogleUserProfileAsync(string accessToken)
        {
            try
            {
                GoogleProfile = await GoogleServices.GetGoogleUserProfileAsync(accessToken);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}