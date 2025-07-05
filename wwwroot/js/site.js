/*$(document).ajaxSend(function (event, request, opt) {
    var securityToken = document.head.querySelector('meta[name="__RequestVerificationToken"]').content;
    request.setRequestHeader("CP-TOKEN", securityToken);
});*/

function isNumberKey(evt) {
    evt = (evt) ? evt : window.event;
    var charCode = (evt.which) ? evt.which : evt.keyCode;
    //if (charCode > 31 && (charCode < 48 || (charCode > 57 && charCode < 96) || charCode > 105) && charCode != 43 && charCode != 45 && charCode != 46) // 45 "-", 43 "+", 46 "."
    //    return false;
    // 110 190.
    // 109 189 -
    // 107 187 =
    var arrA = [45, 43, 46];
    if (charCode > 31 && (charCode < 48 || charCode > 57) && arrA.indexOf(charCode) < 0) // 45 "-", 43 "+", 46 "."
    {
        return false;
    }
    return true;
}

function isNumberAndLetterKey(evt) {
    evt = (evt) ? evt : window.event;
    var charCode = (evt.which) ? evt.which : evt.keyCode;

    if ((charCode >= 48 && charCode <= 57) // 0-9
        || (charCode >= 65 && charCode <= 90)) // a-z
    {
        return true;
    }
    //var inp = String.fromCharCode(event.keyCode);
    //if (/[a-zA-Z0-9-_ ]/.test(inp))

    return false;
}

function ShowPopupDialog(divWrapperPopup, Title, options) {
    let defaultOptions = {
        height: 0,
        idFocus: "",
        confirmClose: true,
    };
    defaultoptions = Object.assign(defaultOptions, options);
    //
    let pHeight = defaultOptions.height;
    let txtIdFocus = defaultOptions.idFocus;

    var _height = pHeight + "px";
    if (pHeight == 0) {
        _height = "auto";
    }

    if ($("#" + divWrapperPopup + " .d-popup-header").length > 0) {
        $("#" + divWrapperPopup + " .d-popup-header").remove();
    }
    // bỏ alert hỏi có cần thoát hay ko nếu yêu cầu có hỏi kho thoát thì mở ra    
    $("#" + divWrapperPopup + " .d-popup").prepend('<div class="d-popup-header"><div class="popup-title">' + Title + '</div><button type="button" onclick="ClosePopupDialog(\'' + divWrapperPopup + '\', ' + (defaultOptions.confirmClose == false ? 'false' : 'true') + ')" class="btn-exit-popup fas fa-times"></button></div>');
    $("#" + divWrapperPopup + " .d-popup").css({ "height": _height });

    $("#" + divWrapperPopup).fadeIn(150).addClass('popup-flex');

    if (txtIdFocus != "") {
        var idFocus = "#" + txtIdFocus;
        $(idFocus).focus().val($(idFocus).val());
    }
    else {
        $('#' + divWrapperPopup + ' .btn-exit-popup').focus();
    }

    $("body").addClass("hide-scroll");
    //fixScrollbar();
    return true;
}

function ClosePopupDialog(divWrapperPopup, confirmClose) {
    confirmClose = confirmClose || false;
    var timer = 150, timerCallback = 200;
    if (confirmClose) {
        nvsConfirm("Bạn có chắc chắn muốn thoát?", function () {
            $("#" + divWrapperPopup).fadeOut(timer);
            setTimeout(function () { $("#" + divWrapperPopup).removeClass('popup-flex'); }, timer);
            $("body").removeClass("hide-scroll");
            //undoScrollbar();

            setTimeout(function () {
                $("#" + divWrapperPopup).trigger("customTriggerClosePopup");
            }, timerCallback);
        });
    }
    else {
        $("#" + divWrapperPopup).fadeOut(timer);
        setTimeout(function () { $("#" + divWrapperPopup).removeClass('popup-flex'); }, timer);
        $("body").removeClass("hide-scroll");
        //undoScrollbar();

        setTimeout(function () {
            $("#" + divWrapperPopup).trigger("customTriggerClosePopup");
        }, timerCallback);
    }
}


// == FIX TABLE

