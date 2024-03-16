function handleCredentialResponse(response) {
    // Extract user information from the response
    
    var us = JSON.parse(atob(response.credential.split('.')[1]));
    console.log(us);
}