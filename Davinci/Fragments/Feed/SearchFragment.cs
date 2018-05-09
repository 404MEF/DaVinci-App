using System.Linq;
using System.Threading.Tasks;

using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Content;
using Android.App;
using Android.Widget;

using Davinci.Adapters.Search;
using Davinci.Activities;
using Davinci.Helper;

namespace Davinci.Fragments.Feed
{
    class SearchFragment : BaseFragment
    {
        const int SEARCH_SUGGESTION_START_LENGTH = 2;
        TextView header;

        AutoCompleteTextView searchField;
        ArrayAdapter<string> suggestionAdapter;

        Button searchBtn;

        RecyclerView popularRecyclerView, searchRecyclerView;
        RecyclerView.LayoutManager popularViewManager, searchViewManager;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.SearchFragment, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            setUI();
            setEvents();

            popularRecyclerView.HasFixedSize = true;
            popularRecyclerView.SetLayoutManager(popularViewManager);

            searchRecyclerView.HasFixedSize = true;
            searchRecyclerView.SetLayoutManager(searchViewManager);
        }

        private void setUI()
        {
            header = View.FindViewById<TextView>(Resource.Id.header);

            searchField = View.FindViewById<AutoCompleteTextView>(Resource.Id.searchCategoryField);
            searchBtn = View.FindViewById<Button>(Resource.Id.searchBtn);

            popularViewManager = new LinearLayoutManager(Context);
            searchViewManager = new LinearLayoutManager(Context);

            popularRecyclerView = View.FindViewById<RecyclerView>(Resource.Id.popularRecyclerView);
            searchRecyclerView = View.FindViewById<RecyclerView>(Resource.Id.searchRecyclerView);

        }

        private void setEvents()
        {
            searchField.AfterTextChanged += (s, e) => getSearchSuggestions();
            searchBtn.Click += (s, e) => searchCategories();
        }

        public override void OnResume()
        {
            base.OnResume();

            getPopularCategories();
        }

        private void getSearchSuggestions()
        {
            if (searchField.Text.Length >= SEARCH_SUGGESTION_START_LENGTH)
            {
                //Get search suggestions
                Task.Run(async () =>
                {
                    return await Api.DavinciApi.GetSearchSuggestions(searchField.Text);
                }).ContinueWith(t =>
                {
                    if (t.Status != TaskStatus.Canceled && t.Result.OK)
                        if (suggestionAdapter == null)
                        {
                            suggestionAdapter = new ArrayAdapter<string>(this.Context, Resource.Layout.Search_dropdownItem, t.Result.results.Select(n => "#" + n.name.Capitalize()).ToArray());
                            searchField.Adapter = suggestionAdapter;
                        }
                        else
                        {
                            suggestionAdapter.Clear();
                            suggestionAdapter.AddAll(t.Result.results.Select(n => "#" + n.name.Capitalize()).ToList());
                            suggestionAdapter.NotifyDataSetChanged();
                        }
                }, TaskScheduler.FromCurrentSynchronizationContext());
            }
            else
            {
                if (suggestionAdapter != null)
                {
                    suggestionAdapter.Clear();
                    suggestionAdapter.NotifyDataSetChanged();
                }

                //Show most popular categories
                header.Text = "Most Popular Categories";
                searchRecyclerView.Visibility = ViewStates.Gone;
                popularRecyclerView.Visibility = ViewStates.Visible;
                //getPopularCategories();
            }
        }

        private void getPopularCategories()
        {
            Task.Run(async () =>
            {
                return await Api.DavinciApi.GetPopularCategories();
            }).ContinueWith(t =>
            {
                if (t.Status != TaskStatus.Canceled && t.Result.OK)
                {
                    var popularAdapter = new CategoryAdapter(this.Context, t.Result.categories);
                    popularAdapter.ItemClick += (c) =>
                    {
                        var intent = new Intent(Application.Context, typeof(CategoryActivity));
                        intent.PutExtra("id", c._id);
                        intent.PutExtra("name", c.name);
                        intent.PutExtra("count", c.imagecount);
                        StartActivity(intent);
                    };
                    popularRecyclerView.SetAdapter(popularAdapter);
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void searchCategories()
        {
            string category = searchField.Text;

            if (string.IsNullOrEmpty(category))
                return;

            header.Text = "Search Results";
            popularRecyclerView.Visibility = ViewStates.Gone;
            searchRecyclerView.Visibility = ViewStates.Visible;

            //Get search results
            Task.Run(async () =>
            {
                return await Api.DavinciApi.SearchCategory(category);
            }).ContinueWith(t =>
            {
                if (t.Status != TaskStatus.Canceled && t.Result.OK)
                {
                    var searchAdapter = new CategoryAdapter(this.Context, t.Result.categories);
                    searchAdapter.ItemClick += (c) =>
                    {
                        var intent = new Intent(Application.Context, typeof(CategoryActivity));
                        intent.PutExtra("id", c._id);
                        intent.PutExtra("name", c.name);
                        intent.PutExtra("count", c.imagecount);
                        StartActivity(intent);
                    };
                    searchRecyclerView.SetAdapter(searchAdapter);
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

    }
}