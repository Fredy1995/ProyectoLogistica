$(document).ready(function () {

    /* Analizamos cada elemento para cambiar el estilo */
    $('#GVTableAnual td').each((n, elemento) => {
        /* Si es un porcentaje al final analizamos el contenido */
        if (elemento.innerText.match('%$')) {
            let valor = parseInt(elemento.innerText);
            if (valor < 0) {
                elemento.style.color = 'red';
                elemento.style.fontWeight = 'bold';

            } else {
                elemento.style.color = 'limegreen';
                elemento.style.fontWeight = 'bold';
            }
        }
    });
    var nColumnas = $("#GVTableAnual tr:last td").length;
    // Busca solo en la penultima columna
    $('#GVTableAnual td:nth-child(' + (nColumnas - 1) + ')').each((n, elemento) => {
        /* Si es un porcentaje al final analizamos el contenido */
        var monto = elemento.innerText;
        var montoFormat = monto.replace(/[$. ]/g, '');

        let valor = parseInt(montoFormat);
        if (valor < 0) {
            elemento.style.color = 'red';
        } else {
            elemento.style.color = 'limegreen';

        }

    });

});