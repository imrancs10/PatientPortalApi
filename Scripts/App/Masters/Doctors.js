/// <reference path="../../jquery-1.10.2.js" />
/// <reference path="../Global/App.js" />
/// <reference path="../Global/Utility.js" />

var doctor = {};

$(document).ready(function () {
    utility.bindDdlByAjax(app.urls.commonDepartmentList, 'ddlDepartment', 'DeparmentName', 'DepartmentId', function () {
        doctor.getData();
    });

});

doctor.addNew = function () {
    if ($('#txtDoctor').length > 0) {
        utility.alert.setAlert(utility.alert.alertType.warning, 'One row is already in add mode');
    }
    else {
        var table = $('#doctorTable');
        var tbody = $(table).find('tbody');
        var trLen = $(tbody).find('tr').length;
        var tr = '<tr>';
        var td = '<td>' + (trLen + 1) + '</td>';
        td = td + '<td> <select class="form-control" required="required">' + $('#ddlDepartment').clone().html() + '</select></td>';
        td = td + '<td> <input type="text" id="txtDoctor" class="form-control" name="txtDoctor" value="" /></td>';
        td = td + '<td><div class="btn-group" role="group" aria-label="Basic example">' +
                            '<button type="button" class="btn btn-secondary" onclick="doctor.save(this)">Save</button>' +
                            '<button type="button" class="btn btn-secondary" onclick="doctor.cancel(this)">Cancel</button>' +
                        '</div></td>';
        tr = tr + td + '</tr>';
        $(tbody).append(tr);
    }
}

doctor.cancel = function (row) {
    $(row).parent().parent().parent().remove();
}

doctor.getData = function () {
    var url = app.urls.doctorList;
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
                td = td + '<td data-deptid="' + ele["DepartmentId"] + '">' + ele[ele1] + '</td>';
            });
            td = td + '<td><div class="btn-group" role="group" aria-label="Basic example">' +
                                '<button type="button" id="btnEdit" class="btn btn-secondary" data-docid="' + ele["DoctorId"] + '" data-deptid="' + ele["DepartmentId"] + '" onclick="doctor.edit(this)">Edit</button>' +
                                '<button type="button" class="btn btn-secondary" data-docid="' + ele["DoctorId"] + '" data-deptid="' + ele["DepartmentId"] + '" onclick="doctor.delete(this)">Delete</button>' +
                            '</div></td>';
            tr = tr + td + '</tr>';
            $(tbody).append(tr);
        });
    });
}

doctor.save = function (row) {
    var mainContainer = $(row).parent().parent().parent();
    var docName = $(mainContainer).find('input[type="text"]').val();
    var deptId = $(mainContainer).find('select').find(':selected').val();

    if (deptId != null && typeof deptId !== undefined && deptId !== '') {

        if (docName != null && typeof docName !== undefined && docName !== '') {

            var url = app.urls.doctorSave;
            var param = {};
            param.doctorName = docName;
            param.deptId = deptId;

            utility.ajax.helperWithData(url, param, function (data) {
                if (data = 'Data has been saved') {
                    utility.alert.setAlert(utility.alert.alertType.success, 'Data has been saved');
                    doctor.getData();
                }
            });

        }
        else {
            utility.alert.setAlert(utility.alert.alertType.warning, 'Doctor name is required');
            $(mainContainer).find('input[type="text"]').focus();
        }
    }
    else {
        utility.alert.setAlert(utility.alert.alertType.warning, 'Please select department');
        $(mainContainer).find('select').focus();
    }
}


doctor.edit = function (row) {
    if ($('#txtDoctor').length > 0) {
        utility.alert.setAlert(utility.alert.alertType.warning, 'One row is already in editable mode');
    }
    else {
        var mainContainer = $(row).parent().parent().parent();
        var doctortd = $(mainContainer).find('td:eq(1)');
        var depttd = $(mainContainer).find('td:eq(2)');

        var docName = $(doctortd).text();
        $(doctortd).empty().append('<input id="txtDoctor" type="text" class="form-control" value="' + docName + '" />');
        $(depttd).empty().append('<select class="form-control" required="required">' + $('#ddlDepartment').clone().html() + '</select>');
        $(depttd).find('select').val($(depttd).data('deptid'));
        $(row).parent().prepend('<button type="button" id="btnUpdate" class="btn btn-secondary" data-docid="' + $(row).data('docid') + '" data-deptid="' + $(row).data('deptid') + '" onclick="doctor.update(this)">Update</button>');
        $(row).parent().append('<button type="button" class="btn btn-secondary" onclick="doctor.cancelEdit(this,' + $(row).data('deptid') + ')">cancel</button>');
        $(row).remove();
    }
}

doctor.update = function (row) {
    var mainContainer = $(row).parent().parent().parent();
    var docName = $(mainContainer).find('input[type="text"]').val();
    var deptId = $(mainContainer).find('select').find(':selected').val();

    if (deptId != null && typeof deptId !== undefined && deptId !== '') {

        if (docName != null && typeof docName !== undefined && docName !== '') {

            var url = app.urls.doctorSave;
            var param = {};
            param.doctorName = docName;
            param.deptId = deptId;

            utility.ajax.helperWithData(url, param, function (data) {
                if (data = 'Data has been updated') {
                    utility.alert.setAlert(utility.alert.alertType.success, 'Data has been updated');
                    doctor.getData();
                }
            });

        }
        else {
            utility.alert.setAlert(utility.alert.alertType.warning, 'Doctor name is required');
            $(mainContainer).find('input[type="text"]').focus();
        }
    }
    else {
        utility.alert.setAlert(utility.alert.alertType.warning, 'Please select department');
        $(mainContainer).find('select').focus();
    }
}

doctor.cancelEdit = function (row, id) {
    var mainContainer = $(row).parent().parent().parent();
    var doctorname = $(mainContainer).find('td:eq(1) input[type="text"]').val();
    var department = $(mainContainer).find('td:eq(2) select').val(id).find(':selected').text();
    $(mainContainer).find('td:eq(1)').empty().text(doctorname);
    $(mainContainer).find('td:eq(2)').empty().text(department);
    $(row).parent().prepend('<button type="button" class="btn btn-secondary" data-id="' + id + '" onclick="doctor.edit(this)">Edit</button>');
    $('#btnUpdate').remove();
    $(row).remove();
}

doctor.delete = function (row) {
    var docId = $(row).data('docid');
    var url = app.urls.doctorDelete;
    utility.ajax.helperWithData(url, { docId: docId }, function (data) {
        if (data = 'Data delete from database') {
            utility.alert.setAlert(utility.alert.alertType.success, 'Data delete from database');
            doctor.getData();
        }
    });
}



