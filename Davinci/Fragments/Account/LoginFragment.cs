using System.Threading.Tasks;

using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Preferences;
using Android.App;
using Android.Content;

using Davinci.Api;

namespace Davinci.Fragments.Account
{
    class LoginFragment : BaseFragment
    {
        EditText usernameField, passwordField;
        Button loginBtn, registerBtn, resetBtn;
        CheckBox rememberBox;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.Account_LoginFragment, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            usernameField = view.FindViewById<EditText>(Resource.Id.Login_usernameField);
            passwordField = view.FindViewById<EditText>(Resource.Id.Login_passwordField);
            rememberBox = view.FindViewById<CheckBox>(Resource.Id.Login_rememberChk);

            loginBtn = view.FindViewById<Button>(Resource.Id.Login_loginButton);
            registerBtn = view.FindViewById<Button>(Resource.Id.Login_registerButton);
            resetBtn = view.FindViewById<Button>(Resource.Id.Login_forgetButton);

            var prefs = PreferenceManager.GetDefaultSharedPreferences(Application.Context);
            var username = prefs.GetString(GetString(Resource.String.username), string.Empty);
            usernameField.Text = username;

            loginBtn.Click += (s, e) => Authenticate();
            registerBtn.Click += (s, e) => Register();
            resetBtn.Click += (s, e) => ResetPassword();
        }

        private void Authenticate()
        {
            bool isValid = validateInput();

            if (!isValid)
                return;

            toggleUiInput();
            loginBtn.Text = "Authenticating...";

            Task.Run(async () =>
            {
                return await DavinciApi.Authenticate(usernameField.Text, passwordField.Text);
            }).ContinueWith(async responseTask =>
            {
                var response = responseTask.Result;

                if (response.OK)
                {
                    saveCredentials(rememberBox.Checked);

                    toggleUiInput();

                    Infobar.Show(this.Context, response.message, Infobar.InfoLevel.Info, GravityFlags.Top | GravityFlags.FillHorizontal);

                    await Task.Delay(500);

                    Feed();
                }
                else
                {
                    toggleUiInput();
                    loginBtn.Text = "Login";

                    Infobar.Show(this.Context, response.message, Infobar.InfoLevel.Error, GravityFlags.Top | GravityFlags.FillHorizontal);
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void toggleUiInput()
        {
            var state = !loginBtn.Enabled;
            loginBtn.Enabled = state;
            registerBtn.Enabled = state;
            resetBtn.Enabled = state;
            usernameField.Enabled = state;
            passwordField.Enabled = state;
            rememberBox.Enabled = state;
        }

        private bool validateInput()
        {
            usernameField.Text = usernameField.Text.Trim();
            passwordField.Text = passwordField.Text.Trim();

            if (string.IsNullOrEmpty(usernameField.Text))
            {
                usernameField.RequestFocus();
                usernameField.SetError("Username cannot be empty", null);
                return false;
            }
            else if (string.IsNullOrEmpty(passwordField.Text))
            {
                passwordField.RequestFocus();
                passwordField.SetError("Password cannot be empty", null);
                return false;
            }

            return true;
        }

        private void saveCredentials(bool savePassword)
        {
            var prefs = PreferenceManager.GetDefaultSharedPreferences(Application.Context);
            var prefsEdit = prefs.Edit();
            prefsEdit.PutString(GetString(Resource.String.username), usernameField.Text);
            if (savePassword)
                prefsEdit.PutString(GetString(Resource.String.userpassword), passwordField.Text);

            prefsEdit.Apply();
        }

        private void Register()
        {
            parentActivity.ShowFragment(new RegisterFragment());
        }

        private void ResetPassword()
        {
            parentActivity.ShowFragment(new ResetPasswordFragment());

        }

        private void Feed()
        {
            StartActivity(new Intent(Application.Context, typeof(FeedActivity)));
            this.Close();
        }
    }
}