function GetColConfigTable($tartable) {
    try {
        var arrIndex = [];
        //
        var $isDataTable = $.fn.DataTable.isDataTable(document.getElementById($tartable));
        var columnOrder = [];
        var columns = [];
        if ($isDataTable) {
            var $table = $(document.getElementById($tartable)).DataTable();
            columnOrder = Array.apply([], $table.colReorder.order());
            columns = Array.apply([], $table.columns().header());
        }

        // Bên Portal chỉ có xem và di chuyển cột nên viết khác
        // Bản gốc bên admin

        $.each(columns, function (idx, el) {
            var _targetcol = $(this).attr('data-colconfig');
            var _targetidx = Number($(this).attr('data-colindex'));
            var _formula = "";
            var _orderindex = -1;

            if ($isDataTable && columnOrder.length > 0) {
                _orderindex = columnOrder.indexOf(_targetidx);
            }
            else {
                _orderindex = _targetidx;
            }

            arrIndex.push({
                colindex: _targetidx,
                colname: _targetcol,
                orderindex: _orderindex,
                visible: true,
                formula: _formula
            });
        });

        return arrIndex;
    } catch (e) {
        console.log(e);
        return null;
    }
}

function CreateFixedTable($tabletarget, $fixColumnOptions, $options) {
    try {
        let defaultOptions = {
            retrieve: true,
            scrollX: true,
            scrollCollapse: true,
            "info": false,
            "paging": false,
            "ordering": false,
            "filter": false,
            "autoWidth": false,
            "language": {
                oPaginate: {
                    sFirst: "&#x25C4;&#x25C4;",
                    sLast: "&#x25BA;&#x25BA;",
                    sNext: "&#x25BA;",
                    sPrevious: "&#x25C4;"
                },
                sEmptyTable: "Không tìm thấy dữ liệu tìm kiếm",
                sLengthMenu: "Hiển thị _MENU_ bản ghi",
                sInfo: "Hiển thị từ _START_ đến _END_ trên tổng _TOTAL_ bản ghi",
                sInfoEmpty: "Hiển thị từ 0 đến 0 trên tổng 0 bản ghi",
            },
            "aLengthMenu": [10, 20, 50, 100],
        };
        let defaultFixColumnOptions = {
            leftColumns: 2,
            rightColumns: 0
        };

        //
        defaultoptions = Object.assign(defaultOptions, $options)
        defaultFixColumnOptions = Object.assign(defaultFixColumnOptions, $fixColumnOptions)
        defaultoptions.fixedColumns = defaultFixColumnOptions;

        //Measure table
        let tablecontainer = $($tabletarget).closest('#fx-table-container');
        if ($($tabletarget).closest('#fx-table-container').length == 0) {
            tablecontainer = $($tabletarget).closest('[data-fx-table-container]');
        }

        tablecontainer.css({
            "height": tablecontainer.height(),
            "width": "100%"
        });
        tablecontainer.find("> *").addClass('hidden');
        var width = tablecontainer.width();
        tablecontainer.css({
            "height": "auto",
            "width": width
        });
        tablecontainer.find("> *").removeClass('hidden');
        //
        var table = $($tabletarget).DataTable(defaultOptions);
        table.columns.adjust(false);
        table.rows().recalcHeight(false);
        table.draw();
        return table;
    } catch (e) {
        console.log(e);
    }
}

// == FIX TABLE - END

//ham fomat kieu tien te khi nhap textbox


function IsValidNumber(strNumber, grouping) {
    try {
        grouping = grouping == undefined ? true : grouping;
        var regexNumberGrouping = /^[\+\-]?\d{1,3}(((,\d{3})+)?(\.\d+)?)$/; // /^[0-9]\d(((,\d{3}){1})?(\.\d{0,2})?)$/
        var regexNumber = /^[\+,\-]?\d+(\.\d+)?$/;
        return regexNumber.test(strNumber) || regexNumberGrouping.test(strNumber);
    } catch (e) {
        return false;
    }
}

