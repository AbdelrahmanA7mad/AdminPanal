$(document).ready(function () {
    $('.js-delete').on('click', function () {
        var btn = $(this);
        var isProduct = btn.closest('table').length > 0; // Check if it's a product based on the closest table element
        var itemType = isProduct ? 'Product' : 'Category'; // Determine the item type for the alert message
        var deleteUrl = isProduct ? `/Home/DeleteProduct/${btn.data('id')}` : `/Home/DeleteCategory/${btn.data('id')}`;

        const swal = Swal.mixin({
            customClass: {
                confirmButton: 'btn btn-danger mx-2',
                cancelButton: 'btn btn-light'
            },
            buttonsStyling: false
        });

        swal.fire({
            title: `Are you sure that you want to delete this ${itemType}?`,
            text: "You won't be able to revert this!",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Yes, delete it!',
            cancelButtonText: 'No, cancel!',
            reverseButtons: true
        }).then((result) => {
            if (result.isConfirmed) {
                $.ajax({
                    url: deleteUrl, // Use the determined URL
                    method: 'DELETE',
                    success: function () {
                        swal.fire(
                            'Deleted!',
                            `${itemType} has been deleted.`,
                            'success'
                        ).then(() => {
                            location.reload(); // Refresh the page after deletion
                        });
                    },
                    error: function () {
                        swal.fire(
                            'Oops...',
                            'Something went wrong.',
                            'error'
                        );
                    }
                });
            }
        });
    });
});
