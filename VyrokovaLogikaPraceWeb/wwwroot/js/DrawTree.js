function toggleNode(nodeId, treeValue, op) {
    var node = document.getElementById('node_' + nodeId);
    var span = node.querySelector('.tf-nc');
    console.log(op);
    // Toggle between tree.Text and op
    span.innerText = (span.innerText === treeValue) ? op : treeValue;
}