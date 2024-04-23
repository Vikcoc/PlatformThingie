import { authenticatedFetch } from '/public/authenticated-fetch';

//i know it's a bad page
//and that this should just be a list of templates, that sends you to the list of attributes
//and that list of attributes would send you further to edit the attribute and permissions
//but that is nicer with ssr or spa
//and i don't want to parse the route for these arguments
//and it is also kinda interesting to manage such a large state machine

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

var editedTemplate = null;
var editedAttribute = null;
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
        else {
            label.onclick = (hand) => {
                if (editedAttribute == null)
                    return;
                if (!hand.target.checked)
                    editedAttribute.dto.permissions.push(name);
                else
                    editedAttribute.dto.permissions.splice(editedAttribute.dto.permissions.indexOf(name), 1);
            }
        }
        
        return spa;
    }

    dto.forEach(x => {
        permContainer.appendChild(createPermission(x, false));
        entPermContainer.appendChild(createPermission(x, true));
    });
}

function flushAttrPermissions(list) {
    var container = document.getElementById("attributePermissionsContainer");
    var checks = Array.from(container.getElementsByTagName("label"));
    checks.forEach(ch =>
        ch.getElementsByTagName("md-checkbox")[0].checked = list.includes(ch.innerText));
}

async function createAttribute(attrDto) {
    //for editedTemplate

    if (editedTemplate == null)
        return;

    var sec = document.createElement("section");
    sec.dto = attrDto;
    sec.classList.add("horizontalLine");
    sec.deselectAction = async () => {
        Array.from(sec.getElementsByClassName("editButton"))
            .forEach(x => x.style.display = '');
        Array.from(sec.getElementsByClassName("resetButton"))
            .forEach(x => x.style.display = 'none');

        var mod = sec.getElementsByClassName("attrDisplay")[0];
        if (mod)
            attrDto.attrValue = await module.getValue(mod);
        var element = await module.inlineDisplay({
            name: attrDto.attrName,
            value: attrDto.attrValue
        });
        element.classList.add("attrDisplay");
        Array.from(sec.getElementsByClassName("attrDisplay"))
            .forEach(x => sec.replaceChild(element, x));
    }
    sec.oldPermissions = sec.dto.permissions.filter(() => true);


    var nam = document.createElement("h3");
    nam.innerText = attrDto.attrName;
    sec.appendChild(nam);
    var scr = document.createElement("p");
    scr.innerText = attrDto.attrAction;
    sec.appendChild(scr);
    var module = await import(attrDto.attrAction);
    var element = await module.inlineDisplay({
        name: attrDto.attrName,
        value: attrDto.attrValue
    });
    element.classList.add("attrDisplay");
    sec.appendChild(element);
    {
        //delete
        var but = document.createElement("md-filled-tonal-icon-button");
        var img = document.createElement("img");
        but.classList.add("deleteButton");
        img.src = "/public/trashcan-logo";
        img.alt = "Delete";
        but.appendChild(img);
        but.onclick = () => {
            sec.parentElement.removeChild(sec);
            editedTemplate.dto.templateAttributes.splice(editedTemplate.dto.templateAttributes.indexOf(attrDto), 1);
            if (editedAttribute == sec)
                editedAttribute = null;
        }
        sec.appendChild(but);
    }
    {
        //edit
        var but = document.createElement("md-filled-tonal-icon-button");
        var img = document.createElement("img");
        but.classList.add("editButton");
        img.src = "/public/edit-logo";
        img.alt = "Edit";
        but.appendChild(img);
        but.onclick = async () => {
            Array.from(sec.getElementsByClassName("editButton"))
                .forEach(x => x.style.display = 'none');
            Array.from(sec.getElementsByClassName("resetButton"))
                .forEach(x => x.style.display = '');
            var editable = await module.editableDisplay({
                name: attrDto.attrName,
                value: attrDto.attrValue
            });
            editable.classList.add("attrDisplay");
            Array.from(sec.getElementsByClassName("attrDisplay"))
                .forEach(x => sec.replaceChild(editable, x));
            if (editedAttribute)
                editedAttribute.deselectAction();
            editedAttribute = sec;
            flushAttrPermissions(sec.dto.permissions);
        }
        sec.appendChild(but);
    }
    {
        //reset
        var but = document.createElement("md-filled-tonal-icon-button");
        var img = document.createElement("img");
        but.classList.add("resetButton");
        img.src = "/public/reset-logo";
        img.alt = "Reset";
        but.appendChild(img);
        but.onclick = async () => {
            for (const x of Array.from(sec.getElementsByClassName("attrDisplay")))
                await module.setEditableDisplay(x, {
                    name: attrDto.attrName,
                    value: attrDto.attrValue
                });
            sec.dto.permissions = sec.oldPermissions.filter(() => true);
            flushAttrPermissions(sec.dto.permissions);
        }
        sec.appendChild(but);
        Array.from(sec.getElementsByClassName("resetButton"))
            .forEach(x => x.style.display = 'none');
    }
    return sec;
}

