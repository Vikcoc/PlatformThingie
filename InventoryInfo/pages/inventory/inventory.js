import { authenticatedFetch } from '/public/authenticated-fetch';

async function GetItems() {
    var res = await authenticatedFetch("/inventory/filtered", {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
        },
        body: JSON.stringify({
            "entityProperties": [
                "test attribute",
                "header text"
            ],
            "templateProperties": [
                "test template attribute"
            ]
        })
    });

    if (!res.ok)
        return;

    var dto = await res.json();

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
        document.body.appendChild(sec);
    });
}

await GetItems();