using System;
using System.Collections.Generic;
using System.Linq;
using AFollestad.MaterialDialogs;
using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using DeepSound.Activities.Playlist;
using DeepSound.Activities.Tabbes;
using DeepSound.Helpers.Controller;
using DeepSound.Helpers.Model;
using DeepSound.Helpers.Utils;
using DeepSound.SQLite;
using DeepSoundClient.Classes.Albums;
using DeepSoundClient.Classes.Global;
using DeepSoundClient.Classes.Playlist;
using DeepSoundClient.Requests;
using Java.Lang;
using Newtonsoft.Json;
using Plugin.Share;
using Plugin.Share.Abstractions;
using Exception = System.Exception;

namespace DeepSound.Activities.Library.Listeners
{
    public class LibrarySynchronizer : Java.Lang.Object, MaterialDialog.IListCallback, MaterialDialog.ISingleButtonCallback
    {
        private readonly HomeActivity ActivityContext;
        private MorePlaylistClickEventArgs MorePlaylistArgs;
        private MoreAlbumsClickEventArgs MoreAlbumsArgs;
        private string TypeDialog = "" , OptionDialog = "";

        public LibrarySynchronizer(Activity activityContext)
        {
            try
            {
                ActivityContext = (HomeActivity) activityContext;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void AddToLiked(SoundDataObject song, int count = 0)
        {
            try
            {
                var item = ActivityContext?.LibraryFragment?.MAdapter?.LibraryList?.FirstOrDefault(a =>a.SectionId == "1");
                if (item == null) return;
                item.SongsCount = count != 0 ? count : item.SongsCount + 1;

                item.BackgroundImage = song.Thumbnail;
                ActivityContext?.LibraryFragment?.MAdapter?.NotifyItemChanged(0, "picture");

                var sqlEntity = new SqLiteDatabase();
                sqlEntity.InsertLibraryItem(item);
                sqlEntity.Dispose();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        

        public void AddToRecentlyPlayed(SoundDataObject song, int count = 0)
        {
            try
            {
                var item = ActivityContext?.LibraryFragment?.MAdapter?.LibraryList?.FirstOrDefault(a => a.SectionId == "2");
                if (item == null) return;
                 
                item.SongsCount = count != 0 ? count : item.SongsCount + 1;

                item.BackgroundImage = song.Thumbnail;  
                ActivityContext?.LibraryFragment?.MAdapter?.NotifyItemChanged(1, "picture");
                var sqlEntity = new SqLiteDatabase();
                sqlEntity.InsertLibraryItem(item);
                sqlEntity.Dispose();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            } 
        }

        public void AddToFavorites(SoundDataObject song, int count = 0)
        {
            try
            {
                var item = ActivityContext?.LibraryFragment?.MAdapter?.LibraryList?.FirstOrDefault(a =>a.SectionId == "3");
                if (item == null) return;
                item.SongsCount = count != 0 ? count : item.SongsCount + 1;

                item.BackgroundImage = song.Thumbnail;
                ActivityContext?.LibraryFragment?.MAdapter?.NotifyItemChanged(2, "picture");

                var sqlEntity = new SqLiteDatabase();
                sqlEntity.InsertLibraryItem(item);
                sqlEntity.Dispose();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void RemoveRecentlyPlayed()
        {
            try
            {
                var item = ActivityContext?.LibraryFragment?.MAdapter?.LibraryList?.FirstOrDefault(a =>a.SectionId == "2");
                if (item == null) return;
                item.SongsCount = 0;
                item.BackgroundImage = "blackdefault";
                ActivityContext?.LibraryFragment?.MAdapter?.NotifyItemChanged(1, "picture");

                var sqlEntity = new SqLiteDatabase();
                sqlEntity.InsertLibraryItem(item);
                sqlEntity.Dispose();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void AddToLatestDownloads(SoundDataObject song, int count = 0)
        {
            try
            {
                var item = ActivityContext?.LibraryFragment?.MAdapter?.LibraryList?.FirstOrDefault(a =>a.SectionId == "4");
                if (item == null) return;
                item.SongsCount = count != 0 ? count : item.SongsCount + 1;
                item.BackgroundImage = song.Thumbnail;
                ActivityContext?.LibraryFragment?.MAdapter?.NotifyItemChanged(3, "picture");

                var sqlEntity = new SqLiteDatabase();
                sqlEntity.InsertLibraryItem(item);
                sqlEntity.Dispose();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            } 
        }

        public void AddToShareSong(SoundDataObject song, int count = 0)
        {
            try
            {
                var item = ActivityContext?.LibraryFragment?.MAdapter?.LibraryList?.FirstOrDefault(a =>a.SectionId == "5");
                if (item == null) return;

                item.SongsCount = count != 0 ? count : item.SongsCount + 1;
                item.BackgroundImage = song.Thumbnail;
                ActivityContext?.LibraryFragment?.MAdapter?.NotifyItemChanged(4, "picture");

                var sqlEntity = new SqLiteDatabase();
                sqlEntity.InsertLibraryItem(item);
                sqlEntity.Dispose();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }


        public void AddToPurchases(SoundDataObject song, int count = 0)
        {
            try
            {
                var item = ActivityContext?.LibraryFragment?.MAdapter?.LibraryList?.FirstOrDefault(a =>a.SectionId == "6");
                if (item == null) return;
                item.SongsCount = count != 0 ? count : item.SongsCount + 1;

                item.BackgroundImage = song.Thumbnail;
                ActivityContext?.LibraryFragment?.MAdapter?.NotifyItemChanged(0, "picture");

                var sqlEntity = new SqLiteDatabase();
                sqlEntity.InsertLibraryItem(item);
                sqlEntity.Dispose();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

        }

        public void PlaylistMoreOnClick(MorePlaylistClickEventArgs args)
        {
            try
            {
                OptionDialog = "Playlist";
                MorePlaylistArgs = args;

                var arrayAdapter = new List<string>();
                var dialogList = new MaterialDialog.Builder(ActivityContext).Theme(AppSettings.SetTabDarkTheme ? Theme.Dark : Theme.Light);
               
                if (UserDetails.IsLogin)
                {
                    arrayAdapter.Add(ActivityContext.GetText(Resource.String.Lbl_DeletePlaylist));
                    arrayAdapter.Add(ActivityContext.GetText(Resource.String.Lbl_EditPlaylist));
                }
                 
                arrayAdapter.Add(ActivityContext.GetText(Resource.String.Lbl_Share));
                arrayAdapter.Add(ActivityContext.GetText(Resource.String.Lbl_Copy));

                dialogList.Title(ActivityContext.GetText(Resource.String.Lbl_Playlist));
                dialogList.Items(arrayAdapter);
                dialogList.PositiveText(ActivityContext.GetText(Resource.String.Lbl_Close)).OnPositive(this);
                dialogList.AlwaysCallSingleChoiceCallback();
                dialogList.ItemsCallback(this).Build().Show();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
         
        public void AlbumsOnMoreClick(MoreAlbumsClickEventArgs args)
        {
            try
            {
                OptionDialog = "Albums";
                MoreAlbumsArgs = args;

                var arrayAdapter = new List<string>();
                var dialogList = new MaterialDialog.Builder(ActivityContext).Theme(AppSettings.SetTabDarkTheme ? Theme.Dark : Theme.Light);

                arrayAdapter.Add(ActivityContext.GetText(Resource.String.Lbl_Share));
                arrayAdapter.Add(ActivityContext.GetText(Resource.String.Lbl_Copy));
                 
                dialogList.Title(ActivityContext.GetText(Resource.String.Lbl_Albums));
                dialogList.Items(arrayAdapter);
                dialogList.PositiveText(ActivityContext.GetText(Resource.String.Lbl_Close)).OnPositive(this);
                dialogList.AlwaysCallSingleChoiceCallback();
                dialogList.ItemsCallback(this).Build().Show();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void OnSelection(MaterialDialog p0, View p1, int itemId, ICharSequence itemString)
        {
            try
            {
                string text = itemString.ToString();
                if (text == ActivityContext.GetText(Resource.String.Lbl_DeletePlaylist))
                {
                    OnMenuDeletePlaylistOnClick();
                }
                else if (text == ActivityContext.GetText(Resource.String.Lbl_EditPlaylist))
                {
                    OnMenuEditPlaylistOnClick();
                } 
                else if (text == ActivityContext.GetText(Resource.String.Lbl_Share))
                {
                    switch (OptionDialog)
                    {
                        case "Playlist":
                            SharePlaylist();
                            break;
                        case "Albums":
                            ShareAlbums();
                            break;
                    }
                } 
                else if (text == ActivityContext.GetText(Resource.String.Lbl_Copy))
                {
                    ClipboardManager clipboard = (ClipboardManager)ActivityContext.GetSystemService(Context.ClipboardService);
                    ClipData clip = null;
                    switch (OptionDialog)
                    {
                        case "Playlist":
                            clip = ClipData.NewPlainText("clipboard", MorePlaylistArgs?.PlaylistClass?.Url);
                            break;
                        case "Albums":
                            clip = ClipData.NewPlainText("clipboard", MoreAlbumsArgs?.AlbumsClass?.Url);
                            break;
                    } 
                    clipboard.PrimaryClip = clip;

                    Toast.MakeText(ActivityContext, ActivityContext.GetText(Resource.String.Lbl_Text_copied), ToastLength.Short).Show();
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
                if (TypeDialog == "DeletePlaylist")
                { 
                    if (p1 == DialogAction.Positive)
                    {
                        ActivityContext.RunOnUiThread(() =>
                        {
                            try
                            {
                                var dataPlaylistFragment = ActivityContext?.PlaylistFragment?.PlaylistAdapter;
                                var list = dataPlaylistFragment?.PlaylistList;
                                var dataPlaylist = list?.FirstOrDefault(a => a.Id == MorePlaylistArgs?.PlaylistClass?.Id);
                                if (dataPlaylist != null)
                                {
                                    int index = list.IndexOf(dataPlaylist);
                                    if (index >= 0)
                                    {
                                        list?.Remove(dataPlaylist);
                                        dataPlaylistFragment?.NotifyItemRemoved(index);
                                    }
                                }

                                var dataMyPlaylistFragment = ActivityContext?.PlaylistFragment?.MyPlaylistFragment?.PlaylistAdapter;
                                var list2 = dataMyPlaylistFragment?.PlaylistList;
                                var dataMyPlaylist = list2?.FirstOrDefault(a => a.Id == MorePlaylistArgs?.PlaylistClass?.Id);
                                if (dataMyPlaylist != null)
                                {
                                    int index = list2.IndexOf(dataMyPlaylist);
                                    if (index >= 0)
                                    {
                                        list2?.Remove(dataMyPlaylist);
                                        dataMyPlaylistFragment?.NotifyItemRemoved(index);
                                    }
                                }

                                Toast.MakeText(ActivityContext,ActivityContext.GetText(Resource.String.Lbl_PlaylistSuccessfullyDeleted), ToastLength.Short).Show();

                                //Sent Api >>
                                RequestsAsync.Playlist.DeletePlaylistAsync(MorePlaylistArgs?.PlaylistClass?.Id.ToString()).ConfigureAwait(false); 
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e); 
                            } 
                        });
                    }
                    else if (p1 == DialogAction.Negative)
                    {
                        p0.Dismiss();
                    }
                }
                else
                {
                    if (p1 == DialogAction.Positive)
                    {
                    }
                    else if (p1 == DialogAction.Negative)
                    {
                        p0.Dismiss();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e); 
            } 
        }
         
        public void OnMenuDeletePlaylistOnClick()
        {
            try
            {
                if (!UserDetails.IsLogin)
                {
                    PopupDialogController dialog = new PopupDialogController(ActivityContext, null, "Login");
                    dialog.ShowNormalDialog(ActivityContext.GetText(Resource.String.Lbl_Login), ActivityContext.GetText(Resource.String.Lbl_Message_Sorry_signin), ActivityContext.GetText(Resource.String.Lbl_Yes), ActivityContext.GetText(Resource.String.Lbl_No));
                    return;
                }

                if (Methods.CheckConnectivity())
                {
                    TypeDialog = "DeletePlaylist";
                    
                    var dialog = new MaterialDialog.Builder(ActivityContext).Theme(AppSettings.SetTabDarkTheme ? Theme.Dark : Theme.Light);
                    dialog.Title(ActivityContext.GetText(Resource.String.Lbl_DeletePlaylist));
                    dialog.Content(ActivityContext.GetText(Resource.String.Lbl_AreYouSureDeletePlaylist));
                    dialog.PositiveText(ActivityContext.GetText(Resource.String.Lbl_Yes)).OnPositive(this);
                    dialog.NegativeText(ActivityContext.GetText(Resource.String.Lbl_No)).OnNegative(this);
                    dialog.AlwaysCallSingleChoiceCallback();
                    dialog.ItemsCallback(this).Build().Show();
                }
                else
                {
                    Toast.MakeText(ActivityContext, ActivityContext.GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
          
        public void OnMenuEditPlaylistOnClick()
        {
            try
            {
                if (!UserDetails.IsLogin)
                {
                    PopupDialogController dialog = new PopupDialogController(ActivityContext, null, "Login");
                    dialog.ShowNormalDialog(ActivityContext.GetText(Resource.String.Lbl_Login), ActivityContext.GetText(Resource.String.Lbl_Message_Sorry_signin), ActivityContext.GetText(Resource.String.Lbl_Yes), ActivityContext.GetText(Resource.String.Lbl_No));
                    return;
                }


                Intent intent = new Intent(ActivityContext,typeof(EditPlaylistActivity));
                intent.PutExtra("ItemData", JsonConvert.SerializeObject(MorePlaylistArgs.PlaylistClass));
                ActivityContext.StartActivity(intent);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private async void SharePlaylist()
        {
            try
            {
                //Share Plugin same as Song
                if (!CrossShare.IsSupported)
                {
                    return;
                }

                await CrossShare.Current.Share(new ShareMessage
                {
                    Title = MorePlaylistArgs?.PlaylistClass?.Name,
                    Text = "",
                    Url = MorePlaylistArgs?.PlaylistClass?.Url
                });
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private async void ShareAlbums()
        {
            try
            {
                //Share Plugin same as Song
                if (!CrossShare.IsSupported)
                {
                    return;
                }

                await CrossShare.Current.Share(new ShareMessage
                {
                    Title = MoreAlbumsArgs?.AlbumsClass?.Title,
                    Text = "",
                    Url = MoreAlbumsArgs?.AlbumsClass?.Url
                });
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        } 
    }
     
    public class MorePlaylistClickEventArgs
    {
        public View View { get; set; }
        public PlaylistDataObject PlaylistClass { get; set; }
    }

    public class MoreAlbumsClickEventArgs
    {
        public View View { get; set; }
        public DataAlbumsObject AlbumsClass { get; set; }
    } 
}