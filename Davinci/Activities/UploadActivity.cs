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

namespace Davinci.Activities
{
    [Activity(Theme = "@style/DavinciTheme.Feed", WindowSoftInputMode = SoftInput.AdjustPan, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    class UploadActivity : ToolbarActivity
    {
        Button takeBtn, chooseBtn, clearBtn, uploadBtn;
        EditText categoryField;
        ImageView imageView;
        TextView imageText;

        Bitmap bitmap;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Upload);

            SetActionBar("Upload");

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
                    Directory = "Davinci",
                });
                StartActivityForResult(intent, 1);
            }
        }

        private void chooseFromGallery()
        {
            var picker = new MediaPicker(this);

            var intent = picker.GetPickPhotoUI();

            StartActivityForResult(intent, 1);
        }

        private void uploadImage()
        {
            var categoryText = categoryField.Text;

            if (imageView.Drawable == null)
            {
                Infobar.Show(this, "No image to upload", Infobar.InfoLevel.Info, GravityFlags.Top | GravityFlags.FillHorizontal);
                return;
            }
            var bm = (imageView.Drawable as BitmapDrawable).Bitmap;


            Task.Run(async () =>
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    await bm.CompressAsync(Bitmap.CompressFormat.Jpeg, 80, memoryStream);

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
                    BitmapFactory.Options options = new BitmapFactory.Options();
                    options.InJustDecodeBounds = true;

                    int height = options.OutHeight;
                    int width = options.OutWidth;

                    int sampleSize = 1;
                    if (height > 256 || width > 256)
                    {
                        int heightRatio = (int)Math.Round((float)height / (float)256);
                        int widthRatio = (int)Math.Round((float)width / (float)256);
                        sampleSize = Math.Min(heightRatio, widthRatio);
                    }

                    options = new BitmapFactory.Options();
                    options.InSampleSize = sampleSize;

                    var bitmap = await BitmapFactory.DecodeStreamAsync(mediaStream, null, options);

                    RunOnUiThread(() =>
                    {
                        imageUploadedUI();

                        imageView.SetImageBitmap(null);
                        if (this.bitmap != null)
                            this.bitmap.Dispose();
                        this.bitmap = null;
                        this.bitmap = bitmap;

                        imageView.SetImageBitmap(bitmap);
                    });
                }
            });
        }

    }
}