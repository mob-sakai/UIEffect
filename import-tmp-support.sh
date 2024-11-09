#!/bin/bash -e

sample=$1

# Check the sample
[ ! -d "Packages/src/Samples~/$sample~" ] && echo "Not found in samples: $sample" && exit 1

# Already imported
[ -e "Assets/$sample" ] && echo "Already imported: $sample" && exit 0

# Remove the old sample
find Assets -name "TextMeshPro Support*" -depth 1 -exec rm {} \;

# Create a symbolic link to the sample
ln -s "../Packages/src/Samples~/$sample~" Assets
mv "Assets/$sample~" "Assets/$sample"

echo "Imported: $sample"