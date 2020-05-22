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
using DeepSoundClient.Classes.Global;
using Java.Util;
using IList = System.Collections.IList;

namespace DeepSound.Activities.Songs.Adapters
{
    public class HSoundAdapter : RecyclerView.Adapter, ListPreloader.IPreloadModelProvider
    {
        public event EventHandler<HSoundAdapterClickEventArgs> OnItemClick;
        public event EventHandler<HSoundAdapterClickEventArgs> OnItemLongClick;

        private readonly Activity ActivityContext;
        public ObservableCollection<SoundDataObject> SoundsList = new ObservableCollection<SoundDataObject>();

        public HSoundAdapter(Activity context)
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

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            try
            {
                //Setup your layout here >> Style_HorizontalSound
                View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Style_HorizontalSoundView, parent, false);
                var vh = new HSoundAdapterViewHolder(itemView, Click, LongClick);
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
                if (viewHolder is HSoundAdapterViewHolder holder)
                {
                    var item = SoundsList[position];
                    if (item != null)
                    {
                        GlideImageLoader.LoadImage(ActivityContext, item.Thumbnail, holder.Image, ImageStyle.CenterCrop, ImagePlaceholders.Drawable);

                        holder.TxtTitle.Text = Methods.FunString.DecodeString(item.Title);

                        holder.TxtSeconderyText.Text = item.CategoryName + " " + ActivityContext.GetText(Resource.String.Lbl_Music) + " - " + Methods.FunString.FormatPriceValue(Convert.ToInt32(item.CountViews.Replace("K","").Replace("M","")));
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
                if (SoundsList != null)
                {
                    return SoundsList.Count;
                }

                return 0;
            }
        }

        public SoundDataObject GetItem(int position)
        {
            return SoundsList[position];
        }

        public override long GetItemId(int position)
        {
            try
            {
                return SoundsList[position].Id;
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

        void Click(HSoundAdapterClickEventArgs args) => OnItemClick?.Invoke(this, args);
        void LongClick(HSoundAdapterClickEventArgs args) => OnItemLongClick?.Invoke(this, args);

        public IList GetPreloadItems(int p0)
        {
            try
            {
                var d = new List<string>();
                var item = SoundsList[p0];

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

    public class HSoundAdapterViewHolder : RecyclerView.ViewHolder
    {
        #region Variables Basic

        public View MainView { get; set; }
        public ImageView Image { get; private set; }
        public TextView TxtTitle { get; private set; }
        public TextView TxtSeconderyText { get; private set; }

        #endregion

        public HSoundAdapterViewHolder(View itemView, Action<HSoundAdapterClickEventArgs> clickListener, Action<HSoundAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            try
            {
                MainView = itemView;

                //Get values
                Image = (ImageView)MainView.FindViewById(Resource.Id.imageSound);
                TxtTitle = MainView.FindViewById<TextView>(Resource.Id.titleTextView);
                TxtSeconderyText = MainView.FindViewById<TextView>(Resource.Id.seconderyText);

                //Event
                itemView.Click += (sender, e) => clickListener(new HSoundAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
                itemView.LongClick += (sender, e) => longClickListener(new HSoundAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }

    public class HSoundAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; } 
    }
}