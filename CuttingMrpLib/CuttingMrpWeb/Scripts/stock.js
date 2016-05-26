var stock = {};
stock.show_fill_pop = function () {
    $('#basic-addon-filter').click(function () {
        alert("FSF")
        $('#basic-addon-filter').popModal({
            html: $('#extra-filter-content'),
            placement: 'bottomRight',
            showCloseBut: true,
            onDocumentClickClose: true,
            onOkBut: function () { },
            onCancelBut: function () { },
            onLoad: function () { },
            onClose: function () { }
        })
    });
}     
