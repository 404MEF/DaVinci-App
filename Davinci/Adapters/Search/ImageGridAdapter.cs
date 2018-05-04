using System.Collections.Generic;
using Android.Content;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using Android.Util;

using Davinci.Components;
using Davinci.Api.Models;

namespace Davinci.Adapters.Search
{
    class ImageGridAdapter : BaseAdapter<Bitmap>
    {
        private Context mContext;
        private List<Bitmap> bitmaps;

        public ImageGridAdapter(Context context,List<PostModel> posts)
        {

            this.mContext = context;

            bitmaps = new List<Bitmap>();
            foreach (var post in posts)
            {
                var imageData = Base64.Decode(post.image, Base64Flags.Default);
                var bitmap = BitmapFactory.DecodeByteArray(imageData, 0, imageData.Length);
                bitmaps.Add(bitmap);
            }

        }
        public override Bitmap this[int position]
        {
            get
            {
                return bitmaps[position];
            }
        }

        public override int Count
        {
            get
            {
                return bitmaps.Count;
            }
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            SquareImageView imageView;

            if (convertView == null)
            {  // if it's not recycled, initialize some attributes
                imageView = new SquareImageView(mContext);
                imageView.SetScaleType(ImageView.ScaleType.CenterCrop);
            }
            else
            {
                imageView = (SquareImageView)convertView;
            }

            imageView.SetImageBitmap(bitmaps[position]);
            return imageView;
        }
    }
}