$(document).ready(function () {
    utility.ajax.helper(app.urls.masters.GetAppSetting, function (data) {
        $('#txtAppointmentSlot').val(data.AppointmentSlot);
        $('#txtCalenderPeriod').val(data.CalenderPeriod);
        $('#txtAppointmentMessage').val(data.AppointmentMessage);
        $('#txtAppointmentLimitPerUser').val(data.AppointmentLimitPerUser);
        $('#txtAppointmentCancelPeriod').val(data.AppointmentCancelPeriod);
        $('#txtAutoCancelMessage').val(data.AutoCancelMessage);
        $('#chkIsActiveAppointmentMessage').prop('checked', data.IsActiveAppointmentMessage);
    },null,"GET");
});

$(document).on('click', '#btnSave', function () {
    var param = {};
    param.AppointmentSlot = $('#txtAppointmentSlot').val();
    param.CalenderPeriod = $('#txtCalenderPeriod').val();
    param.AppointmentMessage = $('#txtAppointmentMessage').val();
    param.AppointmentLimitPerUser = $('#txtAppointmentLimitPerUser').val();
    param.AppointmentCancelPeriod = $('#txtAppointmentCancelPeriod').val();
    param.AutoCancelMessage = $('#txtAutoCancelMessage').val();
    param.AutoCancelMessage = $('#txtAutoCancelMessage').val();
    param.IsActiveAppointmentMessage = $('#chkIsActiveAppointmentMessage').is(':checked');
    utility.ajax.helperWithData(app.urls.masters.SaveAppSetting, param, function (data) {
        utility.alert.setAlert(utility.alert.alertType.success, 'Saved');
    }, null, "POST");
});