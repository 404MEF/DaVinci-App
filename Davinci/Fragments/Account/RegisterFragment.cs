using System.Threading.Tasks;

using Android.OS;
using Android.Views;
using Android.Widget;

using Davinci.Api.Models;

namespace Davinci.Fragments.Account
{
    class RegisterFragment : BaseFragment
    {
        EditText usernameField,emailField, passwordField,passwordConfirmField;
        Button registerButton;
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

            registerButton = view.FindViewById<Button>(Resource.Id.Register_registerButton);

            registerButton.Click += (s, e) => Register();
        }

        private void Register()
        {
            bool isValid = ValidateInput();

            if (!isValid)
                return;

            registerButton.Text = "Registering...";

            mActivity.Window.AddFlags(WindowManagerFlags.NotTouchable);
            //AuthenticationModel response = Task.Run(() => DavinciApi.Authenticate(usernameField.Text, passwordField.Text)).Result;

            if (false)
            {
                //Switch activity
            }
            else
            {
                registerButton.Text = "Register failed";
                mActivity.Window.ClearFlags(WindowManagerFlags.NotTouchable);
                Task.Run(async () =>
                {
                    await Task.Delay(2000);
                    registerButton.Text = "Register";
                });
            }
        }

        private bool ValidateInput()
        {
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

            return true;
        }


    }
}