function serializeJsonUser() {
    var login = document.getElementById("login").value;
    var password = document.getElementById("password").value;

    var account = { Login: login, Password: password };

    var jsonString = JSON.stringify(account);
    console.log(jsonString);

    var xhr = new XMLHttpRequest();
    xhr.open("POST", "add", true);
    xhr.setRequestHeader("Content-Type", "application/json");
    xhr.send(jsonString);
    location.reload();
  }



// function serializeJsonUser() {
//   var login = document.getElementById("login").value;
//   var password = document.getElementById("password").value;

//   var account = { Login: login, Password: password };

//   var jsonString = JSON.stringify(account);
//   console.log(jsonString);

//   var xhr = new XMLHttpRequest();
//   xhr.open("POST", "send", true);
//   xhr.setRequestHeader("Content-Type", "application/json");
  
//   var url = "/users"
//   xhr.onload = function () {
//     if (xhr.status === 200) {
//       if (xhr.responseURL === url) {
//         console.log("got what we wanted, hooray!")
//       } else {
//         console.log("boo, we were redirected from", url, "to", xhr.responseURL)
//       }

//       location.href = xhr.responseURL;
//     }
//   }
  
//   xhr.send(jsonString);
// }