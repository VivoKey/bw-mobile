using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using System;

namespace Bit.Android
{
    [Activity(Theme = "@style/BitwardenTheme.Splash", WindowSoftInputMode = SoftInput.StateHidden)]
    public class AutofillActivity : Activity
    {
        private static TimeSpan _retrySpan = TimeSpan.FromSeconds(2);

        private DateTime? _lastLaunch = null;
        private string _lastQueriedUri;

        public static AutofillCredentials LastCredentials { get; set; }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            LaunchMainActivity(Intent, 932473);
        }

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);
            LaunchMainActivity(intent, 489729);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        protected override void OnResume()
        {
            base.OnResume();
            if(!Intent.HasExtra("uri"))
            {
                Finish();
                return;
            }

            Intent.RemoveExtra("uri");
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if(data == null)
            {
                LastCredentials = null;
            }
            else
            {
                try
                {
                    if(data.GetStringExtra("canceled") != null)
                    {
                        LastCredentials = null;
                    }
                    else
                    {
                        var uri = data.GetStringExtra("uri");
                        var username = data.GetStringExtra("username");
                        var password = data.GetStringExtra("password");

                        LastCredentials = new AutofillCredentials
                        {
                            Username = username,
                            Password = password,
                            Uri = uri,
                            LastUri = _lastQueriedUri
                        };
                    }
                }
                catch
                {
                    LastCredentials = null;
                }
            }

            Finish();
        }

        private void LaunchMainActivity(Intent callingIntent, int requestCode)
        {
            var now = DateTime.UtcNow;
            if(_lastLaunch.HasValue && (now - _lastLaunch.Value <= _retrySpan))
            {
                return;
            }

            _lastLaunch = now;
            _lastQueriedUri = callingIntent?.GetStringExtra("uri");
            if(_lastQueriedUri == null)
            {
                Finish();
                return;
            }

            var intent = new Intent(this, typeof(MainActivity));
            if(!callingIntent.Flags.HasFlag(ActivityFlags.LaunchedFromHistory))
            {
                intent.PutExtra("uri", _lastQueriedUri);
            }
            StartActivityForResult(intent, requestCode);
        }
    }
}
