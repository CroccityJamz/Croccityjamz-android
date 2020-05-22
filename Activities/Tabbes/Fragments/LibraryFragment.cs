using System;
using Android.Graphics;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V7.Widget;
using Android.Views;
using Com.Sothree.Slidinguppanel;
using DeepSound.Activities.Library;
using DeepSound.Activities.Tabbes.Adapters;

namespace DeepSound.Activities.Tabbes.Fragments
{
    public class LibraryFragment : Fragment
    {
        #region Variables Basic

        private HomeActivity GlobalContext;
        private RecyclerView LibraryRecyclerView;
        public LibraryAdapter MAdapter;
        private LinearLayoutManager MLayoutManager;
        
        public LikedFragment LikedFragment;
        public RecentlyPlayedFragment RecentlyPlayedFragment;
        public FavoritesFragment FavoritesFragment;
        public LatestDownloadsFragment LatestDownloadsFragment;
        public SharedFragment SharedFragment;
        public PurchasesFragment PurchasesFragment;
         
        #endregion

        #region General

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            HasOptionsMenu = true;
            // Create your fragment here
            GlobalContext = (HomeActivity) Activity;
             
            if (MAdapter == null)
                MAdapter = new LibraryAdapter(Activity);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            try
            {
                View view = inflater.Inflate(Resource.Layout.TLibraryLayout, container, false);

                InitComponent(view);
                InitToolbar(view);
                SetRecyclerViewAdapters();

                if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
                {
                    Activity.Window.ClearFlags(WindowManagerFlags.TranslucentStatus);
                    Activity.Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
                    Activity.Window.SetStatusBarColor(Color.ParseColor(AppSettings.MainColor));
                }

                MAdapter.ItemClick += MAdapterOnItemClick;

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
                LibraryRecyclerView = (RecyclerView)view.FindViewById(Resource.Id.LibraryRecyler); 
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
                MLayoutManager = new LinearLayoutManager(Activity);
                LibraryRecyclerView.SetLayoutManager(MLayoutManager);
                LibraryRecyclerView.SetAdapter(MAdapter);
                LibraryRecyclerView.HasFixedSize = true;
                LibraryRecyclerView.SetItemViewCacheSize(10);
                LibraryRecyclerView.GetLayoutManager().ItemPrefetchEnabled = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        #region Event

        private void MAdapterOnItemClick(object sender, LibraryAdapterClickEventArgs e)
        {
            try
            {
                var position = e.Position;
                if (position >= 0)
                {
                    var item = MAdapter.GetItem(position);
                    if (item != null)
                    {
                        if (item.SectionId == "1") // Liked 
                        {
                            LikedFragment = new LikedFragment();
                            GlobalContext.FragmentBottomNavigator.DisplayFragment(LikedFragment);
                        }
                        else if (item.SectionId == "2") // Recently Played 
                        {
                            RecentlyPlayedFragment = new RecentlyPlayedFragment();
                            GlobalContext.FragmentBottomNavigator.DisplayFragment(RecentlyPlayedFragment);
                        }
                        else if (item.SectionId == "3") // Favorites 
                        {
                            FavoritesFragment = new FavoritesFragment();
                            GlobalContext.FragmentBottomNavigator.DisplayFragment(FavoritesFragment);
                        }
                        else if (item.SectionId == "4") // Latest Downloads 
                        {
                            LatestDownloadsFragment = new LatestDownloadsFragment();
                            GlobalContext.FragmentBottomNavigator.DisplayFragment(LatestDownloadsFragment);
                        }
                        else if (item.SectionId == "5") // Shared
                        {
                            SharedFragment = new SharedFragment();
                            GlobalContext.FragmentBottomNavigator.DisplayFragment(SharedFragment);
                        }
                        else if (item.SectionId == "6") // Purchases
                        {
                            PurchasesFragment = new PurchasesFragment();
                            GlobalContext.FragmentBottomNavigator.DisplayFragment(PurchasesFragment);
                        }

                        if (GlobalContext.SlidingUpPanel.GetPanelState() == SlidingUpPanelLayout.PanelState.Expanded)
                            GlobalContext.SlidingUpPanel.SetPanelState(SlidingUpPanelLayout.PanelState.Collapsed);
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #endregion 
    }
}