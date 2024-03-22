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

function handleButtonDrawTree() {
    // Get the value of UserInput
    var userInputValue = document.getElementById('UserInput').value.trim();
    var formulaValue;
    // Check if UserInput is empty
    if (userInputValue === '') {
        // If UserInput is empty, get the selected value from the dropdown list (formula)
        var formulaDropdown = document.getElementById('formula');
        var selectedFormula = formulaDropdown.options[formulaDropdown.selectedIndex].value;
        formulaValue = selectedFormula.trim();
    } else {
        // If UserInput is not empty, use its value
         formulaValue = userInputValue;
    }

    $.ajax({
        url: '?handler=DrawTree',
        beforeSend: function (xhr) {
            xhr.setRequestHeader("XSRF-TOKEN",
                $('input:hidden[name="__RequestVerificationToken"]').val());
        },
        type: 'POST',
        contentType: 'application/json',
        dataType: "json",
        data: JSON.stringify(formulaValue),
        success: function (data) {
            if (data.errors.length === 0) {
                // If there are no errors, display the converted tree
                document.getElementById('treeDiv').innerHTML = data.convertedTree;

                // Display the formula
                document.getElementById('formule').innerHTML = 'Synktaktick\u00FD strom formule <span style="color: red;">' + data.formula + '</span>.';

                var buttonsDiv = document.getElementById('buttons');
                buttonsDiv.innerHTML = '';

                // Create and append the "Oznaè spor" button
                var contradictionButton = document.createElement('button');
                contradictionButton.id = 'contradictionButton';
                contradictionButton.className = 'btn btn-primary flex-fill mb-2 mr-1';
                contradictionButton.textContent = 'Ozna\u010d spor';
                buttonsDiv.appendChild(contradictionButton);

                // Create and append the "Ovìø strom" button
                var checkTreeButton = document.createElement('button');
                checkTreeButton.id = 'checkTree';
                checkTreeButton.className = 'btn btn-primary flex-fill mb-2 mr-1';
                checkTreeButton.textContent = 'Ov\u011b\u0159 strom';
                buttonsDiv.appendChild(checkTreeButton);

            } else {
                // If there are errors, display them
                var errorList = '';
                data.Errors.forEach(function (error) {
                    errorList += '<li>' + error + '</li>';
                });

                document.getElementById('errors').innerHTML = '<div class="alert alert-danger"><h4 class="alert-heading">Nesprávná formule:</h4><ul>' + errorList + '</ul></div>';
            }
            attachEventHandlers(); // Call attachEventHandlers inside success callback
        },
        error: function (error) {
            console.error('Error:', error);
            // Handle the error
        }
    });
}

function isContradictionMarked() {
    var spans = document.querySelectorAll('.tf-nc');
    var contradictionMarked = true;

    for (var i = 0; i < spans.length; i++) {
        var span = spans[i];
        // Check if the inner text of the span contains "#"
        if (span.innerText.includes("0/1") && !(span.innerText.includes("?"))) {
            alert("Strom nem\u00e1 spr\u00e1vn\u011B ozna\u010den\u00fd spor");

            contradictionMarked = false;
            break;
        }
    }
    return contradictionMarked;
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
    if (doesTreeContainsHash()) {
        return;
    }
    if (!isContradictionMarked()) {
        return;
    }
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
                document.getElementById('treeDiv').innerHTML = data.convertedTree;
                //Put formula to the element with id 'formula'
                document.getElementById('message').innerHTML = data.message;
                var errors = data.errors;

                // Get the div element where you want to display the alerts
                var errorsDiv = document.getElementById('errors');

                // Clear any existing content in the errors div
                errorsDiv.innerHTML = '';

                // Iterate over the errors list
                errors.forEach(function (error) {
                    // Create a new div element for the alert
                    var alertDiv = document.createElement('div');
                    alertDiv.classList.add('alert', 'alert-info', 'alert-dismissible', 'fade', 'show');
                    alertDiv.setAttribute('role', 'alert');

                    // Create a text node with the error message
                    var errorMessage = document.createTextNode(error);

                    // Append the error message to the alert div
                    alertDiv.appendChild(errorMessage);

                    // Append the alert div to the errors div
                    errorsDiv.appendChild(alertDiv);
                });
                attachEventHandlers();
            },
            error: function (error) {
                console.error('Error:', error);
                // Handle the error
            }
        }); 
}

function attachEventHandlers() {
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


    var checkTreeButton = document.getElementById('checkTree');

    // If the element exists, attach the event listener
    if (checkTreeButton) {
        checkTreeButton.addEventListener('click', handleButtonOverStrom);
    }
    $('#formulaForm').submit(function (event) {
        // Prevent the default form submission
        event.preventDefault();
    });
    document.getElementById('drawTree').addEventListener('click', handleButtonDrawTree);
}

$(document).ready(attachEventHandlers)