function formatNumber(strNumber) {
    try {
        if ($.type(strNumber).toLowerCase() === "number") {
            strNumber = strNumber.toString();
        }

        if (IsValidNumber(strNumber[0].replace(/,/g, ""), false) == true) {

            var arrTemp = strNumber.split('.');
            if (arrTemp.length > 1) {
                return arrTemp[0].replace(/,/g, "").replace(/(\d)(?=(\d{3})+(?!\d))/g, "$1,") + "." + arrTemp[1];
            }
            else {
                return arrTemp[0].replace(/,/g, "").replace(/(\d)(?=(\d{3})+(?!\d))/g, "$1,");
            }
        }
        else {
            return strNumber;
        }
    } catch (e) {
        return strNumber;
    }
}
function validateNumberWithoutComma(ctrl) {
    var nStr = ctrl.value;
    var txtControlId = ctrl.id;
    try {
        var _Vondieule = nStr;
        var _tempvondieule = "";
        var _newtemp = "";
        var _Regex = new RegExp('^[0-9]+$');
        //var _RegexChar = new RegExp('[|,}{+&-=!?;/#\"$%^*()<>`~[]\\]+$');     
        for (var i = 0; i < _Vondieule.length - 1; i++) {// cat het nhung ky tu khong phai la so
            _newtemp += _Vondieule[i];
            if (_Regex.test(_newtemp)) {
                _tempvondieule = _newtemp;
            }
            else {
                $("input[id=" + txtControlId + "]").val(_tempvondieule);
                return;
            }
        }
        if (_Regex.test(_Vondieule))//neu la so thi lay
        {
            _Vondieule = nStr;

        }
        else {
            _Vondieule = _tempvondieule;
        }
        nStr = _Vondieule;
        $("input[id=" + txtControlId + "]").val(nStr);
    } catch (e) {
        $("input[id=" + txtControlId + "]").val(nStr);
    }
}

function jsFormatNumber(ctrl) {
    var nStr = ctrl.value;
    var txtControlId = ctrl.id;
    if (nStr.indexOf('.') != -1)//neu nhap vao dang 1.000.000.000 thi doi thanh 1,000,000,000
    {
        // thay tat ca dau '.' bang dau ','         
        while (nStr.indexOf('.') > -1) {
            nStr = nStr.replace('.', '');
        }
    }
    var _Vondieule = nStr;
    var _tempvondieule = "";
    var _newtemp = "";
    var _Regex = new RegExp('^[0-9,]+$');
    //var _RegexChar = new RegExp('[|,}{+&-=!?;/#\"$%^*()<>`~[]\\]+$');     
    for (var i = 0; i < _Vondieule.length - 1; i++) {// cat het nhung ky tu khong phai la so
        _newtemp += _Vondieule[i];
        if (_Regex.test(_newtemp)) {
            _tempvondieule = _newtemp;
        }
        else {
            $("input[id=" + txtControlId + "]").val(_tempvondieule);
            return;
        }
    }
    if (_Regex.test(_Vondieule))//neu la so thi lay
    {
        _Vondieule = nStr;

    }
    else {
        _Vondieule = _tempvondieule;
    }
    nStr = _Vondieule;

    // cắt toàn bộ số 0 trước trường số
    nStr = nStr.replace(/[,]/g, '');
    while (nStr.indexOf(0) == '0' && nStr.length > 1) {
        nStr = nStr.substring(1);
    }

    var before_length = ctrl.value.length; // lấy chiều dài trước khi thay đổi
    var key_index = doGetCaretPosition(ctrl); // lấy vị trí con trỏ hiện tại

    // 
    //nStr += '';
    x = nStr.split(',');
    x1 = x[0];
    x2 = x.length > 1 ? ',' + x[1] : '';
    var rgx = /(\d+)(\d{3})/;
    while (rgx.test(x1)) {
        x1 = x1.replace(rgx, '$1' + ',' + '$2');
    }
    var result = x1 + x2;
    $("input[id=" + txtControlId + "]").val(result);

    var after_length = document.getElementById(txtControlId).value.length; // lấy thay đổi chiều dài
    setCaretPosition(ctrl, key_index + (parseInt(after_length) - parseInt(before_length))); // set vị trí con trỏ
}

function doGetCaretPosition(ctrl) {
    var CaretPos = 0;   // IE Support
    if (document.selection) {
        ctrl.focus();
        var Sel = document.selection.createRange();
        Sel.moveStart('character', -ctrl.value.length);
        CaretPos = Sel.text.length;
    }
    // Firefox support
    else if (ctrl.selectionStart || ctrl.selectionStart == '0')
        CaretPos = ctrl.selectionStart;
    return (CaretPos);
}

