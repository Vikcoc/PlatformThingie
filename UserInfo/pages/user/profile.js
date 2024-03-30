import { authenticatedFetch } from '/public/authenticated-fetch';

function getSelectableField(availableClaims, selected) {
    var sel = document.createElement("md-outlined-select");
    availableClaims.forEach((x) => {
        var opt = document.createElement("md-select-option");
        opt.value = x;
        opt.textContent = x;
        if (x == selected)
            opt.selected = true;
        sel.appendChild(opt);
    })
    return sel;
}

function getEditableField(claim, availableClaims) {
    var sec = document.createElement("section");
    sec.classList.add("horizontalLine");

    var sel = getSelectableField(availableClaims, claim.authClaimName);
        
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
    var res = await authenticatedFetch("/profile/info", {
        method: "GET",
        headers: {
            "Content-Type": "application/json",
        }
    });

    if (!res.ok)
        return;

    var availableClaimsRes = await authenticatedFetch("/profile/claims", {
        method: "GET",
        headers: {
            "Content-Type": "application/json",
        }
    });

    var payload = await res.json();
    var availableClaims = await availableClaimsRes.json();

    var claimsContainer = document.getElementById("claimsContainer");

    payload.forEach((x) => {
        
        if (x.authClaimRight == 0)
            claimsContainer.appendChild(getEditableField(x, availableClaims));
        else
            claimsContainer.appendChild(getNotEditableField(x));
    })
}

await getMyInfo();