using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Gms.Vision.Faces;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace friendcognition.Droid
{
    public class FaceGraphic : Graphic
    {
        private static readonly float ID_TEXT_SIZE = 40.0f;
        private static readonly float BOX_STROKE_WIDTH = 5.0f;

        private Paint mFacePositionPaint;
        private Paint mIdPaint;
        private Paint mBoxPaint;

        private volatile Face mFace;
        private int mFaceId;

        private bool touching = false;
        private float x, y;
        private float left, right, top, bottom;

        public FaceGraphic(GraphicOverlay overlay) : base(overlay)
        {
            var selectedColor = Color.Blue;

            mFacePositionPaint = new Paint()
            {
                Color = selectedColor
            };
            mIdPaint = new Paint()
            {
                Color = selectedColor,
                TextSize = ID_TEXT_SIZE
            };
            mBoxPaint = new Paint()
            {
                Color = selectedColor
            };
            mBoxPaint.SetStyle(Paint.Style.Stroke);
            mBoxPaint.StrokeWidth = BOX_STROKE_WIDTH;
        }
        public void SetId(int id)
        {
            mFaceId = id;
        }

        public void checkIfTouching()
        {
            touching = DataController.Instance().getTouching();
            if (touching == true)
            {
                x = DataController.Instance().getX();
                y = DataController.Instance().getY();

                if (x > left && x < right && y > top && y < bottom)
                {
                    DataController.Instance().id = mFaceId;
                    Intent i = new Intent(Application.Context, typeof(ProfileActivity));
                    Application.Context.StartActivity(i);
                }

                touching = false;
            }
        }

        /**
         * Updates the face instance from the detection of the most recent frame.  Invalidates the
         * relevant portions of the overlay to trigger a redraw.
         */
        public void UpdateFace(Face face)
        {
            mFace = face;
            PostInvalidate();
            checkIfTouching();
        }

        public override void Draw(Canvas canvas)
        {
            Face face = mFace;
            if (face == null)
            {
                return;
            }

            float x = TranslateX(face.Position.X + face.Width / 2);
            float y = TranslateY(face.Position.Y + face.Height / 2);

            // Draws a bounding box around the face.
            float xOffset = ScaleX(face.Width / 2.0f);
            float yOffset = ScaleY(face.Height / 2.0f);
            left = x - xOffset;
            top = y - yOffset;
            right = x + xOffset;
            bottom = y + yOffset;
            canvas.DrawRect(left, top, right, bottom, mBoxPaint);
        }
    }
}