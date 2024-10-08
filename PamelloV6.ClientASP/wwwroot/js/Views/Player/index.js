﻿let user = null;
let player = null;
let currentSong = null;

let selectedSong = null;

let isTimeEdit = false;

let songListPage = 0;

let playerSelectElement = document.getElementById("player-select");
let playerSelectContainerElement = document.getElementsByClassName("player-select-container")[0];
let playerCreateContainerElement = document.getElementsByClassName("player-create-container")[0];

let playerCreateInput = document.getElementById("player-create-input");

let userCoverElement = document.getElementById("user-cover");
let userNameElement = document.getElementById("user-name");
let userDiscordIdElement = document.getElementById("user-discordid");

let songCoverElement = document.getElementById("song-cover");
let songTitleElement = document.getElementById("song-title");
let songAuthorElement = document.getElementById("song-author");

let songTimeContainerElement = document.getElementsByClassName("song-time-container")[0];
let songTimeElement = document.getElementById("song-time");
let songDurationElement = document.getElementById("song-duration");
let songTimeSlider = document.getElementById("player-slider");

let queueListElement = document.getElementById("queue-list");
let songListElement = document.getElementById("songs-list");

let randomButton = document.getElementById("is-random-queue-button");
let reversedButton = document.getElementById("is-reversed-queue-button");
let noLeftoversButton = document.getElementById("is-no-leftovers-queue-button");

let prevButton = document.getElementById("prev-button");
let ppButton = document.getElementById("pp-button");
let nextButton = document.getElementById("next-button");

let selectedSongCoverElement = document.getElementById("selected-song-cover");

let selectedSongEpisodesListContainer = document.getElementById("selected-song-episodes-list-container");
let currentSongEpisodesListContainer = document.getElementById("current-song-episodes-list-container");

MultipageSet("le", "episodes");
MultipageSet("addition", "songs");

LoadUser();
UpdateSelectedSong();
UpdateSongsList();

SelectSong(2);

function randomButtonClick() {
    InvokeCommand("PlayerQueueRandom", { value: !player.queueIsRandom })
}
function reversedButtonClick() {
    InvokeCommand("PlayerQueueReversed", { value: !player.queueIsReversed })
}
function noLeftoversButtonClick() {
    InvokeCommand("PlayerQueueNoLeftovers", { value: !player.queueIsNoLeftovers })
}

function prevButtonClick() {
    InvokeCommand("PlayerPrev");
}
function ppButtonClick() {
    if (player.isPaused) {
        InvokeCommand("PlayerResume");
    }
    else {
        InvokeCommand("PlayerPause");
    }
}
function nextButtonClick() {
    InvokeCommand("PlayerNext");
}

function removeButtonClick(songQueuePosition) {
    InvokeCommand("PlayerQueueRemoveSong", { songPosition: songQueuePosition })
}
function requestNextButtonClick(songQueuePosition) {
    if (player.nextPositionRequest == songQueuePosition) {
        InvokeCommand("PlayerQueueRequestNext", { position: "" })
    }
    else {
        InvokeCommand("PlayerQueueRequestNext", { position: songQueuePosition })
    }
}

function SliderTimeChanged() {
    isTimeEdit = true;
    songTimeElement.innerHTML = ToStringTime(songTimeSlider.value);
}
function SliderTimeSubmited() {
    isTimeEdit = false;
    InvokeCommand("PlayerRewind", { seconds: songTimeSlider.value });
    UpdatePlayerTime();
}

function addSelectedSongButtonClick() {
    if (selectedSong) {
        InvokeCommand('PlayerQueueAddSong', { songId: selectedSong.id });
    }
}

PamelloEvents.addEventListener("updatedSelectedPlayer", message => {
    user.selectedPlayerId = JSON.parse(message.data);
    console.log(`selected new "${user.selectedPlayerId}" player`);
    LoadPlayer()
});
PamelloEvents.addEventListener("updatedPlayerCurrentSongTimePassed", message => {
    player.currentSongTimePassed = JSON.parse(message.data);
    UpdatePlayerTime()
});
PamelloEvents.addEventListener("updatedPlayerCurrentSongTimeTotal", message => {
    player.currentSongTimeTotal = JSON.parse(message.data);
    UpdatePlayerTime()
});
PamelloEvents.addEventListener("updatedPlayerQueuePosition", message => {
    player.queuePosition = JSON.parse(message.data);
    UpdateQueuePosition();
});
PamelloEvents.addEventListener("updatedPlayerNextPositionRequest", message => {
    player.nextPositionRequest = JSON.parse(message.data);
    UpdateQueuePosition();
});
PamelloEvents.addEventListener("updatedPlayerQueueSongIds", message => {
    player.queueSongIds = JSON.parse(message.data);
    if (player.queueSongIds.length == 0) {
        currentSong = null;
    }

    UpdateQueueSongs()
});

