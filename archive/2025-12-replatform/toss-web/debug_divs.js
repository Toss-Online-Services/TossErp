const fs = require('fs');

const content = fs.readFileSync('pages/crm/index.vue', 'utf8');
const lines = content.split('\n');

let openDivs = [];
let errors = [];

lines.forEach((line, index) => {
  const lineNum = index + 1;
  const trimmed = line.trim();
  
  // Check for opening div tags
  const openMatches = line.match(/<div[^>]*>/g);
  if (openMatches) {
    openMatches.forEach(match => {
      openDivs.push({ line: lineNum, tag: match });
    });
  }
  
  // Check for closing div tags
  const closeMatches = line.match(/<\/div>/g);
  if (closeMatches) {
    closeMatches.forEach(() => {
      if (openDivs.length === 0) {
        errors.push(`Line ${lineNum}: Closing div without matching opening div`);
      } else {
        openDivs.pop();
      }
    });
  }
});

console.log(`Total unclosed divs: ${openDivs.length}`);

if (openDivs.length > 0) {
  console.log('\nUnclosed div tags:');
  openDivs.forEach(div => {
    console.log(`Line ${div.line}: ${div.tag}`);
  });
}

if (errors.length > 0) {
  console.log('\nErrors:');
  errors.forEach(error => console.log(error));
}
