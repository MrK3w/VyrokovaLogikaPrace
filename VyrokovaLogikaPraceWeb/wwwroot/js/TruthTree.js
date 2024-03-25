var currentIndex = 0;
var treeContainer;
var convertedTrees;
var steps; 

function showNextTree() {
    var convertedTreesJson = document.getElementById('convertedTreesJson').value;
    var convertedStepsJson = document.getElementById('convertedStepsJson').value;
    // Parse the JSON data into a JavaScript object
    convertedTrees = JSON.parse(convertedTreesJson);
    steps = JSON.parse(convertedStepsJson);
    // Now you can work with convertedTrees like any other JavaScript array
    console.log(convertedTrees);
    currentIndex++;
    if (currentIndex >= convertedTrees.length) {
        currentIndex = convertedTrees.length - 1; // Ensure currentIndex doesn't exceed the last index
        return;
    }
    updateTreeContainer();
    updateButtonState();
}

function showPreviousTree() {
    var convertedTreesJson = document.getElementById('convertedTreesJson').value;
    var convertedStepsJson = document.getElementById('convertedStepsJson').value;
    // Parse the JSON data into a JavaScript object
    convertedTrees = JSON.parse(convertedTreesJson);
    steps = JSON.parse(convertedStepsJson);
    // Now you can work with convertedTrees like any other JavaScript array
    console.log(convertedTrees);
    currentIndex--;
    if (currentIndex < 0) {
        currentIndex = 0; // Ensure currentIndex doesn't go below 0
        return;
    }
    updateTreeContainer();
    updateButtonState();
}

function showFirstTree() {
    var convertedTreesJson = document.getElementById('convertedTreesJson').value;
    var convertedStepsJson = document.getElementById('convertedStepsJson').value;
    // Parse the JSON data into a JavaScript object
    convertedTrees = JSON.parse(convertedTreesJson);
    steps = JSON.parse(convertedStepsJson);
    currentIndex = 0; // Set to the first step
    updateTreeContainer();
    updateButtonState();
}

function showLastTree() {
    var convertedTreesJson = document.getElementById('convertedTreesJson').value;
    var convertedStepsJson = document.getElementById('convertedStepsJson').value;
    // Parse the JSON data into a JavaScript object
    convertedTrees = JSON.parse(convertedTreesJson);
    steps = JSON.parse(convertedStepsJson);
    if (convertedTrees && convertedTrees.length > 0) {
        currentIndex = convertedTrees.length - 1; // Set to the last step
        updateTreeContainer();
        updateButtonState();
    }
}


function updateTreeContainer() {
    var treeContainer = document.getElementById('treeContainer');
    treeContainer.innerHTML = convertedTrees[currentIndex];
    var stepsContainer = document.getElementById('Steps');
    stepsContainer.innerHTML = steps[currentIndex];
}

function updateButtonState() {
    console.log(currentIndex + " " + convertedTrees.length);
    var prevButton = document.getElementById('prevBtn');
    var nextButton = document.getElementById('nextBtn');
    prevButton.disabled = currentIndex === 0;
    nextButton.disabled = currentIndex === convertedTrees.length - 1;
}