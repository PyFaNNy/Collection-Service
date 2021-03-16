const hubConnection = new signalR.HubConnectionBuilder()
    .withUrl("/Game")
    .build();
document.getElementById("wait").hidden = true;
hubConnection.on('waitingForOpponent', function (data) {
    document.getElementById("wait").hidden = !document.getElementById("wait").hidden;
    document.getElementById("run").hidden = !document.getElementById("run").hidden;
    console.log('waitingForOpponent');
});
hubConnection.on('moveMade', data => {
    for (i = 0; i < data.length; i++) {
        document.getElementById(i).src = "/images/TicTacToe/" + data[i] + ".png";
    }
});

hubConnection.on('GameOver', data => {
    document.getElementById("divInfo").hidden = false;
    document.getElementById("divInfo").append(data)
});
hubConnection.on('RedirectHome', data => {
    window.location.href = `/Game/Games`
});
hubConnection.on('getConnected', function (data) {
    hubConnection.invoke("GetCardGame", email);
});
function Move(id) {
    document.getElementById("divInfo").hidden = true;

    hubConnection.invoke('MakeAMove', id, email);
}
hubConnection.start();