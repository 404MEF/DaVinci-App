using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Globalization;

using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Graphics;

using Davinci.Api.Models;
using Davinci.Components;
using Davinci.Helper;


namespace Davinci.Adapters.Feed
{
    public class FeedPostAdapter : RecyclerView.Adapter
    {
        private List<PostModel> items { get; set; }

        public FeedPostAdapter(FeedModel model)
        {
            this.items = model.data.follows.SelectMany(n => n.posts).OrderBy(k => DateTime.ParseExact(k.createdAt.Split('T')[0], "yyyy-MM-dd", CultureInfo.InvariantCulture)).ToList();
        }

        public override int ItemCount
        {
            get
            {
                return items.Count;
            }
        }

        public void Clear()
        {
            this.items.Clear();
        }

        public void Add(IEnumerable<PostModel> item)
        {
            items.AddRange(item);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var item = items[position];

            var viewHolder = holder as FeedItemAdapterViewHolder;

            var ownerText = string.Format("Uploaded by {0} at {1}", item.owner.username, item.createdAt.Split('T')[0]);
            var categoryText = "#" + item.category.name.Capitalize();

            viewHolder.Owner.Text = ownerText;
            viewHolder.Category.Text = categoryText;
            viewHolder.Ratio.SetRatio(item.LikeRatio);

            //Get vote state
            Task.Run(async () =>
            {
                return await Api.DavinciApi.GetVoteStatus(item._id);
            }).ContinueWith(t =>
            {
                if (t.Result.OK)
                    setVoteButtonState(viewHolder, t.Result.vote);
            }, TaskScheduler.FromCurrentSynchronizationContext());


            viewHolder.Like.Click += (s, e) => vote(viewHolder, item, 1);
            viewHolder.Dislike.Click += (s, e) => vote(viewHolder, item, -1);

            var imageData = Base64.Decode(item.image, Base64Flags.Default);
            var imageSrc = BitmapFactory.DecodeByteArray(imageData, 0, imageData.Length);

            viewHolder.Image.SetImageBitmap(imageSrc);
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var view = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.FeedPost, parent, false);
            return new FeedItemAdapterViewHolder(view);
        }

        private void vote(FeedItemAdapterViewHolder viewHolder, PostModel item, int vote)
        {
            Task.Run(async () =>
            {
                return await Api.DavinciApi.VotePost(item._id, vote);
            }).ContinueWith(t =>
            {
                if (t.Status != TaskStatus.Canceled && t.Result.OK)
                {
                    item.likes = t.Result.likes;
                    item.dislikes = t.Result.dislikes;
                    viewHolder.Ratio.SetRatio(item.LikeRatio);
                    setVoteButtonState(viewHolder, vote);
                }
            },TaskScheduler.FromCurrentSynchronizationContext());
        }


        private void setVoteButtonState(FeedItemAdapterViewHolder viewHolder, int vote)
        {
            if (vote == 1)
            {
                //Liked
                viewHolder.Like.Text = "Liked";
                viewHolder.Like.Enabled = false;
                viewHolder.Dislike.Text = "Dislike";
                viewHolder.Dislike.Enabled = true;
            }
            else if (vote == -1)
            {
                //Disliked
                viewHolder.Dislike.Text = "Disliked";
                viewHolder.Dislike.Enabled = false;
                viewHolder.Like.Text = "Like";
                viewHolder.Like.Enabled = true;
            }
            else
            {
                //Not voted
                viewHolder.Dislike.Text = "Dislike";
                viewHolder.Dislike.Enabled = true;
                viewHolder.Like.Text = "Like";
                viewHolder.Like.Enabled = true;
            }
        }
    }

    public class FeedItemAdapterViewHolder : RecyclerView.ViewHolder
    {
        public ImageView Image { get; private set; }
        public TextView Owner { get; private set; }
        public TextView Category { get; private set; }
        public LikeRatioBar Ratio { get; private set; }
        public Button Like { get; private set; }
        public Button Dislike { get; private set; }

        public FeedItemAdapterViewHolder(View itemView) : base(itemView)
        {
            Image = itemView.FindViewById<ImageView>(Resource.Id.imageView);
            Owner = itemView.FindViewById<TextView>(Resource.Id.detailText);
            Category = itemView.FindViewById<TextView>(Resource.Id.categoryText);
            Ratio = itemView.FindViewById<LikeRatioBar>(Resource.Id.ratioBar);
            Like = itemView.FindViewById<Button>(Resource.Id.likeBtn);
            Dislike = itemView.FindViewById<Button>(Resource.Id.dislikeBtn);

        }
    }
}