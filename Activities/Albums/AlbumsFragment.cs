using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Bumptech.Glide.Integration.RecyclerView;
using Bumptech.Glide.Util;
using DeepSound.Activities.Songs.Adapters;
using DeepSound.Activities.Tabbes;
using DeepSound.Helpers.Ads;
using DeepSound.Helpers.CacheLoaders;
using DeepSound.Helpers.Controller;
using DeepSound.Helpers.MediaPlayerController;
using DeepSound.Helpers.Utils;
using DeepSoundClient.Classes.Albums;
using DeepSoundClient.Classes.Global;
using DeepSoundClient.Requests;
using Newtonsoft.Json;

namespace DeepSound.Activities.Albums
{
    public class AlbumsFragment : Fragment
    {
        #region Variables Basic

        private RecyclerView MRecycler;
        public static RowSoundLiteAdapter MAdapter;
        private LinearLayoutManager MLayoutManager;
        private ViewStub EmptyStateLayout;
        private View Inflated;
        private HomeActivity GlobalContext;
        private TextView AlbumName , CountSoungAlbumText, NameUserText; 
        private ImageView ImageCover, ImageAvatar;
        private CollapsingToolbarLayout CollapsingToolbar;
        private AppBarLayout AppBarLayout;
        private DataAlbumsObject AlbumsObject;
        private string AlbumsId = "";
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
                View view = inflater.Inflate(Resource.Layout.AlbumsLayout, container, false);

                AlbumsId = Arguments.GetString("AlbumsId") ?? "";

                InitComponent(view);
                //InitToolbar(view);
                SetRecyclerViewAdapters();

                SetDataAlbums();

                AdsGoogle.Ad_Interstitial(Context);

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
                CollapsingToolbar = view.FindViewById<CollapsingToolbarLayout>(Resource.Id.collapsingToolbar);
                CollapsingToolbar.Title = " ";  

                AppBarLayout = view.FindViewById<AppBarLayout>(Resource.Id.mainAppBarLayout);
                AppBarLayout.SetExpanded(true);

                MRecycler = view.FindViewById<RecyclerView>(Resource.Id.albums_recyler);
                EmptyStateLayout = view.FindViewById<ViewStub>(Resource.Id.viewStub);

                ImageCover = view.FindViewById<ImageView>(Resource.Id.imageCover);
                ImageAvatar = view.FindViewById<ImageView>(Resource.Id.imageAvatar);
                AlbumName = view.FindViewById<TextView>(Resource.Id.albumName);
                CountSoungAlbumText = view.FindViewById<TextView>(Resource.Id.CountSoungAlbumText);
                NameUserText = view.FindViewById<TextView>(Resource.Id.nameUserText);
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
                MAdapter = new RowSoundLiteAdapter(Activity, "AlbumsFragment") { SoundsList = new ObservableCollection<SoundDataObject>() };
                MAdapter.OnItemClick += MAdapterOnItemClick;
                MLayoutManager = new LinearLayoutManager(Activity);
                MRecycler.SetLayoutManager(MLayoutManager);
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

        //Back
        private void BackIcon_Click(object sender, EventArgs e)
        {
            GlobalContext.FragmentNavigatorBack();
        }


        //Start Play Sound 
        private void MAdapterOnItemClick(object sender, RowSoundLiteAdapterClickEventArgs e)
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
         
        #region Load Albums Songs And Data 

        private void SetDataAlbums()
        {
            try
            {
                AlbumsId = Arguments.GetString("AlbumsId") ?? "";
                if (!string.IsNullOrEmpty(AlbumsId))
                {
                    AlbumsObject = JsonConvert.DeserializeObject<DataAlbumsObject>(Arguments.GetString("ItemData") ?? "");
                    if (AlbumsObject != null)
                    {
                        CollapsingToolbar.Title = AlbumsObject.Title;

                        AlbumName.Text = AlbumsObject.Title;
                        CountSoungAlbumText.Text = AlbumsObject.CountSongs + " " + Context.GetText(Resource.String.Lbl_Songs);
                        NameUserText.Text = AlbumsObject.Publisher.Name;

                        GlideImageLoader.LoadImage(Activity, AlbumsObject.Thumbnail, ImageCover, ImageStyle.CenterCrop, ImagePlaceholders.Drawable);
                        GlideImageLoader.LoadImage(Activity, AlbumsObject.Publisher.Avatar, ImageAvatar, ImageStyle.CenterCrop, ImagePlaceholders.Drawable);
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
                PollyController.RunRetryPolicyFunction(new List<Func<Task>> { LoadAlbumsSongs });
        }

        private async Task LoadAlbumsSongs()
        {
            if (Methods.CheckConnectivity())
            {
                int countList = MAdapter.SoundsList.Count;
                (int apiStatus, var respond) = await RequestsAsync.Albums.GetAlbumSongsAsync(AlbumsId);
                if (apiStatus == 200)
                {
                    if (respond is GetAlbumSongsObject result)
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

                                Activity.RunOnUiThread(() =>
                                {
                                    MAdapter.NotifyItemRangeInserted(countList - 1,MAdapter.SoundsList.Count - countList); });
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

    }
}