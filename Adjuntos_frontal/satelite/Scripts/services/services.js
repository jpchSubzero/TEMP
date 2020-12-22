var uploadField = document.getElementById("file");
var name_cookie = "vctOSS";

window.onload = InitialLoad();

uploadField.onchange = function () {
    if (this.files[0].size > 5242880) {
        var text_error = document.getElementById("error_text");
        text_error.innerHTML = "Error en los datos enviados, no se permite enviar archivos de más de 5 MB.";
        this.value = "";
    };
};

$(".custom-file-input").on("change", function () {
    var fileName = $(this).val().split("\\").pop();
    $(this).siblings(".custom-file-label").addClass("selected").html(fileName);
});



function GetTokenURL() {
    url_base = document.URL;
    var ruta_valida = url_base.includes('Services');
    if (ruta_valida) {
        url_base = url_base.split("Services");
        url_token = document.URL;
        url_token = url_token.split("Index/");
        token = url_token[url_token.length - 1];
    }

    return [url_base, ruta_valida, token];
}

function ValidRequest(token) {
    var cookie_value = GetCookie(name_cookie); 

    if (cookie_value == token) {
        return false;
    } else {
        return true;
    }
}

function InitialLoad() {
    var cookie_exist = GetCookie(name_cookie);

    if (cookie_exist != null) {
        DeleteCookie(name_cookie);
    }
}



function SetCookie(name, value, days=1) {
    var expires = "";
    if (days) {
        var date = new Date();
        date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
        expires = "; expires=" + date.toUTCString();
    }
    document.cookie = name + "=" + (value || "") + expires + "; path=/";
}

function DeleteCookie(name = name_cookie) {
    document.cookie = name + '=; path=/; Expires=Thu, 04 Feb 1993 00:00:01 GMT;';
}



function GetCookie(name) {
    var nameEQ = name + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') c = c.substring(1, c.length);
        if (c.indexOf(nameEQ) == 0) return c.substring(nameEQ.length, c.length);
    }
    return null;
}





$("#formCargArch").submit(function (e) {
    e.preventDefault();

    //Obtencion de ruta base para consumo de solicitud POST
    var url_info = GetTokenURL();

    url_base = url_info[0];
    var ruta_valida = url_info[1];
    var token = url_info[2];

    if (ValidRequest(token)) {

        SetCookie(name_cookie, token)

        $("#loader").removeClass("d-none");
        $("#loader").addClass("spinner-border text-secondary");
        $("#btn-send-utpl").attr("disabled", true);

        if (ruta_valida) {
            // Agregando los datos de archivo y token
            var fileData = new FormData();

            fileData.append('Token', token);
            fileData.append('file', $("#file")[0].files[0]);
        }

        // Valida la informacion enviada en el formulario
        if (window.FormData !== undefined && ruta_valida) {
            $.ajax({
                url: version + '/Services/UpdateFile',
                type: "POST",
                contentType: false,
                processData: false, // No procesar la data
                data: fileData,
                success: function (result) {
                    data = JSON.parse(result)
                    if (data.status != 'False' && data.status != 'false') {
                        DeleteCookie();
                        window.location.href = ambiente + reviSoliReal;
                    } else {
                        DeleteCookie();
                        document.getElementById("error_text").innerHTML = data.message;
                    }
                },
                error: function (err) {
                    DeleteCookie();
                    $("#loader").attr('class', "d-none");
                    $("#btn-send-utpl").attr("disabled", false);
                    document.getElementById("error_text").innerHTML = "Ha ocurrido un error mientras se  procesaba su solicitud, favor intentar más tarde.";
                },
                complete: function () {
                    DeleteCookie();
                    $("#loader").attr('class', "d-none");
                    $("#btn-send-utpl").attr("disabled", false);
                },
                timeout: 50000 // sets timeout to 50 seconds
            });
        } else {
            DeleteCookie();
            $("#loader").attr('class', "d-none");
            $("#btn-send-utpl").attr("disabled", false);
            document.getElementById("error_text").innerHTML = "Ha ocurrido un error mientras se  procesaba su solicitud, favor intentar más tarde.";
        }
    } else {
        //console.log("NO ATENDIDA");
    }

});


