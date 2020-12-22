var uploadField = document.getElementById("file");
var scholarshipFundValue = '';

window.onload = GetRequest();


function GetUrlRequest() {
    version = (url_base.length >= 7) ? "/" + url_base[3] : "";

    if (version == '/Scholarship') {
        version = '';
    }
    return version;
}


function GetRequest() {
    // Agregando los datos de solicitud
    var requestData = new FormData();
    requestData.append('requestData', data_code_split);
    
    $.ajax({
        url: version + '/Scholarship/GetRequirements',
        type: "POST",
        contentType: false,
        processData: false, 
        data: requestData,
        success: function (result) {
            //console.log(result);
            data = JSON.parse(result);
            newRows = "";
            if (data.status != 'False' && data.status != 'false') {
                newRows = GetRowsRequest(data.dataRequest);
            } else {
                $("#nomb_beca").text("NO ENCONTRADO");
                newRows = '<tr class="table-light"><td class="text-center nume-row align-middle text-body-table" scope="row"> </td><td class="text-body-table" id="TEXT_ERROR">' + data.message + '</td><td class="align-middle"> </td></tr>';
            }
            $('#schola-ship-table tr:last').after(newRows);
        },
        error: function (error, status) {
            $("#nomb_beca").text("NO ENCONTRADO");
            newRows = '<tr class="table-light"><td class="text-center nume-row align-middle text-body-table" scope="row"> </td><td class="text-body-table" id="TEXT_ERROR">' +
                "Ha ocurrido un error mientras se recuperaba los requistios de su solicitud, favor intentar más tarde." +
                '</td><td class="align-middle"> </td></tr>';
            $('#schola-ship-table tr:last').after(newRows);
        },
        complete: function () {
            $('#load-request').fadeOut();
            $("#pagebody").fadeIn();
        },
        timeout: 50000 // sets timeout to 50 seconds
    });

   
}


function GetRowsRequest(dataRequest) {
    rowResponse = "";
    statusLock = dataRequest.STATUS_LOCK;
    becaDesc = dataRequest.BECA_DESC;
    scholarshipFundValue = dataRequest.BECA_CODE;
    requirements = dataRequest.REQUIREMENTS;

    $("#nomb_beca").text(becaDesc);

    for (var i = 0; i < requirements.length; i++) {
        textComment = (requirements[i].COMMENT != null) ? requirements[i].COMMENT : "";
        requiredRequest = (requirements[i].REQUIRED == "Y") ? "*" : '';

        linkUpload = (requirements[i].UPLOAD_STATUS == true) ? '<a class="link-modal" title = "Cargar adjunto" href = "javascript:void(0)" onclick = "OpenModalUploadFile(\'' + requirements[i].KVRAREQ_TREQ_CODE + '\')" > ' +
           '<i class="fas fa-file-upload medium-icons color-gey"></i>' +
            '</a>' : '';

        linkView = (requirements[i].URL_REQ != null && requirements[i].URL_REQ != "") ? '<a class="link-modal" title="Ver adjunto" href="' + requirements[i].URL_REQ + '" id="' + requirements[i].URL_REQ + '" target="_blank"><i class="fas fa-eye medium-icons color-gey"></i></a>' : ' <i title="Sin adjunto" class="fas fa-eye-slash medium-icons color-gey"></i>';

        rowResponse = rowResponse +
            '<tr class="table-light">' +
                '<td class="text-center nume-row align-middle text-body-table" scope = "row" >' + (i + 1) + '</td >' +
                '<td class="text-body-table" id="TEXT_' + requirements[i].KVRAREQ_TREQ_CODE + '">' + requirements[i].KVVTREQ_DESC + ' <b>' + requiredRequest + '</b> </td>' +
                '<td class="text-body-table" >' + textComment + '</td >' +
                '<td class="text-body-table" ><b>' + requirements[i].KVVTRST_DESC  + '</b></td >' +
                '<td class="align-middle">' +
                    '<p class="text-center align-middle quit-mg-top" id="' + requirements[i].KVRAREQ_TREQ_CODE + '">' +
                        linkView +
                        linkUpload +
                    '</p>' +
                '</td>' +
            '</tr >';
    }

    if (requirements.length <= 0) {
        rowResponse = rowResponse + '<tr class="table-light"><td class="text-center nume-row align-middle text-body-table" scope="row"> </td><td class="text-body-table" id="TEXT_ERROR">Sin requisitos asignados</td><td class="align-middle"> </td></tr>';
    }

    return rowResponse;
}


