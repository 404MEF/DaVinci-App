using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Views;
using Android.Widget;
using System.Threading.Tasks;

namespace Davinci.Activities
{
    [Activity(Theme = "@style/DavinciTheme.Settings", WindowSoftInputMode = SoftInput.AdjustPan)]
    public class SettingsActivity : ToolbarActivity
    {
        Button changeEmailBtn, changeUsernameBtn, changePasswordBtn, deleteAccountBtn, logoutBtn;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Settings);
            SetActionBar("Settings");

            setUI();
            setEvents();
        }

        private void setUI()
        {
            changePasswordBtn = FindViewById<Button>(Resource.Id.changePasswordBtn);
            changeEmailBtn = FindViewById<Button>(Resource.Id.changeEmailBtn);
            changeUsernameBtn = FindViewById<Button>(Resource.Id.changeUsernameBtn);
            deleteAccountBtn = FindViewById<Button>(Resource.Id.deleteAccountBtn);
            logoutBtn = FindViewById<Button>(Resource.Id.Settings_logoutButton);
        }

        private void setEvents()
        {
            changePasswordBtn.Click += (s, e) => changePasswordAction();
            changeEmailBtn.Click += (s, e) => changeEmailAction();
            changeUsernameBtn.Click += (s, e) => changeUsernameAction();
            deleteAccountBtn.Click += (s, e) => deleteAccountAction();

            logoutBtn.Click += (s, e) => logout();
        }

        private void changePasswordAction()
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(this);
            builder.SetTitle("Change password");

            View viewInflated = LayoutInflater.From(this).Inflate(Resource.Layout.InputDialogPassword, (ViewGroup)changeEmailBtn.RootView, false);
            EditText input = viewInflated.FindViewById<EditText>(Resource.Id.input);
            builder.SetView(viewInflated);

            // Set up the buttons
            builder.SetPositiveButton("Confirm", (s, e) =>
            {
                Task.Run(async () =>
                {
                    return await Api.DavinciApi.ChangeAccount(null, input.Text, null);
                }).ContinueWith(t =>
                {
                    if (t.Result.OK)
                    {
                        Infobar.Show(this, t.Result.message, Infobar.InfoLevel.Info, GravityFlags.Top | GravityFlags.FillHorizontal);
                    }
                    else
                    {
                        Infobar.Show(this, t.Result.message, Infobar.InfoLevel.Error, GravityFlags.Top | GravityFlags.FillHorizontal);
                    }
                    ((AlertDialog)s).Dismiss();
                }, TaskScheduler.FromCurrentSynchronizationContext());
            });
            builder.SetNegativeButton("Cancel", (s, e) =>
            {
                ((AlertDialog)s).Dismiss();
            });

            builder.Show();
        }

        private void changeEmailAction()
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(this);
            builder.SetTitle("Change email");

            View viewInflated = LayoutInflater.From(this).Inflate(Resource.Layout.InputDialogEmail, (ViewGroup)changeEmailBtn.RootView, false);
            EditText input = viewInflated.FindViewById<EditText>(Resource.Id.input);
            builder.SetView(viewInflated);

            // Set up the buttons
            builder.SetPositiveButton("Confirm", (s, e) =>
            {
                Task.Run(async () =>
                {
                    return await Api.DavinciApi.ChangeAccount(null, null, input.Text);
                }).ContinueWith(t =>
                {
                    if (t.Result.OK)
                    {
                        Infobar.Show(this, t.Result.message, Infobar.InfoLevel.Info, GravityFlags.Top | GravityFlags.FillHorizontal);
                    }
                    else
                    {
                        Infobar.Show(this, t.Result.message, Infobar.InfoLevel.Error, GravityFlags.Top | GravityFlags.FillHorizontal);
                    }
                    ((AlertDialog)s).Dismiss();
                }, TaskScheduler.FromCurrentSynchronizationContext());
            });
            builder.SetNegativeButton("Cancel", (s, e) =>
            {
                ((AlertDialog)s).Dismiss();
            });

            builder.Show();
        }

        private void changeUsernameAction()
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(this);
            builder.SetTitle("Change username");

            View viewInflated = LayoutInflater.From(this).Inflate(Resource.Layout.InputDialogUsername, (ViewGroup)changeEmailBtn.RootView, false);
            EditText input = viewInflated.FindViewById<EditText>(Resource.Id.input);
            builder.SetView(viewInflated);

            // Set up the buttons
            builder.SetPositiveButton("Confirm", (s, e) =>
            {
                Task.Run(async () =>
                {
                    return await Api.DavinciApi.ChangeAccount(input.Text, null, null);
                }).ContinueWith(t =>
                {
                    if (t.Result.OK)
                    {
                        Infobar.Show(this, t.Result.message, Infobar.InfoLevel.Info, GravityFlags.Top | GravityFlags.FillHorizontal);
                    }
                    else
                    {
                        Infobar.Show(this, t.Result.message, Infobar.InfoLevel.Error, GravityFlags.Top | GravityFlags.FillHorizontal);
                    }
                    ((AlertDialog)s).Dismiss();
                }, TaskScheduler.FromCurrentSynchronizationContext());
            });
            builder.SetNegativeButton("Cancel", (s, e) =>
            {
                ((AlertDialog)s).Dismiss();
            });

            builder.Show();
        }

        private void deleteAccountAction()
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(this);
            builder.SetTitle("Delete your account");

            View viewInflated = LayoutInflater.From(this).Inflate(Resource.Layout.InputDialogDelete, (ViewGroup)changeEmailBtn.RootView, false);
            EditText input = viewInflated.FindViewById<EditText>(Resource.Id.input);
            builder.SetView(viewInflated);

            // Set up the buttons
            builder.SetPositiveButton("Confirm", (s, e) =>
            {
                Task.Run(async () =>
                {
                    return await Api.DavinciApi.DeleteAccount();
                }).ContinueWith(t =>
                {
                    ((AlertDialog)s).Dismiss();
                    logout();
                }, TaskScheduler.FromCurrentSynchronizationContext());
            });
            builder.SetNegativeButton("Cancel", (s, e) =>
            {
                ((AlertDialog)s).Dismiss();
            });
            builder.Show();
        }

        private void logout()
        {
            var prefsEdit = PreferenceManager.GetDefaultSharedPreferences(Application.Context).Edit();
            prefsEdit.Remove(GetString(Resource.String.userpassword));
            prefsEdit.Apply();
            prefsEdit.Dispose();

            Token.value = string.Empty;

            StartActivity(new Intent(Application.Context, typeof(AccountActivity)));
            this.Finish();
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