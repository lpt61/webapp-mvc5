$(document).ready(function () {
    CreateAutocomplete();
});

function CreateAutocomplete() {
    var inputsToProcess = $('[data-autocomplete]').each(function (index, element) {
        var requestUrl = $(element).attr('data-action');

        $(element).autocomplete({
            minLength: 2,
            source: function (request, response) {
                $.ajax({
                    url: requestUrl,
                    dataType: "json",
                    data: { query: request.term },
                    success: function (data) {
                        response($.map(data, function (item) {
                            return {
                                label: item.Label,
                                value: item.Label,
                                realValue: item.Value
                            };
                        }));
                    },
                });
            },
            select: function (event, ui) {
                var hiddenFieldName = $(this).attr('data-value-name');
                $('#' + hiddenFieldName).val(ui.item.realValue);
            }
        });
    });
}