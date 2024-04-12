// Map to store unique labels and their corresponding IDs
const labelIdMap = new Map();
var finished = true;
var globalCounter = 0;
var nodesGlobalData;
var lengthOfList = 0;
var steps;
var previousStepButton;
var nextStepButton;

function handleButtonDrawDAGButton(tautology) {
    CallAjaxToGetPaths(tautology);
}

function CallAjaxToGetPaths(tautology) {

    var formula = transformInputValue($('#formula').val());

    var requestData = {
        formula: formula,
        tautology: tautology // Include the boolean value
    };

    $.ajax({
        url: '?handler=TruthDAG',
        beforeSend: function (xhr) {
            xhr.setRequestHeader("XSRF-TOKEN",
                $('input:hidden[name="__RequestVerificationToken"]').val());
        },
        type: 'POST',
        contentType: 'application/json',
        dataType: "json",
        data: JSON.stringify(requestData),
        success: function (data) {
            var parsedOutput = JSON.parse(data);
            parsedOutput.VisNodes.sort((a, b) => b.Id - a.Id);
            document.getElementById("zmeny").innerHTML = "";
         
            globalCounter = 0;
            nodesGlobalData = parsedOutput.VisNodes;
            lengthOfList = nodesGlobalData.length;
            steps = parsedOutput.Steps;
            createButtons();
            attachEventHandlers();
            drawGraph();
            updateStepInfo();
        },
        error: function (error) {
            console.error('Error:', error);
        }
    });
}

function drawGraph() {
     createGraph(nodesGlobalData[globalCounter]);
}

