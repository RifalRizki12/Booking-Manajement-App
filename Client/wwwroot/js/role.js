$(document).ready(function () {
    $('#tableRole').DataTable({
        ajax: {
            url: '/role/GetAllRole',
            dataSrc: 'data'
        },
        columns: [
            { data: 'guid' },
            { data: 'name' },
            {
                data: null,
                render: function (data, type, row) {
                    return `<button type="button" class="btn btn-warning edit-button" data-guid="${data.guid}">Edit</button>
                            <button type="button" class="btn btn-danger delete-button" data-guid="${data.guid}">-</button>`;
                }
            }
        ]
    });
});

$('.dt-buttons').removeClass('dt-buttons');

function editRole(guid) {
    // Tambahkan logika untuk mengedit role di sini
}

function deleteRole(guid) {
    // Tambahkan logika untuk menghapus role di sini
}