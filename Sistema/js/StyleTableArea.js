$(document).ready(function () {

    //Antes de aplicar el UpdatePanel
    /* Analizamos cada elemento para cambiar el estilo */
    $('#GVContratos td').each((n, elemento) => {
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
    var nColumnas = $("#GVContratos tr:last td").length;
    // Busca solo en la penultima columna
    $('#GVContratos td:nth-child(' + (nColumnas - 1) + ')').each((n, elemento) => {
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

    //Codigo de javascript para que vuelva a ejecutarse en el updatepanel
    var pageMgr = Sys.WebForms.PageRequestManager.getInstance();
    pageMgr.add_endRequest(Actualizar);
    function Actualizar(sender, args) {
    /* Analizamos cada elemento para cambiar el estilo */
    $('#GVContratos td').each((n, elemento) => {
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
    var nColumnas = $("#GVContratos tr:last td").length;
    // Busca solo en la penultima columna
    $('#GVContratos td:nth-child(' + (nColumnas - 1) + ')').each((n, elemento) => {
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
  }
});

