let input = document.getElementById("code-input");
let error = document.getElementById("error-message");

function AuthWithCode() {
    AuthorizeWithCode(input.value, OnSucces, OnFailure);
}

function OnSucces(token) {
    error.style.display = "none";
    window.location.replace("/Player");
}
function OnFailure(jqXHR, textStatus, errorThrown) {
    error.style.display = "block";
    error.innerHTML = jqXHR.responseText;
}
