var utility = {};

utility.ajax = {};
utility.table = {};
utility.alert = {};
utility.ajax.errorCall = function (x,y,z) {

}
utility.ajax.options = {
    url: '',
    method: "POST",
    contentType: 'application/json',
    error: utility.ajax.errorCall(),
    success:''
};

utility.ajax.helper = function (url, success, error) {
    if (typeof success === 'function') {
        utility.ajax.options.success = success;
    }
    else
        throw new Error('success should be a function');

    if (typeof error !== undefined && typeof error === 'function')
    {
        utility.ajax.options.error = error;
    }

    utility.ajax.options.url = url;    

    $.ajax(utility.ajax.options);
}
utility.ajax.helperWithData = function (url,data, success, error) {
    if (typeof success === 'function') {
        utility.ajax.options.success = success;
    }
    else
        throw new Error('success should be a function');

    if (typeof error !== undefined && typeof error === 'function') {
        utility.ajax.options.error = error;
    }

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
    else
    {
        throw new Error('msg is required');
    }
}

utility.alert.alertType = {};
utility.alert.alertType.warning = "Warning";
utility.alert.alertType.error = "Error";
utility.alert.alertType.info = "Information";
utility.alert.alertType.success = "Success";

utility.bindDdlByAjax = function (methodUrl, ddlId, text, value, callback,htmlData) {
    var urls = app.urls[methodUrl];
    urls = urls === undefined ? methodUrl : urls;
    utility.ajax.helper(urls, function (data) {
        if (typeof data === 'object') {
            var ddl = $('#' + ddlId);
            ddl.find(':gt(0)').remove();
            $(data).each(function (ind, ele) {
                var Value = value === undefined ? ele["Value"] : ele[value];
                var Text = text === undefined ? ele["Text"] : ele[text];
                if(typeof htmlData==undefined)
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