PamelloEvents.addEventListener("updatedPlayerIsPaused", message => {
    player.isPaused = JSON.parse(message.data);
    UpdatePlayerButtons()
});

PamelloEvents.addEventListener("updatedPlayerQueueIsRandom", message => {
    player.queueIsRandom = JSON.parse(message.data);
    UpdatePlayerModes()
});
PamelloEvents.addEventListener("updatedPlayerQueueIsReversed", message => {
    player.queueIsReversed = JSON.parse(message.data);
    UpdatePlayerModes()
});
PamelloEvents.addEventListener("updatedPlayerQueueIsNoLeftovers", message => {
    player.queueIsNoLeftovers = JSON.parse(message.data);
    UpdatePlayerModes()
});

function LoadUser() {
    GetAuthUser((newUser) => {
        user = newUser;
        if (!user) {
            window.location.replace("/");
            return;
        }

        UpdateUserInfo();

        LoadPlayer();
    }, () => {
        window.location.replace("/");
    });
}
function LoadPlayer() {
    if (user.selectedPlayerId) {
        GetPlayer(user.selectedPlayerId, (newPlayer) => {
            player = newPlayer;
            FullUpdatePlayer();
        });
    }
    else {
        player = null;
        FullUpdatePlayer();
    }
}
function LoadSong(after) {
    let songId;
    if (player && player.queueSongIds.length != 0) {
        songId = player.queueSongIds[player.queuePosition]
        GetSong(songId, (newSong) => {
            currentSong = newSong;
            after();
        });
    }
    else {
        currentSong = null;
        after();
    }
}

function FullUpdatePlayer() {
    UpdatePlayerOptions();
    UpdatePlayerButtons();
    UpdatePlayerModes();
    UpdatePlayerTime();
    UpdatePlayerSongInfo();

    UpdateQueueSongs();
    UpdateQueuePosition();
}

