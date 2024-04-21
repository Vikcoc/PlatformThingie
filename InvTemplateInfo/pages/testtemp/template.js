import { authenticatedFetch } from '/public/authenticated-fetch';
async function getActions() {
    var res = await authenticatedFetch("/invtemplate/action", {
        method: "GET",
        headers: {
            "Content-Type": "application/json",
        }
    });

    if (!res.ok)
        return;

    var dto = await res.json();
    var drops = Array.from(document.getElementsByClassName('actionsContainer'));
    drops.forEach(x => {
        dto.forEach(y => {
            var sel = document.createElement("md-select-option");
            sel.setAttribute('value', y);
            sel.textContent = y;
            x.appendChild(sel);
        })
        x.select(dto[0]);
    });
}

async function getPermissions() {
    var res = await authenticatedFetch("/invtemplate/allpermissions", {
        method: "GET",
        headers: {
            "Content-Type": "application/json",
        }
    });

    if (!res.ok)
        return;

    var dto = await res.json();
    
    var permContainer = document.getElementById("attributePermissionsContainer");
    var entPermContainer = document.getElementById("entityAttributePermissionsContainer");

    function createPermission(name, canWrite) {
        var spa = document.createElement("section");
        spa.classList.add("horizontalLine");

        var label = document.createElement("label");
        var check = document.createElement("md-checkbox");
        check.setAttribute('touch-target', 'wrapper');
        label.appendChild(check);
        label.innerHTML += "\n" + name;
        spa.appendChild(label);

        if (canWrite) {
            label = document.createElement("label");
            check = document.createElement("md-checkbox");
            check.setAttribute('touch-target', 'wrapper');
            label.appendChild(check);
            label.innerHTML += "\nWriteable";
            spa.appendChild(label);
        }
        
        return spa;
    }

    dto.forEach(x => {
        permContainer.appendChild(createPermission(x, false));
        entPermContainer.appendChild(createPermission(x, true));
    });
}

