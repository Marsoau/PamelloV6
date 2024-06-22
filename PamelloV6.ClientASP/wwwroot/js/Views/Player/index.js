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