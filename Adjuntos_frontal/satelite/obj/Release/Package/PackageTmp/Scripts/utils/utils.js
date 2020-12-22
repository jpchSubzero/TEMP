//Obtencion de ruta base para consumo de solicitud POST
url_base = document.URL;
url_base = url_base.split("/");

version = (url_base.length >= 7) ? "/" + url_base[3] : "";

formato_perm = $("#name-file").text();

if (version == '/Services' || version == '/Scholarship') {
    version = '';


}

data_code = document.URL;
if (data_code.length > 1) {
    data_code = data_code.split("Index/");
    data_code_split = data_code[1];
} else {
    data_code_split = "";
}