//function to draw graph
function createGraph(nodesData) {
    //if it is not first run we will not change from tree
    const nodes = new vis.DataSet();
    const edges = new vis.DataSet();

    // Iterate through nodesData to create nodes and edges
    for (let i = 0; i < nodesData.length; i++) {
        //if node does not have parentId then it is root
        if (nodesData[i].TruthValue != -1) {
            if (nodesData[i].Contradiction == false && nodesData[i].IsChanged == true) {
                if (nodesData[i].ParentId === 0) {
                    nodes.add({ id: nodesData[i].Id, label: nodesData[i].Operator + " = " + nodesData[i].TruthValue, title: nodesData[i].Label, parentId: nodesData[i].ParentId, truthValue: nodesData[i].TruthValue, color: { background: '#90EE90' } });
                }
                else {
                    nodes.add({ id: nodesData[i].Id, label: nodesData[i].Operator + " = " + nodesData[i].TruthValue, title: nodesData[i].Label, parentId: nodesData[i].ParentId, truthValue: nodesData[i].TruthValue, color: { background: '#90EE90' } });
                    edges.add({ from: nodesData[i].Id, to: nodesData[i].ParentId, arrows: 'to', color: getEdgeColor(nodesData, nodesData[i].ParentId, nodesData[i].Label) });
                }
            }
            else if (nodesData[i].Contradiction == true) {
                if (nodesData[i].ParentId === 0) {
                    nodes.add({ id: nodesData[i].Id, label: nodesData[i].Operator + " = 0/1", title: nodesData[i].Label, parentId: nodesData[i].ParentId, truthValue: nodesData[i].TruthValue, color: { background: '#DF2C14' } });
                }
                else {
                    nodes.add({ id: nodesData[i].Id, label: nodesData[i].Operator + " = 0/1", title: nodesData[i].Label, parentId: nodesData[i].ParentId, truthValue: nodesData[i].TruthValue, color: { background: '#DF2C14' } });
                    edges.add({ from: nodesData[i].Id, to: nodesData[i].ParentId, arrows: 'to', color: getEdgeColor(nodesData, nodesData[i].ParentId, nodesData[i].Label) });
                } 
            }
            else {
                if (nodesData[i].ParentId === 0) {
                    nodes.add({ id: nodesData[i].Id, label: nodesData[i].Operator + " = " + nodesData[i].TruthValue, title: nodesData[i].Label, parentId: nodesData[i].ParentId, truthValue: nodesData[i].TruthValue, color: { background: '#FFD700' } });
                }
                else {
                    nodes.add({ id: nodesData[i].Id, label: nodesData[i].Operator + " = " + nodesData[i].TruthValue, title: nodesData[i].Label, parentId: nodesData[i].ParentId, truthValue: nodesData[i].TruthValue });
                    edges.add({ from: nodesData[i].Id, to: nodesData[i].ParentId, arrows: 'to', color: getEdgeColor(nodesData, nodesData[i].ParentId, nodesData[i].Label) });
                }
            }
        }
        else {
            if (nodesData[i].Contradiction == false && nodesData[i].IsChanged == true) {
                if (nodesData[i].ParentId === 0) {
                    nodes.add({ id: nodesData[i].Id, label: nodesData[i].Operator + " = " + nodesData[i].TruthValue, title: nodesData[i].Label, parentId: nodesData[i].ParentId, truthValue: nodesData[i].TruthValue, color: { background: '#90EE90' } });
                }
                else {
                    nodes.add({ id: nodesData[i].Id, label: nodesData[i].Operator + " = " + nodesData[i].TruthValue, title: nodesData[i].Label, parentId: nodesData[i].ParentId, truthValue: nodesData[i].TruthValue, color: { background: '#90EE90' } });
                    edges.add({ from: nodesData[i].Id, to: nodesData[i].ParentId, arrows: 'to', color: getEdgeColor(nodesData, nodesData[i].ParentId, nodesData[i].Label) });
                }
            }
            else if (nodesData[i].Contradiction == true) {
                if (nodesData[i].ParentId === 0) {
                    nodes.add({ id: nodesData[i].Id, label: nodesData[i].Operator, title: nodesData[i].Label, parentId: nodesData[i].ParentId, truthValue: nodesData[i].TruthValue, color: { background: '#DF2C14' } });
                }
                else {
                    nodes.add({ id: nodesData[i].Id, label: nodesData[i].Operator, title: nodesData[i].Label, parentId: nodesData[i].ParentId, truthValue: nodesData[i].TruthValue, color: { background: '#DF2C14' } });
                    edges.add({ from: nodesData[i].Id, to: nodesData[i].ParentId, arrows: 'to', color: getEdgeColor(nodesData, nodesData[i].ParentId, nodesData[i].Label) });
                }
            }
            else {
                if (nodesData[i].ParentId === 0) {
                    nodes.add({ id: nodesData[i].Id, label: nodesData[i].Operator, title: nodesData[i].Label, parentId: nodesData[i].ParentId, truthValue: nodesData[i].TruthValue, color: { background: '#FFD700' } });
                }
                else {
                    nodes.add({ id: nodesData[i].Id, label: nodesData[i].Operator, title: nodesData[i].Label, parentId: nodesData[i].ParentId, truthValue: nodesData[i].TruthValue });
                    edges.add({ from: nodesData[i].Id, to: nodesData[i].ParentId, arrows: 'to', color: getEdgeColor(nodesData, nodesData[i].ParentId, nodesData[i].Label) });
                }
            }
        }
    }

    // Update the vis.js network
    const container = document.getElementById('treeVisualization');
    const data = { nodes, edges };
    var options = {
        interaction: {
            hover: true
        },
        physics: {
            stabilization: true,
        },
        nodes: {
            physics: false,
            size: 30,
            font: {
                size: 16, // Adjust the font size as needed
            },
        },
    };

    const network = new vis.Network(container, data, options);

    network.on("afterDrawing", function (ctx) {
        drawOnCanvasLabels(ctx, container); // Call drawOnCanvasLabels and pass ctx
    });
    updateNodes(data, network);
}

