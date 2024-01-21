function handleButtonDrawDAGButton() {
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
        success: function (dagPaths) {  
            console.log('Node Object:', dagPaths);
            CreateDAG(dagPaths);
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

function CreateDAG(dagPaths) {
    // Parse the JSON string to get the array of node objects
    const nodesData = JSON.parse(dagPaths);

    // Create nodes based on the dagPaths result
    const nodes = new vis.DataSet(nodesData.map(nodeData => {
        if (nodeData.ParentId === 0) {
            // If ParentId is 0, create a new root node
            return { id: nodeData.Id, label: nodeData.Label };
        } else {
            // If ParentId is not 0, create a new node with ParentId
            return { id: nodeData.Id, label: nodeData.Label, parentId: nodeData.ParentId };
        }
    }));

    // Create edges based on the nodes
    const edges = new vis.DataSet(nodesData.map(nodeData => ({ from: nodeData.Id, to: nodeData.ParentId, arrows: 'to' })));

    // Update the vis.js network
    const container = document.getElementById('treeVisualization');
    const data = { nodes, edges };
    const options = {
        layout: {
            hierarchical: {
                direction: 'UD', // Up-Down layout
            },
        },
    };

    const network = new vis.Network(container, data, options);
}

$(document).ready(function () {
    //button for 'Vykresli DAG'
    document.getElementById('drawDAGButton').addEventListener('click', handleButtonDrawDAGButton);
    $('#DAGForm').submit(function (event) {
        // Prevent the default form submission
        event.preventDefault();
    });
})