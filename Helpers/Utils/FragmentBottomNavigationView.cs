using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Graphics;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Views;
using Com.Gigamole.Navigationtabbar.Ntb;
using Com.Sothree.Slidinguppanel;
using DeepSound.Activities.Tabbes;
using DeepSound.Helpers.Ads;
using DeepSound.Helpers.Model;
using Fragment = Android.Support.V4.App.Fragment;
using FragmentTransaction = Android.Support.V4.App.FragmentTransaction;

namespace DeepSound.Helpers.Utils
{
    public class FragmentBottomNavigationView
    {
        private readonly HomeActivity Context;

        public JavaList<NavigationTabBar.Model> Models;
        public readonly List<Fragment> FragmentListTab0 = new List<Fragment>();
        public readonly List<Fragment> FragmentListTab1 = new List<Fragment>();
        public readonly List<Fragment> FragmentListTab2 = new List<Fragment>();
        public readonly List<Fragment> FragmentListTab3 = new List<Fragment>();
        public readonly List<Fragment> FragmentListTab4 = new List<Fragment>();

        public int PageNumber;

        public FragmentBottomNavigationView(Activity context)
        {
            try
            {
                Context = (HomeActivity)context;
                FragmentListTab0 = new List<Fragment>();
                FragmentListTab1 = new List<Fragment>();
                FragmentListTab2 = new List<Fragment>();
                FragmentListTab3 = new List<Fragment>();
                FragmentListTab4 = new List<Fragment>();
            }
            catch (Exception e)
            {
                Console.WriteLine(e); 
            } 
        }

