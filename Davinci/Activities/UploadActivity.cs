using System;
using System.Threading.Tasks;
using System.IO;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using Android.Util;
using Android.Graphics.Drawables;

using Xamarin.Media;

using Com.Bumptech.Glide;
using Com.dmytrodanylyk;

namespace Davinci.Activities
{
    [Activity(Theme = "@style/DavinciTheme.Feed", WindowSoftInputMode = SoftInput.AdjustPan, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    class UploadActivity : ToolbarActivity
    {
        Button takeBtn, chooseBtn, clearBtn;
        ActionProcessButton uploadBtn;
        EditText categoryField;
        ImageView imageView;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Upload);
            SetActionBar("Upload new image");

            setUI();
            setEvents();

            noImageUI();
        }

        private void setUI()
        {
            takeBtn = FindViewById<Button>(Resource.Id.Upload_takeBtn);
            chooseBtn = FindViewById<Button>(Resource.Id.Upload_chooseBtn);
            clearBtn = FindViewById<Button>(Resource.Id.Upload_clearBtn);
            uploadBtn = FindViewById<ActionProcessButton>(Resource.Id.Upload_uploadBtn);
            categoryField = FindViewById<EditText>(Resource.Id.Upload_categoryField);
            imageView = FindViewById<ImageView>(Resource.Id.Upload_imageView);
        }

        private void setEvents()
        {
            takeBtn.Click += (s, e) => takeWithCamera();
            chooseBtn.Click += (s, e) => chooseFromGallery();
            clearBtn.Click += (s, e) => onClearBtnClick();
            uploadBtn.Click += (s, e) => uploadImage();
        }

        private void imageUI()
        {
            takeBtn.Visibility = chooseBtn.Visibility = ViewStates.Gone;
            clearBtn.Visibility = imageView.Visibility = ViewStates.Visible;
        }

        private void noImageUI()
        {
            takeBtn.Visibility = chooseBtn.Visibility = ViewStates.Visible;
            clearBtn.Visibility = imageView.Visibility = ViewStates.Gone;
        }

        private void onClearBtnClick()
        {
            noImageUI();

            imageView.SetImageDrawable(null);

            GC.Collect();
        }

        private void takeWithCamera()
        {
            var picker = new MediaPicker(this);
            if (picker.IsCameraAvailable)
            {
                var intent = picker.GetTakePhotoUI(new StoreCameraMediaOptions
                {
                    Name = string.Format("photo-{0}.jpg", Guid.NewGuid()),
                    Directory = "Davinci"
                });
                StartActivityForResult(intent, 99);
            }
        }

        private void chooseFromGallery()
        {
            var picker = new MediaPicker(this);

            var intent = picker.GetPickPhotoUI();

            StartActivityForResult(intent, 99);
        }

        private void uploadImage()
        {
            var categoryText = categoryField.Text.Trim();

            if (imageView.Drawable == null)
            {
                Infobar.Show(this, "No image to upload", Infobar.InfoLevel.Info, GravityFlags.Top | GravityFlags.FillHorizontal);
                return;
            }
            else if (string.IsNullOrEmpty(categoryText))
            {
                categoryField.RequestFocus();
                categoryField.SetError("No category specified", null);
                return;
            }

            uploadBtn.setMode(ActionProcessButton.Mode.ENDLESS);
            uploadBtn.setProgress(1);

            var bm = (imageView.Drawable as BitmapDrawable).Bitmap;

            Task.Run(async () =>
            {
                var base64Image = string.Empty;

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    await bm.CompressAsync(Bitmap.CompressFormat.Webp, 85, memoryStream);

                    base64Image = Base64.EncodeToString(memoryStream.ToArray(), Base64Flags.Default);
                }

                return await Api.DavinciApi.UploadPost(base64Image, categoryText);
            }).ContinueWith(async t =>
            {
                var response = t.Result;

                if (t.Status == TaskStatus.Canceled)
                {
                    uploadBtn.setProgress(-1);
                    return;
                }

                if (response.OK)
                {
                    uploadBtn.setProgress(100);
                    await Task.Delay(1000);
                    this.Finish();
                }

                uploadBtn.setProgress(0);

            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            if (resultCode != Result.Ok)
                return;
            var path = "";
            Task.Run(async () =>
            {
                var media = await data.GetMediaFileExtraAsync(this);
                path = media.Path;
            }).ContinueWith(t =>
            {
                Glide.With(this)
                .Load(path)
                .Into(imageView);

                imageUI();
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