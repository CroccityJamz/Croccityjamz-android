using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Android.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Bumptech.Glide;
using Bumptech.Glide.Request;
using DeepSound.Activities.Library.Listeners;
using DeepSound.Helpers.CacheLoaders;
using DeepSound.Helpers.Fonts;
using DeepSound.Helpers.Utils;
using DeepSoundClient.Classes.Playlist;
using Java.Util;
using IList = System.Collections.IList;

namespace DeepSound.Activities.Playlist.Adapters
{
    public class HPlaylistAdapter : RecyclerView.Adapter, ListPreloader.IPreloadModelProvider
    {
        public event EventHandler<PlaylistAdapterClickEventArgs> OnItemClick;
        public event EventHandler<PlaylistAdapterClickEventArgs> OnItemLongClick;

        private readonly Activity ActivityContext;
        public ObservableCollection<PlaylistDataObject> PlaylistList = new ObservableCollection<PlaylistDataObject>();
        private readonly bool ShowMore;
        private readonly LibrarySynchronizer LibrarySynchronizer;

        public HPlaylistAdapter(Activity context , bool showMore = false)
        {
            try
            {
                ActivityContext = context;
                HasStableIds = true;
                ShowMore = showMore;
                LibrarySynchronizer = new LibrarySynchronizer(context);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            try
            {
                //Setup your layout here >> Style_Playlist
                View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Style_PlaylistView, parent, false);
                var vh = new PlaylistAdapterViewHolder(itemView, Click, LongClick);
                return vh;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            try
            {
                if (viewHolder is PlaylistAdapterViewHolder holder)
                {
                    var item = PlaylistList[position];
                    if (item != null)
                    {
                        GlideImageLoader.LoadImage(ActivityContext, item.ThumbnailReady, holder.Image, ImageStyle.CenterCrop, ImagePlaceholders.Drawable);

                        holder.TxtName.Text = Methods.FunString.DecodeString(item.Name);

                        holder.TxtUserName.Text = item.Publisher != null ? Methods.FunString.DecodeString(DeepSoundTools.GetNameFinal(item.Publisher.Value.PublisherClass)) : ActivityContext.GetText(Resource.String.Lbl_Unknown);
                         
                        holder.TxtCountSongs.Text = item.Songs +  " " + ActivityContext.GetText(Resource.String.Lbl_Songs);

                        holder.MoreButton.Visibility = ShowMore ? ViewStates.Visible : ViewStates.Gone;

                        if (!holder.MoreButton.HasOnClickListeners)
                            holder.MoreButton.Click += (sender, e) => LibrarySynchronizer.PlaylistMoreOnClick(new MorePlaylistClickEventArgs { View = holder.MainView, PlaylistClass = item });
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public override int ItemCount
        {
            get
            {
                if (PlaylistList != null)
                {
                    return PlaylistList.Count;
                }

                return 0;
            }
        }

        public PlaylistDataObject GetItem(int position)
        {
            return PlaylistList[position];
        }

        public override long GetItemId(int position)
        {
            try
            {
                return PlaylistList[position].Id;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return 0;
            }
        }

        public override int GetItemViewType(int position)
        {
            try
            {
                return position;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return 0;
            }
        }

        void Click(PlaylistAdapterClickEventArgs args) => OnItemClick?.Invoke(this, args);
        void LongClick(PlaylistAdapterClickEventArgs args) => OnItemLongClick?.Invoke(this, args);

        public IList GetPreloadItems(int p0)
        {
            try
            {
                var d = new List<string>();
                var item = PlaylistList[p0];

                if (item == null)
                    return Collections.SingletonList(p0);

                if (item.ThumbnailReady != "")
                {
                    d.Add(item.ThumbnailReady);
                    return d;
                }

                return d;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Collections.SingletonList(p0);
            }
        }

        public RequestBuilder GetPreloadRequestBuilder(Java.Lang.Object p0)
        {
            return Glide.With(ActivityContext).Load(p0.ToString())
                .Apply(new RequestOptions().CircleCrop());
        }

    }

    public class PlaylistAdapterViewHolder : RecyclerView.ViewHolder
    {
        #region Variables Basic

        public View MainView { get; set; }
        public ImageView Image { get; private set; }
        public TextView TxtName { get; private set; }
        public TextView TxtUserName { get; private set; }
        public TextView TxtCountSongs { get; private set; }
        public TextView TxtIconCount { get; private set; }
        public ImageButton MoreButton { get; private set; }
      
        #endregion

        public PlaylistAdapterViewHolder(View itemView, Action<PlaylistAdapterClickEventArgs> clickListener, Action<PlaylistAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            try
            {
                MainView = itemView;

                //Get values
                Image = (ImageView)MainView.FindViewById(Resource.Id.image);
                TxtName = MainView.FindViewById<TextView>(Resource.Id.name);
                TxtUserName = MainView.FindViewById<TextView>(Resource.Id.UserName);
                TxtCountSongs = MainView.FindViewById<TextView>(Resource.Id.count);
                TxtIconCount = MainView.FindViewById<TextView>(Resource.Id.IconCount);
                MoreButton = MainView.FindViewById<ImageButton>(Resource.Id.more);

                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, TxtIconCount, IonIconsFonts.IosMusicalNotes);

                //Event
                itemView.Click += (sender, e) => clickListener(new PlaylistAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
                itemView.LongClick += (sender, e) => longClickListener(new PlaylistAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }

    public class PlaylistAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}