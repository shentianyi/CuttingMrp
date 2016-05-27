var stock = {};

stock.add_label_to_div = function (content, name, cls) {
    if (content != null && content != "") {
        $("<p class='label label-primary'>" + name + " " + content + "</p>").appendTo(cls).ready(function () {
        });
    }
}