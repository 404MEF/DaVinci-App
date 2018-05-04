using System.Threading.Tasks;

using Android.OS;
using Android.Graphics;
using Android.Views;
using Android.Views.Animations;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;

using RecyclerViewAnimators.Animators;

using Davinci.Adapters.Feed;
using Davinci.Api.Models;
using Davinci.Activities;
using System.Linq;
using Android.Widget;

namespace Davinci.Fragments.Feed
{
    class FeedFragment : BaseFragment
    {
        SwipeRefreshLayout swipeRefreshLayout;
        NestedScrollView scrollView;

        RecyclerView recyclerView;
        RecyclerView.LayoutManager viewManager;
        FeedPostAdapter viewAdapter;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.FeedFragment, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            swipeRefreshLayout = view.FindViewById<SwipeRefreshLayout>(Resource.Id.swipeRefreshLayout);
            swipeRefreshLayout.SetColorSchemeColors(Color.IndianRed, Color.LimeGreen, Color.AliceBlue, Color.LightGoldenrodYellow);
            swipeRefreshLayout.Refresh += (s, e) => getPosts();

            scrollView = view.FindViewById<NestedScrollView>(Resource.Id.scrollView);
            ((ToolbarActivity)parentActivity).ToolBar.Click += (s, e) => scrollView.ScrollTo(0, 0);

            viewManager = new LinearLayoutManager(Context);

            recyclerView = view.FindViewById<RecyclerView>(Resource.Id.recyclerView);
            recyclerView.HasFixedSize = true;
            recyclerView.NestedScrollingEnabled = false;
            recyclerView.SetLayoutManager(viewManager);

            var animator = new SlideInUpAnimator(new OvershootInterpolator(1f));
            recyclerView.SetItemAnimator(animator);

            getPosts();
        }

        private void getPosts()
        {
            Task.Run(async () =>
            {
                FeedModel postCollection = await Api.DavinciApi.GetFeedPosts(20);
                return postCollection;
            }).ContinueWith(t =>
            {
                if (t.Result.data.follows.Count == 0)
                {
                    View.FindViewById<TextView>(Resource.Id.noContent).Visibility = ViewStates.Visible;
                }
                else
                {
                    View.FindViewById<TextView>(Resource.Id.noContent).Visibility = ViewStates.Gone;
                }
                if (viewAdapter == null)
                {
                    viewAdapter = new FeedPostAdapter(t.Result);
                    recyclerView.SetAdapter(viewAdapter);
                }
                else
                {
                    viewAdapter.Clear();
                    viewAdapter.Add(t.Result.data.follows.SelectMany(n => n.posts));
                    viewAdapter.NotifyDataSetChanged();
                }

                swipeRefreshLayout.Refreshing = false;
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

    }
}