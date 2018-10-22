/// <reference path="../../jquery-1.10.2.js" />
/// <reference path="../Global/App.js" />
/// <reference path="../Global/Utility.js" />

$(document).ready(function () {
    var jsonData = jsonPatient;

    utility.bindDdlByAjax(app.urls.commonDepartmentList, 'department', 'DeparmentName', 'DepartmentId', function () {
        $("#department").val(jsonData.Department);
    });

    if (jsonData.PinCode == 0) {
        $("#pincode").val("");
    }

    if (jsonData.DOB == null) {
        $("#DOB").val("");
    }

    setSelectedGender();
    function setSelectedGender() {
        $("#Gender").val(jsonData.Gender);
    }

    setSelectedReligion();
    function setSelectedReligion() {
        $("#religion").val(jsonData.Religion);
    }

    fillCountry();
    function fillCountry() {
        let dropdown = $('#country');
        dropdown.empty();
        dropdown.append('<option value="">Select</option>');
        dropdown.prop('selectedIndex', 0);
        const url = utility.baseUrl + 'Json/countries.json';
        // Populate dropdown with list of provinces
        $.getJSON(url, function (data) {
            $.each(data.countries, function (key, entry) {
                if (entry.name == 'India')
                    dropdown.append($('<option selected="true"></option>').attr('value', entry.id).text(entry.name));
                else
                    dropdown.append($('<option></option>').attr('value', entry.id).text(entry.name));
            })
            //dropdown.val(jsonData.Country);
            fillState();
        });
    }


    function fillState() {
        var countryId = 101;//jsonData.Country;
        let dropdown = $('#state');
        dropdown.empty();
        dropdown.append('<option value="">Select</option>');
        dropdown.prop('selectedIndex', 0);
        const url = utility.baseUrl + 'Json/states.json';
        // Populate dropdown with list of provinces
        $.getJSON(url, function (data) {
            var states = data.states.filter(function (i, n) {
                return i.country_id == countryId;
            });
            $.each(states, function (key, entry) {
                dropdown.append($('<option></option>').attr('value', entry.id).text(entry.name));
            })
            var statesfilter = data.states.filter(function (i, n) {
                return i.name.toLowerCase() == jsonData.State.toLowerCase();
            });
            $("#state").val(statesfilter[0] != undefined ? statesfilter[0].id : 0);
            fillCity();
        });
    }


    function fillCity() {
        var stateId = $("#state").val();//jsonData.State;
        let dropdown = $('#city');
        dropdown.empty();
        dropdown.append('<option value="">Select</option>');
        dropdown.prop('selectedIndex', 0);
        const url = utility.baseUrl + 'Json/cities.json';
        // Populate dropdown with list of provinces
        $.getJSON(url, function (data) {
            var cities = data.cities.filter(function (i, n) {
                return i.state_id == stateId;
            });
            $.each(cities, function (key, entry) {
                dropdown.append($('<option></option>').attr('value', entry.id).text(entry.name));
            })
            var cityfilter = cities.filter(function (i, n) {
                if (i.name.toLowerCase() == jsonData.City.toLowerCase())
                    return true;
                else
                    return false;
            });
            $("#city").val(cityfilter[0] != undefined ? cityfilter[0].id : 0);
        });
    }

    $('#country').on('change', function (e) {
        var optionSelected = $("option:selected", this);
        var valueSelected = this.value;
        fillStateByCountryId(valueSelected)
    });

    fillStateByCountryId(101);
    function fillStateByCountryId(countryId) {
        let dropdown = $('#state');
        dropdown.empty();
        let dropdownCity = $('#city');
        dropdownCity.empty();
        dropdown.append('<option value="">Select</option>');
        dropdown.prop('selectedIndex', 0);
        const url = utility.baseUrl + 'Json/states.json';
        // Populate dropdown with list of provinces
        $.getJSON(url, function (data) {
            var states = data.states.filter(function (i, n) {
                return i.country_id == countryId;
            });
            $.each(states, function (key, entry) {
                dropdown.append($('<option></option>').attr('value', entry.id).text(entry.name));
            })
        });
    }
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
        const url = utility.baseUrl + 'Json/cities.json';
        // Populate dropdown with list of provinces
        $.getJSON(url, function (data) {
            var cities = data.cities.filter(function (i, n) {
                return i.state_id == stateId;
            });
            $.each(cities, function (key, entry) {
                dropdown.append($('<option></option>').attr('value', entry.id).text(entry.name));
            })
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