function toggleNode(nodeId, treeValue, op, truthValue) {
    var node = document.getElementById('node_' + nodeId);
    var span = node.querySelector('.tf-nc');

    // Toggle between tree.Text and op
    span.innerText = (span.innerText === treeValue + "= " + truthValue) ? op + "= " +truthValue: treeValue + "= " + truthValue;
}