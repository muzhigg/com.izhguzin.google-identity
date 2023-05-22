# Overview

The Google Identity Package provides a convenient way to integrate Google Identity and OAuth 2.0 authentication and authorization into your Unity projects. It allows you to easily authenticate users using their Google accounts and obtain access tokens for accessing Google APIs.

# Features

⋅⋅*Seamless integration with Google Identity and OAuth 2.0.
⋅⋅*Supports Standalone, Android and WebGL platforms.
⋅⋅*Simplifies the authorization process and token management.
⋅⋅*Customizable HTML response page after completing authorization.

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