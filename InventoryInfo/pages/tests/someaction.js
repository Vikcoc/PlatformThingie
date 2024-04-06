export async function inlineDisplay(value){
    var image = document.createElement("img");
    image.src = value;
    image.style = 'width: auto; height: 5em;';
    return image;
}