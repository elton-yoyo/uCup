const uri = 'platform/getall';
let todos = [];

function getItems() {
    fetch(uri)
        .then(response => response.json())
        .then(data => json2table(data))
        .catch(error => console.error('Unable to get items.', error));
}

function json2table(jsonString) {
    var json = jsonString;
    var cols = Object.keys(jsonString[0]);

    var headerRow = '';
    var bodyRows = '';

    $("#table").html('<thead><tr></tr></thead><tbody></tbody>');

    cols.map(function (col) {
        headerRow += '<th>' + col + '</th>';
    });

    json.map(function (row) {
        bodyRows += '<tr>';

        cols.map(function (colName) {
            bodyRows += '<td>' + row[colName] + '</td>';
        })

        bodyRows += '</tr>';
    });

    $("#table").find('thead tr').append(headerRow);
    $("#table").find('tbody').append(bodyRows);
}

