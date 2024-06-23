let user = null;
let player = null;
let song = null;

let userCoverElement = document.getElementById("user-cover");
let userNameElement = document.getElementById("user-name");
let userDiscordIdElement = document.getElementById("user-discordid");

let songCoverElement = document.getElementById("song-cover");
let songTitleElement = document.getElementById("song-title");
let songAuthorElement = document.getElementById("song-author");

let songTimeElement = document.getElementById("song-time");
let songDurationElement = document.getElementById("song-duration");
let songTimeSlider = document.getElementById("player-slider");

let queueListElement = document.getElementById("queue-list");

let randomButton = document.getElementById("is-random-queue-button");
let reversedButton = document.getElementById("is-reversed-queue-button");
let noLeftoversButton = document.getElementById("is-no-leftovers-queue-button");

let ppButton = document.getElementById("pp-button");

FirstLoadUser();

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
    if (player.nextPositionRequest == null) {
        InvokeCommand("PlayerQueueRequestNext", { position: songQueuePosition })
    }
    else {
        InvokeCommand("PlayerQueueRequestNext", { position: "" })
    }
}

PamelloEvents.addEventListener("updatedPlayerCurrentSongTimePassed", message => {
    player.currentSongTimePassed = JSON.parse(message.data);
    UpdateSongTime()
});
PamelloEvents.addEventListener("updatedPlayerCurrentSongTimeTotal", message => {
    player.currentSongTimeTotal = JSON.parse(message.data);
    UpdateSongTime()
});
PamelloEvents.addEventListener("updatedPlayerQueuePosition", message => {
    player.queuePosition = JSON.parse(message.data);
    UpdateSongInfo()
    UpdateQueuePosition();
});
PamelloEvents.addEventListener("updatedPlayerNextPositionRequest", message => {
    player.nextPositionRequest = JSON.parse(message.data);
    UpdateQueuePosition();
});
PamelloEvents.addEventListener("updatedPlayerQueueSongIds", message => {
    player.queueSongIds = JSON.parse(message.data);
    UpdateQueueSongs()
});

PamelloEvents.addEventListener("updatedPlayerIsPaused", message => {
    player.isPaused = JSON.parse(message.data);
    UpdatePlayerInfo()
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

function FirstLoadUser() {
    GetUser(1, (newUser) => {
        user = newUser;

        FirstLoadPlayer();
    });
}
function FirstLoadPlayer() {
    if (user.selectedPlayerId) {
        GetPlayer(user.selectedPlayerId, (newPlayer) => {
            player = newPlayer;
            FirstLoadSong();
        });
    }
    else {
        LoadFinal();
    }
}
function FirstLoadSong() {
    let songId = player?.queueSongIds[player?.queuePosition]
    if (songId != null) {
        GetSong(songId, (newSong) => {
            song = newSong;
            LoadFinal();
        });
    }
}
function LoadSong() {
    let songId = player?.queueSongIds[player?.queuePosition]
    if (songId != null) {
        GetSong(songId, (newSong) => {
            song = newSong;
            UpdateSongInfo();
        });
    }
}
function LoadFinal() {
    UpdateUserInfo();
    UpdateSongInfo();
    UpdateSongTime();
    UpdateQueueSongs();
    UpdateQueuePosition();
    UpdatePlayerModes();
}

function UpdateSongInfo() {
    songCoverElement.style.backgroundImage = `url(${song.coverUrl})`;
    songTitleElement.innerHTML = song.title;
    songAuthorElement.innerHTML = song.author;
}
function UpdateSongTime() {
    songTimeElement.innerHTML = player.currentSongTimePassed;
    songDurationElement.innerHTML = player.currentSongTimeTotal;
    songTimeSlider.max = player.currentSongTimeTotal;
    songTimeSlider.value = player.currentSongTimePassed;
}
function UpdatePlayerInfo() {
    ppButton.innerHTML = (player.isPaused) ? "R" : "P";
}
function UpdatePlayerModes() {
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
    LoadSong();

    let songElements = queueListElement.querySelectorAll(`.queue-song-container`);
    let decoratorElement = null;

    for (let songElement of songElements) {
        decoratorElement = songElement.querySelector(".queue-song-decorator");
        if (songElement.id == `queue-song-${player.queuePosition}`) {
            decoratorElement.style.display = "block";
            decoratorElement.innerHTML = "P"
        }
        else if (player.nextPositionRequest != null && songElement.id == `queue-song-${player.nextPositionRequest}`) {
            decoratorElement.style.display = "block";
            decoratorElement.innerHTML = "N"
        }
        else {
            decoratorElement.style.display = "none";
        }
    }
}

function AddQueueSongElement(songPosition) {
    queueListElement.innerHTML += `<div class="queue-song-container" id="queue-song-${songPosition}">
        <div class="queue-song-subcontainer queue-song-left-container">
            <div class="queue-song-decorator">P</div>
            <div class="queue-song-title">Song title</div>
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