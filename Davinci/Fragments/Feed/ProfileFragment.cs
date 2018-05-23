using System.Threading.Tasks;

using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;

using Davinci.Adapters.Feed;
using Davinci.Api.Models;
using System;

namespace Davinci.Fragments.Feed
{
    class ProfileFragment : BaseFragment
    {
        private const int COLUMN_COUNT = 4;

        TextView likesHeader, postsHeader;
        RecyclerView likesRecyclerView, postsRecyclerView;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.ProfileFragment, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            setUI();
            setEvents();
        }

        public override void OnViewTrigger()
        {
            getProfilePosts();
        }

        private void setUI()
        {
            likesHeader = View.FindViewById<TextView>(Resource.Id.likeHeader);
            postsHeader = View.FindViewById<TextView>(Resource.Id.postHeader);
            likesRecyclerView = View.FindViewById<RecyclerView>(Resource.Id.likesRecyclerView);
            postsRecyclerView = View.FindViewById<RecyclerView>(Resource.Id.postsRecyclerView);

            var likesLayoutManager = new GridLayoutManager(this.Context, COLUMN_COUNT);
            var postsLayoutManager = new GridLayoutManager(this.Context, COLUMN_COUNT);

            likesRecyclerView.SetLayoutManager(likesLayoutManager);
            postsRecyclerView.SetLayoutManager(postsLayoutManager);

        }

        private void setEvents()
        {

        }

        private void getProfilePosts()
        {
            Task.Run(async () =>
            {
                return await Api.DavinciApi.GetProfilePosts();
            }).ContinueWith(t =>
            {
                if (t.Status == TaskStatus.Canceled)
                    return;

                if (t.Result.OK)
                {
                    CategoryGridAdapter likedPostAdapter = new CategoryGridAdapter(t.Result.likedPosts);
                    CategoryGridAdapter userPostAdapter = new CategoryGridAdapter(t.Result.userPosts);

                    Action<PostModel> onClickPost = (post) =>
                    {
                        PostFragment postFragment = PostFragment.newInstance(post._id);
                        postFragment.Show(parentActivity.SupportFragmentManager, "post");
                    };
                    likedPostAdapter.ItemClick += onClickPost;
                    userPostAdapter.ItemClick += onClickPost;

                    likesRecyclerView.SetAdapter(likedPostAdapter);
                    postsRecyclerView.SetAdapter(userPostAdapter);
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