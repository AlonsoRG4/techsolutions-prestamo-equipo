(function () {
    "use strict";

    document.addEventListener("DOMContentLoaded", function () {

        var toggleBtn = document.getElementById("tsToggleSidebar");
        var sidebar = document.getElementById("tsSidebar");
        if (toggleBtn && sidebar) {
            toggleBtn.addEventListener("click", function () {
                sidebar.classList.toggle("ts-abierto");
            });
        }

        if (window.jQuery && jQuery.fn.DataTable) {
            jQuery(".ts-datatable").DataTable({
                language: {
                    search: "Buscar:",
                    lengthMenu: "Mostrar _MENU_ registros",
                    info: "Mostrando _START_ a _END_ de _TOTAL_ registros",
                    infoEmpty: "Sin registros disponibles",
                    infoFiltered: "(filtrado de _MAX_ registros totales)",
                    zeroRecords: "No se encontraron resultados",
                    paginate: { first: "Primero", last: "Último", next: "Siguiente", previous: "Anterior" }
                },
                order: []
            });
        }

        document.querySelectorAll("form[data-confirmar]").forEach(function (form) {
            form.addEventListener("submit", function (evento) {
                var mensaje = form.dataset.confirmar || "¿Estás seguro?";
                if (!window.confirm(mensaje)) {
                    evento.preventDefault();
                }
            });
        });

        window.setTimeout(function () {
            document.querySelectorAll(".alert.position-fixed").forEach(function (alerta) {
                alerta.style.transition = "opacity .5s ease";
                alerta.style.opacity = "0";
                window.setTimeout(function () { alerta.remove(); }, 500);
            });
        }, 4000);
    });
})();
