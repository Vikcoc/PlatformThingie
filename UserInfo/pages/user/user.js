import { authenticatedFetch } from '/public/authenticated-fetch';

function resetGroups(active) {
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
    resetGroups(editedUser.groups);
    var buttons = editedUser.getElementsByTagName("md-filled-tonal-icon-button");
    buttons[0].style = "display: none";
    buttons[1].style = "display: true";
    buttons[2].style = "display: true";
}
async function getUsersWithGroups() {
    //todo store the array of groups on the section
    //use edit icon and then save and reset icons
    var res = await authenticatedFetch("/user/all", {
        method: "GET",
        headers: {
            "Content-Type": "application/json",
        }
    });

    if (!res.ok)
        return;

    var dto = await res.json();
    userContainer = document.getElementById("userContainer");
    dto.forEach(us => {
        var sec = document.createElement("section");
        sec.classList.add("horizontalLine");
        sec.groups = us.groups;
        sec.userId = us.userId;

        var emails = document.createElement("div");
        emails.innerText = us.emails.reduce((a, b) => a + "\n" + b);
        sec.appendChild(emails);

        for (const i of [["/public/edit-logo", "Edit", () => grabEdit(sec), "display: true"],
            ["/public/save-logo", "Save", () => console.log('dummy'), "display: none"],
            ["/public/reset-logo", "Reset", () => console.log('dummy'), "display: none"]
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

await getUsersWithGroups();