using System.Threading.Tasks;

using Android.OS;
using Android.Views;
using Android.Widget;

using Davinci.Api;
using Davinci.Api.Models;
using Davinci.Activities;

namespace Davinci.Fragments.Account
{
    class RegisterFragment : BaseFragment
    {
        EditText usernameField, emailField, passwordField, passwordConfirmField;
        Button registerBtn;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.Account_RegisterFragment, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            usernameField = view.FindViewById<EditText>(Resource.Id.Register_usernameField);
            emailField = view.FindViewById<EditText>(Resource.Id.Register_emailField);
            passwordField = view.FindViewById<EditText>(Resource.Id.Register_passwordField);
            passwordConfirmField = view.FindViewById<EditText>(Resource.Id.Register_passwordConfirmField);

            registerBtn = view.FindViewById<Button>(Resource.Id.Register_registerButton);

            registerBtn.Click += (s, e) => Register();
        }

        private void Register()
        {
            bool isValid = validateInput();

            if (!isValid)
                return;

            toggleUiInput();
            registerBtn.Text = "Registering...";

            Task.Run(async () =>
            {
                return await DavinciApi.Register(usernameField.Text, emailField.Text, passwordField.Text);
            }).ContinueWith(responseTask =>
            {
                var response = responseTask.Result;

                if (responseTask.Status == TaskStatus.Canceled)
                {
                    toggleUiInput();
                    registerBtn.Text = "Register";

                    Infobar.Show(this.Context, "Connection error", Infobar.InfoLevel.Info, GravityFlags.Top | GravityFlags.FillHorizontal, false);
                }
                else if (response.OK)
                {
                    Infobar.Show(this.Context, response.message, Infobar.InfoLevel.Info, GravityFlags.Top | GravityFlags.FillHorizontal,false);

                    ((AccountActivity)parentActivity).Login();
                }
                else
                {
                    toggleUiInput();
                    registerBtn.Text = "Register";

                    Infobar.Show(this.Context, response.message, Infobar.InfoLevel.Error, GravityFlags.Top | GravityFlags.FillHorizontal,false);
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void toggleUiInput()
        {
            var state = !registerBtn.Enabled;
            registerBtn.Enabled = state;
            usernameField.Enabled = state;
            emailField.Enabled = state;
            passwordField.Enabled = state;
            passwordConfirmField.Enabled = state;
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