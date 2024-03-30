import { authenticatedFetch } from '/public/authenticated-fetch';
async function getMyInfo() {

    console.log('yes');
    return;


    var res = await authenticatedFetch("./user/my-info", {
        method: "GET",
        headers: {
            "Content-Type": "application/json",
        }
    });

    if (!res.ok)
        return;

    var payload = await res.json();

    payload.forEach((x) => {
        sec = document.createElement("section");
        var inp = document.createElement("md-outlined-text-field");

        inp.label = x.AuthClaim;
        inp.value = x.AuthClaimValue;
        sec.appendChild(inp);
        document.body.appendChild(sec);
    })
}

await getMyInfo();