function getEdgeColor(nodesData, parentId, label) {
    // Find the parent node based on label
    var parentNode = nodesData.find(node => node.Id === parentId);
    //if parentNode label starts with parentheses we need to add that parentheses to temp label to be able to properly check it
    if (parentNode && parentNode.Label[0] == '(')
        label = '(' + label;

    const parentLabelStart = parentNode ? parentNode.Label.substring(0, label.length) : null;
    let edgeColor = 'blue';

    // Check if the parent operator is ¬ or ¬¬ (if it is then there will not be right side)
    if (parentNode && (parentNode.Operator === '¬' || parentNode.Operator === '¬¬')) {
        edgeColor = 'blue';
    } else if (parentLabelStart !== label) {
        edgeColor = 'orange';
    }
    return edgeColor;
}

function getEdgeColorModified(nodesData, parentId, label) {
    // Find the parent node based on label
    var parentNode = nodesData.find(node => node.id === parentId);
    //if parentNode label starts with parentheses we need to add that parentheses to temp label to be able to properly check it
    if (parentNode && parentNode.title[0] == '(')
        label = '(' + label;

    const parentLabelStart = parentNode ? parentNode.title.substring(0, label.length) : null;
    let edgeColor = 'blue';

    // Check if the parent operator is ¬ or ¬¬ (if it is then there will not be right side)
    if (parentNode && (parentNode.label === '¬' || parentNode.label === '¬¬')) {
        edgeColor = 'blue';
    } else if (parentLabelStart !== label) {
        edgeColor = 'orange';
    }
    return edgeColor;
}

function updateNodes(data, network) {
    var dataFromGraph = data.nodes.get();
    var changeTitle = modifyNodes(dataFromGraph);
    var nodesToUpdateFull = dataFromGraph.filter(function (node) {
        return changeTitle.includes(node.title);
    });

    console.log(data.nodes.get());
    network.setData(data);

    // Group nodes by their title property
    var groupedNodes = {};
    nodesToUpdateFull.forEach(function (node) {
        if (!groupedNodes[node.title]) {
            groupedNodes[node.title] = [];
        }
        groupedNodes[node.title].push(node);
    });

    // Iterate over each group of nodes
    for (var title in groupedNodes) {
        if (groupedNodes.hasOwnProperty(title)) {
            var nodesToUpdate = groupedNodes[title];
            var i = 0; // Initialize i here
            nodesToUpdate.slice(1).forEach(function (node) {
                i++; // Increment i for each iteration
                data.edges.add({ from: nodesToUpdate[0].id, to: node.parentId, arrows: 'to', color: getEdgeColorModified(dataFromGraph, node.parentId, node.label) });
                dataFromGraph.forEach(function (allNodes) {
                    if (allNodes.parentId == node.id) {
                        allNodes.parentId = nodesToUpdate[0].id;
                        data.edges.add({ from: allNodes.id, to: nodesToUpdate[0].id, arrows: 'to', color: getEdgeColorModified(dataFromGraph, node.parentId, node.label) });
                    }
                });
                data.nodes.remove({ id: node.id });
            });
        }
    }
    var positions = network.getPositions();
    network.setData(data);
    network.once("stabilizationIterationsDone", function () {
        // Iterate over the positions object and update node positions
        for (const nodeId in positions) {
            if (positions.hasOwnProperty(nodeId)) {
                const position = positions[nodeId];
                network.moveNode(nodeId, position.x, position.y);
            }
        }
    });
}

function drawOnCanvasLabels(ctx, container) {
    // Draw text onto the canvas
    const canvasWidth = container.offsetWidth;
    const canvasHeight = container.offsetHeight;
    const centerX = canvasWidth / 2;
    const centerY = canvasHeight / 2;
    ctx.font = "16px Arial";
    ctx.fillStyle = "black";
    ctx.fillText("Barvy hran:", 100 - centerX, 100 - centerY); // Adjust position as needed
    ctx.fillStyle = "blue";
    ctx.fillRect(195 - centerX, 111 - centerY, 10, 10); // Sample color indicator
    ctx.fillText("Levá strana", 100 - centerX, 120 - centerY); // Adjust position as needed
    ctx.fillStyle = "orange";
    ctx.fillRect(195 - centerX, 133 - centerY, 10, 10); // Sample color indicator
    ctx.fillText("Pravá strana", 100 - centerX, 140 - centerY); // Adjust position as needed
    // Add more color descriptions as needed
}

