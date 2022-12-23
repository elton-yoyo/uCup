const getAllMachineUri = 'platform/getallmachine';

function getAllMachines() {
    fetch(getAllMachineUri + '?' + document.cookie)
        .then(response => response.json())
        .then(data => json2table(data, $("#RentTable")))
        .catch(error => console.error('Unable to get items.', error));
}

function json2table(json, $table) {
    if (json.length == 0) {
        $table.html('<thead><tr></tr></thead><tbody></tbody>');
    } else {
        var cols = Object.keys(json[0]);

        var headerRow = '';
        var bodyRows = '';

        $table.html('<thead><tr></tr></thead><tbody></tbody>');

        cols.map(function(col) {
            headerRow += '<th>' + col + '</th>';
        });

        json.map(function(row) {
            bodyRows += '<tr>';

            cols.map(function(colName) {
                bodyRows += '<td>' + row[colName] + '</td>';
            })

            bodyRows += '</tr>';
        });

        $table.find('thead tr').append(headerRow);
        $table.find('tbody').append(bodyRows);
    }
}
