var contradiction = false;


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

//remove node/s after clickng on Odstran button
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
            //replace 1 to 0 and otherwise
            if (!contradiction) {
                var newContent = currentContent.replace("0", "#").replace("1", "0").replace("#", "1");
                spanElement.text(newContent);
            }
            //if we want to add x for contradiction
            else {
               
                    var hasX = currentContent.indexOf("x") !== -1;

                    var newContent = hasX ? currentContent.replace(" x", "") : currentContent + " x";
                    spanElement.text(newContent);
                
            }
            // Modify the content of the span element

        });

        // Attach a click event handler to the #xButton element
        $("#contradictionButton").on("click", function () {
            // Get the current value of the hiddenNumber input element
            contradiction = !contradiction;
        });

    document.getElementById('checkTree').addEventListener('click', handleButtonOverStrom);

});