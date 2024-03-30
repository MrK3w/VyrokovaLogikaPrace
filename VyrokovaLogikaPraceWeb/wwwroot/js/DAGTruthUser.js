// Map to store unique labels and their corresponding IDs
const labelIdMap = new Map();
var data;
var contradiction = false;
var formula;

function handleButtonDrawDAGButton() {
    CallAjaxToGetPaths();
}

function CallAjaxToGetPaths() {

    formula = transformInputValue($('#formula').val());

    var requestData = {
        formula: formula,
        tautology: true // Include the boolean value
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
            parsedOutput.sort((a, b) => b.Id - a.Id);
            createGraph(parsedOutput);
            prepareButtons();
            attachEventHandlers();
        },
        error: function (error) {
            // Log specific error properties
            console.log('Status:', error.status); // HTTP status code
            console.log('Status Text:', error.statusText); // Textual description of the HTTP status
            console.log('Response Text:', error.responseText); // Response body
            console.log('Ready State:', error.readyState); // Ready state of the request
        }
    });
}
//function to draw graph
function createGraph(nodesData) {
    //if it is not first run we will not change from tree
    const nodes = new vis.DataSet();
    const edges = new vis.DataSet();

    // Iterate through nodesData to create nodes and edges
    for (let i = 0; i < nodesData.length; i++) {
        //if node does not have parentId then it is root
                if (nodesData[i].ParentId === 0) {
                    nodes.add({ id: nodesData[i].Id, label: nodesData[i].Operator + " = " + nodesData[i].TruthValue, title: nodesData[i].Label, parentId: nodesData[i].ParentId, truthValue: nodesData[i].TruthValue, contradiction: false, color: { background: '#FFD700' } });
                }
                else {
                    nodes.add({ id: nodesData[i].Id, label: nodesData[i].Operator + " = " + nodesData[i].TruthValue, title: nodesData[i].Label, parentId: nodesData[i].ParentId, truthValue: nodesData[i].TruthValue, contradiction: false });
                    edges.add({ from: nodesData[i].Id, to: nodesData[i].ParentId, arrows: 'to', color: getEdgeColor(nodesData, nodesData[i].ParentId, nodesData[i].Label) });
                }
    }

    // Update the vis.js network
    const container = document.getElementById('treeVisualization');
    data = { nodes, edges };
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
    network.on("click", function (params) {
        if (!(params.nodes[0] === undefined)) {
            var nodeId = params.nodes[0];
            if (nodeId !== undefined) {
                handleNodeClick(nodeId);
            }
        }
    });
    updateNodes(data, network);
}

function handleNodeClick(nodeId) {
    const existingNode = data.nodes.get(nodeId);
    if (!contradiction) {
        if (existingNode.label.includes('-1')) {
            existingNode.label = existingNode.label.replace('-1', '0');
            existingNode.truthValue = '0';
        }
        else if (existingNode.label.includes('0')) {
            existingNode.label = existingNode.label.replace('0', '1');
            existingNode.truthValue = '1';
        }
        else if (existingNode.label.includes('1')) {
            existingNode.label = existingNode.label.replace('1', '0');
            existingNode.truthValue = '0';
        }
    }
    else {
        if (existingNode.label.includes('?')) {
            existingNode.label = existingNode.label.replace(' ?', '');
            existingNode.contradiction = false;
        }
        else {
            existingNode.label += " ?";
            existingNode.contradiction = true;
        }
    }
    data.nodes.update(existingNode);
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

function prepareButtons() {
    document.getElementById('formule').innerHTML = 'Synktaktick\u00FD strom formule <span style="color: red;">' + formula + '</span>.';

    var buttonsDiv = document.getElementById('buttons');
    var selectListDiv = document.getElementById('selectListGroup');
    buttonsDiv.innerHTML = '';
    selectListDiv.innerHTML = '';

    // Create and append the "Označ spor" button
    var contradictionButton = document.createElement('button');
    contradictionButton.id = 'contradictionButton';
    contradictionButton.className = 'btn btn-primary flex-fill mb-2 mr-1';
    contradictionButton.textContent = 'Ozna\u010d spor';
    buttonsDiv.appendChild(contradictionButton);

    // Create and append the "Ověř strom" button
    var checkTreeButton = document.createElement('button');
    checkTreeButton.id = 'checkTree';
    checkTreeButton.className = 'btn btn-primary flex-fill mb-2 mr-1';
    checkTreeButton.textContent = 'Ov\u011b\u0159 strom';
    buttonsDiv.appendChild(checkTreeButton);
    // Create select element
    // Create select element
    var selectList = document.createElement('select');
    selectList.id = 'mySelect';
    selectList.className = 'form-control mr-2'; // Add Bootstrap form-control class for styling

    // Create options
    var options = ['formule je tautologi\u00ED', 'formule je kontradikc\u00ED', 'formule je splniteln\u00E1']; // Using Unicode escape sequence for 'í'

    // Add options to select
    for (var i = 0; i < options.length; i++) {
        var option = document.createElement('option');
        option.value = options[i];
        option.text = options[i];
        selectList.appendChild(option);
    }

    // Append select list to buttonsDiv
    selectListDiv.appendChild(selectList);
}

function attachEventHandlers() {
    //button for 'Vykresli DAG'
    document.getElementById('drawDAGButton').addEventListener('click', handleButtonDrawDAGButton);

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

    $('#DAGS').submit(function (event) {
        // Prevent the default form submission
        event.preventDefault();
    });
}

$(document).ready(attachEventHandlers)