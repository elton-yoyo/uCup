const rentUri = 'platform/getrentdata';
const returnUri = 'platform/getreturndata';

var intervalId = window.setInterval(function () {
    fetch('platform/clear');
}, $("#seconds").val() * 1000);

function resetCall() {
    clearInterval(intervalId);
    intervalId = window.setInterval(function () {
        fetch('platform/clear');
    }, $("#seconds").val() * 1000);
}

//function stopCall() {
//    clearInterval(intervalId);
//}

function getItems() {
    fetch(rentUri)
        .then(response => response.json())
        .then(data => json2table(data, $("#RentTable")))
        .catch(error => console.error('Unable to get items.', error));

    fetch(returnUri)
        .then(response => response.json())
        .then(data => json2table(data, $("#ReturnTable")))
        .catch(error => console.error('Unable to get items.', error));
}

function json2table(jsonString, $table) {
    var json = jsonString;
    var cols = Object.keys(jsonString[0]);

    var headerRow = '';
    var bodyRows = '';

    $table.html('<thead><tr></tr></thead><tbody></tbody>');

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

    $table.find('thead tr').append(headerRow);
    $table.find('tbody').append(bodyRows);
}

