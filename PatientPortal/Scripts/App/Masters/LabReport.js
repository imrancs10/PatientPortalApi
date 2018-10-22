/// <reference path="../../jquery-1.10.2.js" />
/// <reference path="../Global/App.js" />
/// <reference path="../Global/Utility.js" />

var labreport = {};

$(document).ready(function () {

});

labreport.openModal = function (row) {
    var patientId = $(row).parent().parent().find('td').eq(0).html();
    $("#patientId").val(patientId);
    var regNo = $(row).parent().parent().find('td').eq(3).html();
    $("#registrationNumber").val(regNo);
    $(".modal").modal("show");
}



