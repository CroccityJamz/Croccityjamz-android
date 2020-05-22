using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Android.Graphics;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Views;
using Android.Widget;
using DeepSound.Activities.Albums;
using DeepSound.Activities.Albums.Adapters;
using DeepSound.Activities.Search;
using DeepSound.Activities.Songs;
using DeepSound.Activities.Songs.Adapters;
using DeepSound.Helpers.Controller;
using DeepSound.Helpers.MediaPlayerController;
using DeepSound.Helpers.Utils;
using DeepSoundClient.Classes.Albums;
using DeepSoundClient.Classes.Common;
using DeepSoundClient.Classes.Global;
using DeepSoundClient.Requests;
using Newtonsoft.Json;
using SearchView = Android.Support.V7.Widget.SearchView;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Fragment = Android.Support.V4.App.Fragment;

namespace DeepSound.Activities.Tabbes.Fragments
{
    public class BrowseFragment : Fragment , View.IOnClickListener, View.IOnFocusChangeListener
    {
        #region Variables Basic

        private HAlbumsAdapter AlbumsAdapter;
        public HSoundAdapter TopSongsSoundAdapter;
        private HomeActivity GlobalContext;
        private SwipeRefreshLayout SwipeRefreshLayout;
        private ViewStub EmptyStateLayout, TopSongsViewStub, TopAlbumsViewStub;
        private View Inflated, TopSongsInflated, TopAlbumsInflated;
        private SearchView SearchBox;
        public SearchFragment SearchFragment;
        private AlbumsFragment AlbumsFragment;

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
                View view = inflater.Inflate(Resource.Layout.TBrowseLayout, container, false);

                InitComponent(view);
                InitToolbar(view);
                SetRecyclerViewAdapters();

                StartApiService();
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
                EmptyStateLayout = (ViewStub)view.FindViewById(Resource.Id.viewStub);
                TopSongsViewStub = (ViewStub)view.FindViewById(Resource.Id.viewStubTopSongs);
                TopAlbumsViewStub = (ViewStub)view.FindViewById(Resource.Id.viewStubTopAlbums);

                SwipeRefreshLayout = (SwipeRefreshLayout)view.FindViewById(Resource.Id.swipeRefreshLayout);
                SwipeRefreshLayout.SetColorSchemeResources(Android.Resource.Color.HoloBlueLight, Android.Resource.Color.HoloGreenLight, Android.Resource.Color.HoloOrangeLight, Android.Resource.Color.HoloRedLight);
                SwipeRefreshLayout.Refreshing = true;
                SwipeRefreshLayout.Enabled = true;
                SwipeRefreshLayout.SetProgressBackgroundColorSchemeColor(AppSettings.SetTabDarkTheme ? Color.ParseColor("#424242") : Color.ParseColor("#f7f7f7"));
                SwipeRefreshLayout.Refresh += SwipeRefreshLayoutOnRefresh; 
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
                GlobalContext.SetToolBar(toolbar, "", false);

                SearchBox = view.FindViewById<SearchView>(Resource.Id.browseSearchBox);
                SearchBox.SetIconifiedByDefault(false);
                
                if ((int) Build.VERSION.SdkInt < 23)
                {
                    SearchBox.SetOnClickListener(this);
                    SearchBox.SetOnSearchClickListener(this);
                    SearchBox.SetOnQueryTextFocusChangeListener(this);
                }

                //Change text colors
                var editText = SearchBox.FindViewById<EditText>(Resource.Id.search_src_text);
                editText.SetHintTextColor(Color.Gray);
                editText.SetTextColor(Color.Black);
                Methods.SetFocusable(editText); 
                editText.Touch += EditTextOnClick;

                ImageView searchViewIcon = (ImageView)SearchBox.FindViewById(Resource.Id.search_mag_icon); 
                searchViewIcon.SetColorFilter(Color.Gray);
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
                //Top Songs RecyclerView >> LinearLayoutManager.Horizontal 
                TopSongsSoundAdapter = new HSoundAdapter(Activity) { SoundsList = new ObservableCollection<SoundDataObject>() };
                TopSongsSoundAdapter.OnItemClick += TopSongsSoundAdapterOnOnItemClick;

                // Top Albums RecyclerView >> LinearLayoutManager.Horizontal
                AlbumsAdapter =  new HAlbumsAdapter(Activity);
                AlbumsAdapter.ItemClick += AlbumsAdapterOnItemClick; 
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
         
        #endregion

        #region Events

