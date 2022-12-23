const accountLogin = 'account/login';

function login() {
    let headers = {
        "Content-Type": "application/json",
        "Accept": "application/json",
    }
    
    fetch(accountLogin, {
        method: "POST",
        headers: headers,
        body: JSON.stringify({
            Account:$("#account").val(),
            Password:$("#password").val()
        })
    })
        .then(response => response.json())
        .then(data => setCookie(data))
        .catch(error => console.error('Unable to login.', error));
}

function setCookie(data) {
    const d = new Date();
    d.setTime(d.getTime() + (30*60*1000));
    let expires = "expires="+ d.toUTCString();
    document.cookie = "loginKey=" + data["key"] + ";" + expires + ";path=/";
    
    if (data["key"] != null) {
        window.location.href = '/machine.html';
    }
    else {
        alert("Login Failed");
    }
}
