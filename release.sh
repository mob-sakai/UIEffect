#!/bin/bash -xe

# Release the project with the following steps:
#   1. Update the release version in package.json.
#   2. Update "CHANGELOG.md" using "github_changelog_generator-1.15.0.pre.beta".
#   3. Commit package.json and CHANGELOG.md.
#   4. Merge into master branch.
#   5. Export unitypackage.
#   6. Release using "gh-release-3.2.0". (Upload unitypackage)
UNITY_PATH=/Applications/Unity5.5.0p4/Unity5.5.0p4.app/Contents/MacOS/Unity


# input version
PACKAGE_NAME=`node -pe 'require("./package.json").name'`
echo Github Release: $PACKAGE_NAME
read -p "[? release version (for example: 1.0.0): " RELEASE_VERSION
[ -z "$RELEASE_VERSION" ] && exit


# update version
git checkout -B release develop
sed -i -e "s/\"version\": \(.*\)/\"version\": \"${RELEASE_VERSION}\",/g" package.json


# generate change log
TAG=v$RELEASE_VERSION
git tag $TAG
git push --tags
github_changelog_generator
git tag -d $TAG
git push --delete origin $TAG


# commit files
git add CHANGELOG.md -f
git add package.json -f
git commit -m "update change log"


# merge and push
git checkout master
git merge --no-ff release -m "release $TAG"
git branch -D release
git push origin master


# export .unitypackage and release on Github
PACKAGE_SRC=`node -pe 'require("./package.json").src'`
$UNITY_PATH -quit -batchmode -projectPath "`pwd`" -exportpackage $PACKAGE_SRC $PACKAGE_NAME.unitypackage
gh-release  --draft --assets $PACKAGE_NAME.unitypackage


echo "\n\n$PACKAGE_NAME v$RELEASE_VERSION has been successfully released!\n"
