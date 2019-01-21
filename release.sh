#!/bin/bash -e

# NOTE: Set an environment variable `CHANGELOG_GITHUB_TOKEN` by running the following command at the prompt, or by adding it to your shell profile (e.g., ~/.bash_profile or ~/.zshrc):
#   export CHANGELOG_GITHUB_TOKEN="«your-40-digit-github-token»"
bash <(curl -sL 'https://gist.github.com/mob-sakai/e281baa04e1a47148b62387f9c7967df/raw/unity_release.sh')