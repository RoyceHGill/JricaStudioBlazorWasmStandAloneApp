function SetTimer(seconds) {
    const timer = document.querySelector('#load-count-down')
    if (timer != undefined || timer != null) {
        timer.setAttribute('style', `animation: rotation ${seconds}s linear infinite;`);
    }
}


function ChangeBtnSuccessToBtnWarning(id) {
    const cartButton = document.querySelector("#" + id)

    if (cartButton.classList.contains("btn-success")) {
        cartButton.classList.remove("jay-green-bg")
        cartButton.classList.remove("btn-success")

        cartButton.classList.add("btn-warning")
    }
}

function ChangeBtnWarningToBtnSuccess(id) {
    const cartButton = document.querySelector("#" + id)

    if (cartButton.classList.contains("btn-warning")) {
        cartButton.classList.remove("btn-warning")
        cartButton.classList.add("jay-green-bg")
        cartButton.classList.add("btn-success")

    }
}

function SetVisible(id) {
    const element = document.querySelector('[data-itemId="' + id + '"]')
    if (element.classList.contains("visually-hidden")) {
        element.classList.remove("visually-hidden")
    }
}

function SetHidden(id) {
    const element = document.querySelector('[data-itemId="' + id + '"]')
    if (!element.classList.contains("visually-hidden")) {
        element.classList.add("visually-hidden")
    }
}

function SetDisabled(id) {
    const element = document.querySelector('[data-itemId="' + id + '"]')
    if (!element.classList.contains("disabled")) {
        element.classList.add("disabled")
    }
}

function RemoveDisabled(id) {
    const element = document.querySelector('[data-itemId="' + id + '"]')
    if (element.classList.contains("disabled")) {
        element.classList.remove("disabled")
    }
}

function ToggleTableRowHighlight(id) {
    const isDisabledCheckBox = document.querySelector('[data-itemId="' + id + '"]')
    //const element = isDisabledCheckBox.parentElement.parentElement
    //if (isDisabledCheckBox.checked) {
    //    if (!element.classList.contains('bg-secondary')) {
    //        element.classList.add('bg-secondary')
    //    }
    //}
    //else {
    //    if (element.classList.contains('bg-secondary')) {
    //        element.classList.remove('bg-secondary')
    //    }
    //}
}


