// Map to store unique labels and their corresponding IDs
const labelIdMap = new Map();

function handleButtonDrawGraphButton() {
    CallAjaxToGetPaths(false);
    
    
}

function handleButtonDrawDAGButton() {
    CallAjaxToGetPaths(true);
    createGraph();
}

function CallAjaxToGetPaths(isDag) {
    var userInput = $('#UserInput').val();
    var formula = $('#formula').val();
    var dataToSend = userInput ? userInput : formula;
    $.ajax({
        url: '?handler=DrawDAG',
        beforeSend: function (xhr) {
            xhr.setRequestHeader("XSRF-TOKEN",
                $('input:hidden[name="__RequestVerificationToken"]').val());
        },
        type: 'POST',
        contentType: 'application/json',
        dataType: "json",
        data: JSON.stringify(dataToSend),
        success: function (output) {
            console.log('Node Object:', output);
            createGraph(isDag, output);
        },
        error: function (error) {
            console.error('Error:', error);
            // Handle the error
        }
    });
}




function Node(id, label, parentId = null) {
    this.id = id;
    this.label = label;
    this.parentId = parentId;
}

function createGraph(isDag,dagPaths) {
    // Parse the JSON string to get the array of node objects
    var nodesData = JSON.parse(dagPaths);
    if (isDag) {
        nodesData = modifyNodes(nodesData);
    }

    const existingNodes = {};
    const nodes = new vis.DataSet();
    const edges = new vis.DataSet();


    // Iterate through nodesData to create nodes and edges
    for (let i = 0; i < nodesData.length; i++) {
        const nodeData = nodesData[i];
        const nodeId = nodeData.Id;
        const parentId = nodeData.ParentId;

        if (parentId === 0) {
            // Create a new root node
            nodes.add({ id: nodeId, label: nodeData.Label });
            continue;
        }
        if (!existingNodes[nodeId]) {
            // Create a new node if it doesn't exist
            existingNodes[nodeId] = true;
            nodes.add({ id: nodeId, label: nodeData.Label, parentId });
            edges.add({ from: nodeId, to: parentId, arrows: 'to' });
            continue;
        }

        // Create an edge if the parent node exists
        else if (existingNodes[nodeId]) {
            edges.add({ from: nodeId, to: parentId, arrows: 'to', color: 'orange'});
            continue;
        }
    }
   
    // Update the vis.js network
    const container = document.getElementById('treeVisualization');
  
    const data = { nodes, edges };
    const options = {};
    const network = new vis.Network(container, data, options);
}

function modifyNodes(nodesData) {
    const labelToIdMap = {};

    // Iterate through nodesData and update the IDs based on label
    nodesData.forEach(nodeData => {
        const label = nodeData.Label;
        if (labelToIdMap[label] !== undefined) {
            // If label already exists, override the ID of the next node
            nodeData.Id = labelToIdMap[label];
        } else {
            labelToIdMap[label] = nodeData.Id;
        }
    });

    return nodesData;
}

$(document).ready(function () {
    //button for 'Vykresli DAG'
    document.getElementById('drawGraphButton').addEventListener('click', handleButtonDrawGraphButton);
    document.getElementById('drawDAGButton').addEventListener('click', handleButtonDrawDAGButton);
    $('#DAGForm').submit(function (event) {
        // Prevent the default form submission
        event.preventDefault();
    });
})