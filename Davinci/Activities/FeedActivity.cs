using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using Davinci.Fragments.Dialog;
using Davinci.Fragments.Feed;
using Davinci.Enums.Feed;
using System.Threading.Tasks;
using Xamarin.Media;
using System;
using System.IO;
using Android.Util;
using Android.Graphics;
using Java.IO;

namespace Davinci
{
    [Activity(Theme = "@style/DavinciTheme.Feed", WindowSoftInputMode = SoftInput.AdjustPan, NoHistory = false)]
    public class FeedActivity : BaseActivity
    {
        UploadDialog uploadDialog;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.FeedActivity);


            SetSupportActionBar(FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.actionBar));
            SupportActionBar.SetDisplayShowTitleEnabled(false);

            ShowFragment(new FeedFragment());

            ImageButton settingsButton = FindViewById<ImageButton>(Resource.Id.Feed_settingsButton);
            ImageButton uploadButton = FindViewById<ImageButton>(Resource.Id.Feed_uploadButton);

            settingsButton.Click += (s, e) => Settings();
            uploadButton.Click += (s, e) => Upload();
            uploadButton.LongClick += (s, e) => UploadFromGallery();
        }
        private void Settings()
        {
            StartActivity(new Intent(Application.Context, typeof(SettingsActivity)));
        }
        private void Upload()
        {
            //if (uploadDialog == null)
            //    uploadDialog = new UploadDialog();

            //var trans = SupportFragmentManager.BeginTransaction();
            //uploadDialog.Show(SupportFragmentManager, "dialog_fragment");

            var picker = new MediaPicker(this);
            if (picker.IsCameraAvailable)
            {
                var intent = picker.GetTakePhotoUI(new StoreCameraMediaOptions
                {
                    Name = string.Format("photo-{0}.jpg", Guid.NewGuid()),
                    Directory = "Davinci"
                });
                StartActivityForResult(intent, (int)Request.Camera);
            }
        }

        private void UploadFromGallery()
        {
            var picker = new MediaPicker(this);

            var intent = picker.GetPickPhotoUI();

            StartActivityForResult(intent, (int)Request.Gallery);
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            if (resultCode != Result.Ok)
                return;

            Task.Run(async () =>
            {
                var image = await data.GetMediaFileExtraAsync(this);

                byte[] z;

                using (MemoryStream ms = new MemoryStream())
                {
                    using (var cs = image.GetStream())
                    {
                        cs.CopyTo(ms);
                        z = ms.ToArray();
                    }
                }

                var bm = BitmapFactory.DecodeByteArray(z, 0, z.Length);
                MemoryStream mss = new MemoryStream();
                bm.Compress(Bitmap.CompressFormat.Jpeg, 70, mss);
                z = mss.ToArray();
                var imageData = Base64.EncodeToString(z,Base64Flags.Default);
                var response = await Api.DavinciApi.UploadPost(imageData, "Tech");

                if (response.OK)
                    RunOnUiThread(() => Infobar.Show(this, response.message, Infobar.InfoLevel.Info, GravityFlags.Top | GravityFlags.FillHorizontal));
            });
        }

    }
}