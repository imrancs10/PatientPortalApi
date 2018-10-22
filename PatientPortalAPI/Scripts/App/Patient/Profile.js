/// <reference path="../../jquery-1.10.2.js" />
/// <reference path="../Global/App.js" />
/// <reference path="../Global/Utility.js" />

$(document).ready(function () {
    var jsonData = jsonPatient;

    utility.bindDdlByAjax(app.urls.commonDepartmentList, 'department', 'DeparmentName', 'DepartmentId', function () {
        $("#department option").each(function (i, ele) {
            if ($(ele).text() == jsonData.Department) {
                $("#department").val($(ele).val());
                return;
            }
        });
    });

    fillCountryStateCity();
    function fillCountryStateCity() {
        //Get CIty
        $.ajax({
            dataType: 'json',
            type: 'POST',
            url: '/Home/GetCitieByCItyId',
            data: '{citiId: "' + jsonData.CityId + '" }',
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                $("#cityLabel").html(data.CityName);
            },
            failure: function (response) {
                alert(response);
            },
            error: function (response) {
                alert(response.responseText);
            }
        });
        //get State
        $.ajax({
            dataType: 'json',
            type: 'POST',
            url: '/Home/GetStateByStateId',
            data: '{stateId: "' + jsonData.StateId + '" }',
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                $("#stateLabel").html(data.StateName);
            },
            failure: function (response) {
                alert(response);
            },
            error: function (response) {
                alert(response.responseText);
            }
        });
    }

    setSelectedGender();
    function setSelectedGender() {
        $("#Gender").val(jsonData.Gender);
    }

    setSelectedReligion();
    function setSelectedReligion() {
        $("#religion").val(jsonData.Religion);
    }

    setSelectedTitle();
    function setSelectedTitle() {
        $("#title").val(jsonData.Title);
    }
    setSelectedMaritalStatus();
    function setSelectedMaritalStatus() {
        $("#MaritalStatus").val(jsonData.MaritalStatus);
    }

    fillState();
    function fillState() {
        let dropdown = $('#state');
        dropdown.empty();
        dropdown.append('<option value="">Select</option>');
        dropdown.prop('selectedIndex', 0);
        $.ajax({
            dataType: 'json',
            type: 'POST',
            url: '/Home/GetSates',
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                $.each(data, function (key, entry) {
                    dropdown.append($('<option></option>').attr('value', entry.StateId).text(entry.StateName));
                })
                dropdown.val(jsonData.StateId);
                fillCity();
            },
            failure: function (response) {
                alert(response);
            },
            error: function (response) {
                alert(response.responseText);
            }
        });
    }
    
    function fillCity() {
        var stateId = jsonData.StateId;
        let dropdown = $('#city');
        dropdown.empty();
        dropdown.append('<option value="">Select</option>');
        dropdown.prop('selectedIndex', 0);
        $.ajax({
            dataType: 'json',
            type: 'POST',
            url: '/Home/GetCities',
            data: '{stateId: "' + stateId + '" }',
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                $.each(data, function (key, entry) {
                    dropdown.append($('<option></option>').attr('value', entry.CityId).text(entry.CityName));
                })
                dropdown.val(jsonData.CityId);
            },
            failure: function (response) {
                alert(response);
            },
            error: function (response) {
                alert(response.responseText);
            }
        });
    }

    //$('#country').on('change', function (e) {
    //    var optionSelected = $("option:selected", this);
    //    var valueSelected = this.value;
    //    fillStateByCountryId(valueSelected)
    //});
    //fillStateByCountryId(101);
    //function fillStateByCountryId(countryId) {
    //    let dropdown = $('#state');
    //    dropdown.empty();
    //    let dropdownCity = $('#city');
    //    dropdownCity.empty();
    //    dropdown.append('<option value="">Select</option>');
    //    dropdown.prop('selectedIndex', 0);
    //    const url = utility.baseUrl + 'Json/states.json';
    //    // Populate dropdown with list of provinces
    //    $.getJSON(url, function (data) {
    //        var states = data.states.filter(function (i, n) {
    //            return i.country_id == countryId;
    //        });
    //        $.each(states, function (key, entry) {
    //            dropdown.append($('<option></option>').attr('value', entry.id).text(entry.name));
    //        })
    //    });
    //}
    $('#state').on('change', function (e) {
        var optionSelected = $("option:selected", this);
        var valueSelected = this.value;
        fillCityByStateId(valueSelected);
    });

    function fillCityByStateId(stateId) {
        let dropdown = $('#city');
        dropdown.empty();
        dropdown.append('<option value="">Select</option>');
        dropdown.prop('selectedIndex', 0);
        $.ajax({
            dataType: 'json',
            type: 'POST',
            url: '/Home/GetCities',
            data: '{stateId: "' + stateId + '" }',
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                $.each(data, function (key, entry) {
                    dropdown.append($('<option></option>').attr('value', entry.CityId).text(entry.CityName));
                })
                dropdown.val(jsonData.CityId);
            },
            failure: function (response) {
                alert(response);
            },
            error: function (response) {
                alert(response.responseText);
            }
        });
    }

    $("input:file").change(function () {
        var fileName = $(this).val();
        fileName = fileName.substring(fileName.indexOf('fakepath') + 9, fileName.length);
        $("#filename").html(fileName);
    });
});

function isNumber(evt) {
    evt = (evt) ? evt : window.event;
    var charCode = (evt.which) ? evt.which : evt.keyCode;
    if (charCode > 31 && (charCode < 48 || charCode > 57)) {
        return false;
    }
    return true;
}

$(function () {
    $("#dialog").dialog({
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
});