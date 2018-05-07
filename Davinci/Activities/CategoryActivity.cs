using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;
using Android.Views.Animations;
using System.Threading.Tasks;
using Davinci.Api.Models;
using Davinci.Adapters.Feed;
using Davinci.Activities;
using Davinci.Fragments.Feed;
using Davinci.Adapters.Search;

namespace Davinci.Activities
{
    [Activity(Theme = "@style/DavinciTheme.Feed", WindowSoftInputMode = SoftInput.AdjustPan, NoHistory = true)]
    public class CategoryActivity : ToolbarActivity
    {
        TextView imageCount;
        Button followBtn;

        RecyclerView recyclerView;
        RecyclerView.LayoutManager viewManager;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Category);

            string id = Intent.GetStringExtra("id");
            string name = Intent.GetStringExtra("name");
            int count = Intent.GetIntExtra("count", 0);

            string categoryHeader = "#" + name.First().ToString().ToUpper() + name.Substring(1);

            SetActionBar(categoryHeader);

            imageCount = FindViewById<TextView>(Resource.Id.imageCountField);

            imageCount.Text = count.ToString() + " Images";

            followBtn = FindViewById<Button>(Resource.Id.followBtn);
            followBtn.Click += (s, e) => follow(id);

            viewManager = new GridLayoutManager(this, 4);

            recyclerView = FindViewById<RecyclerView>(Resource.Id.categoryRecyclerView);
            recyclerView.HasFixedSize = true;
            recyclerView.SetLayoutManager(viewManager);

            getFollowStatus(id);
            getPosts(id);
        }


        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.category_toolbar_menu, menu);
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

        private void follow(string id)
        {
            Task.Run(async () =>
            {
                var response = await Api.DavinciApi.FollowCategory(id);
                return response;
            }).ContinueWith(t =>
            {
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

        private void getFollowStatus(string id)
        {
            Task.Run(async () =>
            {
                FollowModel followModel = await Api.DavinciApi.GetFollowStatus(id);
                return followModel;
            }).ContinueWith(t =>
            {
                int followStatus = t.Result.follow;
                if (followStatus == 1)
                {
                    followBtn.Text = "Unfollow";
                }
                else
                {
                    followBtn.Text = "Follow";
                }
=            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void getPosts(string id)
        {
            Task.Run(async () =>
            {
                SingleCategoryModel postCollection = await Api.DavinciApi.GetCategoryPosts(id);
                return postCollection;
            }).ContinueWith(t =>
            {
                RunOnUiThread(() =>
                {
                    var viewAdapter = new CategoryGridAdapter(t.Result.category.posts);
                    viewAdapter.ItemClick += (p) =>
                    {
                        PostFragment postFragment = PostFragment.newInstance(p._id);
                        postFragment.Show(SupportFragmentManager, "post");
                    };
                    recyclerView.SetAdapter(viewAdapter);

                });
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }
    }
}