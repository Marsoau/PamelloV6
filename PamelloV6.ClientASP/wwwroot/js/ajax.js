function GetUser(id, onSucces, onFailure) {
    AjaxDataRequest("User", id, onSucces, onFailure);
}
function GetSong(id, onSucces, onFailure) {
    AjaxDataRequest("Song", id, onSucces, onFailure);
}
function GetEpisode(id, onSucces, onFailure) {
    AjaxDataRequest("Episode", id, onSucces, onFailure);
}
function GetPlaylist(id, onSucces, onFailure) {
    AjaxDataRequest("Playlist", id, onSucces, onFailure);
}
function GetPlayer(id, onSucces, onFailure) {
    AjaxDataRequest("Player", id, onSucces, onFailure);
}
function AuthorizeWithCode(code, onSucces, onFailure) {
    AjaxAuthorizationRequest(code, onSucces, onFailure);
}

function AjaxDataRequest(datatype, id, onSucces, onFailure) {
    $.ajax({
        type: "GET",
        url: `https://localhost:7270/Data/${datatype}?id=${id}`,
        data: {},
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: onSucces,
        failure: onFailure
    })
}

function AjaxAuthorizationRequest(code, onSucces, onFailure) {
    $.ajax({
        type: "GET",
        url: `https://localhost:7270/Authorization/GetToken?code=${code}`,
        data: {},
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: onSucces,
    }).fail(onFailure);
}