        private void SwipeRefreshLayoutOnRefresh(object sender, EventArgs e)
        {
            try
            {
                TopSongsSoundAdapter.SoundsList.Clear();
                TopSongsSoundAdapter.NotifyDataSetChanged();

                AlbumsAdapter.AlbumsList.Clear();
                AlbumsAdapter.NotifyDataSetChanged();
                 
                EmptyStateLayout.Visibility = ViewStates.Gone;

                StartApiService();
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
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void TopSongsSoundAdapterOnOnItemClick(object sender, HSoundAdapterClickEventArgs e)
        {
            try
            {
                if (e.Position > -1)
                {
                    var item = TopSongsSoundAdapter.GetItem(e.Position);
                    if (item != null)
                    {
                        Constant.PlayPos = e.Position;
                        GlobalContext?.SoundController?.StartPlaySound(item, TopSongsSoundAdapter.SoundsList);
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #endregion

        #region Load Data 

        private void StartApiService()
        {
            if (!Methods.CheckConnectivity())
                Toast.MakeText(Activity, Activity.GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
            else
                PollyController.RunRetryPolicyFunction(new List<Func<Task>> { LoadBrowse });
        }

        private async Task LoadBrowse()
        {
            if (Methods.CheckConnectivity())
            {
                int countSongsList = TopSongsSoundAdapter.SoundsList.Count;
                int countAlbumsList = AlbumsAdapter.AlbumsList.Count;
                (int apiStatus, var respond) = await RequestsAsync.Common.GetTrendingAsync();
                if (apiStatus == 200)
                {
                    if (respond is GetTrendingObject result)
                    {
                        var respondSongsList = result.TopSongs?.Count;
                        if (respondSongsList > 0)
                        {
                            if (countSongsList > 0)
                            {
                                foreach (var item in from item in result.TopSongs let check = TopSongsSoundAdapter.SoundsList.FirstOrDefault(a => a.Id == item.Id) where check == null select item)
                                {
                                    TopSongsSoundAdapter.SoundsList.Add(item);
                                }
                            }
                            else
                            {
                                TopSongsSoundAdapter.SoundsList = new ObservableCollection<SoundDataObject>(result.TopSongs);
                            }
                        }
                        else
                        {
                            if (TopSongsSoundAdapter.SoundsList.Count > 10)
                                Toast.MakeText(Context, Context.GetText(Resource.String.Lbl_NoMoreSongs), ToastLength.Short).Show();
                        }

                        var respondList = result.TopAlbums?.Count;
                        if (respondList > 0)
                        {
                            if (countAlbumsList > 0)
                            {
                                foreach (var item in from item in result.TopAlbums let check = AlbumsAdapter.AlbumsList.FirstOrDefault(a => a.Id == item.Id) where check == null select item)
                                {
                                    AlbumsAdapter.AlbumsList.Add(item);
                                }
                            }
                            else
                            {
                                AlbumsAdapter.AlbumsList = new ObservableCollection<DataAlbumsObject>(result.TopAlbums);
                            }
                        }
                        else
                        {
                            if (AlbumsAdapter.AlbumsList.Count > 10)
                                Toast.MakeText(Context, Context.GetText(Resource.String.Lbl_NoMoreAlbums), ToastLength.Short).Show();
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
                SwipeRefreshLayout.Refreshing = false;

                if (TopSongsSoundAdapter.SoundsList.Count > 0)
                {
                    if (TopSongsInflated == null)
                        TopSongsInflated = TopSongsViewStub.Inflate();

                    TemplateRecyclerInflater recyclerInflater = new TemplateRecyclerInflater();
                    recyclerInflater.InflateLayout<SoundDataObject>(Activity, TopSongsInflated, TopSongsSoundAdapter, TemplateRecyclerInflater.TypeLayoutManager.LinearLayoutManagerHorizontal, 0, true, Context.GetText(Resource.String.Lbl_TopSongs_Title));
                    if (!recyclerInflater.MainLinear.HasOnClickListeners)
                    {
                        recyclerInflater.MainLinear.Click += null;
                        recyclerInflater.MainLinear.Click += TopSongsMoreOnClick;
                    }
                }

                if (AlbumsAdapter.AlbumsList?.Count > 0)
                {
                    if (TopAlbumsInflated == null)
                        TopAlbumsInflated = TopAlbumsViewStub.Inflate();

                    TemplateRecyclerInflater recyclerInflater = new TemplateRecyclerInflater();
                    recyclerInflater.InflateLayout<DataAlbumsObject>(Activity, TopAlbumsInflated, AlbumsAdapter, TemplateRecyclerInflater.TypeLayoutManager.GridLayoutManagerVertical, 2, true, Context.GetText(Resource.String.Lbl_TopAlbums_Title));
                    if (!recyclerInflater.MainLinear.HasOnClickListeners)
                    {
                        recyclerInflater.MainLinear.Click += null;
                        recyclerInflater.MainLinear.Click += TopAlbumsMoreOnClick;
                    }
                }

                if (TopSongsSoundAdapter.SoundsList?.Count == 0 && AlbumsAdapter.AlbumsList?.Count == 0)
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
                SwipeRefreshLayout.Refreshing = false;
                Console.WriteLine(e);
            }
        }

        private void TopAlbumsMoreOnClick(object sender, EventArgs e)
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
                bundle.PutString("SongsType", "BrowseTopSongs");

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

        #region SearchView

        private void EditTextOnClick(object sender, View.TouchEventArgs e)
        {
            try
            {
                if (e.Event.Action != MotionEventActions.Down) return;

                Bundle bundle = new Bundle();
                bundle.PutString("Key", "");
                SearchFragment = new SearchFragment
                {
                    Arguments = bundle
                };
                GlobalContext.FragmentBottomNavigator.DisplayFragment(SearchFragment);
                SearchBox.ClearFocus();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public void OnClick(View v)
        {
            try
            {
                if (v.Id != SearchBox.Id)
                    return;

                Bundle bundle = new Bundle();
                bundle.PutString("Key", "");
                SearchFragment = new SearchFragment
                {
                    Arguments = bundle
                };
                GlobalContext.FragmentBottomNavigator.DisplayFragment(SearchFragment);
                SearchBox.ClearFocus();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public void OnFocusChange(View v, bool hasFocus)
        {
            try
            {
                if (v.Id != SearchBox.Id || !hasFocus)
                    return;

                Bundle bundle = new Bundle();
                bundle.PutString("Key", "");
                SearchFragment = new SearchFragment
                {
                    Arguments = bundle
                };
                GlobalContext.FragmentBottomNavigator.DisplayFragment(SearchFragment);
                SearchBox.ClearFocus();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #endregion 
    }
}