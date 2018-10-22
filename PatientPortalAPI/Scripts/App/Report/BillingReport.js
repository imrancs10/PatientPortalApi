/// <reference path="../../jquery-1.10.2.js" />
$(document).on('click', '#_file', function () {
    if ($(this).is(':checked')) {
        $('.file').show();
        $('.url').hide();
    }
});

$(document).on('click', '#_url', function () {
    if ($(this).is(':checked')) {
        $('.file').hide();
        $('.url').show();
    }
});