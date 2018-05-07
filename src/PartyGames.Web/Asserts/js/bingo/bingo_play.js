Bingo.play = Bingo.play || {};

Bingo.play.getBingoKey = function() {

    var bingokey = request.card || $.cookie("bingokey");
    return bingokey;

};

//init
Bingo.play.init = function() {

    $("body").addClass("play");

    $("#btnSubmitName").on("click",
        function(e) {
            e.preventDefault();
            Bingo.play.addPlayer();
        });

    $("#btnChangeName").on("click",
        function(e) {
            e.preventDefault();
            var name = $.trim($("#txtChangeName").val());
            Bingo.play.changeName(name);
        });


    //mock play
    if (request.newkey === "true") {

        $(".enter-your-name").show();

        $("#txtName").val(decodeURIComponent(request.name));
        $("#txtPassword").val(request.pwd);

        $("#btnSubmitName").trigger("click");

    } else {

        var bingokey = Bingo.play.getBingoKey();

        if (bingokey) {
            Bingo.play.getPlayer(bingokey);
        } else {
            $(".enter-your-name").show();
        }
    }

    $(".bingo-loading").hide();
};

//createBingoBox
Bingo.play.createBingoBox = function(game, player) {

    //set key
    $(".bingo-gamekey").html(player.Key);

    //set name
    $(".bingo-no-box .card-name").html("<span class='name'>" + player.CardName + "</span>");

    var $link = $("<a href='#' class='edit-name'><i class='fas fa-pencil-alt fa-xs'></i></a>");

    $link.on("click",
        function(e) {
            e.preventDefault();

            $(".bingo-no-box").hide();
            $(".bingo-change-name").show();

            $("#txtChangeName").val(player.Name);
            $("#txtChangeName").get(0).focus();
        });

    $(".bingo-no-box .card-name").append($link);

    var nos = player.CardNumbers;

    $(".enter-your-name").hide();
    $(".bingo-no-box").show();

    //clear all
    $(".bingo-no-box .inner").html("");

    $.each(nos,
        function(idx, no) {

            var $box = $("<div class='bingoBox'><div class='bingoItem'></div></div>");
            var $inner = $("<div class='bingoNumber' />");

            //$inner.html("<i class='fa fa-star'></i>");
            if (no === 0)
                $inner.html("<img src='/Asserts/images/Feng_Logo_100.jpg' style='width: 70%;'></img>");
            else
                $inner.html(no).attr("data-val", no);

            if (game.RolledNumbers.indexOf(no) > -1)
                $inner.addClass("active");

            $(".bingoItem", $box).append($inner);
            $(".bingo-no-box .inner").append($box);
        });

    Bingo.play.startReceiveLiveData();
};

//addPlayer
Bingo.play.addPlayer = function() {

    if (Bingo.isLoading())
        return false;

    $.ajax({
            url: $.api("addPlayer"),
            type: "POST",
            data: {
                name: $("#txtName").val(),
                password: $("#txtPassword").val()
            },
            beforeSend: function() {
                Bingo.startLoading();
            }
        })
        .done(function(data) {

            if (!Bingo.isValidResponse(data))
                return;

            $.cookie("bingokey", data.Player.Key, { expires: 7 });

            Bingo.play.createBingoBox(data.Game, data.Player);
        })
        .always(function() {
            Bingo.stopLoading();
        });

    return false;
};

//change name
Bingo.play.changeName = function(name) {

    if (Bingo.isLoading())
        return false;

    $.ajax({
            url: $.api("changeName"),
            data: {
                name: name,
                key: Bingo.play.getBingoKey
            },
            type: "POST"
        })
        .done(function(data) {
            if (data.Player) {
                $(".bingo-no-box .card-name .name").html(data.Player.CardName);
            }
        })
        .always(function() {
            Bingo.stopLoading();
            $(".bingo-no-box").show();
            $(".bingo-change-name").hide();

        });
};

//getPlayer
Bingo.play.getPlayer = function(key) {

    if (Bingo.isLoading())
        return false;

    $.ajax({
            url: $.api("getPlayer"),
            type: "POST",
            data: {
                key: key
            },
            beforeSend: function() {
                Bingo.startLoading();
            }
        })
        .done(function(data) {

            //if (!Bingo.isValidResponse(data))
            //return;

            if (!data.Success) {
                $.removeCookie("bingokey");
                //window.location.reload();
                window.location.href = "/bingo/play";
                return;
            }

            Bingo.play.createBingoBox(data.Game, data.Player);
        })
        .always(function() {
            Bingo.stopLoading();
        });

};

//startReceiveLiveData
Bingo.play.startReceiveLiveData = function() {

    if (!$(".bingo-no-box").is(":visible"))
        return;

    if ($(".bingo-no-box").data("interval")) {
        clearInterval($(".bingo-no-box").data("interval"));
    }

    var interval = setInterval(function() {

            //if (Bingo.isLoading())
            //    return false;

            var bingoKey = Bingo.play.getBingoKey();

            $.ajax({
                    url: $.api("liveData"),
                    type: "POST",
                    data: {
                        key: bingoKey
                    }
                })
                .done(function(data) {

                    if (!data.Success) {
                        $.removeCookie("bingokey");
                        window.location.href = "/bingo/play?error=invalid_player";
                        return;
                    }

                    $(".bingoNumber").removeClass("active");

                    $.each(data.Numbers,
                        function(idx, no) {
                            $(".bingoNumber[data-val='" + no + "']").addClass("active");
                        });

                })
                .always(function() {
                    //Bingo.stopLoading();
                });

        },
        5000);

    $(".bingo-no-box").data("interval", interval);
};