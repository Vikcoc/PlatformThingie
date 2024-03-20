function handleCredentialResponse(response) {
    // Extract user information from the response
    
    var us = JSON.parse(atob(response.credential.split('.')[1]));
    console.log(us);

    document.getElementsByTagName('h1')[0].textContent += ' ' + us.given_name;
    document.getElementsByClassName('g_id_signin')[0].remove();
}


function getStuff() {
    tokenClient = google.accounts.oauth2.initTokenClient({
        client_id: "735272745075-c17fa1lkpsv22cafukoae9f9voqbfmcg.apps.googleusercontent.com",
        scope: "profile email",
        prompt: "select_account", // '' | 'none' | 'consent' | 'select_account'
        callback: handleCredentialResponse // your function to handle the response after login. 'access_token' will be returned as property on the response
    });

    tokenClient.requestAccessToken();
    //google.accounts.oauth2.TokenClient.requestAccessToken();
}