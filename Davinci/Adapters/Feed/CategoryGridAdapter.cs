using System.Collections.Generic;

using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Graphics;

using Davinci.Api.Models;
using System;
using Davinci.Components;

namespace Davinci.Adapters.Feed
{
    public class CategoryGridAdapter : RecyclerView.Adapter
    {
        private List<PostModel> items { get; set; }
        public event Action<PostModel> ItemClick;

        public CategoryGridAdapter(List<PostModel> list)
        {
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

        public void Add(PostModel item)
        {
            items.Add(item);
        }

        public void Add(IEnumerable<PostModel> item)
        {
            items.AddRange(item);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var item = items[position];

            var viewHolder = holder as CategoryItemAdapterViewHolder;
            
            var imageData = Base64.Decode(item.image, Base64Flags.Default);
            var imageSrc = BitmapFactory.DecodeByteArray(imageData, 0, imageData.Length);

            viewHolder.Image.SetImageBitmap(imageSrc);
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var view = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.CategoryGridItem, parent, false);
            return new CategoryItemAdapterViewHolder(view,items,ItemClick);
        }
    }

    public class CategoryItemAdapterViewHolder : RecyclerView.ViewHolder
    {
        public SquareImageView Image { get; private set; }
        public CategoryItemAdapterViewHolder(View itemView, List<PostModel> items, Action<PostModel> listener) : base(itemView)
        {
            Image = itemView.FindViewById<SquareImageView>(Resource.Id.gridImageView);

            itemView.Click += (sender, e) => listener(items[base.Position]);

        }
    }
}