using System.Threading.Tasks;

using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;

using Davinci.Adapters;
using Davinci.Api.Models;

namespace Davinci.Fragments.Feed
{
    class FeedFragment : BaseFragment
    {
        RecyclerView recyclerView;
        RecyclerView.Adapter viewAdapter;
        RecyclerView.LayoutManager viewManager;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.FeedFragment, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            Task.Run(async () =>
            {
                PostCollectionModel postCollection = await Api.DavinciApi.GetFeedPosts(0);
                return postCollection;
            }).ContinueWith(t =>
            {
                viewAdapter = new FeedPostAdapter(t.Result.posts);
                RunOnUIThread(() =>
                {
                    Infobar.Show(Context, t.Result.message, Infobar.InfoLevel.Info, GravityFlags.Bottom | GravityFlags.FillHorizontal);

                    viewManager = new LinearLayoutManager(Context);

                    recyclerView = view.FindViewById<RecyclerView>(Resource.Id.Feed_recyclerView);
                    recyclerView.HasFixedSize = true;
                    recyclerView.SetLayoutManager(viewManager);
                    recyclerView.SetAdapter(viewAdapter);
                });
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }
    }
}