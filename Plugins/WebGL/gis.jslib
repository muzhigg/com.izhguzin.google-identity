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

		googleIdentityServiceInitializeImplicitClient: function (initCallbackPtr, clientIdStr, scopeStr, callbackPtr, errorCallbackPtr) {
				
			googleIdentityService.googleIdentityServiceInitializeScript((err) => {
				dynCall('vi', initCallbackPtr, [googleIdentityService.allocateUnmanagedString(err)]);
			}, () => {
				try {
                    googleIdentityService.client = google.accounts.oauth2.initTokenClient({
                        client_id: UTF8ToString(clientIdStr),
                        scope: UTF8ToString(scopeStr),
                        callback: (response) => {
                            // dynCall('vi', callbackPtr, googleIdentityService.allocateUnmanagedString(response));
                        },
                        error_callback: (error) => {
                            // dynCall('vi', errorCallbackPtr, googleIdentityService.allocateUnmanagedString(error));
                        },
                    });
    
                    googleIdentityService.isInitialized = true;
                    dynCall('vi', initCallbackPtr, [googleIdentityService.allocateUnmanagedString('')]);
                } catch (error) {
                    dynCall('vi', initCallbackPtr, [googleIdentityService.allocateUnmanagedString('[WEB GIS ERROR] ' + error.message)]);
                }
            });
		},

		googleIdentityServiceInitializeCodeClient: function (initCallbackPtr, clientIdStr, scopeStr, callbackPtr, errorCallbackPtr) {
			googleIdentityService.googleIdentityServiceInitializeScript((err) => {
				dynCall('vi', initCallbackPtr, [googleIdentityService.allocateUnmanagedString(err)]);
			}, () => {
				try {
                    googleIdentityService.client = google.accounts.oauth2.initCodeClient({
                        client_id: UTF8ToString(clientIdStr),
                        scope: UTF8ToString(scopeStr),
                        ux_mode: 'popup',
                        callback: (response) => {},
                        error_callback: (error) => {},
                    });
    
                    googleIdentityService.isInitialized = true;
                    dynCall('vi', initCallbackPtr, [googleIdentityService.allocateUnmanagedString('')]);
                } catch (error) {
                    dynCall('vi', initCallbackPtr, [googleIdentityService.allocateUnmanagedString('[WEB GIS ERROR] ' + error.message)]);
                }
            });
		},

		allocateUnmanagedString: function (string) {
			const stringBufferSize = lengthBytesUTF8(string) + 1;
			const stringBufferPtr = _malloc(stringBufferSize);
			stringToUTF8(string, stringBufferPtr, stringBufferSize);
			return stringBufferPtr;
		},
	},

    InitializeGisImplicitClient: function (initCallbackPtr, clientIdStr, scopeStr, callbackPtr, errorCallbackPtr) {
        googleIdentityService.googleIdentityServiceInitializeImplicitClient(initCallbackPtr, clientIdStr, scopeStr, callbackPtr, errorCallbackPtr);
    },

	InitializeGisCodeClient: function (initCallbackPtr, clientIdStr, scopeStr, callbackPtr, errorCallbackPtr) {
		googleIdentityService.googleIdentityServiceInitializeCodeClient(initCallbackPtr, clientIdStr, scopeStr, callbackPtr, errorCallbackPtr);
	}
}

autoAddDeps(library, '$googleIdentityService');
mergeInto(LibraryManager.library, library);