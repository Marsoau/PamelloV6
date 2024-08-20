function GetAuthUser(onSucces, onFailure) {
    $.ajax({
        type: "GET",
        url: `https://localhost:58631/Data/User?token=${GetToken()}`,
        data: {},
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: onSucces,
    }).fail(onFailure);
}
function GetUser(id, onSucces, onFailure) {
    AjaxDataRequest("User", id, onSucces, onFailure);
}
function GetSong(id, onSucces, onFailure) {
    AjaxDataRequest("Song", id, onSucces, onFailure);
}
function GetManySongs(page, count, onSucces, onFailure) {
    AjaxManyDataRequest("Song", page, count, onSucces, onFailure);
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
function GetManyPlayers(page, count, onSucces, onFailure) {
    AjaxManyDataRequest("Player", page, count, onSucces, onFailure);
}
function AuthorizeWithCode(code, onSucces, onFailure) {
    AjaxAuthorizationRequest(code, onSucces, onFailure);
}

function AjaxDataRequest(datatype, id, onSucces, onFailure) {
    $.ajax({
        type: "GET",
        url: `https://localhost:58631/Data/${datatype}?id=${id}`,
        data: {},
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: onSucces,
    }).fail(onFailure);
}

function AjaxManyDataRequest(datatype, page, count, onSucces, onFailure) {
    $.ajax({
        type: "GET",
        url: `https://localhost:58631/Data/All${datatype}s?page=${page}&count=${count}`,
        data: {},
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: onSucces,
    }).fail(onFailure);
}

function AjaxAuthorizationRequest(code, onSucces, onFailure) {
    $.ajax({
        type: "GET",
        url: `https://localhost:58631/Authorization/GetToken?code=${code}`,
        data: {},
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: onSucces,
    }).fail(onFailure);
}

function InvokeCommand(commandName, args = {}, after) {
    let url = `https://localhost:58631/Command?name=${commandName}`;
    let commandStr = `${commandName}(`;

    for (let key of Object.keys(args)) {
        url += `&${key}=${args[key]}`;
        commandStr += `${key}: ${args[key]}, `;
    }

    commandStr += ")";

    $.ajax({
        type: "GET",
        url: url,
        data: {},
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        beforeSend: function (request) {
            request.setRequestHeader("user-token", GetToken());
        },
        success: () => {
            if (after) after();
        }
    }).fail((a, b, c) => {
        if (a.statusCode().status == 200) {
            console.log(`Executed command ${commandStr}`);
        }
        else {
            console.log(`Cant execute command "${commandName}";\ncode: ${a.statusCode().status}\nreason: ${a.responseText}`);
        }
        if (after) after();
    });
}
