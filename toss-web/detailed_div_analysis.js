const fs = require('fs');

const content = fs.readFileSync('pages/crm/index.vue', 'utf8');
const lines = content.split('\n');

// Find template boundaries
let templateStart = -1;
let templateEnd = -1;

lines.forEach((line, index) => {
  if (line.trim() === '<template>') templateStart = index;
  if (line.trim() === '</template>') templateEnd = index;
});

console.log(`Template from line ${templateStart + 1} to line ${templateEnd + 1}`);

// Analyze just the template section
const templateLines = lines.slice(templateStart + 1, templateEnd);
let divStack = [];
let issues = [];

templateLines.forEach((line, index) => {
  const lineNum = templateStart + index + 2; // +2 because we start after <template>
  
  // Find opening divs
  const openMatches = [...line.matchAll(/<div[^>]*>/g)];
  openMatches.forEach(match => {
    divStack.push({ line: lineNum, content: match[0], depth: divStack.length });
    console.log(`${'  '.repeat(divStack.length - 1)}OPEN ${lineNum}: ${match[0].substring(0, 50)}...`);
  });
  
  // Find closing divs
  const closeMatches = [...line.matchAll(/<\/div>/g)];
  closeMatches.forEach(() => {
    if (divStack.length === 0) {
      issues.push(`Line ${lineNum}: Extra closing div`);
      console.log(`ERROR ${lineNum}: Extra closing div`);
    } else {
      const opened = divStack.pop();
      console.log(`${'  '.repeat(divStack.length)}CLOSE ${lineNum}: closes div from line ${opened.line}`);
    }
  });
});

console.log(`\nRemaining unclosed divs: ${divStack.length}`);
divStack.forEach(div => {
  console.log(`Unclosed: Line ${div.line}: ${div.content}`);
});
