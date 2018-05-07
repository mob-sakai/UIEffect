#!/bin/bash -e

# NOTE: Set an environment variable `CHANGELOG_GITHUB_TOKEN` by running the following command at the prompt, or by adding it to your shell profile (e.g., ~/.bash_profile or ~/.zshrc):
#   export CHANGELOG_GITHUB_TOKEN="«your-40-digit-github-token»"

# Release the project with the following steps:
#   1. Update the release version in package.json.
#   2. Update "CHANGELOG.md" using "github_changelog_generator-1.15.0.pre.rc".
#   3. Commit package.json and CHANGELOG.md.
#   4. Merge into master branch.
#   5. Export unitypackage.
#   6. Release using "gh-release-3.2.0". (Upload unitypackage)


# input release version
PACKAGE_NAME=`node -pe 'require("./package.json").name'`
echo Github Release: $PACKAGE_NAME
read -p "[? release version (for example: 1.0.0): " RELEASE_VERSION
[ -z "$RELEASE_VERSION" ] && exit


# update version in package.json
git checkout -B release develop
sed -i -e "s/\"version\": \(.*\)/\"version\": \"${RELEASE_VERSION}\",/g" package.json


# generate change log
TAG=v$RELEASE_VERSION
git tag $TAG
git push --tags
github_changelog_generator
git tag -d $TAG
git push --delete origin $TAG


git diff -- CHANGELOG.md
read -p "[? continue? (y/N):" yn
case "$yn" in [yY]*) ;; *) exit ;; esac


# export unitypackage
UNITY_EDITOR=`node -pe 'require("./package.json").unity'`
PACKAGE_SRC=`node -pe 'require("./package.json").src'`
"$UNITY_EDITOR" -quit -batchmode -projectPath "`pwd`" -exportpackage "$PACKAGE_SRC" "$PACKAGE_NAME.unitypackage"


# commit files
git add CHANGELOG.md -f
git add package.json -f
git commit -m "update change log"


# merge and push
git checkout master
git merge --no-ff release -m "release $TAG"
git branch -D release
git push origin master
git checkout develop
git merge --ff master
git push origin develop


# upload unitypackage and release on Github
gh-release  --draft --assets "$PACKAGE_NAME.unitypackage"


echo "\n\n$PACKAGE_NAME v$RELEASE_VERSION has been successfully released!\n"
