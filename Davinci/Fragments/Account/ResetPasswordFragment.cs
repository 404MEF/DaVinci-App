using System.Threading.Tasks;

using Android.OS;
using Android.Views;
using Android.Widget;

using Davinci.Api.Models;

namespace Davinci.Fragments.Account
{
    class ResetPasswordFragment : BaseFragment
    {
        EditText usernameField;
        Button resetButton;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.Account_ResetPasswordFragment, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            setUI();
            setEvents();
        }

        private void setUI()
        {
            usernameField = View.FindViewById<EditText>(Resource.Id.Reset_usernameField);
            resetButton = View.FindViewById<Button>(Resource.Id.Reset_resetButton);
        }

        private void setEvents()
        {
            resetButton.Click += (s, e) => resetPassword();
        }

        private void resetPassword()
        {
            bool isValid = validateInput();

            if (!isValid)
                return;

            parentActivity.Window.AddFlags(WindowManagerFlags.NotTouchable);
            //AuthenticationModel response = Task.Run(() => DavinciApi.Authenticate(usernameField.Text, passwordField.Text)).Result;

            if (false)
            {
                //Switch activity
            }
            else
            {
                resetButton.Text = "Password reset failed";
                parentActivity.Window.ClearFlags(WindowManagerFlags.NotTouchable);
                Task.Run(async () =>
                {
                    await Task.Delay(2000);
                    resetButton.Text = "Reset my password";
                });
            }
        }

        private bool validateInput()
        {
            if (string.IsNullOrEmpty(usernameField.Text))
            {
                usernameField.RequestFocus();
                usernameField.SetError("Username cannot be empty", null);
                return false;
            }

            return true;
        }
    }
}