function handleCredentialResponse(response) {
    // Extract user information from the response
    
    var us = JSON.parse(atob(response.credential.split('.')[1]));
    console.log(us);

    document.getElementsByTagName('h1')[0].textContent += ' ' + us.given_name;
    document.getElementsByClassName('g_id_signin')[0].remove();
}