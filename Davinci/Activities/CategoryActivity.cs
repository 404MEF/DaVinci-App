using System.Linq;
using System.Threading.Tasks;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;

using Davinci.Adapters.Feed;
using Davinci.Fragments.Feed;

namespace Davinci.Activities
{
    [Activity(Theme = "@style/DavinciTheme.Feed", WindowSoftInputMode = SoftInput.AdjustPan, NoHistory = true)]
    public class CategoryActivity : ToolbarActivity
    {
        private const int COLUMN_COUNT = 4;
        private string id;

        TextView imageCount;
        Button followBtn;

        RecyclerView recyclerView;
        RecyclerView.LayoutManager viewManager;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            id = Intent.GetStringExtra("id");
            string name = Intent.GetStringExtra("name");
            int count = Intent.GetIntExtra("count", 0);

            string categoryHeader = "#" + name.First().ToString().ToUpper() + name.Substring(1);

            SetContentView(Resource.Layout.Category);
            SetActionBar(categoryHeader);

            setUI();
            setEvents();

            imageCount.Text = count.ToString() + " Images";

            recyclerView.HasFixedSize = true;
            recyclerView.SetLayoutManager(viewManager);

            getFollowStatus();
            getPosts();
        }

        private void setUI()
        {
            imageCount = FindViewById<TextView>(Resource.Id.imageCountField);
            followBtn = FindViewById<Button>(Resource.Id.followBtn);
            viewManager = new GridLayoutManager(this, COLUMN_COUNT);
            recyclerView = FindViewById<RecyclerView>(Resource.Id.categoryRecyclerView);
        }

        private void setEvents()
        {
            followBtn.Click += (s, e) => follow();
        }

        private void follow()
        {
            Task.Run(async () =>
            {
                return await Api.DavinciApi.FollowCategory(id);
            }).ContinueWith(t =>
            {
                if (t.Status == TaskStatus.Canceled)
                    return;

                if (t.Result.OK)
                {
                    if (followBtn.Text == "Unfollow")
                    {
                        followBtn.Text = "Follow";
                    }
                    else
                    {
                        followBtn.Text = "Unfollow";
                    }
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void getFollowStatus()
        {
            Task.Run(async () =>
            {
                return await Api.DavinciApi.GetFollowStatus(id);
            }).ContinueWith(t =>
            {
                if (t.Status == TaskStatus.Canceled)
                    return;

                if (t.Result.OK)
                {
                    int followStatus = t.Result.follow;
                    if (followStatus == 1)
                        followBtn.Text = "Unfollow";
                    else
                        followBtn.Text = "Follow";
                }
            },TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void getPosts()
        {
            Task.Run(async () =>
            {
                return await Api.DavinciApi.GetCategoryPosts(id);
            }).ContinueWith(t =>
            {
                if (t.Status == TaskStatus.Canceled)
                    return;

                if (t.Result.OK)
                {
                    var viewAdapter = new CategoryGridAdapter(t.Result.category.posts);
                    viewAdapter.ItemClick += (p) =>
                    {
                        PostFragment postFragment = PostFragment.newInstance(p._id);
                        postFragment.Show(SupportFragmentManager, "post");
                    };
                    recyclerView.SetAdapter(viewAdapter);
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.activity_toolbar_menu, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.menu_close:
                    this.Finish();
                    return true;
            }

            return base.OnOptionsItemSelected(item);
        }
    }
}