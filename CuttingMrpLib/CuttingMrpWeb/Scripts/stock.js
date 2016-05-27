var Stock = {};

Stock.init = function () {
    var partNr = $('#PartNr').val();
    var fifofrom = $('#FIFOFrom').val();
    var fifoto = $('#FIFOTo').val();
    var quantityfrom = $('#QuantityFrom').val() > 0 ? $('#QuantityFrom').val() : "";
    var quantityto = $('#QuantityTo').val() > 0 ? $('#QuantityTo').val() : "";
    var wh = $('#Wh').val();
    var position = $('#Position').val();

    Stock.add_string_label_to_div(partNr, 'PartNr Like ', '.filter-p');
    Stock.add_range_label_to_div(fifofrom +"~"+ fifoto, 'FIFO ', '.filter-p');
    Stock.add_range_label_to_div(quantityfrom +"~"+ quantityto, 'Quantity ', '.filter-p');
    Stock.add_string_label_to_div(wh, 'Wh Like', '.filter-p');
    Stock.add_string_label_to_div(position, 'Position Like', '.filter-p');
}

Stock.add_string_label_to_div = function (content, name, cls) {
    if (content != null && content != "") {
        $("<p class='label label-primary' style='margin-left:5px;font-size:.8em;'>" + name + " " + content + "</p>").appendTo(cls).ready(function () {
        });
    }
}

Stock.add_range_label_to_div = function (content,name, cls) {
    var from = content.split("~")[0];
    var to = content.split("~")[1];
    if ((from != "" && from != null)&&(to!=null && to!="")) {
        $("<p class='label label-primary' style='margin-left:5px;font-size:.8em;'>" + name + " : " + from +"~" + to + "</p>").appendTo(cls).ready(function () {
        });
    } else if ((from != "" && from != null) && (to == null || to == "")) {
        $("<p class='label label-primary' style='margin-left:5px;font-size:.8em;'>" + name + ">=" + from + "</p>").appendTo(cls).ready(function () {
        });
    } else if ((from == "" || from == null) && (to != null && to != "")) {
        $("<p class='label label-primary' style='margin-left:5px;font-size:.8em;'>" + name + "<=" + to + "</p>").appendTo(cls).ready(function () {
        });
    }
}

$('.datetime-picker').datetimepicker({
    lang: 'ch'
})