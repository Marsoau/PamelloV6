function MultipageSet(multipageValue, pageValue) {
    console.log(`opening page ${pageValue} for ${multipageValue} multipage collection`)

    let pageContainer = document.getElementById(`multipage-${multipageValue}-page-container`);
    let tabContainer = document.getElementById(`multipage-${multipageValue}-tab-container`);

    let pages = pageContainer.querySelectorAll(`.multipage-page`);
    let tabs = tabContainer.querySelectorAll(`.multipage-tab`);

    for (let page of pages) {
        page.style.display = page.id == `multipage-${multipageValue}-page-${pageValue}` ? "block" : "none";
    }
    for (let tab of tabs) {
        tab.className = tab.id == `multipage-${multipageValue}-tab-${pageValue}` ? "multipage-tab multipage-tab-selected" : "multipage-tab";
    }
}
