﻿const hubConnection = new signalR.HubConnectionBuilder()
    .withUrl("/Game")
    .build();
var GameTag = "";
hubConnection.on("GetGames", function (data) {
    //$("#divRegister").hide();
    var div = document.getElementById('games');
    while (div.firstChild) {
        div.removeChild(div.firstChild);
    }
    for (i = 0; i < data.length; i++) {
        var email = data[i].player1.email;
        var name = data[i].name;
        var tbody = document.getElementById("games");
        var row = document.createElement("TR")
        var td1 = document.createElement("TD")
        td1.appendChild(document.createTextNode(data[i].name))
        var td2 = document.createElement("TD")
        var td3 = document.createElement("TD")
        var btn = document.createElement("button")
        btn.type = 'button';
        btn.setAttribute("id", "btnConnect");
        btn.setAttribute("onclick", "connect(this)");
        btn.setAttribute("class", "btn btn-primary");
        btn.setAttribute("value", email + ' ' + name);
        btn.append("connect");
        td2.append(btn);

        td3.append(email)
        row.appendChild(td1);
        row.appendChild(td3);
        row.appendChild(td2);
        tbody.appendChild(row);
    }
});
hubConnection.on("FindGames", function (data) {
    var div = document.getElementById('games');
    while (div.firstChild) {
        div.removeChild(div.firstChild);
    }
    for (i = 0; i < data.length; i++) {
        var email = data[i].player1.email;
        var name = data[i].tag;
        var tbody = document.getElementById("games");
        var row = document.createElement("TR")
        var td1 = document.createElement("TD")
        td1.appendChild(document.createTextNode(data[i].name))
        var td2 = document.createElement("TD")
        var td3 = document.createElement("TD")
        var btn = document.createElement("button")
        btn.type = 'button';
        btn.setAttribute("id", "btnConnect");
        btn.setAttribute("onclick", "connect(this)");
        btn.setAttribute("class", "btn btn-primary");
        btn.setAttribute("value", email + ' ' + name);
        btn.append("connect");
        td2.append(btn);
        td3.appendChild(document.createTextNode(email))
        row.appendChild(td1);
        row.appendChild(td3);
        row.appendChild(td2);
        tbody.appendChild(row);
    }
});

hubConnection.on("Redirect", function (data) {
    window.location.href = `/Game/Play?id=${data}`
});
hubConnection.on('getConnected', function (data) {
    hubConnection.invoke("GetListGame");
});
document.getElementById("btnRegister").addEventListener("click", function (e) {
    let name = document.getElementById("TagReg").value;
    hubConnection.invoke("RegisterGame", name, email);
});
document.getElementById("btnFind").addEventListener("click", function (e) {
    let tag = document.getElementById("TagFind").value;
    hubConnection.invoke("FindGame", tag);
});

function connect(button) {
    hubConnection.invoke("ConnectGame", button.value + " "+email);
}

hubConnection.start();