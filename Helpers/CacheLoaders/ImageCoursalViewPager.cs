using System;
using System.Collections.ObjectModel;
using Android.App;
using Android.Support.V4.View;
using Android.Views;
using Android.Widget;
using DeepSound.Activities.Tabbes;
using DeepSound.Helpers.MediaPlayerController;
using DeepSound.Helpers.Utils;
using DeepSoundClient.Classes.Global;
using Exception = System.Exception;
using Object = Java.Lang.Object;

namespace DeepSound.Helpers.CacheLoaders
{
    public class ImageCoursalViewPager : PagerAdapter
    {

        private readonly Activity ActivityContext;
        private readonly ObservableCollection<SoundDataObject> PlaylistList;
        private readonly LayoutInflater Inflater;

        public ImageCoursalViewPager(Activity context, ObservableCollection<SoundDataObject> playlistList)
        {
            ActivityContext = context;
            PlaylistList = playlistList;
            Inflater = LayoutInflater.From(context);
        }
         
        public override Object InstantiateItem(ViewGroup view, int position)
        {
            try
            {
                View layout = Inflater.Inflate(Resource.Layout.ImageCoursalLayout, view, false);
                var mainFeaturedVideo = layout.FindViewById<ImageView>(Resource.Id.image);
                var title = layout.FindViewById<TextView>(Resource.Id.titleText);
                var seconderText = layout.FindViewById<TextView>(Resource.Id.seconderyText);
                //var cardView = layout.FindViewById<CardView>(Resource.Id.cardview2);

                if (PlaylistList[position] != null)
                { 
                    title.Text = Methods.FunString.DecodeString(PlaylistList[position].Title);
                    seconderText.Text = PlaylistList[position].CategoryName + " " + ActivityContext.GetText(Resource.String.Lbl_Music);

                    GlideImageLoader.LoadImage(ActivityContext, PlaylistList[position].Thumbnail, mainFeaturedVideo, ImageStyle.CenterCrop, ImagePlaceholders.Drawable);
                }

                if (!layout.HasOnClickListeners)
                {
                    layout.Click += (sender, args) =>
                    {
                        try
                        {
                            Constant.PlayPos = position;
                            ((HomeActivity)ActivityContext)?.SoundController?.StartPlaySound(PlaylistList[position], PlaylistList);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e); 
                        } 
                    };
                }

                view.AddView(layout);

                return layout;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            } 
        }

        

        public override bool IsViewFromObject(View view, Object @object)
        {
            return view.Equals(@object);
        }

        public override int Count
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

        public override void DestroyItem(ViewGroup container, int position, Object @object)
        {
            try
            {
                View view = (View)@object;
                container.RemoveView(view);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

            } 
        } 
    }
    
}