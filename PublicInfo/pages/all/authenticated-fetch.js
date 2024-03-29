export async function authenticatedFetch(route, request) {

    if (sessionStorage["accessToken"]
        && new Date(JSON.parse(atob(sessionStorage["accessToken"].split('.')[1])).exp * 1000) > Date.now()) {
        request.headers["Authorization"] = "Bearer " + sessionStorage["accessToken"];
        return await fetch(route, request);
    }

    if (localStorage["refreshToken"]
        && new Date(JSON.parse(atob(localStorage["refreshToken"].split('.')[1])).exp * 1000) > Date.now()) {

        var res = await fetch("./login/refresh", {
            method: "POST",
            body: '"' + localStorage["refreshToken"] + '"',
            headers: {
                "Content-Type": "application/json",
            }
        });

        if (!res.ok)
            location.href = './login';

        var payload = await res.json();

        sessionStorage["accessToken"] = payload.accessToken;
        localStorage["refreshToken"] = payload.refreshToken;

        request.headers["Authorization"] = "Bearer " + sessionStorage["accessToken"];
        return await fetch(route, request);
    }

    location.href = './login';
}