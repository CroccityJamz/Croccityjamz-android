using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Bumptech.Glide.Integration.RecyclerView;
using Bumptech.Glide.Util;
using DeepSound.Activities.Playlist.Adapters;
using DeepSound.Activities.Songs.Adapters;
using DeepSound.Activities.Tabbes;
using DeepSound.Helpers.Controller;
using DeepSound.Helpers.MediaPlayerController;
using DeepSound.Helpers.Utils;
using DeepSoundClient.Classes.Global;
using DeepSoundClient.Classes.Playlist;
using DeepSoundClient.Requests;
using Me.Relex;
using Newtonsoft.Json;
using Fragment = Android.Support.V4.App.Fragment;

namespace DeepSound.Activities.Playlist
{
    public class PlaylistProfileFragment : Fragment, AppBarLayout.IOnOffsetChangedListener
    {
        #region Variables Basic

        private PlaylistViewPager PlaylistViewPager;
        public RowSoundLiteAdapter MAdapter;
        private HomeActivity GlobalContext;
        private RecyclerView MRecycler; 
        private LinearLayoutManager LayoutManager;
        private ViewStub EmptyStateLayout;
        private View Inflated;
        private AppBarLayout AppBarLayout;
        private CollapsingToolbarLayout CollapsingToolbar;
        private ViewPager InfoPlaylistViewPager; 
        private CircleIndicator CircleIndicator; 
        private TextView TxtNamePlaylist , TxtPublisherName;
        private PlaylistDataObject PlaylistObject;
        private string PlaylistId = "";
        private ImageView BackIcon;
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
                View view = inflater.Inflate(Resource.Layout.PlaylistProfileLayout, container, false);

                InitComponent(view);
                //InitToolbar(view);
                SetRecyclerViewAdapters();

                SetDataPlaylist();
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
                MRecycler = (RecyclerView)view.FindViewById(Resource.Id.recylerSongsPlaylist);
                EmptyStateLayout = view.FindViewById<ViewStub>(Resource.Id.viewStub);

                CollapsingToolbar = view.FindViewById<CollapsingToolbarLayout>(Resource.Id.collapsingToolbar);
                CollapsingToolbar.Title = "";

                AppBarLayout = view.FindViewById<AppBarLayout>(Resource.Id.appBarLayout);
                AppBarLayout.SetExpanded(true);
                AppBarLayout.AddOnOffsetChangedListener(this);

                InfoPlaylistViewPager = view.FindViewById<ViewPager>(Resource.Id.viewpager);
                CircleIndicator = view.FindViewById<CircleIndicator>(Resource.Id.indicator);

                TxtNamePlaylist = view.FindViewById<TextView>(Resource.Id.name);
                TxtPublisherName = view.FindViewById<TextView>(Resource.Id.publishername);

                BackIcon = view.FindViewById<ImageView>(Resource.Id.back);
                BackIcon.Click += BackIcon_Click;
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
                MAdapter = new RowSoundLiteAdapter(Activity, "PlaylistProfileFragment") { SoundsList = new ObservableCollection<SoundDataObject>()};
                MAdapter.OnItemClick += MAdapterOnOnItemClick;
                LayoutManager = new LinearLayoutManager(Activity);
                MRecycler.SetLayoutManager(LayoutManager);
                MRecycler.HasFixedSize = true;
                MRecycler.SetItemViewCacheSize(10);
                MRecycler.GetLayoutManager().ItemPrefetchEnabled = true;
                var sizeProvider = new FixedPreloadSizeProvider(10, 10);
                var preLoader = new RecyclerViewPreloader<SoundDataObject>(Activity, MAdapter, sizeProvider, 10);
                MRecycler.AddOnScrollListener(preLoader);
                MRecycler.SetAdapter(MAdapter);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
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

        #region Event
        private void BackIcon_Click(object sender, EventArgs e)
        {
            GlobalContext.FragmentNavigatorBack();
        }

