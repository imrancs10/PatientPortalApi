var utility = {};

utility.ajax = {};
utility.table = {};
utility.alert = {};
utility.global = {};
utility.ajax.errorCall = function (x, y, z) {

}

var getUrl = window.location;
utility.baseUrl = getUrl.protocol + "//" + getUrl.host + "/";

utility.ajax.options = {
    url: '',
    method: "POST",
    contentType: 'application/json',
    error: utility.ajax.errorCall(),
    success: ''
};

utility.ajax.helper = function (url, success, error,method) {
    if (typeof success === 'function') {
        utility.ajax.options.success = success;
    }
    else
        throw new Error('success should be a function');

    if (typeof error !== undefined && typeof error === 'function') {
        utility.ajax.options.error = error;
    }

    utility.ajax.options.method = method === undefined ? utility.ajax.options.method : method;
    utility.ajax.options.url = url;

    $.ajax(utility.ajax.options);
}
utility.ajax.helperWithData = function (url, data, success, error, method) {
    if (typeof success === 'function') {
        utility.ajax.options.success = success;
    }
    else
        throw new Error('success should be a function');

    if (typeof error !== undefined && typeof error === 'function') {
        utility.ajax.options.error = error;
    }
    utility.ajax.options.method = method === undefined ? "POST" : method;
    utility.ajax.options.url = url;
    utility.ajax.options.data = JSON.stringify(data);
    utility.ajax.options.dataType = 'json';
    $.ajax(utility.ajax.options);
}

utility.alert.setAlert = function (title, msg) {
    if (typeof msg !== undefined) {
        title = typeof title === undefined ? 'Alert' : title;

        var model = $("#alertModel");
        $(model).attr('title', title);
        $(model).find('p').empty().append(msg).show();

        $("#alertModel").dialog({
            resizable: false,
            height: "auto",
            width: 400,
            modal: true,
            buttons: {
                "OK": function () {
                    $(this).dialog("close");
                }
            }
        });
    }
    else {
        throw new Error('msg is required');
    }
}

utility.alert.alertType = {};
utility.alert.alertType.warning = "Warning";
utility.alert.alertType.error = "Error";
utility.alert.alertType.info = "Information";
utility.alert.alertType.success = "Success";

utility.global.getDaysArray = ["SUN", "MON", "TUE", "WED", "THU", "FRI", "SAT"];
utility.global.getFullDaysArray = ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"];
utility.global.getMonthArray = ["JAN", "FEB", "MAR", "APR", "MAY", "JUN", "JUL", "AUG", "SEP", "OCT", "NOV", "DEC"];
utility.global.get24FormarTime = function (time) {
    var hours = Number(time.match(/^(\d+)/)[1]);
    var minutes = Number(time.match(/:(\d+)/)[1]);
    var AMPM = time.match(/\s(.*)$/)[1];
    if (AMPM == "PM" && hours < 12) hours = hours + 12;
    if (AMPM == "AM" && hours == 12) hours = hours - 12;
    var sHours = hours.toString();
    var sMinutes = minutes.toString();
    if (hours < 10) sHours = "0" + sHours;
    if (minutes < 10) sMinutes = "0" + sMinutes;
    return { hour: sHours, minutes: sMinutes };
}
utility.global.timeSplitter = function (minTime, maxTime, minSeed) {
    minSeed = minSeed > 60 ? 30 : (minSeed < 1 ? 30 : minSeed);
    var time = [];
    var minTimeObj = utility.global.get24FormarTime(minTime);
    var maxTimeObj = utility.global.get24FormarTime(maxTime);
    var minMins = minTimeObj.hour * 60;
    var maxMins = maxTimeObj.hour * 60;
    time.push(minTimeObj.hour + ':' + minTimeObj.minutes + (minTimeObj.hour>11?' PM':' AM'));
    for (var i = minMins; i <= maxMins; i+=60) {
        for (var j = minSeed; j <= 60; j += minSeed) {
            time.push((((minMins + j) % 60) == 0 ? ((minMins / 60) + 1) : (minMins / 60)) + ':' + (((minMins + j) % 60) == 0 ? '00' : ((minMins + j) % 60)) + ' ' + ((((minMins + j) % 60) == 0 ? ((minMins / 60) + 1) : (minMins / 60))>=12 ? 'PM' : 'AM'));
        }
        minMins += 60;
    }
    return time;
}


