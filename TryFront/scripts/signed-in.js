function topBarIsSignedIn() {
    if (sessionStorage["accessToken"]
        && new Date(JSON.parse(atob(sessionStorage["accessToken"].split('.')[1])).exp * 1000) > Date.now()) {
        document.getElementById("login-button").style.display = "none";
        document.getElementById("profile-button").style.display = "inline-flex";
    }
}

topBarIsSignedIn();