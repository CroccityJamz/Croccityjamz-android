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
using DeepSound.Helpers.Utils;
using DeepSoundClient.Classes.Albums;
using Java.Util;
using IList = System.Collections.IList;

namespace DeepSound.Activities.Albums.Adapters
{
    public class HAlbumsAdapter : RecyclerView.Adapter, ListPreloader.IPreloadModelProvider
    {
        private readonly Activity ActivityContext;
        public event EventHandler<HAlbumsAdapterClickEventArgs> ItemClick;
        public event EventHandler<HAlbumsAdapterClickEventArgs> ItemLongClick;

        public ObservableCollection<DataAlbumsObject> AlbumsList = new ObservableCollection<DataAlbumsObject>();

        public HAlbumsAdapter(Activity context)
        {
            try
            {
                ActivityContext = context;
                HasStableIds = true;
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
                if (AlbumsList != null)
                    return AlbumsList.Count;
                return 0;
            }
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            try
            {
                //Setup your layout here >> Style_HorizontalSoundView
                var itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Style_HAlbumsView, parent, false);
                var vh = new HAlbumsAdapterViewHolder(itemView, OnClick, OnLongClick);
                return vh;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return null;
            }
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            try
            {
                if (!(viewHolder is HAlbumsAdapterViewHolder holder)) return;

                var item = AlbumsList[position];

                if (item == null) return;

                GlideImageLoader.LoadImage(ActivityContext, item.Thumbnail, holder.Image, ImageStyle.CenterCrop,ImagePlaceholders.Drawable);

                holder.TxtTitle.Text = Methods.FunString.DecodeString(item.Title);

                holder.TxtCountSound.Text = item.CountSongs.ToString();

            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public DataAlbumsObject GetItem(int position)
        {
            return AlbumsList[position];
        }

        public override long GetItemId(int position)
        {
            try
            {
                return AlbumsList[position].Id;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return 0;
            }
        }

        public override int GetItemViewType(int position)
        {
            try
            {
                return position;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return 0;
            }
        }

        private void OnClick(HAlbumsAdapterClickEventArgs args)
        {
            ItemClick?.Invoke(this, args);
        }

        private void OnLongClick(HAlbumsAdapterClickEventArgs args)
        {
            ItemLongClick?.Invoke(this, args);
        }

        public IList GetPreloadItems(int p0)
        {
            try
            {
                var d = new List<string>();
                var item = AlbumsList[p0];

                if (item == null)
                    return Collections.SingletonList(p0);

                if (item.Thumbnail != "")
                {
                    d.Add(item.Thumbnail);
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

    public class HAlbumsAdapterViewHolder : RecyclerView.ViewHolder
    {
        #region Variables Basic

        public View MainView { get;  set; }
        public ImageView Image { get;  set; }
        public TextView TxtTitle { get;  set; }
        public TextView TxtSecondaryText { get;  set; }
        public TextView TxtCountSound { get;  set; }

        #endregion

        public HAlbumsAdapterViewHolder(View itemView, Action<HAlbumsAdapterClickEventArgs> clickListener, Action<HAlbumsAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            try
            {
                MainView = itemView;

                //Get values
                Image = (ImageView)MainView.FindViewById(Resource.Id.imageSound);
                TxtTitle = MainView.FindViewById<TextView>(Resource.Id.titleTextView);
                TxtSecondaryText = MainView.FindViewById<TextView>(Resource.Id.seconderyText);
                TxtCountSound = MainView.FindViewById<TextView>(Resource.Id.image_countSound);

                TxtSecondaryText.Visibility = ViewStates.Gone;
                
                //Event
                itemView.Click += (sender, e) => clickListener(new HAlbumsAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
                itemView.LongClick += (sender, e) => longClickListener(new HAlbumsAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        } 
    }

    public class HAlbumsAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}