utility.bindDdlByAjax = function (methodUrl, ddlId, text, value, callback, htmlData) {
    var urls = app.urls[methodUrl];
    urls = urls === undefined ? methodUrl : urls;
    utility.ajax.helper(urls, function (data) {
        if (typeof data === 'object') {
            var ddl = $('#' + ddlId);
            ddl.find(':gt(0)').remove();
            $(data).each(function (ind, ele) {
                var Value = value === undefined ? ele["Value"] : ele[value];
                var Text = text === undefined ? ele["Text"] : ele[text];
                if (typeof htmlData == undefined)
                    ddl.append('<option value=' + Value + '>' + Text + '</option>');
                else
                    ddl.append('<option value=' + Value + ' data-id="' + ele[htmlData] + '">' + Text + '</option>');
            });
        }
        else
            throw new Error('Invalid parameter: expect object only');

        if (callback)
            callback();
    });
}

utility.bindDdlByAjaxWithParam = function (methodUrl, ddlId, param, text, value, htmlDataAttr, callback) {
    var urls = app.urls[methodUrl];
    urls = urls === undefined ? methodUrl : urls;
    utility.ajax.helperWithData(urls, param, function (data) {

        var ddl = $('#' + ddlId);

        if (htmlDataAttr !== undefined) {
            ddl.data(htmlDataAttr, data);
        }

        ddl.find(':gt(0)').remove(); // Remove all pre. Options

        $(data).each(function (ind, ele) {
            var Value = value === undefined ? ele["Value"] : ele[value];
            var Text = text === undefined ? ele["Text"] : ele[text];
            ddl.append('<option value=' + Value + '>' + Text + '</option>');
        });

        if (callback !== undefined)
            callback();
    });
}


Date.prototype.monthDays = function () {
    var d = new Date(this.getFullYear(), this.getMonth() + 1, 0);
    return d.getDate();
}

Date.prototype.getCustomDetails = function (year, month) {
    var obj = {};
    var date = new Date(year, month + 1, 0);
    var monthStartDay = new Date(date.getFullYear(), date.getMonth(), 1);
    var monthEndDay = new Date(date.getFullYear(), date.getMonth() + 1, 0);
    obj.totalDays = date.getDate();
    obj.firstDayName = utility.global.getDaysArray[monthStartDay.getDay()];
    obj.lastDayName = utility.global.getDaysArray[monthEndDay.getDay()];
    obj.firstDayIndex = monthStartDay.getDay();
    obj.lastDayIndex = monthEndDay.getDay();
    obj.currentYear = date.getFullYear();
    obj.currentMonth = date.getMonth() + 1;
    obj.getDateString = date.toDateString();
    obj.todayDate = new Date().getDate();
    obj.todayMonth = new Date().getMonth()+1;
    obj.todayYear = new Date().getFullYear();
    return obj;
}

Date.prototype.compareDate = function (date1, date2) {
    date1 = new Date(date1);
    date2 = new Date(date2);
    var day1 = date1.getDate();
    var month1 = date1.getMonth() + 1;
    var year1 = date1.getFullYear();
    var day2 = date2.getDate();
    var month2 = date2.getMonth() + 1;
    var year2 = date2.getFullYear();
    var obj={};
    obj.isMonthSame = month1 === month2,
    obj.isDaySame = day1 === day2,
    obj.isYearSame = year1 == year2,
    obj.isDateSame = month1 === month2 ? (day1 === day2 ? (year1 == year2 ? true : false) : false) : false;
    obj.isDateLessOrEqual = month1 <= month2 ? (day1 <= day2 ? (year1 <= year2 ? true : false) : false) : false;
    obj.isDateLess = month1 < month2 ? true : (day1 < day2 ? true : (year1 < year2 ? true : false));
    obj.isDateGreaterOrEqual = month1 >= month2 ? (day1 >= day2 ? (year1 >= year2 ? true : false) : false) : false;
    obj.isDateGreater = month1 > month2 ? (day1 > day2 ? (year1 > year2 ? true : false) : false) : false;
    return obj;
}

$(document).ajaxStart(function () {
    $('.ajaxloader').show();
});

$(document).ajaxComplete(function () {
    $('.ajaxloader').hide();
});

utility.confirmBox = function (message,title,yesFunction,noFunction) {
    $('<div></div>').appendTo('body')
    .html('<div><h6>' + message + '?</h6></div>')
    .dialog({
        modal: true, title: title, zIndex: 10000, autoOpen: true,
        width: 'auto', resizable: false,
        buttons: {
            Yes: yesFunction,
            No: noFunction
        },
        close: function (event, ui) {
            $(this).remove();
        }
    });
};