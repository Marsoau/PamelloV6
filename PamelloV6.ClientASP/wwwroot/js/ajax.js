function GetUser(id, action) {
    AjaxDataRequest("User", id, action);
}
function GetSong(id, action) {
    AjaxDataRequest("Song", id, action);
}
function GetEpisode(id, action) {
    AjaxDataRequest("Episode", id, action);
}
function GetPlaylist(id, action) {
    AjaxDataRequest("Playlist", id, action);
}
function GetPlayer(id, action) {
    AjaxDataRequest("Player", id, action);
}

function AjaxDataRequest(datatype, id, action) {
    $.ajax({
        type: "GET",
        url: `https://localhost:7270/Data/${datatype}?id=${id}`,
        data: {},
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        beforeSend: function () {
            console.log("before");
        },
        success: function (response) {
            console.log("sssuuuccccses")
            action(response)
        },
        complete: function () {
            console.log("complete");
        },
        failure: function (jqXHR, textStatus, errorThrown) {
            console.log("failure");
        }
    })
}