function UpdatePlayerSongInfo() {
    LoadSong(() => {
        if (!currentSong) {
            songCoverElement.style.backgroundImage = "";
            songTitleElement.innerHTML = "";
            songAuthorElement.innerHTML = "";
            return;
        }

        songCoverElement.style.backgroundImage = `url(${currentSong.coverUrl})`;
        songTitleElement.innerHTML = currentSong.title;
        songAuthorElement.innerHTML = currentSong.author;

        UpdateCurrentSongEpisodes();
        UpdatePlayerButtons();
    });
}
function UpdatePlayerTime() {
    if (!currentSong) {
        songTimeContainerElement.style.display = "none";
        return;
    }
    else {
        songTimeContainerElement.style.display = "block";
    }

    songDurationElement.innerHTML = ToStringTime(player.currentSongTimeTotal);
    songTimeSlider.max = player.currentSongTimeTotal;
    if (!isTimeEdit) {
        songTimeElement.innerHTML = ToStringTime(player.currentSongTimePassed);
        songTimeSlider.value = player.currentSongTimePassed;
    }
}
function UpdatePlayerButtons() {
    if (!player) {
        ppButton.disabled = true;
    }
    else {
        ppButton.disabled = false;
        ppButton.innerHTML = (player.isPaused) ? "R" : "P";
    }
    prevButton.disabled = !currentSong;
    nextButton.disabled = !currentSong;
}
function UpdatePlayerModes() {
    if (!player) {
        randomButton.className = "queue-left-controls-button";
        randomButton.disabled = true;
        reversedButton.className = "queue-left-controls-button";
        reversedButton.disabled = true;
        noLeftoversButton.className = "queue-left-controls-button";
        noLeftoversButton.disabled = true;

        return;
    }

    randomButton.disabled = false;
    reversedButton.disabled = false;
    noLeftoversButton.disabled = false;
    randomButton.className = (player.queueIsRandom) ? "queue-left-controls-button green-button" : "queue-left-controls-button";
    reversedButton.className = (player.queueIsReversed) ? "queue-left-controls-button green-button" : "queue-left-controls-button";
    noLeftoversButton.className = (player.queueIsNoLeftovers) ? "queue-left-controls-button green-button" : "queue-left-controls-button";
}
function UpdateUserInfo() {
    userCoverElement.style.backgroundImage = `url(${user.coverUrl})`;
    userNameElement.innerHTML = user.name;
    userDiscordIdElement.innerHTML = user.discordId;
}
function UpdateQueueSongs() {
    queueListElement.innerHTML = "";
    if (!player) return;

    if (player.queueSongIds.length == 0) {
        UpdatePlayerSongInfo();
        UpdatePlayerTime();
        return;
    }
    for (let i = 0; i < player.queueSongIds.length; i++) {
        AddQueueSongElement(i);
        GetSong(player.queueSongIds[i], (newSong) => {
            let songElement = queueListElement.querySelector(`#queue-song-${i}`);
            songElement.querySelector(".queue-song-title").innerHTML = newSong.title;
        })
    }
    UpdateQueuePosition();
}
function UpdateQueuePosition() {
    if (!player) {
        return;
    }
    
    let songElements = queueListElement.querySelectorAll(`.queue-song-container`);
    let decoratorElement = null;

    for (let songElement of songElements) {
        if (songElement.id == `queue-song-${player.queuePosition}`) {
            songElement.className = "queue-song-container queue-current-song-container";
        }
        else {
            songElement.className = "queue-song-container";
        }

        decoratorElement = songElement.querySelector(".queue-song-decorator");
        if (player.nextPositionRequest != null && songElement.id == `queue-song-${player.nextPositionRequest}`) {
            decoratorElement.style.display = "block";
        }
        else {
            decoratorElement.style.display = "none";
        }
    }

    UpdatePlayerSongInfo();
}

function UpdateSelectedSong() {
    if (!selectedSong) {
        selectedSongCoverElement.style.backgroundImage = "none";
        SetPEValue("selected-song-title", "");
        SetPEValue("selected-song-author", "");
        return;
    }

    selectedSongCoverElement.style.backgroundImage = `url(${selectedSong.coverUrl})`;
    SetPEValue("selected-song-title", selectedSong.title);
    SetPEValue("selected-song-author", selectedSong.author);

    UpdateSelectedSongEpisodes();
}

function UpdateSongsList() {
    GetManySongs(0, 20, (songs) => {
        songListElement.innerHTML = "";
        for (let listSong of songs) {
            AddListSongElement(listSong);
        }
    })
}

function UpdateCurrentSongEpisodes() {
    currentSongEpisodesListContainer.innerHTML = "";
    if (!currentSong) return;

    for (let i = 0; i < currentSong.episodeIds.length; i++) {
        AddCurrentSongEpisodeElement(i);
        GetEpisode(currentSong.episodeIds[i], (rEpisode) => {
            let episodeElement = currentSongEpisodesListContainer.querySelector(`#cs-episode-${i}-container`);
            episodeElement.className = `episode-container container-of-episode-${rEpisode.id}`
            episodeElement.querySelector(".episode-title").innerHTML = rEpisode.name;
            episodeElement.querySelector(".episode-time").innerHTML = ToStringTime(rEpisode.start);
            episodeElement.querySelector(".pamello-checkbox").className = "pamello-checkbox " + (!rEpisode.skip ? "pamello-checkbox-active" : "pamello-checkbox-inactive")
        })
    }
}

function UpdateSelectedSongEpisodes() {
    selectedSongEpisodesListContainer.innerHTML = "";
    if (!selectedSong) return;

    for (let i = 0; i < selectedSong.episodeIds.length; i++) {
        AddSelectedSongEpisodeElement(i);
        GetEpisode(selectedSong.episodeIds[i], (rEpisode) => {
            let episodeElement = selectedSongEpisodesListContainer.querySelector(`#ss-episode-${i}-container`);
            episodeElement.className = `episode-container container-of-episode-${rEpisode.id}`
            episodeElement.querySelector(".episode-title").innerHTML = rEpisode.name;
            episodeElement.querySelector(".episode-time").innerHTML = ToStringTime(rEpisode.start);
            episodeElement.querySelector(".pamello-checkbox").className = "pamello-checkbox " + (!rEpisode.skip ? "pamello-checkbox-active" : "pamello-checkbox-inactive")
        })
    }
}