// fomat kiểu number có đấu , ở hàng nghìn và có cả phần thập phân
function jsFormatFloatNumber(input, fixNum) {
    var nStr = input.value;
    var _Regex = new RegExp('^[0-9,.]+$');
    var _tempvondieule = "";
    for (var i = 0; i < nStr.length; i++) {
        if (_Regex.test(nStr[i])) {
            _tempvondieule += nStr[i];
        }
    }
    nStr = _tempvondieule.replace(/,/g, '');

    var _IndexFloat = nStr.indexOf('.');
    var _PhanThapPhan = "";
    var _count_ = (nStr.split(".").length - 1);
    if (_count_ > 1) {
        var Fst = nStr.indexOf('.');
        var nStrTemp = nStr.substring(0, Fst) + "." + nStr.substring(Fst + 1).replace(/\./g, '');
        nStr = nStrTemp;
    }

    if (_IndexFloat >= 0) {
        _IndexFloat = nStr.indexOf('.');
        _PhanThapPhan = nStr.substring(_IndexFloat);
        _PhanThapPhan = fixNum != undefined ? _PhanThapPhan.substr(0, fixNum + 1) : _PhanThapPhan;
        nStr = nStr.substring(0, _IndexFloat);
    }

    while (nStr.length > 1 && nStr[0] == '0') {
        nStr = nStr.substring(1);
    }

    var before_length = input.value.length;
    var key_index = doGetCaretPosition(input);

    x1 = nStr;
    var rgx = /(\d+)(\d{3})/;
    while (rgx.test(x1)) {
        x1 = x1.replace(rgx, '$1' + ',' + '$2');
    }

    var result = x1 + _PhanThapPhan;
    input.value = result;

    var after_length = input.value.length;
    setCaretPosition(input, key_index + (after_length - before_length));
}

function jsFormatFloatNumberNoDot(nStr, txtControlId, fixNum) {

    var _Regex = new RegExp('^[0-9,]+$');
    var _tempvondieule = "";
    for (var i = 0; i < nStr.length; i++) {// cat het nhung ky tu khong phai la so
        if (_Regex.test(nStr[i])) {
            _tempvondieule += nStr[i];
        }
    }
    nStr = _tempvondieule.replace(/,/g, '');

    var _IndexFloat = nStr.indexOf('.');
    var _PhanThapPhan = "";
    var _count_ = 0;
    _count_ = (nStr.split(".").length - 1);
    if (_count_ > 1)// nếu có tồn tại 2 dấu . thì cắt dấu toàn bộ từ dấu . thứ 2 đến hết
    {
        var Fst = nStr.indexOf('.');
        var Snd = nStr.indexOf('.', Fst + 1);// vị trí index của thằng . thứ 2
        var nStrTemp = nStr.substring(0, Fst) + "." + nStr.substring(Fst + 1).replace(/\./g, '');// xóa hết từ thằng . thứ 2
        nStr = nStrTemp;
    }

    //nStr = fixNum != undefined && nStr.indexOf('.') != nStr.length - 1 ? (Number(Number(nStr).toFixed(fixNum)) / 1).toString() : nStr;

    if (_IndexFloat >= 0)// nếu có dấu . thì mới làm tiếp
    {
        _PhanThapPhan = nStr.substring(_IndexFloat, nStr.length);
        _PhanThapPhan = fixNum != undefined ? _PhanThapPhan.substr(0, fixNum + 1) : _PhanThapPhan;

        // cắt lấy phần nguyên để format trước
        nStr = nStr.substring(0, _IndexFloat);
    }

    // cắt toàn bộ số 0 trước trường số
    nStr = nStr.replace(/[,]/g, '');
    while (nStr.indexOf(0) == '0' && nStr.length > 1) {
        nStr = nStr.substring(1);
    }
    var ctrl = document.getElementById(txtControlId);

    var before_length = ctrl.value.length; // lấy chiều dài trước khi thay đổi
    var key_index = doGetCaretPosition(ctrl); // lấy vị trí con trỏ hiện tại

    x = nStr.split(',');
    x1 = x[0];
    x2 = x.length > 1 ? ',' + x[1] : '';
    var rgx = /(\d+)(\d{3})/;
    while (rgx.test(x1)) {
        x1 = x1.replace(rgx, '$1' + ',' + '$2');
    }
    var result = x1 + x2;
    $("input[id=" + txtControlId + "]").val(result + _PhanThapPhan);

    var after_length = document.getElementById(txtControlId).value.length; // lấy thay đổi chiều dài
    setCaretPosition(ctrl, key_index + (parseInt(after_length) - parseInt(before_length))); // set vị trí con trỏ
}
function jsFormatRatio(ctrl) {
    var nStr = ctrl.value; var txtControlId = ctrl.id;
    var _Regex = new RegExp('^[0-9.:]');
    var _tempvondieule = "";
    for (var i = 0; i < nStr.length; i++) {
        if (_Regex.test(nStr[i])) {
            _tempvondieule += nStr[i];
        }
    }
    nStr = _tempvondieule.replace(/,/g, '');

    var indexOfColon = nStr.indexOf(':');
    if (indexOfColon == 0) nStr = "";
    var _count_ = 0;
    _count_ = (nStr.split(":").length - 1);
    if (_count_ > 1)// nếu có tồn tại 2 dấu : thì cắt dấu toàn bộ từ dấu : thứ 2 đến hết
    {
        var Fst = nStr.indexOf(':');
        var Snd = nStr.indexOf(':', Fst + 1);// vị trí index của thằng : thứ 2
        var nStrTemp = nStr.substring(0, Fst) + ":" + nStr.substring(Fst + 1).replace(/\:/g, '');// xóa hết từ thằng : thứ 2
        nStr = nStrTemp;
    }
    $("input[id=" + txtControlId + "]").val(nStr);
}