function modifyNodes(nodesData) {
    const labelToIdMap = {};
    var changeTitles = []; // Change changeTitle to changeTitles to store multiple titles
    finished = true;

    // Iterate through nodesData and update the IDs based on label
    for (const nodeData of nodesData) {
        const label = nodeData.title;

        //if we already have label stored in map we will do operation
        if (labelToIdMap[label] !== undefined) {
            changeTitles.push(label); // Push label to changeTitles array

        } else {
            // If label doesn't exist, store the current ID for future reference
            labelToIdMap[label] = nodeData.id;
        }
    }

    return changeTitles; // Return changeTitles array
}

function updateStepInfo() {
    var stepInfo = steps[globalCounter]; // Assuming steps is an array of step information
    var stepsDiv = document.getElementById("Steps");

    // Clear previous step info
    stepsDiv.innerHTML = "";

    // Create Bootstrap info alert element
    var stepAlert = document.createElement("div");
    stepAlert.classList.add("alert", "alert-info");
    stepAlert.setAttribute("role", "alert");
    stepAlert.innerText = stepInfo; // Set the step information as inner text

    // Append the alert element to the Steps div
    stepsDiv.appendChild(stepAlert);
}

function handleButtonNextStep() {
    if (globalCounter < lengthOfList - 1) {
        globalCounter++;
        drawGraph();
        updateStepInfo();
    } else {
        alert("Toto je poslední krok");
    }
    updateButtonStates();
}

function handleButtonPreviousStep() {
    if (globalCounter > 0) {
        globalCounter--;
        drawGraph();
        updateStepInfo();
    } else {
        alert("Toto je první krok");
    }
    updateButtonStates();
}

function updateButtonStates() {
    var previousStepButton = document.getElementById('previousStepButton');
    var nextStepButton = document.getElementById('nextStepButton');

    if (previousStepButton) {
        previousStepButton.disabled = (globalCounter === 0);
    }

    if (nextStepButton) {
        nextStepButton.disabled = (globalCounter === lengthOfList - 1);
    }
}

function createButtons() {
    var buttonsDiv = document.getElementById('buttons');
    buttonsDiv.innerHTML = '';

    previousStepButton = document.createElement('button');
    previousStepButton.id = 'previousStepButton';
    previousStepButton.className = 'btn btn-primary flex-fill mb-2 mr-1';
    previousStepButton.textContent = 'předchozí krok';
    previousStepButton.disabled = (globalCounter === 0);
    buttonsDiv.appendChild(previousStepButton);

    nextStepButton = document.createElement('button');
    nextStepButton.id = 'nextStepButton';
    nextStepButton.className = 'btn btn-primary flex-fill mb-2 mr-1';
    nextStepButton.textContent = 'další krok';
    nextStepButton.disabled = (globalCounter === lengthOfList);
    buttonsDiv.appendChild(nextStepButton);
}

function attachEventHandlers() {
    //button for 'Vykresli DAG'
    document.getElementById('drawDAGButtonTautotology').addEventListener('click', function () {
        handleButtonDrawDAGButton(true);
    });

    document.getElementById('drawDAGButtonContradiction').addEventListener('click', function () {
        handleButtonDrawDAGButton(false);
    });

    var nextStepButton = document.getElementById('nextStepButton');

    // If the element exists, attach the event listener
    if (nextStepButton) {
        nextStepButton.addEventListener('click', handleButtonNextStep);
    }

    var previousStepButton = document.getElementById('previousStepButton');

    if (previousStepButton) {
        previousStepButton.addEventListener('click', handleButtonPreviousStep)
    }

    $('#DAGS').submit(function (event) {
        // Prevent the default form submission
        event.preventDefault();
    });
}

$(document).ready(attachEventHandlers)