using System.Threading.Tasks;

using Android.OS;
using Android.Views;
using Android.Widget;

using Davinci.Api.Models;

namespace Davinci.Fragments.Account
{
    class LoginFragment : BaseFragment
    {
        EditText usernameField, passwordField;
        Button loginButton;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.LoginFragment, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            usernameField = view.FindViewById<EditText>(Resource.Id.Login_usernameField);
            passwordField = view.FindViewById<EditText>(Resource.Id.Login_passwordField);

            loginButton = view.FindViewById<Button>(Resource.Id.Login_loginButton);
            Button registerBtn = view.FindViewById<Button>(Resource.Id.Login_registerButton);
            Button resetBtn = view.FindViewById<Button>(Resource.Id.Login_forgetButton);


            loginButton.Click += (s, e) => Authenticate();
            registerBtn.Click += (s, e) => Register();
            resetBtn.Click += (s, e) => ResetPassword();
        }

        private void Authenticate()
        {
            bool isValid = ValidateLoginInput();

            if (!isValid)
                return;

            loginButton.Text = "Authenticating...";

            mActivity.Window.AddFlags(WindowManagerFlags.NotTouchable);
            //AuthenticationModel response = Task.Run(() => DavinciApi.Authenticate(usernameField.Text, passwordField.Text)).Result;
            AuthenticationModel response = new AuthenticationModel();
            response.result = "ok1";

            if (response.result == "ok")
            {
                //Switch activity
            }
            else
            {
                loginButton.Text = "Authentication failed";
                Task.Run(async () =>
                {
                    await Task.Delay(2000);
                    mActivity.RunOnUiThread(() => mActivity.Window.ClearFlags(WindowManagerFlags.NotTouchable));
                    mActivity.RunOnUiThread(() => loginButton.Text = "Login");
                });
            }
        }

        private bool ValidateLoginInput()
        {
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
            mActivity.ShowFragment(new RegisterFragment());
        }

        private void ResetPassword()
        {
            mActivity.ShowFragment(new ResetPasswordFragment());

        }
    }
}