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
using DeepSound.Helpers.Fonts;
using DeepSound.Helpers.MediaPlayerController;
using DeepSound.Helpers.Utils;
using DeepSound.Library.Anjo;
using DeepSoundClient.Classes.Global;
using Java.Util;
using IList = System.Collections.IList;

namespace DeepSound.Activities.Songs.Adapters
{
    public class RowSoundAdapter : RecyclerView.Adapter, ListPreloader.IPreloadModelProvider
    {
        private readonly Activity ActivityContext;
        public event EventHandler<RowSoundAdapterClickEventArgs> OnItemClick;
        public event EventHandler<RowSoundAdapterClickEventArgs> OnItemLongClick;

        public ObservableCollection<SoundDataObject> SoundsList = new ObservableCollection<SoundDataObject>();
        private readonly SocialIoClickListeners ClickListeners;
        private readonly string NamePage;
        public RowSoundAdapter(Activity context,string namePage)
        {
            try
            {
                ActivityContext = context;
                NamePage = namePage;
                HasStableIds = true;
                ClickListeners = new SocialIoClickListeners(context);
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
                    return SoundsList.Count;
                return 0;
            }
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            try
            {
                //Setup your layout here >> Style_SongView
                var itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Style_SongView, parent, false);
                var vh = new RowSoundAdapterViewHolder(itemView, OnClick, OnLongClick);
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
                if (!(viewHolder is RowSoundAdapterViewHolder holder)) return;

                var item = SoundsList[position];

                if (item == null) return;

                GlideImageLoader.LoadImage(ActivityContext, item.Thumbnail, holder.Image, ImageStyle.CenterCrop, ImagePlaceholders.Drawable);

                holder.TxtSongName.Text = Methods.FunString.SubStringCutOf(Methods.FunString.DecodeString(item.Title),25);
                holder.TxtGenresName.Text = item.CategoryName + " " + ActivityContext.GetText(Resource.String.Lbl_Music) + " - " + Methods.FunString.FormatPriceValue(Convert.ToInt32(item.CountViews.Replace("K", "").Replace("M", "")));

                holder.CountLike.Text = item.CountLikes.ToString();
                holder.CountStars.Text = item.CountFavorite.ToString();

                holder.CountViews.Text = item.CountViews;
                holder.CountShare.Text = item.CountShares.ToString();
                holder.CountComment.Text = item.CountComment.ToString();
                 
                holder.TxtSongDuration.Text = item.Duration;

                if (item.IsPlay)
                {
                    holder.Image.Visibility = ViewStates.Gone;
                    holder.CardViewImage.Visibility = ViewStates.Gone;
                    holder.Equalizer.Visibility = ViewStates.Visible;
                    holder.Equalizer.AnimateBars();
                }
                else
                {
                    holder.Image.Visibility = ViewStates.Visible;
                    holder.CardViewImage.Visibility = ViewStates.Visible;
                    holder.Equalizer.Visibility = ViewStates.Gone;
                    holder.Equalizer.StopBars();
                }

                if (!holder.MoreButton.HasOnClickListeners)
                    holder.MoreButton.Click += (sender, e) => ClickListeners.OnMoreClick(new MoreSongClickEventArgs { View = holder.MainView, SongsClass = item }, NamePage);
                 
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
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

        private void OnClick(RowSoundAdapterClickEventArgs args)
        {
            OnItemClick?.Invoke(this, args);
        }

        private void OnLongClick(RowSoundAdapterClickEventArgs args)
        {
            OnItemLongClick?.Invoke(this, args);
        }

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

    public class RowSoundAdapterViewHolder : RecyclerView.ViewHolder
    {
        public View MainView { get; private set; }
        public CardView CardViewImage { get; private set; }
        public ImageView Image { get; private set; }
        public EqualizerView Equalizer { get; private set; }
        public TextView TxtSongName { get; private set; }
        public TextView TxtGenresName { get; private set; }
        public TextView IconLike { get; private set; }
        public TextView CountLike { get; private set; }
        public TextView IconStars { get; private set; }
        public TextView CountStars { get; private set; }
        public TextView IconViews { get; private set; }
        public TextView CountViews { get; private set; }
        public TextView IconShare { get; private set; }
        public TextView CountShare { get; private set; }
        public TextView IconComment { get; private set; }
        public TextView CountComment { get; private set; }
        public TextView TxtSongDuration { get; private set; }
        public ImageButton MoreButton { get; private set; }

        public RowSoundAdapterViewHolder(View itemView, Action<RowSoundAdapterClickEventArgs> clickListener, Action<RowSoundAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            try
            {
                MainView = itemView;
                CardViewImage = MainView.FindViewById<CardView>(Resource.Id.cardview2);
                Image = MainView.FindViewById<ImageView>(Resource.Id.imageView_songlist);
                Equalizer = MainView.FindViewById<EqualizerView>(Resource.Id.equalizer_view);
                TxtSongName = MainView.FindViewById<TextView>(Resource.Id.textView_songname);
                TxtGenresName = MainView.FindViewById<TextView>(Resource.Id.textView_catname);

                IconLike = MainView.FindViewById<TextView>(Resource.Id.iconLike);
                CountLike = MainView.FindViewById<TextView>(Resource.Id.textView_songLike);
                IconStars = MainView.FindViewById<TextView>(Resource.Id.iconStars);
                CountStars = MainView.FindViewById<TextView>(Resource.Id.textView_totalrate_songlist);
                IconViews = MainView.FindViewById<TextView>(Resource.Id.iconViews);
                CountViews = MainView.FindViewById<TextView>(Resource.Id.textView_views);
                IconShare = MainView.FindViewById<TextView>(Resource.Id.iconShare);
                CountShare = MainView.FindViewById<TextView>(Resource.Id.textView_share);
                IconComment = MainView.FindViewById<TextView>(Resource.Id.iconComment);
                CountComment = MainView.FindViewById<TextView>(Resource.Id.textView_comment);

                TxtSongDuration = MainView.FindViewById<TextView>(Resource.Id.textView_songduration);

                MoreButton = MainView.FindViewById<ImageButton>(Resource.Id.more);

                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, IconLike, IonIconsFonts.Heart);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, IconStars, IonIconsFonts.AndroidStar);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, IconViews, IonIconsFonts.Play);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, IconShare, IonIconsFonts.AndroidShareAlt);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, IconComment, FontAwesomeIcon.CommentDots);

                //Event
                itemView.Click += (sender, e) => clickListener(new RowSoundAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
                itemView.LongClick += (sender, e) => longClickListener(new RowSoundAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
    }

    public class RowSoundAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}