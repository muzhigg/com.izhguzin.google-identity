const library = { 

	$googleIdentityService: {
	
		isInitialized: false,
		
		isInitializeCalled: false,
		
		client: undefined,
		
		googleIdentityServiceInitializeScript: function (onerrorcallback, onloadcallback) {
			if (googleIdentityService.isInitializeCalled) {
				return;
			}

			googleIdentityService.isInitializeCalled = true;
			const clientScript = document.createElement('script');
			clientScript.onerror = (err) => {
				onerrorcallback('Failed to load Google 3P Authorization Javascript library');
			}
			clientScript.onload = () => {
				onloadcallback();
			}

			document.body.appendChild(clientScript);
			clientScript.src = 'https://accounts.google.com/gsi/client';
		},

		googleIdentityServiceInitializeCodeClient: function (initCallbackPtr, clientIdStr, scopeStr, callbackPtr, errorCallbackPtr) {
			googleIdentityService.googleIdentityServiceInitializeScript((err) => {
				dynCall('vi', initCallbackPtr, [googleIdentityService.allocateUnmanagedString(err)]);
			}, () => {
				try {
                    googleIdentityService.client = google.accounts.oauth2.initCodeClient({
                        client_id: clientIdStr,
                        scope: scopeStr,
                        ux_mode: 'popup',
                        callback: (response) => {
							if (response.error) {
								dynCall('vi', errorCallbackPtr, [googleIdentityService.allocateUnmanagedString(response.error)]);
							} 
							else if (response.code) {
								dynCall('vi', callbackPtr, [googleIdentityService.allocateUnmanagedString(response.code)]);
							}

							
						},
                        error_callback: (error) => {
							dynCall('vi', errorCallbackPtr, [googleIdentityService.allocateUnmanagedString(error.type)]);
						},
                    });
    
                    googleIdentityService.isInitialized = true;
                    dynCall('vi', initCallbackPtr, [googleIdentityService.allocateUnmanagedString('')]);
                } catch (error) {
                    dynCall('vi', initCallbackPtr, [googleIdentityService.allocateUnmanagedString('[WEB GIS ERROR] ' + error.message)]);
                }
            });
		},

		googleIdentityServiceRequestCode: function () {
			googleIdentityService.client.requestCode();
		},

		allocateUnmanagedString: function (string) {
			const stringBufferSize = lengthBytesUTF8(string) + 1;
			const stringBufferPtr = _malloc(stringBufferSize);
			stringToUTF8(string, stringBufferPtr, stringBufferSize);
			return stringBufferPtr;
		},
	},

	InitializeGisCodeClient: function (initCallbackPtr, clientIdStr, scopeStr, callbackPtr, errorCallbackPtr) {
		googleIdentityService.googleIdentityServiceInitializeCodeClient(initCallbackPtr, UTF8ToString(clientIdStr), UTF8ToString(scopeStr), callbackPtr, errorCallbackPtr);
	},

	AuthorizeGIS: function () {
		googleIdentityService.googleIdentityServiceRequestCode();
	},
	
	GetURLFromPage: function () {

        var returnStr = "not found";

        try{

            returnStr = (window.location != window.parent.location)

            ? document.referrer : document.location.href;

        } catch (error) {

            console.error('Error while getting Url: '+ error);

        }

        var bufferSize = lengthBytesUTF8(returnStr) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(returnStr, buffer, bufferSize);
        return buffer;
    }
}

autoAddDeps(library, '$googleIdentityService');
mergeInto(LibraryManager.library, library);