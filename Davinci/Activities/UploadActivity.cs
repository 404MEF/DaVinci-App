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
using Toolbar = Android.Support.V7.Widget.Toolbar;

using Xamarin.Media;

namespace Davinci.Activities
{
    [Activity(Theme = "@style/DavinciTheme.Feed", WindowSoftInputMode = SoftInput.AdjustPan)]
    class UploadActivity : BaseActivity
    {
        Button takeBtn, chooseBtn, clearBtn, uploadBtn;
        EditText categoryField;
        ImageView imageView;
        TextView imageText;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.UploadActivity);

            Toolbar toolBar = FindViewById<Toolbar>(Resource.Id.actionBar);
            toolBar.FindViewById<TextView>(Resource.Id.actionBar_title).Text = "Upload new image";
            SetSupportActionBar(toolBar);
            SupportActionBar.SetDisplayShowTitleEnabled(false);

            takeBtn = FindViewById<Button>(Resource.Id.Upload_takeBtn);
            chooseBtn = FindViewById<Button>(Resource.Id.Upload_chooseBtn);
            clearBtn = FindViewById<Button>(Resource.Id.Upload_clearBtn);
            uploadBtn = FindViewById<Button>(Resource.Id.Upload_uploadBtn);
            categoryField = FindViewById<EditText>(Resource.Id.Upload_catField);
            imageView = FindViewById<ImageView>(Resource.Id.Upload_imageView);
            imageText = FindViewById<TextView>(Resource.Id.Upload_imageText);

            imageClearUI();

            setEvents();
        }

        private void setEvents()
        {
            takeBtn.Click += (s, e) => takeWithCamera();
            chooseBtn.Click += (s, e) => chooseFromGallery();
            clearBtn.Click += (s, e) => onClearBtnClick();
            uploadBtn.Click += (s, e) => uploadImage();
        }

        private void imageUploadedUI()
        {
            takeBtn.Visibility = chooseBtn.Visibility = imageText.Visibility = ViewStates.Gone;
            clearBtn.Visibility = imageView.Visibility = ViewStates.Visible;
        }

        private void imageClearUI()
        {
            takeBtn.Visibility = chooseBtn.Visibility = imageText.Visibility = ViewStates.Visible;
            clearBtn.Visibility = imageView.Visibility = ViewStates.Gone;
        }

        private void onClearBtnClick()
        {
            imageClearUI();

            imageView.SetImageDrawable(null);
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
            var categoryText = categoryField.Text;

            if (imageView.Drawable == null)
            {
                Infobar.Show(this, "No image to upload", Infobar.InfoLevel.Info, GravityFlags.Top | GravityFlags.FillHorizontal);
                return;
            }
            var bitmap = (imageView.Drawable as BitmapDrawable).Bitmap;


            Task.Run(async () =>
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    await bitmap.CompressAsync(Bitmap.CompressFormat.Jpeg, 50, memoryStream);

                    var base64Image = Base64.EncodeToString(memoryStream.ToArray(), Base64Flags.Default);

                    var response = await Api.DavinciApi.UploadPost(base64Image, categoryText);

                    if (response.OK)
                    {
                        RunOnUiThread(() => Infobar.Show(this, response.message, Infobar.InfoLevel.Info, GravityFlags.Top | GravityFlags.FillHorizontal));
                        await Task.Delay(1000);
                        RunOnUiThread(() => this.Finish());
                    }
                }
            });
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            if (resultCode != Result.Ok)
                return;

            Task.Run(async () =>
            {
                var mediaFile = await data.GetMediaFileExtraAsync(this);

                using (var mediaStream = mediaFile.GetStream())
                {
                    var bitmap = await BitmapFactory.DecodeStreamAsync(mediaStream);

                    RunOnUiThread(() =>
                    {
                        imageView.SetImageBitmap(bitmap);
                        imageUploadedUI();
                    });
                }
            });
        }

    }
}