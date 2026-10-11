using System;
using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Avalonia.Android;
using CherylCrossTest;

namespace CherylCrossTest.Android;

// Avalonia 12: the activity is no longer generic. The Application class is
// registered separately through AvaloniaAndroidApplication<TApp>.
[Activity(Label = "CherylCrossTest.Android", Theme = "@style/MyTheme.NoActionBar", Icon = "@drawable/icon",
    MainLauncher = true, ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.UiMode)]
public class MainActivity : AvaloniaMainActivity
{
}

[Application]
public class AndroidApp : AvaloniaAndroidApplication<App>
{
    protected AndroidApp(IntPtr javaReference, JniHandleOwnership transfer)
        : base(javaReference, transfer)
    {
    }
}
