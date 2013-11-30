# QED CI

_A minimal continuous integration server for repositories hosted on GitHub._

:warning: 
QED is just getting started and is only barely useful. It's still needs lots of features and is certainly full of bugs. 
:warning:

QED accepts [post-receive hooks](https://help.github.com/articles/post-receive-hooks) from a GitHub repository and then clones and builds that repository. When finished, it uses the GitHub API to update the commit status.

## Server Installation

**Requirements**

- .NET 4.5
- `git.exe` in the `%PATH%`

**Steps**

1. Clone the source code.
1. Run `msbuild.exe` in the repository's root directory.
1. [Modify the sample build configuration](#build-configuration) in the repository's root directory.
1. [Configure the GitHub repository](#github-repository-configuration).
1. Run `./bin/Debug/qed.exe` from the repository's root directory.
1. Open a browser to `http://localhost:1754`.

## Build Configuration

QED requires a JSON file named `build.config` in its working directory. The format of this file is:

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

Add another [post-receive hook](https://help.github.com/articles/post-receive-hooks) with the event type `pull_request` and a URL of `http://your-host/events/pull-request`. Note that you'll have to [do this via the API](http://blog.half-ogre.com/posts/software/create-pr-hook-for-qed/), as you can't change the event type though github.com. (Eventually, qed will do this for you.)

## Testing Post-Receive Hooks

If you are adding a new feature or fixing a QED bug, you might need to fake a post-receive hook to start a build locally. Here's a PowerShell snippet to fake a push event:

```
$pushPayload = 'payload='+[System.Uri]::EscapeDataString('{
  "ref": "refs/heads/{branch}",
  "after": "{sha}",
  "repository": {
    "name": "{name}",
    "url": "https://github.com/{owner}/{name}",
    "owner": {
      "name": "{owner}"
    }
  }
}')

Invoke-WebRequest -Headers @{"X-GitHub-Event"="push"} -Method Post -Body $pushPayload http://localhost:1754/events/push
```

And to fake a `pull_request` event:

```
$prPayload = 'payload='+[System.Uri]::EscapeDataString('{
  "action": "{state}",
  "number": {pr-number},
  "pull_request": {
    "head": {
      "ref": "{pr-branch-name}",
      "sha": "{pr-sha}"
    }
  },
  "repository": { 
    "name": "{repository-name}", 
    "owner": {
      "login": "{repository-owner}"
    },
    "html_url": "https://github.com/{repository-owner}/{repository-name}"
  }
}')

Invoke-WebRequest -Headers @{"X-GitHub-Event"="pull_request"} -Method Post -Body $prPayload http://localhost:1754/events/pull-request
```
