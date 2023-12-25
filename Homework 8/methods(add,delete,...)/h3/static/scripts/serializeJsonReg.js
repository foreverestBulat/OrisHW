function serializeJsonReg() {
    var login = document.getElementById("login").value;
    var password = document.getElementById("password").value;
    var repeatpassword = document.getElementById("repeatpassword").value;

    var reg = { Login: login, Password: password, RepeatPassword: repeatpassword };

    var jsonString = JSON.stringify(reg);
    console.log(jsonString);

    var xhr = new XMLHttpRequest();
    xhr.open("POST", "send", true);
    xhr.setRequestHeader("Content-Type", "application/json");
    xhr.send(jsonString);
    location.reload();
  }