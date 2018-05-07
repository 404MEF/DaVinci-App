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
using Davinci.Adapters.Feed;
using System.Threading.Tasks;

namespace Davinci.Adapters
{
    class EndlessScrollListener : RecyclerView.OnScrollListener
    {
        private int previousTotal = 0;
        private bool loading = true;
        private int visibleThreshold = 5;
        private int firstVisibleItem, visibleItemCount, totalItemCount;
        private int page = 1;

        private LinearLayoutManager mLayoutManager;

        public event Action<int> OnEndReach;

        public EndlessScrollListener(LinearLayoutManager layoutManager)
        {
            this.mLayoutManager = layoutManager;
        }

        public override void OnScrolled(RecyclerView recyclerView, int dx, int dy)
        {
            base.OnScrolled(recyclerView, dx, dy);

            visibleItemCount = recyclerView.ChildCount;
            totalItemCount = mLayoutManager.ItemCount;
            firstVisibleItem = mLayoutManager.FindFirstVisibleItemPosition();

            if (loading)
            {
                if (totalItemCount > previousTotal)
                {
                    loading = false;
                    previousTotal = totalItemCount;
                }
            }
            if (!loading && (totalItemCount - visibleItemCount) <= (firstVisibleItem + visibleThreshold))
            {
                // End has been reached
                var adapter = recyclerView.GetAdapter() as FeedPostAdapter;

                if (OnEndReach != null)
                {
                    OnEndReach(page);
                    page++;
                }

                loading = true;
            }

        }
    }
}