function AddSelectedSongEpisodeElement(episodePosition) {
    selectedSongEpisodesListContainer.innerHTML += GetEpisodeHtml("ss", episodePosition);
}
function AddCurrentSongEpisodeElement(episodePosition) {
    currentSongEpisodesListContainer.innerHTML += GetEpisodeHtml("cs", episodePosition);
}

function GetEpisodeHtml(key, episodePosition) {
    return `
    <div class="episode-container" id="${key}-episode-${episodePosition}-container">
        <div class="episode-title-container">
            <div class="episode-title" onclick="InvokeCommand('PlayerRewindToEpisode', { episodePosition: ${episodePosition} })"></div>
            <button class="episode-title-edit-button">Edit</button>
        </div>
        <div class="episode-time-container">
            <button class="episode-time-edit-button">Edit</button>
            <div class="episode-time"></div>
        </div>
        <div class="episode-checkbox-container">
            <div class="pamello-checkbox pamello-checkbox-active"></div>
        </div>
    </div>`;
}

function AddListSongElement(song) {
    songListElement.innerHTML += `
    <div class="list-song-container" id="list-song-${song.id}" onclick="SelectSong(${song.id})">
        <div class="list-song-cover" style="background-image: url(${song.coverUrl});"></div>
        <span class="list-song-title">${song.title}</span>
        <button class="list-song-add-button" onclick="InvokeCommand('PlayerQueueAddSong', {songId: ${song.id}})">Add</button>
    </div>`
}
function AddQueueSongElement(songPosition) {
    queueListElement.innerHTML += `
    <div class="queue-song-container" id="queue-song-${songPosition}">
        <div class="queue-song-subcontainer queue-song-left-container">
            <div class="queue-song-decorator" style="display: none;">Next</div>
            <div class="queue-song-title" onclick="InvokeCommand('PlayerGoToSong', {songPosition: ${songPosition}, returnBack: false})"></div>
        </div>
        <div class="queue-song-subcontainer queue-song-hidden-buttons">
            <button class="queue-song-hidden-button" onclick="requestNextButtonClick(${songPosition})">Next</button>
            <button class="queue-song-hidden-button" onclick="removeButtonClick(${songPosition})">Remove</button>
        </div>
    </div>`
}
function GetQueueSongElement(songPosition) {
    return 
}

function SelectSong(songId) {
    GetSong(songId, (rsong) => {
        selectedSong = rsong;
        UpdateSelectedSong();
    })
}



function SelectedPlayerOptionChanged() {
    let selectedValue = playerSelectElement.value;

    if (selectedValue == "none") {
        InvokeCommand("PlayerSelect", { playerId: "" }, () => {
            FullUpdatePlayer();
        });
    }
    else if (selectedValue == "new") {
        PlayerOptionSetCreate();
    }
    else {
        InvokeCommand("PlayerSelect", { playerId: selectedValue }, () => {
            FullUpdatePlayer();
        });
    }
}

function PlayerOptionSetSelect() {
    playerSelectContainerElement.style.display = "grid";
    playerCreateContainerElement.style.display = "none";
}
function PlayerOptionSetCreate() {
    playerSelectContainerElement.style.display = "none";
    playerCreateContainerElement.style.display = "grid";
}

function CreatePlayerClick() {
    if (playerCreateInput.value) {
        InvokeCommand("PlayerCreate", { playerName: playerCreateInput.value }, () => {
            PlayerOptionSetSelect();
            FullUpdatePlayer();
        });
    }
}
function CreateCancelPlayerClick() {
    playerCreateInput.value = player?.id;
    PlayerOptionSetSelect();
}

