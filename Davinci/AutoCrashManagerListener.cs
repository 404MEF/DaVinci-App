using HockeyApp.Android;

namespace HockeyApp.Android
{
    class AutoCrashManagerListener : CrashManagerListener
    {
        public override bool ShouldAutoUploadCrashes()
        {
            return true;
        }
    }
}