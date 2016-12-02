/// <reference path="jquery-3.1.1.js" />
var Init = function () {
    var Username = "";
    var usersElement =$('section','div.users');
    var usersContainer = $('div.users');
    usersContainer.hide();
    $('form.connect').submit(function (e) {
        e.preventDefault();
        $('div.load').toggle();
        startWS($('input[name="username"]', e.form).val());
    });
    var startWS = function (user) {
        var client = new WebSocket('ws://localhost:5000/ws');
        client.onopen = () => {
            $('div.load').toggle();
            client.send("login:" + user);
        };
        client.onmessage = (evt) => {
            var data = {message: evt.data.split(':')};
            data.method = data.message[0];
            data.object = JSON.parse(data.message[1]);
            console.log(data.object);
            switch (data.method) {
                case 'login':
                    $.each(data.object, function (ind, val) {
                        usersElement.append('<span data-name="' + val  + '">' + val + '</span>');
                    });
                    $('form.connect').hide();
                    usersContainer.show();
                    break;
            }
        };

    }

};



$(Init);