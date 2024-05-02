import { authenticatedFetch } from '/public/authenticated-fetch';


var attributes = [];
var entityAttributes = [];

async function GetAttributes() {
    var res = await authenticatedFetch("/inventory/attributes", {
        method: "GET",
        headers: {
            "Content-Type": "application/json",
        }
    });

    if (!res.ok)
        return;

    var dto = await res.json();

    var cont = document.getElementById("attributes");
    cont.innerHTML = '';

    dto.forEach(x => {
        var sec = document.createElement("label");
        var check = document.createElement("md-checkbox");
        check.setAttribute('touch-target', 'wrapper');
        sec.appendChild(check);
        sec.innerHTML += "\n" + x;
        sec.onclick = (hand) => {
            if (!hand.target.checked)
                attributes.push(x);
            else
                attributes.splice(attributes.indexOf(x), 1);

        }
        cont.appendChild(sec);
    });
}

async function GetEntityAttributes() {
    var res = await authenticatedFetch("/inventory/entity-attributes", {
        method: "GET",
        headers: {
            "Content-Type": "application/json",
        }
    });

    if (!res.ok)
        return;

    var dto = await res.json();

    var cont = document.getElementById("entityAttributes");
    cont.innerHTML = '';

    dto.forEach(x => {
        var sec = document.createElement("label");
        var check = document.createElement("md-checkbox");
        check.setAttribute('touch-target', 'wrapper');
        sec.appendChild(check);
        sec.innerHTML += "\n" + x;
        sec.onclick = (hand) => {
            if (!hand.target.checked)
                entityAttributes.push(x);
            else
                entityAttributes.splice(entityAttributes.indexOf(x), 1);

        }
        cont.appendChild(sec);
    });
}
async function GetItems(attr, eattr) {
    if (attr.length == 0 || eattr.length == 0)
        window.alert("Please select at least one of each attributes");

    var res = await authenticatedFetch("/inventory/filtered", {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
        },
        body: JSON.stringify({
            "entityProperties": eattr,
            "templateProperties": attr
        })
    });

    if (!res.ok)
        return;

    var dto = await res.json();

    var cont = document.getElementById("entityContainer");
    cont.innerHTML = '';

    await dto.forEach(async x => {
        var sec = document.createElement("section");
        sec.classList.add("horizontalLine");

        await x.templateProperties.forEach(async y => {
            var module = await import(y.scriptName);
            var element = await module.inlineDisplay(y);
            sec.appendChild(element);
        });

        await x.entityProperties.forEach(async y => {
            var module = await import(y.scriptName);
            var element = await module.inlineDisplay(y);
            sec.appendChild(element);
        });
        cont.appendChild(sec);
    });
}

document.getElementById("loadEntities").onclick = () => GetItems(attributes, entityAttributes);
await GetAttributes();
await GetEntityAttributes();