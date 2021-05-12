$(document).ready(function () {
    var UP = document.getElementById('LblFooterUP').innerText;
    var UR = document.getElementById('LblFooterUR').innerText;
    var PP = document.getElementById('LblFooterPP').innerText;
    var PR = document.getElementById('LblFooterPR').innerText;
    //Tamaño de fuente
    $('#LblFooterUP').css("font-size", "12px");
    $('#LblFooterUR').css("font-size", "12px");
    $('#LblFooterPP').css("font-size", "12px");
    $('#LblFooterPR').css("font-size", "12px");
    //Fuente en negrita
    $('#LblFooterUP').css("font-weight", "bold");
    $('#LblFooterUR').css("font-weight", "bold");
    $('#LblFooterPP').css("font-weight", "bold");
    $('#LblFooterPR').css("font-weight", "bold");
    //Eliminar singos a los labels y los convierto a valores enteros
    let valUP = parseInt(UP.replace(/[$. ]/g, ''));
    let valUR = parseInt(UR.replace(/[$. ]/g, ''));
    let valPP = parseInt(PP.replace(/[$. ]/g, ''));
    let valPR = parseInt(PR.replace(/[$. ]/g, ''));
    if (valUP < 0) {
        $('#LblFooterUP').css("color", "red");

    } else {
        $('#LblFooterUP').css("color", "#0080c0");
    }
    if (valUR < 0) {
        $('#LblFooterUR').css("color", "red");

    } else {
        $('#LblFooterUR').css("color", "blue");
    }
    if (valPP < 0) {
        $('#LblFooterPP').css("color", "red");

    } else {
        $('#LblFooterPP').css("color", "#0080c0");
    }
    if (valPR < 0) {
        $('#LblFooterPR').css("color", "red");

    } else {
        $('#LblFooterPR').css("color", "blue");
    }
});