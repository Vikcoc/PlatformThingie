import { authenticatedFetch } from '/public/authenticated-fetch';

function resetPermissions(active) {
    var checks = Array.from(document.getElementsByTagName("label"));
    checks.forEach(ch =>
        ch.getElementsByTagName("md-checkbox")[0].checked = active.includes(ch.innerText));
}

let editedGroup = null;
function grabEdit(parent) {
    if (editedGroup != null) {
        var buttons = editedGroup.getElementsByTagName("md-filled-tonal-icon-button");
        buttons[0].style = "display: true";
        buttons[1].style = "display: none";
        buttons[2].style = "display: none";
        buttons[3].style = "display: none";
    }

    editedGroup = parent;
    if (parent == null) {
        resetPermissions([]);
        return;
    }
    resetPermissions(editedGroup.permissions);
    var buttons = editedGroup.getElementsByTagName("md-filled-tonal-icon-button");
    buttons[0].style = "display: none";
    buttons[1].style = "display: true";
    buttons[2].style = "display: true";
    buttons[3].style = "display: true";
}

function createGroup(groupName, permissions) {
    var sec = document.createElement("section");
    sec.classList.add("horizontalLine");
    sec.permissions = permissions.filter(val => true);
    sec.groupName = groupName;

    var group = document.createElement("div");
    group.innerText = groupName;
    sec.appendChild(group);

    for (const i of [["/public/edit-logo", "Edit", () => grabEdit(sec), "display: true"],
    ["/public/save-logo", "Save", async () => {
        var res = await authenticatedFetch("/user/group", {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify({
                groupName: sec.groupName,
                permissions: sec.permissions
            })
        });

        if (!res.ok)
            window.alert("Couldn't save");
        else {
            grabEdit(null);
            permissions = sec.permissions;
        }
    }, "display: none"],
    ["/public/reset-logo", "Reset", () => {
        sec.permissions = permissions.filter(() => true);
        resetPermissions(sec.permissions);
    }, "display: none"],
    ["/public/trashcan-logo", "Remove", async () => {
        var res = await authenticatedFetch("/user/group", {
            method: "DELETE",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify(sec.groupName)
        });

        if (!res.ok)
            window.alert("Couldn't delete");
        else {
            sec.parentElement.removeChild(sec);
            resetPermissions([]);
        }
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

    return sec
}
async function getGroupsWithPermissions() {
    var res = await authenticatedFetch("/user/group/all", {
        method: "GET",
        headers: {
            "Content-Type": "application/json",
        }
    });

    if (!res.ok)
        return;

    var dto = await res.json();
    var groupsContainer = document.getElementById("groupContainer");
    dto.forEach(us => {
        groupsContainer.appendChild(createGroup(us.groupName, us.permissions));
    });

    var pCont = document.getElementById("plusContainer");
    groupsContainer.removeChild(pCont);
    groupsContainer.appendChild(pCont);

}

async function getPermissions() {
    var res = await authenticatedFetch("/user/group/permissions", {
        method: "GET",
        headers: {
            "Content-Type": "application/json",
        }
    });

    if (!res.ok)
        return;

    var dto = await res.json();
    var permissionsContainer = document.getElementById("permissionContainer");
    dto.forEach(gr => {
        var sec = document.createElement("label");
        var check = document.createElement("md-checkbox");
        check.setAttribute('touch-target', 'wrapper');
        sec.appendChild(check);
        sec.innerHTML += "\n" + gr;
        sec.onclick = (hand) => {
            if (editedGroup == null)
                return;
            if (!hand.target.checked)
                editedGroup.permissions.push(gr);
            else
                editedGroup.permissions = editedGroup.permissions.filter(val => val != gr);

        }

        permissionsContainer.appendChild(sec);
    });
}

function makeGroupButton() {
    var name = document.getElementById("groupName");
    var grp = createGroup(name.value, []);

    var groupsContainer = document.getElementById("groupContainer");
    groupsContainer.appendChild(grp);
    grabEdit(grp);

    name.value = "";
    var pCont = document.getElementById("plusContainer");
    groupsContainer.removeChild(pCont);
    groupsContainer.appendChild(pCont);
    
}

document.getElementById("plusButton").onclick = makeGroupButton

await getPermissions();
await getGroupsWithPermissions();