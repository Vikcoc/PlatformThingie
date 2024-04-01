async function handleCredentialResponse(response) {
    // Extract user information from the response
    
    //var us = JSON.parse(atob(response.credential.split('.')[1]));
    //console.log(us);

    var res = await fetch("./login/google", {
        method: "POST",
        body: '"' + response.credential + '"',
        headers: {
            "Content-Type": "application/json",
        }
    });
    
    if (!res.ok)
        return;

    var payload = await res.json();

    sessionStorage["accessToken"] = payload.accessToken;
    sessionStorage["refreshToken"] = payload.refreshToken;

    var a = document.createElement('a');
    a.href = document.referrer;
    a.hostname;

    if (document.referrer != "" && a.hostname == location.hostname)
        location.href = document.referrer
    else
        location.href = "./";
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