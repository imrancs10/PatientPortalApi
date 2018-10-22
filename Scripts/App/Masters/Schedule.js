/// <reference path="../../jquery-1.10.2.js" />
/// <reference path="../Global/App.js" />
/// <reference path="../Global/Utility.js" />

var schedule = {};

$(document).ready(function () {
    utility.bindDdlByAjax(app.urls.doctorList, 'ddlDoctor', 'DoctorName', 'DoctorId', function () {
        schedule.getData();
    }, 'DepartmentName');

    utility.bindDdlByAjax(app.urls.commonGetDaysList, 'ddlDay', 'DayName', 'DayId');

    schedule.bindTime('ddlTime');

});

schedule.addNew = function () {
    if ($('#txtDoctor').length > 0) {
        utility.alert.setAlert(utility.alert.alertType.warning, 'One row is already in add mode');
    }
    else {
        var table = $('#doctorTable');
        var tbody = $(table).find('tbody');
        var trLen = $(tbody).find('tr').length;
        var tr = '<tr>';
        var td = '<td>' + (trLen + 1) + '</td>';
        td = td + '<td> <select class="form-control" id="ddlNewDoctor" required="required">' + $('#ddlDoctor').clone().html() + '</select></td>';
        td = td + '<td></td>';
        td = td + '<td> <select class="form-control" id="ddlNewDay" required="required">' + $('#ddlDay').clone().html() + '</select></td>';
        td = td + '<td> <select class="form-control" id="ddlNewTimeFrom" required="required">' + $('#ddlTime').clone().html() + '</select></td>';
        td = td + '<td> <select class="form-control" id="ddlNewTimeTo" required="required">' + $('#ddlTime').clone().html() + '</select></td>';
        td = td + '<td><div class="btn-group" role="group" aria-label="Basic example">' +
                            '<button type="button" class="btn btn-secondary" onclick="schedule.save(this)">Save</button>' +
                            '<button type="button" class="btn btn-secondary" onclick="schedule.cancel(this)">Cancel</button>' +
                        '</div></td>';
        tr = tr + td + '</tr>';
        $(tbody).append(tr);
    }
}

schedule.cancel = function (row) {
    $(row).parent().parent().parent().remove();
}

schedule.getData = function () {
    var url = app.urls.scheduleList;
    utility.ajax.helper(url, function (data) {
        var table = $('#doctorTable');
        if (table.length == 0)
            throw new Error('Table not found');
        var tbody = $(table).find('tbody');
        tbody.empty();
        var binderArray = [];

        $(table).find('thead tr th').each(function (ind, ele) {
            if ($(ele).attr('name') !== undefined) {
                binderArray.push($(ele).attr('name'));
            }
        });

        $(data).each(function (ind, ele) {
            var tr = '<tr>';
            var td = '<td>' + (ind + 1) + '</td>';
            $(binderArray).each(function (ind1, ele1) {
                td = td + '<td data-deptid="' + ele["DepartmentId"] + '" >' + ele[ele1] + '</td>';
            });
            td = td + '<td><div id="btn' + (ind + 1) + '" class="btn-group" role="group" aria-label="Basic example" data-data="">' +
                                '<button type="button" id="btnEdit" class="btn btn-secondary"  onclick="schedule.edit(this)">Edit</button>' +
                                '<button type="button" class="btn btn-secondary" data-data="' + ele + '" data-docid="' + ele["DoctorId"] + '" data-deptid="' + ele["DepartmentId"] + '" onclick="schedule.delete(this)">Delete</button>' +
                            '</div></td>';
            tr = tr + td + '</tr>';
            $(tbody).append(tr);
            $('#btn' + (ind + 1)).data('data', ele);
        });
    });
}

