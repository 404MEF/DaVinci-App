using System.Collections.Generic;
using System.Linq;

using Android.Views;
using Android.Widget;
using Android.Content;
using Android.Support.V7.Widget;

using Davinci.Api.Models;
using System;

namespace Davinci.Adapters.Search
{
    public class CategoryAdapter : RecyclerView.Adapter
    {
        private Context mContext;
        private List<CategoryModel> items;
        public event Action<CategoryModel> ItemClick;

        public CategoryAdapter(Context context,List<CategoryModel> list)
        {
            this.mContext = context;
            this.items = list;
        }

        public override int ItemCount
        {
            get
            {
                return items.Count;
            }
        }

        public void Clear()
        {
            this.items.Clear();
        }

        public void Add(List<CategoryModel> list)
        {
            this.items = list;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var item = items[position];

            var viewHolder = holder as CategoryAdapterViewHolder;

            viewHolder.ImageCount.Text = string.Format("{0} Images", item.imagecount);
            viewHolder.Category.Text = "#" + item.name.First().ToString().ToUpper() + item.name.Substring(1);

            viewHolder.ImageGrid.Adapter = new ImageGridAdapter(mContext, item.posts);
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var view = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.SearchPost, parent, false);
            return new CategoryAdapterViewHolder(view,items,ItemClick);
        }
    }

    public class CategoryAdapterViewHolder : RecyclerView.ViewHolder
    {
        public TextView Category { get; private set; }
        public TextView ImageCount { get; private set; }
        public GridView ImageGrid { get; private set; }

        public CategoryAdapterViewHolder(View itemView, List<CategoryModel> items, Action<CategoryModel> listener) : base(itemView)
        {
            ImageGrid = itemView.FindViewById<GridView>(Resource.Id.gridView);
            ImageCount = itemView.FindViewById<TextView>(Resource.Id.imageCountField);
            Category = itemView.FindViewById<TextView>(Resource.Id.categoryField);

            itemView.Click += (sender, e) => listener(items[base.Position]);
        }
    }
}