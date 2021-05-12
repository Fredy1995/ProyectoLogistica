$(document).ready(function () {
    var nFilas = $("#GvDatosReportes tr").length;
    var nColumnas = $("#GvDatosReportes tr:last td").length;
    //Poner en negrita el texto de Ingresos
    $('#GvDatosReportes:last tr:nth-child(2) td:nth-child(1)').css({ 'font-weight': 'bold' });
    //Colorear Filas y Columnas según su posición Color Amarillo
    $('#GvDatosReportes td:nth-child(' + nColumnas + ')').css({ 'background-color': 'yellow' });
    $('#GvDatosReportes td:nth-child(' + (nColumnas - 1) + ')').css({ 'background-color': 'yellow' });
    $('#GvDatosReportes td:nth-child(' + (nColumnas - 2) + ')').css({ 'background-color': 'yellow' });
    $('#GvDatosReportes tr:nth-child(' + (nFilas) + ')').css({ 'background-color': 'yellow', 'font-weight': 'bold' });
    $('#GvDatosReportes tr:nth-child(' + (nFilas - 1) + ')').css({ 'background-color': 'yellow', 'font-weight': 'bold' });
    $('#GvDatosReportes tr:nth-child(' + (nFilas - 2) + ')').css({ 'background-color': 'yellow', 'font-weight': 'bold' });
    $('#GvDatosReportes tr:nth-child(' + (nFilas - 3) + ')').css({ 'background-color': 'yellow', 'font-weight': 'bold' });
    $('#GvDatosReportes tr:nth-child(' + (nFilas - 4) + ')').css({ 'background-color': 'yellow', 'font-weight': 'bold' });
    $('#GvDatosReportes tr:nth-child(' + (nFilas - 5) + ')').css({ 'background-color': 'yellow', 'font-weight': 'bold' });
    $('#GvDatosReportes tr:nth-child(' + (nFilas - 6) + ')').css({ 'background-color': 'yellow', 'font-weight': 'bold' });

    //Colorear Filas y Columas  según su posición Color Melón
    $('#GvDatosReportes:last tr:nth-child(' + (nFilas - 6) + ') td:nth-child(' + (nColumnas - 2) + ')').css({ 'background-color': '#ffcc99' });
    $('#GvDatosReportes:last tr:nth-child(' + (nFilas - 5) + ') td:nth-child(' + (nColumnas - 2) + ')').css({ 'background-color': '#ffcc99' });
    $('#GvDatosReportes:last tr:nth-child(' + (nFilas - 4) + ') td:nth-child(' + (nColumnas - 2) + ')').css({ 'background-color': '#ffcc99' });
    $('#GvDatosReportes:last tr:nth-child(' + (nFilas - 3) + ') td:nth-child(' + (nColumnas - 2) + ')').css({ 'background-color': '#ffcc99' });
    $('#GvDatosReportes:last tr:nth-child(' + (nFilas - 2) + ') td:nth-child(' + (nColumnas - 2) + ')').css({ 'background-color': '#ffcc99' });
    $('#GvDatosReportes:last tr:nth-child(' + (nFilas - 1) + ') td:nth-child(' + (nColumnas - 2) + ')').css({ 'background-color': '#ffcc99' });
    $('#GvDatosReportes:last tr:nth-child(' + (nFilas) + ') td:nth-child(' + (nColumnas - 2) + ')').css({ 'background-color': '#ffcc99' });

    $('#GvDatosReportes:last tr:nth-child(' + (nFilas - 6) + ') td:nth-child(' + (nColumnas - 1) + ')').css({ 'background-color': '#ffcc99' });
    $('#GvDatosReportes:last tr:nth-child(' + (nFilas - 5) + ') td:nth-child(' + (nColumnas - 1) + ')').css({ 'background-color': '#ffcc99' });
    $('#GvDatosReportes:last tr:nth-child(' + (nFilas - 4) + ') td:nth-child(' + (nColumnas - 1) + ')').css({ 'background-color': '#ffcc99' });
    $('#GvDatosReportes:last tr:nth-child(' + (nFilas - 3) + ') td:nth-child(' + (nColumnas - 1) + ')').css({ 'background-color': '#ffcc99' });
    $('#GvDatosReportes:last tr:nth-child(' + (nFilas - 2) + ') td:nth-child(' + (nColumnas - 1) + ')').css({ 'background-color': '#ffcc99' });
    $('#GvDatosReportes:last tr:nth-child(' + (nFilas - 1) + ') td:nth-child(' + (nColumnas - 1) + ')').css({ 'background-color': '#ffcc99' });
    $('#GvDatosReportes:last tr:nth-child(' + (nFilas) + ') td:nth-child(' + (nColumnas - 1) + ')').css({ 'background-color': '#ffcc99' });

    $('#GvDatosReportes:last tr:nth-child(' + (nFilas - 6) + ') td:nth-child(' + (nColumnas) + ')').css({ 'background-color': '#ffcc99' });
    $('#GvDatosReportes:last tr:nth-child(' + (nFilas - 5) + ') td:nth-child(' + (nColumnas) + ')').css({ 'background-color': '#ffcc99' });
    $('#GvDatosReportes:last tr:nth-child(' + (nFilas - 4) + ') td:nth-child(' + (nColumnas) + ')').css({ 'background-color': '#ffcc99' });
    $('#GvDatosReportes:last tr:nth-child(' + (nFilas - 3) + ') td:nth-child(' + (nColumnas) + ')').css({ 'background-color': '#ffcc99' });
    $('#GvDatosReportes:last tr:nth-child(' + (nFilas - 2) + ') td:nth-child(' + (nColumnas) + ')').css({ 'background-color': '#ffcc99' });
    $('#GvDatosReportes:last tr:nth-child(' + (nFilas - 1) + ') td:nth-child(' + (nColumnas) + ')').css({ 'background-color': '#ffcc99' });
    $('#GvDatosReportes:last tr:nth-child(' + (nFilas) + ') td:nth-child(' + (nColumnas) + ')').css({ 'background-color': '#ffcc99' });

   
    
});
