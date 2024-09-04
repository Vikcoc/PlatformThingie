export async function inlineDisplay(prop) {
    var value = prop.value == ""
        ? {
        name: "",
        value: ""
        }
        : JSON.parse(prop.value);

    var parent = document.createElement("div");
    parent.style.display = "flex";
    parent.style.flexDirection = "column";

    var name = document.createElement("h4");
    name.textContent = value.name;
    parent.appendChild(name);

    var image = document.createElement("img");
    image.src = value.value;
    image.style = 'width: auto; height: 5em;';
    parent.appendChild(image);

    return parent;
}

export async function editableDisplay(prop) {
    var value = prop.value == ""
        ? {
            name: "",
            value: ""
        }
        : JSON.parse(prop.value);

    var parent = document.createElement("div");
    parent.style.display = "flex";
    parent.style.flexDirection = "column";

    var name = document.createElement("md-outlined-text-field");
    name.label = prop.name;
    name.value = value.name;
    parent.appendChild(name);

    var image = document.createElement("md-outlined-text-field");
    image.label = 'image';
    image.value = value.value;
    parent.appendChild(image);

    return parent;
}

export async function getValue(elem) {
    var arr = elem.getElementsByTagName("md-outlined-text-field");
    return JSON.stringify({
        name: arr[0].value,
        value: arr[1].value
    });
}

export async function setEditableDisplay(elem, prop) {
    var value = prop.value == ""
        ? {
            name: "",
            value: ""
        }
        : JSON.parse(prop.value);
    var arr = elem.getElementsByTagName("md-outlined-text-field");

    arr[0].value = value.name;
    arr[1].value = value.value;
}