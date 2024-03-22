var contradiction = false;

//check if tree is fully completed
function doesTreeContainsHash() {
    var spans = document.querySelectorAll('.tf-nc');
    var containsHash = false;

    for (var i = 0; i < spans.length; i++) {
        var span = spans[i];
        // Check if the inner text of the span contains "#"
        if (span.innerText.includes('#')) {
            alert("Strom nen\u00ED zcela vypln\u011Bn\u00FD");
            containsHash = true;
            break;
        }
    }
    return containsHash;
}

//Send tree to verification
function handleButtonOverStrom() {
    var elements = document.querySelectorAll(".tf-tree.tf-gap-sm");
    var innerHTMLContent;
    if (elements.length > 0) {
        // Element exists, you can proceed with accessing its innerHTML
        innerHTMLContent = elements[0].innerHTML;
    } else {
        // Element does not exist, throw an alert
        alert("Strom nebyl vytvoøen");
        return;
    }
    if (!doesTreeContainsHash()) {
        $.ajax({
            url: '?handler=CheckTree',
            beforeSend: function (xhr) {
                xhr.setRequestHeader("XSRF-TOKEN",
                    $('input:hidden[name="__RequestVerificationToken"]').val());
            },
            type: 'POST',
            contentType: 'application/json',
            dataType: "json",
            data: JSON.stringify(innerHTMLContent),
            success: function (data) {
                console.log('Success:', data);


            },
            error: function (error) {
                console.error('Error:', error);
                // Handle the error
            }
        });
    }
}

$(document).ready(function () {
        // Attach a click event handler to the .tf-nc span element
        $(".tf-nc").on("click", function () {
            // Get the current span element
            var spanElement = $(this);

            // Get the current content of the span element
            var currentContent = spanElement.text();

            var indexOfEqual = currentContent.indexOf('=');

            // Extract the content after '=' character
            var contentAfterEqual = currentContent.substring(indexOfEqual + 1).trim();
            var contradictionSymbol = "";
            if (contentAfterEqual.includes('?')) contradictionSymbol = " ?"
            var newContent;
            //replace 1 to 0 and otherwise
            if (!contradiction) {
                // If content after '=' is '#', replace it with '1'
                if (contentAfterEqual === '#' + contradictionSymbol) {
                    newContent = currentContent.substring(0, indexOfEqual + 1) + ' 1' + contradictionSymbol;
                } else {
                    // Otherwise, toggle between '0', '1', and '0/1'
                    if (contentAfterEqual === '0' + contradictionSymbol) {
                        newContent = currentContent.substring(0, indexOfEqual + 1) + ' 1' + contradictionSymbol;
                    } else if (contentAfterEqual === '1' + contradictionSymbol) {
                        newContent = currentContent.substring(0, indexOfEqual + 1) + ' 0/1' + contradictionSymbol;
                    } else if (contentAfterEqual === '0/1' + contradictionSymbol) {
                        newContent = currentContent.substring(0, indexOfEqual + 1) + ' 0' + contradictionSymbol;
                    }
                }
                spanElement.text(newContent);

            }
            //if we want to add x for contradiction
            else {
               
                    var hasX = currentContent.indexOf("?") !== -1;

                    var newContent = hasX ? currentContent.replace(" ?", "") : currentContent + " ?";
                    spanElement.text(newContent);
                
            }
            // Modify the content of the span element

        });

        // Attach a click event handler to the #xButton element
        $("#contradictionButton").on("click", function () {
            // Get the current value of the hiddenNumber input element
            contradiction = !contradiction;
            // Check if contradiction is true
            if (contradiction) {
                // Change the button color to green
                $("#contradictionButton").css("background-color", "green");
            } else {
                // Change the button color to default color
                $("#contradictionButton").css("background-color", ""); // or you can set it to the default color you want
            }
        });

    document.getElementById('checkTree').addEventListener('click', handleButtonOverStrom);

});