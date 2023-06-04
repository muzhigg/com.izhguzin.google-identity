# Google Identity for Unity

# Overview

The Google Identity Package provides a convenient way to integrate Google Identity and OAuth 2.0 authentication and authorization into your Unity projects. It allows you to easily authenticate users using their Google accounts and obtain access tokens for accessing Google APIs.

# Features

* Seamless integration with Google Identity and OAuth 2.0.  
* Supports Standalone, Android and WebGL platforms.  
* Simplifies the authorization process and token management.  
* Customizable HTML response page after completing authorization.  

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

## WebGL

On the WebGL platform, the package uses the Google 3P Authorization JavaScript library. Before you start working with it, you will also need to follow a few steps:

1. Open the Credentials page in the Google Cloud console and edit your web OAuth client ID. You need to specify authorized JavaScript origins. If you are testing your project on a local server, add the URI http://localhost. In other cases, specify the actual origin.
2. Now you can initialize the GoogleIdentityService:
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

## Authorization

To start the authorization flow, simply call the GoogleIdentityService.AuthorizeAsync() method.
```csharp
    public async Task AuthorizeAsync()
    {
        try
        {
            TokenResponse  response   = await GoogleIdentityService.Instance.AuthorizeAsync();
            UserCredential credential = response.GetUserCredential();

            Debug.LogWarning($"Hello {credential.GivenName}!");
        }
        catch (AuthorizationFailedException e)
        {
            Debug.LogException(e);
        }
    }
```

## Refresh Token and Revoke Access

The token you received is valid for one hour. After it expires, you can refresh it.
```csharp
    public async Task RefreshAsync(TokenResponse tokenResponse)
    {
        try
        {
            if (tokenResponse.IsEffectivelyExpired()) 
                await tokenResponse.RefreshTokenAsync();
        }
        catch (RequestFailedException exception)
        {
            Debug.LogException(exception);
        }
    }
```

You can also provide users with the ability to revoke access.
```csharp
    public async Task RevokeAccessAsync(TokenResponse tokenResponse)
    {
        try
        {
            await tokenResponse.RevokeAccessAsync();
        }
        catch (RequestFailedException exception)
        {
            Debug.LogException(exception);
        }
    }
```

## Token Storage

If you want to persist the token between sessions, you can use the ITokenStorage interface to implement token storage and retrieval.

> Why is this necessary? During the user's first authorization, the TokenResponse object will contain a RefreshToken that is needed to refresh the AccessToken. If you force the user to authorize again, Google will not provide you with a RefreshToken.

```csharp
	using System.Threading.Tasks;
using Izhguzin.GoogleIdentity;
using UnityEngine;

public class TokenStorageExample : ITokenStorage
{
    public Task<bool> SaveTokenAsync(string userId, string jsonToken)
    {
        // This is just an example.
        PlayerPrefs.SetString(userId, jsonToken);

        return Task.FromResult(true);
    }

    public Task<string> LoadTokenAsync(string userId)
    {
        // This is just an example.
        string jsonToken = PlayerPrefs.GetString(userId);

        return Task.FromResult(jsonToken);
    }
}
```

Complete the initialization by setting the SetTokenStorage option.

```csharp
    private async void Start()
    {
        GoogleAuthOptions.Builder optionsBuilder = new();
        optionsBuilder.SetCredentials("your-client-id", "your-client-secret")
            .SetScopes(Scopes.OpenId, Scopes.Email, Scopes.Profile)
            .SetTokenStorage(new TokenStorageExample());

        try
        {
            await GoogleIdentityService.InitializeAsync(optionsBuilder.Build());
        }
        catch (InitializationException exception)
        {
            Debug.LogError($"Failed to initialize Google Identity Service: {exception.Message}");
        }
    }
```

Now, after successful authorization, you can save your token.
```csharp
    public async Task AuthorizeAsync()
    {
        try
        {
            TokenResponse response = await GoogleIdentityService.Instance.AuthorizeAsync();

            // The method can return false if the save operation failed,
            // as well as if your TokenResponse does not contain a RefreshToken.
            bool successfullySaved = await response.StoreAsync("default");

            if (successfullySaved == false && string.IsNullOrEmpty(response.RefreshToken))
            {
                Debug.LogError("You have lost the RefreshToken!");
            }
        }
        catch (AuthorizationFailedException e)
        {
            Debug.LogException(e);
        }
    }
```

