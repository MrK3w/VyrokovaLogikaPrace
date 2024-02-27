function transformInputValue(inputValue) {
    return inputValue
    .replace(/&/g, '∧')
    .replace(/\|/g, '∨')
    .replace(/=/g, '≡')
    .replace(/-/g, '¬')
    .replace(/>/g, '⇒')
    .replace(/--/g, '¬¬');

}
