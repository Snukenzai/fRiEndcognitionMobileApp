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


        /**
         * Updates the face instance from the detection of the most recent frame.  Invalidates the
         * relevant portions of the overlay to trigger a redraw.
         */
        public void UpdateFace(Face face)
        {
            mFace = face;
            PostInvalidate();
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
            float left = x - xOffset;
            float top = y - yOffset;
            float right = x + xOffset;
            float bottom = y + yOffset;
            canvas.DrawRect(left, top, right, bottom, mBoxPaint);
        }
    }
}