# qed

_A minimal build daemon and web-based build manager for repositories hosted on GitHub._

_qed_ accepts [post-receive hooks](https://help.github.com/articles/post-receive-hooks) from a GitHub repository and then clones and builds that repository. When finished, it uses the GitHub API to update the commit status.

## Server Installation

**Requirements**

- .NET 4.5
- `git.exe` in the `%PATH%`

**Steps**

1. Download the latest release .zip file. 
1. Extract the archive.
1. [Create a build configuration](#build-configuration) in the extracted directory.
1. [Configure the GitHub repository](#github-repository-configuration).
1. Run `qed.exe`.
1. Open a browser to `http://localhost:1754`.

## Build Configuration

_qed_ requires a JSON file named `build_config.json` in its working directory. The format of this file is:

```
[
  {  "owner": "the-repository-owner",
     "name": "the-repository-name",
     "token": "an-oauth-token-with-repo-scope",
     "command": "the-command-to-run-e.g.-powershell.exe",
     "commandArguments": "the-command-arguments-e.g.-cibuild.ps1"
  },
  // additional build configurations
]
```

## GitHub Repository Configuration

Add a [post-receive hook](https://help.github.com/articles/post-receive-hooks) with the event type `push` (the default) and a URL of `http://your-host/events/push`.
