var Bingo = Bingo || {};

Bingo.startLoading = function () {
    $("body").addClass("loading");
};

Bingo.stopLoading = function() {
    $("body").removeClass("loading");
};

Bingo.isLoading = function() {
    return $("body").hasClass("loading");
};

Bingo.ajaxCustomError = function(response) {

    if (!response)
        return false;

    if (response.Success === false) {
        //alert message
        if (response.Message) {
            alert(response.Message.replace(new RegExp("<br />", "g"), "\n"));
        } else if (response.Messages) {
            alert(response.Messages.join("\n"));
        }

        //redirect to another page
        if (response.Redirect) {
            window.location.href = response.Redirect;
        }

        return false;
    }

    return true;
};

Bingo.isValidResponse = function(response) {

    if (!response)
        return true;

    if (response.Success === false) {
        Bingo.ajaxCustomError(response);
        return false;
    }


    return true;
};
Bingo.dashboard = Bingo.dashboard || {};

var aAudio;

Bingo.dashboard.init = function() {

    aAudio = new Audio("/Asserts/mp3/Bingo-Draw2.mp3");

    setInterval(function() {
            $("div.name[data-active=true]").toggleClass("active");
        },
        500);

    $("body").addClass("dashboard");

    $.ajax({
            url: $.api("liveData"),
            type: "POST",
            data: { key: "dashboard" }
        })
        .done(function(data) {
            if (data.Success) {

                //col numbers
                $(".cols .no").removeClass("active");
                $.each(data.Numbers,
                    function(idx, no) {
                        $(".cols .no[data-val='" + no + "']").addClass("active");
                    });

                Bingo.dashboard.getPlayerlist();

                if (data.Numbers.length === 0) {
                    var timer = setInterval(Bingo.dashboard.getPlayerlist, 10000);
                    $(".player-list").data("timer", timer);
                }
            }
        });

    $(".btn-roll").on("click",
        function(e) {
            e.preventDefault();
            Bingo.dashboard.roll();
        });
};

Bingo.dashboard.getPlayerlist = function() {

    var $playerList = $(".player-list");
    var $championList = $(".champion-list");

    $("tr.data", $playerList).remove();
    $("div.name", $championList).remove();

    //player list
    $.ajax({
            url: $.api("playerList"),
            type: "POST"
        })
        .done(function(data) {

            if (data.Success) {

                $.each(data.RankingList,
                    function(idx, player) {
                        var $tr = $("<tr class='data' />");

                        $tr.append("<td>" + player.Ranking + "</td>");
                        $tr.append("<td>" + player.CardNo + "</td>");

                        var $name = $("<span class='name'>" + player.Name + "</span>");

                        $name.on("click",
                            function(e) {
                                e.preventDefault();
                                var link = $.api("play");
                                window.open(link + "?card=" + player.Key,
                                    "carddetails",
                                    "location=yes,height=750,width=520,scrollbars=no,status=yes");
                            });

                        var $nameTd = $("<td />");
                        $nameTd.append($name);
                        $tr.append($nameTd);

                        if (player.BingoMark1 <= 1) {
                            $tr.append("<td> - </td>");
                            $tr.append("<td> - </td>");
                        } else {
                            $tr.append("<td>" + player.BingoMark2 + "</td>");
                            $tr.append("<td><span class='circle-no'>" + "" + player.BingoMark1 + "</span></td>");
                        }


                        $playerList.append($tr);
                    });

                var hasNewChampion = false;

                $.each(data.ChampionList,
                    function(idx, player) {

                        var $div = $("<div class='name' />");

                        if (player.Active) {
                            hasNewChampion = true;
                            $div.attr("data-active", true);
                        }

                        var cpHtml = "<span class='ranking'>第 " +
                            player.Ranking +
                            " 名</span><span class='circle-no'>" +
                            player.BingoMark2 +
                            "</span>";
                        cpHtml += "<br />#" + player.CardNo + " " + player.Name;

                        $div.html(cpHtml);

                        $div.on("click",
                            function(e) {
                                e.preventDefault();
                                var link = $.api("play");
                                window.open(link + "?card=" + player.Key,
                                    "carddetails",
                                    "location=yes,height=750,width=520,scrollbars=yes,status=yes");
                            });

                        $championList.append($div);
                    });

                $(".champion-count").html(data.ChampionList.length);

                if (hasNewChampion)
                    Bingo.dashboard.newWinner();
            }
        });


};

Bingo.dashboard.roll = function() {


    var $this = $(".btn-roll");

    if ($this.hasClass("rolling"))
        return;

    aAudio.play();
    $this.addClass("rolling");

    var timer = setInterval(function() {
            $(".bingo-digit.d1").html(Math.floor(Math.random() * 10));
            $(".bingo-digit.d2").html(Math.floor(Math.random() * 10));
        },
        100);

    $this.data("timer", timer);

    if ($(".player-list").data("timer")) {
        clearInterval($(".player-list").data("timer"));
        $(".player-list").data("timer", null);
    }

    $.ajax({
            url: $.api("bingoRoll"),
            type: "POST"
        })
        .done(function(data) {

            clearInterval($this.data("timer"));

            if (data.D1 && data.D2) {
                $(".cols .no[data-val='" + data.NewNumber + "']").addClass("active");
                $(".bingo-digit.d1").html(data.D1);
                $(".bingo-digit.d2").html(data.D2);
            }

            Bingo.dashboard.getPlayerlist();
            $this.removeClass("rolling");
        });

};

Bingo.dashboard.newWinner = function() {

    $("body").addClass("new-winner");
    $("body").append("<div class='new-winner_bg'><img src='/Asserts/images/Bingo_Win_3a.png' /></div>");

    $(".new-winner_bg img").addClass("slideInDown animated");

    setTimeout(function() {
            $(".new-winner_bg img").attr("class", "");
            $(".new-winner_bg img").addClass("animated swing infinite");
            setTimeout(function() {
                    $(".new-winner_bg img").attr("class", "");
                    $(".new-winner_bg img").addClass("animated zoomOutUp");
                    setTimeout(function() {
                            $("body").removeClass("new-winner");
                            $(".new-winner_bg").remove();
                        },
                        1000);
                },
                3000);

        },
        1000);

};
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