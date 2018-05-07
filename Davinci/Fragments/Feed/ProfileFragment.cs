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
using System.Threading.Tasks;
using Davinci.Api.Models;
using Davinci.Adapters.Feed;

namespace Davinci.Fragments.Feed
{
    class ProfileFragment : BaseFragment
    {
        TextView likesHeader, postsHeader;

        RecyclerView likesRecyclerView, postsRecyclerView;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.ProfileFragment, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            likesHeader = view.FindViewById<TextView>(Resource.Id.likeHeader);
            postsHeader = view.FindViewById<TextView>(Resource.Id.postHeader);
            likesRecyclerView = view.FindViewById<RecyclerView>(Resource.Id.likesRecyclerView);
            postsRecyclerView = view.FindViewById<RecyclerView>(Resource.Id.postsRecyclerView);

            var likesLayoutManager = new GridLayoutManager(this.Context, 4);
            var postsLayoutManager = new GridLayoutManager(this.Context, 4);

            likesRecyclerView.SetLayoutManager(likesLayoutManager);
            postsRecyclerView.SetLayoutManager(postsLayoutManager);

            getProfilePosts();
        }

        private void getProfilePosts()
        {
            Task.Run(async () =>
            {
                return await Api.DavinciApi.GetProfilePosts();
            }).ContinueWith(t =>
            {
                if (t.Result.OK)
                {
                    CategoryGridAdapter adapter = new CategoryGridAdapter(t.Result.posts);
                    adapter.ItemClick += (post) =>
                    {
                        PostFragment postFragment = PostFragment.newInstance(post._id);
                        postFragment.Show(parentActivity.SupportFragmentManager, "post");
                    };
                    likesRecyclerView.SetAdapter(adapter);
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        //private void getPopularCategories()
        //{
        //    Task.Run(async () =>
        //    {
        //        CategoryCollectionModel categoryCollection = await Api.DavinciApi.GetPopularCategories();
        //        return categoryCollection;
        //    }).ContinueWith(t =>
        //    {
        //        var k = t.Result.categories;
        //        RunOnUIThread(() =>
        //        {
        //            var popularAdapter = new CategoryAdapter(this.Context, t.Result.categories);
        //            popularAdapter.ItemClick += (c) =>
        //            {
        //                var intent = new Intent(Application.Context, typeof(CategoryActivity));
        //                intent.PutExtra("id", c._id);   
        //                intent.PutExtra("name", c.name);
        //                intent.PutExtra("count", c.imagecount);
        //                StartActivity(intent);
        //            };
        //            popularRecyclerView.SetAdapter(popularAdapter);
        //        });
        //    }, TaskScheduler.FromCurrentSynchronizationContext());
        //}

        //private void searchCategories()
        //{
        //    string category = searchField.Text;

        //    if (string.IsNullOrEmpty(category))
        //        return;

        //    header.Text = "Search Results";
        //    popularRecyclerView.Visibility = ViewStates.Gone;
        //    searchRecyclerView.Visibility = ViewStates.Visible;

        //    //Get search results
        //    Task.Run(async () =>
        //    {
        //        var response = await Api.DavinciApi.SearchCategory(category);
        //        return response;
        //    }).ContinueWith(t =>
        //    {
        //        RunOnUIThread(() =>
        //        {
        //            var searchAdapter = new CategoryAdapter(this.Context, t.Result.categories);
        //            searchAdapter.ItemClick += (c) =>
        //            {
        //                var intent = new Intent(Application.Context, typeof(CategoryActivity));
        //                intent.PutExtra("id", c._id);
        //                intent.PutExtra("name", c.name);
        //                intent.PutExtra("count", c.imagecount);
        //                StartActivity(new Intent(Application.Context, typeof(CategoryActivity)));
        //            };
        //            searchRecyclerView.SetAdapter(searchAdapter);
        //        });
        //    }, TaskScheduler.FromCurrentSynchronizationContext());
        //}
    }
}