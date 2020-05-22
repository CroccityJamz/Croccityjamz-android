using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using AFollestad.MaterialDialogs;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Support.V4.View;
using Android.Views;
using Android.Widget;
using Com.Luseen.Autolinklibrary;
using DeepSound.Activities.Albums;
using DeepSound.Activities.Albums.Adapters;
using DeepSound.Activities.MyContacts;
using DeepSound.Activities.MyProfile;
using DeepSound.Activities.Songs;
using DeepSound.Activities.Songs.Adapters;
using DeepSound.Activities.Tabbes;
using DeepSound.Activities.Tabbes.Adapters;
using DeepSound.Helpers.Ads;
using DeepSound.Helpers.CacheLoaders;
using DeepSound.Helpers.Controller;
using DeepSound.Helpers.Fonts;
using DeepSound.Helpers.MediaPlayerController;
using DeepSound.Helpers.Model;
using DeepSound.Helpers.Utils;
using DeepSoundClient.Classes.Albums;
using DeepSoundClient.Classes.Global;
using DeepSoundClient.Classes.User;
using DeepSoundClient.Requests;
using Java.Lang;
using Liaoinstan.SpringViewLib.Widgets;
using Newtonsoft.Json;
using Exception = System.Exception;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using DefaultHeader = DeepSound.Helpers.PullSwipeStyles.DefaultHeader;

namespace DeepSound.Activities.UserProfile
{
    public class UserProfileFragment : Fragment, AppBarLayout.IOnOffsetChangedListener, SpringView.IOnFreshListener, MaterialDialog.IListCallback, MaterialDialog.ISingleButtonCallback
    {
        #region Variables Basic

        private HSoundAdapter LatestSongsAdapter, TopSongsAdapter, StoreAdapter;
        private ActivityAdapter ActivitiesAdapter;
        private HAlbumsAdapter AlbumsAdapter;
        private HomeActivity GlobalContext;
        private ImageView ImageAvatar, ImageCover;
        private TextView TxtFullName, IconCamera, IconVerified, IconPro, TxtUserName, TxtCountFollowers, TxtCountFollowing, TxtFollowers, TxtFollowing;
        private TextView IconInfo;
        private AutoLinkTextView TxtAbout;
        private LinearLayout LoadingLayout;
        private ViewStub EmptyStateLayout, LatestSongsViewStub, TopSongsViewStub, AlbumsViewStub, StoreViewStub, ActivitiesViewStub;
        private View Inflated, LatestSongsInflated, TopSongsInflated, AlbumsInflated, StoreInflated, ActivitiesInflated;
        private AppBarLayout AppBarLayout;
        private CollapsingToolbarLayout CollapsingToolbar;
        private FloatingActionButton BtnFollow;
        private SpringView SwipeRefreshLayout;
        private ImageButton ButtonMore;
        private AlbumsFragment AlbumsFragment;
        private ProfileObject.Details DetailsCounter; 
        private UserDataObject DataUser;
        private string UserId = "";
     
        #endregion

