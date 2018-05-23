using System.Threading.Tasks;
using System.Linq;

using Android.OS;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using Android.Content;
using Android.App;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;

using Davinci.Adapters.Feed;
using Davinci.Activities;
using Android.Support.Design.Widget;

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

            setUI();
            setEvents();

            swipeRefreshLayout.SetColorSchemeColors(Color.IndianRed, Color.LimeGreen, Color.AliceBlue, Color.LightGoldenrodYellow);
 
            //var scrollListener = new EndlessScrollListener(viewManager);
            recyclerView.HasFixedSize = true;
            recyclerView.NestedScrollingEnabled = true;
            recyclerView.SetLayoutManager(viewManager);
            //recyclerView.AddOnScrollListener(scrollListener);
            //scrollListener.OnEndReach += (e) => getPosts(e);

            getPosts(0);
        }

        private void setUI()
        {
            scrollView = View.FindViewById<ScrollView>(Resource.Id.scrollView);
            noContentTextView = View.FindViewById<TextView>(Resource.Id.noContent);
            fab = View.FindViewById<FloatingActionButton>(Resource.Id.fab);
            swipeRefreshLayout = View.FindViewById<SwipeRefreshLayout>(Resource.Id.swipeRefreshLayout);

            recyclerView = View.FindViewById<RecyclerView>(Resource.Id.recyclerView);
            viewManager = new LinearLayoutManager(Context);
        }

        private void setEvents()
        {
            //Start upload activity when fab is clicked
            fab.Click += (s, e) => parentActivity.StartActivity(new Intent(Application.Context, typeof(UploadActivity)));

            //Scroll to top
            ((ToolbarActivity)parentActivity).ToolBar.Click += (s, e) => scrollView.ScrollTo(0, 0);

            //Refresh posts
            swipeRefreshLayout.Refresh += (s, e) => getPosts(0);
        }

        private void getPosts(int page)
        {
            Task.Run(async () =>
            {
                return await Api.DavinciApi.GetFeedPosts(page);
            }).ContinueWith(t =>
            {
                if (t.Status == TaskStatus.Canceled || t.Result.data.follows.Count == 0)
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