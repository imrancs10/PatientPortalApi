var leave = {};
$(document).ready(function () {
    utility.bindDdlByAjax(app.urls.commonDepartmentList, 'ddlDepartments', 'DeparmentName', 'DepartmentId');
});

$(document).on('change', '#ddlDepartments', function () {
    let deptId = $('#ddlDepartments option:selected').val();
    if (deptId !== '' && deptId !== undefined) {
        let param = {};
        param.deptId = deptId;
        utility.bindDdlByAjaxWithParam(app.urls.commonGetDoctorList, 'ddlDoctors', param, 'DoctorName', 'DoctorId');

    }
    else {
        utility.alert.setAlert(utility.alert.alertType.warning, 'Please select the department');
        $('#ddlDoctors option:gt(0)').remove();
    }
    $('#docTable tbody').empty().append('<tr><td colspan="4">No Record Found</td</tr>');
});

$(document).on('change', '#ddlDoctors', function () {
    let doctorId = $('#ddlDoctors option:selected').val();
    let tboty = $('#docTable tbody');
    if (doctorId !== '' && doctorId !== undefined) {
        let param = {};
        param.doctorId = doctorId;
        utility.ajax.helperWithData(app.urls.masters.GetDoctorLeaveList, param, function (leaveList) {
            let tr = '';
            let index = 0;
            $(tboty).empty();
            $(leaveList).each(function (ind, ele) {
                index += 1;
                var lDate = new Date(parseInt(ele.LeaveDate.substr(6)));
                tr += '<tr>';
                tr += '<td>' + index + '</td>';
                tr += '<td>' + ele.DepartmentName + '</td>';
                tr += '<td>' + ele.DoctorName + '</td>';
                tr += '<td>' + lDate.toDateString() + '</td>';
            });
            if (tr != '') {
                $(tboty).append(tr);
            } else {
                $(tboty).append('<tr><td colspan="4">No Record Found</td</tr>');
            }
        });
    }
    else {
        utility.alert.setAlert(utility.alert.alertType.warning, 'Please select the doctor');
        $(tboty).empty().append('<tr><td colspan="4">No Record Found</td</tr>');
    }
});

$(document).on('click', '#btnSave', function () {
    let deptId = $('#ddlDepartments option:selected').val();
    let doctorId = $('#ddlDoctors option:selected').val();
    let leavedate = $('#txtleavedate').val();
    let param = {};

    if (deptId !== '' && deptId !== undefined) {
        if (doctorId !== '' && doctorId !== undefined) {
            if (leavedate !== '' && leavedate !== undefined) {
                utility.confirmBox('Are you sure..!\n\n if you mark absent to ' + $('#ddlDoctors option:selected').text() + ' then all booked appointment of ' + $('#txtleavedate').val() + ' will autometacally cancelled.', 'Confirmation', function () {
                    $(this).dialog("close");

                    param.doctorId = doctorId;
                    param.leavedate = leavedate;
                    utility.ajax.helperWithData(app.urls.masters.SaveDoctorLeave, param, function (data) {
                        if (data == 'Data has been saved') {
                            utility.alert.setAlert(utility.alert.alertType.success, $('#ddlDoctors option:selected').text() + ' Marked absent');
                            $('#ddlDoctors').change();
                        }
                        else if (data == 'Your Session has been expired') {
                            utility.alert.setAlert(utility.alert.alertType.error, 'Either you logged out or your session is expired');
                            window.location = location.protocol + '//' + location.host; //Redirect to defult page
                        }
                        else if (data == 'Data already exists') {
                            utility.alert.setAlert(utility.alert.alertType.error, 'You have already marked absent to ' + $('#ddlDoctors option:selected').text() + ' on ' + leavedate);
                        }
                        else if(data=='Date should not be in past')
                        {
                            utility.alert.setAlert(utility.alert.alertType.error, 'Selected date is already passed.! please select date today onward');
                        }
                    });

                }, function () {
                    $(this).dialog("close");
                });
            }
            else {
                utility.alert.setAlert(utility.alert.alertType.warning, 'Please doctor leave date');
            }
        }
        else {
            utility.alert.setAlert(utility.alert.alertType.warning, 'Please select the doctor');
        }
    }
    else {
        utility.alert.setAlert(utility.alert.alertType.warning, 'Please select the department');
    }
});