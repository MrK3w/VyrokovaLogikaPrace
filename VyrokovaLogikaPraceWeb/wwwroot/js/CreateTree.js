var globalInput; // Declare globalInput
var formula;
var newButton = null;

// Function to escape special characters in the validSymbols array
function escapeRegExp(string) {
    return string.replace(/[.*+?^${}()|[\]\\]/g, '\\$&');
}

// Function to handle node click
function handleNodeClick() {
    // Set it to an empty string to reset to default
    $(".tf-nc").css("border-color", "");
    // Saving current node into global property
    globalInput = $(this);
    // Setting current node into blue border
    $(this).css("border-color", "blue");
}

function isValid(text) {
    var binarySymbols = ['∧', '∨', '≡', '⇒'];
    var unarySymbols = ['¬', '¬¬'];
    type = "";

    if (/^[a-zA-Z]+$/.test(text)) {
        type = "literal";
    }
    else if (binarySymbols.includes(text)) {
        type = "binary symbol";
    }
    else if (unarySymbols.includes(text)) {
        type = "unary symbol";
    }
    else {
        console.log("Invalid symbol");
    }

    var spanElement = document.querySelector('.tf-nc[style="border-color: blue;"]');
    if (spanElement) {
        //get to parent
        var parent = spanElement.parentNode;
        var foundUl = findNestedUl(parent);
        var directChildLiElements = countDirectChildLiElements(foundUl);

        if (type == "literal") {
            if (directChildLiElements != 0) {
                alert("Literál nemůže mít žádného potomka!");
                return false;
            }
            return true;
        }

        if (type == "unary symbol") {
            if (directChildLiElements.length != 1) {
                alert("Negace musí mít jen jednoho potomka!");
                return false;
            }
            return true;
        }

        if (type == "binary symbol") {
            if (directChildLiElements.length != 2) {
                alert("Binární operátory musí mít dva potomky!");
                return false;
            }
            return true;
        }
        return false;
    }
}


function handleButtonZmenaTextuClick() {
    //Get value from input
    var inputValue = $('#TreeInput').val();
    inputValue = transformInputValue(inputValue);
    //check if input is valid
    var validSymbols = ['∧', '∨', '≡', '¬', '⇒', '¬¬', '▭'];
    var regex = new RegExp('^[a-zA-Z]+$|^(' + validSymbols.map(escapeRegExp).join('|') + ')$');
    if (regex.test(inputValue)) {
        if (globalInput) {
            if (inputValue == '▭') {
                globalInput.text(inputValue);
                return;
            }
            if (isValid(inputValue)) globalInput.text(inputValue);
        }
    } else {
        alert("Nespravný vstup. Zadej pouze literál nebo logickou spojku!");
    }
}

//remove node/s after clickng on Odstran button
function handleButtonOdstranNoduClick() {
    if (globalInput) {
        var closestLi = globalInput.closest('li');
        var closestUl = closestLi.parent();
        var numberOfLi = closestUl.find('li').length;
        if (numberOfLi == 1) {
            closestUl.remove();
        }
        else {
            closestLi.remove();
        }
        // Check if the div with tree is empty
        var divElement = document.querySelector('.tf-tree');

        if (divElement.innerHTML.trim() === '') {
            divElement.remove();
        }
    }
}

//remove node/s after clickng on Odstran button
function handleButtonUlozFormuli() {
    $.ajax({
        url: '?handler=SaveFormula',
        beforeSend: function (xhr) {
            xhr.setRequestHeader("XSRF-TOKEN",
                $('input:hidden[name="__RequestVerificationToken"]').val());
        },
        type: 'POST',
        contentType: 'application/json',
        dataType: "json",
        data: JSON.stringify(formula),
        success: function (data) {
            console.log('Success:', data);

            // Clear any existing messages
            $('#message').empty();
            if (data.errors.length === 0) {
                // If no errors, display "Formula saved" message
                $('#message').html('<div class="alert alert-success">Formule uložena</div>');

            }
            else {
                // If there are errors, display the error messages
                var errorHtml = '<div class="alert alert-danger">' +
                    '<h4 class="alert-heading">Nesprávná formule:</h4>' +
                    '<ul>';

                $.each(data.errors, function (index, error) {
                    errorHtml += '<li>' + error + '</li>';
                });

                errorHtml += '</ul></div>';

                $('#message').html(errorHtml);
            }
            $('#saveFormula').remove();
            newButton = null;
        },
        error: function (error) {
            console.error('Error:', error);
            // Handle the error
        }
    });
}