function setCaretPosition(ctrl, pos) {
    if (ctrl.setSelectionRange) {
        ctrl.focus();
        ctrl.setSelectionRange(pos, pos);
    }
    else if (ctrl.createTextRange) {
        var range = ctrl.createTextRange();
        range.collapse(true);
        range.moveEnd('character', pos);
        range.moveStart('character', pos);
        range.select();
    }
}

function CompareValidNumber(strNum1, strNum2) {
    if (strNum1 == "" || strNum2 == "" || parseFloat(strNum1.replace(/,/g, "")) <= 0 || parseFloat(strNum2.replace(/,/g, "")) <= 0) {
        return true;
    }
    if (parseFloat(strNum1.replace(/,/g, "")) <= parseFloat(strNum2.replace(/,/g, ""))) {
        return true
    }
    return false;
}
// convert  object's key from upper to lower (1st character) 
function toCamel(o) {
    var newO, origKey, newKey, value;
    if (o instanceof Array) {
        return o.map(function (value) {
            if (typeof value === "object") {
                value = toCamel(value);
            }
            return value;
        });
    } else {
        newO = {};
        for (origKey in o) {
            if (o.hasOwnProperty(origKey)) {
                newKey = (origKey.charAt(0).toLowerCase() + origKey.slice(1) || origKey).toString();
                value = o[origKey];
                if (value instanceof Array || (value !== null && value.constructor === Object)) {
                    value = toCamel(value);
                }
                newO[newKey] = value;
            }
        }
    }
    return newO;
}

function toCamelUpper(o) {
    var newO, origKey, newKey, value;
    if (o instanceof Array) {
        return o.map(function (value) {
            if (typeof value === "object") {
                value = toCamelUpper(value);
            }
            return value;
        });
    } else {
        newO = {};
        for (origKey in o) {
            if (o.hasOwnProperty(origKey)) {
                newKey = (origKey.charAt(0).toUpperCase() + origKey.slice(1) || origKey).toString();
                value = o[origKey];
                if (value instanceof Array || (value !== null && value.constructor === Object)) {
                    value = toCamelUpper(value);
                }
                newO[newKey] = value;
            }
        }
    }
    return newO;
}

function formatNullOrUndefined(str) {
    try {
        if (str == "undefined" || str == "null") {
            return "";
        } else {
            return str;
        }
    } catch (e) {
        return str;
    }
}
// -- CHECK DATE
function isEmptyOrValid_InputDate(strDate) {
    if (strDate == "" || strDate == "__/__/____") {
        return true;
    }
    if (isDate_ddMMyyyy(strDate)) {
        return true;
    }
    return false;
}

function CheckValidDate(strDate, outMsg) {
    if (strDate == "" || strDate == "__/__/____") {
        outMsg.value = " không được để trống";
        return false;
    }
    if (!isDate_ddMMyyyy(strDate)) {
        outMsg.value = " không hợp lệ";
        return false;
    }
    return true;
}

function isPositiveNumber(strNum, outMsg) {
    if (strNum == "") {
        outMsg.value = " không được để trống";
        return false;
    }
    if (parseFloat(strNum.replace(/,/g, '')) <= 0) {
        outMsg.value = "phải lớn hơn 0";
        return false;
    }
    return true;
}

function isYear_yyyy(strYear) {
    var currVal = strYear;
    if (currVal == '')
        return false;
    var rxDatePattern = /^(\d{4})$/;
    var dtArray = currVal.match(rxDatePattern); // is format OK?
    if (dtArray == null)
        return false;
    return true;
}

