/// <reference path="jquery-3.1.1.js" />
var Init = function () {
    var Username = "";
    var usersElement = $('ul', 'div.users');
    var usersContainer = $('div.users');
    var loggedIn = false;
    var arrayUsers = [];
    usersContainer.hide();
    $('form.connect').submit(function (e) {
        e.preventDefault();
        $('div.load').toggle();
        startWS($('input[name="username"]', e.form).val());
    });
    
    var startWS = function (user) {
        var client = new WebSocket('ws://localhost:5000/ws');
        client.onclose = function () {
            $('form.connect').show();
            usersContainer.hide();
            loggedIn = false;
        };
        client.onopen = function(evt) {
            $('div.load').toggle();
            client.send("login:" + user);
        };
        client.onmessage = function(evt) {
            var data = { message: evt.data.split(':') };
            data.method = data.message[0];
            data.object = JSON.parse(data.message[1]);
            switch (data.method) {
                case 'login':
                    if (data.object.constructor === Array) {
                        $.each(data.object, function (ind, val) {
                            if (arrayUsers.filter(function (e) {
                                return e === val;
                            }).length === 0) {
                                arrayUsers.push(val);
                                usersElement.append('<li data-name="' + val + '">' + val + '</li>');
                            }
                        });
                    }
                    else {
                        if (arrayUsers.filter(function (e) {
                                return e === data.object
                        }).length === 0) {
                            arrayUsers.push(data.object);
                            usersElement.append('<li data-name="' + data.object + '">' + data.object + '</li>');
                        }
                    }
                    if (!loggedIn) {
                        $('form.connect').hide();
                        usersContainer.show();
                        loggedIn = true;
                    }
                    break;
                case 'removelogin':
                    arrayUsers.splice(arrayUsers.indexOf(data.object), 1);
                    $("li", usersElement).filter(function () {
                        return $(this).data('name') === data.object;
                    }).remove();
                    break;
            }
        };
        $('input[name="close"]', 'div.users > header').on('click', function (e) {
            client.close();
            arrayUsers = [];
            usersElement.children().remove();


        });
    }
};

$(Init);