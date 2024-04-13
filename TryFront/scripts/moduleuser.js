async function LoadModule(){
    var module = await import('/module');

    console.log(module.lada.name);
}

document.getElementById('load-button').onclick = LoadModule;