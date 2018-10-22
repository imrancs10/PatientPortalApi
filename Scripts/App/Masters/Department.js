/// <reference path="../../jquery-1.10.2.js" />
/// <reference path="../Global/App.js" />
/// <reference path="../Global/Utility.js" />
'use strict'
var department = {};
$(document).ready(function () {
    department.getData();
});

department.getData = function () {
    var url = app.urls.departmentList;
    utility.ajax.helper(url, function (data) {
        var table = $('#deptTable');
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
                td = td + '<td>' + ele[ele1] + '</td>';
            });
            td = td + '<td><div class="btn-group" role="group" aria-label="Basic example">' +
                                '<button type="button" id="btnEdit" class="btn btn-secondary" data-id=' + ele["DepartmentId"] + ' onclick="department.edit(this)">Edit</button>' +
                                '<button type="button" class="btn btn-secondary" data-id=' + ele["DepartmentId"] + ' onclick="department.delete(this)">Delete</button>' +
                            '</div></td>';
            tr = tr + td + '</tr>';
            $(tbody).append(tr);
        });
    });
}

department.addNew = function () {
    if ($('[name="txtDepartment"]').length > 0) {
        utility.alert.setAlert(utility.alert.alertType.warning, 'One row is already in add mode');
    }
    else {
        var table = $('#deptTable');
        var tbody = $(table).find('tbody');
        var trLen = $(tbody).find('tr').length;
        var tr = '<tr>';
        var td = '<td>' + (trLen + 1) + '</td>';
        td = td + '<td> <input type="text" class="form-control" name="txtDepartment" value="" /></td>';
        td = td + '<td><div class="btn-group" role="group" aria-label="Basic example">' +
                            '<button type="button" class="btn btn-secondary" onclick="department.save(this)">Save</button>' +
                            '<button type="button" class="btn btn-secondary" onclick="department.cancel(this)">Cancel</button>' +
                        '</div></td>';
        tr = tr + td + '</tr>';
        $(tbody).append(tr);
    }
}

department.cancel = function (row) {
    $(row).parent().parent().parent().remove();
}



department.edit = function (row) {
    if ($('[name="txtDepartment"]').length > 0) {
        utility.alert.setAlert(utility.alert.alertType.warning, 'One row is already in editable mode');
    }
    else {
        var td = $(row).parent().parent().parent().find('td:eq(1)');
        var preText = $(td).text();
        $(td).empty().append('<input type="text" name="txtDepartment" class="form-control" value="' + preText + '" />');
        $(row).parent().prepend('<button type="button" id="btnUpdate" class="btn btn-secondary" data-id="' + $(row).data('id') + '" onclick="department.update(this)">Update</button>');
        $(row).parent().append('<button type="button" class="btn btn-secondary" onclick="department.cancelEdit(this,' + $(row).data('id') + ')">cancel</button>');
        $(row).remove();
    }
}

department.update = function (row) {
    var deptName = $(row).parent().parent().parent().find('input[type="text"]').val();
    var deptId = $(row).data('id');
    var url = app.urls.departmentEdit;
    utility.ajax.helperWithData(url, { deptName: deptName, deptId: deptId }, function (data) {
        if (data = 'Data has been updated') {
            utility.alert.setAlert(utility.alert.alertType.success, 'Data has been updated');
            department.getData();
        }
    });
}

department.cancelEdit = function (row,id) {
    var preText = $(row).parent().parent().parent().find('td:eq(1) input[type="text"]').val();
    $(row).parent().parent().parent().find('td:eq(1)').empty().text(preText);
    $(row).parent().prepend('<button type="button" class="btn btn-secondary" data-id="' + id + '" onclick="department.edit(this)">Edit</button>');
    $('#btnUpdate').remove();
    $(row).remove();
}

department.delete = function (row) {
    var deptId = $(row).data('id');
    var url = app.urls.departmentDelete;
    utility.ajax.helperWithData(url, { deptId: deptId }, function (data) {
        if (data = 'Data delete from database') {
            utility.alert.setAlert(utility.alert.alertType.success, 'Data delete from database');
            department.getData();
        }
    });
}

department.save = function (row) {
    var deptName = $(row).parent().parent().parent().find('input[type="text"]').val();
    var url = app.urls.departmentSave;
    utility.ajax.helperWithData(url, { deptName: deptName }, function (data) {
        if (data = 'Data has been saved') {
            utility.alert.setAlert(utility.alert.alertType.success, 'Data has been saved');
            department.getData();
        }
    });
}