        public void SetupNavigation(NavigationTabBar navigationTabBar)
        {
            try
            { 
                Models = new JavaList<NavigationTabBar.Model>
                {
                    new NavigationTabBar.Model.Builder(ContextCompat.GetDrawable(Context, Resource.Drawable.ic_tab_feed),Color.ParseColor("#ffffff")).Title(Context.GetText(Resource.String.Lbl_Feed)).Build(),
                    new NavigationTabBar.Model.Builder(ContextCompat.GetDrawable(Context, Resource.Drawable.ic_tab_playlist),Color.ParseColor("#ffffff")).Title(Context.GetText(Resource.String.Lbl_Playlist)).Build(),
                    new NavigationTabBar.Model.Builder(ContextCompat.GetDrawable(Context, Resource.Drawable.ic_tab_browse),Color.ParseColor("#ffffff")).Title(Context.GetText(Resource.String.Lbl_Browse)).Build(),
                };
                
                if (UserDetails.IsLogin)
                {
                    Models.Add(new NavigationTabBar.Model.Builder(ContextCompat.GetDrawable(Context, Resource.Drawable.ic_tab_library),Color.ParseColor("#ffffff")).Title(Context.GetText(Resource.String.Lbl_Library)).Build());
                    Models.Add(new NavigationTabBar.Model.Builder(ContextCompat.GetDrawable(Context, Resource.Drawable.ic_tab_profile), Color.ParseColor("#ffffff")).Title(Context.GetText(Resource.String.Lbl_Profile)).Build());
                }

                var eee = NavigationTabBar.BadgeGravity.Top;
                navigationTabBar.SetBadgeGravity(eee);
                navigationTabBar.BadgeBgColor = Color.ParseColor(AppSettings.MainColor);
                navigationTabBar.BadgeTitleColor = Color.White;
                 
                if (AppSettings.SetTabColoredTheme)
                {
                    Models.First(a => a.Title == Context.GetText(Resource.String.Lbl_Feed)).Color = Color.ParseColor(AppSettings.TabColoredColor);
                    Models.First(a => a.Title == Context.GetText(Resource.String.Lbl_Playlist)).Color = Color.ParseColor(AppSettings.TabColoredColor);
                    Models.First(a => a.Title == Context.GetText(Resource.String.Lbl_Browse)).Color = Color.ParseColor(AppSettings.TabColoredColor);

                    if (UserDetails.IsLogin)
                    {
                        Models.First(a => a.Title == Context.GetText(Resource.String.Lbl_Library)).Color = Color.ParseColor(AppSettings.TabColoredColor);
                        Models.First(a => a.Title == Context.GetText(Resource.String.Lbl_Profile)).Color = Color.ParseColor(AppSettings.TabColoredColor);
                    }
                    navigationTabBar.BgColor = Color.ParseColor(AppSettings.MainColor);
                    navigationTabBar.ActiveColor = Color.White;
                    navigationTabBar.InactiveColor = Color.White;
                }
                else if (AppSettings.SetTabDarkTheme)
                {
                    Models.First(a => a.Title == Context.GetText(Resource.String.Lbl_Feed)).Color = Color.ParseColor(AppSettings.MainColor);
                    Models.First(a => a.Title == Context.GetText(Resource.String.Lbl_Playlist)).Color = Color.ParseColor(AppSettings.MainColor);
                    Models.First(a => a.Title == Context.GetText(Resource.String.Lbl_Browse)).Color = Color.ParseColor(AppSettings.MainColor);
                    if (UserDetails.IsLogin)
                    {
                        Models.First(a => a.Title == Context.GetText(Resource.String.Lbl_Library)).Color = Color.ParseColor(AppSettings.MainColor);
                        Models.First(a => a.Title == Context.GetText(Resource.String.Lbl_Profile)).Color = Color.ParseColor(AppSettings.MainColor);
                    }

                    navigationTabBar.BgColor = Color.ParseColor("#000000");
                    navigationTabBar.ActiveColor = Color.White;
                    navigationTabBar.InactiveColor = Color.White;
                }
                else
                {
                    Models.First(a => a.Title == Context.GetText(Resource.String.Lbl_Feed)).Color = Color.ParseColor("#ffffff");
                    Models.First(a => a.Title == Context.GetText(Resource.String.Lbl_Playlist)).Color = Color.ParseColor("#ffffff");
                    Models.First(a => a.Title == Context.GetText(Resource.String.Lbl_Browse)).Color = Color.ParseColor("#ffffff"); ;
                    if (UserDetails.IsLogin)
                    {
                        Models.First(a => a.Title == Context.GetText(Resource.String.Lbl_Library)).Color = Color.ParseColor("#ffffff");
                        Models.First(a => a.Title == Context.GetText(Resource.String.Lbl_Profile)).Color = Color.ParseColor("#ffffff");
                    }

                    navigationTabBar.BgColor = Color.White;
                    navigationTabBar.ActiveColor = Color.ParseColor(AppSettings.MainColor);
                    navigationTabBar.InactiveColor = Color.ParseColor("#bfbfbf");
                }

                navigationTabBar.Models = Models;
                 
                navigationTabBar.IsScaled = false;
                navigationTabBar.IconSizeFraction = (float)0.450;
                navigationTabBar.BehaviorEnabled = true;
                 
                //navigationTabBar.SetBadgePosition(NavigationTabBar.BadgePosition.Center);
                if (AppSettings.SetTabIsTitledWithText)
                {
                    navigationTabBar.SetTitleMode(NavigationTabBar.TitleMode.All);
                    navigationTabBar.IsTitled = true;
                }

                navigationTabBar.StartTabSelected += NavigationTabBarOnStartTabSelected;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void NavigationTabBarOnStartTabSelected(object sender, NavigationTabBar.StartTabSelectedEventArgs e)
        {
            try
            {
                switch (e.P1)
                {
                    case 0:
                        PageNumber = 0;
                        ShowFragment0();
                        AdsGoogle.Ad_Interstitial(Context);
                        break; 
                    case 1:
                        PageNumber = 1;
                        ShowFragment1();
                        AdsGoogle.Ad_RewardedVideo(Context); 
                        break; 
                    case 2:
                        PageNumber = 2;
                        //NavigationTabBar.Model tabNotifications = Models.FirstOrDefault(a => a.Title == Context.GetText(Resource.String.Lbl_Browse));
                        //tabNotifications?.HideBadge();
                        ShowFragment2();
                        AdsGoogle.Ad_Interstitial(Context);
                        break; 
                    case 3:
                        PageNumber = 3; 
                        ShowFragment3();
                        AdsGoogle.Ad_RewardedVideo(Context);
                        break;
                    case 4:
                        PageNumber = 4;
                        ShowFragment4();
                        AdsGoogle.Ad_Interstitial(Context);
                        break;  
                    default:
                        PageNumber = 0;
                        ShowFragment0();
                        break;
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private static void HideFragmentFromList(List<Fragment> fragmentList, FragmentTransaction ft)
        {
            try
            {
                if (fragmentList.Count <= 0) return;
                foreach (var fra in fragmentList.Where(fra => fra.IsAdded && fra.IsVisible))
                {
                    ft.Hide(fra);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
         
        public void DisplayFragment(Fragment newFragment)
        {
            try
            {
                FragmentTransaction ft = Context.SupportFragmentManager.BeginTransaction();

                HideFragmentFromList(FragmentListTab0, ft);
                HideFragmentFromList(FragmentListTab1, ft);
                HideFragmentFromList(FragmentListTab2, ft);
                if (UserDetails.IsLogin)
                {
                    HideFragmentFromList(FragmentListTab3, ft);
                    HideFragmentFromList(FragmentListTab4, ft);
                }
              
                if (PageNumber == 0)
                    if (!FragmentListTab0.Contains(newFragment))
                        FragmentListTab0.Add(newFragment);

                if (PageNumber == 1)
                    if (!FragmentListTab1.Contains(newFragment))
                        FragmentListTab1.Add(newFragment);

                if (PageNumber == 2)
                    if (!FragmentListTab2.Contains(newFragment))
                        FragmentListTab2.Add(newFragment);
             
                if (UserDetails.IsLogin)
                {
                    if (PageNumber == 3)
                        if (!FragmentListTab3.Contains(newFragment))
                            FragmentListTab3.Add(newFragment);

                    if (PageNumber == 4)
                        if (!FragmentListTab4.Contains(newFragment))
                            FragmentListTab4.Add(newFragment);
                }
                  

                if (!newFragment.IsAdded)
                    ft.Add(Resource.Id.mainFragmentHolder, newFragment, newFragment.Id.ToString());

                ft.Show(newFragment).Commit();

            }
            catch (Exception e)
            {
                Console.WriteLine(e);

            } 
        }

        public void DisplayFragment2(Fragment newFragment)
        {
            try
            {
                FragmentTransaction ft = Context.SupportFragmentManager.BeginTransaction();

                HideFragmentFromList(FragmentListTab0, ft);
                HideFragmentFromList(FragmentListTab1, ft);
                HideFragmentFromList(FragmentListTab2, ft);
                if (UserDetails.IsLogin)
                {

                    HideFragmentFromList(FragmentListTab3, ft);
                    HideFragmentFromList(FragmentListTab4, ft);
                }
                 
                if (PageNumber == 0)
                    if (!FragmentListTab0.Contains(newFragment))
                        FragmentListTab0.Add(newFragment);

                if (PageNumber == 1)
                    if (!FragmentListTab1.Contains(newFragment))
                        FragmentListTab1.Add(newFragment);

                if (PageNumber == 2)
                    if (!FragmentListTab2.Contains(newFragment))
                        FragmentListTab2.Add(newFragment);

                if (UserDetails.IsLogin)
                {
                    if (PageNumber == 3)
                        if (!FragmentListTab3.Contains(newFragment))
                            FragmentListTab3.Add(newFragment);

                    if (PageNumber == 4)
                        if (!FragmentListTab4.Contains(newFragment))
                            FragmentListTab4.Add(newFragment);

                }

                if (!newFragment.IsAdded)
                    ft.Add(Resource.Id.mainFragmentHolder, newFragment, newFragment.Id.ToString());

                ft.Show(newFragment).CommitAllowingStateLoss();

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void RemoveFragment(Fragment oldFragment)
        {
            try
            {
                FragmentTransaction ft = Context.SupportFragmentManager.BeginTransaction();

                if (PageNumber == 0)
                    if (FragmentListTab0.Contains(oldFragment))
                        FragmentListTab0.Remove(oldFragment);

                if (PageNumber == 1)
                    if (FragmentListTab1.Contains(oldFragment))
                        FragmentListTab1.Remove(oldFragment);

                if (PageNumber == 2)
                    if (FragmentListTab2.Contains(oldFragment))
                        FragmentListTab2.Remove(oldFragment);
                if (UserDetails.IsLogin)
                {
                    if (PageNumber == 3)
                        if (FragmentListTab3.Contains(oldFragment))
                            FragmentListTab3.Remove(oldFragment);

                    if (PageNumber == 4)
                        if (FragmentListTab4.Contains(oldFragment))
                            FragmentListTab4.Remove(oldFragment);

                }

                HideFragmentFromList(FragmentListTab0, ft);
                HideFragmentFromList(FragmentListTab1, ft);
                HideFragmentFromList(FragmentListTab2, ft);
                if (UserDetails.IsLogin)
                {
                    HideFragmentFromList(FragmentListTab3, ft);
                    HideFragmentFromList(FragmentListTab4, ft);
                }
                   
                if (oldFragment.IsAdded)
                    ft.Remove(oldFragment);

                if (PageNumber == 0)
                {
                    var currentFragment = FragmentListTab0[FragmentListTab0.Count - 1];
                    ft.Show(currentFragment).Commit();
                }
                else if (PageNumber == 1)
                {
                    var currentFragment = FragmentListTab1[FragmentListTab1.Count - 1];
                    ft.Show(currentFragment).Commit();
                }
                else if (PageNumber == 2)
                {
                    var currentFragment = FragmentListTab2[FragmentListTab2.Count - 1];
                    ft.Show(currentFragment).Commit();
                }
                else if (PageNumber == 3 && UserDetails.IsLogin)
                {
                    var currentFragment = FragmentListTab3[FragmentListTab3.Count - 1];
                    ft.Show(currentFragment).Commit();
                }
                else if (PageNumber == 4 && UserDetails.IsLogin)
                {
                    var currentFragment = FragmentListTab4[FragmentListTab4.Count - 1];
                    ft.Show(currentFragment).Commit();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            } 
        }

        public void BackStackClickFragment()
        {
            try
            {
                if (PageNumber == 0)
                {
                    if (FragmentListTab0.Count > 1)
                    {
                        var currentFragment = FragmentListTab0[FragmentListTab0.Count - 1];
                        if (currentFragment != null)
                            RemoveFragment(currentFragment);
                    }
                    else
                    {
                        Context.Finish();
                    }
                }
                else if (PageNumber == 1)
                {
                    if (FragmentListTab1.Count > 1)
                    {
                        var currentFragment = FragmentListTab1[FragmentListTab1.Count - 1];
                        if (currentFragment != null)
                            RemoveFragment(currentFragment);
                    }
                    else
                    {
                        Context.Finish();
                    }
                }
                else if (PageNumber == 2)
                {
                    if (FragmentListTab2.Count > 1)
                    {
                        var currentFragment = FragmentListTab2[FragmentListTab2.Count - 1];
                        if (currentFragment != null)
                            RemoveFragment(currentFragment);
                    }
                    else
                    {
                        Context.Finish();
                    }
                }
                else if (PageNumber == 3 && UserDetails.IsLogin)
                {
                    if (FragmentListTab3.Count > 1)
                    {
                        var currentFragment = FragmentListTab3[FragmentListTab3.Count - 1];
                        if (currentFragment != null)
                            RemoveFragment(currentFragment);
                    }
                    else
                    {
                        Context.Finish();
                    }
                }
                else if (PageNumber == 4 && UserDetails.IsLogin)
                {
                    if (FragmentListTab4.Count > 1)
                    {
                        var currentFragment = FragmentListTab4[FragmentListTab4.Count - 1];
                        if (currentFragment != null)
                            RemoveFragment(currentFragment);
                    }
                    else
                    {
                        Context.Finish();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            } 
        }

        private void ShowFragment0()
        {
            try
            {
                if (FragmentListTab0.Count < 0) return;

                Fragment currentFragment;
                // If user presses it while still on that tab it removes all fragments from the list
                if (FragmentListTab0.Count > 1)
                {
                    FragmentTransaction ft = Context.SupportFragmentManager.BeginTransaction();

                    for (var index = FragmentListTab0.Count - 1; index > 0; index--)
                    {
                        var oldFragment = FragmentListTab0[index];
                        if (FragmentListTab0.Contains(oldFragment))
                            FragmentListTab0.Remove(oldFragment);

                        if (oldFragment.IsAdded)
                            ft.Remove(oldFragment);

                        HideFragmentFromList(FragmentListTab0, ft);
                    }

                    currentFragment = FragmentListTab0[FragmentListTab0.Count - 1];
                    ft.Show(currentFragment).Commit(); 
                }
                else
                {
                    currentFragment = FragmentListTab0[FragmentListTab0.Count - 1];
                    if (currentFragment != null)
                        DisplayFragment(currentFragment);
                }

                Context.MoreMultiButtons.Visibility = ViewStates.Gone;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void ShowFragment1()
        {
            try
            {
                if (FragmentListTab1.Count < 0) return;
                var currentFragment = FragmentListTab1[FragmentListTab1.Count - 1];
                if (currentFragment != null)
                    DisplayFragment(currentFragment);

                Context.MoreMultiButtons.Visibility = ViewStates.Gone;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void ShowFragment2()
        {
            try
            {
                if (FragmentListTab2.Count < 0) return;
                var currentFragment = FragmentListTab2[FragmentListTab2.Count - 1];
                if (currentFragment != null)
                    DisplayFragment(currentFragment);

                Context.MoreMultiButtons.Visibility = ViewStates.Gone;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void ShowFragment3()
        {
            try
            {
                if (FragmentListTab3.Count < 0) return;
                var currentFragment = FragmentListTab3[FragmentListTab3.Count - 1];
                if (currentFragment != null)
                {
                    DisplayFragment(currentFragment);

                    Context.MoreMultiButtons.Visibility = ViewStates.Gone;
                } 
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void ShowFragment4()
        {
            try
            {
                if (FragmentListTab4.Count < 0) return;
                var currentFragment = FragmentListTab4[FragmentListTab4.Count - 1];
                if (currentFragment != null)
                {
                    DisplayFragment(currentFragment); 
                }

                Context.MoreMultiButtons.Visibility = Context.SlidingUpPanel != null && Context.SlidingUpPanel.GetPanelState() == SlidingUpPanelLayout.PanelState.Expanded ? ViewStates.Gone : ViewStates.Visible;

                if (!AppSettings.ShowButtonUploadSingleSong)
                {
                    Context.MoreMultiButtons.GetChildAt(0).Visibility = ViewStates.Gone; 
                }

                if (!AppSettings.ShowButtonUploadAlbum)
                {
                    Context.MoreMultiButtons.GetChildAt(1).Visibility = ViewStates.Gone;
                }

                if (!AppSettings.ShowButtonUploadSingleSong && !AppSettings.ShowButtonUploadAlbum)
                {
                    Context.MoreMultiButtons.GetChildAt(0).Visibility = ViewStates.Gone;
                    Context.MoreMultiButtons.GetChildAt(1).Visibility = ViewStates.Gone;
                    Context.MoreMultiButtons.Visibility = ViewStates.Gone;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
         
    }
}