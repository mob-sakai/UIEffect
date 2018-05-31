#!/bin/bash -e

# NOTE: Set an environment variable `CHANGELOG_GITHUB_TOKEN` by running the following command at the prompt, or by adding it to your shell profile (e.g., ~/.bash_profile or ~/.zshrc):
#   export CHANGELOG_GITHUB_TOKEN="«your-40-digit-github-token»"

# Release the project with the following steps:
#   1. Update the release version in package.json.
#   2. Open unity project to check compile error.
#   3. Update "CHANGELOG.md" using "github_changelog_generator-1.15.0.pre.rc".
#   4. Check changelog and continue proccess.
#   5. Export unitypackage.
#   6. Commit package.json and CHANGELOG.md.
#   7. Merge into master branch.
#   8. Release using "gh-release-3.2.0". (with upload unitypackage)


# input release version
echo -e ">> Start Github Release:"
PACKAGE_NAME=`node -pe 'require("./package.json").name'`
echo -e ">> Package name: ${PACKAGE_NAME}"
CURRENT_VERSION=`grep -o -e "\"version\".*$" package.json | sed -e "s/\"version\": \"\(.*\)\".*$/\1/"`
read -p "[? release version (for current: ${CURRENT_VERSION}): " RELEASE_VERSION
[ -z "${RELEASE_VERSION}" ] && exit


# update version in package.json
echo -e "\n>> Update version... package.json"
git checkout -B release develop
sed -i -e "s/\"version\": \(.*\)/\"version\": \"${RELEASE_VERSION}\",/g" package.json


# check unity editor
UNITY_VER=`sed -e "s/m_EditorVersion: \(.*\)/\1/g" ProjectSettings/ProjectVersion.txt`
UNITY_EDITOR="/Applications/Unity/Hub/Editor/${UNITY_VER}/Unity.app/Contents/MacOS/Unity"
echo -e "\n>> Check unity editor... ${UNITY_VER} (${UNITY_EDITOR})"
"$UNITY_EDITOR" -quit -batchmode -projectPath "`pwd`"
echo -e ">> OK"

# generate change log
CHANGELOG_GENERATOR_ARG=`grep -o -e ".*git\"$" package.json | sed -e "s/^.*\/\([^\/]*\)\/\([^\/]*\).git.*$/--user \1 --project \2/"`
CHANGELOG_GENERATOR_ARG="--future-release v${RELEASE_VERSION} ${CHANGELOG_GENERATOR_ARG}"
echo -e "\n>> Generate change log... ${CHANGELOG_GENERATOR_ARG}"
github_changelog_generator ${CHANGELOG_GENERATOR_ARG}

git diff -- CHANGELOG.md
read -p "[? is the change log correct? (y/N):" yn
case "$yn" in [yY]*) ;; *) exit ;; esac


# export unitypackage
PACKAGE_SRC=`node -pe 'require("./package.json").src'`
echo -e "\n>> Export unitypackage... ${PACKAGE_SRC}"
"$UNITY_EDITOR" -quit -batchmode -projectPath "`pwd`" -exportpackage "$PACKAGE_SRC" "$PACKAGE_NAME.unitypackage"
echo -e ">> OK"

# commit release files
echo -e "\n>> Commit release files..."
git add CHANGELOG.md -f
git add package.json -f
git commit -m "update change log"


# merge and push
echo -e "\n>> Merge and push..."
git checkout master
git merge --no-ff release -m "release $TAG"
git branch -D release
git push origin master
git checkout develop
git merge --ff master
git push origin develop


# upload unitypackage and release on Github
gh-release  --assets "$PACKAGE_NAME.unitypackage"


echo -e "\n\n>> $PACKAGE_NAME v$RELEASE_VERSION has been successfully released!\n"
