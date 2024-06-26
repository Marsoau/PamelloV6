function MultipageSet(multipageValue, pageValue) {
    console.log(`opening page "${pageValue}" from "${multipageValue}" multipages collection`)

    let pageContainer = document.getElementById(`multipage-${multipageValue}-page-container`);
    let tabContainer = document.getElementById(`multipage-${multipageValue}-tab-container`);

    let pages = pageContainer.querySelectorAll(`.multipage-page`);
    let tabs = tabContainer.querySelectorAll(`.multipage-tab`);

    for (let page of pages) {
        page.style.display = page.id == `multipage-${multipageValue}-page-${pageValue}` ? "grid" : "none";
    }
    for (let tab of tabs) {
        tab.className = tab.id == `multipage-${multipageValue}-tab-${pageValue}` ? "multipage-tab multipage-tab-selected" : "multipage-tab";
        tab.querySelector("input").disabled = false;
    }
}

function MultipageDisable(multipageValue) {
    let pageContainer = document.getElementById(`multipage-${multipageValue}-page-container`);
    let tabContainer = document.getElementById(`multipage-${multipageValue}-tab-container`);

    let pages = pageContainer.querySelectorAll(`.multipage-page`);
    let tabs = tabContainer.querySelectorAll(`.multipage-tab`);

    for (let page of pages) {
        page.style.display = "none";
    }
    for (let tab of tabs) {
        tab.className = "multipage-tab";
        tab.querySelector("input").disabled = true;
    }
}

function EditPEValue(peName) {
    let peElement = document.querySelector(`#${peName}-pe`);

    let valueElement = peElement.querySelector(".protected-edit-value");
    let inputElement = peElement.querySelector(".protected-edit-input");

    valueElement.style.display = "none";
    inputElement.style.display = "grid";

    inputElement.querySelector("input").value = valueElement.innerHTML;
}

function SavePEValue(peName, action) {
    let peElement = document.querySelector(`#${peName}-pe`);

    let valueElement = peElement.querySelector(".protected-edit-value");
    let inputElement = peElement.querySelector(".protected-edit-input");

    valueElement.style.display = "block";
    inputElement.style.display = "none";
}

function SetPEValue(peName, value) {
    let peElement = document.querySelector(`#${peName}-pe`);

    let valueElement = peElement.querySelector(".protected-edit-value");
    let inputElement = peElement.querySelector(".protected-edit-input");

    valueElement.innerHTML = value;
    inputElement.value = value;
}

function SaveToken(token) {
    document.cookie = `token=${token}`;
}

function GetToken() {
    let value = `; ${document.cookie}`;
    let parts = value.split(`; token=`);
    let token = null;
    if (parts.length === 2) token = parts.pop().split(';').shift();

    if (token) return token;
    return null;
}

function RemoveToken() {
    document.cookie = `token=;`;
    window.location.replace("/");

}