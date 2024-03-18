tk = '';

async function handleCredentialResponse(response) {
    // Extract user information from the response
    
    var us = JSON.parse(atob(response.credential.split('.')[1]));
    console.log(us);

    tk = response.credential;

    var res = await (await fetch("./login/google", {
        method: "POST",
        body: '"' + response.credential + '"',
        headers: {
            "Content-Type": "application/json",
        }
    })).json();

    sessionStorage["accessToken"] = res.accessToken;
    localStorage["refreshToken"] = res.refreshToken;

    history.back();
}

async function someTest() {
    
    var res = await (await fetch("./login/test", {
        method: "GET",
        headers: {
            "Content-Type": "application/json",
            "Authorization": "Bearer " + sessionStorage["accessToken"]
        }
    })).text();
    console.log(res);
}