# Example with Unity Gaming Services

Below is an example of using Google Identity together with Unity Authentication and Unity Cloud Save packages.

```csharp
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Izhguzin.GoogleIdentity;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using Unity.Services.Core;
using UnityEngine;
using RequestFailedException = Izhguzin.GoogleIdentity.RequestFailedException;

public class ExampleScript : MonoBehaviour
{
    private TokenResponse _googleTokenResponse;
    public  bool          IsSignedInWithGoogle => IsPlayerLinkedWithGoogle();

    private async void Start()
    {
        await InitServicesAsync();

        try
        {
            await SignInAnonymouslyAsync();
        }
        catch (Exception)
        {
            Debug.LogError("Critical error, unable to log in anonymously.");
            throw;
        }
    }

    private async Task InitServicesAsync()
    {
        GoogleAuthOptions.Builder optionsBuilder = new();
        optionsBuilder.SetCredentials("your-client-id", "your-client-secret")
            .SetScopes(Scopes.OpenId, Scopes.Email, Scopes.Profile)
            .SetListeningTcpPorts(new[] { 5000, 5001, 5002, 5003, 5005 });

        try
        {
            await GoogleIdentityService.InitializeAsync(optionsBuilder.Build());
            await UnityServices.InitializeAsync();
        }
        catch (AuthorizationFailedException exception)
        {
            Debug.LogException(exception);
        }
        catch (ServicesInitializationException)
        {
            Debug.LogError("Critical error, unable to log in anonymously.");
            throw;
        }
    }

    private async Task SignInAnonymouslyAsync()
    {
        try
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
        catch (AuthenticationException exception) when (exception.ErrorCode ==
                                                        AuthenticationErrorCodes.InvalidSessionToken)
        {
            await SignInAnonymouslyAsync();
        }
    }

    private bool IsPlayerLinkedWithGoogle()
    {
        return string.IsNullOrEmpty(AuthenticationService.Instance.PlayerInfo.GetGoogleId());
    }

    public async Task SignInWithGoogleAsync()
    {
        if (IsSignedInWithGoogle) return;

        try
        {
            _googleTokenResponse = await GoogleIdentityService.Instance.AuthorizeAsync();
            await AuthenticationService.Instance.LinkWithGoogleAsync(_googleTokenResponse.IdToken);
        }
        // Google Identity Exception
        catch (AuthorizationFailedException exception)
        {
            Debug.LogException(exception);
        }
        // Unity Authentication Exception
        catch (AuthenticationException exception) when (exception.ErrorCode ==
                                                        AuthenticationErrorCodes.AccountAlreadyLinked)
        {
            AuthenticationService.Instance.SignOut(true);
            await AuthenticationService.Instance.SignInWithGoogleAsync(_googleTokenResponse.IdToken);
        }
    }
}
```

# FAQ

1. __I lost my RefreshToken.__  
Using a non-expired token, revoke access by calling the TokenResponse.RevokeAsync method, and then authorize again.

2. __How can I find out what specific error occurred during the request?__  
Each method, when an error occurs, will throw a RequestFailedException or AuthorizationFailedException. These classes contain an ErrorCode property, which can be compared to constants from the CommonErrorCodes and AndroidCommonErrorCodes classes. This way, you can determine whether the error is critical or if the user simply changed their mind about authorizing.
__Note__: CommonErrorCodes.SignInCanceled doesn't always mean that the user closed the authorization window. If you've configured your project incorrectly in the Google Cloud console, there may be an error instead of the agreement screen in the authorization window, and when the user closes it, you'll get a SignInCanceled error.

3. __Is this package only for user identification?__  
No. You can obtain access tokens for various Google APIs if you set the necessary scopes.

4. __I plan to use my own Android Activity.__  
You can use any activity, just add a dependency on the [com.izhguzin.gsi](https://github.com/muzhigg/android-gis-proxy-for-unity) library and initialize the GoogleSignInClientProxy. Keep in mind that your custom activity must inherit from AppCompatActivity.

# Known issues

+ When authorizing on the WebGL platform, instead of the consent screen, there may be an error "unauthorized JavaScript origin". This error is caused by the fact that Google accepts the origin along with the query parameters. Usually, trying to log in again helps.
