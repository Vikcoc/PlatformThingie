async function handleCredentialResponse(response) {
    // Extract user information from the response
    tk = response.credential;
    var us = JSON.parse(atob(response.credential.split('.')[1]));
    //console.log(us);

    var res = await (await fetch("./about/token", {
        method: "POST",
        body: '"' + tk + '"',
        headers: {
            "Content-type": "application/json"
        }
    })).text();
    console.log(res);

    document.getElementsByTagName('h1')[0].textContent += ' ' + us.given_name;
    document.getElementsByClassName('g_id_signin')[0].remove();
}