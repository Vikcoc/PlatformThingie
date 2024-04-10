export async function inlineDisplay(prop){
    var image = document.createElement("img");
    image.src = prop.value;
    image.style = 'width: auto; height: 5em;';
    return image;
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