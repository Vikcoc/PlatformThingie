import { authenticatedFetch } from '/public/authenticated-fetch';

async function TryQuery() {
    var res = await authenticatedFetch("/inventory/all/filtered", {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
        },
        body: JSON.stringify({
            "entityProperties": [
                "test attribute"
            ],
            "templateProperties": [
                "test template attribute"
            ]
        })
    });

    console.log(res.ok);
}

document.getElementById('load-button').onclick = TryQuery;