function UpdatePlayerOptions() {
    let optionNone = document.createElement("option");
    optionNone.value = "none";
    optionNone.text = "-";
    let optionNew = document.createElement("option");
    optionNew.value = "new";
    optionNew.text = "New...";

    GetManyPlayers(0, 10, (playersResponse) => {
        playerSelectElement.innerHTML = "";
        playerSelectElement.add(optionNone);
        for (let playerResponse of playersResponse) {
            let option = document.createElement("option");
            option.value = playerResponse.id;
            option.text = playerResponse.name;
            option.selected = playerResponse.id == player?.id;

            playerSelectElement.add(option);
        }
        playerSelectElement.add(optionNew);
    });
}

function ToStringTime(seconds) {
    let h = Math.floor(seconds / 3600);
    let m = Math.floor((seconds - h * 3600) / 60);
    let s = seconds % 60;
    return `${(h) ? (h + ":") : ("")}${(m > 9) ? ("") : ("0")}${(m) ? (m) : ("0")}:${(s > 9) ? ("") : ("0")}${(s) ? (s) : ("0")}`
    return ((h) ? (`${h}:${m}:${s}`) : (`${m}:${s}`))
}

/*
let user = null;
let player = null;
let song = null;

let songCoverElement = document.getElementById("song-cover");
let songTitileElement = document.getElementById("song-title");
let songAuthorElement = document.getElementById("song-author");
let songTimeElement = document.getElementById("song-time");
let songDurationElement = document.getElementById("song-duration");

let queueListElement = document.getElementById("queue-list");

FirstLoadUser();

function FirstLoadUser() {
    GetUser(1, (newUser) => {
        user = newUser;
        user.selectedPlayerId = 1;
        FirstLoadPlayer();
    });
}
function FirstLoadPlayer() {
    if (user.selectedPlayerId) {
        GetPlayer(user.selectedPlayerId, (newPlayer) => {
            player = newPlayer;
            LoadSong();
            UpdateQueue();
        });
    }
}
function LoadSong() {
    let songId = player?.queueSongIds[player?.queuePosition]
    if (songId != null) {
        GetSong(songId, (newSong) => {
            song = newSong;
            Update();
        });
    }
}

PamelloEvents.addEventListener("updatedPlayerQueuePosition", message => {
    player.queuePosition = message.data;
    LoadSong()
    UpdateQueue()
});
PamelloEvents.addEventListener("updatedPlayerCurrentSongTimePassed", message => {
    player.currentSongTimePassed = message.data;
    Update()
});
PamelloEvents.addEventListener("updatedPlayerCurrentSongTimeTotal", message => {
    player.currentSongTimeTotal = message.data;
    Update()
});
PamelloEvents.addEventListener("updatedPlayerQueueSongIds", message => {
    player.queueSongIds = message.data;
    UpdateQueue()
});

function Update() {
    songCoverElement.style.backgroundImage = `url(${song.coverUrl})`;
    songTitileElement.innerHTML = song.title;
    songAuthorElement.innerHTML = song.author;

    songTimeElement.innerHTML = player.currentSongTimePassed;
    songDurationElement.innerHTML = player.currentSongTimeTotal;
}

function UpdateQueue() {
    ClearQueueSongs();
    let len = player.queueSongIds.length;
    for (let i = 0; i < len; i++) {
        AddQueueSong(i, player.queueSongIds[i], player.queuePosition == i);
    }
}

function AddQueueSong(songPosition, songId, isCurrent) {
    console.log(`adding ${songId}`);
    queueListElement.innerHTML += `<div class="queue-song${(isCurrent ? " queue-current-song" : "")}" id="queue-song-${songPosition}">Loading...</div>`
    GetSong(songId, (newSong) => {
        $(`#queue-song-${songPosition}`).text(newSong.title);
    });
}

function ClearQueueSongs() {
    queueListElement.innerHTML = ""
}
*/

