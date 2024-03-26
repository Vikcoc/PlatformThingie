import { authenticatedFetch } from '/public/authenticated-fetch';
async function getMyInfo() {
    var res = await authenticatedFetch("./profile/info", {
        method: "GET",
        headers: {
            "Content-Type": "application/json",
        }
    });

    if (!res.ok)
        return;

    var payload = await res.json();

    payload.forEach((x) => {
        var sec = document.createElement("section");
        var inp = document.createElement("md-outlined-text-field");

        inp.label = x.authClaimName;
        inp.value = x.authClaimValue;
        sec.appendChild(inp);
        document.body.appendChild(sec);
    })
}

await getMyInfo();