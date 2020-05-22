using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using DeepSound.Activities.Library;
using DeepSound.Activities.Playlist;
using DeepSound.Activities.Playlist.Adapters;
using DeepSound.Helpers.Controller;
using DeepSound.Helpers.Model;
using DeepSound.Helpers.Utils;
using DeepSoundClient.Classes.Playlist;
using DeepSoundClient.Requests;
using Liaoinstan.SpringViewLib.Widgets;
using Newtonsoft.Json;
using DefaultHeader = DeepSound.Helpers.PullSwipeStyles.DefaultHeader;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace DeepSound.Activities.Tabbes.Fragments
{
    public class PlaylistFragment : Fragment, SpringView.IOnFreshListener
    {
        #region Variables Basic

        public HPlaylistAdapter PlaylistAdapter;
        private HPlaylistAdapter PublicPlaylistAdapter;
        private HomeActivity GlobalContext;
        private SpringView SwipeRefreshLayout;
        private ViewStub EmptyStateLayout, PublicPlaylistViewStub, PlaylistViewStub;
        private View Inflated , PublicPlaylistInflated, PlaylistInflated;
        private RecyclerViewOnScrollListener PlaylistScrollEvent, PublicPlaylistScrollEvent;
        private PlaylistProfileFragment PlaylistProfileFragment;
        private ProgressBar ProgressBar;
        private FloatingActionButton BtnAdd;
        public MyPlaylistFragment MyPlaylistFragment;
        private TemplateRecyclerInflater RecyclerInflaterPlaylist, RecyclerInflaterPublicPlaylist;
       
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
                View view = inflater.Inflate(Resource.Layout.TPlaylistLayout, container, false);

                InitComponent(view);
                InitToolbar(view);
                SetRecyclerViewAdapters();

                if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
                {
                    Activity.Window.ClearFlags(WindowManagerFlags.TranslucentStatus);
                    Activity.Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
                    Activity.Window.SetStatusBarColor(Color.ParseColor(AppSettings.MainColor));
                }

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
                ProgressBar = view.FindViewById<ProgressBar>(Resource.Id.progress);
                ProgressBar.Visibility = ViewStates.Visible;

                PlaylistViewStub = (ViewStub)view.FindViewById(Resource.Id.viewStubPlaylist);
                PublicPlaylistViewStub = (ViewStub)view.FindViewById(Resource.Id.viewStubPublicePlaylist);
                EmptyStateLayout = (ViewStub)view.FindViewById(Resource.Id.viewStub);

                SwipeRefreshLayout = (SpringView)view.FindViewById(Resource.Id.material_style_ptr_frame);
                SwipeRefreshLayout.SetType(SpringView.Type.Overlap);
                SwipeRefreshLayout.Header = new DefaultHeader(Activity);
                SwipeRefreshLayout.Footer = new Helpers.PullSwipeStyles.DefaultFooter(Activity);
                SwipeRefreshLayout.Enable = true;
                SwipeRefreshLayout.SetListener(this);

                BtnAdd = (FloatingActionButton)view.FindViewById(Resource.Id.floatingAdd); 
                BtnAdd.Visibility = UserDetails.IsLogin ? ViewStates.Visible : ViewStates.Gone;
                BtnAdd.Click += BtnAddOnClick;
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
                GlobalContext.SetToolBar(toolbar,"",false); 
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
                //Playlist RecyclerView
                PlaylistAdapter = new HPlaylistAdapter(Activity,true) { PlaylistList =  new ObservableCollection<PlaylistDataObject>() };
                PlaylistAdapter.OnItemClick += PlaylistAdapterOnOnItemClick;

                //Public Playlist RecyclerView
                PublicPlaylistAdapter = new HPlaylistAdapter(Activity) { PlaylistList = new ObservableCollection<PlaylistDataObject>() };
                PublicPlaylistAdapter.OnItemClick += PublicPlaylistAdapterOnOnItemClick;
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
                PlaylistAdapter.PlaylistList.Clear();
                PlaylistAdapter.NotifyDataSetChanged();

                PublicPlaylistAdapter.PlaylistList.Clear();
                PublicPlaylistAdapter.NotifyDataSetChanged();

                EmptyStateLayout.Visibility = ViewStates.Gone;

                StartApiService();
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
                if (PlaylistScrollEvent.IsLoading == false)
                {
                    PlaylistScrollEvent.IsLoading = true;
                    var item = PlaylistAdapter.PlaylistList.LastOrDefault();
                    if (item != null)
                    { 
                        GetPlaylist(item.Id.ToString()).ConfigureAwait(false);
                        PlaylistScrollEvent.IsLoading = false;
                    }
                }

                if (PublicPlaylistScrollEvent.IsLoading == false)
                {
                    PublicPlaylistScrollEvent.IsLoading = true;
                    var item = PublicPlaylistAdapter.PlaylistList.LastOrDefault();
                    if (item != null)
                    { 
                        GetPublicPlaylist(item.Id.ToString()).ConfigureAwait(false);
                        PublicPlaylistScrollEvent.IsLoading = false;
                    }
                } 
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        #region Get Public Playlist Api 

        private void StartApiService()
        {
            if (Methods.CheckConnectivity())
            {
                PollyController.RunRetryPolicyFunction(UserDetails.IsLogin
                    ? new List<Func<Task>> {() => GetPublicPlaylist(), () => GetPlaylist()}
                    : new List<Func<Task>> {() => GetPublicPlaylist()});
            }
            else
            {
                SwipeRefreshLayout.OnFinishFreshAndLoad();

                Inflated = EmptyStateLayout.Inflate();
                EmptyStateInflater x = new EmptyStateInflater();
                x.InflateLayout(Inflated, EmptyStateInflater.Type.NoConnection);
                if (!x.EmptyStateButton.HasOnClickListeners)
                {
                    x.EmptyStateButton.Click += null;
                    x.EmptyStateButton.Click += EmptyStateButtonOnClick;
                }

                Toast.MakeText(Context, GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Long).Show();
            }
        }
        
        private async Task GetPublicPlaylist(string offsetPublicPlaylist = "0")
        {
            if (PublicPlaylistScrollEvent != null && PublicPlaylistScrollEvent.IsLoading)
                return;

            if (PublicPlaylistScrollEvent != null) PublicPlaylistScrollEvent.IsLoading = true;

            int countList = PublicPlaylistAdapter.PlaylistList.Count;
            (int apiStatus, var respond) = await RequestsAsync.Playlist.GetPublicPlaylistAsync("20", offsetPublicPlaylist);
            if (apiStatus.Equals(200))
            {
                if (respond is PlaylistObject result)
                {
                    var respondList = result.Playlist.Count;
                    if (respondList > 0)
                    {
                        foreach (var item in from item in result.Playlist let check = PublicPlaylistAdapter.PlaylistList.FirstOrDefault(a => a.Id == item.Id) where check == null select item)
                        {
                            PublicPlaylistAdapter.PlaylistList.Add(item);
                        }

                        if (countList > 0)
                        {
                            Activity.RunOnUiThread(() => { PublicPlaylistAdapter.NotifyItemRangeInserted(countList - 1, PublicPlaylistAdapter.PlaylistList.Count - countList); });
                        }
                        else
                        {
                            Activity.RunOnUiThread(() =>
                            {
                                if (PublicPlaylistInflated == null)
                                    PublicPlaylistInflated = PublicPlaylistViewStub.Inflate();

                                RecyclerInflaterPublicPlaylist = new TemplateRecyclerInflater();
                                RecyclerInflaterPublicPlaylist.InflateLayout<PlaylistDataObject>(Activity, PublicPlaylistInflated, PublicPlaylistAdapter, TemplateRecyclerInflater.TypeLayoutManager.LinearLayoutManagerHorizontal, 0, true, Activity.GetString(Resource.String.Lbl_Playlist));

                                if (PublicPlaylistScrollEvent == null)
                                {
                                    RecyclerViewOnScrollListener playlistRecyclerViewOnScrollListener = new RecyclerViewOnScrollListener(RecyclerInflaterPublicPlaylist.LayoutManager);
                                    PublicPlaylistScrollEvent = playlistRecyclerViewOnScrollListener;
                                    PublicPlaylistScrollEvent.LoadMoreEvent += PublicPlaylistScrollEventOnLoadMoreEvent;
                                    RecyclerInflaterPublicPlaylist.Recyler.AddOnScrollListener(playlistRecyclerViewOnScrollListener);
                                    PublicPlaylistScrollEvent.IsLoading = false;
                                }
                            }); 
                        }
                    }
                    else
                    {
                        if (RecyclerInflaterPublicPlaylist.Recyler != null)
                            if (PublicPlaylistAdapter.PlaylistList.Count > 10 && !RecyclerInflaterPublicPlaylist.Recyler.CanScrollVertically(1))
                                Toast.MakeText(Context, Context.GetText(Resource.String.Lbl_NoMorePlaylist), ToastLength.Short).Show();
                    }
                }
            }
            else Methods.DisplayReportResult(Activity, respond);
             
            Activity.RunOnUiThread(ShowEmptyPage); 
        }

        private async Task GetPlaylist(string offsetPlaylist = "0")
        {
            if (PlaylistScrollEvent != null && PlaylistScrollEvent.IsLoading)
                return;

            if (PlaylistScrollEvent != null) PlaylistScrollEvent.IsLoading = true;
              
            if (!UserDetails.IsLogin)
                return;
             
            int countList = PlaylistAdapter.PlaylistList.Count;
            (int apiStatus, var respond) = await RequestsAsync.Playlist.GetPlaylistAsync(UserDetails.UserId.ToString(), "20", offsetPlaylist);
            if (apiStatus.Equals(200))
            {
                if (respond is PlaylistObject result)
                {
                    var respondList = result.Playlist.Count;
                    if (respondList > 0)
                    {
                        foreach (var item in from item in result.Playlist let check = PlaylistAdapter.PlaylistList.FirstOrDefault(a => a.Id == item.Id) where check == null select item)
                        {
                            PlaylistAdapter.PlaylistList.Add(item);
                        }

                        if (countList > 0)
                        {
                            PlaylistAdapter.NotifyItemRangeInserted(countList - 1, PlaylistAdapter.PlaylistList.Count - countList);
                        }
                        else
                        {
                            if (PlaylistInflated == null)
                                PlaylistInflated = PlaylistViewStub.Inflate();

                            RecyclerInflaterPlaylist = new TemplateRecyclerInflater();
                            RecyclerInflaterPlaylist.InflateLayout<PlaylistDataObject>(Activity, PlaylistInflated, PlaylistAdapter, TemplateRecyclerInflater.TypeLayoutManager.LinearLayoutManagerHorizontal, 0, true, Activity.GetString(Resource.String.Lbl_MyPlaylist));
                            if (!RecyclerInflaterPlaylist.MainLinear.HasOnClickListeners)
                            {
                                RecyclerInflaterPlaylist.MainLinear.Click += null;
                                RecyclerInflaterPlaylist.MainLinear.Click += PlaylistMoreOnClick;
                            }

                            if (PlaylistScrollEvent == null)
                            {
                                RecyclerViewOnScrollListener playlistRecyclerViewOnScrollListener = new RecyclerViewOnScrollListener(RecyclerInflaterPlaylist.LayoutManager);
                                PlaylistScrollEvent = playlistRecyclerViewOnScrollListener;
                                PlaylistScrollEvent.LoadMoreEvent += PlaylistScrollEventOnLoadMoreEvent;
                                RecyclerInflaterPlaylist.Recyler.AddOnScrollListener(playlistRecyclerViewOnScrollListener);
                                PlaylistScrollEvent.IsLoading = false;
                            }
                        }
                    }
                    else
                    {
                        if (RecyclerInflaterPlaylist.Recyler != null)
                            if (PlaylistAdapter.PlaylistList.Count > 10 && !RecyclerInflaterPlaylist.Recyler.CanScrollVertically(1))
                                Toast.MakeText(Context, Context.GetText(Resource.String.Lbl_NoMorePlaylist), ToastLength.Short).Show();
                    }
                }
            }
            else Methods.DisplayReportResult(Activity, respond);

            Activity.RunOnUiThread(ShowEmptyPage);

        }
         
        private void ShowEmptyPage()
        {
            try
            {
                if (PublicPlaylistScrollEvent != null) PublicPlaylistScrollEvent.IsLoading = false;
                if (PlaylistScrollEvent != null) PlaylistScrollEvent.IsLoading = false;

                SwipeRefreshLayout.OnFinishFreshAndLoad();

                if (ProgressBar.Visibility == ViewStates.Visible)
                    ProgressBar.Visibility = ViewStates.Gone;
                 
                if (PublicPlaylistAdapter?.PlaylistList?.Count == 0 && PlaylistAdapter?.PlaylistList?.Count == 0)
                {

                    if (Inflated == null)
                        Inflated = EmptyStateLayout.Inflate();

                    EmptyStateInflater x = new EmptyStateInflater();
                    x.InflateLayout(Inflated, EmptyStateInflater.Type.NoPlaylist);
                    if (x.EmptyStateButton.HasOnClickListeners)
                    {
                        x.EmptyStateButton.Click += null;
                    }

                    EmptyStateLayout.Visibility = ViewStates.Visible;
                } 
            }
            catch (Exception e)
            {
                if (PublicPlaylistScrollEvent != null) PublicPlaylistScrollEvent.IsLoading = false;
                if (PlaylistScrollEvent != null) PlaylistScrollEvent.IsLoading = false;

                SwipeRefreshLayout.OnFinishFreshAndLoad();
                if (ProgressBar.Visibility == ViewStates.Visible)
                    ProgressBar.Visibility = ViewStates.Gone;
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

        #region Scroll

        private void PublicPlaylistScrollEventOnLoadMoreEvent(object sender, EventArgs e)
        {
            try
            {
                //Code get last id where LoadMore >>
                var item = PublicPlaylistAdapter.PlaylistList.LastOrDefault(); 
                if (item != null && !string.IsNullOrEmpty(item.Id.ToString()) && !PublicPlaylistScrollEvent.IsLoading)
                    GetPublicPlaylist(item.Id.ToString()).ConfigureAwait(false); 
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
         
        //My Playlist
        private void PlaylistScrollEventOnLoadMoreEvent(object sender, EventArgs e)
        {
            try
            {
                //Code get last id where LoadMore >>
                var item = PlaylistAdapter.PlaylistList.LastOrDefault();
                if (item != null && !string.IsNullOrEmpty(item.Id.ToString()) && !PlaylistScrollEvent.IsLoading)
                    GetPlaylist(item.Id.ToString()).ConfigureAwait(false); 
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
         
        #endregion

        #region Event 
         
        private void PlaylistAdapterOnOnItemClick(object sender, PlaylistAdapterClickEventArgs e)
        {
            try
            {
                var item = PlaylistAdapter.PlaylistList[e.Position];
                if (item != null)
                {
                    Bundle bundle = new Bundle();
                    bundle.PutString("ItemData", JsonConvert.SerializeObject(item));
                    bundle.PutString("PlaylistId", item.Id.ToString());

                    PlaylistProfileFragment = new PlaylistProfileFragment
                    {
                        Arguments = bundle
                    };

                    GlobalContext.FragmentBottomNavigator.DisplayFragment(PlaylistProfileFragment);
                } 
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void PublicPlaylistAdapterOnOnItemClick(object sender, PlaylistAdapterClickEventArgs e)
        {
            try
            {
                var item = PublicPlaylistAdapter.PlaylistList[e.Position];
                if (item != null)
                {
                    Bundle bundle = new Bundle();
                    bundle.PutString("ItemData", JsonConvert.SerializeObject(item));
                    bundle.PutString("PlaylistId", item.Id.ToString());

                    PlaylistProfileFragment = new PlaylistProfileFragment
                    {
                        Arguments = bundle
                    };

                    GlobalContext.FragmentBottomNavigator.DisplayFragment(PlaylistProfileFragment);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void PlaylistMoreOnClick(object sender, EventArgs e)
        {
            try
            {
                MyPlaylistFragment = new MyPlaylistFragment();
                GlobalContext.FragmentBottomNavigator.DisplayFragment(MyPlaylistFragment);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        // Create Playlist
        private void BtnAddOnClick(object sender, EventArgs e)
        {
            try
            {
                if (!UserDetails.IsLogin)
                {
                    PopupDialogController dialog = new PopupDialogController(GlobalContext, null, "Login");
                    dialog.ShowNormalDialog(GlobalContext.GetText(Resource.String.Lbl_Login), GlobalContext.GetText(Resource.String.Lbl_Message_Sorry_signin), GlobalContext.GetText(Resource.String.Lbl_Yes), GlobalContext.GetText(Resource.String.Lbl_No));
                    return;
                }

                StartActivity(new Intent(Activity, typeof(CreatePlaylistActivity)));
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #endregion
    }
}