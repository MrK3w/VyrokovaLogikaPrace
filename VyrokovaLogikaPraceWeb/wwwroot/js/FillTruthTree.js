var contradiction = false;

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
                //if it is literal add or remove x
                if ((/^[A-Za-z]/.test(currentContent))) {
                    var hasX = currentContent.indexOf("x") !== -1;

                    var newContent = hasX ? currentContent.replace(" x", "") : currentContent + " x";
                    spanElement.text(newContent);
                }
            }
            // Modify the content of the span element

        });

        // Attach a click event handler to the #xButton element
        $("#xButton").on("click", function () {
            // Get the current value of the hiddenNumber input element
            contradiction = !contradiction;
        });
});