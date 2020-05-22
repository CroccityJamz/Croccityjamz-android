using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Android.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Bumptech.Glide;
using Bumptech.Glide.Request;
using DeepSound.Helpers.CacheLoaders;
using DeepSound.Helpers.MediaPlayerController;
using DeepSound.Helpers.Utils;
using DeepSoundClient.Classes.User;
using Java.Util;
using IList = System.Collections.IList;

namespace DeepSound.Activities.Tabbes.Adapters
{
    public class ActivityAdapter : RecyclerView.Adapter, ListPreloader.IPreloadModelProvider
    {
        public event EventHandler<ActivityAdapterClickEventArgs> OnItemClick;
        public event EventHandler<ActivityAdapterClickEventArgs> OnItemLongClick;

        private readonly Activity ActivityContext;
        private readonly HomeActivity GlobalContext;
        public ObservableCollection<ActivityDataObject> ActivityList = new ObservableCollection<ActivityDataObject>();
        private readonly SocialIoClickListeners ClickListeners;

        public ActivityAdapter(Activity context)
        {
            try
            {
                ActivityContext = context;
                HasStableIds = true;
                ClickListeners = new SocialIoClickListeners(context); 
                GlobalContext = HomeActivity.GetInstance(); 
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
                //Setup your layout here >> Style_RowSongsPlaylistView
                View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Style_ActivityView, parent, false);
                var vh = new ActivityAdapterViewHolder(itemView, Click, LongClick);
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
                if (viewHolder is ActivityAdapterViewHolder holder)
                {
                    var item = ActivityList[position];
                    if (item != null)
                    {
                        GlideImageLoader.LoadImage(ActivityContext, item.UserData.Avatar, holder.ImageUser, ImageStyle.CircleCrop, ImagePlaceholders.Drawable);
                        GlideImageLoader.LoadImage(ActivityContext, item.SThumbnail, holder.ImageSong, ImageStyle.CenterCrop, ImagePlaceholders.Drawable);

                        holder.TxtName.Text = DeepSoundTools.GetNameFinal(item.UserData);
                        holder.TxtTitle.Text = Methods.FunString.SubStringCutOf(Methods.FunString.DecodeString(item.ActivityText) + " " + item.ActivityTimeFormatted, 35);

                        holder.TxtTitleSong.Text = Methods.FunString.SubStringCutOf(Methods.FunString.DecodeString(item.TrackData.Title), 25);

                        if (!holder.TxtTitle.HasOnClickListeners)
                            holder.TxtTitle.Click += (sender, e) => GlobalContext?.OpenProfile(item.UserData.Id, item.UserData);

                        if (!holder.MoreButton.HasOnClickListeners)
                            holder.MoreButton.Click += (sender, e) => ClickListeners.OnMoreClick(new MoreSongClickEventArgs { View = holder.MainView, SongsClass = item.TrackData });
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
                if (ActivityList != null)
                {
                    return ActivityList.Count;
                }

                return 0;
            }
        }

        public ActivityDataObject GetItem(int position)
        {
            return ActivityList[position];
        }

        public override long GetItemId(int position)
        {
            try
            {
                return ActivityList[position].TrackData.Id;
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

        void Click(ActivityAdapterClickEventArgs args) => OnItemClick?.Invoke(this, args);
        void LongClick(ActivityAdapterClickEventArgs args) => OnItemLongClick?.Invoke(this, args);

        public IList GetPreloadItems(int p0)
        {
            try
            {
                var d = new List<string>();
                var item = ActivityList[p0];

                if (item == null)
                    return Collections.SingletonList(p0);

                if (item.SThumbnail != "")
                {
                    d.Add(item.SThumbnail);
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

    public class ActivityAdapterViewHolder : RecyclerView.ViewHolder
    {
        #region Variables Basic

        public View MainView { get; private set; }
        public ImageView ImageUser { get; private set; }
        public ImageView ImageSong { get; private set; }
        public TextView TxtName { get; private set; }
        public TextView TxtTitle { get; private set; }
        public TextView TxtTitleSong { get; private set; }
        public TextView TxtTime { get; private set; }
        public ImageButton MoreButton { get; private set; }


        #endregion

        public ActivityAdapterViewHolder(View itemView, Action<ActivityAdapterClickEventArgs> clickListener, Action<ActivityAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            try
            {
                MainView = itemView;

                ImageUser = MainView.FindViewById<ImageView>(Resource.Id.imageUser);
                TxtName = MainView.FindViewById<TextView>(Resource.Id.name);
                TxtTitle = MainView.FindViewById<TextView>(Resource.Id.title);  
                MoreButton = MainView.FindViewById<ImageButton>(Resource.Id.more);

                ImageSong = MainView.FindViewById<ImageView>(Resource.Id.imageSong);
                TxtTitleSong = MainView.FindViewById<TextView>(Resource.Id.titleSong); 
                TxtTime = MainView.FindViewById<TextView>(Resource.Id.time);
                 
                //Event
                itemView.Click += (sender, e) => clickListener(new ActivityAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
                itemView.LongClick += (sender, e) => longClickListener(new ActivityAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }

    public class ActivityAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}