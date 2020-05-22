using System;
using Android.Animation;
using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Util;
using Android.Widget;

namespace DeepSound.Library.Anjo
{
    public class CircleButton : ImageView
    {
        private static readonly int PressedColorLightUp = 255 / 25, PressedRingAlpha = 75, DefaultPressedRingWidthDip = 4, AnimationTimeId = Android.Resource.Integer.ConfigShortAnimTime;
         
        private int CenterY, CenterX, OuterRadius, PressedRingRadius;

        private Paint CirclePaint, FocusPaint;

        private float AnimationProgress;

        private int PressedRingWidth;
        private Color DefaultColor = Color.Black, PressedColor;
        private ObjectAnimator PressedAnimator;

        protected CircleButton(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public CircleButton(Context context) : base(context)
        {
            Init(context, null);
        }

        public CircleButton(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            Init(context, attrs);
        }

        public CircleButton(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            Init(context, attrs);
        }

        public CircleButton(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
        {
            Init(context, attrs);
        }

        private void Init(Context context, IAttributeSet attrs)
        {
            try
            {
                Focusable = true;
                SetScaleType(ScaleType.CenterInside);
                Clickable =true;

                CirclePaint = new Paint(PaintFlags.AntiAlias); 
                CirclePaint.SetStyle(Paint.Style.Fill); 

                FocusPaint = new Paint(PaintFlags.AntiAlias); 
                FocusPaint.SetStyle(Paint.Style.Stroke);
         
                PressedRingWidth = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, DefaultPressedRingWidthDip, Resources.DisplayMetrics);

                var color = Color.Black;
                if (attrs != null)
                {
                    var a = context.ObtainStyledAttributes(attrs, Resource.Styleable.CircleButton);
                    color = a.GetColor(Resource.Styleable.CircleButton_cb_color, color);
                    PressedRingWidth = (int)a.GetDimension(Resource.Styleable.CircleButton_cb_pressedRingWidth, PressedRingWidth);
                    a.Recycle();
                }

                SetColor(color);

                FocusPaint.StrokeWidth=PressedRingWidth;
                var pressedAnimationTime = Resources.GetInteger(AnimationTimeId);
                PressedAnimator = ObjectAnimator.OfFloat(this, "animationProgress", 0f, 0f);
                PressedAnimator.SetDuration(pressedAnimationTime);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        protected override void DispatchSetPressed(bool pressed)
        {
            base.DispatchSetPressed(pressed);

            try
            {
                if (CirclePaint != null)
                    CirclePaint.Color = pressed ? PressedColor : DefaultColor;

                if (pressed)
                    ShowPressedRing();
                else
                    HidePressedRing();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        protected override void OnDraw(Canvas canvas)
        {
            try
            {
                canvas.DrawCircle(CenterX, CenterY, PressedRingRadius + AnimationProgress, FocusPaint);
                canvas.DrawCircle(CenterX, CenterY, OuterRadius - PressedRingWidth, CirclePaint);
                base.OnDraw(canvas);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        protected override void OnSizeChanged(int w, int h, int oldW, int oldH)
        {
            try
            {
                base.OnSizeChanged(w, h, oldW, oldH);
                CenterX = w / 2;
                CenterY = h / 2;
                OuterRadius = Math.Min(w, h) / 2;
                PressedRingRadius = OuterRadius - PressedRingWidth - PressedRingWidth / 2;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public float GetAnimationProgress()
        {
            return AnimationProgress;
        }
         
        /// <summary>
        /// 
        /// </summary>
        /// <param name="animationProgress"></param>
        public void SetAnimationProgress(float animationProgress)
        {
            AnimationProgress = animationProgress;
            Invalidate();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="color"></param>
        public void SetColor(Color color)
        {
            try
            {
                DefaultColor = color;
                PressedColor = GetHighlightColor(color, PressedColorLightUp);

                CirclePaint.Color=DefaultColor;
                FocusPaint.Color=DefaultColor;
                FocusPaint.Alpha=PressedRingAlpha;

                Invalidate();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void HidePressedRing()
        {
            try
            {
                PressedAnimator.SetFloatValues(PressedRingWidth, 0f);
                PressedAnimator.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void ShowPressedRing()
        {
            try
            {
                PressedAnimator.SetFloatValues(AnimationProgress, PressedRingWidth);
                PressedAnimator.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private Color GetHighlightColor(Color color, int amount)
        {
            return Color.Argb(Math.Min(255, Color.GetAlphaComponent(color)), Math.Min(255, Color.GetRedComponent(color) + amount),Math.Min(255, Color.GetGreenComponent(color) + amount), Math.Min(255, Color.GetBlueComponent(color) + amount));
        }

    }
}