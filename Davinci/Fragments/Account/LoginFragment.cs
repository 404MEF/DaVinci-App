using System.Threading.Tasks;

using Android.OS;
using Android.Views;
using Android.Widget;

using Davinci.Api;

namespace Davinci.Fragments.Account
{
    class LoginFragment : BaseFragment
    {
        EditText usernameField, passwordField;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.LoginFragment, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            usernameField = view.FindViewById<EditText>(Resource.Id.Login_usernameField);
            passwordField = view.FindViewById<EditText>(Resource.Id.Login_passwordField);

            Button loginButton = view.FindViewById<Button>(Resource.Id.Login_loginButton);
            Button registerBtn = view.FindViewById<Button>(Resource.Id.Login_registerButton);
            Button resetBtn = view.FindViewById<Button>(Resource.Id.Login_forgetButton);


            loginButton.Click += (s, e) => Authenticate();
            registerBtn.Click += (s, e) => Register();
            resetBtn.Click += (s, e) => ResetPassword();
        }

        private async void Authenticate()
        {
            bool isValid = validateInput();

            if (!isValid)
                return;

            //parentActivity.Window.AddFlags(WindowManagerFlags.NotTouchable);

            var response = await DavinciApi.Authenticate(usernameField.Text, passwordField.Text);

            if (response.isOK)
            {
                //Switch to main feed
                Infobar.Show(this.Context, this.View, response.message, Infobar.InfoLevel.Info, GravityFlags.Top | GravityFlags.FillHorizontal);
            }
            else
            {
                //parentActivity.RunOnUiThread(() => parentActivity.Window.ClearFlags(WindowManagerFlags.NotTouchable));
                Infobar.Show(this.Context, this.View, response.message, Infobar.InfoLevel.Error, GravityFlags.Top | GravityFlags.FillHorizontal);
            }
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

        private void Register()
        {
            parentActivity.ShowFragment(new RegisterFragment());
        }

        private void ResetPassword()
        {
            parentActivity.ShowFragment(new ResetPasswordFragment());

        }
    }
}