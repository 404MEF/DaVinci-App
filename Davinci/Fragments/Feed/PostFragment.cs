using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;
using RecyclerViewAnimators.Animators;
using Android.Views.Animations;
using System.Threading.Tasks;
using Davinci.Api.Models;
using Davinci.Adapters.Feed;
using DialogFragment = Android.Support.V4.App.DialogFragment;
using Android.App;
using Davinci.Components;
using Android.Graphics;
using Android.Util;

namespace Davinci.Fragments.Feed
{
    class PostFragment : DialogFragment
    {
        TextView header;
        Button likeBtn, dislikeBtn;
        public static PostFragment newInstance(string id)
        {
            PostFragment fragment = new PostFragment();
            Bundle args = new Bundle();
            args.PutString("id", id);
            fragment.Arguments = args;
            return fragment;
        }

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {

            return base.OnCreateDialog(savedInstanceState);
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            Dialog.Window.RequestFeature(WindowFeatures.NoTitle);
            base.OnActivityCreated(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.FeedPost, null);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            string id = Arguments.GetString("id");

            header = view.FindViewById<TextView>(Resource.Id.categoryHeader);
            likeBtn = View.FindViewById<Button>(Resource.Id.likeBtn);
            dislikeBtn = View.FindViewById<Button>(Resource.Id.dislikeBtn);


            likeBtn.Click += (s, e) => votePost(id, 1);
            dislikeBtn.Click += (s, e) => votePost(id, 0);

            getPostDetails(id);
        }

        private void setVoteButtonState(int vote)
        {
            if (vote == 1)
            {
                //Liked
                likeBtn.Text = "Liked";
                likeBtn.Enabled = false;
                dislikeBtn.Text = "Dislike";
                dislikeBtn.Enabled = true;
            }
            else if (vote == 0)
            {
                //Disliked
                dislikeBtn.Text = "Disliked";
                dislikeBtn.Enabled = false;
                likeBtn.Text = "Like";
                likeBtn.Enabled = true;
            }
            else
            {
                //Not voted
                dislikeBtn.Text = "Dislike";
                dislikeBtn.Enabled = true;
                likeBtn.Text = "Like";
                likeBtn.Enabled = true;
            }
        }

        private void getPostDetails(string id)
        {
            Task.Run(async () =>
            {
                SinglePostModel postModel = await Api.DavinciApi.GetPostDetail(id);
                VoteModel voteModel = await Api.DavinciApi.GetVoteStatus(id);

                return new { p = postModel, v = voteModel };
            }).ContinueWith(t =>
            {
                //set fields
                View.FindViewById<TextView>(Resource.Id.categoryText).Text = "Owner: " + t.Result.p.post.owner.username;
                View.FindViewById<TextView>(Resource.Id.detailText).Text = "Uploaded at " + t.Result.p.post.createdAt.Split('T')[0];
       
                setVoteButtonState(t.Result.v.vote);

                var imageData = Base64.Decode(t.Result.p.post.image, Base64Flags.Default);
                var bitmap = BitmapFactory.DecodeByteArray(imageData, 0, imageData.Length);
                View.FindViewById<SquareImageView>(Resource.Id.imageView).SetImageBitmap(bitmap);
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void votePost(string id, int vote)
        {
            bool likeState = likeBtn.Enabled;
            bool dislikeState = likeBtn.Enabled;

            likeBtn.Enabled = false;
            dislikeBtn.Enabled = false;

            Task.Run(async () =>
            {
                var response = await Api.DavinciApi.VotePost(id, vote);

                return response;
            }).ContinueWith(t =>
            {
                if (t.Result.OK)
                {
                    setVoteButtonState(vote);
                }
                else
                {
                    likeBtn.Enabled = likeState;
                    dislikeBtn.Enabled = dislikeState;
                }
            },TaskScheduler.FromCurrentSynchronizationContext());
        }
    }
}