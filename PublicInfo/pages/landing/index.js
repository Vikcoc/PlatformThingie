var res = await fetch("/sites", {
    method: "GET",
    headers: {
        "Content-Type": "application/json",
    }
});

if (!res.ok)
    window.alert('Failed to get sites');
else {
    var sites = await res.json();
    var butContainer = document.getElementsByClassName('horizontalLine')[0];

    sites.forEach(x => {
        var but = document.createElement("md-filled-tonal-button");
        but.textContent = x.name;
        but.onclick = () => location.href = x.link;
        butContainer.appendChild(but);
    })
}