/*
let currentUser = null;
let selectedPlayer = null;

PamelloEvents.addEventListener("updatedPlayerIsPaused", message => updatedPlayerIsPaused(message.data));
PamelloEvents.addEventListener("updatedPlayerCurrentSongTimePassed", message => updatedPlayerCurrentSongTimePassed(message.data));
PamelloEvents.addEventListener("updatedPlayerCurrentSongTimeTotal", message => updatedPlayerCurrentSongTimeTotal(message.data));
PamelloEvents.addEventListener("updatedPlayerQueuePosition", message => updatedPlayerQueuePosition(message.data));
PamelloEvents.addEventListener("updatedPlayerNextPositionRequest", message => updatedPlayerNextPositionRequest(message.data));

let isPausedElement = document.getElementById("is-paused");
let songTimeElement = document.getElementById("song-time");
let songDurationElement = document.getElementById("song-duration");
let queuePositionElement = document.getElementById("queue-position");
let requestedPositionElement = document.getElementById("requested-position");

function updatedPlayerIsPaused(value) {
    selectedPlayer.isPaused = value;
    UpdatePlayerValues();
}
function updatedPlayerCurrentSongTimePassed(value) {
    selectedPlayer.currentSongTimePassed = value;
    UpdatePlayerValues();
}
function updatedPlayerCurrentSongTimeTotal(value) {
    selectedPlayer.currentSongTimeTotal = value;
    UpdatePlayerValues();
}
function updatedPlayerQueuePosition(value) {
    selectedPlayer.queuePosition = value;
    UpdatePlayerValues();
}
function updatedPlayerNextPositionRequest(value) {
    selectedPlayer.nextPositionRequest = value;
    UpdatePlayerValues();
}
function updatedPlayerQueueSongIds(value) {
    console.log(`testestest: ${value}`);
    selectedPlayer.queueSongIds = value;
    UpdatePlayerValues();
}

let playerSelectElement = document.getElementById("player-select");

function LoadUser(user) {
    GetAuthUser((user) => {
        currentUser = user;
        currentUser.selectedPlayerId = 1;
        LoadPlayer();
    });
}
function LoadPlayer(player) {
    if (currentUser.selectedPlayerId) {
        GetPlayer(currentUser.selectedPlayerId, (player) => {
            selectedPlayer = player;
            LoadFinal();
        });
    }
    else {
        LoadFinal();
    }
}
function LoadFinal() {
    UpdatePlayers();
    UpdatePlayerValues();
}

LoadUser();

function UpdatePlayers() {
    let len = playerSelectElement.length;
    for (let i = 0; i < len; i++) {
        playerSelectElement.remove(0);
    }
    
    let option = document.createElement("option");
    option.classList = "option-none";
    option.text = "None";
    playerSelectElement.add(option);
    GetManyPlayers(0, 10, (players) => {
        $.each(players, function (index, player) {
            option = document.createElement("option");
            option.text = player.name;
            playerSelectElement.add(option);
            console.log(`${player.id} and ${selectedPlayer.id}`);
            if (player.id == selectedPlayer.id) {
                console.log(`true ${playerSelectElement.selectedIndex}, ${index}`);
                playerSelectElement.selectedIndex = 1;
                console.log(`true ${playerSelectElement.selectedIndex}`);
            }
        });
    });
}

function UpdatePlayerValues() {
    isPausedElement.innerHTML = selectedPlayer.isPaused
    songTimeElement.innerHTML = selectedPlayer.currentSongTimePassed
    songDurationElement.innerHTML = selectedPlayer.currentSongTimeTotal
    queuePositionElement.innerHTML = selectedPlayer.queuePosition
    requestedPositionElement.innerHTML = selectedPlayer.nextPositionRequest
}

let selectPlayers = document.getElementById("player-select");
let selectedPlayerId = null;

//UpdatePlayers();

PamelloEvents.addEventListener("playerCreated", (event) => { UpdatePlayers(); });
PamelloEvents.addEventListener("updatedSelectedPlayer", (event) => {
    selectedPlayerId = event.data;
    UpdatePlayers();
});

function UpdatePlayers() {
    console.log(`le ${selectPlayers.length}`);
    let len = selectPlayers.length;
    for (let i = 0; i < len; i++) {
        console.log(`removing ${i}`);
        selectPlayers.remove(0);
    }

    console.log(selectPlayers);

    let option = document.createElement("option");
    option.classList = "option-none";
    option.text = "None";
    selectPlayers.add(option);
    GetManyPlayers(0, 10, (players) => {
        $.each(players, function (index, player) {
            console.log(`adding ${index}`);
            if (player.id == selectedPlayerId) {
                selectPlayers.selectedIndex = index + 1;
            }
            option = document.createElement("option");
            option.text = player.name;
            selectPlayers.add(option);
        });
    });
}

*/