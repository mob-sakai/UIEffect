const fs = require("node:fs");
const path = require("node:path");
const version = process.argv[2];

if (!version) {
  throw new Error("Release version must be specified.");
}

// Find package.json files in each sample directory.
const packageFiles = fs
  .readdirSync("Samples~", { withFileTypes: true })
  .filter((entry) => entry.isDirectory())
  .map((entry) => path.join("Samples~", entry.name, "package.json"))
  .filter((filePath) => fs.existsSync(filePath));

// Update each sample package version and internal package dependencies to the release version.
for (const filePath of packageFiles) {
  const packageJson = JSON.parse(fs.readFileSync(filePath, "utf8"));

  // Update the package version to the release version.
  let changed = packageJson.version !== version;
  packageJson.version = version;

  // Update internal package dependencies to the same release version.
  if (packageJson.dependencies !== undefined) {
    for (const dependencyName of Object.keys(packageJson.dependencies)) {
      if (dependencyName.startsWith("com.coffee.ui-effect") && packageJson.dependencies[dependencyName] !== version) {
        packageJson.dependencies[dependencyName] = version;
        changed = true;
      }
    }
  }

  // Write only when the package version or an internal dependency changed.
  if (changed) {
    fs.writeFileSync(filePath, `${JSON.stringify(packageJson, null, 2)}\n`);
  }
}
