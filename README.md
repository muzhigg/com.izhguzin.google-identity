# Overview

The Google Identity Package provides a convenient way to integrate Google Identity and OAuth 2.0 authentication and authorization into your Unity projects. It allows you to easily authenticate users using their Google accounts and obtain access tokens for accessing Google APIs.

# Features

*Seamless integration with Google Identity and OAuth 2.0.
*Supports Standalone, Android and WebGL platforms.
*Simplifies the authorization process and token management.
*Customizable HTML response page after completing authorization.

# Installation

To use the Google Identity Package in your Unity project, follow these steps:

1. Install the package from the Git URL: https://github.com/muzhigg/com.izhguzin.google-identity.git
2. Create a new project in the Google Cloud console. https://console.cloud.google.com/
3. Go to the [OAuth consent screen](https://console.cloud.google.com/apis/credentials/consent) and configure it as you want. __Note:__ If you do not plan to change your publication status to "in production" right away, specify your email in the Test users section
4. Open the [Credentials page](https://console.cloud.google.com/). Click Create credentials > OAuth client ID. Select the web application type and click create.
5. Add the required configuration to your project by following the instructions provided in the Usage section below.

# Usage

Before using the package, you also need to prepare a project. Below are the steps to set up the package for each platform.

## Standalone

The authorization flow on the standalone platform listens on the local web server. Therefore, before initializing the GoogleIdentityService, set the listening ports.

```csharp
// Example code for initializing the Google Identity Service
using Izhguzin.GoogleIdentity;
using UnityEngine;

public class ExampleScript : MonoBehaviour
{
    private async void Start()
    {
        GoogleAuthOptions.Builder optionsBuilder = new GoogleAuthOptions.Builder();
        optionsBuilder.SetCredentials("your-client-id", "your-client-secret")
            // Set only those scopes that you selected on the OAuth consent screen
            .SetScopes(Scopes.OpenId, Scopes.Email, Scopes.Profile)
            // The first free port will be selected for listening
            .SetListeningTcpPorts(new[] { 5000, 5001, 5002, 5003, 5004 });

        try
        {
            await GoogleIdentityService.InitializeAsync(optionsBuilder.Build());
        }
        catch (InitializationException exception)
        {
            Debug.LogError($"Failed to initialize Google Identity Service: {exception.Message}");
        }
    }
}
```

Then open the Credentials page in the Google Cloud console. And open your OAuth 2.0 Client ID for editing. In the "Authorized redirect URIs" section, specify your URIs in the format "http://127.0.0.1:{your-port}/"

## Android

Requirements:
A compatible Android device that runs Android 4.4 or newer and includes the Google Play Store or an emulator with an AVD that runs the Google APIs platform based on Android 4.2.2 or newer and has Google Play services version 15.0.0 or newer.

The package uses the Google Sign In library on Android devices. To use the Google Identity package, you need to follow these steps:
 
1. Sign your application with your key in the project settings.
2. In the project settings, select "Custom Main Manifest", "Custom Main Gradle Template", and "Custom Gradle Properties Template".
3. __Important:__ Also, in the "Other Settings" section, override the default package name.
4. Download and import the External Dependency Manager for Unity plugin into your project. https://github.com/googlesamples/unity-jar-resolver/tags
5. Resolve Android dependencies (Assets -> External Dependency Manager -> Android Resolver -> Resolve). 
6. You also need to edit AndroidManifest.xml. If you do not plan to use your own activity, replace the following lines.
```
<activity android:name="com.unity3d.player.UnityPlayerActivity"
                  android:theme="@style/UnityThemeSelector">
            <intent-filter>
                <action android:name="android.intent.action.MAIN" />
                <category android:name="android.intent.category.LAUNCHER" />
            </intent-filter>
            <meta-data android:name="unityplayer.UnityActivity" android:value="true" />
        </activity>
```

```
<activity android:name="com.izhguzin.gsi.GsiAppCompatActivity"
                  android:theme="@style/UnityAppCompatThemeSelector">
            <intent-filter>
                <action android:name="android.intent.action.MAIN" />
                <category android:name="android.intent.category.LAUNCHER" />
            </intent-filter>
            <meta-data android:name="unityplayer.UnityActivity" android:value="true" />
        </activity>
```
8. Open the Credentials page in the Google Cloud console and create a new OAuth client ID with the Android application type. In the Package Name field, enter the name from step 3. Also, enter the SHA1 fingerprint of your key. __Note__: You will not need the Android Client ID, but it must be created.
9. Now you can initialize the GoogleIdentityService:
```csharp
// Example code for initializing the Google Identity Service
using Izhguzin.GoogleIdentity;
using UnityEngine;

public class ExampleScript : MonoBehaviour
{
    private async void Start()
    {
        GoogleAuthOptions.Builder optionsBuilder = new();
        optionsBuilder.SetCredentials("your-client-id", "your-client-secret")
            // Set only those scopes that you selected on the OAuth consent screen
            .SetScopes(Scopes.OpenId, Scopes.Email, Scopes.Profile);

        try
        {
            await GoogleIdentityService.InitializeAsync(optionsBuilder.Build());
        }
        catch (InitializationException exception)
        {
            Debug.LogError($"Failed to initialize Google Identity Service: {exception.Message}");
        }
    }
}
```