function displayError(error) {
    $("#categorySuccess").attr("hidden", "hidden");
    $("#categoryError").removeAttr("hidden").html(error);
}

function displaySuccess(message) {
    $("#categoryError").attr("hidden", "hidden");
    $("#categorySuccess").removeAttr("hidden").html(message);
}
document.addEventListener("DOMContentLoaded", function () {
    $("#btnCreateCategory").click(function () {
        var categoryName = $("#categoryName").val();

        if (!categoryName.trim()) {
            displayError("The Category Name is empty!!");
        } else {
            $.ajax({
                type: 'POST',
                url: '/Category/Create',
                data: {
                    'categoryName': categoryName
                },
                success: function (data) {
                    if (data.search("successfully") > 0) {
                        displaySuccess("Create Category successfully!! Reload the page for the change!");
                        //location.reload();
                    } else {
                        displayError(data);
                    }
                },
                error: function (data) {
                    displayError(data);
                }
            });
        }
    });
});