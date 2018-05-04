using System.Linq;
using System.Threading.Tasks;

using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;

using Davinci.Adapters.Search;
using Davinci.Api.Models;
using RecyclerViewAnimators.Animators;
using Davinci.Activities;
using Android.Content;
using Android.App;

namespace Davinci.Fragments.Feed
{
    class SearchFragment : BaseFragment
    {
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

            header = view.FindViewById<TextView>(Resource.Id.header);

            searchField = view.FindViewById<AutoCompleteTextView>(Resource.Id.searchCategoryField);
            searchField.AfterTextChanged += SearchField_AfterTextChanged;

            searchBtn = view.FindViewById<Button>(Resource.Id.searchBtn);
            searchBtn.Click += (s, e) => searchCategories();

            popularViewManager = new LinearLayoutManager(Context);
            searchViewManager = new LinearLayoutManager(Context);

            popularRecyclerView = view.FindViewById<RecyclerView>(Resource.Id.popularRecyclerView);
            popularRecyclerView.HasFixedSize = true;
            popularRecyclerView.SetLayoutManager(popularViewManager);

            searchRecyclerView = view.FindViewById<RecyclerView>(Resource.Id.searchRecyclerView);
            searchRecyclerView.HasFixedSize = true;
            searchRecyclerView.SetLayoutManager(searchViewManager);

            var animator = new SlideInUpAnimator(new OvershootInterpolator(1f));
            popularRecyclerView.SetItemAnimator(animator);
            searchRecyclerView.SetItemAnimator(animator);
        }

        public override void OnResume()
        {
            base.OnResume();

            getPopularCategories();
        }

        private void SearchField_AfterTextChanged(object sender, Android.Text.AfterTextChangedEventArgs e)
        {
            if (searchField.Text.Length >= 3)
            {
                //Get search suggestions
                Task.Run(async () =>
                {
                    SearchSuggestionModel suggestions = await Api.DavinciApi.GetSearchSuggestions(searchField.Text);
                    return suggestions;
                }).ContinueWith(t =>
                {
                    if (suggestionAdapter == null)
                    {
                        suggestionAdapter = new ArrayAdapter<string>(this.Context, Resource.Layout.Search_dropdownItem, t.Result.results.Select(n => n.name).ToArray());
                        searchField.Adapter = suggestionAdapter;
                    }
                    else
                    {
                        suggestionAdapter.Clear();
                        suggestionAdapter.AddAll(t.Result.results.Select(n => n.name).ToList());
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
                getPopularCategories();
            }
        }

        private void getPopularCategories()
        {
            Task.Run(async () =>
            {
                CategoryCollectionModel categoryCollection = await Api.DavinciApi.GetPopularCategories();
                return categoryCollection;
            }).ContinueWith(t =>
            {
                var k = t.Result.categories;
                RunOnUIThread(() =>
                {
                    var popularAdapter = new CategoryAdapter(this.Context, t.Result.categories);
                    popularAdapter.ItemClick += (c) =>
                    {
                        var intent = new Intent(Application.Context, typeof(CategoryActivity));
                        intent.PutExtra("id", c._id);
                        intent.PutExtra("name", c.name);
                        intent.PutExtra("count", c.imagecount);
                        StartActivity(intent);
                        //StartActivity(new Intent(Application.Context, typeof(CategoryActivity)));

                    };
                    popularRecyclerView.SetAdapter(popularAdapter);
                });
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
                var response = await Api.DavinciApi.SearchCategory(category);
                return response;
            }).ContinueWith(t =>
            {
                RunOnUIThread(() =>
                {
                    var searchAdapter = new CategoryAdapter(this.Context, t.Result.categories);
                    searchAdapter.ItemClick += (c) =>
                    {
                        var intent = new Intent(Application.Context, typeof(CategoryActivity));
                        intent.PutExtra("id", c._id);
                        intent.PutExtra("name", c.name);
                        intent.PutExtra("count", c.imagecount);
                        //StartActivity(new Intent(Application.Context, typeof(CategoryActivity)));
                    };
                    searchRecyclerView.SetAdapter(searchAdapter);
                });
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }
    }
}