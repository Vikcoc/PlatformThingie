import { authenticatedFetch } from '/public/authenticated-fetch';

async function GetTemplates() {
    var res = await authenticatedFetch("/inventory/templates", {
        method: "GET",
        headers: {
            "Content-Type": "application/json",
        }
    });

    if (!res.ok)
        return;

    var dto = await res.json();

    await dto.forEach(async x => {
        var sec = document.createElement("section");
        sec.classList.add("horizontalLine");

        var but = document.createElement("md-filled-tonal-button");
        but.textContent = "Create";
        sec.appendChild(but);

        await x.templateProperties.forEach(async y => {
            var module = await import(y.scriptName);
            var element = await module.inlineDisplay(y);
            sec.appendChild(element);
        });

        await x.entityProperties.forEach(async y => {
            var module = await import(y.scriptName);
            var element = y.writeable ? await module.editableDisplay(y) : await module.inlineDisplay(y);
            element.baseInfo = y;
            sec.appendChild(element);
        });

        but.onclick = async () => {
            var values = [];
            //because.foreach doesn't await
            for (const y of sec.children) {
                if (!y.baseInfo || !y.baseInfo.writeable)
                    continue;
                var module = await import(y.baseInfo.scriptName);
                values.push({
                    Name: y.baseInfo.name,
                    Value: await module.getValue(y)
                });
            }

            var res = await authenticatedFetch("/inventory", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify({
                    templateName: x.templateName,
                    templateVersion: x.templateVersion,
                    EntityProperties: values
                })
            });

            if (!res.ok) {
                window.alert("Failed to create");
                return;
            }

            await Array.from(sec.children).forEach(async y => {
                if (!y.baseInfo || !y.baseInfo.writeable)
                    return;
                var module = await import(y.baseInfo.scriptName);
                await module.setEditableDisplay(y, y.baseInfo);
            });
        };

        document.body.appendChild(sec);
    });
}

GetTemplates();