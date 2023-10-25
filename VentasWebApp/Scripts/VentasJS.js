var tablaArticulos;
 function eliminarArticulo(codigo) {
    $.ajax({
        url: "/Home/EliminarArticulo",
        method: "POST",
        data: { Codigo_Articulo: codigo },
        success: function (data) {

            alert(data.message);
            location.reload(true);
            
        }
    });
}
function editarArticulo(codigo) {
    $.ajax({
        url: "/Home/ObtenerArticulo",
        method: "POST",
        data: { Codigo_Articulo: codigo },
        dataType: "json",
        success: function (data) {
            $("#txtCodigo").val(data.Codigo_Articulo);
            $("#txtArticulo").val(data.Nombre_Articulo);
            $("#txtPrecio").val(data.Precio_Articulo);
            pintarCheckBox(data.ValidateIVA);
            administrarPropiedadesActulizar();
        }
    });
}
function pintarCheckBox(statusCheckBox) {
    var checkBox = document.getElementById("checkIVA");
    if (statusCheckBox) {
        checkBox.checked = true;
    }
    else {
        checkBox.checked = false;
    }
}

function administrarPropiedadesActulizar() {
    var btnAgregarArticulo = document.getElementById("btnAgregarArticulo");
    btnAgregarArticulo.style.display = "none";
    var btnActualizarArticulo = document.getElementById("btnActualizarArticulo");
    btnActualizarArticulo.style.display = "block";
    var txtCodigo = document.getElementById("txtCodigo");
    txtCodigo.disabled = true;
}

function btnActualizarArticulo() {
    let vCodigo = $("#txtCodigo").val();
    let vArticulo = $("#txtArticulo").val();
    let vPrecio = parseFloat($("#txtPrecio").val());
    let vIVA = $("#checkIVA").prop("checked");
    $.ajax({
        url: "/Home/ActualizarArticulo",
        method: "POST",
        data: { Codigo_Articulo: vCodigo, Nombre_Articulo: vArticulo, Precio_Articulo: vPrecio, ValidateIVA: vIVA },
        success: function (data) {

            location.reload(true);

        }
    });
}

function btnAgregarArticulo() {
    let vCodigo = $("#txtCodigo").val();
    let vArticulo = $("#txtArticulo").val();
    let vPrecio = parseFloat($("#txtPrecio").val());
    let vIVA = $("#checkIVA").prop("checked");
    if (vCodigo && vArticulo && !isNaN(vPrecio)) {
        $.ajax({
            url: "/Home/ArticulosVenta",
            method: "POST",
            data: { Codigo_Articulo: vCodigo, Nombre_Articulo: vArticulo, Precio_Articulo: vPrecio, ValidateIVA: vIVA },
            success: function (data) {
                if (data.numeroError == 1) {
                    alert(data.message);
                } else {
                    location.reload(true);
                }
            }
        });
    } else {
        alert("Por favor, complete todos los campos correctamente.");
    }
}
function detectarTab(event) {
    if (event.key === "Tab") {
        event.preventDefault();
        let vCodigo = $("#txtCodigo").val();
        $.ajax({
            url: "/Home/ObtenerArticulo",
            method: "POST",
            data: { Codigo_Articulo: vCodigo },
            success: function (data) {
                $("#txtNombreArticulo").val(data.Nombre_Articulo);
            }
        });
    }
}
$(document).ready(function () {
    
    tablaArticulos();

    var codigosEnTabla = [];
    $("#btnAgregarFactura").on("click", function () {
        let vCodigo = $("#txtCodigo").val();
        let vCantidad = $("#txtCantidad").val();
        if (vCodigo && !isNaN(vCantidad)) {
            $.ajax({
                url: "/Home/Factura",
                method: "POST",
                data: { Codigo_Articulo: vCodigo, Cantidad: vCantidad },
                dataType: "json",
                success: function (data) {
                        if (data.numeroError == 1) {
                            $("#txtNombreArticulo").val(data.Nombre_Articulo);
                            if (!codigosEnTabla.includes(data.data.Codigo_Articulo)) {
                                $("#tbFactura tbody").append(
                                    "<tr>" +
                                    "<td>" + data.data.Codigo_Articulo + "</td>" +
                                    "<td>" + data.data.Nombre_Articulo + "</td>" +
                                    "<td>" + data.data.Precio_Articulo + "</td>" +
                                    "<td>" + data.data.IVA + "</td>" +
                                    "<td>" + vCantidad + "</td>" +
                                    "<td>" + data.data.Total + "</td>" +
                                    "</tr>"
                                );
                                codigosEnTabla.push(data.data.Codigo_Articulo);
                            } else {
                                alert("El artículo " + data.data.Nombre_Articulo + " ya está incluido.");

                            }
                        } else {
                            alert(data.message);
                    }
                    $("#txtCodigo").val("");
                    $("#txtCantidad").val("");
                    $("#btnImprimirFactura").prop("disabled", false);

                }
            });
        } else {
            alert("Por favor, complete todos los campos correctamente.");
        }
    });

    $("#btnImprimirFactura").on("click", function () {
        var data = [];
        var vUserId = document.getElementById('sesion-data').getAttribute('data-UserId');
        $('#tbFactura tbody tr').each(function () {
            let vCodigo = $(this).find('td:eq(0)').text();
            let vNombre = $(this).find('td:eq(1)').text();
            let vPrecio = $(this).find('td:eq(2)').text();
            let vIVA = $(this).find('td:eq(3)').text();
            let vCantidad = $(this).find('td:eq(4)').text();
            let vTotal = $(this).find('td:eq(5)').text();

            data.push({ Codigo_Articulo: vCodigo, Nombre_Articulo: vNombre, Precio_Articulo: vPrecio, IVA: vIVA, Cantidad: vCantidad, Total: vTotal});
        });
        $.ajax({
            url: "/Home/GuardarFactura", 
            method: "POST",
            data: JSON.stringify({ datosTabla: data, id: vUserId }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {

                alert(data.message);
            },
            error: function (data) {
                alert(data.message);
            }
        });

    });

   
    function tablaArticulos() {
        tablaArticulos = $('#tbArticulo').DataTable({
            searching: false,
            lengthChange: false,
            paging: false,
            info: false,
            ordering: false,
            ajax: {
                url: "listaArt",
                type: "GET",
                datatype: "json",
                "dataSrc": ""
            },
            columns: [
                { "data": "Codigo_Articulo" },
                { "data": "Nombre_Articulo" },
                {
                    "data": "Precio_Articulo",
                    "render": function (data, type, row) {
                        return formatCurrency(data);
                    }
                },
                {
                    "data": "IVA",
                    "render": function (data, type, row) {
                        return formatCurrency(data);
                    }
                },
                {
                    "data": "Total",
                    "render": function (data, type, row) {
                        return formatCurrency(data);
                    }
                },
                {
                    "data": null,
                    "render": function (data, type, row) {
                        return '<button class="btn btn-info btn-sm" onclick="editarArticulo(' + row.Codigo_Articulo + ')"> Editar </button>' +
                            '<button class="btn btn-danger btn-sm ml-3" onclick="eliminarArticulo(' + row.Codigo_Articulo + ')">Eliminar</button>';
                    }
                }
            ]
        });
    }

    
    function formatCurrency(value) {
        return '₡' + value.toFixed(2);
    }

});