using System;

using Java.IO;
using Android.OS;
using Android.Views;
using Android.Support.V4.App;
using Android.Widget;
using Android.Content;
using Android.Provider;
using Android.Graphics;

using System.Threading.Tasks;
using Xamarin.Media;

namespace Davinci.Fragments.Dialog
{
    class UploadDialog : DialogFragment
    {
        public static class App
        {
            public static File _file;
            public static File _dir;
            public static Bitmap bitmap;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.ImageSourceDialog, null);
            return view;
        }
        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            var cameraBtn = view.FindViewById<Button>(Resource.Id.ImageSourceDialog_cancelBtn);
            var galleryBtn = view.FindViewById<Button>(Resource.Id.ImageSourceDialog_cancelBtn);
            var cancelBtn = view.FindViewById<Button>(Resource.Id.ImageSourceDialog_cancelBtn);

            cameraBtn.Click += (s, e) => Camera();
            galleryBtn.Click += (s, e) => Gallery();
            cancelBtn.Click += (s, e) => Dismiss();
        }
        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            Dialog.Window.RequestFeature(WindowFeatures.NoTitle);
            base.OnActivityCreated(savedInstanceState);
            Dialog.Window.Attributes.WindowAnimations = Resource.Style.Animation_AppCompat_Dialog;
            Dialog.Window.DecorView.SetBackgroundColor(Color.Transparent);
        }

        private void CreateDirectoryForPictures()
        {
            App._dir = new File(
                Android.OS.Environment.GetExternalStoragePublicDirectory(
                    Android.OS.Environment.DirectoryPictures), "Davinci");
            if (!App._dir.Exists())
            {
                App._dir.Mkdirs();
            }
        }

        private void Camera()
        {
            //CreateDirectoryForPictures();

            //Intent intent = new Intent(MediaStore.ActionImageCapture);
            //App._file = new File(App._dir, string.Format("photo-{0}.jpg", Guid.NewGuid()));
            //intent.PutExtra(MediaStore.ExtraOutput, Android.Net.Uri.FromFile(App._file));
            //StartActivityForResult(intent, (int)Request.Camera);

            //var picker = new MediaPicker(Context);
            //if (picker.IsCameraAvailable)
            //{
            //    var intent = picker.GetTakePhotoUI(new StoreCameraMediaOptions
            //    {
            //        Name = string.Format("photo-{0}.jpg", Guid.NewGuid()),
            //        Directory = "Davinci"
            //    });
            //    StartActivityForResult(intent, (int)Request.Camera);
            //}
            //this.Dismiss();
        }

        private void Gallery()
        {
            //var picker = new MediaPicker(Context);

            //var intent = picker.GetPickPhotoUI();

            //StartActivityForResult(intent, (int)Request.Gallery);
            //this.Dismiss();
            //var intent = new Intent(Intent.ActionPick, MediaStore.Images.Media.ExternalContentUri);
            //intent.SetType("image/*");
            //StartActivityForResult(Intent.CreateChooser(intent, "Select Image"), (int)Request.Gallery);
        }
    }
}