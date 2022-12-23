//Changes text of button to loading when app is busy
function ShowLoading(id) {
    document.getElementById(id).innerHTML = "Loading...";
}

//Show warning message when editing certain values
function ShowWarning(id) {
    var element = document.getElementById(id);
    if (element.classList.contains("visually-hidden")) {
        element.classList.toggle("visually-hidden");
    }
}

//Only show current and previous nav links
function DisableLink(id, id2) {
    var element = document.getElementById(id);
    var element2 = document.getElementById(id2);
    element.classList.toggle("visually-hidden");
    if (element2 != null) {
        element.classList.toggle("visually-hidden");
    }
}