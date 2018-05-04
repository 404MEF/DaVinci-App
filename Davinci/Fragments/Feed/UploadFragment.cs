using System.Threading.Tasks;

using Android.OS;
using Android.Views;
using Android.Widget;
using Xamarin.Media;
using Android.Content;
using Android.App;
using Android.Graphics;
using System.IO;
using Android.Graphics.Drawables;
using Android.Util;
using System;
using Davinci.Activities;

namespace Davinci.Fragments.Feed
{
    class UploadFragment : BaseFragment
    {
        Button takeBtn, chooseBtn, clearBtn, uploadBtn;
        EditText categoryField;
        ImageView imageView;
        TextView imageText;


        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.Upload, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            ((ToolbarActivity)parentActivity).SetTitle("Upload New Image");

            takeBtn = view.FindViewById<Button>(Resource.Id.Upload_takeBtn);
            chooseBtn = view.FindViewById<Button>(Resource.Id.Upload_chooseBtn);
            clearBtn = view.FindViewById<Button>(Resource.Id.Upload_clearBtn);
            uploadBtn = view.FindViewById<Button>(Resource.Id.Upload_uploadBtn);
            categoryField = view.FindViewById<EditText>(Resource.Id.Upload_catField);
            imageView = view.FindViewById<ImageView>(Resource.Id.Upload_imageView);
            imageText = view.FindViewById<TextView>(Resource.Id.Upload_imageText);

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
            var picker = new MediaPicker(this.Context);
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
            var picker = new MediaPicker(this.Context);

            var intent = picker.GetPickPhotoUI();

            StartActivityForResult(intent, 99);
        }

        private void uploadImage()
        {
            var categoryText = categoryField.Text;

            if (imageView.Drawable == null)
            {
                Infobar.Show(this.Context, "No image to upload", Infobar.InfoLevel.Info, GravityFlags.Top | GravityFlags.FillHorizontal);
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
                        RunOnUIThread(() => Infobar.Show(this.Context, response.message, Infobar.InfoLevel.Info, GravityFlags.Top | GravityFlags.FillHorizontal));
                        await Task.Delay(1000);
                        //RunOnUIThread(() => this.Finish());
                    }
                }
            });
        }

        public override void OnActivityResult(int requestCode, int resultCode, Intent data)
        {
            if (resultCode != (int)Result.Ok)
                return;

            Task.Run(async () =>
            {
                var mediaFile = await data.GetMediaFileExtraAsync(this.Context);

                using (var mediaStream = mediaFile.GetStream())
                {
                    var bitmap = await BitmapFactory.DecodeStreamAsync(mediaStream);
                    RunOnUIThread(() =>
                    {
                        imageView.SetImageBitmap(bitmap);
                        imageUploadedUI();
                    });
                }
            });

        }

    }
}