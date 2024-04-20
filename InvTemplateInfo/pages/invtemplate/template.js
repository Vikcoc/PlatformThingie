import { authenticatedFetch } from '/public/authenticated-fetch';

function MakeTemplate() {

}
async function MakeAttribute(container, beforeChild) {
    var scriptName = document.getElementById("attrAct").lastSelectedOption.value;
    var propName = document.getElementById("attrName");

    var sec = document.createElement("section");
    sec.classList.add("horizontalLine");

    var module = await import(scriptName);
    var element = await module.editableDisplay({
        name: propName.value,
        value: ""
    });
    element.baseInfo = {
        name: propName.value,
    };
    sec.appendChild(element);
    container.insertBefore(sec, beforeChild);
    propName.value = "";
}

async function MakeEntityAttribute() {

}

async function SelectPermissions(permissions) {

}

async function MoveTemplateToTop(template) {
    //container.insertBefore(newFreeformLabel, container.firstChild);
}

async function GetTemplates() {
    var res = await authenticatedFetch("/invtemplate/permission/all", {
        method: "GET",
        headers: {
            "Content-Type": "application/json",
        }
    });

    if (!res.ok)
        return;

    var dto = await res.json();
}

async function GetActions(){
    var res = await authenticatedFetch("/invtemplate/action", {
        method: "GET",
        headers: {
            "Content-Type": "application/json",
        }
    });

    if (!res.ok)
        return;
        
    var dto = await res.json();
    var drops = [
        document.getElementById("entAttrAct"),
        document.getElementById("attrAct")
    ];
    drops.forEach(x => {
        var fsel = document.createElement("md-select-option");
        fsel.value = dto[0];
        fsel.selected = true;
        fsel.textContent = dto[0];
        x.appendChild(fsel);
        

        for (var i = 1; i < dto.length; i++) {
            var sel = document.createElement("md-select-option");
            sel.setAttribute('value', dto[i]);
            sel.textContent = dto[i];
            x.appendChild(sel);
        }

        x.select(dto[0]);
    });
}

async function GetPermissions() {
    var res = await authenticatedFetch("/invtemplate/allpermissions", {
        method: "GET",
        headers: {
            "Content-Type": "application/json",
        }
    });

    if (!res.ok)
        return;

    var dto = await res.json();

    var permContainer = document.getElementById("permissionContainer");

    dto.forEach(x => {
        var spa = document.createElement("section");
        spa.classList.add("horizontalLine");

        var label = document.createElement("label");
        var check = document.createElement("md-checkbox");
        check.setAttribute('touch-target', 'wrapper');
        label.appendChild(check);
        label.innerHTML += "\n" + x;
        spa.appendChild(label);

        label = document.createElement("label");
        check = document.createElement("md-checkbox");
        check.setAttribute('touch-target', 'wrapper');
        label.appendChild(check);
        label.innerHTML += "\nWriteable";
        spa.appendChild(label);

        permContainer.appendChild(spa);
    })
}

document.getElementById("attrPlusButton").onclick = async (obj) =>
    await MakeAttribute(obj.target.parentElement.parentElement, obj.target.parentElement);

await GetActions();
await GetPermissions();