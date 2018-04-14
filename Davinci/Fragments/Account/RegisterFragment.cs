using System.Threading.Tasks;

using Android.OS;
using Android.Views;
using Android.Widget;

using Davinci.Api;
using Davinci.Api.Models;

namespace Davinci.Fragments.Account
{
    class RegisterFragment : BaseFragment
    {
        EditText usernameField, emailField, passwordField, passwordConfirmField;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.RegisterFragment, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            usernameField = view.FindViewById<EditText>(Resource.Id.Register_usernameField);
            emailField = view.FindViewById<EditText>(Resource.Id.Register_emailField);
            passwordField = view.FindViewById<EditText>(Resource.Id.Register_passwordField);
            passwordConfirmField = view.FindViewById<EditText>(Resource.Id.Register_passwordConfirmField);

            Button registerButton = view.FindViewById<Button>(Resource.Id.Register_registerButton);

            registerButton.Click += (s, e) => Register();
        }

        private async void Register()
        {
            bool isValid = validateInput();

            if (!isValid)
                return;

            //parentActivity.Window.AddFlags(WindowManagerFlags.NotTouchable);
            var response = await DavinciApi.Register(usernameField.Text, emailField.Text, passwordField.Text);

            if (response.isOK)
            {
                //Switch activity
                Infobar.Show(this.Context, this.View, response.message, Infobar.InfoLevel.Info, GravityFlags.Top | GravityFlags.FillHorizontal);
                await Task.Delay(1000);
                parentActivity.ShowFragment(new LoginFragment());
                parentActivity.ClearStack();
            }
            else
            {
                //parentActivity.RunOnUiThread(() => parentActivity.Window.ClearFlags(WindowManagerFlags.NotTouchable));
                Infobar.Show(this.Context, this.View, response.message, Infobar.InfoLevel.Warning, GravityFlags.Top | GravityFlags.FillHorizontal);

            }
        }

        private bool validateInput()
        {
            usernameField.Text = usernameField.Text.Trim();
            emailField.Text = emailField.Text.Trim();
            passwordField.Text = passwordField.Text.Trim();
            passwordConfirmField.Text = passwordConfirmField.Text.Trim();

            if (string.IsNullOrEmpty(usernameField.Text))
            {
                usernameField.RequestFocus();
                usernameField.SetError("Username cannot be empty", null);
                return false;
            }
            else if (string.IsNullOrEmpty(emailField.Text))
            {
                emailField.RequestFocus();
                emailField.SetError("Email cannot be empty", null);
                return false;
            }
            else if (string.IsNullOrEmpty(passwordField.Text))
            {
                passwordField.RequestFocus();
                passwordField.SetError("Password cannot be empty", null);
                return false;
            }
            else if (string.IsNullOrEmpty(passwordConfirmField.Text))
            {
                passwordConfirmField.RequestFocus();
                passwordConfirmField.SetError("Password cannot be empty", null);
                return false;
            }
            else if (passwordField.Text != passwordConfirmField.Text)
            {
                passwordConfirmField.RequestFocus();
                passwordConfirmField.SetError("Passwords do not match", null);
                return false;
            }

            return true;
        }


    }
}