function isMonth_MMyyyy(strDate) {
    var currVal = strDate;
    if (currVal == '')
        return false;
    var rxDatePattern = /^(\d{2})(\/|-)(\d{4})$/;
    var dtArray = currVal.match(rxDatePattern); // is format OK?
    if (dtArray == null)
        return false;
    //Checks for MM/yyyy format.
    dtMonth = dtArray[1];
    dtYear = dtArray[3];
    if (dtMonth < 1 || dtMonth > 12)
        return false;
    return true;
}

function isDate_ddMMyyyy(strDate) {
    try {
        var currVal = strDate;
        if (currVal == '')
            return false;
        if (currVal == '01/01/0001')
            return false;
        var rxDatePattern = /^(\d{2})(\/|-)(\d{2})(\/|-)(\d{4})$/;
        var dtArray = currVal.match(rxDatePattern); // is format OK?
        if (dtArray == null)
            return false;
        //Checks for dd/mm/yyyy format.
        dtDay = dtArray[1];
        dtMonth = dtArray[3];
        dtYear = dtArray[5];
        if (dtMonth < 1 || dtMonth > 12)
            return false;
        else if (dtDay < 1 || dtDay > 31)
            return false;
        else if (dtYear <= 0)
            return false;
        else if ((dtMonth == 4 || dtMonth == 6 || dtMonth == 9 || dtMonth == 11) && dtDay == 31)
            return false;

        else if (dtMonth == 2) {
            var isleap = (dtYear % 4 == 0 && (dtYear % 100 != 0 || dtYear % 400 == 0));
            if (dtDay > 29 || (dtDay == 29 && !isleap))
                return false;
        }
        return true;
    } catch (e) {
        return false;
    }
}

function CheckCompare2Date_ddMMyyyy(strDate1, strDate2, compareType) {
    try {
        var _compareType = compareType || '>=';
        var rxDatePattern = /^(\d{2})(\/|-)(\d{2})(\/|-)(\d{4})$/;

        // Check xem có phải string date không đã
        var dtArray_1 = strDate1.match(rxDatePattern); // is format OK?
        var dtArray_2 = strDate2.match(rxDatePattern); // is format OK?

        if (dtArray_1 == null || dtArray_2 == null)
            return false
        // Convert sang date để so sanhs
        var _date_1 = new Date(dtArray_1[5], dtArray_1[3] - 1, dtArray_1[1]);
        var _date_2 = new Date(dtArray_2[5], dtArray_2[3] - 1, dtArray_2[1]);
        if (_compareType == ">=" && _date_1 > _date_2) {
            return false;
        }
        if (_compareType == ">" && _date_1 >= _date_2) {
            return false;
        }
        return true;
    }
    catch (e) {
        return false;
    }
}

function CheckCompareValidDate(strDate1, strDate2, compareType) {
    var _compareType = compareType || '>=';
    if (!isDate_ddMMyyyy(strDate1)) {
        return true;
    }
    if (!isDate_ddMMyyyy(strDate2)) {
        return true;
    }
    if (CheckCompare2Date_ddMMyyyy(strDate1, strDate2, _compareType)) {
        return true;
    }
    return false;
}

function StringToDate_ddMMyyyy(strDate, strDelemiter) {
    strDelemiter = strDelemiter || '/';

    if (isDate_ddMMyyyy(strDate) == false) {
        return null;
    }

    var dateParts = strDate.split(strDelemiter);
    var year = dateParts[2];
    var month = dateParts[1];
    var day = dateParts[0];

    if (isNaN(day) || isNaN(month) || isNaN(year))
        return null;
    return new Date(year, month - 1, day);

}

function DateToString_ddMMyyyy(dDate, strDelemiter) {
    strDelemiter = strDelemiter || '/';

    var _month = dDate.getMonth() + 1;
    var _day = dDate.getDate();
    var _year = dDate.getFullYear();
    if (_day < 10) {
        _day = '0' + _day;
    }
    if (_month < 10) {
        _month = '0' + _month;
    }

    return _day + strDelemiter + _month + strDelemiter + _year;
}
function FormatDate_Import(strDate) {
    if (strDate.length > 10) {
        return DateToString_ddMMyyyy(new Date(strDate));
    } else {
        return strDate;
    }
}
Date.prototype.addHours = function (h) {
    var copiedDate = new Date(this.getTime());
    copiedDate.setHours(copiedDate.getHours() + h);
    return copiedDate;
}

Date.prototype.addDays = function (h) {
    var copiedDate = new Date(this.getTime());
    copiedDate.setDate(copiedDate.getDate() + h);
    return copiedDate;
}

