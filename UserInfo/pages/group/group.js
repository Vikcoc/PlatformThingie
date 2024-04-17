import { authenticatedFetch } from '/public/authenticated-fetch';

function resetPermissions(active) {
    var checks = Array.from(document.getElementsByTagName("label"));
    checks.forEach(ch =>
        ch.getElementsByTagName("md-checkbox")[0].checked = active.includes(ch.innerText));
}

let editedUser = null;
function grabEdit(parent) {
    if (editedUser != null) {
        var buttons = editedUser.getElementsByTagName("md-filled-tonal-icon-button");
        buttons[0].style = "display: true";
        buttons[1].style = "display: none";
        buttons[2].style = "display: none";
    }

    editedUser = parent;
    if (parent == null) {
        resetPermissions([]);
        return;
    }
    resetPermissions(editedUser.groups);
    var buttons = editedUser.getElementsByTagName("md-filled-tonal-icon-button");
    buttons[0].style = "display: none";
    buttons[1].style = "display: true";
    buttons[2].style = "display: true";
}
async function getGroupsWithPermissions() {
    var res = await authenticatedFetch("/group/all", {
        method: "GET",
        headers: {
            "Content-Type": "application/json",
        }
    });

    if (!res.ok)
        return;

    var dto = await res.json();
    var userContainer = document.getElementById("groupContainer");
    dto.forEach(us => {
        var sec = document.createElement("section");
        sec.classList.add("horizontalLine");
        sec.groups = us.groups.filter(val => true);
        sec.userId = us.userId;

        var emails = document.createElement("div");
        emails.innerText = us.emails.reduce((a, b) => a + "\n" + b);
        sec.appendChild(emails);

        for (const i of [["/public/edit-logo", "Edit", () => grabEdit(sec), "display: true"],
        ["/public/save-logo", "Save", async () => {
            var res = await authenticatedFetch("/group", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify({
                    userId: sec.userId,
                    groups: sec.groups
                })
            });

            if (!res.ok)
                window.alert("Couldn't save");
            else {
                grabEdit(null);
                us.groups = sec.groups;
            }
        }, "display: none"],
        ["/public/reset-logo", "Reset", () => {
            sec.groups = us.groups.filter(val => true);
            resetPermissions(sec.groups);
        }, "display: none"]
        ]) {
            var but = document.createElement("md-filled-tonal-icon-button");
            but.onclick = i[2];
            but.style = i[3];

            var icon = document.createElement("img");
            icon.src = i[0];
            icon.alt = i[1];
            but.appendChild(icon);

            sec.appendChild(but);
        }

        userContainer.appendChild(sec);
    });
}

async function getGroups() {
    var res = await authenticatedFetch("/group/permissions", {
        method: "GET",
        headers: {
            "Content-Type": "application/json",
        }
    });

    if (!res.ok)
        return;

    var dto = await res.json();
    var groupsContainer = document.getElementById("permissionContainer");
    dto.forEach(gr => {
        var sec = document.createElement("label");
        var check = document.createElement("md-checkbox");
        check.setAttribute('touch-target', 'wrapper');
        sec.appendChild(check);
        sec.innerHTML += "\n" + gr;
        sec.onclick = (hand) => {
            if (editedUser == null)
                return;
            if (!hand.target.checked)
                editedUser.groups.push(gr);
            else
                editedUser.groups = editedUser.groups.filter(val => val != gr);

        }

        groupsContainer.appendChild(sec);
    });
}

await getGroups();
await getGroupsWithPermissions();