using System.Threading.Tasks;

using Android.OS;
using Android.Graphics;
using Android.Views;
using Android.Views.Animations;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;

using Davinci.Adapters.Feed;
using Davinci.Api.Models;
using Davinci.Activities;
using System.Linq;
using Android.Widget;
using Refractored.Fab;
using Android.Content;
using Android.App;
using Davinci.Adapters;

namespace Davinci.Fragments.Feed
{
    class FeedFragment : BaseFragment
    {
        SwipeRefreshLayout swipeRefreshLayout;
        ScrollView scrollView;

        FloatingActionButton fab;
        TextView noContentTextView;

        RecyclerView recyclerView;
        LinearLayoutManager viewManager;
        FeedPostAdapter viewAdapter;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.FeedFragment, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            scrollView = view.FindViewById<ScrollView>(Resource.Id.scrollView);
            recyclerView = view.FindViewById<RecyclerView>(Resource.Id.recyclerView);

            noContentTextView = view.FindViewById<TextView>(Resource.Id.noContent);

            fab = view.FindViewById<FloatingActionButton>(Resource.Id.fab);
            fab.AttachToRecyclerView(recyclerView);
            fab.Click += (s, e) => parentActivity.StartActivity(new Intent(Application.Context, typeof(UploadActivity)));

            swipeRefreshLayout = view.FindViewById<SwipeRefreshLayout>(Resource.Id.swipeRefreshLayout);
            swipeRefreshLayout.SetColorSchemeColors(Color.IndianRed, Color.LimeGreen, Color.AliceBlue, Color.LightGoldenrodYellow);
            swipeRefreshLayout.Refresh += (s, e) => getPosts(0);

            ((ToolbarActivity)parentActivity).ToolBar.Click += (s, e) => scrollView.ScrollTo(0, 0);

            viewManager = new LinearLayoutManager(Context);
            var scrollListener = new EndlessScrollListener(viewManager);

            recyclerView.HasFixedSize = true;
            recyclerView.NestedScrollingEnabled = false;
            recyclerView.SetLayoutManager(viewManager);
            recyclerView.AddOnScrollListener(scrollListener);

            getPosts(0);
            //scrollListener.OnEndReach += (e) => getPosts(e);
        }

        private void getPosts(int page)
        {
            Task.Run(async () =>
            {
                FeedModel postCollection = await Api.DavinciApi.GetFeedPosts(page);
                return postCollection;
            }).ContinueWith(t =>
            {
                if (t.Result.data.follows.Count == 0)
                {
                    noContentTextView.Visibility = ViewStates.Visible;
                }
                else
                {
                    noContentTextView.Visibility = ViewStates.Gone;

                    if (viewAdapter == null)
                    {
                        viewAdapter = new FeedPostAdapter(t.Result);
                        recyclerView.SetAdapter(viewAdapter);
                    }
                    else
                    {
                        if (page == 0)
                            viewAdapter.Clear();

                        viewAdapter.Add(t.Result.data.follows.SelectMany(n => n.posts));
                        viewAdapter.NotifyDataSetChanged();
                    }
                }

                swipeRefreshLayout.Refreshing = false;
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

    }
}