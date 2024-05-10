import { authenticatedFetch } from '/public/authenticated-fetch';

async function GetItems() {

    var attributes = Array.from(document.getElementsByClassName("template-attributes"))
        .flatMap(x => Array.from(x.getElementsByTagName("md-checkbox"))
            .filter(x => x.checked)
            .map(x => x.parentElement.textContent));
    var entityAttributes = Array.from(document.getElementsByClassName("entity-attributes"))
        .flatMap(x => Array.from(x.getElementsByTagName("md-checkbox"))
            .filter(x => x.checked)
            .map(x => x.parentElement.textContent));

    if (attributes.length == 0 || entityAttributes.length == 0) {
        window.alert("Please select at least one of each attributes");
        return;
    }

    var res = await authenticatedFetch("/inventory/filtered", {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
        },
        body: JSON.stringify({
            "entityProperties": entityAttributes,
            "templateProperties": attributes
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

function CreateAttrEnt(dto) {
    var a = document.createElement("h3");
    a.textContent = dto.templateName + " V" + dto.templateVersion;
    var c = document.createElement("section");
    c.classList.add("horizontalLine", "template-attributes");
    {
        dto.templateProperties.forEach(x => {
            var sec = document.createElement("label");
            var check = document.createElement("md-checkbox");
            check.setAttribute('touch-target', 'wrapper');
            sec.appendChild(check);
            sec.innerHTML += x;
            c.appendChild(sec);
        });
    }
    var e = document.createElement("section");
    e.classList.add("horizontalLine", "entity-attributes");
    {
        dto.entityProperties.forEach(x => {
            var sec = document.createElement("label");
            var check = document.createElement("md-checkbox");
            check.setAttribute('touch-target', 'wrapper');
            sec.appendChild(check);
            sec.innerHTML += x;
            e.appendChild(sec);
        });
    }
    return [a, c, e];
}
async function GetAttributesForTemplates() {
    var res = await authenticatedFetch("/inventory/templates-with", {
        method: "GET",
        headers: {
            "Content-Type": "application/json",
        }
    });

    if (!res.ok)
        return;

    var dto = await res.json();

    var container = document.getElementById("attribute-container");

    dto.forEach(x => CreateAttrEnt(x).forEach(y => container.appendChild(y)));
}

document.getElementById("loadEntities").onclick = GetItems;
await GetAttributesForTemplates();