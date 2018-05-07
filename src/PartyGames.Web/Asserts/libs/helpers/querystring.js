//https://stackoverflow.com/questions/901115/how-can-i-get-query-string-values-in-javascript
var qs = window.location.search.replace("?", "").split("&"),
    request = {};
$.each(qs,
    function(i, v) {
        var initial, pair = v.split("=");
        if (initial = request[pair[0]]) {
            if (!$.isArray(initial)) {
                request[pair[0]] = [initial];
            }
            request[pair[0]].push(pair[1]);
        } else {
            request[pair[0]] = pair[1];
        }
        return;
    });