(DEVELOP BRANCH)
===

**NOTE: This branch is for development purposes only.**  
**NOTE: To use a released package, see [Releases page](/../../releases) or [default branch](/../..).**

<br><br><br>

## How to contribute this repository

See [CONTRIBUTING.md](/../../blob/develop/CONTRIBUTING.md) and [CODE_OF_CONDUCT.md](/../../blob/develop/CODE_OF_CONDUCT.md).

<br><br><br>

## How to develop

1. Fork this repository.
1. Clone the forked repository to local.
1. Create your branch from `develop` branch.
1. Develop the package.
1. Commit with a message based on [Conventional Commits](https://www.conventionalcommits.org/).
1. Fill out the description, link any related issues and submit your pull request.  
   **NOTE: Create a pull request to merge into `develop` branch**

### Committed messages in the most common cases

| Case | Commit message|
| -- | -- |
| Added a new feature | feat: add new feature |
| Added a suggested feature #999 | feat: add new feature<br>Close #999 |
| Fixed a bug | fix: a problem |
| Fixed a reported bug #999 | fix: a problem<br>Close #999 |
| Added features that include breaking changes | feat: add new feature<br><br>BREAKING CHANGE: Details of the changes |

<br><br><br>

## How to release

**NOTE: The contributor does not need to perform a release operation.**

When you push to `preview`, `master` or `v1.x` branch, this package is automatically released by GitHub Action.  
Internally, a npm tool [semantic-release](https://semantic-release.gitbook.io/semantic-release/) is used to release.

* Update version in `package.json` 
* Update CHANGELOG.md
* Commit documents and push
* Update and tag upm branch
* Release on GitHub
* ~~Publish npm registory~~

Alternatively, you can release it manually with the following command:

```bash
$ npm run release -- --no-ci
```