uploadField.onchange = function () {
    if (this.files[0].size > limitSizeFileValue) {
        var text_error = document.getElementById("error_text");
        text_error.innerHTML = "Error en los datos enviados, no se permite enviar archivos de más de " + limitSizeFileNameValue+'.';
        this.value = "";
    };
};

$(".custom-file-input").on("change", function () {
    var fileName = $(this).val().split("\\").pop();
    $(this).siblings(".custom-file-label").addClass("selected").html(fileName);
});

function OpenModalUploadFile(requestCode) {

    textReq = $("#TEXT_" + requestCode).text();
    $("#req-name").html("<b>Requisito:</b> " + textReq);
    $("#request_code").val(requestCode);
    $("#file").val('');
    $("#name-file").text(formato_perm);
    $("#error_text").text('');
    loaderClass = document.getElementById('loaderUploadFile').getAttribute("class");

    if (loaderClass != "d-none") {
        HideLoader("#loaderUploadFile", "#btn-send-utpl");
    }

    $('#uploadFile').modal('show');
}

function OpenModalDeleteFile(requestCode) {
    $("#request_code_delete").val(requestCode)
    $("#error_text_delete").text('');

    loaderClass = document.getElementById('loaderDeleteFile').getAttribute("class");

    if (loaderClass != "d-none") {
        HideLoader("#loaderDeleteFile", "#btn-dele-utpl");
    }

    $('#deleteFile').modal('show');
}

function ShowLoader(idLoader, btn) {
    $(idLoader).removeClass("d-none");
    $(idLoader).addClass("spinner-border text-secondary");
    $(btn).attr("disabled", true);
}

function HideLoader(idLoader, btn) {
    $(idLoader).attr('class', "d-none");
    $(btn).attr("disabled", false);
}

function UploadFileUser()
{
    nomb_beca = $("#nomb_beca").text();
    requestCode = $("#request_code").val();
    ShowLoader("#loaderUploadFile", "#btn-send-utpl");
    textReq = $("#TEXT_" + requestCode).text();
    
    //Obtencion de ruta base para consumo de solicitud POST
    url_base = document.URL;
    var ruta_valida = url_base.includes('Scholarship');
    if (ruta_valida) {
        url_base = url_base.split("Scholarship");
        url_token = document.URL;
        url_token = url_token.split("Index/");
        token = url_token[url_token.length - 1];

        // Agregando los datos de archivo y codigo de requisito
        var fileData = new FormData();
        fileData.append('scholarShipName', nomb_beca);
        fileData.append('scholarshipFundValue', scholarshipFundValue);
        fileData.append('requestCode', requestCode);
        fileData.append('requestDesc', textReq);
        fileData.append('file', $("#file")[0].files[0]);
        fileData.append('requestData', token);
    }

    
    // Valida la informacion enviada en el formulario
    if (window.FormData !== undefined && ruta_valida) {
        $.ajax({
            url: version + '/Scholarship/Upload',
            type: "POST",
            contentType: false,
            processData: false, // No procesar la data
            data: fileData,
            success: function (result) {
                //console.log(result);
                data = JSON.parse(result);
                if (data.status != 'False' && data.status != 'false') {
                    //console.log(data);
                    var viewIcon = '<a class="link-modal" title="Ver adjunto" href="' + data.url + '" target="_blank">' +
                        '<i class="fas fa-eye medium-icons color-gey"></i>                                    ' +
                        '</a>';
                    var updateIcon = '<a class="link-modal" title="Cargar adjunto" href="javascript:void(0)" onclick="OpenModalUploadFile(\'' + data.requestCode + '\')">' +
                        '<i class="fas fa-file-upload medium-icons color-gey"></i>                                    ' +
                        '</a>'; 

                    newContent = viewIcon + updateIcon;                    
                    //newContent = viewIcon;                    
                    contentModal = $('#' + data.requestCode).html(newContent);
                    $('#uploadFile').modal('hide');
                } else {
                    if ($("#request_code").val() == data.requestCode) {
                        document.getElementById("error_text").innerHTML = data.message;
                    }                    
                }
            },
            error: function ( error, status) {
                //console.log(xhr);
                //console.log(error, status);
                HideLoader("#loaderUploadFile", "#btn-send-utpl");
                document.getElementById("error_text").innerHTML = error;
            },
            complete: function () {
                HideLoader("#loaderUploadFile", "#btn-send-utpl");
            },
            timeout: 50000 // sets timeout to 50 seconds
        });
    } else {
        HideLoader("#loaderUploadFile", "#btn-send-utpl");
        document.getElementById("error_text").innerHTML = "Ha ocurrido un error mientras se  procesaba su solicitud, favor intentar más tarde.";
    }
}



