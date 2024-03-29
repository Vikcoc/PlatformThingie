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

    var claimsContainer = document.getElementById("claimsContainer");

    payload.forEach((x) => {
        var sec = document.createElement("section");
        sec.classList.add("horizontalLine");

        if (x.authClaimRight == 0) {
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
            inp.label = x.authClaimName;
            inp.value = x.authClaimValue;

            var del = document.createElement("md-filled-tonal-icon-button");
            var img = document.createElement("img");
            img.src = "/public/trashcan-logo";
            img.alt = "Delete";
            del.appendChild(img);

            sec.appendChild(sel);
            sec.appendChild(inp);
            sec.appendChild(del);
        }
        else {
            var claim = document.createElement("h3");
            claim.textContent = x.authClaimName + ":";
            var val = document.createElement("p");
            val.textContent = x.authClaimValue;

            sec.appendChild(claim);
            sec.appendChild(val);
        }
        
        claimsContainer.appendChild(sec);
    })
}

await getMyInfo();