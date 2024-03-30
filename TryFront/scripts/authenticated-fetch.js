export async function authenticatedFetch(route, request) {

    if (sessionStorage["accessToken"]
        && new Date(JSON.parse(atob(sessionStorage["accessToken"].split('.')[1])).exp * 1000) > Date.now()) {
        request.headers["Authorization"] = "Bearer " + sessionStorage["accessToken"];
        return await fetch(route, request);
    }

    if (sessionStorage["refreshToken"]
        && new Date(JSON.parse(atob(sessionStorage["refreshToken"].split('.')[1])).exp * 1000) > Date.now()) {

        var res = await fetch("./login/refresh", {
            method: "POST",
            body: '"' + sessionStorage["refreshToken"] + '"',
            headers: {
                "Content-Type": "application/json",
            }
        });

        if (!res.unauthorised)
            location.href = './login';

        var payload = await res.json();

        sessionStorage["accessToken"] = payload.accessToken;
        sessionStorage["refreshToken"] = payload.refreshToken;
    
        request.headers["Authorization"] = "Bearer " + sessionStorage["accessToken"];
        return await fetch(route, request);
    }

    location.href = './login';
}