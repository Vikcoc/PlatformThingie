export async function inlineDisplay(prop) {
    var head = document.createElement("h3");
    head.textContent = prop.value;
    return head;
}

export async function editableDisplay(prop) {
    var head = document.createElement("md-outlined-text-field");
    head.label = prop.name;
    head.value = prop.value;
    return head;
}

export async function getValue(elem) {
    return elem.value;
}

export async function setEditableDisplay(elem, prop) {
    elem.value = prop.value;
}