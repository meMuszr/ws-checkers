/// <reference path="jquery-3.1.1.js" />
var Init = function () {
    var Username = "";
    var usersElement = $('ul', 'div.users');
    var usersContainer = $('div.users');
    var gameContainer = $('div.game');
    var gameId = null;
    var isTurn = null;
    var color = null;
    var itemHighlight = null;
    var loggedIn = false;
    var client = null;
    var sendPoint = {};
    var arrayUsers = [];
    var gameHTML = '';
    usersContainer.hide();
    gameContainer.hide();
    $('form.connect').submit(function (e) {
        e.preventDefault();
        $('div.load').toggle();
        startWS($('input[name="username"]', e.form).val());
    });
    var startWS = function (user) {
        client = new WebSocket('ws://localhost:5000/ws');
        client.onclose = function () {
            $('form.connect').show();
            usersContainer.hide();
            loggedIn = false;
        };
        client.onopen = function (evt) {
            $('div.load').toggle();
            client.send("login=" + user);
        };
        client.onmessage = function (evt) {
            var data = { message: evt.data.split('=') };
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
                                return e === data.object;
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
                    if (data.object.constructor === Array) {
                        $.each(data.object, function (i, val) {
                            arrayUsers.splice(arrayUsers.indexOf(val), 1);
                            $("li", usersElement).filter(function () {
                                return $(this).data('name') === val;
                            }).remove();
                        });
                    }
                    arrayUsers.splice(arrayUsers.indexOf(data.object), 1);
                    $("li", usersElement).filter(function () {
                        return $(this).data('name') === data.object;
                    }).remove();
                    break;
                case 'createGame':
                    if (data.object.constructor === Array) {
                        $.each(data.object, function (ind, val) {
                            arrayUsers.splice(arrayUsers.indexOf(val), 1);
                            $("li", usersElement).filter(function () {
                                return $(this).data('name') === val;
                            }).remove();
                        });
                 //       ('section', gameContainer).append(gameHTML);
                        usersContainer.hide();
                        gameContainer.show();
                    }
                    break;
                case 'gameId':
                    gameId = data.object;
                    break;
                case 'isTurn':
                    isTurn = data.object.isTurn;
                    color = data.object.Color;
                    break;
                case 'move':
                    if (itemHighlight !== null) itemHighlight.removeClass('highlight');
                    if (data.object.UpdateMove) {
                        var gameBoard = $('section > div', gameContainer);
                        var oldPiece = gameBoard.eq(data.object.OldPoint.Y).children().eq(data.object.OldPoint.X);
                        var newPiece = gameBoard.eq(data.object.NewPoint.Y).children().eq(data.object.NewPoint.X);

                        if (data.object.RemovePoint) {
                            gameBoard.eq(data.object.RemovePoint.Y).children().eq(data.object.RemovePoint.X).html('');
                        }
                        if (data.object.GameState === 3) {
                            $('header > h2', 'div.game').text('game over :(');
                        }
                        newPiece.html(oldPiece.html());
                        oldPiece.html('');
                        isTurn = !isTurn;

                    }
                    break;

            }
        };
        $('input[name="close"]', 'div.users > header').on('click', function (e) {
            client.close();
            arrayUsers = [];
            usersElement.children().remove();


        });
        $('ul', usersContainer).on('click', 'li', function (e) {
            if (Username === e.currentTarget.innerText || client === null) return;
            if (loggedIn) client.send("newGame=" + e.currentTarget.innerText);

        });
    };

    $("div > div ", gameContainer).on('click', function (e) {
        if (!isTurn) return;
        if ($(this).index() % 2 === $(this).parent().index() % 2) return;
        console.log($(this).index() + "," + $(this).parent().index());
        var elementClicked = $(e.currentTarget);
        if (itemHighlight === null) {
            if (elementClicked.find('circle').attr('fill') === 'red' && color === 'White'
                || elementClicked.find('circle').attr('fill') === 'green' && color === 'Black') {
                sendPoint.OldPoint = {
                    X: $(this).index(),
                    Y: $(this).parent().index()
                };
                itemHighlight = elementClicked;
                itemHighlight.addClass('highlight');
            }
        }
        else {
            if (elementClicked.find('circle').attr('fill') === 'red' && color === 'White'
                || elementClicked.find('circle').attr('fill') === 'green' && color === 'Black') {
                sendPoint.OldPoint = {
                    X: $(this).index(),
                    Y: $(this).parent().index()
                };
                itemHighlight.removeClass('highlight');
                itemHighlight = elementClicked;
                itemHighlight.addClass('highlight');
            }
            else if (elementClicked.not(":has(*)").length > 0) {
                sendPoint.NewPoint = {
                    X: $(this).index(),
                    Y: $(this).parent().index()
                };
                client.send("makeMove=" + JSON.stringify(sendPoint));
            }


        }


    });

};

$(Init);