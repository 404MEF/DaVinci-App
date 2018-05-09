using System.Threading.Tasks;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.App;
using Android.Graphics;
using Android.Util;
using DialogFragment = Android.Support.V4.App.DialogFragment;

using Davinci.Api.Models;
using Davinci.Components;

namespace Davinci.Fragments.Feed
{
    class PostFragment : DialogFragment
    {
        private string id;

        TextView headerTextView, detailTextView;
        ImageView imageView;
        Button likeBtn, dislikeBtn;
        LikeRatioBar ratioBar;

        public static PostFragment newInstance(string id)
        {
            PostFragment fragment = new PostFragment();
            Bundle args = new Bundle();
            args.PutString("id", id);
            fragment.Arguments = args;
            return fragment;
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
            id = Arguments.GetString("id");

            setUI();
            setEvents();

            getPostDetails();
        }

        private void setUI()
        {
            headerTextView = View.FindViewById<TextView>(Resource.Id.categoryText);
            detailTextView = View.FindViewById<TextView>(Resource.Id.detailText);
            likeBtn = View.FindViewById<Button>(Resource.Id.likeBtn);
            dislikeBtn = View.FindViewById<Button>(Resource.Id.dislikeBtn);
            ratioBar = View.FindViewById<LikeRatioBar>(Resource.Id.ratioBar);
            imageView = View.FindViewById<SquareImageView>(Resource.Id.imageView);
        }

        private void setEvents()
        {
            likeBtn.Click += (s, e) => sendVote(id, 1);
            dislikeBtn.Click += (s, e) => sendVote(id, -1);
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
            else if (vote == -1)
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

        private void getPostDetails()
        {
            Task.Run(async () =>
            {
                SinglePostModel postModel = await Api.DavinciApi.GetPostDetail(id);
                VoteModel voteModel = await Api.DavinciApi.GetVoteStatus(id);

                return new { p = postModel, v = voteModel };
            }).ContinueWith(t =>
            {
                //set fields
                headerTextView.Text = "Owner: " + t.Result.p.post.owner.username;
                detailTextView.Text = "Uploaded at " + t.Result.p.post.createdAt.Split('T')[0];

                setVoteButtonState(t.Result.v.vote);

                ratioBar.SetRatio(t.Result.p.post.LikeRatio);

                var imageData = Base64.Decode(t.Result.p.post.image, Base64Flags.Default);
                var bitmap = BitmapFactory.DecodeByteArray(imageData, 0, imageData.Length);

                imageView.SetImageBitmap(bitmap);

            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void sendVote(string id, int vote)
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

                    ratioBar.SetRatio(getLikeRatio(t.Result.likes, t.Result.dislikes));
                }
                else
                {
                    likeBtn.Enabled = likeState;
                    dislikeBtn.Enabled = dislikeState;
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private int getLikeRatio(int likes, int dislikes)
        {
            return (int)(100 * likes / (likes + dislikes));
        }
    }
}