schedule.save = function (row) {
    var mainContainer = $(row).parent().parent().parent();
    var ddlNewDoctor = $(mainContainer).find('#ddlNewDoctor');
    var ddlNewDay = $(mainContainer).find('#ddlNewDay');
    var ddlNewTimeFrom = $(mainContainer).find('#ddlNewTimeFrom');
    var ddlNewTimeTo = $(mainContainer).find('#ddlNewTimeTo');
    var param = {};
    param.DoctorId = $(ddlNewDoctor).find(':selected').val();
    param.DayId = $(ddlNewDay).find(':selected').val();
    param.TimeFrom = $(ddlNewTimeFrom).find(':selected').val();
    param.TimeTo = $(ddlNewTimeTo).find(':selected').val();
    param.TimeFromMeridiumId = $(ddlNewTimeFrom).find(':selected').data('meridiumid');
    param.TimeToMeridiumId = $(ddlNewTimeTo).find(':selected').data('meridiumid');


    if (param.DoctorId != null && typeof param.DoctorId !== undefined && param.DoctorId !== '') {

        if (param.DayId != null && typeof param.DayId !== undefined && param.DayId !== '') {

            if (param.TimeFrom != null && typeof param.TimeFrom !== undefined && param.TimeFrom !== '') {

                if (param.TimeTo != null && typeof param.TimeTo !== undefined && param.TimeTo !== '') {
                    var url = app.urls.scheduleSave;

                    utility.ajax.helperWithData(url, param, function (data) {
                        if (data == 'Data has been saved') {
                            utility.alert.setAlert(utility.alert.alertType.success, 'Data has been saved');
                            doctor.getData();
                        }
                    });
                }
                else {
                    utility.alert.setAlert(utility.alert.alertType.warning, 'Please select time to');
                    $(ddlNewTimeFrom).focus();
                }
            }
            else {
                utility.alert.setAlert(utility.alert.alertType.warning, 'Please select time from');
                $(ddlNewTimeFrom).focus();
            }
        }
        else {
            utility.alert.setAlert(utility.alert.alertType.warning, 'Please select doctor');
            $(ddlNewDay).focus();
        }
    }
    else {
        utility.alert.setAlert(utility.alert.alertType.warning, 'Please select doctor');
        $(ddlNewDoctor).focus();
    }
}


schedule.edit = function (row) {
    if ($('#ddlEditDoctor').length > 0) {
        utility.alert.setAlert(utility.alert.alertType.warning, 'One row is already in editable mode');
    }
    else {
        var rowData = $(row).parent().data('data');
        var mainContainer = $(row).parent().parent().parent();
        var doctortd = $(mainContainer).find('td:eq(1)');
        var depttd = $(mainContainer).find('td:eq(2)');
        var daytd = $(mainContainer).find('td:eq(3)');
        var timefrom = $(mainContainer).find('td:eq(4)');
        var timeto = $(mainContainer).find('td:eq(5)');

        var docName = $(doctortd).text();
        $(doctortd).empty().append('<select class="form-control" id="ddlEditDoctor" required="required">' + $('#ddlDoctor').clone().html() + '</select>');
       
        $(daytd).empty().append('<select class="form-control" id="ddlEditDay" required="required">' + $('#ddlDay').clone().html() + '</select>');
        $(timefrom).empty().append('<select class="form-control" id="ddlEditTimeFrom" required="required">' + $('#ddlTime').clone().html() + '</select>');
        $(timeto).empty().append('<select class="form-control" id="ddlEditTimeTo" required="required">' + $('#ddlTime').clone().html() + '</select>');
        $(doctortd).find('select').val(rowData.DoctorID).change();
        $(daytd).find('select').val(rowData.DayId);
        $(timefrom).find('select').val($(timefrom).find('select option:contains("' + rowData.TimeFrom + '")').val());
        $(timeto).find('select').val($(timeto).find('select option:contains("' + rowData.TimeTo + '")').val());
        $(row).parent().prepend('<button type="button" id="btnUpdate" class="btn btn-secondary" data-id="' + rowData.DoctorScheduleDayID + '" onclick="schedule.update(this)">Update</button>');
        $(row).parent().append('<button type="button" class="btn btn-secondary" onclick="schedule.cancelEdit(this)">cancel</button>');
        $(row).remove();
    }
}

