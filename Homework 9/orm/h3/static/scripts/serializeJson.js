function serializeJsonUser() {
    var login = document.getElementById("login").value;
    var password = document.getElementById("password").value;

    var account = { Login: login, Password: password };

    var jsonString = JSON.stringify(account);
    console.log(jsonString);

    var xhr = new XMLHttpRequest();
    xhr.open("POST", "send", true);
    xhr.setRequestHeader("Content-Type", "application/json");
    xhr.send(jsonString);
    location.reload();
}