import { authenticatedFetch } from '/public/authenticated-fetch';



function createButton(name, canDelete, canSave) {
    var sec = document.createElement("section");
    sec.classList.add("horizontalLine");

    var per = document.createElement("h4");
    per.innerText = name;

    sec.appendChild(per);

    if (canDelete) {
        var but = document.createElement("md-filled-tonal-icon-button");
        var icon = document.createElement("img");
        icon.src = "/public/trashcan-logo";
        icon.alt = "Delete";
        but.appendChild(icon);
        sec.appendChild(but);

        async function actualDel() {
            var res = await authenticatedFetch("/invtemplate/permission", {
                method: "DELETE",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify(name)
            });

            if (!res.ok) {
                window.alert("Cannot delete");
            }
            else {
                sec.parentElement.removeChild(sec);
            }
        }

        if (canSave) {
            but.onclick = () => sec.parentElement.removeChild(sec);

            var but2 = document.createElement("md-filled-tonal-icon-button");
            var icon2 = document.createElement("img");
            icon2.src = "/public/save-logo";
            icon2.alt = "Save";
            but2.onclick = async () => {
                var res = await authenticatedFetch("/invtemplate/permission", {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json",
                    },
                    body: JSON.stringify(name)
                });

                if (!res.ok) {
                    window.alert("Cannot save");
                }
                else {
                    but.onclick = actualDel;
                    sec.removeChild(but2);
                }
            };
            but2.appendChild(icon2);
            sec.appendChild(but2);
        }
        else {
            but.onclick = actualDel;
        }
    }
    return sec;
}

async function GetPermissions() {
    var res = await authenticatedFetch("/invtemplate/permission/all", {
        method: "GET",
        headers: {
            "Content-Type": "application/json",
        }
    });


    if (!res.ok)
        return;

    var dto = await res.json();

    var pcont = document.getElementById("permissionsContainer");
    var acont = document.getElementById("permPlusContainer");

    dto.forEach(async x => {
        pcont.appendChild(createButton(x.permissionName, x.canDelete, false));
    });

    pcont.removeChild(acont);
    pcont.appendChild(acont);
}

function onPlusClick() {
    var name = document.getElementById("templateName");
    var namePar = name.parentElement;

    var container = namePar.parentElement;
    container.appendChild(createButton(name.value, true, true));
    name.value = "";

    container.removeChild(namePar);
    container.appendChild(namePar);
}

document.getElementById("permPlusButton").onclick = onPlusClick;
await GetPermissions();