async function flushAttributes(dtos) {
    var container = document.getElementById("attributeContainer");
    var pluses = Array.from(container.getElementsByClassName("plusContainer"));
    container.innerHTML = '';
    editedAttribute = null;

    for (const x of dtos) {
        var elem = await createAttribute(x);
        container.appendChild(elem);
    }

    pluses.forEach(x => container.appendChild(x));
}

function createTemplate(templateDto, exists) {
    async function editAction(arg) {
        if (editedTemplate)
            editedTemplate.deselectAction();

        var parent = arg.target.parentElement;
        editedTemplate = parent;

        var pparent = parent.parentElement;
        pparent.removeChild(parent);
        pparent.insertBefore(parent, pparent.firstChild);
        document.body.scrollIntoView();

        await flushAttributes(parent.dto.templateAttributes);

        Array.from(parent.getElementsByClassName("saveButton"))
            .forEach(x => x.style.display = '');
        Array.from(parent.getElementsByClassName("editButton"))
            .forEach(x => x.style.display = 'none');
        Array.from(parent.getElementsByClassName("releaseButton"))
            .forEach(x => x.style.display = 'none');
    }
    async function saveAction(arg) {
        var parent = arg.target.parentElement;

        var res = await authenticatedFetch("/invtemplate", {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify({
                templateName: parent.dto.templateName,
                templateVersion: parent.dto.templateVersion,
                templateAttributes: parent.dto.templateAttributes,
                entityAttributes: parent.dto.entityAttributes
            })
        });

        if (!res.ok) {
            window.alert("Cannot save");
            return;
        }

        flushAttributes([]);

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
        var el = createTemplate({
            templateName: parent.dto.templateName,
            templateVersion: parent.dto.templateVersion + 1,
            released: false,
            latest: true,
            templateAttributes: parent.dto.templateAttributes,
            entityAttributes: parent.dto.entityAttributes
        })
        el.makeParentLatest = parent.makeLatest;
        parent.parentElement.insertBefore(el, parent.parentElement.firstChild);
        document.body.scrollIntoView();
        if (editedTemplate)
            editedTemplate.deselectAction();
        editedTemplate = el;
        Array.from(parent.getElementsByClassName("plusButton"))
            .forEach(x => x.style.display = 'none');
        parent.dto.latest = false;
    }
    async function releaseAction(arg) {
        var parent = arg.target.parentElement;

        var res = await authenticatedFetch("/invtemplate/release", {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify({
                templateName: parent.dto.templateName,
                templateVersion: parent.dto.templateVersion
            })
        });

        if (!res.ok) {
            window.alert("Cannot release");
            return;
        }

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

    if (!templateDto.released) {
        var but = document.createElement("md-filled-tonal-icon-button");
        var img = document.createElement("img");
        but.classList.add("editButton");
        but.onclick = editAction;
        img.src = "/public/edit-logo";
        img.alt = "Edit";
        but.appendChild(img);
        if(!exists)
            but.style.display = 'none';
        sec.appendChild(but);

        but = document.createElement("md-filled-tonal-icon-button");
        img = document.createElement("img");
        but.classList.add("saveButton");
        but.onclick = saveAction;
        img.src = "/public/save-logo";
        img.alt = "Edit";
        but.appendChild(img);
        if (exists)
            but.style.display = 'none';
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

    Array.from(document.getElementById("templateContainer").getElementsByClassName("plusContainer"))
        .forEach(x => {
            var par = x.parentElement;
            par.removeChild(x);
            par.appendChild(x);
        })
}

function templatesPlusButton() {
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
}
function attributesPlusButton() {
    Array.from(document.getElementById("attributeContainer").getElementsByClassName("plusContainer"))
        .flatMap(x => Array.from(x.getElementsByClassName("plusButton")))
        .forEach(x => x.onclick = async (arg) => {
            if (editedTemplate == null)
                return;
            var dto = {
                attrName: Array.from(arg.target.parentElement.getElementsByClassName("name")).map(x => x.value).reduce((a, b) => a + b),
                attrValue: "",
                attrAction: arg.target.parentElement.getElementsByClassName("actionsContainer")[0]?.lastSelectedOption.value ?? '',
                permissions: []
            };
            editedTemplate.dto.templateAttributes.push(dto);
            var temp = await createAttribute(dto);
            arg.target.parentElement.parentElement.insertBefore(temp, arg.target.parentElement)
            Array.from(arg.target.parentElement.getElementsByClassName("name")).forEach(x => x.value = "");
        });
}

templatesPlusButton();
attributesPlusButton();
await getActions();
await getPermissions();
await getTemplates();