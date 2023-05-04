mergeInto(LibraryManager.library, {

	InitGsiClient: function(webClientId) {
		const sdkScript = document.createElement('script');
		sdkScript.src = 'https://apis.google.com/js/platform.js';
		sdkScript.async = true;
		sdkScript.defer = true;
		
		document.head.appendChild(sdkScript);
		
		sdkScript.onload = function () {
			gapi.load('auth2', function() {
				gapi.auth2.init({
					client_id: UTF8ToString(webClientId),
					fetch_basic_profile: true,
					ux_mode: 'popup',
				}).then(function() {
					console.log('Google Sign In ready to use');
				}, function() {
					console.log('Failed to init GSI');
				});
			});
		}
	},
	
	SignIn: function(onSuccess, onFailure) {
		gapi.auth2.getAuthInstance().signIn().then(googleUser => {
			var id_token = googleUser.getAuthResponse().id_token;
			var buffSize = lengthBytesUTF8(id_token) + 1;
			var buf = _malloc(buffSize);
			stringToUTF8(id_token, buf, buffSize);
			dynCall('vi', onSuccess, [buf]);
		}, reason => {
			var returnStr = reason;
			var bufferSize = lengthBytesUTF8(returnStr) + 1;
			var buffer = _malloc(bufferSize);
			stringToUTF8(returnStr, buffer, bufferSize);
			dynCall('vi', onFailure, [buffer]);
		});
	},

});