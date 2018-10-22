/// <reference path="../../jquery-1.10.2.js" />
/// <reference path="App.js" />

var app = {};

app.urls = {};
app.urls.masters = {};

app.urls.commonDepartmentList = '/common/GetDepartments';
app.urls.commonGetDaysList = '/common/GetDaysList';
app.urls.commonGetDoctorList = '/common/GetDoctorList';

app.urls.departmentList = '/masters/GetDepartments';
app.urls.departmentSave = '/masters/SaveDepartment';
app.urls.departmentEdit = '/masters/EditDepartment';
app.urls.departmentDelete = '/masters/DeleteDepartment';

app.urls.doctorList = '/masters/GetDoctors';
app.urls.doctorSave = '/masters/SaveDoctor';
app.urls.doctorEdit = '/masters/EditDoctor';
app.urls.doctorDelete = '/masters/DeleteDoctor';

app.urls.scheduleList = '/masters/GetSchedule';
app.urls.scheduleSave = '/masters/SaveSchedule';
app.urls.scheduleEdit = '/masters/EditSchedule';
app.urls.scheduleDelete = '/masters/DeleteSchedule';

app.urls.appointmentdeptWiseDoctorScheduleList = "/Appointment/deptWiseDoctorScheduleList";
app.urls.appointmentdayWiseDoctorScheduleList = "/Appointment/DayWiseDoctorScheduleList";
app.urls.appointmentDateWiseDoctorAppointmentList = "/Appointment/DateWiseDoctorAppointmentList";
app.urls.appointmentSaveAppointment = "/Appointment/SaveAppointment";

app.urls.appointmentGetPatientAppointmentList = "/Appointment/GetPatientAppointmentList";
app.urls.appointmentCancelAppointment = "/Appointment/CancelAppointment";

app.urls.masters.GetDoctorLeaveList = '/masters/GetDoctorLeaveList';
app.urls.masters.SaveDoctorLeave = '/masters/SaveDoctorLeave';
app.urls.masters.SaveAppSetting = '/masters/SaveAppSetting';
app.urls.masters.GetAppSetting = '/masters/GetAppSetting';