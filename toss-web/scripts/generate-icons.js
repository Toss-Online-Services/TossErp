/**
 * PWA Icon Generation Script
 * 
 * This script generates PWA icons in various sizes from the source SVG.
 * 
 * To generate icons, you need to:
 * 1. Install sharp: npm install -D sharp
 * 2. Run: node scripts/generate-icons.js
 * 
 * Or use an online tool:
 * - https://realfavicongenerator.net/
 * - https://www.pwabuilder.com/imageGenerator
 * 
 * Required sizes: 72x72, 96x96, 128x128, 144x144, 152x152, 192x192, 384x384, 512x512
 */

const fs = require('fs');
const path = require('path');

// Try to use sharp if available
let sharp;
try {
  sharp = require('sharp');
} catch (e) {
  console.log('Sharp not installed. Please install it: npm install -D sharp');
  console.log('Or use an online icon generator tool.');
  process.exit(0);
}

const sizes = [72, 96, 128, 144, 152, 192, 384, 512];
const inputSvg = path.join(__dirname, '../public/icon.svg');
const outputDir = path.join(__dirname, '../public/icons');

// Ensure output directory exists
if (!fs.existsSync(outputDir)) {
  fs.mkdirSync(outputDir, { recursive: true });
}

async function generateIcons() {
  console.log('Generating PWA icons...');
  
  for (const size of sizes) {
    const outputPath = path.join(outputDir, `icon-${size}x${size}.png`);
    
    try {
      await sharp(inputSvg)
        .resize(size, size)
        .png()
        .toFile(outputPath);
      
      console.log(`✓ Generated ${size}x${size} icon`);
    } catch (error) {
      console.error(`✗ Failed to generate ${size}x${size}:`, error.message);
    }
  }
  
  // Also generate favicon
  const faviconPath = path.join(__dirname, '../public/favicon.ico');
  try {
    await sharp(inputSvg)
      .resize(32, 32)
      .png()
      .toFile(faviconPath.replace('.ico', '.png'));
    console.log('✓ Generated favicon.png (rename to .ico if needed)');
  } catch (error) {
    console.error('✗ Failed to generate favicon:', error.message);
  }
  
  console.log('\n✨ Icon generation complete!');
}

generateIcons().catch(console.error);