        //Start Play Sound  
        private void MAdapterOnOnItemClick(object sender, RowSoundLiteAdapterClickEventArgs e)
        {
            try
            {
                var item = MAdapter.GetItem(e.Position);
                if (item != null)
                {
                    Constant.PlayPos = e.Position;
                    GlobalContext?.SoundController?.StartPlaySound(item, MAdapter.SoundsList);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #endregion

        #region Load Playlist Songs And Data 

        private void SetDataPlaylist()
        {
            try
            {
                PlaylistId = Arguments.GetString("PlaylistId") ?? "";
                if (!string.IsNullOrEmpty(PlaylistId))
                {
                    PlaylistObject = JsonConvert.DeserializeObject<PlaylistDataObject>(Arguments.GetString("ItemData") ?? "");
                    if (PlaylistObject != null)
                    {
                        CollapsingToolbar.Title = PlaylistObject.Name;

                        TxtNamePlaylist.Text = PlaylistObject.Name;
                        TxtPublisherName.Text = PlaylistObject?.Publisher != null ? Context.GetText(Resource.String.Lbl_By) + " " + DeepSoundTools.GetNameFinal(PlaylistObject?.Publisher.Value.PublisherClass) : Context.GetText(Resource.String.Lbl_By) + " " + Context.GetText(Resource.String.Lbl_Unknown);

                        if (InfoPlaylistViewPager.Adapter == null)
                        {
                            PlaylistViewPager = new PlaylistViewPager(Activity, PlaylistObject);
                            InfoPlaylistViewPager.Adapter = PlaylistViewPager;
                            InfoPlaylistViewPager.CurrentItem = 0;
                            CircleIndicator.SetViewPager(InfoPlaylistViewPager);
                        }
                        InfoPlaylistViewPager.Adapter.NotifyDataSetChanged(); 
                    }

                    StartApiService();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void StartApiService()
        {
            if (!Methods.CheckConnectivity())
                Toast.MakeText(Activity, Activity.GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
            else
                PollyController.RunRetryPolicyFunction(new List<Func<Task>> { LoadPlaylistSongs });
        }
         
        private async Task LoadPlaylistSongs()
        {
            if (Methods.CheckConnectivity())
            {
                int countList = MAdapter.SoundsList.Count;
                (int apiStatus, var respond) = await RequestsAsync.Playlist.GetPlaylistSongsAsync(PlaylistId);
                if (apiStatus == 200)
                {
                    if (respond is PlaylistSongsObject result)
                    {
                        var respondList = result.Songs?.Count;
                        if (respondList > 0)
                        {
                            if (countList > 0)
                            {
                                foreach (var item in from item in result.Songs let check = MAdapter.SoundsList.FirstOrDefault(a => a.Id == item.Id) where check == null select item)
                                {
                                    MAdapter.SoundsList.Add(item);
                                }

                                Activity.RunOnUiThread(() => { MAdapter.NotifyItemRangeInserted(countList - 1, MAdapter.SoundsList.Count - countList); });
                            }
                            else
                            {
                                MAdapter.SoundsList = new ObservableCollection<SoundDataObject>(result.Songs);
                                Activity.RunOnUiThread(() => { MAdapter.NotifyDataSetChanged(); });
                            }
                        }
                        else
                        {
                            if (MAdapter.SoundsList.Count > 10 && !MRecycler.CanScrollVertically(1))
                                Toast.MakeText(Context, Context.GetText(Resource.String.Lbl_NoMoreSongs), ToastLength.Short).Show();
                        }
                    }
                }
                else Methods.DisplayReportResult(Activity, respond);

                Activity.RunOnUiThread(ShowEmptyPage);
            }
            else
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
            }
        }

        private void ShowEmptyPage()
        {
            try
            {
                if (MAdapter.SoundsList.Count > 0)
                {
                    MRecycler.Visibility = ViewStates.Visible;
                    EmptyStateLayout.Visibility = ViewStates.Gone;
                }
                else
                {
                    MRecycler.Visibility = ViewStates.Gone;

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
                Console.WriteLine(e);
            }
        }

        //No Internet Connection 
        private void EmptyStateButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                StartApiService();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
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

                Console.WriteLine(scale);
                //TxtNamePlaylist.ScaleX = scale >= 0 ? scale : 0;
                //TxtNamePlaylist.ScaleY = scale >= 0 ? scale : 0;

                //TxtPublisherName.ScaleX = scale >= 0 ? scale : 0;
                //TxtPublisherName.ScaleY = scale >= 0 ? scale : 0;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

    }
}