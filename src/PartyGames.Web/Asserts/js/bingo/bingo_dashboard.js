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