var editedTemplate = null;
function createTemplateFromParent(parent, templateDto, exists) {
    function editAction(arg) {
        var parent = arg.target.parentElement;
        //draw the attributes

        Array.from(parent.getElementsByClassName("saveButton"))
            .forEach(x => x.style.display = '');
        Array.from(parent.getElementsByClassName("editButton"))
            .forEach(x => x.style.display = 'none');
        Array.from(parent.getElementsByClassName("releaseButton"))
            .forEach(x => x.style.display = 'none');
        if (editedTemplate)
            editedTemplate.deselectAction();
        editedTemplate = arg.target.parentElement;
    }
    function saveAction(arg) {
        var parent = arg.target.parentElement;
        //actually save
        if (parent.prevVersion)
            removeEdit(prevVersion);

        Array.from(parent.getElementsByClassName("saveButton"))
            .forEach(x => x.style.display = 'none');
        Array.from(parent.getElementsByClassName("editButton"))
            .forEach(x => x.style.display = '');
        Array.from(parent.getElementsByClassName("deleteButton"))
            .forEach(x => parent.removeChild(x));
        Array.from(parent.getElementsByClassName("releaseButton"))
            .forEach(x => x.style.display = '');
        editedTemplate = null;
    }
    function plusAction(arg) {
        var parent = arg.target.parentElement;
        //create new template and add to parent.parentElement
        var el = createTemplateFromParent(parent, {
            templateName: parent.dto.templateName,
            templateVersion: parent.dto.templateVersion + 1,
            released: false,
            latest: true,
            templateAttributes: parent.dto.templateAttributes,
            entityAttributes: parent.dto.entityAttributes
        })
        el.makeParentLatest = parent.makeLatest;
        parent.parentElement.insertBefore(el, parent.parentElement.firstChild);
        document.scrollIntoView();
        Array.from(parent.getElementsByClassName("plusButton"))
            .forEach(x => x.style.display = 'none');
        parent.dto.latest = false;
    }
    function releaseAction(arg) {
        var parent = arg.target.parentElement;
        //send release to be
        Array.from(parent.getElementsByClassName("saveButton"))
            .forEach(x => parent.removeChild(x));
        Array.from(parent.getElementsByClassName("editButton"))
            .forEach(x => parent.removeChild(x));
        Array.from(parent.getElementsByClassName("plusButton"))
            .forEach(x => x.style.display = parent.dto.latest ? '' : 'none');
        parent.dto.released = true;
    }
    function deleteAction(arg) {
        var parent = arg.target.parentElement;
        var parentContainer = parent.parentElement;
        parentContainer.removeChild(parent);
        if (parent.makeParentLatest)
            parent.makeParentLatest();
    }

    var sec = document.createElement("section");

    sec.deselectAction = () => {
        Array.from(sec.getElementsByClassName("saveButton"))
            .forEach(x => x.style.display = 'none');
        Array.from(sec.getElementsByClassName("editButton"))
            .forEach(x => x.style.display = '');
        Array.from(sec.getElementsByClassName("releaseButton"))
            .forEach(x => x.style.display = sec.getElementsByClassName("deleteButton").length == 0 ? '' : 'none');
    };
    sec.makeLatest = () => {
        Array.from(sec.getElementsByClassName("plusButton"))
            .forEach(x => x.style.display = '');
        sec.dto.latest = true;
    }

    sec.classList.add("horizontalLine");
    sec.dto = templateDto;
    var name = document.createElement("h3");
    name.innerText = templateDto.templateName + " V" + templateDto.templateVersion;
    sec.appendChild(name);

    if (!templateDto.released && templateDto.latest) {
        var but = document.createElement("md-filled-tonal-icon-button");
        var img = document.createElement("img");
        but.classList.add("editButton");
        but.onclick = editAction;
        img.src = "/public/edit-logo";
        img.alt = "Edit";
        but.appendChild(img);
        but.style.display = 'none';
        sec.appendChild(but);

        but = document.createElement("md-filled-tonal-icon-button");
        img = document.createElement("img");
        but.classList.add("saveButton");
        but.onclick = saveAction;
        img.src = "/public/save-logo";
        img.alt = "Edit";
        but.appendChild(img);
        sec.appendChild(but);
    }

    if (templateDto.latest) {
        var but = document.createElement("md-filled-tonal-icon-button");
        var img = document.createElement("img");
        but.classList.add("plusButton");
        but.onclick = plusAction;
        img.src = "/public/plus-logo";
        img.alt = "Add";
        but.appendChild(img);
        sec.appendChild(but);

        if (!templateDto.released)
            but.style.display = 'none';
    }

    
    var releaseBut = document.createElement("md-filled-tonal-button");
    releaseBut.classList.add("releaseButton");
    releaseBut.onclick = releaseAction;
    releaseBut.innerText = "Release";
    sec.appendChild(releaseBut);
    if (!exists) {
        releaseBut.style.display = 'none';
        var but = document.createElement("md-filled-tonal-icon-button");
        var img = document.createElement("img");
        but.classList.add("deleteButton");
        but.onclick = deleteAction;
        img.src = "/public/trashcan-logo";
        img.alt = "Delete";
        but.appendChild(img);
        sec.appendChild(but);
    }

    return sec;
}

function createTemplate(templateDto, exists) {
    return createTemplateFromParent(null, templateDto, exists);
}

async function getTemplates() {
    var res = await authenticatedFetch("/invtemplate/all", {
        method: "GET",
        headers: {
            "Content-Type": "application/json",
        }
    });

    if (!res.ok)
        return;

    var dto = await res.json();

    var templateContainer = document.getElementById("templateContainer");

    dto.forEach(x => {
        templateContainer.appendChild(createTemplate(x, true));
    })
}


Array.from(document.getElementById("templateContainer").getElementsByClassName("plusContainer"))
    .flatMap(x => Array.from(x.getElementsByClassName("plusButton")))
    .forEach(x => x.onclick = (arg) => {
        var temp = createTemplate({
            templateName: Array.from(arg.target.parentElement.getElementsByClassName("name")).map(x => x.value).reduce((a, b) => a + b),
            templateVersion: 0,
            released: false,
            latest: true,
            templateAttributes: [],
            entityAttributes: []
        }, false);
        arg.target.parentElement.parentElement.insertBefore(temp, arg.target.parentElement.parentElement.firstChild)
        Array.from(arg.target.parentElement.getElementsByClassName("name")).forEach(x => x.value = "");
        if (editedTemplate)
            editedTemplate.deselectAction();
        editedTemplate = temp;
        document.body.scrollIntoView();
    });

await getActions();
await getPermissions();
await getTemplates();