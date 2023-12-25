// Function to escape special characters in the validSymbols array
function escapeRegExp(string) {
    return string.replace(/[.*+?^${}()|[\]\\]/g, '\\$&');
}

var globalInput; // Declare globalInput

function showDialog(title, message) {
    // You can use a library like Bootstrap modal or a custom dialog implementation
    // For simplicity, using the built-in alert function here
    alert(title + "\n\n" + message);
}

// Case of interactive tree
$(document).ready(function () {

    // Buttons for text
    $(".insert-button").click(function () {
        var buttonText = $(this).data("text");
        $("#TreeInput").val(buttonText);
    });

    document.getElementById('submitButton').addEventListener('click', function () {
        // Perform your action here
        var inputValue = $('#TreeInput').val();
        var validSymbols = ['∧', '∨', '≡', '¬', '⇒', '¬¬'];
        var regex = new RegExp('^[a-zA-Z]+$|^(' + validSymbols.map(escapeRegExp).join('|') + ')$');
        console.log("Input value: " + inputValue);
        if (regex.test(inputValue)) {
            if (globalInput) globalInput.text(inputValue);
        } else {
            alert("Invalid Input. Please enter only alphabetic letters, '∧', '¬', or '¬¬'.");
        }
    });

    $(".tf-nc").on("click", function () {
        // Get the current span element
        $(".tf-nc").css("border-color", "");  // Set it to an empty string to reset to default
        var spanElement = $(this);
        console.log("HTML type of $(this):", $(this).prop("nodeName"));
        globalInput = spanElement;
        console.log(globalInput);
        $(this).css("border-color", "blue");
    });
})