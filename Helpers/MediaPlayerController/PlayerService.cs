using System;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V4.Media.Session;
using Android.Telephony;
using Android.Widget;
using DeepSound.Activities;
using DeepSound.Activities.Tabbes;
using DeepSound.Helpers.Model;
using DeepSound.Helpers.Utils;
using DeepSoundClient;
using DeepSoundClient.Classes.Global;
using DeepSoundClient.Classes.Tracks;
using DeepSoundClient.Requests;
using Exception = System.Exception;
using Uri = Android.Net.Uri;

namespace DeepSound.Helpers.MediaPlayerController
{
    [Service]
    public class PlayerService : IntentService
    {
        #region Variables Basic

        public static string ActionFirstPlay;
        public static string ActionBackward;
        public static string ActionForward;
        public static string ActionSeekto;
        public static string ActionPlay;
        public static string ActionPause;
        public static string ActionStop;
        public static string ActionSkip;
        public static string ActionRewind;
        public static string ActionNotiPlay;
        private NotificationCompat.Builder Notification;
        private RemoteViews BigViews, SmallViews;
        private readonly string NotificationChannelId = "deepsound_ch_1";
        private NotificationManager MNotificationManager;
        private MyBroadcastReceiver OnCallIncome;
        private static PlayerService Service;
        private HomeActivity GlobalContext;
        private SoundDataObject Item;

        #endregion

        #region General

        public static PlayerService GetPlayerService()
        {
            return Service;
        }

        public PlayerService() : base("PlayerService")
        {

        }

