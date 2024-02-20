const jose = require('jose')
handler = '';
async function handleCredentialResponse(response) {
    // Extract user information from the response
    console.log(response.credential);
    console.log(response.profile);

    const JWKS = jose.createRemoteJWKSet(new URL('https://www.googleapis.com/oauth2/v3/certs'));
    handler = await jose.jwtVerify(jwt, JWKS)
}

//window.onload = function () {
//    google.accounts.id.initialize({
//        client_id: '735272745075-c17fa1lkpsv22cafukoae9f9voqbfmcg.apps.googleusercontent.com',
//        callback: handleCredentialResponse
//    });
//    google.accounts.id.prompt();
//};
