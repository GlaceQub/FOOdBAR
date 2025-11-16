function applyStatusFilters() {
    var selectedStatusIds = $('#status-filter-pills .status-filter-pill[aria-pressed="true"]').map(function () {
        return $(this).attr('data-status-id').toString();
    }).get();

    $('.bestelling-row').each(function () {
        var statusId = $(this).attr('data-status-id').toString();
        if (selectedStatusIds.includes(statusId)) {
            $(this).show();
        } else {
            $(this).hide();
        }
    });
}

// On page load, apply the initial filter
$(document).ready(function () {
    applyStatusFilters();

    // Status button filter logic
    $('#status-filter-pills').on('click', '.status-filter-pill', function () {
        var $btn = $(this);
        // Get color from btn-@color or btn-outline-@color
        var colorMatch = $btn.attr('class').match(/btn-(outline-)?(\w+)/);
        var color = colorMatch ? colorMatch[2] : 'light';
        var isActive = $btn.attr('aria-pressed') === 'true';

        if (isActive) {
            $btn
                .removeClass('btn-' + color)
                .addClass('btn-outline-' + color);
            $btn.attr('aria-pressed', 'false');
        } else {
            $btn
                .removeClass('btn-outline-' + color)
                .addClass('btn-' + color);
            $btn.attr('aria-pressed', 'true');
        }

        applyStatusFilters();
    });

    // Status dropdown AJAX logic
    $('.status-dropdown').on('change', function () {
        var form = $(this).closest('form');
        $.ajax({
            type: form.attr('method'),
            url: form.attr('action'),
            data: form.serialize(),
            success: function () {
                var statusId = form.find('select[name="StatusId"]').val();
                var row = form.closest('.list-group-item');
                row.removeClass(function (index, className) {
                    return (className.match(/list-group-item-\S+/g) || []).join(' ');
                });
                var newClass = "list-group-item-" + (typeof statusColors !== 'undefined' ? (statusColors[statusId] || "light") : "light");
                row.addClass(newClass);
                row.attr('data-status-id', statusId);
                applyStatusFilters();
            },
            error: function () {
                alert('Status update failed.');
            }
        });
    });
});