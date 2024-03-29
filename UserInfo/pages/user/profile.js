import { authenticatedFetch } from '/public/authenticated-fetch';

function getEditableField(claim) {
    var sec = document.createElement("section");
    sec.classList.add("horizontalLine");
    var sel = document.createElement("md-outlined-select");

    var opt = document.createElement("md-select-option");
    opt.textContent = "";
    sel.appendChild(opt);
    var opt = document.createElement("md-select-option");
    opt.textContent = "Yes";
    sel.appendChild(opt);
    var opt = document.createElement("md-select-option");
    opt.textContent = "No";
    sel.appendChild(opt);

    var inp = document.createElement("md-outlined-text-field");
    inp.label = claim.authClaimName;
    inp.value = claim.authClaimValue;

    var del = document.createElement("md-filled-tonal-icon-button");
    var img = document.createElement("img");
    img.src = "/public/trashcan-logo";
    img.alt = "Delete";
    del.appendChild(img);

    del.onclick = () => claimsContainer.removeChild(sec);

    sec.appendChild(sel);
    sec.appendChild(inp);
    sec.appendChild(del);

    return sec
}

function getNotEditableField(claim) {
    var sec = document.createElement("section");
    sec.classList.add("horizontalLine");

    var sel = document.createElement("h3");
    sel.textContent = claim.authClaimName + ":";
    var inp = document.createElement("p");
    inp.textContent = claim.authClaimValue;

    sec.appendChild(sel);
    sec.appendChild(inp);

    return sec;
}

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

    var claimsContainer = document.getElementById("claimsContainer");

    payload.forEach((x) => {
        
        if (x.authClaimRight == 0)
            claimsContainer.appendChild(getEditableField(x));
        else
            claimsContainer.appendChild(getNotEditableField(x));
    })
}

await getMyInfo();