// -- END CHECK DATE

// CHECK EMAIL

function IsValidEmail(email) {
    var reg = /^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$/
    if (reg.test(email)) {
        return true;
    }
    else {
        return false;
    }
}

// END CHECK EMAIL

// CHECK PHONENUMBER

function IsValidPhone(phone) {
    try {
        phone = phone || "";
        phone = phone.replace('(+84)', '0');
        phone = phone.replace('+84', '0');
        phone = phone.replace('0084', '0');
        phone = phone.replace(/ /g, '');

        return /^0[1-9](\d{8}|\d{9})$/.test(phone);
    } catch (e) {
        console.log(e.message);
        return false;
    }
}

// END CHECK PHONENUMBER

// CHECK UNICODE

function IsContainUnicode(pStrString) {
    try {
        strRegex = /à|á|ạ|ả|ã|â|ầ|ấ|ậ|ẩ|ẫ|ă|ằ|ắ|ặ|ẳ|ẵ|è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ|ì|í|ị|ỉ|ĩ|ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ|ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ|ỳ|ý|ỵ|ỷ|ỹ|đ/gi;
        return strRegex.test(pStrString);
    } catch (e) {
        return false;
    }
}

function CheckPasswordRule(value) {
    if (!/\d/.test(value)) //digit
        return false;
    if (!/[a-z]/.test(value)) // lowercase
        return false;
    if (!/[A-Z]/.test(value)) // uppercase
        return false;
    if (!/[^0-9a-zA-Z]/.test(value)) { // special characters
        return false;
    }
    return true;
}

// CHECK UNICODE - END

//CHECK ONLY NUMBER
function IsValidOnlyNumber(cmt) {
    try {
        cmt = cmt || "";
        cmt = cmt.replace(/ /g, '');
        return /[0-9]$/.test(cmt);
    } catch (e) {
        console.log(e.message);
        return false;
    }
}
//END CHECK

//trim input text 
function trimInput() {
    var len = $("input[type=text]").length;
    for (var x = 0; x < len; x++) {
        var str = $("input[type=text]:eq( " + x + " )").val() || "";
        var newRes = str.trim();
        $("input[type=text]:eq( " + x + " )").val(newRes);
    }
    var lenArea = $("textarea").length;
    for (var x = 0; x < lenArea; x++) {
        var str = $("textarea:eq( " + x + " )").val() || "";
        var newRes = str.trim();
        $("textarea:eq( " + x + " )").val(newRes);
    }
}

function showStatusComment($comment) {
    try {
        let htmlDisplay = '<div class="flex-row">';
        htmlDisplay += '<div class="flex-col-12"><span class="input-content text-left">' + $comment + '</span></div>';
        htmlDisplay += '</div>';

        window.swal({
            title: "LÝ DO TỪ CHỐI DUYỆT",
            text: "",
            html: htmlDisplay,
            showCancelButton: false,
            allowOutsideClick: false,
            //allowEscapeKey: false,
        }).then(function () {
            //
        }, function () {
            //
        });;
    } catch (e) {
    }
}
function showViewDescription($viewName, $soCV, $ngayCV, $lyDo) {
    try {
        let htmlDisplay = '<div class="flex-row">' +
            '<div class="flex-col-2"><i class="input-title">Số CV</i></div>' +
            '<div class="flex-col-4">' +
            '<b class="input-content">' + $soCV + '</b>' +
            '</div>' +
            '<div class="flex-col-2"><i class="input-title">Ngày CV</i></div>' +
            '<div class="flex-col-4">' +
            '<b class="input-content">' + $ngayCV + '</b>' +
            '</div>' +
            '</div>' +
            '<div class="flex-row">' +
            '<div class="flex-col-2"><i class="input-title">Lý do</i></div>' +
            '<div class="flex-col-10">' +
            '<b class="input-content">' + $lyDo + '</b>' +
            '</div>' +
            '</div>';
        window.swal({
            title: $viewName,
            text: "",
            html: htmlDisplay,
            showCancelButton: false,
            allowOutsideClick: false,
            //allowEscapeKey: false,
        }).then(function () {
            //
        }, function () {
            //
        });;
    } catch (e) {
    }
}

