function loadModal(url) {
    $.get(url)
        .done(function (result) {
            $('#global-modal-body').html(result)
        })
    
}

var urlBase = ''