        protected override void OnHandleIntent(Intent intent)
        {

        }

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        public override void OnCreate()
        {
            try
            {
                base.OnCreate();
                Service = this;

                Task.Run(() =>
                {
                    GlobalContext = HomeActivity.GetInstance();
                    MNotificationManager = (NotificationManager)GetSystemService(NotificationService);

                    OnCallIncome = new MyBroadcastReceiver();

                    RegisterReceiver(OnCallIncome, new IntentFilter("android.intent.action.PHONE_STATE"));
                });

                if (Constant.ArrayListPlay.Count > 0)
                {
                    CreateNoti();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            try
            {
                base.OnStartCommand(intent, flags, startId);
                Task.Run(() =>
                {
                    string action = intent.Action;
                    if (action == ActionFirstPlay)
                    {
                        HandleFirstPlay();
                    }
                    else if (action == ActionSeekto)
                    {
                        SeekTo(intent.Extras.GetInt("seekto"));
                    }
                    else if (action == ActionPlay)
                    {
                        Play();
                    }
                    else if (action == ActionPause)
                    {
                        Pause();
                    }
                    else if (action == ActionStop)
                    {
                        Stop(intent);
                    }
                    else if (action == ActionRewind)
                    {
                        Previous();
                    }
                    else if (action == ActionSkip)
                    {
                        Next();
                    }
                    else if (action == ActionBackward)
                    {
                        Backward();
                    }
                    else if (action == ActionForward)
                    {
                        Forward();
                    }
                    else if (action == ActionNotiPlay)
                    {
                        if (Constant.MediaPlayer != null && Constant.MediaPlayer.IsPlaying)
                        {
                            Pause();
                        }
                        else
                        {
                            Play();
                        }
                    }

                    return StartCommandResult.Sticky;
                });

                return StartCommandResult.Sticky;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return StartCommandResult.NotSticky;
            }
        }

        private void HandleFirstPlay()
        {
            try
            {
                Constant.IsPlayed = false;

                SetPlayAudio();
                ShowNotification();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void SeekTo(int seek)
        {
            try
            {
                int totalDuration = Constant.MediaPlayer.Duration;
                int currentPosition = MusicUtils.ProgressToTimer(seek, totalDuration);

                // forward or backward to certain seconds
                Constant.MediaPlayer.SeekTo(currentPosition);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void Play()
        {
            try
            {
                if (Constant.IsPlayed)
                {
                    Constant.MediaPlayer.Start();
                    GlobalContext?.SoundController?.SetProgress();
                }
                else
                {
                    HandleFirstPlay();
                }
                GlobalContext?.SoundController?.RotateImageAlbum();
                ChangePlayPauseIcon();
                UpdateNotiPlay(Constant.MediaPlayer.IsPlaying);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void Pause()
        {
            try
            {
                Constant.MediaPlayer.Pause();

                GlobalContext?.SoundController?.RotateImageAlbum();
                ChangePlayPauseIcon();
                UpdateNotiPlay(Constant.MediaPlayer.IsPlaying);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void Stop(Intent intent)
        {
            try
            {
                GlobalContext?.SoundController?.RotateImageAlbum();
                ChangePlayPauseIcon();
                Constant.IsPlayed = false;
                GlobalContext?.SoundController?.ReleaseSound();
                GlobalContext?.SoundController?.Destroy();
                RemoveNoti();
                UnregisterReceiver(OnCallIncome);
                StopService(intent);
                StopForeground(true);

                GlobalContext?.OffWakeLock();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void Previous()
        {
            try
            {
                SetBuffer(true);
                if (Constant.IsSuffle)
                {
                    Random rand = new Random();
                    if (Constant.ArrayListPlay.Count > 0)
                        Constant.PlayPos = rand.Next(Constant.ArrayListPlay.Count);

                }
                else
                {
                    if (Constant.PlayPos > 0)
                    {
                        Constant.PlayPos -= 1;
                    }
                    else
                    {
                        Constant.PlayPos = Constant.ArrayListPlay.Count - 1;
                    }
                }
                GlobalContext?.SoundController?.RotateImageAlbum();
                HandleFirstPlay();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void Next()
        {
            try
            {
                SetBuffer(true);
                if (Constant.IsSuffle)
                {
                    Random rand = new Random();
                    Constant.PlayPos = rand.Next(Constant.ArrayListPlay.Count - 1 + 1);
                }
                else
                {
                    if (Constant.PlayPos < Constant.ArrayListPlay.Count - 1)
                    {
                        Constant.PlayPos += 1;
                    }
                    else
                    {
                        Constant.PlayPos = 0;
                    }
                }

                GlobalContext?.SoundController?.RotateImageAlbum();
                HandleFirstPlay();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void Backward()
        {
            try
            {
                var bTime = 10000; // 10 Sec
                if (Constant.MediaPlayer != null)
                {
                    var sTime = Constant.MediaPlayer.CurrentPosition;

                    if ((sTime - bTime) > 0)
                    {
                        sTime -= bTime;
                        Constant.MediaPlayer.SeekTo(sTime);

                        GlobalContext?.SoundController?.SeekUpdate();
                    }
                    else
                    {
                        Toast.MakeText(Constant.Context, "Cannot jump backward 10 seconds", ToastLength.Short).Show();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void Forward()
        {
            try
            {
                var fTime = 10000; // 10 Sec
                if (Constant.MediaPlayer != null)
                {
                    var eTime = Constant.MediaPlayer.Duration;
                    var sTime = Constant.MediaPlayer.CurrentPosition;
                    if ((sTime + fTime) <= eTime)
                    {
                        sTime += fTime;
                        Constant.MediaPlayer.SeekTo(sTime);

                        GlobalContext?.SoundController?.SeekUpdate();
                    }
                    else
                    {
                        Toast.MakeText(Constant.Context, "Cannot jump forward 10 seconds", ToastLength.Short).Show();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }



        #region data Noti

        private void ShowNotification()
        {
            try
            {
                StartForeground(101, Notification.Build());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void RemoveNoti()
        {
            try
            {
                MNotificationManager.CancelAll();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void CreateNoti()
        {
            try
            {
                BigViews = new RemoteViews(PackageName, Resource.Layout.CustomNotificationLayout);
                SmallViews = new RemoteViews(PackageName, Resource.Layout.CustomNotificationSmallLayout);

                Intent notificationIntent = new Intent(this, typeof(SplashScreenActivity));
                notificationIntent.SetAction(Intent.ActionMain);
                notificationIntent.AddCategory(Intent.CategoryLauncher);
                notificationIntent.PutExtra("isnoti", true);
                PendingIntent pendingIntent = PendingIntent.GetActivity(this, 0, notificationIntent, PendingIntentFlags.UpdateCurrent);

                Intent previousIntent = new Intent(this, typeof(PlayerService));
                previousIntent.SetAction(ActionRewind);
                PendingIntent ppreviousIntent = PendingIntent.GetService(this, 0, previousIntent, 0);

                Intent playIntent = new Intent(this, typeof(PlayerService));
                playIntent.SetAction(ActionNotiPlay);
                PendingIntent pplayIntent = PendingIntent.GetService(this, 0, playIntent, 0);

                Intent nextIntent = new Intent(this, typeof(PlayerService));
                nextIntent.SetAction(ActionSkip);
                PendingIntent pnextIntent = PendingIntent.GetService(this, 0, nextIntent, 0);

                Intent closeIntent = new Intent(this, typeof(PlayerService));
                closeIntent.SetAction(ActionStop);
                PendingIntent pcloseIntent = PendingIntent.GetService(this, 0, closeIntent, 0);

                Notification = new NotificationCompat.Builder(this, NotificationChannelId)
                    .SetLargeIcon(BitmapFactory.DecodeResource(Resources, Resource.Mipmap.icon))
                    .SetContentTitle(AppSettings.ApplicationName)
                    .SetPriority((int)NotificationPriority.Max)
                    .SetContentIntent(pendingIntent)
                    .SetSmallIcon(Resource.Drawable.icon_notification)
                    .SetTicker(Constant.ArrayListPlay[Constant.PlayPos]?.Title)
                    .SetChannelId(NotificationChannelId)
                    .SetOngoing(true)
                    .SetAutoCancel(true)
                    .SetOnlyAlertOnce(true);

                NotificationChannel mChannel;
                if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
                {
                    NotificationImportance importance = NotificationImportance.Low;
                    mChannel = new NotificationChannel(NotificationChannelId, AppSettings.ApplicationName, importance);
                    MNotificationManager.CreateNotificationChannel(mChannel);

                    MediaSessionCompat mMediaSession = new MediaSessionCompat(Application.Context, AppSettings.ApplicationName);
                    mMediaSession.SetFlags(MediaSessionCompat.FlagHandlesMediaButtons | MediaSessionCompat.FlagHandlesTransportControls);

                    Notification.SetStyle(new Android.Support.V4.Media.App.NotificationCompat.MediaStyle()
                            .SetMediaSession(mMediaSession.SessionToken).SetShowCancelButton(true)
                            .SetShowActionsInCompactView(0, 1, 2)
                            .SetCancelButtonIntent(MediaButtonReceiver.BuildMediaButtonPendingIntent(Application.Context, PlaybackStateCompat.ActionStop)))
                        .AddAction(new NotificationCompat.Action(Resource.Xml.ic_skip_previous, "Previous", ppreviousIntent))
                        .AddAction(new NotificationCompat.Action(Resource.Xml.ic_pause, "Pause", pplayIntent))
                        .AddAction(new NotificationCompat.Action(Resource.Xml.ic_skip_next, "Next", pnextIntent))
                        .AddAction(new NotificationCompat.Action(Resource.Drawable.ic_action_close, "Close", pcloseIntent));
                }
                else
                {
                    string songName = Methods.FunString.DecodeString(Constant.ArrayListPlay[Constant.PlayPos]?.Title);
                    string genresName = Methods.FunString.DecodeString(Constant.ArrayListPlay[Constant.PlayPos]?.CategoryName) + " " + Application.Context.Resources.GetString(Resource.String.Lbl_Music);

                    BigViews.SetOnClickPendingIntent(Resource.Id.imageView_noti_play, pplayIntent);
                    BigViews.SetOnClickPendingIntent(Resource.Id.imageView_noti_next, pnextIntent);
                    BigViews.SetOnClickPendingIntent(Resource.Id.imageView_noti_prev, ppreviousIntent);
                    BigViews.SetOnClickPendingIntent(Resource.Id.imageView_noti_close, pcloseIntent);
                    SmallViews.SetOnClickPendingIntent(Resource.Id.status_bar_collapse, pcloseIntent);

                    BigViews.SetImageViewResource(Resource.Id.imageView_noti_play, Android.Resource.Drawable.IcMediaPause);
                    BigViews.SetTextViewText(Resource.Id.textView_noti_name, songName);
                    SmallViews.SetTextViewText(Resource.Id.status_bar_track_name, songName);
                    BigViews.SetTextViewText(Resource.Id.textView_noti_artist, genresName);
                    SmallViews.SetTextViewText(Resource.Id.status_bar_artist_name, genresName);
                    BigViews.SetImageViewResource(Resource.Id.imageView_noti, Resource.Mipmap.icon);
                    SmallViews.SetImageViewResource(Resource.Id.status_bar_album_art, Resource.Mipmap.icon);
                    Notification.SetCustomContentView(SmallViews).SetCustomBigContentView(BigViews);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void UpdateNoti()
        {
            try
            {
                string songName = Methods.FunString.DecodeString(Constant.ArrayListPlay[Constant.PlayPos]?.Title);
                string genresName = Methods.FunString.DecodeString(Constant.ArrayListPlay[Constant.PlayPos]?.CategoryName) + " " + GlobalContext.GetText(Resource.String.Lbl_Music);

                if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
                {
                    Notification.SetContentTitle(songName);
                    Notification.SetContentText(genresName);
                }
                else
                {
                    BigViews.SetTextViewText(Resource.Id.textView_noti_name, songName);
                    BigViews.SetTextViewText(Resource.Id.textView_noti_artist, genresName);
                    SmallViews.SetTextViewText(Resource.Id.status_bar_artist_name, genresName);
                    SmallViews.SetTextViewText(Resource.Id.status_bar_track_name, songName);
                }
                UpdateNotiPlay(Constant.MediaPlayer.IsPlaying);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void UpdateNotiPlay(bool isPlay)
        {
            try
            {
                string songName = Methods.FunString.DecodeString(Constant.ArrayListPlay[Constant.PlayPos]?.Title);
                string genresName = Methods.FunString.DecodeString(Constant.ArrayListPlay[Constant.PlayPos]?.CategoryName) + " " + GlobalContext.GetText(Resource.String.Lbl_Music);

                if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
                {
                    Intent playIntent = new Intent(this, typeof(PlayerService));
                    playIntent.SetAction(ActionNotiPlay);
                    PendingIntent pPreviousIntent = PendingIntent.GetService(this, 0, playIntent, 0);

                    if (isPlay)
                    {
                        if (Notification.MActions.Count > 0)
                            Notification.MActions[1] = new NotificationCompat.Action(Resource.Xml.ic_pause, "Pause", pPreviousIntent);
                    }
                    else
                    {
                        if (Notification.MActions.Count > 0)
                            Notification.MActions[1] = new NotificationCompat.Action(Resource.Xml.ic_play_arrow, "Play", pPreviousIntent);
                    }

                    if (!string.IsNullOrEmpty(songName))
                        Notification.SetContentTitle(songName);
                    if (!string.IsNullOrEmpty(genresName))
                        Notification.SetContentText(genresName);
                }
                else
                {
                    if (isPlay)
                    {
                        BigViews.SetImageViewResource(Resource.Id.imageView_noti_play, Android.Resource.Drawable.IcMediaPause);
                    }
                    else
                    {
                        BigViews.SetImageViewResource(Resource.Id.imageView_noti_play, Android.Resource.Drawable.IcMediaPause);
                    }

                    if (!string.IsNullOrEmpty(songName))
                        BigViews.SetTextViewText(Resource.Id.textView_noti_name, songName);
                    if (!string.IsNullOrEmpty(genresName))
                        BigViews.SetTextViewText(Resource.Id.textView_noti_artist, genresName);
                    if (!string.IsNullOrEmpty(genresName))
                        SmallViews.SetTextViewText(Resource.Id.status_bar_artist_name, genresName);
                    if (!string.IsNullOrEmpty(songName))
                        SmallViews.SetTextViewText(Resource.Id.status_bar_track_name, songName);
                }

                var url = Constant.ArrayListPlay[Constant.PlayPos]?.Thumbnail?.Replace(" ", "%20");
                if (!string.IsNullOrEmpty(url))
                {
                    var bit = BitmapUtil.GetImageBitmapFromUrl(url);
                    if (bit != null)
                        Notification.SetLargeIcon(bit);
                }


                MNotificationManager.Notify(101, Notification.Build());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        #region Player && Listener

        public void OnCompletion()
        {
            try
            {
                if (IsReset)
                    return;

                Item = Constant.ArrayListPlay[Constant.PlayPos];

                if (Item != null && Item.Price != 0 && !Item.IsOwner && !Item.IsPurchased)
                {
                    //This song is paid, You must pay 
                    if (!string.IsNullOrEmpty(Item.DemoTrack))
                        GlobalContext?.OpenDialogPurchase(Item);
                }
                else
                {
                    if (Constant.IsRepeat)
                    {
                        Constant.MediaPlayer.SeekTo(0);
                        Constant.MediaPlayer.Start();
                    }
                    else
                    {
                        if (Constant.IsSuffle)
                        {
                            Random rand = new Random();
                            Constant.PlayPos = rand.Next((Constant.ArrayListPlay.Count - 1) + 1);
                            SetPlayAudio();
                        }
                        else
                        {
                            SetNext();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public async void OnPrepared()
        {
            try
            {
                Constant.IsPlayed = true;
                Constant.MediaPlayer.Start();
                IsReset = false;

                SetBuffer(false);

                UpdateNoti();

                if (Item != null)
                {
                    //add to Recent Played
                    if (UserDetails.IsLogin)
                        GlobalContext?.LibrarySynchronizer.AddToRecentlyPlayed(Item);

                    (int apiStatus, var respond) = await RequestsAsync.Tracks.GetTrackInfoAsync(Item.Id.ToString()).ConfigureAwait(false);
                    if (apiStatus.Equals(200))
                    {
                        if (respond is GetTrackInfoObject result)
                        {
                            var data = Constant.ArrayListPlay.FirstOrDefault(a => a.Id == Item.Id);
                            if (data != null)
                            {
                                data = result.Data;
                                Item = result.Data;
                            }

                            Console.WriteLine(data);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        private void ChangeText()
        {
            try
            {
                if (Item != null)
                    GlobalContext?.SoundController?.LoadSoundData(Item);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private bool IsReset;
        private void SetPlayAudio()
        {
            try
            {
                if (Constant.MediaPlayer == null) return;

                try
                {
                    if (Constant.MediaPlayer.IsPlaying)
                        Constant.MediaPlayer.Stop();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

                IsReset = true;
                Constant.MediaPlayer.Reset();

                Item = Constant.ArrayListPlay[Constant.PlayPos];
                if (Item == null) return;

                ChangeText();
                Uri mediaUri;

                if (Item.Price == 0 || Item.IsOwner || Item.IsPurchased)
                {
                    mediaUri = Uri.Parse(Item.AudioLocation);
                }
                else if (!string.IsNullOrEmpty(Item.DemoTrack))
                {
                    if (!Item.DemoTrack.Contains(Client.WebsiteUrl))
                        Item.DemoTrack = Client.WebsiteUrl + "/" + Item.DemoTrack;

                    mediaUri = Uri.Parse(Item.DemoTrack);
                }
                else
                    mediaUri = Uri.Parse(Item.AudioLocation);

                Constant.MediaPlayer.SetDataSource(Constant.Context, mediaUri);
                Constant.MediaPlayer.PrepareAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void SetBuffer(bool isBuffer)
        {
            try
            {
                if (isBuffer) return;
                GlobalContext.RunOnUiThread(() =>
                {
                    GlobalContext?.SoundController?.SetProgress();
                    ChangePlayPauseIcon();
                    GlobalContext?.SoundController?.SeekUpdate();
                    GlobalContext?.SoundController?.RotateImageAlbum();
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void SetNext()
        {
            try
            {
                if (Constant.PlayPos < Constant.ArrayListPlay.Count - 1)
                    Constant.PlayPos += 1;
                else
                    Constant.PlayPos = 0;

                GlobalContext?.SoundController?.RotateImageAlbum();

                SetPlayAudio();

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void ChangePlayPauseIcon()
        {
            try
            {
                GlobalContext?.SoundController?.ChangePlayPauseIcon();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private class MyBroadcastReceiver : BroadcastReceiver
        {
            public override void OnReceive(Context context, Intent intent)
            {
                try
                {
                    string a = intent.GetStringExtra(TelephonyManager.ExtraState);

                    if (Constant.MediaPlayer.IsPlaying)
                    {
                        if (a.Equals(TelephonyManager.ExtraStateOffhook) || a.Equals(TelephonyManager.ExtraStateRinging))
                        {
                            Constant.MediaPlayer.Pause();
                        }
                        else if (a.Equals(TelephonyManager.ExtraStateIdle))
                        {
                            Constant.MediaPlayer.Start();
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

    }
}