$(document).ready(function () {
    $(".side-menu > ul > li > ul").hide();
    $(".rotate-down .menucate-lv2").show();
    $(".side-menu > ul > li > a").click(function () {
        $(".side-menu > ul > li").removeClass('rotate-down');

        $(".side-menu > ul > li > ul").slideUp(300);
        if (!$(this).next(".side-menu > ul > li > ul").is(":visible")) {
            $(this).next(".side-menu > ul > li > ul").slideDown(300);
            $(this).parent().addClass('rotate-down');
        }
    });
    $('.side-menu > ul > li.active').slideDown();
});
function addDays(date, num) {
    var newDate = new Date(date.getTime() + num * 86400000);
    return DateToString_ddMMyyyy(newDate);
}

function addMonths(date, num) {
    var newDate = new Date(date.setMonth(date.getMonth() + num));
    return DateToString_ddMMyyyy(newDate);
}

function addYears(date, num) {
    var newDate = new Date(date.setFullYear(date.getFullYear() + num));
    return DateToString_ddMMyyyy(newDate);
}

//Check không cho nhập các ký tự đặc biệt
function isValidSpecialChar(str) {
    return !/[~`!@@#$%\^&*()+=\-\[\]\\';,/{}|\\":<>\?.\s]/g.test(str);
}
function CheckKyTuDacBietShoKyTu(str) {
    try {
        var _specialKey = '|,}{@+&=!?;/#\"$%^*()<>`~[]\\';
        for (var i = 0; i < _specialKey.length; i++) {

            if (str.indexOf(_specialKey[i]) != -1) {
                return _specialKey[i];
            }
        }
        return '';
    } catch (e) {
        alert('Loi hàm CheckKyTuDacBiet' + e.message); return -1;
    }
}
//  
// Hàm check dữ liệu nhập vào có chứa kỹ tự kiểu unicode hay không 
// return true nếu ko chứ 
// return false nếu có
function FuncCheckUnikey(pStrString, p_MsgErr, p_Tag) {
    var VietNamKey = "á,à,ạ,ả,ã,â,ấ,ầ,ậ,ẩ,ẫ,ă,ắ,ằ,ặ,ẳ,ẵ,é,è,ẹ,ẻ,ẽ,ê,ế,ề,ệ,ể,ễ,ó,ò,ọ,ỏ,õ,ô,ố,ồ,ộ,ổ,ỗ,ơ,ớ,ờ,ợ,ở,ỡ,ú,ù,ụ,ủ,ũ,ư,ứ,ừ,ự,ử,ữ,í,ì,ị,ỉ,ĩ,đ,ý,ỳ,ỵ,ỷ,ỹ,Á,À,Ạ,Ả,Ã,Â,Ấ,Ầ,Ậ,Ẩ,Ẫ,Ă,Ắ,Ằ,Ặ,Ẳ,Ẵ,É,È,Ẹ,Ẻ,Ẽ,Ê,Ế,Ề,Ệ,Ể,Ễ,Ó,Ò,Ọ,Ỏ,Õ,Ô,Ố,Ồ,Ộ,Ổ,Ỗ,Ơ,Ớ,Ờ,Ợ,Ở,Ỡ,Ú,Ù,Ụ,Ủ,Ũ,Ư,Ứ,Ừ,Ự,Ử,Ữ,Í,Ì,Ị,Ỉ,Ĩ,Đ,Ý,Ỳ,Ỵ,Ỷ,Ỹ";
    $("#divErr").remove();
    for (var i = 0; i < pStrString.length; i++) {
        if (VietNamKey.indexOf(pStrString[i]) != -1) {
            var _Err = " <div id=\"divErr\" style=\"color:red;padding-top:15px;clear:both; padding-bottom:10px ;\">" + p_MsgErr + "</div>";
            var _tagHtml = "#" + p_Tag;
            $(_tagHtml).append(_Err);
            return -1;
        }
    }
    return 0;
}

function FomartNumberCurency(nStr, txtControlId) {
    var _Vondieule = nStr;
    var _tempvondieule = "";
    var _newtemp = "";
    var _Regex = new RegExp('^[0-9]+$');
    //var _RegexChar = new RegExp('[|,}{+&-=!?;/#\"$%^*()<>`~[]\\]+$');     
    for (var i = 0; i < _Vondieule.length; i++) {// cat het nhung ky tu khong phai la so
        _newtemp += _Vondieule[i];
        if (_Regex.test(_newtemp)) {
            _tempvondieule = _newtemp;
        }
    }
    if (_Regex.test(_Vondieule))//neu la so thi lay
    {
        _Vondieule = nStr;

    }
    else {
        _Vondieule = _tempvondieule;
    }
    $("input[id=" + txtControlId + "]").val(_tempvondieule);
    return true;
}