schedule.update = function (row) {
    var mainContainer = $(row).parent().parent().parent();
    var ddlNewDoctor = $(mainContainer).find('#ddlEditDoctor');
    var ddlNewDay = $(mainContainer).find('#ddlEditDay');
    var ddlNewTimeFrom = $(mainContainer).find('#ddlEditTimeFrom');
    var ddlNewTimeTo = $(mainContainer).find('#ddlEditTimeTo');
    var param = {};
    param.DoctorId = $(ddlNewDoctor).find(':selected').val();
    param.DayId = $(ddlNewDay).find(':selected').val();
    param.TimeFrom = $(ddlNewTimeFrom).find(':selected').val();
    param.TimeTo = $(ddlNewTimeTo).find(':selected').val();
    param.TimeFromMeridiumId = $(ddlNewTimeFrom).find(':selected').data('meridiumid');
    param.TimeToMeridiumId = $(ddlNewTimeTo).find(':selected').data('meridiumid');
    param.ScheduleId = $(row).parent().data('data').DoctorScheduleDayID;

    if (param.DoctorId != null && typeof param.DoctorId !== undefined && param.DoctorId !== '') {

        if (param.DayId != null && typeof param.DayId !== undefined && param.DayId !== '') {

            if (param.TimeFrom != null && typeof param.TimeFrom !== undefined && param.TimeFrom !== '') {

                if (param.TimeTo != null && typeof param.TimeTo !== undefined && param.TimeTo !== '') {
                    var url = app.urls.scheduleEdit;

                    utility.ajax.helperWithData(url, param, function (data) {
                        if (data == 'Data has been updated') {
                            utility.alert.setAlert(utility.alert.alertType.success, 'Data has been updated');
                            schedule.getData();
                        }
                    });
                }
                else {
                    utility.alert.setAlert(utility.alert.alertType.warning, 'Please select time to');
                    $(ddlNewTimeFrom).focus();
                }
            }
            else {
                utility.alert.setAlert(utility.alert.alertType.warning, 'Please select time from');
                $(ddlNewTimeFrom).focus();
            }
        }
        else {
            utility.alert.setAlert(utility.alert.alertType.warning, 'Please select doctor');
            $(ddlNewDay).focus();
        }
    }
    else {
        utility.alert.setAlert(utility.alert.alertType.warning, 'Please select doctor');
        $(ddlNewDoctor).focus();
    }
}

schedule.cancelEdit = function (row, id) {
    var mainContainer = $(row).parent().parent().parent();
    id = $(row).parent().data('data');
    $(mainContainer).find('td:eq(1)').empty().text(id.DoctorName);
    $(mainContainer).find('td:eq(2)').empty().text(id.DepartmentName);
    $(mainContainer).find('td:eq(3)').empty().text(id.DayName);
    $(mainContainer).find('td:eq(4)').empty().text(id.TimeFrom);
    $(mainContainer).find('td:eq(5)').empty().text(id.TimeTo);
    $(row).parent().prepend('<button type="button" class="btn btn-secondary" data-id="' + id + '" onclick="schedule.edit(this)">Edit</button>');
    $('#btnUpdate').remove();
    $(row).remove();
}

schedule.delete = function (row) {
    var mainContainer = $(row).parent().parent().parent();
    var ddlNewDoctor = $(mainContainer).find('#ddlEditDoctor');
    var ddlNewDay = $(mainContainer).find('#ddlEditDay');
    var ddlNewTimeFrom = $(mainContainer).find('#ddlEditTimeFrom');
    var ddlNewTimeTo = $(mainContainer).find('#ddlEditTimeTo');
    var param = {};
    param.DoctorId = $(ddlNewDoctor).find(':selected').val();
    param.DayId = $(ddlNewDay).find(':selected').val();
    param.TimeFrom = $(ddlNewTimeFrom).find(':selected').val();
    param.TimeTo = $(ddlNewTimeTo).find(':selected').val();
    param.TimeFromMeridiumId = $(ddlNewTimeFrom).find(':selected').data('meridiumid');
    param.TimeToMeridiumId = $(ddlNewTimeTo).find(':selected').data('meridiumid');
    param.ScheduleId = $(row).parent().data('data').DoctorScheduleDayID;
    var url = app.urls.scheduleDelete;
    utility.ajax.helperWithData(url, param, function (data) {
        if (data == 'Data delete from database') {
            utility.alert.setAlert(utility.alert.alertType.success, 'Data delete from database');
            schedule.getData();
        }
    });
}

schedule.bindTime = function (ddlId) {
    var ddl = $('#' + ddlId);
    $(ddl).find('option:gt(0)').remove();
    for (var i = 1; i <= 24; i++) {
        if (i < 12)
            $(ddl).append('<option data-meridiumid="1" data-timeid="'+i+'" value="' + i + '">' + i + ' AM</option>');
        else if (i == 12)
            $(ddl).append('<option data-meridiumid="2" data-timeid="' + i + '" value="' + i + '">' + i + ' PM</option>');
        else if (i > 12 && i < 23)
            $(ddl).append('<option data-meridiumid="2" data-timeid="' + (i - 12) + '" value="' + (i - 12) + '">' + (i - 12) + ' PM</option>');
        else
            $(ddl).append('<option data-meridiumid="1" data-timeid="' + (i) + '" value="' + (i - 12) + '">' + (i - 12) + ' AM</option>');
    }
}

$(document).on('change', '#ddlNewDoctor', function () {
    var deptName = $(this).find(':selected').data('id');
    $(this).parent().next().empty().text(deptName);
})


