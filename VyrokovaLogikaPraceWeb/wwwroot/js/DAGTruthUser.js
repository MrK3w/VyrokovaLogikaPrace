// Map to store unique labels and their corresponding IDs
const labelIdMap = new Map();
var data;
var contradiction = false;
var formula;
var typeOfFormula;
var network;
var errorsDiv;

function handleButtonDrawDAGButton() {
    CallAjaxToGetPaths();
}

function CallAjaxToGetPaths() {

    formula = transformInputValue($('#formula').val());

    $.ajax({
        url: '?handler=TruthDAG',
        beforeSend: function (xhr) {
            xhr.setRequestHeader("XSRF-TOKEN",
                $('input:hidden[name="__RequestVerificationToken"]').val());
        },
        type: 'POST',
        contentType: 'application/json',
        dataType: "json",
        data: JSON.stringify(formula),
        success: function (data) {
            typeOfFormula = data.typeOfFormula;
            data.visNodes.sort((a, b) => b.Id - a.Id);
            createGraph(data.visNodes);
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

function checkTree() {

    errorsDiv = document.getElementById("errors");

    // Clear previous step info
    errorsDiv.innerHTML = "";

    var selectedValue = document.getElementById("mySelect").value;
    var nodes = data.nodes.get();
    var edges = data.edges.get();
    clearColor(nodes);
    network.setData(data);
    if (!allNodesAreFilled()) {

        network.setData(data);
        addError("Některé uzly jsou nevyplněné.");
        return;
    }
    if (selectedValue == "tautology" && nodes[0].truthValue == 1) {
        nodes[0].color.background = "red";
        network.setData(data);
        addError("Pokud se snažíme dokázat tautologii, tak nemůže mít hlavní uzel pravidovstní hodnotu 1.");
        return;
    }
    if (selectedValue == "contradiction" && nodes[0].truthValue == 0) {
        nodes[0].color.background = "red";
        network.setData(data);
        addError("Pokud se snažíme dokázat kontradikci, tak nemůže mít hlavní uzel pravidovstní hodnotu 0.");
        return;
    }
    for (let i = 0; i < nodes.length; i++) {
        let childrenNodes = nodes.filter(node => node.parentId === nodes[i].id);
        getSide(childrenNodes, edges);
        if (childrenNodes.length == 0) continue;
        checkEdges(nodes[i], childrenNodes);
        
    }
    network.setData(data);
}


function addError(message) {
    // Create a new div for the Bootstrap alert
    var newAlert = document.createElement("div");
    newAlert.classList.add("alert", "alert-danger");
    newAlert.setAttribute("role", "alert");
    newAlert.textContent = message;

    // Append the new alert to the errorsDiv
    errorsDiv.appendChild(newAlert);
}

function getSide(childrenNodes, edges) {
    for (let i = 0; i < childrenNodes.length; i++) {
        for (let j = 0; j < edges.length; j++) {
            if (childrenNodes[i].id == edges[j].from && childrenNodes[i].parentId == edges[j].to) {
                if (edges[j].color == "blue") {
                    childrenNodes[i].side = "left";
                }
                else {
                    childrenNodes[i].side = "right";
                }
            }
        }
    }
}

function checkEdges(nodes, childrenNodes) {
    var operator = nodes.label[0];
    if (nodes.label[1] == '¬') operator += '¬';
    var parentTruthValue = nodes.truthValue;
    var leftChildTruthValue = -1;
    var rightChildTruThValue = -1;
    if (childrenNodes.length == 2) {
        if (childrenNodes[0].side == "left") {
            leftChildTruthValue = childrenNodes[0].truthValue;
            rightChildTruThValue = childrenNodes[1].truthValue;
        }
        else {
            leftChildTruthValue = childrenNodes[1].truthValue;
            rightChildTruThValue = childrenNodes[0].truthValue;
        }
    }

    else {
        leftChildTruthValue = childrenNodes[0].truthValue;
    }
    if (operator == "≡") {
        if (parentTruthValue == 0) {
            if (leftChildTruthValue != rightChildTruThValue) return;
            else {
                var node = data.nodes.get(childrenNodes[0].id);
                node.color.background = 'red';
                node = data.nodes.get(childrenNodes[1].id);
                node.color.background = 'red';
                addError("Pokud je pravdivostní hodnota hlavního uzlu 0 a operátor ekvivalence tak potomci nesmí mít stejnou hodnotu");


            }
        }
        else {
            if (leftChildTruthValue == rightChildTruThValue) return;
            else {
                var node = data.nodes.get(childrenNodes[0].id);
                node.color.background = 'red';
                node = data.nodes.get(childrenNodes[1].id);
                node.color.background = 'red';
                addError("Pokud je pravdivostní hodnota hlavního uzlu 1 a operátor ekvivalence tak potomci nesmí mít jinou hodnotu")
            }
        }
    }
    else if (operator == "⇒") {
        if (parentTruthValue == 0) {
            if (leftChildTruthValue == 1 && rightChildTruThValue == 0) return;
            else {
                var node = data.nodes.get(childrenNodes[0].id);
                node.color.background = 'red';
                node = data.nodes.get(childrenNodes[1].id);
                node.color.background = 'red';
                addError("Pokud je pravdivostní hodnota hlavního uzlu 0 a operátor implikace tak levý potomek musí mít hodnotu 1 a pravý 0");


            }
        }
        else {
            if (leftChildTruthValue == 1 && rightChildTruThValue == 0) {
                var node = data.nodes.get(childrenNodes[0].id);
                node.color.background = 'red';
                node = data.nodes.get(childrenNodes[1].id);
                node.color.background = 'red';
                addError("Pokud je pravdivostní hodnota hlavního uzlu 1 a operátor implikace tak levý potomek nesmí mít pravdivostní hodnotu 1 a pravý 0");
            }
            else {
                return;
            }
        }
    }
    else if (operator == "∨") {
        if (parentTruthValue == 0) {
            if (leftChildTruthValue == 0 && rightChildTruThValue == 0) return;
            else {
                var node = data.nodes.get(childrenNodes[0].id);
                node.color.background = 'red';
                node = data.nodes.get(childrenNodes[1].id);
                node.color.background = 'red';
                addError("Pokud je pravdivostní hodnota hlavního uzlu 0 a operátor disjunkce tak levý potomek musí mít hodnotu 0 a pravý 0");
            }
        }
        else {
            if (leftChildTruthValue == 0 && rightChildTruThValue == 0) {
                var node = data.nodes.get(childrenNodes[0].id);
                node.color.background = 'red';
                node = data.nodes.get(childrenNodes[1].id);
                node.color.background = 'red';
                addError("Pokud je pravdivostní hodnota hlavního uzlu 1 a operátor disjunkce tak levý potomek nesmí mít hodnotu 0 a pravý 0");
            }
            else {
                return;
            }
        }
    }
    else if (operator == "∧") {
        if (parentTruthValue == 0) {
            if (leftChildTruthValue == 1 && rightChildTruThValue == 1) {
                var node = data.nodes.get(childrenNodes[0].id);
                node.color.background = 'red';
                node = data.nodes.get(childrenNodes[1].id);
                node.color.background = 'red';
                addError("Pokud je pravdivostní hodnota hlavního uzlu 0 a operátor konjunkce tak levý potomek nesmí mít hodnotu 0 a pravý 0");
            }
            else {
                return;
            }
        }
        else {
            if (leftChildTruthValue == 1 && rightChildTruThValue == 1) {
                return;
            }
            else {
                var node = data.nodes.get(childrenNodes[0].id);
                node.color.background = 'red';
                node = data.nodes.get(childrenNodes[1].id);
                node.color.background = 'red';
                addError("Pokud je pravdivostní hodnota hlavního uzlu 1 a operátor konjunkce tak levý potomek musí mít hodnotu 1 a pravý 1");
            }
        }
    }
    else if (operator = "¬") {
        if (parentTruthValue == 0) {
            if (leftChildTruthValue == 1) return;
            else {
                var node = data.nodes.get(childrenNodes[0].id);
                node.color.background = 'red';
                addError("Pokud je pravdivostní hodnota hlavního uzlu 0 a operátor negace tak potomek nesmí mít hodnotu 0");
            }
        }
        else {
            if (leftChildTruthValue == 0) return;
            else {
                var node = data.nodes.get(childrenNodes[0].id);
                node.color.background = 'red';
                addError("Pokud je pravdivostní hodnota hlavního uzlu 1 a operátor negace tak potomek nesmí mít hodnotu 1");
            }
        }
    }
    else if (operator = "¬¬")
    {
        if (parentTruthValue == 0) {
            if (leftChildTruthValue == 0) return;
            else {
                var node = data.nodes.get(childrenNodes[0].id);
                node.color.background = 'red';
                addError("Pokud je pravdivostní hodnota hlavního uzlu 0 a operátor dvojí negace tak potomek nesmí mít hodnotu 1");
            }
        }
        else {
            if (leftChildTruthValue == 1) return;
            else {
                var node = data.nodes.get(childrenNodes[0].id);
                node.color.background = 'red';
                addError("Pokud je pravdivostní hodnota hlavního uzlu 1 a operátor dvojí negace tak potomek nesmí mít hodnotu 0");
            }
        }
    }
}

function allNodesAreFilled() {
    var nodes = data.nodes.get();
    var filled = true;
    for (let i = 0; i < nodes.length; i++) {
        if (nodes[i].truthValue == -1) {
            nodes[i].color.background = 'red';
            filled = false;
        }
    }
    return filled;
}

function clearColor() {
    var nodes = data.nodes.get();
    for (let i = 0; i < nodes.length; i++) {
        if (nodes[i].parentId == 0) {
            nodes[i].color.background = '#FFD700';
        }
        else {
            nodes[i].color.background = '#97c2fc';
        }
    }
    return;
}

//function to draw graph
function createGraph(nodesData) {
    //if it is not first run we will not change from tree
    const nodes = new vis.DataSet();
    const edges = new vis.DataSet();

    // Iterate through nodesData to create nodes and edges
    for (let i = 0; i < nodesData.length; i++) {
        //if node does not have parentId then it is root
                if (nodesData[i].parentId === 0) {
                    nodes.add({ id: nodesData[i].id, label: nodesData[i].operator + " = " + nodesData[i].truthValue, title: nodesData[i].label, parentId: nodesData[i].parentId, truthValue: nodesData[i].truthValue, contradiction: false, color: { background: '#FFD700' } });
                }
                else {
                    nodes.add({ id: nodesData[i].id, label: nodesData[i].operator + " = " + nodesData[i].truthValue, title: nodesData[i].label, parentId: nodesData[i].parentId, truthValue: nodesData[i].truthValue, contradiction: false, color: { background: '#97c2fc' } });
                    edges.add({ from: nodesData[i].id, to: nodesData[i].parentId, arrows: 'to', color: getEdgeColor(nodesData, nodesData[i].parentId, nodesData[i].label) });
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

    network = new vis.Network(container, data, options);

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
        if (existingNode.parentId != 0) {
        if (existingNode.label.includes('?')) {
            existingNode.label = existingNode.label.replace(' ?', '');
            existingNode.contradiction = false;
        }
        else {
            existingNode.label += " ?";
            existingNode.contradiction = true;
            }
        }
    }
    data.nodes.update(existingNode);
}

function getEdgeColor(nodesData, parentId, label) {
    // Find the parent node based on label
    var parentNode = nodesData.find(node => node.id === parentId);
    //if parentNode label starts with parentheses we need to add that parentheses to temp label to be able to properly check it
    if (parentNode && parentNode.label[0] == '(')
        label = '(' + label;

    const parentLabelStart = parentNode ? parentNode.label.substring(0, label.length) : null;
    let edgeColor = 'blue';

    // Check if the parent operator is ¬ or ¬¬ (if it is then there will not be right side)
    if (parentNode && (parentNode.operator === '¬' || parentNode.operator === '¬¬')) {
        edgeColor = 'blue';
    } else if (parentLabelStart !== label) {
        edgeColor = 'orange';
    }
    return edgeColor;
}

function getEdgeColorModified(nodesData, parentId, label) {
    label = label.replace(" ", "");
    label = label.split('=')[0];
    // Find the parent node based on label
    var parentNode = nodesData.find(node => node.id === parentId);
    //if parentNode label starts with parentheses we need to add that parentheses to temp label to be able to properly check it
    if (parentNode && parentNode.title[0] == '(')
        label = '(' + label;

    const parentLabelStart = parentNode ? parentNode.title.substring(0, label.length) : null;
    let edgeColor = 'blue';

    // Check if the parent operator is ¬ or ¬¬ (if it is then there will not be right side)
    if (parentNode && (parentNode.label.startsWith('¬') || parentNode.label.startsWith('¬¬'))) {

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
    var optionsText = ['formule je tautologi\u00ED', 'formule je kontradikc\u00ED', 'formule je splniteln\u00E1']; // Using Unicode escape sequence for 'í'
    var options = ['tautology', 'contradiction', 'satisfiable'];
    // Add options to select
    for (var i = 0; i < options.length; i++) {
        var option = document.createElement('option');
        option.value = options[i];
        option.text = optionsText[i];
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

    $("#checkTree").on("click", function () {
        checkTree();
    });

    $('#DAGS').submit(function (event) {
        // Prevent the default form submission
        event.preventDefault();
    });
}

$(document).ready(attachEventHandlers)