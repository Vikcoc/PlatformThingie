export async function inlineDisplay(value) {
    var head = document.createElement("h3");
    head.textContent = value;
    return head;
}