        #region General

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            HasOptionsMenu = true;
            // Create your fragment here
            GlobalContext = (HomeActivity)Activity;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            try
            {
                View view = inflater.Inflate(Resource.Layout.UserProfileLayout, container, false);

                UserId = Arguments.GetString("UserId") ?? "";
                DataUser = JsonConvert.DeserializeObject<UserDataObject>(Arguments.GetString("ItemData") ?? "");
                 
                InitComponent(view);
                InitToolbar(view);
                SetRecyclerViewAdapters();

                GetMyInfoData();

                AdsGoogle.Ad_RewardedVideo(Context);

                return view;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public override void OnLowMemory()
        {
            try
            {
                GC.Collect(GC.MaxGeneration);
                base.OnLowMemory();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        #region Functions

        private void InitComponent(View view)
        {
            try
            {
                CollapsingToolbar = (CollapsingToolbarLayout)view.FindViewById(Resource.Id.collapsingToolbar);
                CollapsingToolbar.Title = "";

                AppBarLayout = view.FindViewById<AppBarLayout>(Resource.Id.appBarLayout);
                AppBarLayout.SetExpanded(true);
                AppBarLayout.AddOnOffsetChangedListener(this);

                LoadingLayout = (LinearLayout)view.FindViewById(Resource.Id.Loading_LinearLayout);
                LoadingLayout.Visibility = ViewStates.Visible;

                SwipeRefreshLayout = (SpringView)view.FindViewById(Resource.Id.material_style_ptr_frame);
                SwipeRefreshLayout.SetType(SpringView.Type.Overlap);
                SwipeRefreshLayout.Header = new DefaultHeader(Activity);
                SwipeRefreshLayout.Footer = new Helpers.PullSwipeStyles.DefaultFooter(Activity);
                SwipeRefreshLayout.Enable = true;
                SwipeRefreshLayout.SetListener(this);

                ButtonMore = (ImageButton)view.FindViewById(Resource.Id.more);
                ButtonMore.Click += ButtonMoreOnClick;

                IconInfo = (TextView)view.FindViewById(Resource.Id.info);
                IconInfo.Click += IconInfoOnClick;

                ImageAvatar = (ImageView)view.FindViewById(Resource.Id.imageAvatar);
                ImageCover = (ImageView)view.FindViewById(Resource.Id.imageCover);

                TxtFullName = (TextView)view.FindViewById(Resource.Id.fullNameTextView);
                TxtUserName = (TextView)view.FindViewById(Resource.Id.userNameTextView);

                TxtFollowers = (TextView)view.FindViewById(Resource.Id.FollowersTextView);
                TxtCountFollowers = (TextView)view.FindViewById(Resource.Id.countFollowersTextView);
                TxtFollowers.Click += TxtFollowersOnClick;
                TxtCountFollowers.Click += TxtFollowersOnClick;

                TxtFollowing = (TextView)view.FindViewById(Resource.Id.FollowingTextView);
                TxtCountFollowing = (TextView)view.FindViewById(Resource.Id.countFollowingTextView);
                TxtFollowing.Click += TxtFollowingOnClick;
                TxtCountFollowing.Click += TxtFollowingOnClick;

                IconCamera = (TextView)view.FindViewById(Resource.Id.iconCamera);
                IconVerified = (TextView)view.FindViewById(Resource.Id.verified);
                IconPro = (TextView)view.FindViewById(Resource.Id.pro);

                EmptyStateLayout = (ViewStub)view.FindViewById(Resource.Id.viewStub);
                LatestSongsViewStub = (ViewStub)view.FindViewById(Resource.Id.viewStubLatestSongs);
                TopSongsViewStub = (ViewStub)view.FindViewById(Resource.Id.viewStubTopSongs);
                AlbumsViewStub = (ViewStub)view.FindViewById(Resource.Id.viewStubAlbums);
                StoreViewStub = (ViewStub)view.FindViewById(Resource.Id.viewStubStore);
                ActivitiesViewStub = (ViewStub)view.FindViewById(Resource.Id.viewStubActivities);

                TxtAbout = (AutoLinkTextView)view.FindViewById(Resource.Id.AboutTextview);

                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, IconCamera, IonIconsFonts.AndroidCamera);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, IconVerified, IonIconsFonts.CheckmarkCircled);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, IconPro, FontAwesomeIcon.Rocket);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, IconInfo, IonIconsFonts.InformationCircled);

                IconVerified.Visibility = ViewStates.Invisible;
                IconPro.Visibility = ViewStates.Invisible;

                BtnFollow = (FloatingActionButton)view.FindViewById(Resource.Id.FollowFab);
                BtnFollow.Click+= BtnFollowOnClick;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
         
        private void InitToolbar(View view)
        {
            try
            {
                var toolbar = view.FindViewById<Toolbar>(Resource.Id.toolbar);
                GlobalContext.SetToolBar(toolbar, " ", true, false);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void SetRecyclerViewAdapters()
        {
            try
            {
                //Latest Songs RecyclerView >> LinearLayoutManager.Horizontal 
                LatestSongsAdapter = new HSoundAdapter(Activity) { SoundsList = new ObservableCollection<SoundDataObject>() };
                LatestSongsAdapter.OnItemClick += LatestSongsAdapterOnOnItemClick;

                //Top Songs RecyclerView >> LinearLayoutManager.Horizontal 
                TopSongsAdapter = new HSoundAdapter(Activity) { SoundsList = new ObservableCollection<SoundDataObject>() };
                TopSongsAdapter.OnItemClick += TopSongsAdapterOnOnItemClick;

                //Albums RecyclerView >> LinearLayoutManager.Horizontal 
                AlbumsAdapter = new HAlbumsAdapter(Activity);
                AlbumsAdapter.ItemClick += AlbumsAdapterOnItemClick;

                //Store RecyclerView >> LinearLayoutManager.Horizontal 
                StoreAdapter = new HSoundAdapter(Activity) { SoundsList = new ObservableCollection<SoundDataObject>() };
                StoreAdapter.OnItemClick += TopSongsAdapterOnOnItemClick;

                //Activities RecyclerView >> LinearLayoutManager.Vertical
                ActivitiesAdapter = new ActivityAdapter(Activity) { ActivityList = new ObservableCollection<ActivityDataObject>() };
                ActivitiesAdapter.OnItemClick += ActivitiesAdapterOnOnItemClick;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        #region Event

        //Start Play Sound 
        private void TopSongsAdapterOnOnItemClick(object sender, HSoundAdapterClickEventArgs e)
        {
            try
            {
                if (e.Position > -1)
                {
                    var item = TopSongsAdapter.GetItem(e.Position);
                    if (item != null)
                    {
                        Constant.PlayPos = e.Position;
                        GlobalContext?.SoundController?.StartPlaySound(item, TopSongsAdapter.SoundsList);
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Start Play Sound 
        private void LatestSongsAdapterOnOnItemClick(object sender, HSoundAdapterClickEventArgs e)
        {
            try
            {
                if (e.Position > -1)
                {
                    var item = LatestSongsAdapter.GetItem(e.Position);
                    if (item != null)
                    {
                        Constant.PlayPos = e.Position;
                        GlobalContext?.SoundController?.StartPlaySound(item, LatestSongsAdapter.SoundsList);
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Start Play Sound 
        private void ActivitiesAdapterOnOnItemClick(object sender, ActivityAdapterClickEventArgs e)
        {
            try
            {
                if (e.Position > -1)
                {
                    var item = ActivitiesAdapter.GetItem(e.Position);
                    if (item != null)
                    {
                        Constant.PlayPos = 0;
                        GlobalContext?.SoundController?.StartPlaySound(item.TrackData, new ObservableCollection<SoundDataObject>() { item.TrackData });
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Open profile Albums
        private void AlbumsAdapterOnItemClick(object sender, HAlbumsAdapterClickEventArgs e)
        {
            try
            {
                if (e.Position > -1)
                {
                    var item = AlbumsAdapter.GetItem(e.Position);
                    if (item != null)
                    {
                        Bundle bundle = new Bundle();
                        bundle.PutString("ItemData", JsonConvert.SerializeObject(item));
                        bundle.PutString("AlbumsId", item.Id.ToString());
                        AlbumsFragment = new AlbumsFragment
                        {
                            Arguments = bundle
                        };
                        GlobalContext.FragmentBottomNavigator.DisplayFragment(AlbumsFragment);
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Follow  
        private void BtnFollowOnClick(object sender, EventArgs e)
        {
            try
            {
                if (!UserDetails.IsLogin)
                {
                    PopupDialogController dialog = new PopupDialogController(GlobalContext, null, "Login");
                    dialog.ShowNormalDialog(GlobalContext.GetText(Resource.String.Lbl_Login), GlobalContext.GetText(Resource.String.Lbl_Message_Sorry_signin), GlobalContext.GetText(Resource.String.Lbl_Yes), GlobalContext.GetText(Resource.String.Lbl_No));
                    return;
                }

                if (Methods.CheckConnectivity())
                {
                    switch (BtnFollow.Tag.ToString())
                    {
                        case "Add": //Sent follow 
                            BtnFollow.SetBackgroundColor(Color.ParseColor(AppSettings.MainColor));
                            BtnFollow.SetImageResource(Resource.Drawable.ic_tick);
                            BtnFollow.Tag = "friends";

                            Toast.MakeText(Context, Context.GetText(Resource.String.Lbl_Sent_successfully_followed),ToastLength.Short).Show();
                            RequestsAsync.User.FollowUnFollowUserAsync(UserId, true).ConfigureAwait(false);
                            break;
                        case "friends": //Sent un follow 
                            BtnFollow.SetBackgroundColor(Color.ParseColor("#444444"));
                            BtnFollow.SetImageResource(Resource.Drawable.ic_add);
                            BtnFollow.Tag = "Add";

                            Toast.MakeText(Context, Context.GetText(Resource.String.Lbl_Sent_successfully_Unfollowed),ToastLength.Short).Show();
                            RequestsAsync.User.FollowUnFollowUserAsync(UserId, false).ConfigureAwait(false);
                            break;
                    }
                }
                else
                {
                    Toast.MakeText(Context, Context.GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //wael
        private void ActivitiesMoreOnClick(object sender, EventArgs e)
        {
            try
            {

            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
        private void StoreMoreOnClick(object sender, EventArgs e)
        {
            try
            {
                Bundle bundle = new Bundle();
                bundle.PutString("SongsType", "UserProfileStore");

                SongsByTypeFragment songsByTypeFragment = new SongsByTypeFragment
                {
                    Arguments = bundle
                };
                GlobalContext.FragmentBottomNavigator.DisplayFragment(songsByTypeFragment);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //wael
        private void AlbumsMoreOnClick(object sender, EventArgs e)
        {
            try
            {
                
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void TopSongsMoreOnClick(object sender, EventArgs e)
        {
            try
            {
                Bundle bundle = new Bundle();
                bundle.PutString("SongsType", "UserProfileTopSongs");

                SongsByTypeFragment songsByTypeFragment = new SongsByTypeFragment
                {
                    Arguments = bundle
                };
                GlobalContext.FragmentBottomNavigator.DisplayFragment(songsByTypeFragment);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void LatestSongsMoreOnClick(object sender, EventArgs e)
        {
            try
            {
                Bundle bundle = new Bundle();
                bundle.PutString("SongsType", "UserProfileLatestSongs");

                SongsByTypeFragment songsByTypeFragment = new SongsByTypeFragment
                {
                    Arguments = bundle
                };
                GlobalContext.FragmentBottomNavigator.DisplayFragment(songsByTypeFragment);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //More 
        private void ButtonMoreOnClick(object sender, EventArgs e)
        {
            try
            {
                var arrayAdapter = new List<string>();
                var dialogList = new MaterialDialog.Builder(Context).Theme(AppSettings.SetTabDarkTheme ? Theme.Dark : Theme.Light);
                if (UserDetails.IsLogin)
                    arrayAdapter.Add(Context.GetText(Resource.String.Lbl_Block));

                arrayAdapter.Add(Context.GetText(Resource.String.Lbl_CopyLinkToProfile));
                dialogList.Items(arrayAdapter);
                dialogList.PositiveText(Context.GetText(Resource.String.Lbl_Close)).OnPositive(this);
                dialogList.AlwaysCallSingleChoiceCallback();
                dialogList.ItemsCallback(this).Build().Show();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Show Info User
        private void IconInfoOnClick(object sender, EventArgs e)
        {
            try
            {
                Intent intent = new Intent(Activity, typeof(DialogInfoUserActivity));
                intent.PutExtra("ItemDataUser", JsonConvert.SerializeObject(DataUser));
                intent.PutExtra("ItemDataDetails", JsonConvert.SerializeObject(DetailsCounter));
                Activity.StartActivity(intent);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Open User Contact >> Following
        private void TxtFollowingOnClick(object sender, EventArgs e)
        {
            try
            {
                Bundle bundle = new Bundle();
                bundle.PutString("UserId", UserId);
                bundle.PutString("UserType", "Following");

                ContactsFragment contactsFragment = new ContactsFragment
                {
                    Arguments = bundle
                };
                GlobalContext.FragmentBottomNavigator.DisplayFragment(contactsFragment);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Open User Contact >> Followers
        private void TxtFollowersOnClick(object sender, EventArgs e)
        {
            try
            { 
                Bundle bundle = new Bundle();
                bundle.PutString("UserId", UserId);
                bundle.PutString("UserType", "Followers");

                ContactsFragment contactsFragment = new ContactsFragment
                {
                    Arguments = bundle
                };
                GlobalContext.FragmentBottomNavigator.DisplayFragment(contactsFragment); 
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
         
        #endregion

        #region Menu

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            try
            {
                switch (item.ItemId)
                {
                    case Android.Resource.Id.Home:
                        GlobalContext.FragmentNavigatorBack();
                        return true;
                }
                return base.OnOptionsItemSelected(item);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return false;
            }
        }

        #endregion
         
        #region Load Data User

        private void GetMyInfoData()
        {
            try
            {
                LoadDataUser();

                new Handler(Looper.MainLooper).Post(new Runnable(Run));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private async void Run()
        {
            try
            {
                if (Methods.CheckConnectivity())
                {
                    var (apiStatus, respond) = await RequestsAsync.User.ProfileAsync(UserId, "followers,following,albums,activities,latest_songs,top_songs,store").ConfigureAwait(false);
                    if (apiStatus.Equals(200))
                    {
                        if (respond is ProfileObject result)
                        {
                            if (result.Data != null)
                            {
                                DataUser = result.Data;

                                Activity.RunOnUiThread(() =>
                                {
                                    try
                                    {
                                        LoadDataUser();

                                        if (result.details != null)
                                        {
                                            DetailsCounter = result.details;
                                            TxtCountFollowers.Text = Methods.FunString.FormatPriceValue(result.details.Followers);
                                            TxtCountFollowing.Text = Methods.FunString.FormatPriceValue(result.details.Following);
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        Console.WriteLine(e);
                                    }
                                });

                                //Add Latest Songs
                                if (result.Data?.Latestsongs?.Count > 0)
                                {
                                    LatestSongsAdapter.SoundsList = new ObservableCollection<SoundDataObject>(result.Data?.Latestsongs);
                                }

                                //Add Latest Songs
                                if (result.Data?.TopSongs?.Count > 0)
                                {
                                    TopSongsAdapter.SoundsList = new ObservableCollection<SoundDataObject>(result.Data?.TopSongs);
                                }

                                //Add Albums
                                if (result.Data?.Albums?.Count > 0)
                                {
                                    AlbumsAdapter.AlbumsList = new ObservableCollection<DataAlbumsObject>(result.Data?.Albums);
                                }

                                //Add Store
                                if (result.Data?.Store?.Count > 0)
                                {
                                    StoreAdapter.SoundsList = new ObservableCollection<SoundDataObject>(result.Data?.Store);
                                }

                                //Add Activities
                                if (result.Data?.Activities?.Count > 0)
                                {
                                    ActivitiesAdapter.ActivityList = new ObservableCollection<ActivityDataObject>(result.Data.Activities);
                                }

                                //SqLiteDatabase dbDatabase = new SqLiteDatabase();
                                //dbDatabase.InsertOrUpdate_DataMyInfo(result.Data);
                                //dbDatabase.Dispose();
                            }
                        }
                    }
                    else Methods.DisplayReportResult(Activity, respond);

                    Activity.RunOnUiThread(ShowEmptyPage);
                }
                else
                {
                    Activity.RunOnUiThread(() =>
                    {
                        Inflated = EmptyStateLayout.Inflate();
                        EmptyStateInflater x = new EmptyStateInflater();
                        x.InflateLayout(Inflated, EmptyStateInflater.Type.NoConnection);
                        if (!x.EmptyStateButton.HasOnClickListeners)
                        {
                            x.EmptyStateButton.Click += null;
                            x.EmptyStateButton.Click += EmptyStateButtonOnClick;
                        }

                        Toast.MakeText(Context, GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
                    });
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        //No Internet Connection 
        private void EmptyStateButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                new Handler(Looper.MainLooper).Post(new Runnable(Run));
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void ShowEmptyPage()
        {
            try
            {
                LoadingLayout.Visibility = ViewStates.Gone;

                if (LatestSongsAdapter.SoundsList?.Count > 0)
                {
                    if (LatestSongsInflated == null)
                        LatestSongsInflated = LatestSongsViewStub.Inflate();

                    TemplateRecyclerInflater recyclerInflater = new TemplateRecyclerInflater();
                    recyclerInflater.InflateLayout<SoundDataObject>(Activity, LatestSongsInflated, LatestSongsAdapter, TemplateRecyclerInflater.TypeLayoutManager.LinearLayoutManagerHorizontal, 0, true, Context.GetText(Resource.String.Lbl_LatestSongs_Title));
                    if (!recyclerInflater.MainLinear.HasOnClickListeners)
                    {
                        recyclerInflater.MainLinear.Click += null;
                        recyclerInflater.MainLinear.Click += LatestSongsMoreOnClick;
                    }
                }

                if (TopSongsAdapter.SoundsList?.Count > 0)
                {
                    if (TopSongsInflated == null)
                        TopSongsInflated = TopSongsViewStub.Inflate();

                    TemplateRecyclerInflater recyclerInflater = new TemplateRecyclerInflater();
                    recyclerInflater.InflateLayout<SoundDataObject>(Activity, TopSongsInflated, TopSongsAdapter, TemplateRecyclerInflater.TypeLayoutManager.LinearLayoutManagerHorizontal, 0, true, Context.GetText(Resource.String.Lbl_TopSongs_Title));
                    if (!recyclerInflater.MainLinear.HasOnClickListeners)
                    {
                        recyclerInflater.MainLinear.Click += null;
                        recyclerInflater.MainLinear.Click += TopSongsMoreOnClick;
                    }
                }

                if (AlbumsAdapter.AlbumsList?.Count > 0)
                {
                    if (AlbumsInflated == null)
                        AlbumsInflated = AlbumsViewStub.Inflate();

                    TemplateRecyclerInflater recyclerInflater = new TemplateRecyclerInflater();
                    recyclerInflater.InflateLayout<DataAlbumsObject>(Activity, AlbumsInflated, AlbumsAdapter, TemplateRecyclerInflater.TypeLayoutManager.LinearLayoutManagerHorizontal, 0, true, Context.GetText(Resource.String.Lbl_Albums));
                    if (!recyclerInflater.MainLinear.HasOnClickListeners)
                    {
                        recyclerInflater.MainLinear.Click += null;
                        recyclerInflater.MainLinear.Click += AlbumsMoreOnClick;
                    }
                }

                if (StoreAdapter.SoundsList?.Count > 0)
                {
                    if (StoreInflated == null)
                        StoreInflated = StoreViewStub.Inflate();

                    TemplateRecyclerInflater recyclerInflater = new TemplateRecyclerInflater();
                    recyclerInflater.InflateLayout<SoundDataObject>(Activity, StoreInflated, StoreAdapter, TemplateRecyclerInflater.TypeLayoutManager.LinearLayoutManagerHorizontal, 0, true, Context.GetText(Resource.String.Lbl_Store_Title));
                    if (!recyclerInflater.MainLinear.HasOnClickListeners)
                    {
                        recyclerInflater.MainLinear.Click += null;
                        recyclerInflater.MainLinear.Click += StoreMoreOnClick;
                    }
                }

                if (ActivitiesAdapter.ActivityList?.Count > 0)
                {
                    if (ActivitiesInflated == null)
                        ActivitiesInflated = ActivitiesViewStub.Inflate();

                    TemplateRecyclerInflater recyclerInflater = new TemplateRecyclerInflater();
                    recyclerInflater.InflateLayout<ActivityDataObject>(Activity, ActivitiesInflated, ActivitiesAdapter, TemplateRecyclerInflater.TypeLayoutManager.LinearLayoutManagerVertical, 0, true, Context.GetText(Resource.String.Lbl_Activities_Title));
                    if (!recyclerInflater.MainLinear.HasOnClickListeners)
                    {
                        recyclerInflater.MainLinear.Click += null;
                        recyclerInflater.MainLinear.Click += ActivitiesMoreOnClick;
                    }
                }

                if (LatestSongsAdapter.SoundsList?.Count == 0 && TopSongsAdapter.SoundsList?.Count == 0 && AlbumsAdapter.AlbumsList?.Count == 0 && StoreAdapter.SoundsList?.Count == 0 && ActivitiesAdapter.ActivityList?.Count == 0)
                {
                    if (Inflated == null)
                        Inflated = EmptyStateLayout.Inflate();

                    EmptyStateInflater x = new EmptyStateInflater();
                    x.InflateLayout(Inflated, EmptyStateInflater.Type.NoSound);
                    if (x.EmptyStateButton.HasOnClickListeners)
                    {
                        x.EmptyStateButton.Click += null;
                    }

                    EmptyStateLayout.Visibility = ViewStates.Visible;
                }
            }
            catch (Exception e)
            {
                LoadingLayout.Visibility = ViewStates.Gone;
                Console.WriteLine(e);
            }
        }

        private void LoadDataUser()
        {
            try
            {
                CollapsingToolbar.Title = DeepSoundTools.GetNameFinal(DataUser);

                GlideImageLoader.LoadImage(Activity, DataUser.Avatar, ImageAvatar, ImageStyle.CenterCrop, ImagePlaceholders.Drawable);
                GlideImageLoader.LoadImage(Activity, DataUser.Cover, ImageCover, ImageStyle.CenterCrop, ImagePlaceholders.Drawable);

                TxtFullName.Text = DeepSoundTools.GetNameFinal(DataUser);
                TxtUserName.Text = "@" + DataUser.Username;
                 
                TextSanitizer aboutSanitizer = new TextSanitizer(TxtAbout, Activity);
                aboutSanitizer.Load(DeepSoundTools.GetAboutFinal(DataUser));
                 
                IconPro.Visibility = DataUser.IsPro == 1 ? ViewStates.Visible : ViewStates.Invisible;
                IconVerified.Visibility = DataUser.Verified == 1 ? ViewStates.Visible : ViewStates.Invisible;

                if (DataUser.IsFollowing) // My Friend
                {
                    BtnFollow.SetBackgroundColor(Color.ParseColor(AppSettings.MainColor));
                    BtnFollow.SetImageResource(Resource.Drawable.ic_tick);
                    BtnFollow.Tag = "friends";
                }
                else  //Not Friend
                {
                    BtnFollow.SetBackgroundColor(Color.ParseColor("#444444"));
                    BtnFollow.SetImageResource(Resource.Drawable.ic_add);
                    BtnFollow.Tag = "Add";
                } 
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        #region appBarLayout

        public void OnOffsetChanged(AppBarLayout appBarLayout, int verticalOffset)
        {
            try
            {
                int minHeight = ViewCompat.GetMinimumHeight(CollapsingToolbar) * 2;
                float scale = (float)(minHeight + verticalOffset) / minHeight;

                BtnFollow.ScaleX = scale >= 0 ? scale : 0;
                BtnFollow.ScaleY = scale >= 0 ? scale : 0;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        #region Refresh

        public void OnRefresh()
        {
            try
            {
                LatestSongsAdapter.SoundsList.Clear();
                LatestSongsAdapter.NotifyDataSetChanged();

                TopSongsAdapter.SoundsList.Clear();
                TopSongsAdapter.NotifyDataSetChanged();

                AlbumsAdapter.AlbumsList.Clear();
                AlbumsAdapter.NotifyDataSetChanged();

                StoreAdapter.SoundsList.Clear();
                StoreAdapter.NotifyDataSetChanged();

                ActivitiesAdapter.ActivityList.Clear();
                ActivitiesAdapter.NotifyDataSetChanged();

                EmptyStateLayout.Visibility = ViewStates.Gone;

                new Handler(Looper.MainLooper).Post(new Runnable(Run));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void OnLoadMore()
        {
            try
            {

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        #region MaterialDialog

        public async void OnSelection(MaterialDialog p0, View p1, int itemId, ICharSequence itemString)
        {
            try
            {
                string text = itemString.ToString();
                if (text == Context.GetText(Resource.String.Lbl_Block))
                {
                    if (Methods.CheckConnectivity())
                    {
                        Toast.MakeText(Context, Context.GetText(Resource.String.Lbl_Blocked_successfully), ToastLength.Long).Show();

                        await RequestsAsync.User.BlockUnBlockUserAsync(DataUser.Id.ToString(), true).ConfigureAwait(false);

                        GlobalContext.FragmentNavigatorBack();
                    }
                    else
                    {
                        Toast.MakeText(Context, Context.GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
                    } 
                }
                else if (text == Context.GetText(Resource.String.Lbl_CopyLinkToProfile))
                {
                    string url = DataUser?.Url;
                    GlobalContext.SoundController.ClickListeners.OnMenuCopyOnClick(url);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void OnClick(MaterialDialog p0, DialogAction p1)
        {
            try
            {
                if (p1 == DialogAction.Positive)
                {
                }
                else if (p1 == DialogAction.Negative)
                {
                    p0.Dismiss();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion
    }
}