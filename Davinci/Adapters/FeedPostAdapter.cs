using System.Collections.Generic;

using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Graphics;

using Davinci.Api.Models;
using Davinci.Components;

namespace Davinci.Adapters
{
    public class FeedPostAdapter : RecyclerView.Adapter
    {
        public List<PostModel> items { get; private set; }

        public FeedPostAdapter(List<PostModel> list)
        {
            this.items = list;
        }

        public override int ItemCount
        {
            get
            {
                return items.Count;
            }
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var item = items[position];

            var viewHolder = holder as FeedItemAdapterViewHolder;

            viewHolder.Owner.Text = string.Format("Uploaded by {0} at {1}", item.owner,item.createdAt);
            viewHolder.Category.Text = item.category;

            viewHolder.Ratio.CreateView();
            viewHolder.Ratio.SetRatio(item.LikeRatio);

            var imageData = Base64.Decode(item.image.data, Base64Flags.Default);
            var imageSrc = BitmapFactory.DecodeByteArray(imageData, 0, imageData.Length);

            viewHolder.Image.SetImageBitmap(imageSrc);
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var view = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.FeedPost,parent,false);
            return new FeedItemAdapterViewHolder(view);
        }
    }

    public class FeedItemAdapterViewHolder : RecyclerView.ViewHolder
    {
        public ImageView Image { get; private set; }
        public TextView Owner { get; private set; }
        public TextView Category { get; private set; }
        public LikeRatioBar Ratio { get; private set; }
        public FeedItemAdapterViewHolder(View itemView) : base(itemView)
        {
            Image = itemView.FindViewById<ImageView>(Resource.Id.imageView);
            Owner = itemView.FindViewById<TextView>(Resource.Id.detailText);
            Category = itemView.FindViewById<TextView>(Resource.Id.categoryText);
            Ratio = itemView.FindViewById<LikeRatioBar>(Resource.Id.ratioBar);
        }
    }
}