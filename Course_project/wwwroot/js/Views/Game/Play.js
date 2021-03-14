document.getElementById("divInfo").hidden = true;
const hubConnection = new signalR.HubConnectionBuilder()
    .withUrl("/Game")
    .build();
hubConnection.on('waitingForMove', function (data) {
    document.getElementById("divInfo").hidden = false;
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
    hubConnection.invoke("GetCardGame", "@User.Identity.Name");
});
function Move(id) {
    //if (span.hasClass("notAvailable")) {
    //    return;
    //}
    document.getElementById("divInfo").hidden = true;

    hubConnection.invoke('MakeAMove', id, "@User.Identity.Name");
}
hubConnection.start();