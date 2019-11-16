var mainApp = {};

(function(){
	var firebase = app_firebase;
	var uid = null;
	firebase.auth().onAuthStateChanged(function(user) {
	  if (user) {
		  if(user.email != "administrator@admin.com")
			  window.location.replace("login.html")
		// User is signed in.
		uid = user.email;
		console.log(uid);
	  }else{
		  uid = null;
		  window.location.replace("login.html")
	  }
	});
	
	function logOut(){
		firebase.auth().signOut();
	}
	
	mainApp.logOut = logOut;
})()

