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