//Create formula from just create tree
function handleButtonVytvorFormuliClick() {
    //get html tree from page
    $('#message').html('');
    var elements = document.querySelectorAll(".tf-tree.tf-gap-sm");
    var innerHTMLContent;
    if (elements.length > 0) {
        // Element exists, you can proceed with accessing its innerHTML
        innerHTMLContent  = elements[0].innerHTML;
    } else {
        // Element does not exist, throw an alert
        alert("Strom nebyl vytvořen");
        return;
    }


    //check if each node is filled with valid content
    if (isContentPresent()) {
        alert("Některá z nod je nevyplněná!");
        return;
    }
    //ajax calling function
    $.ajax({
        url: '?handler=CreateFormula',
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
            // Put the convertedTree to the element with id 'createdTree'
            document.getElementById('createdTree').innerHTML = data.convertedTree;
            //Put formula to the element with id 'formula'
            formula = data.formula;
            document.getElementById('formula').innerHTML = "Vytvořená formule je " + data.formula;
            //create new button for saving this new formula
            if (newButton == null) {
                newButton = $('<input type="submit" class="btn btn-primary flex-fill mb-2" value="Ulož formuli" id="saveFormula"/>');
                $('#buttonContainer').append(newButton);
            }

            // Add any additional logic or event handlers for the new button if needed
            $('#saveFormula').on('click', handleButtonUlozFormuli);
            // set functions to node
            $(".tf-nc").on("click", handleNodeClick);
        },
        error: function (error) {
            console.error('Error:', error);
            // Handle the error
        }
    });
}

//check if we have some unfilled node
function isContentPresent() {
    var spans = document.querySelectorAll('.tf-nc');

    for (var i = 0; i < spans.length; i++) {
        if (spans[i].textContent.trim() === '▭') {
            return true;
        }
    }

    return false;
}

// Helper function to find the nested ul element
function findNestedUl(parent) {
    for (var i = 0; i < parent.childNodes.length; i++) {
        var childNode = parent.childNodes[i];
        if (childNode.nodeType === 1 && childNode.tagName.toLowerCase() === 'ul') {
            return childNode;
        }
    }
    return undefined;
}

// Helper function to count direct child li elements
function countDirectChildLiElements(foundUl) {
    return foundUl ? Array.from(foundUl.children).filter(child => child.tagName.toLowerCase() === 'li') : 0;
}

// Helper function to create a new span element
function createNewSpan() {
    var newSpan = document.createElement('span');
    newSpan.className = 'tf-nc';
    newSpan.textContent = ' ▭ ';
    return newSpan;
}

// Helper function to create a new li element with a given span
function createNewLiWithSpan(newSpan) {
    var newLi = document.createElement('li');
    newLi.appendChild(newSpan);
    return newLi;
}

function createNewUlWithLi(newSpan) {
    var newUl = document.createElement('ul');
    newUl.appendChild(createNewLiWithSpan(newSpan));
    return newUl;
}

// Helper function to handle button click for adding a new node
function handleButtonPridejNoduClick() {
    var spanElement = document.querySelector('.tf-nc[style="border-color: blue;"]');
    globalInput = $(spanElement);
    if (spanElement) {
        var parent = spanElement.parentNode;
        var foundUl = findNestedUl(parent);
        var directChildLiElements = countDirectChildLiElements(foundUl);

        if (directChildLiElements == 0) {
            if (/^[a-zA-Z]+$/.test(spanElement.textContent)) {
                alert("Literal nemuze mit potomka!");
                return;
            }
            spanElement.insertAdjacentElement('afterend', createNewUlWithLi(createNewSpan()));
        } else if (directChildLiElements.length == 1) {
            if (spanElement.textContent == '¬' || spanElement.textContent == "¬¬") {
                alert("Negace muze mit jen jednoho potomka!");
                return;
            }
            foundUl.appendChild(createNewLiWithSpan(createNewSpan()));
        } else if (directChildLiElements.length == 2) {
            alert("Maximalni pocet potomku!");
        }
    }
    else {
        var divTree = document.createElement('div');
        divTree.className = 'tf-tree tf-gap-sm';

        var listItem = document.createElement('li');

        var spanElement = document.createElement('span');
        spanElement.className = 'tf-nc';
        spanElement.style.borderColor = 'blue';
        spanElement.textContent = '▭';

        // Append elements to create the hierarchy
        listItem.appendChild(spanElement);
        divTree.appendChild(listItem);

        // Append the main tree div to the container with id "createdTree"
        document.getElementById('createdTree').appendChild(divTree);
    }

    // set functions to node
    $(".tf-nc").on("click", handleNodeClick);
}

function showDialog(title, message) {
    alert(title + "\n\n" + message);
}

$(document).ready(function () {
    // Buttons for adding symbols into texbox
    $(".insert-button").click(function () {
        var buttonText = $(this).data("text");
        $("#TreeInput").val(buttonText);
    });

    //button for 'Zmena textu'
    document.getElementById('zmenaTextuButton').addEventListener('click', handleButtonZmenaTextuClick);

    //button for 'Odstra nodu'
    document.getElementById('odstranNoduButton').addEventListener('click', handleButtonOdstranNoduClick);

    document.getElementById('pridejNoduButton').addEventListener('click', handleButtonPridejNoduClick);

    document.getElementById('vytvorFormuliButton').addEventListener('click', handleButtonVytvorFormuliClick);

    $('#formulaForm').submit(function (event) {
        // Prevent the default form submission
        event.preventDefault();
    });

    //function to add into nodes click function
    $(".tf-nc").on("click", handleNodeClick);
})